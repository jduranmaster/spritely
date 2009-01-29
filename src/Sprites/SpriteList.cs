using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Text;

namespace Spritely
{
	public class SpriteList
	{
		private Document m_doc;
		private Spriteset m_ss;

		private int m_nSprites;
		private int m_nTiles;

		public SpriteType[] SpriteTypes = new SpriteType[]
		{
			new SpriteType("1x1",		1,1,	Sprite.GBASize.Size8,		Sprite.GBAShape.Square),
			new SpriteType("1x2",		1,2,	Sprite.GBASize.Size8,		Sprite.GBAShape.Tall),
			new SpriteType("1x4",		1,4,	Sprite.GBASize.Size16,		Sprite.GBAShape.Tall),
			new SpriteType("2x1",		2,1,	Sprite.GBASize.Size8,		Sprite.GBAShape.Wide),
			new SpriteType("2x2",		2,2,	Sprite.GBASize.Size16,		Sprite.GBAShape.Square),
			new SpriteType("2x4",		2,4,	Sprite.GBASize.Size32,		Sprite.GBAShape.Tall),
			new SpriteType("4x1",		4,1,	Sprite.GBASize.Size16,		Sprite.GBAShape.Wide),
			new SpriteType("4x2",		4,2,	Sprite.GBASize.Size32,		Sprite.GBAShape.Wide),
			new SpriteType("4x4",		4,4,	Sprite.GBASize.Size32,		Sprite.GBAShape.Square),
			new SpriteType("4x8",		4,8,	Sprite.GBASize.Size64,		Sprite.GBAShape.Tall),
			new SpriteType("8x4",		8,4,	Sprite.GBASize.Size64,		Sprite.GBAShape.Wide),
			new SpriteType("8x8",		8,8,	Sprite.GBASize.Size64,		Sprite.GBAShape.Square),
		};

		public SpriteList(Document doc, Spriteset ts)
		{
			m_doc = doc;
			m_ss = ts;
		}

		public int NumSprites
		{
			get { return m_nSprites; }
		}

		public int NumTiles
		{
			get { return m_nTiles; }
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;

			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					s.UpdateDocument(doc);
				}
			}
		}

		public Sprite AddSprite(int nWidth, int nHeight, string strName, int id, string strDesc, int nSubpalette, bool fAddUndo)
		{
			return AddSprite_(nWidth, nHeight, strName, id, strDesc, nSubpalette, fAddUndo);
		}

		public Sprite AddSprite(int nWidth, int nHeight, string strName, int id, string strDesc, bool fAddUndo)
		{
			return AddSprite_(nWidth, nHeight, strName, id, strDesc, 0, fAddUndo);
		}

		private Sprite AddSprite_(int nWidth, int nHeight, string strName, int id, string strDesc, int nSubpalette, bool fAddUndo)
		{
			List<Sprite> slist = null;

			// Make sure that the requested size is valid.
			foreach (SpriteType st in SpriteTypes)
			{
				if (st.Width == nWidth && st.Height == nHeight)
					slist = st.Sprites;
			}

			// Invalid sprite size - return.
			if (slist == null)
				return null;

			Sprite s = new Sprite(m_doc, m_ss, nWidth, nHeight, strName, id, strDesc, nSubpalette);
			return AddSprite(s, slist, fAddUndo);
		}

		public Sprite AddSprite(Sprite s, bool fAddUndo)
		{
			List<Sprite> slist = null;

			foreach (SpriteType st in SpriteTypes)
			{
				if (st.Width == s.TileWidth && st.Height == s.TileHeight)
					slist = st.Sprites;
			}

			if (slist == null)
				return null;

			return AddSprite(s, slist, fAddUndo);
		}

		private Sprite AddSprite(Sprite s, List<Sprite> slist, bool fAddUndo)
		{
			slist.Add(s);

			m_nSprites++;
			m_nTiles += (s.TileWidth * s.TileHeight);

			if (fAddUndo)
			{
				UndoMgr undo = m_doc.Undo();
				if (undo != null)
					undo.Push(new UndoAction_AddSprite(undo, m_ss, s, true));
			}

			m_doc.Owner.HandleSpriteTypeChanged(m_ss);
			return s;
		}

		public Sprite DuplicateSprite(Sprite sToCopy)
		{
			if (sToCopy == null)
				return null;

			// Calulate an appropriate name for the copy
			string strNewBaseName;
			int nCopy = 1;
			string strNewName;
			string strCopySuffix = ResourceMgr.GetString("CopySuffix");
			Match m = Regex.Match(sToCopy.Name, String.Format("^(.*){0}([0-9]*)$", strCopySuffix));
			if (m.Success)
			{
				strNewBaseName = String.Format("{0}{1}", m.Groups[1].Value, strCopySuffix);
				if (m.Groups[2].Value != "")
					nCopy = Int32.Parse(m.Groups[2].Value);
				strNewName = String.Format("{0}{1}", strNewBaseName, ++nCopy);
			}
			else
			{
				strNewBaseName = String.Format("{0}{1}", sToCopy.Name, strCopySuffix);
				strNewName = strNewBaseName;
			}

			while (HasNamedSprite(strNewName))
				strNewName = String.Format("{0}{1}", strNewBaseName, ++nCopy);

			Sprite sNew = AddSprite(sToCopy.TileWidth, sToCopy.TileHeight, strNewName, m_ss.NextTileId++, sToCopy.Description, true);
			sNew.Duplicate(sToCopy);
			return sNew;
		}

		public void RemoveSprite(Sprite sToRemove, bool fAddUndo)
		{
			SpriteType stToRemove = null;
			Sprite sPrev = null, sCurr = null,  sNext = null;
			Sprite sNewSelection = null;

			if (sToRemove == null)
				return;

			// Determine which sprite should be selected when this one is removed.
			foreach (SpriteType st in SpriteTypes)
			{
				if (sNewSelection != null)
					break;

				foreach (Sprite s in st.Sprites)
				{
					sPrev = sCurr;
					sCurr = sNext;
					sNext = s;
					if (s == sToRemove)
						stToRemove = st;
					if (sCurr == sToRemove)
					{
						sNewSelection = sNext;
						break;
					}
				}
			}
			// If the last sprite is deleted, select the one before it.
			if (sNext == sToRemove)
				sNewSelection = sCurr;

			int nTiles = sToRemove.NumTiles;
			if (stToRemove == null)
				return;

			if (stToRemove.Sprites.Remove(sToRemove))
			{
				UndoMgr undo = m_doc.Undo();
				m_ss.CurrentSprite = null;
				if (undo != null)
					m_ss.CurrentSprite = undo.FindMostRecentSprite();
				if (m_ss.CurrentSprite == null)
					m_ss.CurrentSprite = sNewSelection;

				m_nSprites--;
				m_nTiles -= nTiles;

				if (fAddUndo && undo != null)
					undo.Push(new UndoAction_AddSprite(undo, m_ss, sToRemove, false));

				m_doc.Owner.HandleSpriteTypeChanged(m_ss);
			}
		}

		// Remove from the old SpriteType and add it to the new one
		public void MoveToCorrectSpriteType(Sprite sprite)
		{
			SpriteType stRemove = null;
			SpriteType stAdd = null;
			foreach (SpriteType st in SpriteTypes)
			{
				if (st.Sprites.Contains(sprite))
					stRemove = st;
				if (st.Width == sprite.TileWidth && st.Height == sprite.TileHeight)
					stAdd = st;
			}
			if (stRemove != stAdd)
			{
				if (stRemove != null)
					stRemove.Sprites.Remove(sprite);
				if (stAdd != null)
					stAdd.Sprites.Add(sprite);
				m_doc.Owner.HandleSpriteTypeChanged(m_ss);
			}
		}

		public bool ResizeSelectedSprite(int tileNewWidth, int tileNewHeight)
		{
			Sprite sToResize = m_ss.CurrentSprite;
			if (sToResize == null)
				return false;

			int nOldTiles = sToResize.NumTiles;

			if (!sToResize.Resize(tileNewWidth, tileNewHeight))
				return false;

			m_nTiles += sToResize.NumTiles - nOldTiles;

			//TODO: adjust the background map

			MoveToCorrectSpriteType(sToResize);
			return true;
		}

		// Return true if successfully rotated.
		public bool RotateSelectedSprite(Sprite.RotateDirection dir)
		{
			Sprite sToRotate = m_ss.CurrentSprite;
			if (sToRotate == null)
				return false;

			int tileNewWidth = sToRotate.TileHeight;
			int tileNewHeight = sToRotate.TileWidth;

			if (!sToRotate.Rotate(dir))
				return false;

			MoveToCorrectSpriteType(sToRotate);
			return true;
		}

		/// <summary>
		/// Check if a sprite with the specified name already exists in this SpriteList.
		/// </summary>
		/// <param name="strName">The name of the sprite to look for.</param>
		/// <returns>True if a sprite with that name exists.</returns>
		public bool HasNamedSprite(string strName)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					if (s.Name == strName)
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Is this sprite the first/last one in the group of SpriteTypes
		/// </summary>
		/// <param name="sprite">The sprite to check</param>
		/// <param name="fIsFirst">Returns true if this is the first sprite</param>
		/// <param name="fIsLast">Returns true if this is the last sprite</param>
		public void IsFirstLastSpriteOfType(Sprite sprite, out bool fIsFirst, out bool fIsLast)
		{
			fIsFirst = false;
			fIsLast = false;

			foreach (SpriteType st in SpriteTypes)
			{
				if (st.Sprites.Contains(sprite))
				{
					bool fFirst = true;
					bool fLast = false;
					foreach (Sprite s in st.Sprites)
					{
						if (s == sprite)
						{
							if (fFirst)
								fIsFirst = true;
							fLast = true;
						}
						else
							fLast = false;
						fFirst = false;
					}
					if (fLast)
						fIsLast = true;
					return;
				}
			}
		}

		// Is this the first sprite in the SpriteList?
		public bool IsFirstSprite(Sprite sBase)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					return s == sBase;
				}
			}
			return false;
		}

		// Is this the last sprite in the SpriteList?
		public bool IsLastSprite(Sprite sBase)
		{
			bool fIsLast = false;
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					fIsLast = (s == sBase);
				}
			}
			return fIsLast;
		}

		// Return the next sprite after the given sprite in the SpriteList.
		// Returns null if the given sprite is the last one, or if the given sprite doesn't exist.
		public Sprite NextSprite(Sprite sBase)
		{
			bool fReturnNextSprite = false;
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					if (fReturnNextSprite)
						return s;
					if (s == sBase)
						fReturnNextSprite = true;
				}
			}
			return null;
		}

		// Return the previous sprite before the given sprite in the SpriteList.
		// Returns null if the given sprite is the first one, or if the given sprite doesn't exist.
		public Sprite PrevSprite(Sprite sBase)
		{
			Sprite sPrev = null;
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					if (s == sBase)
						return sPrev;
					sPrev = s;
				}
			}
			return null;
		}

		/// <summary>
		/// Find the sprite that owns this tile.
		/// </summary>
		/// <param name="nTileID"></param>
		/// <returns></returns>
		public Sprite FindSprite(int nTileID)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					if (nTileID >= s.FirstTileID && nTileID < s.FirstTileID + s.NumTiles)
						return s;
				}
			}
			return null;
		}

		public void FlushBitmaps()
		{
			foreach (SpriteType st in SpriteTypes)
				foreach (Sprite s in st.Sprites)
					s.FlushBitmaps();
		}

		public void ShiftPixels(Toolbox_Sprite.ShiftArrow shift)
		{
			if (m_ss.CurrentSprite == null)
				return;
			
			m_ss.CurrentSprite.ShiftPixels(shift);
		}

		#region Save/Export

		public void Save(System.IO.TextWriter tw, bool fBackground)
		{
			int nExportSpriteID = 0;
			int nExportFirstTileID = 0;

			tw.WriteLine("\t\t<spriteset16 name=\"sprites\" id=\"0\" desc=\"\" palette_id=\"0\">");

			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					s.Save(tw, nExportSpriteID, nExportFirstTileID);
					nExportSpriteID++;
					nExportFirstTileID += st.Width * st.Height;
				}
			}

			tw.WriteLine("\t\t</spriteset16>");
		}

		public void Export_AssignIDs()
		{
			int nSpriteExportID = 0;
			int nFirstTileID = 0;
			int nMaskIndex = 0;

			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					s.Export_AssignIDs(nSpriteExportID, nFirstTileID, nMaskIndex);
					nSpriteExportID++;
					nFirstTileID += st.Width * st.Height;
					nMaskIndex += s.CalcMaskSize();
				}
			}
		}

		public void Export_SpriteInfo(System.IO.TextWriter tw)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					s.Export_SpriteInfo(tw, st.Size, st.Shape);
				}
			}
		}

		public void Export_SpriteIDs(System.IO.TextWriter tw, string strSpritesetName)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					s.Export_SpriteIDs(tw, strSpritesetName);
				}
			}
		}

		public void Export_TileIDs(System.IO.TextWriter tw, string strSpritesetName)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					s.Export_TileIDs(tw, strSpritesetName);
				}
			}
		}

		public void Export_TileData(System.IO.TextWriter tw)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
					s.Export_TileData(tw);
			}
		}

		public void Export_SpriteMaskData(System.IO.TextWriter tw)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
					s.Export_SpriteMaskData(tw);
			}
		}

		#endregion

	}
}
