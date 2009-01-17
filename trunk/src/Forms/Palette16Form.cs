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

		public bool HasLocation = false;

		public Palette16Form(ProjectMainForm parent, Palette p)
		{
			m_parent = parent;
			m_palette = p;

			InitializeComponent();

			MdiParent = parent;
			FormBorderStyle = FormBorderStyle.SizableToolWindow;
			Text = "Palette '" + p.Name + "'";
		}

		#region Palette Select

		private void pbPaletteSelect_MouseDown(object sender, MouseEventArgs e)
		{

		}

		private void pbPaletteSelect_MouseMove(object sender, MouseEventArgs e)
		{

		}

		private void pbPaletteSelect_MouseUp(object sender, MouseEventArgs e)
		{

		}

		private void pbPaletteSelect_Paint(object sender, PaintEventArgs e)
		{
			m_palette.DrawSelector(e.Graphics);
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
			m_palette.CurrentSubpalette.Draw(e.Graphics);
		}

		#endregion

		#region Palette Swatch

		private void pbPaletteSwatch_Paint(object sender, PaintEventArgs e)
		{
			m_palette.CurrentSubpalette.DrawSwatch(e.Graphics);
		}

		#endregion
	}
}