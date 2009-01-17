using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Spritely
{
	public class Spriteset
	{
		private Document m_doc;
		private string m_strName;
		private int m_id;
		private string m_strDesc;
		private Palette m_palette;

		private SpriteList m_sl;

		private SpritesetForm m_winSpriteset;

		// List of maps that are based on this spritelist.
		private List<Map> m_Maps;

		public Spriteset(Document doc, string strName, int id, string strDesc, Palette pal)
		{
			m_doc = doc;
			m_strName = strName;
			m_id = id;
			m_strDesc = strDesc;
			m_palette = pal;

			m_winSpriteset = null;
			m_sl = new SpriteList(doc, this, pal, pal.IsBackground);
			m_Maps = new List<Map>();
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;
			m_sl.UpdateDocument(doc);
		}

		public SpritesetForm SpritesetWindow
		{
			get
			{
				if (m_winSpriteset == null)
					m_winSpriteset = new SpritesetForm(m_doc.Owner.NewUI, this);
				return m_winSpriteset;
			}
		}

		// TODO: remove the SpriteList class and this accessor
		public SpriteList SpriteList
		{
			get { return m_sl; }
		}

		/// <summary>
		/// The name of this spriteset.
		/// </summary>
		public string Name
		{
			get { return m_strName; }
		}

		public int Id
		{
			get { return m_id; }
		}

		/// <summary>
		/// The total number of sprites in this spriteset.
		/// </summary>
		public int NumSprites
		{
			get { return m_sl.NumSprites; }
		}

		/// <summary>
		/// The total number of tiles in this spriteset.
		/// </summary>
		public int NumTiles
		{
			get { return m_sl.NumTiles; }
		}

		private int m_nNextTileId = 0;

		public int NextTileId
		{
			get { return m_nNextTileId; }
			set { m_nNextTileId = value; }
		}

		private int m_nNextSpriteId = 0;

		public int NextSpriteId
		{
			get { return m_nNextSpriteId; }
			set { m_nNextSpriteId = value; }
		}

		public string GenerateUniqueSpriteName()
		{
			// When we're auto-generating sprite names, make sure the new name doesn't collide with
			// the names of any already-existing sprites.
			string strName = AutoGenerateSpriteName();
			while (HasNamedSprite(strName))
				strName = AutoGenerateSpriteName();
			return strName;
		}

		public string AutoGenerateSpriteName()
		{
			return String.Format("S{0}", NextSpriteId++);
		}

		public void RemoveSelectedSprite()
		{
			Sprite sToRemove = m_sl.CurrentSprite;
			m_sl.RemoveSprite(sToRemove, true);

			foreach (Map m in m_Maps)
			{
				m.RemoveSpriteTilesFromMap(sToRemove);
			}
		}

		public Sprite AddSprite(int nWidth, int nHeight, string strName, int id, string strDesc, bool fAddUndo)
		{
			return m_sl.AddSprite(nWidth, nHeight, strName, id, strDesc, 0, fAddUndo);
		}

		public Sprite AddSprite(int nWidth, int nHeight, string strName, int id, string strDesc, int nSubpalette, bool fAddUndo)
		{
			if (id == -1)
				id = NextTileId++;
			Sprite s = m_sl.AddSprite(nWidth, nHeight, strName, id, strDesc, nSubpalette, fAddUndo);
			return s;
		}

		/// <summary>
		/// Find the sprite that owns this tile.
		/// </summary>
		/// <param name="nTileID">Tile id</param>
		/// <returns>The sprite that owns this tile</returns>
		public Sprite FindSprite(int nTileID)
		{
			return m_sl.FindSprite(nTileID);
		}

		public void SelectFirstSprite()
		{
			m_sl.SelectFirstSprite();
		}

		/// <summary>
		/// Flush the bitmaps for all sprites so that they get regenerated.
		/// </summary>
		public void FlushBitmaps()
		{
			m_sl.FlushBitmaps();
		}

		/// <summary>
		/// The currently selected sprite in the spriteset.
		/// </summary>
		public Sprite CurrentSprite
		{
			get { return m_sl.CurrentSprite; }
			set { m_sl.CurrentSprite = value; }
		}

		/// <summary>
		/// Does the spriteset contain a sprite with the given name?
		/// </summary>
		/// <param name="strName">The sprite name to check</param>
		/// <returns>True if the sprite name exists in the spriteset</returns>
		public bool HasNamedSprite(string strName)
		{
			return m_sl.HasNamedSprite(strName);
		}

		/// <summary>
		/// Add a map to the list of maps that are based on this SpriteList.
		/// Only used for background SpriteLists.
		/// </summary>
		/// <param name="m">The map</param>
		public void AddMap(Map m)
		{
			m_Maps.Add(m);
		}

		public bool LoadXML_spriteset16(XmlNode xnode)
		{
			foreach (XmlNode xn in xnode.ChildNodes)
			{
				if (xn.Name == "sprite16")
				{
					string strName = XMLUtils.GetXMLAttribute(xn, "name");
					int id = XMLUtils.GetXMLIntegerAttribute(xn, "id");
					string strDesc = XMLUtils.GetXMLAttribute(xn, "desc");
					string strSize = XMLUtils.GetXMLAttribute(xn, "size");
					int nSubpaletteId = XMLUtils.GetXMLIntegerAttribute(xn, "subpalette_id");

					string[] aSize = strSize.Split('x');
					int nWidth = XMLUtils.ParseInteger(aSize[0]);
					int nHeight = XMLUtils.ParseInteger(aSize[1]);

					Sprite s = AddSprite(nWidth, nHeight, strName, NextSpriteId++, strDesc, nSubpaletteId, true);
					if (!s.LoadXML_sprite16(xn))
						return false;
				}
			}
			return true;
		}

		public void Save(System.IO.TextWriter tw, bool fOldFormat)
		{
			m_sl.Save(tw, fOldFormat, false);
		}

		private int m_nExportId;

		public void Export_AssignIDs(int nSpritesetExportId)
		{
			m_nExportId = nSpritesetExportId;
			m_sl.Export_AssignIDs();
		}

		public void Export_SpritesetInfo(System.IO.TextWriter tw)
		{
			tw.WriteLine(String.Format("\t{{{0,4},{1,4},{2,4},{3,4} }}, // Spriteset #{4} : {5}",
				0, NumSprites, NumTiles, m_palette.ExportId, m_nExportId, m_strName));
		}

		public void Export_SpritesetIDs(System.IO.TextWriter tw)
		{
			tw.WriteLine(String.Format("const int kSpriteset_{0} = {1};", m_strName, m_nExportId));
		}

		public void Export_BgTilesetInfo(System.IO.TextWriter tw)
		{
			tw.WriteLine(String.Format("\t{{{0,4},{1,4},{2,4} }}, // BgTileset #{3} : {4}",
				0, NumTiles, m_palette.ExportId, m_nExportId, m_strName));
		}

		public void Export_BgTilesetIDs(System.IO.TextWriter tw)
		{
			tw.WriteLine(String.Format("const int kBgTileset_{0} = {1};", m_strName, m_nExportId));
		}

		public void Export_SpriteInfo(System.IO.TextWriter tw)
		{
			m_sl.Export_SpriteInfo(tw);
		}

		public void Export_SpriteIDs(System.IO.TextWriter tw)
		{
			m_sl.Export_SpriteIDs(tw, m_strName);
		}

		public void Export_TileIDs(System.IO.TextWriter tw)
		{
			m_sl.Export_TileIDs(tw, m_strName);
		}

		public void Export_TileData(System.IO.TextWriter tw)
		{
			m_sl.Export_TileData(tw);
		}

		public void Export_SpriteMaskData(System.IO.TextWriter tw)
		{
			m_sl.Export_SpriteMaskData(tw);
		}

	}
}
