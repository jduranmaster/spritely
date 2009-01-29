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
		public string AppName = "Spritely";

		private Document m_doc;
		private RecentFiles m_recent;

		private OldMainForm m_oldui;

		private TabMgr[] m_tabs;
		private TabMgr m_tabCurrent;

		public ProjectMainForm(string strFilename)
		{
			bool fNewDocument = true;

			InitializeComponent();

			// If we were given a filename, than open the specified file.
			if (strFilename != "")
			{
				m_doc = new Document(this);
				if (m_doc.Open(strFilename))
				{
					fNewDocument = false;
					SetTitleBar(m_doc.Name);
				}
			}

			// otherwise, create a brand new (empty) document.
			if (fNewDocument)
				Handle_NewDocument();

			// Clear out the Undo stack to remove the default sprites.
			m_doc.ResetUndo();

			// Init the list of recent files.
			m_recent = new RecentFiles(this, menuFile_RecentFiles);

			m_oldui = new OldMainForm(m_doc, this);
		}

		public OldMainForm OldUI
		{
			get { return m_oldui; }
		}

		public int ContentWidth
		{
			get { return ClientSize.Width - 6; }
		}

		public int ContentHeight
		{
			get { return ClientSize.Height - menuBar.Height - tabSet.Height - 6; }
		}

		private void SetTitleBar(string strFilename)
		{
			this.Text = String.Format("{0} - {1}", AppName, strFilename);
		}

		#region Window events

		private void ProjectMainForm_Load(object sender, EventArgs e)
		{
			m_tabs = new TabMgr[(int)TabMgr.TabId.MAX];

			TabMgr tabSprites = new TabMgr(this, TabMgr.TabId.Sprites);
			m_tabs[(int)TabMgr.TabId.Sprites] = tabSprites;

			TabMgr tabBackgrounds = new TabMgr(this, TabMgr.TabId.Backgrounds);
			m_tabs[(int)TabMgr.TabId.Backgrounds] = tabBackgrounds;

			m_tabCurrent = tabSprites;
			tabSet.SelectedIndex = (int)m_tabCurrent.Id;

			AddProjectMenuItems();
			AddSubwindowsToTabs();
			HandleEverythingChanged();

			m_doc.HasUnsavedChanges = false;
		}

		private void ProjectMainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// The close event has already been sent to the frontmost child window,
			// which hides itself and cancels the Close.
			// This causes 2 problems:
			//   If we show a dialog (e.g., to ask if we need to save changes), then the
			//     front child window will disappear when the dialog is shown.
			//   The main window Close is cancelled.
			// Show all current windows in the tab to undo this
			m_tabCurrent.ShowWindows();
			// Un-cancel the Close so that the main form can close.
			e.Cancel = false;

			// Make sure the document is closed properly.
			if (m_doc != null && !m_doc.Close())
			{
				e.Cancel = true;
				return;
			}
		}

		private void ProjectMainForm_Shown(object sender, EventArgs e)
		{
			m_tabCurrent.ShowWindows();
		}

		#endregion

		#region Document

		public Document Document
		{
			get { return m_doc; }
		}

		private void Handle_NewDocument()
		{
			m_doc = new Document(this);
			m_doc.InitializeEmptyDocument();

			SetTitleBar("Untitled");
		}

		public Palette ActivePalette()
		{
			if (m_doc == null || m_tabCurrent == null)
				return null;

			if (m_tabCurrent.Id == TabMgr.TabId.Sprites)
				return m_doc.Palettes.CurrentPalette;
			else if (m_tabCurrent.Id == TabMgr.TabId.Backgrounds)
				return m_doc.BackgroundPalettes.CurrentPalette;
			return null;
		}

		public Spriteset ActiveSpriteset()
		{
			if (m_doc == null || m_tabCurrent == null)
				return null;

			if (m_tabCurrent.Id == TabMgr.TabId.Sprites)
				return m_doc.Spritesets.Current;
			else if (m_tabCurrent.Id == TabMgr.TabId.Backgrounds)
				return m_doc.BackgroundSpritesets.Current;
			return null;
		}

		public Sprite ActiveSprite()
		{
			if (m_doc == null || m_tabCurrent == null)
				return null;

			if (m_tabCurrent.Id == TabMgr.TabId.Sprites)
				return m_doc.Spritesets.Current.CurrentSprite;
			else if (m_tabCurrent.Id == TabMgr.TabId.Backgrounds)
				return m_doc.BackgroundSpritesets.Current.CurrentSprite;
			return null;
		}

		public Map ActiveMap()
		{
			if (m_doc == null || m_tabCurrent == null)
				return null;

			if (m_tabCurrent.Id == TabMgr.TabId.Sprites)
				return null;
			else if (m_tabCurrent.Id == TabMgr.TabId.Backgrounds)
				return m_doc.BackgroundMaps.CurrentMap;
			return null;
		}

		#endregion

		#region Tabs

		public TabMgr GetTab(TabMgr.TabId id)
		{
			return m_tabs[(int)id];
		}

		public void ShowTabs()
		{
			tabSet.Show();
		}

		public void HideTabs()
		{
			tabSet.Hide();
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
			// Hide windows from old tab.
			m_tabCurrent.HideWindows();

			// Show windws for new tab.
			m_tabCurrent = m_tabs[tabSet.SelectedIndex];
			m_tabCurrent.ShowWindows();
		}

		#endregion

		#region Subwindow Management

		public void ResizeSubwindow(Form f)
		{
			if (f.WindowState == FormWindowState.Maximized)
			{
				// When maximized: hide the tabs and enable the control box.
				// The control box is needed to allow the user to un-minimize the window.
				HideTabs();
				if (!f.ControlBox)
					f.ControlBox = true;
			}
			else
			{
				// Normal display mode: show the tabs and disable the control bar.
				// The control bar shows the min/max/close boxes in the window and
				// we don't want them for our subwindows.
				ShowTabs();
				// Turn off the control box only if it is already turned on.
				// Otherwise, the subwindow will start to shrink mysteriously when
				// you switch tabs.
				if (f.ControlBox)
					f.ControlBox = false;
			}
		}

		/// <summary>
		/// Close (actually, just hide) the specified subwindow.
		/// </summary>
		/// <param name="f"></param>
		public void CloseSubwindow(Form f)
		{
			// Un-maximize the window before closing.
			if (f.WindowState == FormWindowState.Maximized)
			{
				f.WindowState = FormWindowState.Normal;
			}

			// Hide the subwindow instead of closing.
			//f.Hide();
		}

		public void AddSubwindowsToTabs()
		{
			TabMgr tabSprites = GetTab(TabMgr.TabId.Sprites);
			tabSprites.AddSpritesetWindow(m_doc.Spritesets.Current.SpritesetWindow);
			tabSprites.AddSpriteWindow(m_doc.Spritesets.Current.SpriteWindow);
			tabSprites.AddPaletteWindow(m_doc.Palettes.CurrentPalette.PaletteWindow);
			tabSprites.ArrangeWindows();

			TabMgr tabBackgrounds = GetTab(TabMgr.TabId.Backgrounds);
			tabBackgrounds.AddSpritesetWindow(m_doc.BackgroundSpritesets.Current.SpritesetWindow);
			tabBackgrounds.AddSpriteWindow(m_doc.BackgroundSpritesets.Current.SpriteWindow);
			tabBackgrounds.AddPaletteWindow(m_doc.BackgroundPalettes.CurrentPalette.PaletteWindow);
			tabBackgrounds.AddMapWindow(m_doc.BackgroundMaps.CurrentMap.MapWindow);
			tabBackgrounds.ArrangeWindows();
		}

		/// <summary>
		/// When a document is closed, we delete all subwindows.
		/// </summary>
		public void DeleteAllSubwindows()
		{
			GetTab(TabMgr.TabId.Sprites).RemoveAllWindows();
			GetTab(TabMgr.TabId.Backgrounds).RemoveAllWindows();
			GC.Collect();
		}

		/// <summary>
		/// We've just created a new document or loaded one from a file.
		/// Everything has changed, so make sure everything is updated.
		/// </summary>
		public void HandleEverythingChanged()
		{
			HandleSpriteDataChanged(m_doc.Spritesets.Current);
			HandleSpriteTypeChanged(m_doc.Spritesets.Current);
			HandleColorDataChange(m_doc.Palettes.CurrentPalette);

			HandleSpriteDataChanged(m_doc.BackgroundSpritesets.Current);
			HandleSpriteTypeChanged(m_doc.BackgroundSpritesets.Current);
			HandleColorDataChange(m_doc.BackgroundPalettes.CurrentPalette);
			HandleMapDataChange(m_doc.BackgroundMaps.CurrentMap);
		}

		/// <summary>
		/// The current sprite seletion has changed.
		/// </summary>
		public void HandleSpriteSelectionChanged(Spriteset ss)
		{
			m_doc.HasUnsavedChanges = true;
			ss.Palette.PaletteWindow.SpriteSelectionChanged();
			ss.SpritesetWindow.SpriteSelectionChanged();
			ss.SpriteWindow.SpriteSelectionChanged();
		}

		/// <summary>
		/// The data for one (or more) of the sprites has changed.
		/// </summary>
		public void HandleSpriteDataChanged(Spriteset ss)
		{
			m_doc.HasUnsavedChanges = true;
			ss.SpritesetWindow.SpriteDataChanged();
			ss.SpriteWindow.SpriteDataChanged();

			if (ss.IsBackground)
			{
				Map m = ActiveMap();
				if (m != null)
					m.MapWindow.SpriteDataChanged();
			}
		}

		/// <summary>
		/// A new subpalette has been selected, notify all other windows that are potentially
		/// impacted by this change
		/// </summary>
		public void HandleSubpaletteSelectChange(Palette p)
		{
			p.PaletteWindow.SubpaletteSelectChanged();
			Spriteset ss;
			if (p.IsBackground)
			{
				ss = m_doc.BackgroundSpritesets.Current;
				Map m = ActiveMap();
				if (m != null)
					m.MapWindow.SubpaletteSelectChanged();
			}
			else
			{
				ss = m_doc.Spritesets.Current;
			}

			ss.SpritesetWindow.SubpaletteSelectChanged();
			ss.SpriteWindow.SubpaletteSelectChanged();
		}

		/// <summary>
		/// A new color has been selected in the subpalette. Update everyone
		/// who needs to be notified.
		/// </summary>
		public void HandleColorSelectChange(Palette p)
		{
			p.PaletteWindow.ColorSelectChanged();
		}

		/// <summary>
		/// A color value has changed in the current palette. Notify other windows.
		/// </summary>
		public void HandleColorDataChange(Palette p)
		{
			p.PaletteWindow.ColorDataChanged();
		}

		/// <summary>
		/// The map data has changed.
		/// </summary>
		public void HandleMapDataChange(Map m)
		{
			m.MapWindow.MapDataChanged();
		}

		/// <summary>
		/// One of the spritetypes has changed.
		/// E.g., by having a sprite added, deleted or having its tile geometry changed.
		/// </summary>
		public void HandleSpriteTypeChanged(Spriteset ss)
		{
			m_doc.HasUnsavedChanges = true;

			SpritesetForm win = ss.SpritesetWindow;
			if (win != null)
				win.RecalcScrollHeights();
		}

		#endregion
			
		#region User Messaging

		public void Info(string str)
		{
			MessageBox.Show(str, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public void NYI()
		{
			// "Sorry - Not Yet Implemented"
			MessageBox.Show(ResourceMgr.GetString("NYI"), AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public void Warning(string str)
		{
			MessageBox.Show(str, AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		public void Error(string str)
		{
			MessageBox.Show(str, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// Pose a yes/no question to the user
		/// </summary>
		/// <param name="str">The yes/no question to pose to the user</param>
		/// <returns>True if yes, false if no</returns>
		public bool AskYesNo(string str)
		{
			DialogResult result = MessageBox.Show(str, AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.Yes)
				return true;
			return false;
		}

		/// <summary>
		/// Pose a yes/no question to the user, allowing the user to cancel
		/// </summary>
		/// <param name="str">The yes/no question to pose to the user</param>
		/// <param name="fCancel">Flag indicating if the user has cancelled</param>
		/// <returns>True if yes, false if no</returns>
		public bool AskYesNoCancel(string str, out bool fCancel)
		{
			fCancel = false;
			DialogResult result = MessageBox.Show(this, str, AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
			if (result == DialogResult.Cancel)
				fCancel = true;
			if (result == DialogResult.Yes)
				return true;
			return false;
		}

		#endregion

	}
}
