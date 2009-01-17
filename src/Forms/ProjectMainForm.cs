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

			this.IsMdiContainer = true;

			m_ProjectTreeView = new ProjectTreeViewForm(this, doc);
			m_ProjectTreeView.Show();
		}

		public int MenuBarHeight
		{
			get { return menuBar.Location.Y + menuBar.Height; }
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

	}
}
