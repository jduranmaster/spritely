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

		private Document m_doc;
		private RecentFiles m_recent;

		private Toolbox_Sprite m_SpriteToolbox;
		private Toolbox_Sprite m_BackgroundSpriteToolbox;
		private Toolbox_Map m_BackgroundMapToolbox;

		public enum Tab
		{
			Sprites = 0,
			BackgroundSprites,
			BackgroundMap,
			MAX,
			Unknown,
		};

		/// <summary>
		/// Index of the currently visible tab in the tabset.
		/// </summary>
		private Tab m_eCurrentTab = Tab.Sprites;

		public MainForm(string strFilename)
		{
			bool fNewDocument = true;

			InitializeComponent();

			// Init the list of recent files.
			m_recent = new RecentFiles(this, menuFile_RecentFiles);

			if (strFilename != "")
			{
				m_doc = new Document(this);
				if (m_doc.Open(strFilename))
				{
					fNewDocument = false;
					SetTitleBar(m_doc.Name);
				}
			}

			if (fNewDocument)
				Handle_NewDocument();

			m_SpriteToolbox = new Toolbox_Sprite(this);
			m_BackgroundSpriteToolbox = new Toolbox_Sprite(this);
			m_BackgroundMapToolbox = new Toolbox_Map(this);

			if (fNewDocument)
			{
				// Add a default sprite so that first-time users don't have to add a new
				// sprite in order to get started
				m_doc.AddDefaultSprite();
			}

			Handle_AllSpritesChanged();

			// Set edit zoom level to 16 pixels
			cbS_Zoom.SelectedIndex = 4;
			AdjustSpriteListScrollbar(Tab.Sprites, 0, 0);
			UpdatePaletteColor(Tab.Sprites);

			cbBS_Zoom.SelectedIndex = 4;
			AdjustSpriteListScrollbar(Tab.BackgroundSprites, 0, 0);
			UpdatePaletteColor(Tab.BackgroundSprites);

			AdjustSpriteListScrollbar(Tab.BackgroundMap, 0, 0);

			// Clear out the Undo stack to remove the default sprites.
			m_doc.ResetUndo();
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
			switch (keyData)
			{
				case Keys.Alt | Keys.Left:
				case Keys.Alt | Keys.Up:
					sl = GetSpriteList(m_eCurrentTab);
					s = sl.PrevSprite(sl.CurrentSprite);
					if (s != null)
					{
						sl.CurrentSprite = s;
						Handle_SpritesChanged(m_eCurrentTab);
					}
					return true;
				case Keys.Alt | Keys.Right:
				case Keys.Alt | Keys.Down:
					sl = GetSpriteList(m_eCurrentTab);
					s = sl.NextSprite(sl.CurrentSprite);
					if (s != null)
					{
						sl.CurrentSprite = s;
						Handle_SpritesChanged(m_eCurrentTab);
					}
					return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void Handle_NewDocument()
		{
			m_doc = new Document(this);

			// Add some default palettes
			PaletteMgr spal = m_doc.GetSpritePalettes(Tab.Sprites);
			spal.AddDefaultPalette("Color1", Palette.DefaultColorSet.Color1);
			spal.AddDefaultPalette("Color2", Palette.DefaultColorSet.Color2);
			spal.AddDefaultPalette("Gray", Palette.DefaultColorSet.GrayScale);
			spal.AddDefaultPalette("Blank1", Palette.DefaultColorSet.BlackAndWhite);
			spal.AddDefaultPalette("Blank2", Palette.DefaultColorSet.BlackAndWhite);
			spal.AddDefaultPalette("Blank3", Palette.DefaultColorSet.BlackAndWhite);
			spal.AddDefaultPalette("Blank4", Palette.DefaultColorSet.BlackAndWhite);
			spal.AddDefaultPalette("Blank5", Palette.DefaultColorSet.BlackAndWhite);
			spal.AddDefaultPalette("Blank6", Palette.DefaultColorSet.BlackAndWhite);
			spal.AddDefaultPalette("Blank7", Palette.DefaultColorSet.BlackAndWhite);
			spal.AddDefaultPalette("Blank8", Palette.DefaultColorSet.BlackAndWhite);
			spal.AddDefaultPalette("Blank9", Palette.DefaultColorSet.BlackAndWhite);
			spal.AddDefaultPalette("Blank10", Palette.DefaultColorSet.BlackAndWhite);
			spal.AddDefaultPalette("Blank11", Palette.DefaultColorSet.BlackAndWhite);
			spal.AddDefaultPalette("Blank12", Palette.DefaultColorSet.BlackAndWhite);
			spal.AddDefaultPalette("Blank13", Palette.DefaultColorSet.BlackAndWhite);

			PaletteMgr bspal = m_doc.GetSpritePalettes(Tab.BackgroundSprites);
			bspal.AddDefaultPalette("Color1", Palette.DefaultColorSet.Color1);
			bspal.AddDefaultPalette("Color2", Palette.DefaultColorSet.Color2);
			bspal.AddDefaultPalette("Gray", Palette.DefaultColorSet.GrayScale);
			bspal.AddDefaultPalette("Blank1", Palette.DefaultColorSet.BlackAndWhite);
			bspal.AddDefaultPalette("Blank2", Palette.DefaultColorSet.BlackAndWhite);
			bspal.AddDefaultPalette("Blank3", Palette.DefaultColorSet.BlackAndWhite);
			bspal.AddDefaultPalette("Blank4", Palette.DefaultColorSet.BlackAndWhite);
			bspal.AddDefaultPalette("Blank5", Palette.DefaultColorSet.BlackAndWhite);
			bspal.AddDefaultPalette("Blank6", Palette.DefaultColorSet.BlackAndWhite);
			bspal.AddDefaultPalette("Blank7", Palette.DefaultColorSet.BlackAndWhite);
			bspal.AddDefaultPalette("Blank8", Palette.DefaultColorSet.BlackAndWhite);
			bspal.AddDefaultPalette("Blank9", Palette.DefaultColorSet.BlackAndWhite);
			bspal.AddDefaultPalette("Blank10", Palette.DefaultColorSet.BlackAndWhite);
			bspal.AddDefaultPalette("Blank11", Palette.DefaultColorSet.BlackAndWhite);
			bspal.AddDefaultPalette("Blank12", Palette.DefaultColorSet.BlackAndWhite);
			bspal.AddDefaultPalette("Blank13", Palette.DefaultColorSet.BlackAndWhite);

			Handle_AllSpritesChanged();
			SetTitleBar("Untitled");
		}

		private void Handle_AllSpritesChanged()
		{
			Handle_SpritesChanged(Tab.Sprites);
			Handle_SpritesChanged(Tab.BackgroundSprites);
			Handle_SpritesChanged(Tab.BackgroundMap);
		}

		private void Handle_SpritesChanged(Tab tab)
		{
			if (m_doc.GetCurrentSprite(tab) != null)
				m_doc.GetSpritePalettes(tab).CurrentPaletteID = m_doc.GetCurrentSprite(tab).PaletteID;

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

		public void Info(string str)
		{
			MessageBox.Show(str, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public void NYI()
		{
			//MessageBox.Show("Sorry - Not Yet Implemented", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
			get { return m_eCurrentTab; }
		}

		private void tabSet_SelectedIndexChanged(object sender, EventArgs e)
		{
			Tab tabNew = (Tab)tabSet.SelectedIndex;
			switch (tabNew)
			{
				case Tab.Sprites:
					break;
				case Tab.BackgroundSprites:
					break;
				case Tab.BackgroundMap:
					UpdateBackgroundMapPalette();
					break;
			}
			m_eCurrentTab = tabNew;

			UpdateZoom(tabNew);
			ActivateMenuItems();
			UndoMgr.LoadDebugWindow(m_doc.Undo());
		}

		private void tabSet_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (m_eCurrentTab != Tab.Sprites && m_eCurrentTab != Tab.BackgroundSprites)
				return;

			char ch = e.KeyChar;
			if (ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f')
			{
				Palette p = m_doc.GetSpritePalettes(m_eCurrentTab).CurrentPalette;
				int nIndex = ch - '0';
				if (nIndex > 9)
					nIndex -= ('a' - '0' - 10);
				p.CurrentColor = nIndex;
				UpdatePaletteSelect(m_eCurrentTab);
				//p.RecordUndoAction("key select");
				e.Handled = true;
			}
			else if (ch == 'p')
			{
				GetToolbox(m_eCurrentTab).CurrentTool = Toolbox.ToolType.Pencil;
				GetToolboxWindow(m_eCurrentTab).Invalidate();
			}
			else if (ch == 'g')
			{
				GetToolbox(m_eCurrentTab).CurrentTool = Toolbox.ToolType.FloodFill;
				GetToolboxWindow(m_eCurrentTab).Invalidate();
			}
			else if (ch == 'i')
			{
				GetToolbox(m_eCurrentTab).CurrentTool = Toolbox.ToolType.Eyedropper;
				GetToolboxWindow(m_eCurrentTab).Invalidate();
			}
			else if (ch == 'x')
			{
				GetToolbox(m_eCurrentTab).CurrentTool = Toolbox.ToolType.Eraser;
				GetToolboxWindow(m_eCurrentTab).Invalidate();
			}
			else if (ch == 'k')
			{
				Options.Palette_ShowPaletteIndex = !Options.Palette_ShowPaletteIndex;
				GetPaletteWindow(m_eCurrentTab).Invalidate();
				GetPaletteSwatchWindow(m_eCurrentTab).Invalidate();
			}
		}

		#endregion

		#region Tab <-> Control Mapping

		private Tab GetTab_SpriteList(PictureBox pbox)
		{
			if (pbox == pbS_SpriteList)
				return Tab.Sprites;
			if (pbox == pbBS_SpriteList)
				return Tab.BackgroundSprites;
			if (pbox == pbBM_SpriteList)
				return Tab.BackgroundMap;
			return Tab.Unknown;
		}

		private Tab GetTab_SpriteListScrollbar(VScrollBar sb)
		{
			if (sb == sbS_SpriteList)
				return Tab.Sprites;
			if (sb == sbBS_SpriteList)
				return Tab.BackgroundSprites;
			if (sb == sbBM_SpriteList)
				return Tab.BackgroundMap;
			return Tab.Unknown;
		}

		private Tab GetTab_EditSprite(PictureBox pbox)
		{
			if (pbox == pbS_EditSprite)
				return Tab.Sprites;
			if (pbox == pbBS_EditSprite)
				return Tab.BackgroundSprites;
			return Tab.Unknown;
		}

		private Tab GetTab_Toolbox(PictureBox pbox)
		{
			if (pbox == pbS_Toolbox)
				return Tab.Sprites;
			if (pbox == pbBS_Toolbox)
				return Tab.BackgroundSprites;
			if (pbox == pbBM_Toolbox)
				return Tab.BackgroundMap;
			return Tab.Unknown;
		}

		private Tab GetTab_ToolboxZoom(ComboBox cb)
		{
			if (cb == cbS_Zoom)
				return Tab.Sprites;
			if (cb == cbBS_Zoom)
				return Tab.BackgroundSprites;
			return Tab.Unknown;
		}

		private Tab GetTab_Palette(PictureBox pbox)
		{
			if (pbox == pbS_Palette)
				return Tab.Sprites;
			if (pbox == pbBS_Palette)
				return Tab.BackgroundSprites;
			if (pbox == pbBM_Palette)
				return Tab.BackgroundMap;
			return Tab.Unknown;
		}

		private Tab GetTab_PaletteSelect(PictureBox pbox)
		{
			if (pbox == pbS_PaletteSelect)
				return Tab.Sprites;
			if (pbox == pbBS_PaletteSelect)
				return Tab.BackgroundSprites;
			if (pbox == pbBM_PaletteSelect)
				return Tab.BackgroundMap;
			return Tab.Unknown;
		}

		private Tab GetTab_PaletteSwatch(PictureBox pbox)
		{
			if (pbox == pbS_PaletteSwatch)
				return Tab.Sprites;
			if (pbox == pbBS_PaletteSwatch)
				return Tab.BackgroundSprites;
			// BackgroundMap doesn't have a palette swatch
			return Tab.Unknown;
		}

		private SpriteList GetSpriteList(Tab tab)
		{
			return m_doc.GetSprites(tab);
		}

		private PictureBox GetSpriteListWindow(Tab tab)
		{
			if (tab == Tab.Sprites)
				return pbS_SpriteList;
			if (tab == Tab.BackgroundSprites)
				return pbBS_SpriteList;
			if (tab == Tab.BackgroundMap)
				return pbBM_SpriteList;
			return null;
		}

		private VScrollBar GetSpriteListScrollbar(Tab tab)
		{
			if (tab == Tab.Sprites)
				return sbS_SpriteList;
			if (tab == Tab.BackgroundSprites)
				return sbBS_SpriteList;
			if (tab == Tab.BackgroundMap)
				return sbBM_SpriteList;
			return null;
		}

		private PictureBox GetEditSpriteWindow(Tab tab)
		{
			if (tab == Tab.Sprites)
				return pbS_EditSprite;
			if (tab == Tab.BackgroundSprites)
				return pbBS_EditSprite;
			// BackgroundMap doesn't have a sprite editor
			return null;
		}

		private PictureBox GetEditMapWindow(Tab tab)
		{
			// Sprites don't have maps.
			if (tab == Tab.BackgroundMap)
				return pbBM_EditBackgroundMap;
			return null;
		}

		public PictureBox GetToolboxWindow(Tab tab)
		{
			if (tab == Tab.Sprites)
				return pbS_Toolbox;
			if (tab == Tab.BackgroundSprites)
				return pbBS_Toolbox;
			if (tab == Tab.BackgroundMap)
				return pbBM_Toolbox;
			return null;
		}

		private PictureBox GetPaletteWindow(Tab tab)
		{
			if (tab == Tab.Sprites)
				return pbS_Palette;
			if (tab == Tab.BackgroundSprites)
				return pbBS_Palette;
			if (tab == Tab.BackgroundMap)
				return pbBM_Palette;
			return null;
		}

		private PictureBox GetPaletteSelectWindow(Tab tab)
		{
			if (tab == Tab.Sprites)
				return pbS_PaletteSelect;
			if (tab == Tab.BackgroundSprites)
				return pbBS_PaletteSelect;
			if (tab == Tab.BackgroundMap)
				return pbBM_PaletteSelect;
			return null;
		}

		private PictureBox GetPaletteSwatchWindow(Tab tab)
		{
			if (tab == Tab.Sprites)
				return pbS_PaletteSwatch;
			if (tab == Tab.BackgroundSprites)
				return pbBS_PaletteSwatch;
			return null;
		}

		private Toolbox GetToolbox(Tab tab)
		{
			if (tab == Tab.Sprites)
				return m_SpriteToolbox;
			if (tab == Tab.BackgroundSprites)
				return m_BackgroundSpriteToolbox;
			if (tab == Tab.BackgroundMap)
				return m_BackgroundMapToolbox;
			return null;
		}

		private ComboBox GetToolboxZoomCombobox(Tab tab)
		{
			if (tab == Tab.Sprites)
				return cbS_Zoom;
			if (tab == Tab.BackgroundSprites)
				return cbBS_Zoom;
			return null;
		}

		private Label GetRedLabel(Tab tab)
		{
			if (tab == Tab.Sprites)
				return lS_RHex;
			if (tab == Tab.BackgroundSprites)
				return lBS_RHex;
			return null;
		}
		
		private Label GetGreenLabel(Tab tab)
		{
			if (tab == Tab.Sprites)
				return lS_GHex;
			if (tab == Tab.BackgroundSprites)
				return lBS_GHex;
			return null;
		}
		
		private Label GetBlueLabel(Tab tab)
		{
			if (tab == Tab.Sprites)
				return lS_BHex;
			if (tab == Tab.BackgroundSprites)
				return lBS_BHex;
			return null;
		}

		private HScrollBar GetRedScrollbar(Tab tab)
		{
			if (tab == Tab.Sprites)
				return sbS_Red;
			if (tab == Tab.BackgroundSprites)
				return sbBS_Red;
			return null;
		}

		private HScrollBar GetGreenScrollbar(Tab tab)
		{
			if (tab == Tab.Sprites)
				return sbS_Green;
			if (tab == Tab.BackgroundSprites)
				return sbBS_Green;
			return null;
		}

		private HScrollBar GetBlueScrollbar(Tab tab)
		{
			if (tab == Tab.Sprites)
				return sbS_Blue;
			if (tab == Tab.BackgroundSprites)
				return sbBS_Blue;
			return null;
		}

		#endregion

	}
}
