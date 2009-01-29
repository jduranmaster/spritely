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
		public string AppName = "Spritely";

		private Document m_doc;

		private ProjectMainForm m_newui;

		private Toolbox_Sprite m_SpriteToolbox;
		private Toolbox_Sprite m_BackgroundSpriteToolbox;
		private Toolbox_Map m_BackgroundMapToolbox;

		/// <summary>
		/// Info about each of the tabs in the form.
		/// </summary>
		private OldTab[] m_tabs;

		/// <summary>
		/// Currently visible tab.
		/// </summary>
		private OldTab m_tabCurrent;

		public OldMainForm(Document doc, ProjectMainForm newui)
		{
			m_doc = doc;
			m_newui = newui;

			InitializeComponent();

			m_SpriteToolbox = new Toolbox_Sprite();
			m_BackgroundSpriteToolbox = new Toolbox_Sprite();
			m_BackgroundMapToolbox = new Toolbox_Map();

			// Create tabs
			m_tabs = new OldTab[(int)OldTab.Type.MAX];
			InitTabs();
			m_tabCurrent = GetTab(OldTab.Type.Sprites);
		}

		/// <summary>
		/// Initialize the tab structures for this form with links to the appropriate
		/// controls for each tab.
		/// </summary>
		private void InitTabs()
		{
			OldTab tab;
			
			tab = new OldTab(OldTab.Type.Sprites, this);
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
			m_tabs[(int)OldTab.Type.Sprites] = tab;

			tab = new OldTab(OldTab.Type.BackgroundSprites, this);
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
			m_tabs[(int)OldTab.Type.BackgroundSprites] = tab;

			tab = new OldTab(OldTab.Type.BackgroundMap, this);
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
			m_tabs[(int)OldTab.Type.BackgroundMap] = tab;
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			Handle_AllSpritesChanged();

			// Set edit zoom level to 16 pixels
			cbS_Zoom.SelectedIndex = 4;
			AdjustAllSpriteListScrollbars();
			UpdatePaletteColor(GetTab(OldTab.Type.Sprites));

			cbBS_Zoom.SelectedIndex = 4;
			AdjustAllBackgroundSpriteListScrollbars();
			UpdatePaletteColor(GetTab(OldTab.Type.BackgroundSprites));

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
			OldTab tab = m_tabCurrent;
			switch (keyData)
			{
				case Keys.Alt | Keys.Left:
				case Keys.Alt | Keys.Up:
					sl = tab.SpriteList;;
					s = null;// sl.PrevSprite(sl.CurrentSprite);
					if (s != null)
					{
						//sl.CurrentSprite = s;
						Handle_SpritesChanged(tab);
					}
					return true;
				case Keys.Alt | Keys.Right:
				case Keys.Alt | Keys.Down:
					sl = tab.SpriteList;
					s = null;// sl.NextSprite(sl.CurrentSprite);
					if (s != null)
					{
						//sl.CurrentSprite = s;
						Handle_SpritesChanged(tab);
					}
					return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void Handle_NewDocument()
		{
			m_doc = new Document(m_newui);
			m_doc.InitializeEmptyDocument();

			Handle_AllSpritesChanged();
			SetTitleBar("Untitled");
		}

		private void Handle_AllSpritesChanged()
		{
			Handle_SpritesChanged(GetTab(OldTab.Type.Sprites));
			Handle_SpritesChanged(GetTab(OldTab.Type.BackgroundSprites));
			Handle_SpritesChanged(GetTab(OldTab.Type.BackgroundMap));
		}

		private void Handle_SpritesChanged(OldTab tab)
		{
			//if (tab.Spritesets.Current.CurrentSprite != null)
			//	tab.Palettes.CurrentPalette.CurrentSubpaletteID = tab.Spritesets.Current.CurrentSprite.PaletteID;

			// Updating the palette causes a cascade of updates that results in the sprites and
			// bg maps being updated.
			UpdatePaletteColor(tab);

			UpdateSpriteInfo(tab);
			ActivateMenuItems();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
		}

		public Document Doc
		{
			get { return m_doc; }
		}

		public OldTab GetTab(OldTab.Type tab)
		{
			return m_tabs[(int)tab];
		}

		private void SetTitleBar(string strFilename)
		{
			this.Text = String.Format("{0} - {1}", AppName, strFilename);
		}

		#region TabSet

		public OldTab CurrentTab
		{
			get { return m_tabCurrent; }
		}

		private void tabSet_SelectedIndexChanged(object sender, EventArgs e)
		{
			OldTab.Type tabNew = (OldTab.Type)tabSet.SelectedIndex;
			switch (tabNew)
			{
				case OldTab.Type.Sprites:
					break;
				case OldTab.Type.BackgroundSprites:
					break;
				case OldTab.Type.BackgroundMap:
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
			OldTab tab = m_tabCurrent;
			if (tab.TabType != OldTab.Type.Sprites && tab.TabType != OldTab.Type.BackgroundSprites)
				return;

			char ch = e.KeyChar;
			if (ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f')
			{
				//Subpalette p = null;// tab.Palettes.CurrentPalette.CurrentSubpalette;
				int nIndex = ch - '0';
				if (nIndex > 9)
					nIndex -= ('a' - '0' - 10);
				//p.CurrentColor = nIndex;
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
