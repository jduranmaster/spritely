using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class Document
	{
		private MainForm m_form;

		private struct DocumentData
		{
			public Palettes Palettes;
			public Spritesets Spritesets;
			public Palettes BackgroundPalettes;
			public Spritesets BackgroundSpritesets;
			public Maps BackgroundMaps;
			public FileHandler Filer;
		}
		DocumentData m_data;

		/// <summary>
		/// A copy of the BackgroundSpritePalettes that is used when editing the BackgroundMap
		/// to select the palette override for the tiles in the background.
		/// </summary>
		private Palette BackgroundMapPalette;

		/// <summary>
		/// Undo managers for each tab.
		/// </summary>
		public UndoMgr[] m_Undo;

		/// <summary>
		/// Initialize a Spritely document.
		/// </summary>
		/// <param name="form">The display form that owns this document</param>
		public Document(MainForm form)
		{
			m_form = form;

			m_data.Palettes = new Palettes(this, Palettes.Type.Sprite);
			m_data.Spritesets = new Spritesets(this, false);
			m_data.BackgroundPalettes = new Palettes(this, Palettes.Type.Background);
			m_data.BackgroundSpritesets = new Spritesets(this, true);
			m_data.BackgroundMaps = new Maps(this);

			m_data.Filer = new FileHandler(this);

			int numTabs = (int)MainForm.Tab.MAX;
			m_Undo = new UndoMgr[numTabs];
			for (int iTab = 0; iTab < numTabs; iTab++)
				m_Undo[iTab] = new UndoMgr();

			// TODO: remove this, share BG palettes
			BackgroundMapPalette = new Palette(this, null, "bgmpal", -1, "");
			BackgroundMapPalette.HilightSelectedColor = false;
		}

		/// <summary>
		/// Add a default sprite to the new document when Spritely is first launched.
		/// This is so that first-time users don't see an empty window - they can 
		/// start editing their first sprite immediately.
		/// </summary>
		public void InitializeEmptyDocumnt()
		{
			// Palettes
			Palette pal = m_data.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
			pal.SetDefaultPalette();

			// Spritesets
			m_data.Spritesets.AddSpriteset(Options.DefaultSpritesetName, Options.DefaultPaletteId, "", pal);

			// Add a single 2x2 sprite.
			m_data.Spritesets.Current.AddSprite(2, 2, "", -1, "", false);

			// Background palettes
			Palette bgpal = m_data.BackgroundPalettes.AddPalette16(Options.DefaultBgPaletteName, 0, "");
			bgpal.SetDefaultPalette();

			// Background tiles (bgsprites and bgtilegroups)
			m_data.BackgroundSpritesets.AddSpriteset(Options.DefaultBgPaletteName, Options.DefaultBgPaletteId, "", bgpal);

			// Add a single blank background tile.
			m_data.BackgroundSpritesets.Current.AddSprite(1, 1, "", -1, "", false);

			// Background tile map
			m_data.BackgroundMaps.AddMap("map", 0, "", m_data.BackgroundSpritesets.Current);
		}

		/// <summary>
		/// Display an error message using a hard-coded string.
		/// All calls to this routine should be converted to use ErrorId()
		/// so that they can be localized.
		/// </summary>
		/// <param name="strFormat">Hard-coded error message string</param>
		/// <param name="args">Arguments to be folded into the error string</param>
		public void ErrorString(string strFormat, params object[] args)
		{
			string strMessage = String.Format(strFormat, args);
			if (m_form == null)
				System.Console.WriteLine(ResourceMgr.GetString("ERROR") + strMessage);
			else
				m_form.Error(strMessage);
		}

		/// <summary>
		/// Display an error string using the given message-id.
		/// </summary>
		/// <param name="strMessageId">The name (message-id) of the string so display</param>
		/// <param name="args">Arguments to be folded into the error string</param>
		public void ErrorId(string strMessageId, params object[] args)
		{
			string strFormat = ResourceMgr.GetString(strMessageId);
			string strMessage = String.Format(strFormat, args);
			if (m_form == null)
				System.Console.WriteLine(ResourceMgr.GetString("ERROR") + strMessage);
			else
				m_form.Error(strMessage);
		}

		/// <summary>
		/// Display a warning message using a hard-coded string.
		/// All calls to this routine should be converted to use WarningId()
		/// so that they can be localized.
		/// </summary>
		/// <param name="strFormat">Hard-coded warning message string</param>
		/// <param name="args">Arguments to be folded into the warning string</param>
		public void WarningString(string strFormat, params object[] args)
		{
			string strMessage = String.Format(strFormat, args);
			if (m_form == null)
				System.Console.WriteLine(ResourceMgr.GetString("WARNING") + strMessage);
			else
				m_form.Warning(strMessage);
		}

		/// <summary>
		/// Display a warning string using the given message-id.
		/// </summary>
		/// <param name="strMessageId">The name (message-id) of the string so display</param>
		/// <param name="args">Arguments to be folded into the warning string</param>
		public void WarningId(string strMessageId, params object[] args)
		{
			string strFormat = ResourceMgr.GetString(strMessageId);
			string strMessage = String.Format(strFormat, args);
			if (m_form == null)
				System.Console.WriteLine(ResourceMgr.GetString("WARNING") + strMessage);
			else
				m_form.Warning(strMessage);
		}

		/// <summary>
		/// Pose a Yes/No question to the user.
		/// </summary>
		/// <param name="strMessage">Question to ask</param>
		/// <returns>True = yes; False = no</returns>
		public bool AskYesNo(string strMessage)
		{
			if (m_form == null)
			{
				System.Console.WriteLine("AskYesNo: " + strMessage);
				return false;
			}
			else
				return m_form.AskYesNo(strMessage);
		}

		/// <summary>
		/// Pose a Yes/No question to the user, allowing them to cancel.
		/// </summary>
		/// <param name="strMessage">Question to ask</param>
		/// <param name="fCancel">True is the user cancelled</param>
		/// <returns>True = yes; False = no</returns>
		public bool AskYesNoCancel(string strMessage, out bool fCancel)
		{
			if (m_form == null)
			{
				System.Console.WriteLine("AskYesNoCancel: " + strMessage);
				fCancel = false;
				return false;
			}
			else
				return m_form.AskYesNoCancel(strMessage, out fCancel);
		}

		/// <summary>
		/// The form that owns this document.
		/// </summary>
		public MainForm Owner
		{
			get { return m_form; }
		}

		/// <summary>
		/// The name of the file used to store this document.
		/// </summary>
		public string Name
		{
			get { return m_data.Filer.Filename; }
		}

		/// <summary>
		/// The filer used to manage loads/saves for this document.
		/// </summary>
		public FileHandler Filer
		{
			get { return m_data.Filer; }
		}

		/// <summary>
		/// Foreground spritesets.
		/// </summary>
		public Spritesets Spritesets
		{
			get { return m_data.Spritesets; }
		}

		/// <summary>
		/// Background spritesets (tilesets).
		/// </summary>
		public Spritesets BackgroundSpritesets
		{
			get { return m_data.BackgroundSpritesets; }
		}

		public Spriteset GetSpriteset(MainForm.Tab tab)
		{
			if (tab == MainForm.Tab.Sprites)
				return m_data.Spritesets.Current;
			if (tab == MainForm.Tab.BackgroundSprites)
				return m_data.BackgroundSpritesets.Current;
			if (tab == MainForm.Tab.BackgroundMap)
				return m_data.BackgroundSpritesets.Current;
			return null;
		}

		public SpriteList GetSprites(MainForm.Tab tab)
		{
			if (tab == MainForm.Tab.Sprites)
				return m_data.Spritesets.Current.SpriteList;
			if (tab == MainForm.Tab.BackgroundSprites)
				return m_data.BackgroundSpritesets.Current.SpriteList;
			if (tab == MainForm.Tab.BackgroundMap)
				return m_data.BackgroundSpritesets.Current.SpriteList;
			return null;
		}

		public Sprite GetCurrentSprite(MainForm.Tab tab)
		{
			if (tab == MainForm.Tab.Sprites)
				return m_data.Spritesets.Current.CurrentSprite;
			if (tab == MainForm.Tab.BackgroundSprites)
				return m_data.BackgroundSpritesets.Current.CurrentSprite;
			if (tab == MainForm.Tab.BackgroundMap)
				return m_data.BackgroundSpritesets.Current.CurrentSprite;
			return null;
		}

		/// <summary>
		/// Foreground (sprite) palettes.
		/// </summary>
		public Palettes Palettes
		{
			get { return m_data.Palettes; }
		}

		/// <summary>
		/// Background palettes.
		/// </summary>
		public Palettes BackgroundPalettes
		{
			get { return m_data.BackgroundPalettes; }
		}

		public Palette GetSpritePalette(int id)
		{
			return m_data.Palettes.GetPalette(id);
		}

		public Palette GetBackgroundPalette(int id)
		{
			return m_data.BackgroundPalettes.GetPalette(id);
		}

		public Palette GetBackgroundMapPalette()
		{
			return BackgroundMapPalette;
		}

		/// <summary>
		/// Get the sprite palette for the current tab.
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		public Palette GetSpritePalette(MainForm.Tab tab)
		{
			if (tab == MainForm.Tab.Sprites)
				return GetSpritePalette(Options.DefaultPaletteId);
			if (tab == MainForm.Tab.BackgroundSprites)
				return GetBackgroundPalette(Options.DefaultBgPaletteId);
			if (tab == MainForm.Tab.BackgroundMap)
				return BackgroundMapPalette;
			return null;
		}

		public Maps BackgroundMaps
		{
			get { return m_data.BackgroundMaps; }
		}

		/// <summary>
		/// Are there unsaved changes for this documnt?
		/// </summary>
		public bool HasUnsavedChanges
		{
			get { return m_data.Filer.HasUnsavedChanges; }
			set { m_data.Filer.HasUnsavedChanges = value; }
		}

		/// <summary>
		/// Return the current Undo manager.
		/// </summary>
		/// <returns></returns>
		public UndoMgr Undo()
		{
			return m_Undo[(int)m_form.CurrentTab];
		}

		/// <summary>
		/// Clear out all the info from all of the Undo buffers.
		/// </summary>
		public void ResetUndo()
		{
			for (int i=0; i<(int)MainForm.Tab.MAX; ++i)
				m_Undo[i].Reset();
		}

		/// <summary>
		/// Flush all the bitmaps for all of the spritesets so that they can be
		/// regenerated.
		/// </summary>
		public void FlushBitmaps()
		{
			m_data.Spritesets.FlushBitmaps();
			m_data.BackgroundSpritesets.FlushBitmaps();
		}

		#region File

		/// <summary>
		/// Open a new file using the File open dialog.
		/// </summary>
		/// <returns>True if a new file was successfully opened</returns>
		public bool Open()
		{
			// Open into a separate doc so that we can keep the current doc untouched in case
			// there is an error.
			Document doc = new Document(m_form);
			if (!doc.m_data.Filer.OpenFile())
				return false;

			Open_(doc);
			return true;
		}

		/// <summary>
		/// Open the named file.
		/// </summary>
		/// <param name="strFilename">Name of file to open</param>
		/// <returns>True if the file was successfully opened</returns>
		public bool Open(string strFilename)
		{
			// Open into a separate doc so that we can keep the current doc untouched in case
			// there is an error.
			Document doc = new Document(m_form);
			if (!doc.m_data.Filer.OpenFile(strFilename))
				return false;

			Open_(doc);
			return true;
		}

		/// <summary>
		/// Copy the newly opened Document into the current document.
		/// </summary>
		/// <param name="doc">The newly opened document</param>
		private void Open_(Document doc)
		{
			// Copy data from newly loaded doc into this doc
			m_data = doc.m_data;
			// Update the document references to point to this document
			m_data.Palettes.UpdateDocument(this);
			m_data.Spritesets.UpdateDocument(this);
			m_data.BackgroundPalettes.UpdateDocument(this);
			m_data.BackgroundSpritesets.UpdateDocument(this);
			m_data.BackgroundMaps.UpdateDocument(this);
			m_data.Filer.UpdateDocument(this);

			Spriteset ss = m_data.Spritesets.Current;
			if (ss != null)
				ss.SelectFirstSprite();
			Spriteset ts = m_data.BackgroundSpritesets.Current;
			if (ts != null)
				ts.SelectFirstSprite();
			ResetUndo();
		}

		public bool Close()
		{
			return m_data.Filer.Close();
		}

		public void Save()
		{
			m_data.Filer.SaveFile();
		}

		public void SaveAs()
		{
			m_data.Filer.SaveFileAs();
		}

		#endregion

		#region Export

		/// <summary>
		/// Export this document/project as source code.
		/// </summary>
		public void Export()
		{
			string strProjectDir;
			bool fNDS, fProject;
			FileHandler.ExportResult result = m_data.Filer.Export(Name, out strProjectDir, out fNDS, out fProject);

			if (result == FileHandler.ExportResult.Cancel)
				return;

			if (result == FileHandler.ExportResult.OK)
			{
				string strTarget = fNDS ? "NDS" : "GBA";
				StringBuilder sb = new StringBuilder("");
				if (fProject)
				{
					// "Successfully exported {0} project to "{1}"."
					sb.Append(String.Format(ResourceMgr.GetString("ExportProjectSuccess"), strTarget, strProjectDir));
					sb.Append("\r\n");
					// "Go to this directory and run "make" to build your project."
					sb.Append(ResourceMgr.GetString("InstrHowToMake"));
				}
				else
				{
					// "Successfully exported {0} sprite data to "{1}"."
					sb.Append(String.Format(ResourceMgr.GetString("ExportSpritesSuccess"), strTarget, strProjectDir));
				}
				m_form.Info(sb.ToString());

				// The platform may have changed, update the map toolbox to reflect the current platform.
				if (m_form.CurrentTab == MainForm.Tab.BackgroundMap)
					m_form.GetToolboxWindow(MainForm.Tab.BackgroundMap).Invalidate();
			}
			else
			{
				// "Unable to export!"
				ErrorId("ExportFail");
			}
		}

		#endregion

	}
}