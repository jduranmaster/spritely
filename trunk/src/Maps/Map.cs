using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public partial class Map
	{
		private Document m_doc;
		private Spriteset m_bgtiles;

		private string m_strName;
		private int m_id;
		private string m_strDesc;

		// Tiles to highlight under the cursor in the Background Map.
		private int m_tileSpriteX;
		private int m_tileSpriteY;

		private static Pen m_penHilight = new Pen(Color.FromArgb(128, Color.Red), 3);
		private static Pen m_penHilight2 = new Pen(Color.FromArgb(128, Color.Black), 1);

		//private List<MapBlock> m_mapblocks;

		// We don't assign the real tile ids until export time, so we need to keep track of each
		// tile in the background tile map by recording the sprite and the tile index
		// into that sprite.
		struct BackgroundMapTileInfo
		{
			public int nTileIndex;
			public int nSubpalette;
			public bool fHFlip;
			public bool fVFlip;

			public BackgroundMapTileInfo(int nIndex, int subpalette)
			{
				nTileIndex = nIndex;
				nSubpalette = subpalette;
				fHFlip = false;
				fVFlip = false;
			}
		};

		private const int kMaxMapTilesX = 32;
		private const int kMaxMapTilesY = 32;
		private const int kGBAScreenTilesX = 30;
		private const int kGBAScreenTilesY = 20;
		private const int kNDSScreenTilesX = 32;
		private const int kNDSScreenTilesY = 24;

		/// <summary>
		/// The map of background tiles.
		/// </summary>
		private BackgroundMapTileInfo[,] m_BackgroundMap = null;

		public Map(Document doc, string strName, int id, string strDesc, Spriteset bgtiles)
		{
			m_doc = doc;
			m_bgtiles = bgtiles;
			m_bgtiles.AddMap(this);

			m_strName = strName;
			m_id = id;
			m_strDesc = strDesc;

			m_BackgroundMap = new BackgroundMapTileInfo[kMaxMapTilesX, kMaxMapTilesY];

			int nDefaultTile = -1;
			if (bgtiles.CurrentSprite != null)
				nDefaultTile = bgtiles.CurrentSprite.FirstTileID;
			for (int ix = 0; ix < kMaxMapTilesX; ix++)
				for (int iy = 0; iy < kMaxMapTilesY; iy++)
				{
					m_BackgroundMap[ix, iy].nTileIndex = nDefaultTile;
					m_BackgroundMap[ix, iy].nSubpalette = 0;
				}

			m_tileSpriteX = -1;
			m_tileSpriteY = -1;
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;
		}

		public string Name
		{
			get { return m_strName; }
		}

		/// <summary>
		/// Replace all occurences of this sprite in the background map with the
		/// default sprite.
		/// </summary>
		/// <param name="s"></param>
		public void RemoveSpriteTilesFromMap(Sprite sToRemove)
		{
			for (int ix = 0; ix < kMaxMapTilesX; ix++)
			{
				for (int iy = 0; iy < kMaxMapTilesY; iy++)
				{
					int nTile = m_BackgroundMap[ix, iy].nTileIndex;
					int nSpriteTile1 = sToRemove.FirstTileID;
					int nSpriteTileN = sToRemove.FirstTileID + sToRemove.NumTiles-1;
					if (nTile >= nSpriteTile1 && nTile <= nSpriteTileN)
						m_BackgroundMap[ix, iy].nTileIndex = -1;
				}
			}
		}

		public bool SetBackgroundTile(int x, int y, int nTileID, int nSubpaletteID)
		{
			if (x < 0 || x >= kMaxMapTilesX || y < 0 || y >= kMaxMapTilesY)
				return false;
			m_BackgroundMap[x, y].nTileIndex = nTileID;
			m_BackgroundMap[x, y].nSubpalette = nSubpaletteID;
			return true;
		}

		public bool GetBackgroundTile(int x, int y, out int nTileID, out int nSubpaletteID)
		{
			if (x < 0 || x >= kMaxMapTilesX || y < 0 || y >= kMaxMapTilesY)
			{
				nTileID = 0;
				nSubpaletteID = 0;
				return false;
			}
			nTileID = m_BackgroundMap[x, y].nTileIndex;
			nSubpaletteID = m_BackgroundMap[x, y].nSubpalette;
			return true;
		}

		public bool GetFlip(int x, int y, out bool fHorizontal, out bool fVertical)
		{
			if (x < 0 || x >= kMaxMapTilesX || y < 0 || y >= kMaxMapTilesY)
			{
				fHorizontal = false;
				fVertical = false;
				return false;
			}
			fHorizontal = m_BackgroundMap[x, y].fHFlip;
			fVertical = m_BackgroundMap[x, y].fVFlip;
			return true;
		}

		public bool HandleMouse_EditMap(int pxX, int pxY)
		{
			Sprite spriteSelected = m_bgtiles.CurrentSprite;
			if (spriteSelected == null)
				return false;

			if (pxX < 0 || pxY < 0)
				return false;

			// Convert screen pixel (x,y) to map coordinate (x,y).
			int x = pxX / Tile.SmallBitmapScreenSize;
			int y = pxY / Tile.SmallBitmapScreenSize;

			if (x >= kMaxMapTilesX || y >= kMaxMapTilesY)
				return false;

			bool fUpdate = false;
			int nIndex = 0;
			for (int iy = 0; iy < spriteSelected.TileHeight; iy++)
			{
				if (y + iy >= kMaxMapTilesY)
				{
					nIndex++;
					continue;
				}
				for (int ix = 0; ix < spriteSelected.TileWidth; ix++)
				{
					if (x + ix >= kMaxMapTilesX)
					{
						nIndex++;
						continue;
					}
					m_BackgroundMap[x + ix, y + iy].nTileIndex = spriteSelected.FirstTileID + nIndex;
					// TODO: choose correct palette
					m_BackgroundMap[x + ix, y + iy].nSubpalette = 0;
					nIndex++;
					fUpdate = true;
				}
			}

			return fUpdate;
		}

		public bool HandleMouseMove_EditMap(int pxX, int pxY)
		{
			Sprite spriteSelected = m_bgtiles.CurrentSprite;
			if (spriteSelected == null)
				return false;

			// Convert screen pixel (x,y) to map coordinate (x,y).
			int x = pxX / Tile.SmallBitmapScreenSize;
			int y = pxY / Tile.SmallBitmapScreenSize;

			if (pxX < 0 || pxY < 0 || x >= kMaxMapTilesX || y >= kMaxMapTilesY)
			{
				// Turn off the hilight if we currently have one.
				if (m_tileSpriteX != -1 || m_tileSpriteY != -1)
				{
					m_tileSpriteX = -1;
					m_tileSpriteY = -1;
					return true;
				}
				return false;
			}

			// Has the current mouse position changed?
			if (x != m_tileSpriteX || y != m_tileSpriteY)
			{
				m_tileSpriteX = x;
				m_tileSpriteY = y;
				return true;
			}

			// No change.
			return false;
		}

		public void DrawBackgroundMap(Graphics g)
		{
			int pxX = 0;
			int pxY = 0;
			for (int ix = 0; ix < kMaxMapTilesX; ix++)
			{
				pxY = 0;
				for (int iy = 0; iy < kMaxMapTilesY; iy++)
				{
					bool fDrawn = false;
					int nTileIndex = m_BackgroundMap[ix, iy].nTileIndex;
					Sprite s = m_bgtiles.FindSprite(nTileIndex);
					if (s != null)
					{
						Tile t = s.GetTile(nTileIndex - s.FirstTileID);
						if (t != null)
						{
							t.DrawSmallTile(g, pxX, pxY);
							fDrawn = true;
						}
					}

					if (!fDrawn)
					{
						int pxInset = Tile.SmallBitmapScreenSize / 4;
						int pxX0i = pxX + pxInset;
						int pxY0i = pxY + pxInset;
						int pxX1i = pxX + Tile.SmallBitmapScreenSize - pxInset;
						int pxY1i = pxY + Tile.SmallBitmapScreenSize - pxInset;
						g.DrawLine(Pens.Firebrick, pxX0i, pxY0i, pxX1i, pxY1i);
						g.DrawLine(Pens.Firebrick, pxX0i, pxY1i, pxX1i, pxY0i);
					}
					pxY += Tile.SmallBitmapScreenSize;
				}
				pxX += Tile.SmallBitmapScreenSize;
			}

			// Draw the grid and border.
			int pxTileSize = Tile.SmallBitmapScreenSize;
			pxX = 0;
			pxY = 0;
			int pxWidth = pxTileSize * kMaxMapTilesX;
			int pxHeight = pxTileSize * kMaxMapTilesY;

			if (Options.BackgroundMap_ShowGrid)
			{
				Pen penTileBorder = Pens.LightGray;

				// Draw a border around each tile.
				for (int i = pxX + pxTileSize; i < pxWidth; i += pxTileSize)
					g.DrawLine(penTileBorder, i, pxY, i, pxHeight);
				for (int i = pxY + pxTileSize; i < pxHeight; i += pxTileSize)
					g.DrawLine(penTileBorder, pxX, i, pxWidth, i);
			}

			// Draw the outer border.
			g.DrawRectangle(Pens.Black, pxX, pxY, pxWidth, pxHeight);

			if (Options.BackgroundMap_ShowScreen)
			{
				if (Options.Platform == Options.PlatformType.GBA)
				{
					pxWidth = pxTileSize * kGBAScreenTilesX;
					pxHeight = pxTileSize * kGBAScreenTilesY;
				}
				else
				{
					pxWidth = pxTileSize * kNDSScreenTilesX;
					pxHeight = pxTileSize * kNDSScreenTilesY;
				}
				g.DrawRectangle(m_penHilight, pxX, pxY, pxWidth, pxHeight);
				g.DrawRectangle(m_penHilight2, pxX, pxY, pxWidth, pxHeight);
			}

			// Draw a border around the current background "sprite".
			Sprite spriteSelected = m_bgtiles.CurrentSprite;
			if (m_tileSpriteX != -1 && m_tileSpriteY != -1 && spriteSelected != null)
			{
				pxX = m_tileSpriteX * pxTileSize;
				pxY = m_tileSpriteY * pxTileSize;
				pxWidth = spriteSelected.TileWidth* pxTileSize;
				pxHeight = spriteSelected.TileHeight* pxTileSize;
				g.DrawRectangle(m_penHilight, pxX, pxY, pxWidth, pxHeight);
				g.DrawRectangle(m_penHilight2, pxX, pxY, pxWidth, pxHeight);
			}
		}

	}
}
 