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
		private Palette m_Palettes;

		private bool m_fBackground;

		private int m_nScrollPosition;
		private Font m_font = new Font("Arial", 8, FontStyle.Bold);

		private Sprite m_spriteSelected = null;

		private int m_nSprites;
		private int m_nTiles;

		private const int MaxTilesX = 8;
		private const int MaxTilesY = 17;
		private const int MarginX = 0;

		private static Pen m_penHilight = new Pen(Color.FromArgb(128, Color.Red), 3);

		private SpriteType[] SpriteTypes = new SpriteType[]
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

		public SpriteList(Document doc, Spriteset ts, Palette palette, bool fBackground)
		{
			m_doc = doc;
			m_ss = ts;
			m_Palettes = palette;

			m_fBackground = fBackground;

			m_nScrollPosition = 0;
		}

		public Palette Palettes
		{
			get { return m_Palettes; }
		}

		public int NumSprites
		{
			get { return m_nSprites; }
		}

		public int NumTiles
		{
			get { return m_nTiles; }
		}

		public Sprite CurrentSprite
		{
			get { return m_spriteSelected; }
			set { m_spriteSelected = value; }
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
			m_spriteSelected = s;

			m_nSprites++;
			m_nTiles += (s.TileWidth * s.TileHeight);

			if (fAddUndo)
			{
				UndoMgr undo = m_doc.Undo();
				if (undo != null)
					undo.Push(new UndoAction_AddSprite(undo, this, s, true));
			}

			RecalcScrollHeights();
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
				m_spriteSelected = null;
				if (undo != null)
					m_spriteSelected = undo.FindMostRecentSprite();
				if (m_spriteSelected == null)
					m_spriteSelected = sNewSelection;

				m_nSprites--;
				m_nTiles -= nTiles;
				RecalcScrollHeights();

				if (fAddUndo && undo != null)
					undo.Push(new UndoAction_AddSprite(undo, this, sToRemove, false));
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
			}
		}

		public bool ResizeSelectedSprite(int tileNewWidth, int tileNewHeight)
		{
			Sprite sToResize = m_spriteSelected;
			if (sToResize == null)
				return false;

			int nOldTiles = sToResize.NumTiles;

			if (!sToResize.Resize(tileNewWidth, tileNewHeight))
				return false;

			m_nTiles += sToResize.NumTiles - nOldTiles;

			//TODO: adjust the background map

			MoveToCorrectSpriteType(sToResize);
			RecalcScrollHeights();
			return true;
		}

		// Return true if successfully rotated.
		public bool RotateSelectedSprite(Sprite.RotateDirection dir)
		{
			Sprite sToRotate = m_spriteSelected;
			if (sToRotate == null)
				return false;

			int tileNewWidth = sToRotate.TileHeight;
			int tileNewHeight = sToRotate.TileWidth;

			if (!sToRotate.Rotate(dir))
				return false;

			MoveToCorrectSpriteType(sToRotate);
			RecalcScrollHeights();
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
		/// Select the first sprite.
		/// This is useful after loading a file of sprites since otherwise the selected
		/// sprite will be the last one loaded from the file.
		/// </summary>
		public void SelectFirstSprite()
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					m_spriteSelected = s;
					return;
				}
			}
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

		private int m_nTotalScrollHeight = 0;

		public int VisibleScrollRows
		{
			get {return MaxTilesY;}
		}

		public int MaxScrollRows
		{
			get { return m_nTotalScrollHeight; }
		}

		private void RecalcScrollHeights()
		{
			m_nTotalScrollHeight = 0;
			foreach (SpriteType st in SpriteTypes)
			{
				st.FirstLine = m_nTotalScrollHeight;
				if (st.Sprites.Count == 0)
					st.ScrollHeight = 0;
				else
				{
					// Number of rows required for this SpriteType
					int nRows = ((st.Width * st.Sprites.Count) + (MaxTilesX - 1)) / MaxTilesX;
					// 1 (for the title bar) + rows * height of each row (in tiles)
					st.ScrollHeight = 1 + (nRows * st.Height);
				}

				m_nTotalScrollHeight += st.ScrollHeight;
			}

			if (m_doc.Owner != null)
			{
				if (m_fBackground)
					m_doc.Owner.AdjustAllBackgroundSpriteListScrollbars();
				else
				{
					m_doc.Owner.AdjustAllSpriteListScrollbars();
					//m_Owner.NewUI.
				}
			}
		}

		public void ScrollTo(int nPosition)
		{
			m_nScrollPosition = nPosition;
		}

		public void FlushBitmaps()
		{
			foreach (SpriteType st in SpriteTypes)
				foreach (Sprite s in st.Sprites)
					s.FlushBitmaps();
		}

		/// <returns>True if the SpriteList needs to be redrawn.</returns>
		public bool HandleMouse(int pxX, int pxY)
		{
			if (pxX < 0 || pxY < 0)
				return false;

			// Convert screen pixel (x,y) to tile (x,y).
			int nTileX = pxX / Tile.SmallBitmapScreenSize;
			int nTileY = pxY / Tile.SmallBitmapScreenSize;

			// Ignore if outside the SpriteList bounds.
			if (nTileX >= MaxTilesX || nTileY >= MaxTilesY)
				return false;

			// Adjust for the current scroll position.
			nTileY += m_nScrollPosition;

			foreach (SpriteType st in SpriteTypes)
			{
				if (st.Sprites.Count != 0
					// Ignore (st.FirstLine == nTileY) because this is a click on the label
					&& st.FirstLine < nTileY
					&& (st.FirstLine + st.ScrollHeight) > nTileY
					)
				{
					// Calculate which tile was clicked on within this SpriteType.
					nTileY -= (st.FirstLine + 1);

					int nSpriteX = nTileX / st.Width;
					int nSpriteY = nTileY / st.Height;
					int nSpritesPerRow = MaxTilesX / st.Width;
					int nSelectedSprite = (nSpriteY * nSpritesPerRow) + nSpriteX;

					// Don't allow selection beyond the end of the valid sprites.
					if (nSelectedSprite >= st.Sprites.Count)
						return false;

					// Update the selection if a new tile has been selected.
					if (m_spriteSelected != st.Sprites[nSelectedSprite])
					{
						m_spriteSelected = st.Sprites[nSelectedSprite];
						return true;
					}
				}
			}
			return false;
		}

		public bool HandleMouse_Edit(int pxX, int pxY, Toolbox.ToolType tool)
		{
			if (m_spriteSelected == null)
				return false;

			if (pxX < 0 || pxY < 0)
				return false;

			// Convert screen pixel (x,y) to sprite pixel index (x,y).
			int nPixelX = pxX / Tile.BigBitmapPixelSize;
			int nPixelY = pxY / Tile.BigBitmapPixelSize;

			return m_spriteSelected.Click(nPixelX, nPixelY, tool);
		}

		// Call this on mouseUp when the current editing tool has finished.
		public void HandleMouse_FinishEdit(Toolbox.ToolType tool)
		{
			if (m_spriteSelected == null)
				return;

			m_spriteSelected.FinishEdit(tool);
		}

		public void ShiftPixels(Toolbox_Sprite.ShiftArrow shift)
		{
			if (m_spriteSelected == null)
				return;
			
			m_spriteSelected.ShiftPixels(shift);
		}

		public void DrawList(Graphics g)
		{
			int pxTileSize = Tile.SmallBitmapScreenSize;		// size of each tile (in screen pixels)

			int pxX = MarginX;
			int pxY = 0;
			int nRow = 0;
			int pxScrollOffset = m_nScrollPosition * pxTileSize + 1;

			// Information about currently selected sprite.
			Rectangle rSelectedSprite = new Rectangle(0,0, 0,0);
			bool fSelectedSprite = false;

			Brush brTitleBar;
			if (m_fBackground)
				brTitleBar = Brushes.DarkViolet;
			else
				brTitleBar = Brushes.Green;

			foreach (SpriteType st in SpriteTypes)
			{
				if (st.Sprites.Count != 0)
				{
					// Draw label for sprite size.
					if (nRow >= m_nScrollPosition && nRow < (m_nScrollPosition + MaxTilesY))
					{
						const int pxFontAdjustX = 2;	// Font indent
						const int pxFontAdjustY = 1;	// Font baseline adjust
						g.FillRectangle(brTitleBar, pxX+1, pxY+2 - pxScrollOffset, (MaxTilesX * pxTileSize) -1, pxTileSize - 3);
						g.DrawString(st.Name, m_font, Brushes.White, pxX + pxFontAdjustX, pxY + pxFontAdjustY - pxScrollOffset);
						//g.DrawString(st.ScrollHeight.ToString(), m_font, Brushes.White, (MaxTilesX * (pxTileSize - 2)) + pxFontAdjustX, nY + pxFontAdjustY - pxScrollOffset);
					}
					pxY += pxTileSize;
					nRow++;

					int pxSpriteWidth = st.Width * pxTileSize;
					int pxSpriteHeight = st.Height * pxTileSize;

					foreach (Sprite s in st.Sprites)
					{
						// Move to a new row if necessary.
						if (pxX >= MaxTilesX * pxTileSize)
						{
							pxY += pxSpriteHeight;
							nRow += st.Height;
							pxX = MarginX;
						}

						int pxX0 = pxX;
						int pxY0 = pxY - pxScrollOffset;

						// Don't draw unless some part of the sprite is visible.
						if (nRow >= m_nScrollPosition && nRow <= (m_nScrollPosition + MaxTilesY)
							|| ((nRow + st.Height) >= m_nScrollPosition && (nRow + st.Height) < (m_nScrollPosition + MaxTilesY))
							)
						{
							// Draw the sprite.
							s.DrawSmallSprite(g, pxX0, pxY0);

							// Draw a border around the sprite.
							g.DrawRectangle(Pens.Gray, pxX0, pxY0, pxSpriteWidth, pxSpriteHeight);

							if (m_spriteSelected == s)
							{
								// Record the bounds of the currently selected sprite so that we can draw
								// a rectangle border around it. We want this border to be drawn on top of
								// all of the sprites (since it may extend slightly on top of neighboring
								// sprites), so we need to draw it last. 
								fSelectedSprite = true;
								rSelectedSprite = new Rectangle(pxX0, pxY0, pxSpriteWidth, pxSpriteHeight);
							}
						}

						pxX += pxSpriteWidth;
					}

					// Position pen for the next label.
					pxX = MarginX;
					pxY += pxSpriteHeight;
					nRow += st.Height;
				}
			}

			// Draw a heavy border around the current sprite (if any).
			if (fSelectedSprite)
				g.DrawRectangle(m_penHilight, rSelectedSprite);
		}

		// Draw the currently selected sprite
		public void DrawEditSprite(Graphics g)
		{
			if (m_spriteSelected != null)
				m_spriteSelected.DrawEditSprite(g);
		}

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

	}
}
