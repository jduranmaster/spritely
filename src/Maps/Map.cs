using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public partial class Map
	{
		private Document m_doc;
		private Spriteset m_ss;

		private string m_strName;
		private int m_id;
		private string m_strDesc;

		private MapForm m_winMap;

		//private List<MapBlock> m_mapblocks;

		// We don't assign the real tile ids until export time, so we need to keep track of each
		// tile in the background map by recording the sprite and the tile index
		// into that sprite.
		struct BackgroundMapTileInfo
		{
			public int nTileIndex;
			public int nSubpalette;
			public bool fHFlip;
			public bool fVFlip;

			public BackgroundMapTileInfo(Sprite s, int nIndex, int subpalette)
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
			m_ss = bgtiles;
			m_ss.AddMap(this);

			m_strName = strName;
			m_id = id;
			m_strDesc = strDesc;

			m_BackgroundMap = new BackgroundMapTileInfo[kMaxMapTilesX, kMaxMapTilesY];

			int nDefaultTile = -1;
			if (bgtiles.CurrentSprite != null)
				nDefaultTile = bgtiles.CurrentSprite.FirstTileId;
			for (int ix = 0; ix < kMaxMapTilesX; ix++)
				for (int iy = 0; iy < kMaxMapTilesY; iy++)
				{
					m_BackgroundMap[ix, iy].nTileIndex = nDefaultTile;
					m_BackgroundMap[ix, iy].nSubpalette = 0;
				}

			if (m_doc.Owner != null)
			{
				m_winMap = new MapForm(m_doc.Owner, this, bgtiles, null); ;
			}
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;
		}

		public MapForm MapWindow
		{
			get { return m_winMap; }
		}

		public string Name
		{
			get { return m_strName; }
		}

		public Spriteset Spriteset
		{
			get { return m_ss; }
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
					int nSpriteTile1 = sToRemove.FirstTileId;
					int nSpriteTileN = sToRemove.FirstTileId + sToRemove.NumTiles-1;
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


	}
}
 