using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class SpritesetForm : Form
	{
		private ProjectMainForm m_parent;
		private Spriteset m_ss;
		private SpriteList m_sl;

		public bool HasLocation = false;

		public SpritesetForm(ProjectMainForm parent, Spriteset ss)
		{
			m_parent = parent;
			m_ss = ss;
			m_sl = ss.SpriteList;

			InitializeComponent();

			MdiParent = parent;
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			Text = "Spriteset '" + ss.Name + "'";
		}

		private void SpritesetForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.Hide();
			e.Cancel = true;
		}

		public void AdjustScrollbar(int nVisibleRows, int nMaxRows)
		{
			if (nVisibleRows >= nMaxRows)
			{
				sbSprites.Enabled = false;
				sbSprites.Value = 0;
			}
			else
			{
				sbSprites.Enabled = true;
				sbSprites.Minimum = 0;
				sbSprites.Maximum = nMaxRows - 2;
				sbSprites.LargeChange = nVisibleRows - 1;
			}
			pbSprites.Invalidate();
		}

		private void sbSprites_ValueChanged(object sender, EventArgs e)
		{
			m_sl.ScrollTo(sbSprites.Value);
			pbSprites.Invalidate();
		}

		private bool m_fSpriteList_Selecting = false;

		private void pbSprites_MouseDown(object sender, MouseEventArgs e)
		{
			m_fSpriteList_Selecting = true;
			if (m_sl.HandleMouse(e.X, e.Y))
			{
				Handle_SpritesChanged();
			}
		}

		private void pbSprites_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fSpriteList_Selecting)
			{
				if (m_sl.HandleMouse(e.X, e.Y))
				{
					Handle_SpritesChanged();
				}
			}
		}

		private void pbSprites_MouseUp(object sender, MouseEventArgs e)
		{
			m_fSpriteList_Selecting = false;
		}

		private void pbSprites_DoubleClick(object sender, EventArgs e)
		{
			SpriteForm winSprite = new SpriteForm(m_parent, m_sl.CurrentSprite);
			winSprite.Show();
		}

		private void pbSprites_Paint(object sender, PaintEventArgs e)
		{
			PictureBox pbSpriteList = sender as PictureBox;
			m_sl.DrawList(e.Graphics);
		}

		private void Handle_SpritesChanged()
		{
			//if (m_doc.GetCurrentSprite() != null)
			//	m_doc.GetSpritePalettes().CurrentPaletteID = m_doc.GetCurrentSprite().PaletteID;

			// Updating the palette causes a cascade of updates that results in the sprites and
			// bg maps being updated.
			//UpdatePaletteColor();

			//UpdateSpriteInfo();
			//ActivateMenuItems();
			pbSprites.Invalidate();
		}

	}
}
