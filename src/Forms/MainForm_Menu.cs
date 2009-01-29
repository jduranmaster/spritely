using NUnit.Framework;
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
		/// <summary>
		/// Enable/disable menu items as appropriate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuBar_MenuActivate(object sender, EventArgs e)
		{
			ActivateMenuItems();
		}

		/// <summary>
		/// Enable/disable menu items as appropriate
		/// </summary>
		private void ActivateMenuItems()
		{
			bool fEditingSprites = (m_tabCurrent.TabType == OldTab.Type.Sprites || m_tabCurrent.TabType == OldTab.Type.BackgroundSprites);

			// Enable/disable File menu items
			menuFile.Enabled = true;
			menuFile_New.Enabled = true;
			menuFile_Open.Enabled = true;
			menuFile_Close.Enabled = true;
			menuFile_Save.Enabled = true;
			menuFile_SaveAs.Enabled = true;
			menuFile_Export.Enabled = true;
			menuFile_RecentFiles.Enabled = false;
			menuFile_Exit.Enabled = true;

			menuEdit.Enabled = true;
			UndoMgr undo = m_doc.Undo();
			menuEdit_Undo.Enabled = (undo != null && undo.CanUndo());
			menuEdit_Redo.Enabled = (undo != null && undo.CanRedo());
			menuEdit_Cut.Enabled = false;
			menuEdit_Copy.Enabled = false;
			menuEdit_Paste.Enabled = false;

			// Enable/disable Sprite menu items
			OldTab tab = m_tabCurrent;
			menuSprite.Enabled = (tab.TabType == OldTab.Type.Sprites || tab.TabType == OldTab.Type.BackgroundSprites);
			Sprite s = tab.Spritesets.Current.CurrentSprite;
			if ((tab.TabType == OldTab.Type.Sprites || tab.TabType == OldTab.Type.BackgroundSprites)
				&& s != null
				)
			{
				menuSprite_New.Enabled = true;
				menuSprite_Duplicate.Enabled = true;
				menuSprite_Clear.Enabled = !s.IsEmpty();
				menuSprite_Resize.Enabled = true;
				menuSprite_Delete.Enabled = true;

				menuSprite_Resize_1x1.Enabled = !s.IsSize(1, 1);
				menuSprite_Resize_1x2.Enabled = !s.IsSize(1, 2);
				menuSprite_Resize_1x4.Enabled = !s.IsSize(1, 4);
				menuSprite_Resize_2x1.Enabled = !s.IsSize(2, 1);
				menuSprite_Resize_2x2.Enabled = !s.IsSize(2, 2);
				menuSprite_Resize_2x4.Enabled = !s.IsSize(2, 4);
				menuSprite_Resize_4x1.Enabled = !s.IsSize(4, 1);
				menuSprite_Resize_4x2.Enabled = !s.IsSize(4, 2);
				menuSprite_Resize_4x4.Enabled = !s.IsSize(4, 4);

				bool fFirst, fLast;
				tab.SpriteList.IsFirstLastSpriteOfType(s, out fFirst, out fLast);
				menuSprite_Properties.Enabled = true;
				menuSprite_Rotate.Enabled = true;
				menuSprite_Rotate_Clockwise.Enabled = true;
				menuSprite_Rotate_Counterclockwise.Enabled = true;
				menuSprite_Flip.Enabled = true;
				menuSprite_Flip_Horizontal.Enabled = true;
				menuSprite_Flip_Vertical.Enabled = true;
				menuSprite_Arrange.Enabled = false;
				menuSprite_Arrange_MoveUp.Enabled = false;// !fFirst;
				menuSprite_Arrange_MoveDown.Enabled = false;// !fLast;
			}
			else
			{
				// The 'new sprite' option is always enabled when were in sprite editing mode.
				menuSprite_New.Enabled = fEditingSprites;

				// Disable all sprite editing options if there is no sprite selection or
				// if we're not editing sprites.
				menuSprite_Duplicate.Enabled = false;
				menuSprite_Clear.Enabled = false;
				menuSprite_Resize.Enabled = false;
				menuSprite_Delete.Enabled = false;
				menuSprite_Properties.Enabled = false;
				menuSprite_Rotate.Enabled = false;
				menuSprite_Rotate_Clockwise.Enabled = false;
				menuSprite_Rotate_Counterclockwise.Enabled = false;
				menuSprite_Flip.Enabled = false;
				menuSprite_Flip_Horizontal.Enabled = false;
				menuSprite_Flip_Vertical.Enabled = false;
				menuSprite_Arrange.Enabled = false;
				menuSprite_Arrange_MoveUp.Enabled = false;
				menuSprite_Arrange_MoveDown.Enabled = false;
			}

			// Enable/disable Palette menu items
			menuPalette.Enabled = (tab.TabType == OldTab.Type.Sprites || tab.TabType == OldTab.Type.BackgroundSprites);
			Palette pm = tab.Palettes.CurrentPalette;
			Subpalette p = null;
			//if (pm != null)
			//	p = pm.CurrentSubpalette;
			if (p != null)
			{
				menuPalette_Copy.Enabled = false;
				menuPalette_Paste.Enabled = false;
				menuPalette_Clear.Enabled = false;
				menuPalette_ViewEncoding.Enabled = fEditingSprites;
				menuPalette_Color.Enabled = false;
				menuPalette_Color_Copy.Enabled = false;
				menuPalette_Color_Paste.Enabled = false;
				menuPalette_Color_Clear.Enabled = false;
			}
			else
			{
				menuPalette_Copy.Enabled = false;
				menuPalette_Paste.Enabled = false;
				menuPalette_Clear.Enabled = false;
				menuPalette_ViewEncoding.Enabled = false;
				menuPalette_Color.Enabled = false;
				menuPalette_Color_Copy.Enabled = false;
				menuPalette_Color_Paste.Enabled = false;
				menuPalette_Color_Clear.Enabled = false;
			}

			menuOptions.Enabled = true;
			menuOptions_Sprite.Enabled = true;
			menuOptions_Palette.Enabled = true;

			// Used for debugging only - set to false for release builds.
			//menuTest.Visible = false;
		}

		#region File Menu

		private void menuFile_New_Click(object sender, EventArgs e)
		{
		}

		private void menuFile_Open_Click(object sender, EventArgs e)
		{
		}

		private void menuFile_Close_Click(object sender, EventArgs e)
		{
		}

		private void menuFile_Save_Click(object sender, EventArgs e)
		{
		}

		private void menuFile_SaveAs_Click(object sender, EventArgs e)
		{
		}

		private void menuFile_Export_Click(object sender, EventArgs e)
		{
		}

		public void menuFile_RecentFiles_Click(object sender, EventArgs e)
		{
		}

		private void menuFile_Exit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		#endregion

		#region Edit Menu

		private void menuEdit_Undo_Click(object sender, EventArgs e)
		{
		}

		private void menuEdit_Redo_Click(object sender, EventArgs e)
		{
		}

		private void menuEdit_Cut_Click(object sender, EventArgs e)
		{
		}

		private void menuEdit_Copy_Click(object sender, EventArgs e)
		{
		}

		private void menuEdit_Paste_Click(object sender, EventArgs e)
		{
		}

		#endregion

		#region Sprite Menu

		private void menuSprite_New_Click(object sender, EventArgs e)
		{
		}

		private void menuSprite_Clear_Click(object sender, EventArgs e)
		{
		}

		private void menuSprite_Duplicate_Click(object sender, EventArgs e)
		{
		}

		private void menuSprite_Resize_Click(object sender, EventArgs e)
		{
		}

		private void menuSprite_Delete_Click(object sender, EventArgs e)
		{
		}

		private void menuSprite_Rotate_Clockwise_Click(object sender, EventArgs e)
		{
		}

		private void menuSprite_Rotate_Counterclockwise_Click(object sender, EventArgs e)
		{
		}

		private void menuSprite_Rotate_180_Click(object sender, EventArgs e)
		{
		}

		private void menuSprite_Flip_Horizontal_Click(object sender, EventArgs e)
		{
		}

		private void menuSprite_Flip_Vertical_Click(object sender, EventArgs e)
		{
		}

		private void menuSprite_Flip_Both_Click(object sender, EventArgs e)
		{
		}

		private void menuSprite_Properties_Click(object sender, EventArgs e)
		{
		}

		private void menuSprite_Arrange_MoveUp_Click(object sender, EventArgs e)
		{
		}

		private void menuSprite_Arrange_MoveDown_Click(object sender, EventArgs e)
		{
		}

		#endregion

		#region Palette Menu

		private void menuPalette_EditColors_Click(object sender, EventArgs e)
		{
		}

		#endregion

		#region Options Menu

		private void menuOptions_Sprite_Click(object sender, EventArgs e)
		{
		}

		private void menuOptions_Palette_Click(object sender, EventArgs e)
		{
		}

		private void menuOptions_Map_Click(object sender, EventArgs e)
		{
		}

		private void menuOptions_XXX_Click(int nOptionPageIndex)
		{
		}

		#endregion

		#region Test Menu

		private void menuTest_RunUnittests_Click(object sender, EventArgs e)
		{
		}

		private void menuTestLoadImage_Click(object sender, EventArgs e)
		{
		}

		private void menuTest_ShowUndoHistory_Click(object sender, EventArgs e)
		{
		}

		private void menuTest_CollisionTest_Click(object sender, EventArgs e)
		{
		}

		private void menuTest_ShowProjectWindow_Click(object sender, EventArgs e)
		{
		}

		#endregion

	}
}
