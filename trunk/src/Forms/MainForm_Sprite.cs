using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Spritely
{
	public partial class OldMainForm : Form
	{
		#region SpriteList

		public void AdjustAllSpriteListScrollbars()
		{
			AdjustSpriteListScrollbar(GetTab(OldTab.Type.Sprites));
		}

		public void AdjustAllBackgroundSpriteListScrollbars()
		{
			AdjustSpriteListScrollbar(GetTab(OldTab.Type.BackgroundSprites));
			AdjustSpriteListScrollbar(GetTab(OldTab.Type.BackgroundMap));
		}

		public void AdjustSpriteListScrollbar(OldTab tab)
		{
			VScrollBar sbSpriteList = tab.SpriteListScrollbar;
			PictureBox pbSpriteList = tab.SpriteListWindow;
			SpriteList sl = tab.SpriteList;

			int nVisibleRows = 0;// TODO: sl.VisibleScrollRows;
			int nMaxRows = 0;// TODO: sl.MaxScrollRows;

			if (nVisibleRows >= nMaxRows)
			{
				sbSpriteList.Enabled = false;
				sbSpriteList.Value = 0;
			}
			else
			{
				sbSpriteList.Enabled = true;
				sbSpriteList.Minimum = 0;
				sbSpriteList.Maximum = nMaxRows - 2;
				sbSpriteList.LargeChange = nVisibleRows - 1;
			}
			pbSpriteList.Invalidate();
		}

		private void S_SpriteList_ValueChanged(object sender, EventArgs e)
		{
			SpriteList_ValueChanged(GetTab(OldTab.Type.Sprites));
		}

		private void BS_SpriteList_ValueChanged(object sender, EventArgs e)
		{
			SpriteList_ValueChanged(GetTab(OldTab.Type.BackgroundSprites));
		}

		private void BM_SpriteList_ValueChanged(object sender, EventArgs e)
		{
			SpriteList_ValueChanged(GetTab(OldTab.Type.BackgroundMap));
		}

		private void SpriteList_ValueChanged(OldTab tab)
		{
			VScrollBar sbSpriteList = tab.SpriteListScrollbar;
			PictureBox pbSpriteList = tab.SpriteListWindow;
			//TODO: tab.SpriteList.ScrollTo(sbSpriteList.Value);
			pbSpriteList.Invalidate();
		}

		private bool m_fSpriteList_Selecting = false;

		private void S_SpriteList_MouseDown(object sender, MouseEventArgs e)
		{
			SpriteList_MouseDown(GetTab(OldTab.Type.Sprites), e);
		}

		private void BS_SpriteList_MouseDown(object sender, MouseEventArgs e)
		{
			SpriteList_MouseDown(GetTab(OldTab.Type.BackgroundSprites), e);
		}

		private void BM_SpriteList_MouseDown(object sender, MouseEventArgs e)
		{
			SpriteList_MouseDown(GetTab(OldTab.Type.BackgroundMap), e);
		}

		private void SpriteList_MouseDown(OldTab tab, MouseEventArgs e)
		{
			PictureBox pbSpriteList = tab.SpriteListWindow;

			m_fSpriteList_Selecting = true;
			//if (tab.SpriteList.HandleMouse(e.X, e.Y))
			{
				Handle_SpritesChanged(tab);
			}
		}

		private void S_SpriteList_MouseMove(object sender, MouseEventArgs e)
		{
			SpriteList_MouseMove(GetTab(OldTab.Type.Sprites), e);
		}

		private void BS_SpriteList_MouseMove(object sender, MouseEventArgs e)
		{
			SpriteList_MouseMove(GetTab(OldTab.Type.BackgroundSprites), e);
		}

		private void BM_SpriteList_MouseMove(object sender, MouseEventArgs e)
		{
			SpriteList_MouseMove(GetTab(OldTab.Type.BackgroundMap), e);
		}

		private void SpriteList_MouseMove(OldTab tab, MouseEventArgs e)
		{
			PictureBox pbSpriteList = tab.SpriteListWindow;

			if (m_fSpriteList_Selecting)
			{
			//	if (tab.SpriteList.HandleMouse(e.X, e.Y))
				{
					Handle_SpritesChanged(tab);
				}
			}
		}

		private void S_SpriteList_MouseUp(object sender, MouseEventArgs e)
		{
			SpriteList_MouseUp(GetTab(OldTab.Type.Sprites));
		}

		private void BS_SpriteList_MouseUp(object sender, MouseEventArgs e)
		{
			SpriteList_MouseUp(GetTab(OldTab.Type.BackgroundSprites));
		}

		private void BM_SpriteList_MouseUp(object sender, MouseEventArgs e)
		{
			SpriteList_MouseUp(GetTab(OldTab.Type.BackgroundMap));
		}

		private void SpriteList_MouseUp(OldTab tab)
		{
			m_fSpriteList_Selecting = false;
		}

		private void S_SpriteList_Paint(object sender, PaintEventArgs e)
		{
		}

		private void BS_SpriteList_Paint(object sender, PaintEventArgs e)
		{
		}

		private void BM_SpriteList_Paint(object sender, PaintEventArgs e)
		{
		}

		private void SpriteList_Paint(OldTab tab, PaintEventArgs e)
		{
		}

		#endregion

		#region Sprite Info

		private void lS_SpriteInfo_DoubleClick(object sender, EventArgs e)
		{
			// Double-clicking is the same as selecting the Sprite::Properties menu item
			menuSprite_Properties_Click(null, null);
		}

		public void UpdateSpriteInfo(OldTab tab)
		{
			if (tab.TabType != OldTab.Type.Sprites)
				return;

			Sprite s = tab.Spritesets.Current.CurrentSprite;

			if (s == null)
			{
				lS_SpriteInfo.Text = "";
			}
			else
			{
				lS_SpriteInfo.Text = s.Name;
				if (s.Description != "")
					lS_SpriteInfo.Text += " : " + s.Description;
			}
		}

		#endregion

		#region Edit Sprite

		private void S_EditSprite_MouseDown(object sender, MouseEventArgs e)
		{
		}

		private void BS_EditSprite_MouseDown(object sender, MouseEventArgs e)
		{
		}

		private void EditSprite_MouseDown(OldTab tab, MouseEventArgs e)
		{
		}

		private void S_EditSprite_MouseMove(object sender, MouseEventArgs e)
		{
		}

		private void BS_EditSprite_MouseMove(object sender, MouseEventArgs e)
		{
		}

		private void EditSprite_MouseMove(OldTab tab, MouseEventArgs e)
		{
		}

		private void S_EditSprite_MouseUp(object sender, MouseEventArgs e)
		{
		}

		private void BS_EditSprite_MouseUp(object sender, MouseEventArgs e)
		{
		}

		private void S_EditSprite_Paint(object sender, PaintEventArgs e)
		{
		}

		private void BS_EditSprite_Paint(object sender, PaintEventArgs e)
		{
		}

		#endregion

	}
}
