using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class Palette256Form : Form
	{
		private ProjectMainForm m_parent;
		private Palette m_palette;

		public Palette256Form(ProjectMainForm parent, Palette p)
		{
			m_parent = parent;
			m_palette = p;

			InitializeComponent();

			MdiParent = parent;
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			StartPosition = FormStartPosition.Manual;
			Visible = false;
			ControlBox = false;

			Text = "Palette '" + p.Name + "'";
		}

		#region Window events

		private void Palette256Form_Resize(object sender, EventArgs e)
		{
			m_parent.ResizeSubwindow(this);
		}

		private void Palette256Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_parent.CloseSubwindow(this);
			e.Cancel = true;
		}

		#endregion

		#region Palette

		private void pbPalette_MouseDown(object sender, MouseEventArgs e)
		{

		}

		private void pbPalette_MouseMove(object sender, MouseEventArgs e)
		{

		}

		private void pbPalette_MouseUp(object sender, MouseEventArgs e)
		{

		}

		private void pbPalette_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Font f = new Font("Arial Black", 10);
			// TODO: query fontmetrics and center text in box
			int[] nLabelXOffset = new int[16] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 6 };

			int nRows = 16;
			int nColumns = 16;
			int pxSize = 10;

			int pxInset = 2;

			for (int iRow = 0; iRow < nRows; iRow++)
			{
				for (int iColumn = 0; iColumn < nColumns; iColumn++)
				{
					int nIndex = iRow * nColumns + iColumn;

					int pxX0 = 1 + iColumn * pxSize;
					int pxY0 = 1 + iRow * pxSize;

					//g.FillRectangle(CurrentSubpalette.Brush(nIndex%16), pxX0, pxY0, pxSize, pxSize);

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
						//int pxLabelOffsetX = nLabelXOffset[nIndex];
						//int pxLabelOffsetY = 2;
						//g.DrawString(Label(nIndex), f, LabelBrush(nIndex), pxX0 + pxLabelOffsetX, pxY0 + pxLabelOffsetY);
					}

					// Draw a border around each color swatch.
					g.DrawRectangle(Pens.White, pxX0, pxY0, pxSize, pxSize);
				}
			}

			g.DrawRectangle(Pens.Black, 0, 0, 2 + nColumns * pxSize, 2 + nRows * pxSize);

			// Hilight the currently selected color.
			//if (m_mgr.HilightSelectedColor)
			//{
			//	int x = (m_data.currentColor % nColumns) * pxSize;
			//	int y = (m_data.currentColor / nColumns) * pxSize;
			//	g.DrawRectangle(m_penHilight, x + 1, y + 1, pxSize, pxSize);
			//}
		}

		#endregion

	}
}