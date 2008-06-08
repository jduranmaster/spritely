using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class PaletteMgr
	{
		private Document m_doc;

		/// <summary>
		/// Is this a set of background palettes?
		/// </summary>
		private Boolean m_fBackground;

		/// <summary>
		/// Hilight the selected color?
		/// </summary>
		private bool m_fHilightSelectedColor;

		private Palette[] m_palettes;

		private const int m_nMaxPalettes = 16;
		private int m_nAllocatedPalettes;
		private int m_nCurrentPalette;

		private const int m_pxSelectorHeight = 16;
		private const int m_pxSelectorWidth = 12;

		private Dictionary<string, int> m_mapPaletteNameToID;

		public PaletteMgr(Document doc, bool fBackground)
		{
			m_doc = doc;
			m_fBackground = fBackground;
			m_fHilightSelectedColor = true;

			m_palettes = new Palette[m_nMaxPalettes];
			m_nAllocatedPalettes = 0;
			m_nCurrentPalette = 0;

			m_mapPaletteNameToID = new Dictionary<string, int>();
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;

			for (int i = 0; i < m_nMaxPalettes; i++)
			{
				m_palettes[i].UpdateDocument(doc);
			}
		}

		public bool HilightSelectedColor
		{
			get { return m_fHilightSelectedColor; }
			set { m_fHilightSelectedColor = value; }
		}

		public int NumPalettes
		{
			get { return m_nAllocatedPalettes; }
		}

		public int CurrentPaletteID
		{
			get { return m_nCurrentPalette; }
			set { m_nCurrentPalette = value; }
		}

		public Palette CurrentPalette
		{
			get { return m_palettes[m_nCurrentPalette]; }
		}

		public Palette GetPalette(int nIndex)
		{
			return m_palettes[nIndex];
		}

		public int GetNamedPaletteID(string strName)
		{
			if (m_mapPaletteNameToID.ContainsKey(strName))
				return m_mapPaletteNameToID[strName];
			return 0;
		}

		public Palette GetNamedPalette(string strName)
		{
			if (m_mapPaletteNameToID.ContainsKey(strName))
				return m_palettes[m_mapPaletteNameToID[strName]];
			return null;
		}

		public void AddDefaultPalette(string strID, Palette.DefaultColorSet eDefault)
		{
			if (m_nAllocatedPalettes < m_nMaxPalettes)
			{
				//TODO: check for unique string id
				m_mapPaletteNameToID.Add(strID, m_nAllocatedPalettes);
				m_palettes[m_nAllocatedPalettes] = new Palette(m_doc, this, m_nAllocatedPalettes, eDefault);
				m_nAllocatedPalettes++;
			}
		}

		public void AddMissingPalettes()
		{
			for (int i = 0; i < m_nMaxPalettes; i++)
			{
				if (m_palettes[i] == null)
				{
					//TODO: paletteid=m_nAllocatedPalettes is not guaranteed to be unique
					m_mapPaletteNameToID.Add(String.Format("{0}", m_nAllocatedPalettes), m_nAllocatedPalettes);
					m_palettes[i] = new Palette(m_doc, this, m_nAllocatedPalettes, Palette.DefaultColorSet.BlackAndWhite);
					m_nAllocatedPalettes++;
				}
			}
		}

		public void CopyPalettes(PaletteMgr orig)
		{
			m_fBackground = orig.m_fBackground;
			m_nAllocatedPalettes = orig.m_nAllocatedPalettes;
			m_nCurrentPalette = orig.m_nCurrentPalette;
			for (int i = 0; i < m_nAllocatedPalettes; i++)
			{
				m_palettes[i] = new Palette(m_doc, this, 0);
				m_palettes[i].Copy(orig.m_palettes[i]);
			}
		}

		public bool ImportPalette(string strID, uint[] uiPalette)
		{
			bool fResult = false;
			if (m_nAllocatedPalettes < m_nMaxPalettes)
			{
				m_mapPaletteNameToID.Add(strID, m_nAllocatedPalettes);
				m_palettes[m_nAllocatedPalettes] = new Palette(m_doc, this, 0);
				fResult = m_palettes[m_nAllocatedPalettes].Import(uiPalette);
				if (fResult)
					m_nAllocatedPalettes++;
			}
			return fResult;
		}

		/// <summary>
		/// Handle a mouse move in the palette selector
		/// </summary>
		/// <param name="pxX"></param>
		/// <param name="pxY"></param>
		/// <returns>True if a new palette is selected</returns>
		public bool HandleSelectorMouse(int pxX, int pxY)
		{
			if (pxX < 0 || pxY < 0)
				return false;

			// Convert pixel (x,y) to palette (x,y).
			int nX = pxX / m_pxSelectorWidth;
			int nY = pxY / m_pxSelectorHeight;

			// Ignore if outside the bounds.
			if (nX >= m_nAllocatedPalettes || nY != 0)
				return false;

			// Update the selection if a new palette has been selected.
			if (m_nCurrentPalette != nX)
			{
				m_nCurrentPalette = nX;
				return true;
			}

			return false;
		}

		public void DrawSelector(Graphics g)
		{
			Font f = new Font("Arial Narrow", 8);
			String[] strPaletteName = new String[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
			int pxLabelOffsetX = 2;
			int pxLabelOffsetY = 1;

			for (int i = 0; i < m_nMaxPalettes; i++)
			{
				int pxX0 = i * m_pxSelectorWidth;
				int pxY0 = 0;

				Brush brBackground = new SolidBrush(System.Drawing.SystemColors.Control);
				Brush brFont = Brushes.DarkGray;

				if (i == m_nCurrentPalette)
				{
					brBackground = Brushes.DarkGray;
					brFont = Brushes.White;
				}
				else if (i < m_nAllocatedPalettes)
				{
					brBackground = Brushes.White;
					brFont = Brushes.Black;
				}

				g.FillRectangle(brBackground, pxX0, pxY0, m_pxSelectorWidth, m_pxSelectorHeight);
				g.DrawString(strPaletteName[i], f, brFont, pxX0 + pxLabelOffsetX, pxY0 + pxLabelOffsetY);

				// Draw a border around each palette index.
				g.DrawRectangle(Pens.Gray, pxX0, pxY0, m_pxSelectorWidth, m_pxSelectorHeight);
			}
		}

		public void Save(System.IO.TextWriter tw)
		{
			if (m_fBackground)
				tw.WriteLine("\t<bgpalettes>");
			else
				tw.WriteLine("\t<palettes>");

			for (int i = 0; i < m_nAllocatedPalettes; i++)
			{
				tw.WriteLine(String.Format("\t\t<palette id=\"{0}\">", i));
				m_palettes[i].Save(tw);
				tw.WriteLine("\t\t</palette>");
			}

			if (m_fBackground)
				tw.WriteLine("\t</bgpalettes>");
			else
				tw.WriteLine("\t</palettes>");
		}

		public void ExportGBA_Palette(System.IO.TextWriter tw)
		{
			for (int i = 0; i < m_nAllocatedPalettes; i++)
			{
				m_palettes[i].ExportGBA(tw);
			}
		}

	}
}
