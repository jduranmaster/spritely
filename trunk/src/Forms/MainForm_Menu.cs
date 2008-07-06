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
			bool fEditingSprites = (m_eCurrentTab == Tab.Sprites || m_eCurrentTab == Tab.BackgroundSprites);

			// Enable/disable File menu items
			menuFile.Enabled = true;
			menuFile_New.Enabled = true;
			menuFile_Open.Enabled = true;
			menuFile_Close.Enabled = true;
			menuFile_Save.Enabled = true;
			menuFile_SaveAs.Enabled = true;
			menuFile_Export.Enabled = true;
			menuFile_RecentFiles.Enabled = (m_recent.Count != 0);
			menuFile_Exit.Enabled = true;

			menuEdit.Enabled = true;
			UndoMgr undo = m_doc.Undo();
			menuEdit_Undo.Enabled = (undo != null && undo.CanUndo());
			menuEdit_Redo.Enabled = (undo != null && undo.CanRedo());
			menuEdit_Cut.Enabled = false;
			menuEdit_Copy.Enabled = false;
			menuEdit_Paste.Enabled = false;

			// Enable/disable Sprite menu items
			menuSprite.Enabled = (m_eCurrentTab == Tab.Sprites || m_eCurrentTab == Tab.BackgroundSprites);
			Sprite s = m_doc.GetCurrentSprite(m_eCurrentTab);
			if ((m_eCurrentTab == Tab.Sprites || m_eCurrentTab == Tab.BackgroundSprites)
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
				m_doc.GetSprites(m_eCurrentTab).IsFirstLastSpriteOfType(s, out fFirst, out fLast);
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
			menuPalette.Enabled = (m_eCurrentTab == Tab.Sprites || m_eCurrentTab == Tab.BackgroundSprites);
			Palette p = m_doc.GetSpritePalettes(m_eCurrentTab).CurrentPalette;
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
			menuTest.Visible = false;
		}

		private void menuFile_New_Click(object sender, EventArgs e)
		{
			if (!m_doc.Close())
				return;

			Handle_NewDocument();
		}

		private void menuFile_Open_Click(object sender, EventArgs e)
		{
			if (!m_doc.Close())
				return;

			if (!m_doc.Open())
				return;

			m_recent.AddFile(m_doc.Name);

			Handle_AllSpritesChanged();
			SetTitleBar(m_doc.Name);
		}

		private void menuFile_Close_Click(object sender, EventArgs e)
		{
			if (!m_doc.Close())
				return;

			Handle_NewDocument();
		}

		private void menuFile_Save_Click(object sender, EventArgs e)
		{
			m_doc.Save();
			SetTitleBar(m_doc.Name);
		}

		private void menuFile_SaveAs_Click(object sender, EventArgs e)
		{
			m_doc.SaveAs();
			SetTitleBar(m_doc.Name);
		}

		private void menuFile_Export_Click(object sender, EventArgs e)
		{
			pbS_SpriteList.Focus();
			m_doc.Export();
		}

		public void menuFile_RecentFiles_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem mi = (sender as ToolStripMenuItem);
			string strTag = mi.Tag as string;
			if (strTag == null || strTag == "")
				return;
			int nIndex = Int32.Parse(strTag);
			string strFilename = m_recent.GetNthRecentFile(nIndex);
			if (strFilename == "")
				return;

			if (!System.IO.File.Exists(strFilename))
			{
				Warning(String.Format(ResourceMgr.GetString("FileDoesntExist"), strFilename));
				m_recent.RemoveFile(strFilename);
				return;
			}

			if (!m_doc.Close())
				return;

			if (!m_doc.Open(strFilename))
				return;

			m_recent.AddFile(m_doc.Name);

			Handle_AllSpritesChanged();
			SetTitleBar(m_doc.Name);
		}

		private void menuFile_Exit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void menuEdit_Undo_Click(object sender, EventArgs e)
		{
			UndoMgr undo = m_doc.Undo();
			if (undo == null)
				return;
			undo.ApplyUndo();

			Handle_AllSpritesChanged();
		}

		private void menuEdit_Redo_Click(object sender, EventArgs e)
		{
			UndoMgr undo = m_doc.Undo();
			if (undo == null)
				return;
			undo.ApplyRedo();

			Handle_AllSpritesChanged();
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

		private void menuSprite_New_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem mi = (sender as ToolStripMenuItem);
			string strTag = mi.Tag as string;
			if (strTag == null || strTag == "")
				return;
			string[] aSize = strTag.Split('x');
			int nWidth = Int32.Parse(aSize[0]);
			int nHeight = Int32.Parse(aSize[1]);

			m_doc.GetSprites(m_eCurrentTab).AddSprite(nWidth, nHeight, "", "", true);
			Handle_SpritesChanged(m_eCurrentTab);
			m_doc.HasUnsavedChanges = true;
		}

		private void menuSprite_Clear_Click(object sender, EventArgs e)
		{
			Sprite s = m_doc.GetCurrentSprite(m_eCurrentTab);
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				//if (!AskYesNo("Are you sure you want to erase the data in the currently selected sprite?"))
				if (!AskYesNo(ResourceMgr.GetString("EraseCurrentSprite")))
					return;
				s.Clear();
				s.RecordUndoAction("clear");
				m_doc.HasUnsavedChanges = true;

				Handle_SpritesChanged(m_eCurrentTab);
			}
		}

		private void menuSprite_Duplicate_Click(object sender, EventArgs e)
		{
			Sprite s = m_doc.GetCurrentSprite(m_eCurrentTab);
			if (s == null)
				return;

			if (m_doc.GetSprites(m_eCurrentTab).DuplicateSelectedSprite())
			{
				m_doc.HasUnsavedChanges = true;
				Handle_SpritesChanged(m_eCurrentTab);
			}
		}

		private void menuSprite_Resize_Click(object sender, EventArgs e)
		{
			bool fBackground = (m_eCurrentTab == Tab.BackgroundSprites
									|| m_eCurrentTab == Tab.BackgroundMap);

			Sprite s = m_doc.GetCurrentSprite(m_eCurrentTab);
			if (s == null)
				return;

			ToolStripMenuItem mi = (sender as ToolStripMenuItem);
			string strTag = mi.Tag as string;
			if (strTag == null || strTag == "")
				return;
			string[] aSize = strTag.Split('x');
			int tileWidth = Int32.Parse(aSize[0]);
			int tileHeight = Int32.Parse(aSize[1]);

			if (m_doc.GetSprites(m_eCurrentTab).ResizeSelectedSprite(tileWidth, tileHeight))
			{
				s.RecordUndoAction("resize");
				m_doc.HasUnsavedChanges = true;
				Handle_SpritesChanged(m_eCurrentTab);
			}
		}

		private void menuSprite_Delete_Click(object sender, EventArgs e)
		{
			Sprite s = m_doc.GetCurrentSprite(m_eCurrentTab);
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				//if (!AskYesNo("Are you sure you want to delete the currently selected sprite?"))
				if (!AskYesNo(ResourceMgr.GetString("DeleteCurrentSprite")))
					return;
			}

			m_doc.GetSprites(m_eCurrentTab).RemoveSelectedSprite();

			Handle_SpritesChanged(m_eCurrentTab);
			m_doc.HasUnsavedChanges = true;
		}

		private void menuSprite_Rotate_Clockwise_Click(object sender, EventArgs e)
		{
			Sprite s = m_doc.GetCurrentSprite(m_eCurrentTab);
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				SpriteList sl = m_doc.GetSprites(m_eCurrentTab);
				if (sl.RotateSelectedSprite(Sprite.RotateDirection.Clockwise90))
				{
					s.RecordUndoAction("rotatecw");
					m_doc.HasUnsavedChanges = true;
					Handle_SpritesChanged(m_eCurrentTab);
				}
			}
		}

		private void menuSprite_Rotate_Counterclockwise_Click(object sender, EventArgs e)
		{
			Sprite s = m_doc.GetCurrentSprite(m_eCurrentTab);
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				SpriteList sl = m_doc.GetSprites(m_eCurrentTab);
				if (sl.RotateSelectedSprite(Sprite.RotateDirection.Counterclockwise90))
				{
					s.RecordUndoAction("rotateccw");
					m_doc.HasUnsavedChanges = true;
					Handle_SpritesChanged(m_eCurrentTab);
				}
			}
		}

		private void menuSprite_Rotate_180_Click(object sender, EventArgs e)
		{
			Sprite s = m_doc.GetCurrentSprite(m_eCurrentTab);
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				SpriteList sl = m_doc.GetSprites(m_eCurrentTab);
				if (sl.RotateSelectedSprite(Sprite.RotateDirection.Clockwise180))
				{
					s.RecordUndoAction("rotate180");
					m_doc.HasUnsavedChanges = true;
					Handle_SpritesChanged(m_eCurrentTab);
				}
			}
		}

		private void menuSprite_Flip_Horizontal_Click(object sender, EventArgs e)
		{
			Sprite s = m_doc.GetCurrentSprite(m_eCurrentTab);
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				s.Flip(true, false);
				s.RecordUndoAction("fliph");
				m_doc.HasUnsavedChanges = true;

				Handle_SpritesChanged(m_eCurrentTab);
			}
		}

		private void menuSprite_Flip_Vertical_Click(object sender, EventArgs e)
		{
			Sprite s = m_doc.GetCurrentSprite(m_eCurrentTab);
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				s.Flip(false, true);
				s.RecordUndoAction("flipv");
				m_doc.HasUnsavedChanges = true;

				Handle_SpritesChanged(m_eCurrentTab);
			}
		}

		private void menuSprite_Flip_Both_Click(object sender, EventArgs e)
		{
			Sprite s = m_doc.GetCurrentSprite(m_eCurrentTab);
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				s.Flip(true, true);
				s.RecordUndoAction("flipboth");
				m_doc.HasUnsavedChanges = true;

				Handle_SpritesChanged(m_eCurrentTab);
			}
		}

		private void menuSprite_Properties_Click(object sender, EventArgs e)
		{
			SpriteProperties properties = new SpriteProperties(this, m_doc.GetSprites(m_eCurrentTab));
			properties.ShowDialog();
		}

		private void menuSprite_Arrange_MoveUp_Click(object sender, EventArgs e)
		{

		}

		private void menuSprite_Arrange_MoveDown_Click(object sender, EventArgs e)
		{

		}

		private void menuPalette_EditColors_Click(object sender, EventArgs e)
		{
			Palette p = m_doc.GetSpritePalettes(m_eCurrentTab).CurrentPalette;
			if (p == null)
				return;

			ColorEncodingView cedit = new ColorEncodingView(p);
			DialogResult result = cedit.ShowDialog();

			UpdatePaletteColor(m_eCurrentTab);
			if (result == DialogResult.Yes)
				m_doc.HasUnsavedChanges = true;
		}

		private void menuOptions_Sprite_Click(object sender, EventArgs e)
		{
			menuOptions_XXX_Click(0);
		}

		private void menuOptions_Palette_Click(object sender, EventArgs e)
		{
			menuOptions_XXX_Click(1);
		}

		private void menuOptions_Map_Click(object sender, EventArgs e)
		{
			menuOptions_XXX_Click(2);
		}

		private void menuOptions_XXX_Click(int nOptionPageIndex)
		{
			OptionsEdit opt = new OptionsEdit(nOptionPageIndex);
			DialogResult result = opt.ShowDialog();

			// If any of the options have changed...
			if (result == DialogResult.Yes)
			{
				PictureBox pb;
				pb = GetEditSpriteWindow(m_eCurrentTab);
				if (pb != null)
					pb.Invalidate();
				pb = GetEditMapWindow(m_eCurrentTab);
				if (pb != null)
					pb.Invalidate();
				pb = GetPaletteWindow(m_eCurrentTab);
				if  (pb != null)
					pb.Invalidate();
				pb = GetPaletteSwatchWindow(m_eCurrentTab);
				if (pb != null)
					pb.Invalidate();
			}
		}

		private void menuHelp_About_Click(object sender, EventArgs e)
		{
			About about = new About();
			about.ShowDialog();
		}


		private void menuTestLoadImage_Click(object sender, EventArgs e)
		{
			// bitmap import test
			Bitmap b = new Bitmap(@"c:\gamedev\projects\test.png");
			Sprite s = m_doc.GetCurrentSprite(m_eCurrentTab);
			if (s == null)
				return;
			s.ImportBitmap(b);
		}

		private void menuTest_ShowUndoHistory_Click(object sender, EventArgs e)
		{
			menuTest_ShowUndoHistory.Checked = !menuTest_ShowUndoHistory.Checked;
			UndoMgr.ShowDebugWindow = menuTest_ShowUndoHistory.Checked;
		}

	}
}
