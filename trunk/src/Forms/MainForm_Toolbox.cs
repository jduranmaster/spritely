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
		#region Toolbox

		private bool m_fToolbox_Selecting = false;

		private void S_Toolbox_MouseDown(object sender, MouseEventArgs e)
		{
			Toolbox_MouseDown(GetTab(OldTab.Type.Sprites), e);
		}

		private void BS_Toolbox_MouseDown(object sender, MouseEventArgs e)
		{
			Toolbox_MouseDown(GetTab(OldTab.Type.BackgroundSprites), e);
		}

		private void BM_Toolbox_MouseDown(object sender, MouseEventArgs e)
		{
			Toolbox_MouseDown(GetTab(OldTab.Type.BackgroundMap), e);
		}

		private void Toolbox_MouseDown(OldTab tab, MouseEventArgs e)
		{
			PictureBox pbToolbox = tab.ToolboxWindow;
			Toolbox toolbox = tab.Toolbox;
			PictureBox pbSpriteList = tab.SpriteListWindow;
			PictureBox pbEditSprite = tab.EditSpriteWindow;

			Toolbox_Sprite sprite_toolbox = toolbox as Toolbox_Sprite;

			if (sprite_toolbox != null)
			{
				if (sprite_toolbox.HilightedShiftArrow() != Toolbox_Sprite.ShiftArrow.None)
				{
					sprite_toolbox.SetMouseDownShiftArrow(true);
					pbToolbox.Invalidate();

					tab.SpriteList.ShiftPixels(sprite_toolbox.HilightedShiftArrow());
					tab.Spritesets.Current.CurrentSprite.RecordUndoAction("shift");
					pbSpriteList.Invalidate();
					pbEditSprite.Invalidate();
					m_doc.HasUnsavedChanges = true;
					return;
				}
			}

			if (toolbox.HandleMouse(e.X, e.Y))
			{
				pbToolbox.Invalidate();
				if (toolbox as Toolbox_Map != null)
					pbBM_EditBackgroundMap.Invalidate();
			}
			m_fToolbox_Selecting = true;
		}

		private void S_Toolbox_MouseMove(object sender, MouseEventArgs e)
		{
			Toolbox_MouseMove(GetTab(OldTab.Type.Sprites), e);
		}

		private void BS_Toolbox_MouseMove(object sender, MouseEventArgs e)
		{
			Toolbox_MouseMove(GetTab(OldTab.Type.BackgroundSprites), e);
		}

		private void BM_Toolbox_MouseMove(object sender, MouseEventArgs e)
		{
			Toolbox_MouseMove(GetTab(OldTab.Type.BackgroundMap), e);
		}

		private void Toolbox_MouseMove(OldTab tab, MouseEventArgs e)
		{
			PictureBox pbToolbox = tab.ToolboxWindow;
			Toolbox toolbox = tab.Toolbox;

			if (m_fToolbox_Selecting)
			{
				if (toolbox.HandleMouse(e.X, e.Y))
				{
					pbToolbox.Invalidate();
					if (toolbox as Toolbox_Map != null)
						pbBM_EditBackgroundMap.Invalidate();
				}
			}
			else
			{
				if (toolbox.HandleMouseMove(e.X, e.Y))
					pbToolbox.Invalidate();
			}
		}

		private void S_Toolbox_MouseUp(object sender, MouseEventArgs e)
		{
			Toolbox_MouseUp(GetTab(OldTab.Type.Sprites), e);
		}

		private void BS_Toolbox_MouseUp(object sender, MouseEventArgs e)
		{
			Toolbox_MouseUp(GetTab(OldTab.Type.BackgroundSprites), e);
		}

		private void BM_Toolbox_MouseUp(object sender, MouseEventArgs e)
		{
			Toolbox_MouseUp(GetTab(OldTab.Type.BackgroundMap), e);
		}

		private void Toolbox_MouseUp(OldTab tab, MouseEventArgs e)
		{
			PictureBox pbToolbox = tab.ToolboxWindow;
			Toolbox toolbox = tab.Toolbox;

			Toolbox_Sprite sprite_toolbox = toolbox as Toolbox_Sprite;
			if (sprite_toolbox != null)
				sprite_toolbox.SetMouseDownShiftArrow(false);

			m_fToolbox_Selecting = false;
			pbToolbox.Invalidate();
			if (toolbox as Toolbox_Map != null)
				pbBM_EditBackgroundMap.Invalidate();
		}

		private void S_Toolbox_MouseLeave(object sender, EventArgs e)
		{
			Toolbox_MouseLeave(GetTab(OldTab.Type.Sprites), e);
		}

		private void BS_Toolbox_MouseLeave(object sender, EventArgs e)
		{
			Toolbox_MouseLeave(GetTab(OldTab.Type.BackgroundSprites), e);
		}

		private void BM_Toolbox_MouseLeave(object sender, EventArgs e)
		{
			Toolbox_MouseLeave(GetTab(OldTab.Type.BackgroundMap), e);
		}

		private void Toolbox_MouseLeave(OldTab tab, EventArgs e)
		{
			PictureBox pbToolbox = tab.ToolboxWindow;
			Toolbox toolbox = tab.Toolbox;

			Toolbox_Sprite sprite_toolbox = toolbox as Toolbox_Sprite;
			if (sprite_toolbox != null)
				sprite_toolbox.SetMouseDownShiftArrow(false);

			if (toolbox.HandleMouseMove(-10, -10))
			{
				pbToolbox.Invalidate();
				if (toolbox as Toolbox_Map != null)
					pbBM_EditBackgroundMap.Invalidate();
			}
		}

		private void S_Toolbox_Paint(object sender, PaintEventArgs e)
		{
			Toolbox_Paint(GetTab(OldTab.Type.Sprites), e);
		}

		private void BS_Toolbox_Paint(object sender, PaintEventArgs e)
		{
			Toolbox_Paint(GetTab(OldTab.Type.BackgroundSprites), e);
		}

		private void BM_Toolbox_Paint(object sender, PaintEventArgs e)
		{
			Toolbox_Paint(GetTab(OldTab.Type.BackgroundMap), e);
		}

		private void Toolbox_Paint(OldTab tab, PaintEventArgs e)
		{
			PictureBox pbToolbox = tab.ToolboxWindow;
			Toolbox toolbox = tab.Toolbox;
			toolbox.Draw(e.Graphics);
		}

		private void S_Zoom_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void BS_Zoom_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void UpdateZoom(OldTab tab)
		{
		}

		#endregion

	}
}
