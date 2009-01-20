using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class ProjectMainForm : Form
	{
		private Document m_doc;
		private ProjectTreeViewForm m_ProjectTreeView;

		public ProjectMainForm(Document doc)
		{
			m_doc = doc;

			InitializeComponent();

			//this.IsMdiContainer = true;

			m_ProjectTreeView = new ProjectTreeViewForm(this, doc);
			m_ProjectTreeView.Show();
		}

		public int MenuBarHeight
		{
			get { return 0; }//return menuBar.Location.Y + menuBar.Height; }
		}

		/// <summary>
		/// Open a new Spriteset window.
		/// </summary>
		public void OpenSpritesetWindow()
		{
			SpritesetForm winSpriteset = m_doc.Spritesets.Current.SpritesetWindow;
			if (!winSpriteset.HasLocation)
			{
				winSpriteset.Left = m_ProjectTreeView.Width;
				winSpriteset.Top = 0;
				winSpriteset.HasLocation = true;
			}
			winSpriteset.Show();
		}

		/// <summary>
		/// Open a new Palette window.
		/// </summary>
		public void OpenPalette16Window()
		{
			Palette16Form winPalette16 = m_doc.GetSpritePalette(Options.DefaultPaletteId).PaletteWindow;
			if (!winPalette16.HasLocation)
			{
				winPalette16.Left = m_ProjectTreeView.Width;
				winPalette16.Top = 0;
				winPalette16.HasLocation = true;
			}
			winPalette16.Show();
		}

		private void tabSet_DrawItem(object sender, DrawItemEventArgs e)
		{
			Graphics g = e.Graphics;

			TabPage tp = tabSet.TabPages[e.Index];
			string strTabName = tp.Text;

			Rectangle r = e.Bounds;
			SizeF size = g.MeasureString(strTabName, e.Font);
			float x = r.X + (r.Width - size.Width) / 2;
			float y = r.Y + 4;

			if (tabSet.SelectedIndex == e.Index)
			{
				g.FillRectangle(Brushes.White, r);
				g.DrawString(strTabName, e.Font, Brushes.Black, x, y + 1);
			}
			else
			{
				Color c = ControlPaint.Dark(this.BackColor, 0.01f);
				System.Drawing.Drawing2D.LinearGradientBrush b = new System.Drawing.Drawing2D.LinearGradientBrush(
					new Point(0, 0), new Point(0, 30), Color.White, c);
				g.FillRectangle(b, r);
				g.DrawString(strTabName, e.Font, Brushes.Black, x, y);
				g.DrawLine(Pens.Black, r.Left, r.Bottom, r.Right, r.Bottom);
			}

			// Draw a line out past the last tab.
			if (e.Index == tabSet.TabPages.Count - 1)
			{
				g.DrawLine(Pens.Black, r.Right, r.Bottom, r.Right+tabSet.Width, r.Bottom);
			}
		}

		private void tabSet_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

	}
}
