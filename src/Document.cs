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
			public PaletteMgr SpritePalettes;
			public SpriteList SpriteList;
			public PaletteMgr BackgroundSpritePalettes;
			public SpriteList BackgroundSpriteList;
			public BackgroundMap BackgroundMap;
			public FileHandler Filer;
		}
		DocumentData m_data;

		/// <summary>
		/// A copy of the BackgroundSpritePalettes that is used when editing the BackgroundMap
		/// to select the palette override for the tiles in the background.
		/// </summary>
		private PaletteMgr BackgroundMapPalettes;

		/// <summary>
		/// Undo managers for each tab.
		/// </summary>
		public UndoMgr[] m_Undo;

		public Document(MainForm form)
		{
			m_form = form;

			m_data.SpritePalettes = new PaletteMgr(this, false);
			m_data.SpriteList = new SpriteList(this, m_data.SpritePalettes);

			m_data.BackgroundSpritePalettes = new PaletteMgr(this, true);
			m_data.BackgroundSpriteList = new SpriteList(this, m_data.BackgroundSpritePalettes);
			m_data.BackgroundMap = new BackgroundMap(this, m_data.BackgroundSpriteList);

			m_data.Filer = new FileHandler(this);

			m_Undo = new UndoMgr[(int)MainForm.Tab.MAX];
			m_Undo[(int)MainForm.Tab.Sprites] = new UndoMgr();
			m_Undo[(int)MainForm.Tab.BackgroundSprites] = new UndoMgr();
			m_Undo[(int)MainForm.Tab.BackgroundMap] = new UndoMgr();

			BackgroundMapPalettes = new PaletteMgr(this, true);
			BackgroundMapPalettes.HilightSelectedColor = false;
		}

		public void AddDefaultSprite()
		{
			// Add a single 2x2 sprite.
			m_data.SpriteList.AddSprite(2, 2, "", "");

			// Add a single blank background tile.
			m_data.BackgroundSpriteList.AddSprite(1, 1, "", "");
		}

		public MainForm Owner
		{
			get { return m_form; }
		}

		public string Name
		{
			get { return m_data.Filer.Filename; }
		}

		public SpriteList GetSprites(MainForm.Tab tab)
		{
			if (tab == MainForm.Tab.Sprites)
				return m_data.SpriteList;
			if (tab == MainForm.Tab.BackgroundSprites)
				return m_data.BackgroundSpriteList;
			if (tab == MainForm.Tab.BackgroundMap)
				return m_data.BackgroundSpriteList;
			return null;
		}

		public Sprite GetCurrentSprite(MainForm.Tab tab)
		{
			if (tab == MainForm.Tab.Sprites)
				return m_data.SpriteList.CurrentSprite;
			if (tab == MainForm.Tab.BackgroundSprites)
				return m_data.BackgroundSpriteList.CurrentSprite;
			if (tab == MainForm.Tab.BackgroundMap)
				return m_data.BackgroundSpriteList.CurrentSprite;
			return null;
		}

		public PaletteMgr GetSpritePalettes(MainForm.Tab tab)
		{
			if (tab == MainForm.Tab.Sprites)
				return m_data.SpritePalettes;
			if (tab == MainForm.Tab.BackgroundSprites)
				return m_data.BackgroundSpritePalettes;
			if (tab == MainForm.Tab.BackgroundMap)
				return BackgroundMapPalettes;
			return null;
		}

		public bool HasUnsavedChanges
		{
			get { return m_data.Filer.HasUnsavedChanges; }
			set { m_data.Filer.HasUnsavedChanges = value; }
		}

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

		public void FlushBitmaps()
		{
			m_data.SpriteList.FlushBitmaps();
			m_data.BackgroundSpriteList.FlushBitmaps();
		}

		public string GenerateUniqueSpriteName()
		{
			// When we're auto-generating sprite names, make sure the new name doesn't collide with
			// the names of any already-existing sprites.
			string strName = Sprite.AutoGenerateSpriteName();
			while (m_data.SpriteList.HasNamedSprite(strName))
				strName = Sprite.AutoGenerateSpriteName();
			return strName;
		}

		#region File

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

		private void Open_(Document doc)
		{
			// Copy data from newly loaded doc into this doc
			m_data = doc.m_data;
			// Update the document references to point to this document
			m_data.SpritePalettes.UpdateDocument(this);
			m_data.SpriteList.UpdateDocument(this);
			m_data.BackgroundSpritePalettes.UpdateDocument(this);
			m_data.BackgroundSpriteList.UpdateDocument(this);
			m_data.BackgroundMap.UpdateDocument(this);
			m_data.Filer.UpdateDocument(this);

			m_data.SpriteList.SelectFirstSprite();
			m_data.BackgroundSpriteList.SelectFirstSprite();
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
			}
			else
				// "Unable to export!"
				m_form.Error(ResourceMgr.GetString("ExportFail"));
		}

		#endregion

	}
}
