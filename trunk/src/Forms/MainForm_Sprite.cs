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

		public void AdjustAllSpriteListScrollbars(int nVisibleRows, int nMaxRows)
		{
			AdjustSpriteListScrollbar(Tab.Sprites, nVisibleRows, nMaxRows);
		}

		public void AdjustAllBackgroundSpriteListScrollbars(int nVisibleRows, int nMaxRows)
		{
			AdjustSpriteListScrollbar(Tab.BackgroundSprites, nVisibleRows, nMaxRows);
			AdjustSpriteListScrollbar(Tab.BackgroundMap, nVisibleRows, nMaxRows);
		}

		public void AdjustSpriteListScrollbar(Tab tab, int nVisibleRows, int nMaxRows)
		{
			VScrollBar sbSpriteList = GetSpriteListScrollbar(tab);
			PictureBox pbSpriteList = GetSpriteListWindow(tab);

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

		private void SpriteList_ValueChanged(object sender, EventArgs e)
		{
			VScrollBar sbSpriteList = sender as VScrollBar;
			Tab tab = GetTab_SpriteListScrollbar(sbSpriteList);
			PictureBox pbSpriteList = GetSpriteListWindow(tab);
			m_doc.GetSprites(tab).ScrollTo(sbSpriteList.Value);
			pbSpriteList.Invalidate();
		}

		private bool m_fSpriteList_Selecting = false;

		private void SpriteList_MouseDown(object sender, MouseEventArgs e)
		{
			PictureBox pbSpriteList = sender as PictureBox;
			Tab tab = GetTab_SpriteList(pbSpriteList);

			m_fSpriteList_Selecting = true;
			if (m_doc.GetSprites(tab).HandleMouse(e.X, e.Y))
			{
				Handle_SpritesChanged(tab);
			}
		}

		private void SpriteList_MouseMove(object sender, MouseEventArgs e)
		{
			PictureBox pbSpriteList = sender as PictureBox;
			Tab tab = GetTab_SpriteList(pbSpriteList);

			if (m_fSpriteList_Selecting)
			{
				if (m_doc.GetSprites(tab).HandleMouse(e.X, e.Y))
				{
					Handle_SpritesChanged(tab);
				}
			}
		}

		private void SpriteList_MouseUp(object sender, MouseEventArgs e)
		{
			m_fSpriteList_Selecting = false;
		}

		private void SpriteList_Paint(object sender, PaintEventArgs e)
		{
			PictureBox pbSpriteList = sender as PictureBox;
			Tab tab = GetTab_SpriteList(pbSpriteList);
			m_doc.GetSprites(tab).DrawList(e.Graphics);
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
			if (tab != Tab.Sprites)
				return;

			Sprite s = m_doc.GetCurrentSprite(tab);

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

		private void EditSprite_MouseDown(object sender, MouseEventArgs e)
		{
			m_fEditSprite_Selecting = true;

			// Right click is handled the same as the Eraser tool
			m_fEditSprite_Erase = (e.Button == MouseButtons.Right);

			EditSprite_MouseMove(sender, e);
		}

		private void EditSprite_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fEditSprite_Selecting)
			{
				PictureBox pbEditSprite = sender as PictureBox;
				Tab tab = GetTab_EditSprite(pbEditSprite);
				Toolbox toolbox = GetToolbox(tab);

				Toolbox.ToolType tool = toolbox.CurrentTool;
				if (m_fEditSprite_Erase)
					tool = Toolbox.ToolType.Eraser;
				
				if (m_doc.GetSprites(tab).HandleMouse_Edit(e.X, e.Y, tool))
				{
					if (tool == Toolbox.ToolType.Eyedropper)
					{
						UpdatePaletteSelect(tab);
					}
					else
					{
						PictureBox pbSpriteList = GetSpriteListWindow(tab);
						pbSpriteList.Invalidate();
						pbEditSprite.Invalidate();
						m_doc.HasUnsavedChanges = true;
					}
				}
			}
		}

		private void EditSprite_MouseUp(object sender, MouseEventArgs e)
		{
			if (m_fEditSprite_Selecting)
			{
				PictureBox pbEditSprite = sender as PictureBox;
				Tab tab = GetTab_EditSprite(pbEditSprite);
				Toolbox toolbox = GetToolbox(tab);
				Toolbox.ToolType tool = toolbox.CurrentTool;

				// Close out the edit action.
				m_doc.GetSprites(tab).HandleMouse_FinishEdit(tool);

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
					m_doc.GetCurrentSprite(tab).RecordUndoAction(strTool);
			}

			m_fEditSprite_Selecting = false;
		}

		private void EditSprite_Paint(object sender, PaintEventArgs e)
		{
			PictureBox pbEditSprite = sender as PictureBox;
			Tab tab = GetTab_EditSprite(pbEditSprite);
			m_doc.GetSprites(tab).DrawEditSprite(e.Graphics);
		}

		#endregion

	}
}
