using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class Palette
	{
		private Document m_doc;
		private PaletteMgr m_mgr;
		private int m_nPaletteID;

		// This class should contain all of the user-editable data for the Tile.
		// It is used by the Undo class
		public class UndoData
		{
			/// <summary>
			/// Array of RGB values for each color in the palette.
			/// </summary>
			public int[] cRed, cGreen, cBlue;

			public int numColors;

			/// <summary>
			/// Index of the currently selected color in the palette.
			/// </summary>
			public int currentColor;

			private UndoData() {}

			public UndoData(int nColors)
			{
				numColors = nColors;
				cRed = new int[numColors];
				cGreen = new int[numColors];
				cBlue = new int[numColors];
				currentColor = 0;
			}

			public UndoData(UndoData data)
			{
				numColors = data.numColors;
				currentColor = data.currentColor;
				cRed = new int[numColors];
				cGreen = new int[numColors];
				cBlue = new int[numColors];
				for (int i = 0; i < numColors; i++)
				{
					cRed[i] = data.cRed[i];
					cGreen[i] = data.cGreen[i];
					cBlue[i] = data.cBlue[i];
				}
			}

			public bool Equals(UndoData data)
			{
				if (currentColor != data.currentColor
					|| numColors != data.numColors
					)
					return false;

				for (int i = 0; i < numColors; i++)
				{
					if (cRed[i] != data.cRed[i]
						|| cGreen[i] != data.cGreen[i]
						|| cBlue[i] != data.cBlue[i]
						)
						return false;
				}
				return true;
			}

		}
		private UndoData m_data;

		/// <summary>
		/// A snapshot of the palette data from the last undo checkpoint.
		/// </summary>
		private UndoData m_snapshot;

		/// <summary>
		/// Number of colors in the palette.
		/// </summary>
		private const int k_nColors = 16;

		/// <summary>
		/// Size of each color swatch in the palette (in pixels).
		/// </summary>
		private const int k_pxSwatchSize = 24;

		private const int k_nSwatchColumns = 8;
		private const int k_nSwatchRows = 2;
		
		/// <summary>
		/// Array of brushes for each color in the palette.
		/// </summary>
		private SolidBrush[] m_Brush;

		/// <summary>
		/// Pen used to hilight the current color in the palette.
		/// </summary>
		private static Pen m_penHilight = new Pen(Color.FromArgb(128, Color.Red), 3);

		public enum DefaultColorSet {
			None,
			BlackAndWhite,
			Color1,
			Color2,
			GrayScale,
		};
		
		public Palette(Document doc, PaletteMgr mgr, int nPaletteID)
		{
			Init(doc, mgr, nPaletteID, DefaultColorSet.None);
		}

		public Palette(Document doc, PaletteMgr mgr, int nPaletteID, DefaultColorSet eDefaultColorSet)
		{
			Init(doc, mgr, nPaletteID, eDefaultColorSet);
		}

		private void Init(Document doc, PaletteMgr mgr, int nPaletteID, DefaultColorSet eDefaultColorSet)
		{
			m_doc = doc;
			m_mgr = mgr;
			m_nPaletteID = nPaletteID;
			//TODO: check for unique palette id
			//TODO: add mechanism for auto-generating unique od
			//if (nPaletteID == -1)
			//	m_nPaletteID = GenerateUniquePaletteID();

			m_data = new UndoData(k_nColors);
			m_Brush = new SolidBrush[k_nColors];

			switch (eDefaultColorSet)
			{
				case DefaultColorSet.None:
					break;
				case DefaultColorSet.BlackAndWhite:
					DefaultPalette_BlackAndWhite();
					break;
				case DefaultColorSet.Color1:
					DefaultPalette_Color1();
					break;
				case DefaultColorSet.Color2:
					DefaultPalette_Color2();
					break;
				case DefaultColorSet.GrayScale:
					DefaultPalette_GrayScale();
					break;
				default:
					break;
			}

			// Default color = black (index 1)
			m_data.currentColor = 1;

			m_snapshot = GetUndoData();
		}

		private void DefaultPalette_BlackAndWhite()
		{
			UpdateColor(0, 0x7fff);		// transparent (white)
			UpdateColor(1, 0x0000);		// black
			for (int i = 2; i < 16; i++)
				UpdateColor(i, 0x7fff);	// white
		}

		private void DefaultPalette_Color1()
		{
			int i = 0;
			UpdateColor(i++, 0x1f, 0x1f, 0x1f);		// transparent (white)
			UpdateColor(i++, 0x00, 0x00, 0x00);		// black
			UpdateColor(i++, 0x10, 0x00, 0x00);		// dk red
			UpdateColor(i++, 0x00, 0x10, 0x00);		// green
			UpdateColor(i++, 0x10, 0x10, 0x00);		// greenish yellow
			UpdateColor(i++, 0x00, 0x00, 0x10);		// blue
			UpdateColor(i++, 0x10, 0x00, 0x10);		// purple
			UpdateColor(i++, 0x00, 0x10, 0x10);		// aqua

			UpdateColor(i++, 0x1f, 0x1f, 0x1f);		// white
			UpdateColor(i++, 0x10, 0x10, 0x10);		// gray
			UpdateColor(i++, 0x1f, 0x00, 0x00);		// red
			UpdateColor(i++, 0x00, 0x1f, 0x00);		// lt green
			UpdateColor(i++, 0x1f, 0x1f, 0x00);		// yellow
			UpdateColor(i++, 0x00, 0x00, 0x1f);		// blue
			UpdateColor(i++, 0x1f, 0x00, 0x1f);		// magenta
			UpdateColor(i++, 0x00, 0x1f, 0x1f);		// cyan
			if (i != 16)
				//System.Windows.Forms.MessageBox.Show("Wrong number of colors in palette");
				System.Windows.Forms.MessageBox.Show(ResourceMgr.GetString("ErrorNumColorsInPalette0"));
		}

		private void DefaultPalette_Color2()
		{
			int i = 0;
			UpdateColor(i++, 0x7fff);		// transparent (white)
			UpdateColor(i++, 0x0000);		// black
			UpdateColor(i++, 0x7fff);		// white
			UpdateColor(i++, 0x0c59);		// red
			UpdateColor(i++, 0x0e04);		// green
			UpdateColor(i++, 0x6d08);		// blue
			UpdateColor(i++, 0x0fdf);		// yellow
			UpdateColor(i++, 0x6b64);		// cyan
			UpdateColor(i++, 0x56b5);		// lt gray
			UpdateColor(i++, 0x318c);		// dk gray
			UpdateColor(i++, 0x0d11);		// brown
			UpdateColor(i++, 0x5a9f);		// lt red (pink)
			UpdateColor(i++, 0x4351);		// lt green
			UpdateColor(i++, 0x7e93);		// lt blue
			UpdateColor(i++, 0x1e7f);		// orange
			UpdateColor(i++, 0x5cd1);		// purple
			if (i != 16)
				//System.Windows.Forms.MessageBox.Show("Wrong number of colors in palette");
				System.Windows.Forms.MessageBox.Show(ResourceMgr.GetString("ErrorNumColorsInPalette0"));
		}

		private void DefaultPalette_GrayScale()
		{
			UpdateColor(0, 0x7fff);		// transparent (white)
			for (int i = 1, cGray=0x00; i < 8; i++, cGray += 2)
				UpdateColor(i, cGray, cGray, cGray);
			UpdateColor(8, 0x10, 0x10, 0x10);
			for (int i = 9, cGray = 0x13; i < 16; i++, cGray += 2)
				UpdateColor(i, cGray, cGray, cGray);
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;
		}

		public void Copy(Palette pal)
		{
			for (int i = 0; i < k_nColors; i++)
				UpdateColor(i, pal.Red(i), pal.Green(i), pal.Blue(i));
			m_snapshot = GetUndoData();
		}

		public int PaletteID
		{
			get { return m_nPaletteID; }
		}

		public int Red() { return m_data.cRed[m_data.currentColor]; }
		public int Green() { return m_data.cGreen[m_data.currentColor]; }
		public int Blue() { return m_data.cBlue[m_data.currentColor]; }

		public int Red(int nIndex) { return m_data.cRed[nIndex]; }
		public int Green(int nIndex) { return m_data.cGreen[nIndex]; }
		public int Blue(int nIndex) { return m_data.cBlue[nIndex]; }

		/// <summary>
		/// Return the 16-bit encoding for the specified palette index.
		/// </summary>
		/// <param name="nIndex">The index of the palette entry</param>
		/// <returns>The encoding for the specified palette entry</returns>
		public int Encoding(int nIndex)
		{
			return Encoding(m_data.cRed[nIndex], m_data.cGreen[nIndex], m_data.cBlue[nIndex]);
		}

		public int Encoding(int cRed, int cGreen, int cBlue)
		{
			return cRed | (cGreen << 5) | (cBlue << 10);
		}

		/// <summary>
		/// The index of the currently selected color in the palette.
		/// </summary>
		public int CurrentColor
		{
			get { return m_data.currentColor; }
			set { m_data.currentColor = value; }
		}

		/// <summary>
		/// Return the brush corresponding to this index in the palette.
		/// </summary>
		/// <param name="nIndex">Index into palette</param>
		/// <returns>Brush for this palette index</returns>
		public SolidBrush Brush(int nIndex)
		{
			return m_Brush[nIndex];
		}

		public SolidBrush Brush()
		{
			return m_Brush[m_data.currentColor];
		}

		private String[] k_strPaletteLabel = new String[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };

		public string Label(int nIndex)
		{
			return k_strPaletteLabel[nIndex];
		}

		/// <summary>
		/// Return a brush that can be used to draw a label over this index in the palette.
		/// </summary>
		/// <param name="nIndex">Index into palette</param>
		/// <returns>Brush for drawing text over this palette index color</returns>
		public Brush LabelBrush(int nIndex)
		{
			if (m_data.cRed[nIndex] + m_data.cGreen[nIndex] + m_data.cBlue[nIndex] > 24)
				return Brushes.Black;
			return Brushes.White;
		}

		public void UpdateColor(int cRed, int cGreen, int cBlue)
		{
			UpdateColor(m_data.currentColor, cRed, cGreen, cBlue);
		}

		public void UpdateColor(int nIndex, int cRed, int cGreen, int cBlue)
		{
			m_data.cRed[nIndex] = cRed;
			m_data.cGreen[nIndex] = cGreen;
			m_data.cBlue[nIndex] = cBlue;

			if (m_Brush[nIndex] != null)
				m_Brush[nIndex].Dispose();
			m_Brush[nIndex] = new SolidBrush(Color.FromArgb(cRed * 8, cGreen * 8, cBlue * 8));
		}

		public void UpdateColor(int nIndex, uint color)
		{
			int cRed, cGreen, cBlue;
			ExtractColors(color, out cRed, out cGreen, out cBlue);
			UpdateColor(nIndex, cRed, cGreen, cBlue);
		}

		public bool HandleMouse(int pxX, int pxY)
		{
			if (pxX < 0 || pxY < 0)
				return false;

			// Convert pixel (x,y) to palette (x,y).
			int nX = pxX / k_pxSwatchSize;
			int nY = pxY / k_pxSwatchSize;

			// Ignore if outside the SpriteList bounds.
			if (nX >= k_nSwatchColumns || nY >= k_nSwatchRows)
				return false;

			int nSelectedColor = nY * k_nSwatchColumns + nX;

			// Update the selection if a new color has been selected.
			if (m_data.currentColor != nSelectedColor)
			{
				m_data.currentColor = nSelectedColor;
				return true;
			}

			return false;
		}

		public void Draw(Graphics g)
		{
			Font f = new Font("Arial Black", 10);
			int[] nLabelXOffset = new int[16] { 6,6,6,6,6,6,6,6,6,6,5,5,5,5,5,6 };

			int pxInset = k_pxSwatchSize / 4;

			for (int iRow = 0; iRow < k_nSwatchRows; iRow++)
			{
				for (int iColumn = 0; iColumn < k_nSwatchColumns; iColumn++)
				{
					int nIndex = iRow * k_nSwatchColumns + iColumn;

					int pxX0 = 1 + iColumn * k_pxSwatchSize;
					int pxY0 = 1+ iRow * k_pxSwatchSize;

					g.FillRectangle(Brush(nIndex), pxX0, pxY0, k_pxSwatchSize, k_pxSwatchSize);

					// Draw a red X over the transparent color (index 0).
					if (Options.Palette_ShowRedXForTransparent && nIndex == 0)
					{
						int pxX0i = pxX0 + pxInset;
						int pxY0i = pxY0 + pxInset;
						int pxX1i = pxX0 + k_pxSwatchSize - pxInset;
						int pxY1i = pxY0 + k_pxSwatchSize - pxInset;
						g.DrawLine(Pens.Firebrick, pxX0i, pxY0i, pxX1i, pxY1i);
						g.DrawLine(Pens.Firebrick, pxX0i, pxY1i, pxX1i, pxY0i);
					}

					// Draw the palette index in each swatch.
					if (Options.Palette_ShowPaletteIndex)
					{
						int pxLabelOffsetX = nLabelXOffset[nIndex];
						int pxLabelOffsetY = 2;
						g.DrawString(Label(nIndex), f, LabelBrush(nIndex), pxX0 + pxLabelOffsetX, pxY0 + pxLabelOffsetY);
					}

					// Draw a border around each color swatch.
					g.DrawRectangle(Pens.White, pxX0, pxY0, k_pxSwatchSize, k_pxSwatchSize);
				}
			}

			g.DrawRectangle(Pens.Black, 0, 0, 2+ k_nSwatchColumns * k_pxSwatchSize, 2+ k_nSwatchRows * k_pxSwatchSize);

			// Hilight the currently selected color.
			if (m_mgr.HilightSelectedColor)
			{
				int x = (m_data.currentColor % k_nSwatchColumns) * k_pxSwatchSize;
				int y = (m_data.currentColor / k_nSwatchColumns) * k_pxSwatchSize;
				g.DrawRectangle(m_penHilight, x + 1, y + 1, k_pxSwatchSize, k_pxSwatchSize);
			}
		}

		public void DrawSwatch(Graphics g)
		{
			Font f = new Font("Arial Black", 10);
			int[] nLabelXOffset = new int[16] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 6 };

			int pxInset = k_pxSwatchSize / 4;

			int nIndex = m_data.currentColor;

			int pxX0 = 1;
			int pxY0 = 1;

			g.FillRectangle(Brush(nIndex), pxX0, pxY0, k_pxSwatchSize, k_pxSwatchSize);

			// Draw a red X over the transparent color (index 0).
			if (Options.Palette_ShowRedXForTransparent && nIndex == 0)
			{
				int pxX0i = pxX0 + pxInset;
				int pxY0i = pxY0 + pxInset;
				int pxX1i = pxX0 + k_pxSwatchSize - pxInset;
				int pxY1i = pxY0 + k_pxSwatchSize - pxInset;
				g.DrawLine(Pens.Firebrick, pxX0i, pxY0i, pxX1i, pxY1i);
				g.DrawLine(Pens.Firebrick, pxX0i, pxY1i, pxX1i, pxY0i);
			}

			// Draw the palette index in each swatch.
			if (Options.Palette_ShowPaletteIndex)
			{
				int pxLabelOffsetX = nLabelXOffset[nIndex];
				int pxLabelOffsetY = 2;
				g.DrawString(Label(nIndex), f, LabelBrush(nIndex), pxX0 + pxLabelOffsetX, pxY0 + pxLabelOffsetY);
			}

			// Draw a border around each color swatch.
			g.DrawRectangle(Pens.White, pxX0, pxY0, k_pxSwatchSize, k_pxSwatchSize);

			g.DrawRectangle(Pens.Black, 0, 0, 2 + k_pxSwatchSize, 2 + k_pxSwatchSize);
		}

		public bool Import(uint[] uiPalette)
		{
			for (int i = 0; i < 16; i++)
			{
				ImportColor(i, uiPalette[i]);
			}
			return true;
		}

		public void ImportColor(int nIndex, uint color)
		{
			int cRed, cGreen, cBlue;

			ExtractColors(color, out cRed, out cGreen, out cBlue);
			UpdateColor(nIndex, cRed, cGreen, cBlue);
		}

		private void ExtractColors(uint color, out int cRed, out int cGreen, out int cBlue)
		{
			cRed = (int)(color & 0x001F);
			color >>= 5;
			cGreen = (int)(color & 0x001F);
			color >>= 5;
			cBlue = (int)(color & 0x001F);
		}

		public void RecordUndoAction(string strDesc)
		{
			UndoMgr undo = m_doc.Undo();
			if (undo == null)
				return;

			UndoData data = GetUndoData();
			UndoAction_PaletteEdit action = new UndoAction_PaletteEdit(undo, this, m_snapshot, data, strDesc);
			bool fHandled = false;

			if (action.IsSelectionChange())
			{
				// Ignore selection changes
				fHandled = true;
			}
			else
			{
				// If the last undo action is also a PaletteEdit, try to merge them together
				UndoAction_PaletteEdit last_action = undo.GetCurrent() as UndoAction_PaletteEdit;
				if (last_action != null)
				{
					// Merge 2 color changes (as long as they edit the same color)
					int last_color, last_color_val1, last_color_val2;
					int this_color, this_color_val1, this_color_val2;
					if (last_action.IsColorChange(out last_color, out last_color_val1, out last_color_val2)
						&& action.IsColorChange(out this_color, out this_color_val1, out this_color_val2)
						)
					{
						if (last_color == this_color)
						{
							// If this color change takes it back to the original value, then
							// delete the UndoAction.
							if (last_color_val1 == this_color_val2)
								undo.DeleteCurrent();
							else
								last_action.UpdateRedoData(data);
							fHandled = true;
						}
					}
				}
			}
			if (!fHandled)
				undo.Push(action);

			// Update the snapshot for the next UndoAction
			RecordSnapshot();
		}

		private UndoData GetUndoData()
		{
			UndoData undo = new UndoData(k_nColors);
			RecordUndoData(ref undo);
			return undo;
		}

		public void RecordSnapshot()
		{
			RecordUndoData(ref m_snapshot);
		}

		private void RecordUndoData(ref UndoData undo)
		{
			undo.currentColor = m_data.currentColor;

			for (int i = 0; i < k_nColors; i++)
			{
				undo.cRed[i] = m_data.cRed[i];
				undo.cGreen[i] = m_data.cGreen[i];
				undo.cBlue[i] = m_data.cBlue[i];
			}
		}

		public void ApplyUndoData(UndoData undo)
		{
			m_data.currentColor = undo.currentColor;

			for (int i = 0; i < k_nColors; i++)
				UpdateColor(i, undo.cRed[i], undo.cGreen[i], undo.cBlue[i]);

			RecordSnapshot();
		}

		public void Save(System.IO.TextWriter tw)
		{
			WriteData(tw, "\t\t\t");
		}

		public bool ExportGBA(System.IO.TextWriter tw)
		{
			tw.WriteLine("\t// Palette");
			WriteData(tw, "\t");
			return true;
		}

		private bool WriteData(System.IO.TextWriter tw, string strIndent)
		{
			StringBuilder sb = null;
			for (int i = 0; i < 16; i++)
			{
				if ((i % 8) == 0)
				{
					if (sb != null)
						tw.WriteLine(sb.ToString());
					sb = new StringBuilder(strIndent);
				}
				sb.Append(String.Format("0x{0:x4},", Encoding(i)));
			}
			if (sb != null)
				tw.WriteLine(sb.ToString());
			return true;
		}
	}
}
