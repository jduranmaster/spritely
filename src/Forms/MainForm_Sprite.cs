using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Spritely
{
	public partial class MainForm : Form
	{
		#region SpriteList

		public void AdjustAllSpriteListScrollbars()
		{
			AdjustSpriteListScrollbar(GetTab(Tab.Type.Sprites));
		}

		public void AdjustAllBackgroundSpriteListScrollbars()
		{
			AdjustSpriteListScrollbar(GetTab(Tab.Type.BackgroundSprites));
			AdjustSpriteListScrollbar(GetTab(Tab.Type.BackgroundMap));
		}

		public void AdjustSpriteListScrollbar(Tab tab)
		{
			VScrollBar sbSpriteList = tab.SpriteListScrollbar;
			PictureBox pbSpriteList = tab.SpriteListWindow;
			SpriteList sl = tab.SpriteList;

			int nVisibleRows = sl.VisibleScrollRows;
			int nMaxRows = sl.MaxScrollRows;

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
			SpriteList_ValueChanged(GetTab(Tab.Type.Sprites));
		}

		private void BS_SpriteList_ValueChanged(object sender, EventArgs e)
		{
			SpriteList_ValueChanged(GetTab(Tab.Type.BackgroundSprites));
		}

		private void BM_SpriteList_ValueChanged(object sender, EventArgs e)
		{
			SpriteList_ValueChanged(GetTab(Tab.Type.BackgroundMap));
		}

		private void SpriteList_ValueChanged(Tab tab)
		{
			VScrollBar sbSpriteList = tab.SpriteListScrollbar;
			PictureBox pbSpriteList = tab.SpriteListWindow;
			tab.SpriteList.ScrollTo(sbSpriteList.Value);
			pbSpriteList.Invalidate();
		}

		private bool m_fSpriteList_Selecting = false;

		private void S_SpriteList_MouseDown(object sender, MouseEventArgs e)
		{
			SpriteList_MouseDown(GetTab(Tab.Type.Sprites), e);
		}

		private void BS_SpriteList_MouseDown(object sender, MouseEventArgs e)
		{
			SpriteList_MouseDown(GetTab(Tab.Type.BackgroundSprites), e);
		}

		private void BM_SpriteList_MouseDown(object sender, MouseEventArgs e)
		{
			SpriteList_MouseDown(GetTab(Tab.Type.BackgroundMap), e);
		}

		private void SpriteList_MouseDown(Tab tab, MouseEventArgs e)
		{
			PictureBox pbSpriteList = tab.SpriteListWindow;

			m_fSpriteList_Selecting = true;
			if (tab.SpriteList.HandleMouse(e.X, e.Y))
			{
				Handle_SpritesChanged(tab);
			}
		}

		private void S_SpriteList_MouseMove(object sender, MouseEventArgs e)
		{
			SpriteList_MouseMove(GetTab(Tab.Type.Sprites), e);
		}

		private void BS_SpriteList_MouseMove(object sender, MouseEventArgs e)
		{
			SpriteList_MouseMove(GetTab(Tab.Type.BackgroundSprites), e);
		}

		private void BM_SpriteList_MouseMove(object sender, MouseEventArgs e)
		{
			SpriteList_MouseMove(GetTab(Tab.Type.BackgroundMap), e);
		}

		private void SpriteList_MouseMove(Tab tab, MouseEventArgs e)
		{
			PictureBox pbSpriteList = tab.SpriteListWindow;

			if (m_fSpriteList_Selecting)
			{
				if (tab.SpriteList.HandleMouse(e.X, e.Y))
				{
					Handle_SpritesChanged(tab);
				}
			}
		}

		private void S_SpriteList_MouseUp(object sender, MouseEventArgs e)
		{
			SpriteList_MouseUp(GetTab(Tab.Type.Sprites));
		}

		private void BS_SpriteList_MouseUp(object sender, MouseEventArgs e)
		{
			SpriteList_MouseUp(GetTab(Tab.Type.BackgroundSprites));
		}

		private void BM_SpriteList_MouseUp(object sender, MouseEventArgs e)
		{
			SpriteList_MouseUp(GetTab(Tab.Type.BackgroundMap));
		}

		private void SpriteList_MouseUp(Tab tab)
		{
			m_fSpriteList_Selecting = false;
		}

		private void S_SpriteList_Paint(object sender, PaintEventArgs e)
		{
			SpriteList_Paint(GetTab(Tab.Type.Sprites), e);
		}

		private void BS_SpriteList_Paint(object sender, PaintEventArgs e)
		{
			SpriteList_Paint(GetTab(Tab.Type.BackgroundSprites), e);
		}

		private void BM_SpriteList_Paint(object sender, PaintEventArgs e)
		{
			SpriteList_Paint(GetTab(Tab.Type.BackgroundMap), e);
		}

		private void SpriteList_Paint(Tab tab, PaintEventArgs e)
		{
			PictureBox pbSpriteList = tab.SpriteListWindow;
			tab.SpriteList.DrawList(e.Graphics);
		}

		#endregion

		#region Sprite Info

		private void lS_SpriteInfo_DoubleClick(object sender, EventArgs e)
		{
			// Double-clicking is the same as selecting the Sprite::Properties menu item
			menuSprite_Properties_Click(null, null);
		}

		public void UpdateSpriteInfo(Tab tab)
		{
			if (tab.TabType != Tab.Type.Sprites)
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

		private bool m_fEditSprite_Selecting = false;
		private bool m_fEditSprite_Erase = false;

		private void S_EditSprite_MouseDown(object sender, MouseEventArgs e)
		{
			EditSprite_MouseDown(GetTab(Tab.Type.Sprites), e);
		}

		private void BS_EditSprite_MouseDown(object sender, MouseEventArgs e)
		{
			EditSprite_MouseDown(GetTab(Tab.Type.BackgroundSprites), e);
		}

		private void EditSprite_MouseDown(Tab tab, MouseEventArgs e)
		{
			m_fEditSprite_Selecting = true;

			// Right click is handled the same as the Eraser tool
			m_fEditSprite_Erase = (e.Button == MouseButtons.Right);

			EditSprite_MouseMove(tab, e);
		}

		private void S_EditSprite_MouseMove(object sender, MouseEventArgs e)
		{
			EditSprite_MouseMove(GetTab(Tab.Type.Sprites), e);
		}

		private void BS_EditSprite_MouseMove(object sender, MouseEventArgs e)
		{
			EditSprite_MouseMove(GetTab(Tab.Type.BackgroundSprites), e);
		}

		private void EditSprite_MouseMove(Tab tab, MouseEventArgs e)
		{
			if (m_fEditSprite_Selecting)
			{
				PictureBox pbEditSprite = tab.EditSpriteWindow;
				Toolbox toolbox = tab.Toolbox;

				Toolbox.ToolType tool = toolbox.CurrentTool;
				if (m_fEditSprite_Erase)
					tool = Toolbox.ToolType.Eraser;
				
				if (tab.SpriteList.HandleMouse_Edit(e.X, e.Y, tool))
				{
					if (tool == Toolbox.ToolType.Eyedropper)
					{
						UpdatePaletteSelect(tab);
					}
					else
					{
						PictureBox pbSpriteList = tab.SpriteListWindow;
						pbSpriteList.Invalidate();
						pbEditSprite.Invalidate();
						m_doc.HasUnsavedChanges = true;
					}
				}
			}
		}

		private void S_EditSprite_MouseUp(object sender, MouseEventArgs e)
		{
			EditSprite_MouseUp(GetTab(Tab.Type.Sprites));
		}

		private void BS_EditSprite_MouseUp(object sender, MouseEventArgs e)
		{
			EditSprite_MouseUp(GetTab(Tab.Type.BackgroundSprites));
		}

		private void EditSprite_MouseUp(Tab tab)
		{
			if (m_fEditSprite_Selecting)
			{
				PictureBox pbEditSprite = tab.EditSpriteWindow;
				Toolbox toolbox = tab.Toolbox;
				Toolbox.ToolType tool = toolbox.CurrentTool;

				// Close out the edit action.
				tab.SpriteList.HandleMouse_FinishEdit(tool);

				// Record the undo action.
				string strTool = "";
				bool fRecordUndo = false;
				switch (tool)
				{
					//case Toolbox.ToolType.Select - no  undo
					case Toolbox.ToolType.Pencil:
						strTool = "pencil";
						fRecordUndo = true;
						break;
					//case Toolbox.ToolType.Eyedropper - no undo
					case Toolbox.ToolType.FloodFill:
						strTool = "bucket";
						fRecordUndo = true;
						break;
					case Toolbox.ToolType.Eraser:
						strTool = "eraser";
						break;
				}

				if (fRecordUndo)
				{
					Sprite s = tab.Spritesets.Current.CurrentSprite;
					if (s != null)
						s.RecordUndoAction(strTool);
				}
			}

			m_fEditSprite_Selecting = false;
		}

		private void S_EditSprite_Paint(object sender, PaintEventArgs e)
		{
			EditSprite_Paint(GetTab(Tab.Type.Sprites), e);
		}

		private void BS_EditSprite_Paint(object sender, PaintEventArgs e)
		{
			EditSprite_Paint(GetTab(Tab.Type.BackgroundSprites), e);
		}

		private void EditSprite_Paint(Tab tab, PaintEventArgs e)
		{
			PictureBox pbEditSprite = tab.EditSpriteWindow;
			tab.SpriteList.DrawEditSprite(e.Graphics);
		}

		#endregion

	}
}
