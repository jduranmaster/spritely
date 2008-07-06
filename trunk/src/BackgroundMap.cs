using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Text;

namespace Spritely
{
	public class BackgroundMap
	{
		private Document m_doc;
		private MainForm m_Owner;
		private SpriteList m_sl;

		// Tiles to highlight under the cursor in the Background Map.
		private int m_tileSpriteX;
		private int m_tileSpriteY;

		private static Pen m_penHilight = new Pen(Color.FromArgb(128, Color.Red), 3);
		private static Pen m_penHilight2 = new Pen(Color.FromArgb(128, Color.Black), 1);

		// We don't assign tile ids until export time, so we need to keep track of each
		// tile in the background tile map by recording the sprite and the tile index
		// into that sprite.
		struct BackgroundMapTileInfo
		{
			public Sprite sprite;
			public int nTileIndex;
			public int nPalette;

			public BackgroundMapTileInfo(Sprite s, int nIndex, int palette)
			{
				sprite = s;
				nTileIndex = nIndex;
				nPalette = palette;
			}
		};

		private const int kMaxMapTilesX = 32;
		private const int kMaxMapTilesY = 32;
		private const int kGBAScreenTilesX = 30;
		private const int kGBAScreenTilesY = 20;
		private const int kNDSScreenTilesX = 32;
		private const int kNDSScreenTilesY = 24;

		/// <summary>
		/// This 1x1 sprite is used as the default tile for the background map.
		/// </summary>
		private Sprite m_spriteDefaultMapSprite = null;

		/// <summary>
		/// The map of background tiles.
		/// </summary>
		private BackgroundMapTileInfo[,] m_BackgroundMap = null;

		public BackgroundMap(Document doc, SpriteList sl)
		{
			m_doc = doc;
			m_Owner = doc.Owner;
			m_sl = sl;
			m_sl.Map = this;

			m_BackgroundMap = new BackgroundMapTileInfo[kMaxMapTilesX, kMaxMapTilesY];
			for (int ix = 0; ix < kMaxMapTilesX; ix++)
				for (int iy = 0; iy < kMaxMapTilesY; iy++)
				{
					m_BackgroundMap[ix, iy].sprite = null;
					m_BackgroundMap[ix, iy].nPalette = 0;
				}

			m_tileSpriteX = -1;
			m_tileSpriteY = -1;
		}

		public MainForm Owner
		{
			get { return m_Owner; }
		}

		public Sprite DefaultMapSprite
		{
			get { return m_spriteDefaultMapSprite; }
			set { m_spriteDefaultMapSprite = value; }
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;
			m_Owner = doc.Owner;
		}

		// Fill any empty map spaces with this sprite.
		public void FillEmptyMapTiles(Sprite s)
		{
			if (m_spriteDefaultMapSprite == null && s.IsSize(1, 1))
			{
				m_spriteDefaultMapSprite = s;
				for (int ix = 0; ix < kMaxMapTilesX; ix++)
				{
					for (int iy = 0; iy < kMaxMapTilesY; iy++)
					{
						if (m_BackgroundMap[ix, iy].sprite == null)
						{
							m_BackgroundMap[ix, iy].sprite = m_spriteDefaultMapSprite;
							m_BackgroundMap[ix, iy].nTileIndex = 0;
						}
					}
				}
			}
		}

		/// <summary>
		/// Replace all occurences of this sprite in the background map with the
		/// default sprite.
		/// </summary>
		/// <param name="s"></param>
		public void RemoveSpriteFromMap(Sprite sToRemove)
		{
			for (int ix = 0; ix < kMaxMapTilesX; ix++)
			{
				for (int iy = 0; iy < kMaxMapTilesY; iy++)
				{
					if (m_BackgroundMap[ix, iy].sprite == sToRemove)
					{
						m_BackgroundMap[ix, iy].sprite = m_spriteDefaultMapSprite;
						m_BackgroundMap[ix, iy].nTileIndex = 0;
					}
				}
			}
		}

		public bool SetBackgroundTile(int x, int y, int nTileID, int nPaletteID)
		{
			Sprite s = m_sl.FindSprite(nTileID);
			if (s == null)
				return false;

			m_BackgroundMap[x, y].sprite = s;
			m_BackgroundMap[x, y].nTileIndex = nTileID - s.FirstTileID;
			m_BackgroundMap[x, y].nPalette = nPaletteID;
			return true;
		}

		public bool HandleMouse_EditMap(int pxX, int pxY)
		{
			Sprite spriteSelected = m_sl.CurrentSprite;
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
					m_BackgroundMap[x + ix, y + iy].sprite = spriteSelected;
					m_BackgroundMap[x + ix, y + iy].nTileIndex = nIndex;
					//m_BackgroundMap[x + ix, y + iy].nPalette = ;
					nIndex++;
					fUpdate = true;
				}
			}

			return fUpdate;
		}

		public bool HandleMouseMove_EditMap(int pxX, int pxY)
		{
			Sprite spriteSelected = m_sl.CurrentSprite;
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
					Sprite s = m_BackgroundMap[ix, iy].sprite;
					if (s != null)
					{
						Tile t = s.GetTile(m_BackgroundMap[ix, iy].nTileIndex);
						t.DrawSmallTile(g, pxX, pxY);
					}
					else
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
			Sprite spriteSelected = m_sl.CurrentSprite;
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

		public void Save(System.IO.TextWriter tw)
		{
			tw.WriteLine("\t<map>");
			for (int iy = 0; iy < kMaxMapTilesY; iy++)
			{
				tw.Write("\t\t<row");
				tw.Write(String.Format(" y=\"{0}\"", iy));
				tw.WriteLine(">");
				for (int ix = 0; ix < kMaxMapTilesX; ix++)
				{
					Sprite s = m_BackgroundMap[ix, iy].sprite;
					//TODO: ensure that s is never null, don't allow last 1x1 tile to be deleted
					int nFirstTile = 0;
					if (s != null)
						nFirstTile = s.FirstTileID;
					int nTile = m_BackgroundMap[ix, iy].nTileIndex;
					tw.Write("\t\t\t<bgtile");
					tw.Write(String.Format(" x=\"{0}\"", ix));
					tw.Write(String.Format(" tileid=\"{0}\"", nFirstTile + nTile));
					//TODO: tw.Write(String.Format(" palette=\"{0}\"", s.PaletteID));
					tw.WriteLine(" />");
				}
				tw.WriteLine("\t\t</row>");
			}
			tw.WriteLine("\t</map>");
		}

		public void ExportGBA_BackgroundTileMap(System.IO.TextWriter tw)
		{
			for (int iy = 0; iy < kMaxMapTilesY; iy++)
			{
				StringBuilder sb = new StringBuilder();
				for (int ix = 0; ix < kMaxMapTilesX; ix++)
				{
					Sprite s = m_BackgroundMap[ix, iy].sprite;
					int nFirstTile = 0;
					int nPaletteID = 0;
					if (s != null)
					{
						nFirstTile = s.FirstTileID;
						nPaletteID = s.PaletteID;
					}
					int nTile = m_BackgroundMap[ix, iy].nTileIndex;
					//TODO: load palette from background instead of inheriting directly from sprite
					//TODO: add suport for h/v flipped tiles
					int nMap = (nPaletteID << 12) | (nFirstTile + nTile);
					sb.Append(String.Format("0x{0:x4},", nMap));
				}
				tw.WriteLine("\t{0}", sb.ToString());
			}
		}

	}
}
