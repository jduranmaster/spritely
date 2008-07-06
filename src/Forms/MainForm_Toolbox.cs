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
		#region Toolbox

		private bool m_fToolbox_Selecting = false;

		private void Toolbox_MouseDown(object sender, MouseEventArgs e)
		{
			PictureBox pbToolbox = sender as PictureBox;
			Tab tab = GetTab_Toolbox(pbToolbox);
			Toolbox toolbox = GetToolbox(tab);
			PictureBox pbSpriteList = GetSpriteListWindow(tab);
			PictureBox pbEditSprite = GetEditSpriteWindow(tab);

			Toolbox_Sprite sprite_toolbox = toolbox as Toolbox_Sprite;

			if (sprite_toolbox != null)
			{
				if (sprite_toolbox.HilightedShiftArrow() != Toolbox_Sprite.ShiftArrow.None)
				{
					sprite_toolbox.SetMouseDownShiftArrow(true);
					pbToolbox.Invalidate();

					m_doc.GetSprites(tab).ShiftPixels(sprite_toolbox.HilightedShiftArrow());
					m_doc.GetCurrentSprite(tab).RecordUndoAction("shift");
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

		private void Toolbox_MouseMove(object sender, MouseEventArgs e)
		{
			PictureBox pbToolbox = sender as PictureBox;
			Tab tab = GetTab_Toolbox(pbToolbox);
			Toolbox toolbox = GetToolbox(tab);

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

		private void Toolbox_MouseUp(object sender, MouseEventArgs e)
		{
			PictureBox pbToolbox = sender as PictureBox;
			Tab tab = GetTab_Toolbox(pbToolbox);
			Toolbox toolbox = GetToolbox(tab);

			Toolbox_Sprite sprite_toolbox = toolbox as Toolbox_Sprite;
			if (sprite_toolbox != null)
				sprite_toolbox.SetMouseDownShiftArrow(false);

			m_fToolbox_Selecting = false;
			pbToolbox.Invalidate();
			if (toolbox as Toolbox_Map != null)
				pbBM_EditBackgroundMap.Invalidate();
		}

		private void Toolbox_MouseLeave(object sender, EventArgs e)
		{
			PictureBox pbToolbox = sender as PictureBox;
			Tab tab = GetTab_Toolbox(pbToolbox);
			Toolbox toolbox = GetToolbox(tab);

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

		private void Toolbox_Paint(object sender, PaintEventArgs e)
		{
			PictureBox pbToolbox = sender as PictureBox;
			Tab tab = GetTab_Toolbox(pbToolbox);
			Toolbox toolbox = GetToolbox(tab);
			toolbox.Draw(e.Graphics);
		}

		private void Zoom_SelectedIndexChanged(object sender, EventArgs e)
		{
			Tab tab = GetTab_ToolboxZoom(sender as ComboBox);
			UpdateZoom(tab);
		}

		private void UpdateZoom(Tab tab)
		{
			if (tab != Tab.Sprites && tab != Tab.BackgroundSprites)
				return;

			ComboBox cbZoom = GetToolboxZoomCombobox(tab);
			PictureBox pbEditSprite = GetEditSpriteWindow(tab);

			switch (cbZoom.SelectedIndex)
			{
				case 0: Tile.BigBitmapPixelSize = 1; break;
				case 1: Tile.BigBitmapPixelSize = 2; break;
				case 2: Tile.BigBitmapPixelSize = 4; break;
				case 3: Tile.BigBitmapPixelSize = 8; break;
				case 4: Tile.BigBitmapPixelSize = 16; break;
				case 5: Tile.BigBitmapPixelSize = 32; break;
			}

			pbEditSprite.Invalidate();
		}

		#endregion

	}
}
