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
		public string AppName = "Spritely";

		private ProjectMainForm m_project;
		private Document m_doc;
		private RecentFiles m_recent;

		private Toolbox_Sprite m_SpriteToolbox;
		private Toolbox_Sprite m_BackgroundSpriteToolbox;
		private Toolbox_Map m_BackgroundMapToolbox;

		/// <summary>
		/// Info about each of the tabs in the form.
		/// </summary>
		private Tab[] m_tabs;

		/// <summary>
		/// Currently visible tab.
		/// </summary>
		private Tab m_tabCurrent;

		public MainForm(string strFilename)
		{
			bool fNewDocument = true;

			InitializeComponent();

			m_SpriteToolbox = new Toolbox_Sprite();
			m_BackgroundSpriteToolbox = new Toolbox_Sprite();
			m_BackgroundMapToolbox = new Toolbox_Map();

			// Create tabs
			m_tabs = new Tab[(int)Tab.Type.MAX];
			InitTabs();
			m_tabCurrent = GetTab(Tab.Type.Sprites);

			// Init the list of recent files.
			m_recent = new RecentFiles(this, menuFile_RecentFiles);

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

			Handle_AllSpritesChanged();

			// Set edit zoom level to 16 pixels
			cbS_Zoom.SelectedIndex = 4;
			AdjustAllSpriteListScrollbars();
			UpdatePaletteColor(GetTab(Tab.Type.Sprites));

			cbBS_Zoom.SelectedIndex = 4;
			AdjustAllBackgroundSpriteListScrollbars();
			UpdatePaletteColor(GetTab(Tab.Type.BackgroundSprites));

			// Clear out the Undo stack to remove the default sprites.
			m_doc.ResetUndo();

			// Create the new UI form.
			m_project = new ProjectMainForm(m_doc);
		}

		/// <summary>
		/// Initialize the tab structures for this form with links to the appropriate
		/// controls for each tab.
		/// </summary>
		private void InitTabs()
		{
			Tab tab;
			
			tab = new Tab(Tab.Type.Sprites, this);
			tab.PaletteWindow = pbS_Palette;
			tab.PaletteSelectWindow = pbS_PaletteSelect;
			tab.PaletteSwatchWindow = pbS_PaletteSwatch;
			tab.RedLabel = lS_RHex;
			tab.GreenLabel = lS_GHex;
			tab.BlueLabel = lS_BHex;
			tab.RedScrollbar = sbS_Red;
			tab.GreenScrollbar = sbS_Green;
			tab.BlueScrollbar = sbS_Blue;
			tab.SpriteListWindow = pbS_SpriteList;
			tab.SpriteListScrollbar = sbS_SpriteList;
			tab.EditSpriteWindow = pbS_EditSprite;
			tab.EditMapWindow = null;
			tab.Toolbox = m_SpriteToolbox;
			tab.ToolboxWindow = pbS_Toolbox;
			tab.ToolboxZoomCombobox = cbS_Zoom;
			m_tabs[(int)Tab.Type.Sprites] = tab;

			tab = new Tab(Tab.Type.BackgroundSprites, this);
			tab.PaletteWindow = pbBS_Palette;
			tab.PaletteSelectWindow = pbBS_PaletteSelect;
			tab.PaletteSwatchWindow = pbBS_PaletteSwatch;
			tab.RedLabel = lBS_RHex;
			tab.GreenLabel = lBS_GHex;
			tab.BlueLabel = lBS_BHex;
			tab.RedScrollbar = sbBS_Red;
			tab.GreenScrollbar = sbBS_Green;
			tab.BlueScrollbar = sbBS_Blue;
			tab.SpriteListWindow = pbBS_SpriteList;
			tab.SpriteListScrollbar = sbBS_SpriteList;
			tab.EditSpriteWindow = pbBS_EditSprite;
			tab.EditMapWindow = null;
			tab.Toolbox = m_BackgroundSpriteToolbox;
			tab.ToolboxWindow = pbBS_Toolbox;
			tab.ToolboxZoomCombobox = cbBS_Zoom;
			m_tabs[(int)Tab.Type.BackgroundSprites] = tab;

			tab = new Tab(Tab.Type.BackgroundMap, this);
			tab.PaletteWindow = pbBM_Palette;
			tab.PaletteSelectWindow = pbBM_PaletteSelect;
			tab.PaletteSwatchWindow = null;
			tab.RedLabel = null;
			tab.GreenLabel = null;
			tab.BlueLabel = null;
			tab.RedScrollbar = null;
			tab.GreenScrollbar = null;
			tab.BlueScrollbar = null;
			tab.SpriteListWindow = pbBM_SpriteList;
			tab.SpriteListScrollbar = sbBM_SpriteList;
			tab.EditSpriteWindow = null;
			tab.EditMapWindow = pbBM_EditBackgroundMap;
			tab.Toolbox = m_BackgroundMapToolbox;
			tab.ToolboxWindow = pbBM_Toolbox;
			tab.ToolboxZoomCombobox = null;
			m_tabs[(int)Tab.Type.BackgroundMap] = tab;
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			// This allows the form to preview the key before it is passed to the control.
			this.KeyPreview = true;

			/*
			// Create the ToolTips.
			ToolTip tt = new ToolTip();
			tt.AutoPopDelay = 5000;
			tt.InitialDelay = 1000;
			tt.ReshowDelay = 500;

			tt.SetToolTip(this.pbS_SpriteList, "List of currently defined sprites");
			tt.SetToolTip(this.gbS_SpritePalette, "The default palette for the current sprite");
			 */
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			SpriteList sl;
			Sprite s;
			Tab tab = m_tabCurrent;
			switch (keyData)
			{
				case Keys.Alt | Keys.Left:
				case Keys.Alt | Keys.Up:
					sl = tab.SpriteList;;
					s = sl.PrevSprite(sl.CurrentSprite);
					if (s != null)
					{
						sl.CurrentSprite = s;
						Handle_SpritesChanged(tab);
					}
					return true;
				case Keys.Alt | Keys.Right:
				case Keys.Alt | Keys.Down:
					sl = tab.SpriteList;
					s = sl.NextSprite(sl.CurrentSprite);
					if (s != null)
					{
						sl.CurrentSprite = s;
						Handle_SpritesChanged(tab);
					}
					return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void Handle_NewDocument()
		{
			m_doc = new Document(this);
			m_doc.InitializeEmptyDocument();

			Handle_AllSpritesChanged();
			SetTitleBar("Untitled");
		}

		private void Handle_AllSpritesChanged()
		{
			Handle_SpritesChanged(GetTab(Tab.Type.Sprites));
			Handle_SpritesChanged(GetTab(Tab.Type.BackgroundSprites));
			Handle_SpritesChanged(GetTab(Tab.Type.BackgroundMap));
		}

		private void Handle_SpritesChanged(Tab tab)
		{
			if (tab.Spritesets.Current.CurrentSprite != null)
				tab.Palettes.CurrentPalette.CurrentSubpaletteID = tab.Spritesets.Current.CurrentSprite.PaletteID;

			// Updating the palette causes a cascade of updates that results in the sprites and
			// bg maps being updated.
			UpdatePaletteColor(tab);

			UpdateSpriteInfo(tab);
			ActivateMenuItems();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!m_doc.Close())
				e.Cancel = true;
		}

		public Document Doc
		{
			get { return m_doc; }
		}

		public ProjectMainForm NewUI
		{
			get { return m_project; }
		}

		public Tab GetTab(Tab.Type tab)
		{
			return m_tabs[(int)tab];
		}

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
			DialogResult result = MessageBox.Show(str, AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
			if (result == DialogResult.Cancel)
				fCancel = true;
			if (result == DialogResult.Yes)
				return true;
			return false;
		}

		private void SetTitleBar(string strFilename)
		{
			this.Text = String.Format("{0} - {1}", AppName, strFilename);
		}

		#region TabSet

		public Tab CurrentTab
		{
			get { return m_tabCurrent; }
		}

		private void tabSet_SelectedIndexChanged(object sender, EventArgs e)
		{
			Tab.Type tabNew = (Tab.Type)tabSet.SelectedIndex;
			switch (tabNew)
			{
				case Tab.Type.Sprites:
					break;
				case Tab.Type.BackgroundSprites:
					break;
				case Tab.Type.BackgroundMap:
					UpdateBackgroundMapPalette();
					break;
			}
			m_tabCurrent = GetTab(tabNew);

			UpdateZoom(m_tabCurrent);
			ActivateMenuItems();
			UndoMgr.LoadDebugWindow(m_doc.Undo());
		}

		private void tabSet_KeyPress(object sender, KeyPressEventArgs e)
		{
			Tab tab = m_tabCurrent;
			if (tab.TabType != Tab.Type.Sprites && tab.TabType != Tab.Type.BackgroundSprites)
				return;

			char ch = e.KeyChar;
			if (ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f')
			{
				Subpalette p = tab.Palettes.CurrentPalette.CurrentSubpalette;
				int nIndex = ch - '0';
				if (nIndex > 9)
					nIndex -= ('a' - '0' - 10);
				p.CurrentColor = nIndex;
				UpdatePaletteSelect(tab);
				//p.RecordUndoAction("key select");
				e.Handled = true;
			}
			else if (ch == 'p')
			{
				tab.Toolbox.CurrentTool = Toolbox.ToolType.Pencil;
				tab.ToolboxWindow.Invalidate();
			}
			else if (ch == 'g')
			{
				tab.Toolbox.CurrentTool = Toolbox.ToolType.FloodFill;
				tab.ToolboxWindow.Invalidate();
			}
			else if (ch == 'i')
			{
				tab.Toolbox.CurrentTool = Toolbox.ToolType.Eyedropper;
				tab.ToolboxWindow.Invalidate();
			}
			else if (ch == 'x')
			{
				tab.Toolbox.CurrentTool = Toolbox.ToolType.Eraser;
				tab.ToolboxWindow.Invalidate();
			}
			else if (ch == 'k')
			{
				Options.Palette_ShowPaletteIndex = !Options.Palette_ShowPaletteIndex;
				tab.PaletteWindow.Invalidate();
				tab.PaletteSwatchWindow.Invalidate();
			}
		}

		#endregion

	}
}
