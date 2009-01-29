using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class Palette16Form : Form
	{
		private ProjectMainForm m_parent;
		private Palette m_palette;

		public Palette16Form(ProjectMainForm parent, Palette p)
		{
			m_parent = parent;
			m_palette = p;

			InitializeComponent();

			MdiParent = parent;
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			StartPosition = FormStartPosition.Manual;
			Visible = false;
			ControlBox = false;

			if (m_palette.IsBackground)
				Text = "BgPalette '" + p.Name + "'";
			else
				Text = "Palette '" + p.Name + "'";

		}

		#region Window events

		private void Palette16Form_Resize(object sender, EventArgs e)
		{
			m_parent.ResizeSubwindow(this);
		}

		private void Palette16Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_parent.CloseSubwindow(this);
			e.Cancel = true;
		}

		#endregion

		#region Subwindow updates

		/// <summary>
		/// The selected sprite has changed.
		/// </summary>
		public void SpriteSelectionChanged()
		{
			// New sprite could mean new subpalette, so update the windows.
			pbPaletteSelect.Invalidate();
			pbPalette.Invalidate();
			pbFgSwatch.Invalidate();
		}

		/// <summary>
		/// The data for one of the sprites has changed.
		/// </summary>
		public void SpriteDataChanged()
		{
			// Nothing to update.
		}

		/// <summary>
		/// The currently subpalette selection has changed.
		/// </summary>
		public void SubpaletteSelectChanged()
		{
			pbPaletteSelect.Invalidate();
			pbPalette.Invalidate();
			pbFgSwatch.Invalidate();
		}

		/// <summary>
		/// A new color has been selected.
		/// </summary>
		public void ColorSelectChanged()
		{
			pbPalette.Invalidate();
			pbFgSwatch.Invalidate();
		}

		/// <summary>
		/// The current color value has changed in the palette.
		/// </summary>
		public void ColorDataChanged()
		{
			pbPalette.Invalidate();
			pbFgSwatch.Invalidate();
		}

		#endregion

		#region Palette Select

		private const int k_nSelectorRows = 2;
		private const int k_nSelectorColumns = 8;

		/// <summary>
		/// Width (in pixels) of the subpalette selector.
		/// </summary>
		private const int k_pxSelectorWidth = 19;

		/// <summary>
		/// Height (in pixels) of the subpalette selector.
		/// </summary>
		private const int k_pxSelectorHeight = 15;

		private bool m_fSubpaletteSelect_Selecting = false;
		private int m_fSubpaletteSelect_OriginalSubpalette = 0;

		private void pbPaletteSelect_MouseDown(object sender, MouseEventArgs e)
		{
			m_fSubpaletteSelect_OriginalSubpalette = m_palette.CurrentSubpalette;
			m_fSubpaletteSelect_Selecting = true;

			pbPaletteSelect_MouseMove(sender, e);
		}

		private void pbPaletteSelect_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fSubpaletteSelect_Selecting)
			{
				if (HandleMouse_SelectSubpalette(e.X, e.Y))
				{
					m_parent.HandleSubpaletteSelectChange(m_palette);
				}
			}
		}

		private void pbPaletteSelect_MouseUp(object sender, MouseEventArgs e)
		{
			if (!m_fSubpaletteSelect_Selecting)
				return;
			m_fSubpaletteSelect_Selecting = false;

			// Record an undo action if the current palette selection has changed
			if (m_fSubpaletteSelect_OriginalSubpalette != m_palette.CurrentSubpalette)
			{
				Sprite s =  m_parent.ActiveSprite();
				s.RecordUndoAction("palette select");
			}
		}

		/// <summary>
		/// Handle a mouse move in the subpalette selector
		/// </summary>
		/// <param name="pxX"></param>
		/// <param name="pxY"></param>
		/// <returns>True if a new palette is selected</returns>
		private bool HandleMouse_SelectSubpalette(int pxX, int pxY)
		{
			if (pxX < 0 || pxY < 0)
				return false;

			// Convert pixel (x,y) to subpalette (x,y).
			int nX = pxX / k_pxSelectorWidth;
			int nY = pxY / k_pxSelectorHeight;

			// Ignore if outside the subpalette selector bounds.
			if (nX >= k_nSelectorColumns || nY >= k_nSelectorRows)
				return false;

			int nSelectedSubpalette = nY * k_nSelectorColumns + nX;

			// Update the selection if a new palette has been selected.
			if (m_palette.CurrentSubpalette != nSelectedSubpalette)
			{
				m_palette.CurrentSubpalette = nSelectedSubpalette;
				return true;
			}

			return false;
		}

		private void pbPaletteSelect_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Font f = new Font("Arial Narrow", 8);

			int pxLabelOffsetX = 5;
			int pxLabelOffsetY = 0;

			int nRows = k_nSelectorRows;
			int nColumns = k_nSelectorColumns;
			int pxWidth = k_pxSelectorWidth;
			int pxHeight = k_pxSelectorHeight;

			for (int y = 0; y < nRows; y++)
			{
				for (int x = 0; x < nColumns; x++)
				{
					int pxX0 = x * pxWidth;
					int pxY0 = y * pxHeight;
					int i = y * nColumns + x;

					Brush brBackground = Brushes.White;
					Brush brFont = Brushes.Black;

					if (i == m_palette.CurrentSubpalette)
					{
						brBackground = Brushes.DarkGray;
						brFont = Brushes.White;
					}

					g.FillRectangle(brBackground, pxX0, pxY0, pxWidth, pxHeight);
					string strSubpaletteName = String.Format("{0:X1}", i);
					g.DrawString(strSubpaletteName, f, brFont, pxX0 + pxLabelOffsetX, pxY0 + pxLabelOffsetY);

					// Draw a border around each palette index.
					g.DrawRectangle(Pens.Gray, pxX0, pxY0, pxWidth, pxHeight);
				}
			}
		}

		#endregion

		#region Palette

		/// <summary>
		/// Size of each color square in the palette (in pixels).
		/// </summary>
		private const int k_pxColorSize = 24;

		/// <summary>
		/// Number of color columns displayed in the palette. 
		/// </summary>
		private const int k_nPaletteColumns = 4;

		/// <summary>
		/// Number of color rows displayed in the palette.
		/// </summary>
		private const int k_nPaletteRows = 4;

		/// <summary>
		/// Pen used to hilight the current color in the palette.
		/// </summary>
		private static Pen m_penHilight = new Pen(Color.FromArgb(128, Color.Red), 3);

		private bool m_fPalette_Selecting = false;
		private int m_fPalette_OriginalColor = 0;

		private void pbPalette_MouseDown(object sender, MouseEventArgs e)
		{
			m_fPalette_OriginalColor = m_palette.CurrentSubpalette;
			m_fPalette_Selecting = true;

			pbPaletteSelect_MouseMove(sender, e);
		}

		private void pbPalette_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fPalette_Selecting)
			{
				if (HandleMouse_Palette(e.X, e.Y))
				{
					m_parent.HandleColorSelectChange(m_palette);
				}
			}
		}

		private void pbPalette_MouseUp(object sender, MouseEventArgs e)
		{
			m_fPalette_Selecting = false;

			// Record an undo action if the current color selection has changed
			if (m_fPalette_OriginalColor != m_palette.CurrentSubpalette)
			{
				Subpalette sp = m_palette.GetCurrentSubpalette();
				sp.RecordUndoAction("select color");
			}
		}

		private bool HandleMouse_Palette(int pxX, int pxY)
		{
			if (pxX < 0 || pxY < 0)
				return false;

			// Convert pixel (x,y) to palette (x,y).
			int nX = pxX / k_pxColorSize;
			int nY = pxY / k_pxColorSize;

			// Ignore if outside the SpriteList bounds.
			if (nX >= k_nPaletteColumns || nY >= k_nPaletteRows)
				return false;

			int nSelectedColor = nY * k_nPaletteColumns + nX;

			// Update the selection if a new color has been selected.
			Subpalette sp = m_palette.GetCurrentSubpalette();
			if (sp.CurrentColor != nSelectedColor)
			{
				sp.CurrentColor = nSelectedColor;
				return true;
			}

			return false;
		}

		private void pbPalette_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Font f = new Font("Arial Black", 10);
			Subpalette sp = m_palette.GetCurrentSubpalette();

			// TODO: query fontmetrics and center text in box
			int[] nLabelXOffset = new int[16] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 6 };

			int nRows = k_nPaletteRows;
			int nColumns = k_nPaletteColumns;
			int pxSize = k_pxColorSize;

			int pxInset = pxSize / 4;

			for (int iRow = 0; iRow < nRows; iRow++)
			{
				for (int iColumn = 0; iColumn < nColumns; iColumn++)
				{
					int nIndex = iRow * nColumns + iColumn;

					int pxX0 = 1 + iColumn * pxSize;
					int pxY0 = 1 + iRow * pxSize;

					g.FillRectangle(sp.Brush(nIndex), pxX0, pxY0, pxSize, pxSize);

					// Draw a red X over the transparent color (index 0).
					if (Options.Palette_ShowRedXForTransparent && nIndex == 0)
					{
						int pxX0i = pxX0 + pxInset;
						int pxY0i = pxY0 + pxInset;
						int pxX1i = pxX0 + pxSize - pxInset;
						int pxY1i = pxY0 + pxSize - pxInset;
						g.DrawLine(Pens.Firebrick, pxX0i, pxY0i, pxX1i, pxY1i);
						g.DrawLine(Pens.Firebrick, pxX0i, pxY1i, pxX1i, pxY0i);
					}

					// Draw the palette index in each swatch.
					if (Options.Palette_ShowPaletteIndex)
					{
						int pxLabelOffsetX = nLabelXOffset[nIndex];
						int pxLabelOffsetY = 2;
						g.DrawString(sp.Label(nIndex), f, sp.LabelBrush(nIndex), pxX0 + pxLabelOffsetX, pxY0 + pxLabelOffsetY);
					}

					// Draw a border around each color swatch.
					g.DrawRectangle(Pens.White, pxX0, pxY0, pxSize, pxSize);
				}
			}

			g.DrawRectangle(Pens.Black, 0, 0, 2 + nColumns * pxSize, 2 + nRows * pxSize);

			// Hilight the currently selected color.
			int x = (sp.CurrentColor % nColumns) * pxSize;
			int y = (sp.CurrentColor / nColumns) * pxSize;
			g.DrawRectangle(m_penHilight, x + 1, y + 1, pxSize, pxSize);
		}

		#endregion

		#region Palette Swatch

		/// <summary>
		/// Size of the fg/bg color swatch (in pixels).
		/// </summary>
		private const int k_pxSwatchSize = 24;

		private void pbFgSwatch_Paint(object sender, PaintEventArgs e)
		{
			Subpalette sp = m_palette.GetCurrentSubpalette();
			DrawSwatch(e.Graphics, sp.CurrentColor);
		}

		private void pbBgSwatch_Paint(object sender, PaintEventArgs e)
		{
			DrawSwatch(e.Graphics, 0);
		}

		public void DrawSwatch(Graphics g, int nIndex)
		{
			Subpalette sp = m_palette.GetCurrentSubpalette();

			Font f = new Font("Arial Black", 10);
			// TODO: query fontmetrics and center text in box
			int[] nLabelXOffset = new int[16] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 6 };

			int pxInset = k_pxSwatchSize / 4;

			int pxX0 = 1;
			int pxY0 = 1;

			g.FillRectangle(sp.Brush(nIndex), pxX0, pxY0, k_pxSwatchSize, k_pxSwatchSize);

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
				g.DrawString(sp.Label(nIndex), f, sp.LabelBrush(nIndex), pxX0 + pxLabelOffsetX, pxY0 + pxLabelOffsetY);
			}

			// Draw a border around each color swatch.
			g.DrawRectangle(Pens.White, pxX0, pxY0, k_pxSwatchSize, k_pxSwatchSize);

			g.DrawRectangle(Pens.Black, 0, 0, 2 + k_pxSwatchSize, 2 + k_pxSwatchSize);
		}

		#endregion

	}
}