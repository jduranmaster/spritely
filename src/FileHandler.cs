using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

// TODO: catch exceptions when loading to ensure that file is closed properly

namespace Spritely
{
	class FileHandler
	{
		private MainForm m_Owner;
		private Document m_doc;

		private string m_strFilename;
		private bool m_fHasUnsavedChanges;

		public FileHandler(Document doc)
		{
			m_Owner = doc.Owner;
			m_doc = doc;

			m_strFilename = "";
			m_fHasUnsavedChanges = false;
		}

		public string Filename
		{
			get { return m_strFilename; }
		}

		public bool HasUnsavedChanges
		{
			get { return m_fHasUnsavedChanges; }
			set { m_fHasUnsavedChanges = value; }
		}

		public void UpdateDocument(Document doc)
		{
			m_Owner = doc.Owner;
			m_doc = doc;
		}

		public bool OpenFile()
		{
			OpenFileDialog OpenFileDialog;
			OpenFileDialog = new OpenFileDialog();
			OpenFileDialog.InitialDirectory = @"";
			OpenFileDialog.Filter = "XML files (*.xml)|*.xml|All files|*.*";

			if (OpenFileDialog.ShowDialog() == DialogResult.OK)
				return OpenFile(OpenFileDialog.FileName);

			return false;
		}

		public bool OpenFile(string strFileName)
		{
			if (!LoadFile(strFileName))
				return false;

			m_strFilename = strFileName;
			m_fHasUnsavedChanges = false;
			return true;
		}

		public bool Close()
		{
			if (m_fHasUnsavedChanges)
			{
				bool fCancel;
				bool fSave = m_Owner.AskYesNoCancel(ResourceMgr.GetString("SaveChanges"), out fCancel);
				if (fCancel)
					return false;
				if (fSave)
				{
					if (!SaveFile())
						// Implicitly cancel if we were unable to save.
						return false;
				}
			}

			m_strFilename = "";
			m_fHasUnsavedChanges = false;
			return true;
		}

		public bool SaveFile()
		{
			if (m_strFilename == "")
				return SaveFileAs();
			else
				return SaveFile_(m_strFilename);
		}

		public bool SaveFileAs()
		{
			bool fResult = false;

			SaveFileDialog SaveFileDialog;
			SaveFileDialog = new SaveFileDialog();
			SaveFileDialog.InitialDirectory = @"";
			SaveFileDialog.Filter = "XML files (*.xml)|*.xml|All files|*.*";
			if (SaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				fResult = SaveFile_(SaveFileDialog.FileName);
				if (fResult)
					m_strFilename = SaveFileDialog.FileName;
			}

			return fResult;
		}

		//TODO: save into temp file and overwrite original if successful
		// so that if there is an exception, the old file is still intact.
		private bool SaveFile_(string strPath)
		{
			TextWriter tw;

			try
			{
				tw = new StreamWriter(strPath);
			}
			catch (Exception ex)
			{
				m_Owner.Error(ResourceMgr.GetString("ExceptionOpenWrite") + ex.Message);
				return false;
			}

			tw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			tw.WriteLine("<gba_tileset>");

			m_doc.GetSpritePalettes(MainForm.Tab.Sprites).Save(tw);
			m_doc.GetSprites(MainForm.Tab.Sprites).Save(tw);

			m_doc.GetSpritePalettes(MainForm.Tab.BackgroundSprites).Save(tw);
			m_doc.GetSprites(MainForm.Tab.BackgroundSprites).Save(tw);

			tw.WriteLine("</gba_tileset>");
			tw.Close();

			m_fHasUnsavedChanges = false;
			return true;
		}

		private bool LoadFile(string strFile)
		{
			XmlTextReader xr = new XmlTextReader(strFile);
			XmlDocument xd = new XmlDocument();
			try
			{
				xd.Load(xr);
			}
			catch (Exception ex)
			{
				m_Owner.Error(ResourceMgr.GetString("ExceptionParseXML") + ex.Message);
				return false;
			}
			xr.Close();

			return LoadXML(xd.ChildNodes);
		}

		#region XML

		private static string GetXMLAttribute(XmlNode n, string strAttr)
		{
			XmlAttribute attr = n.Attributes.GetNamedItem(strAttr) as XmlAttribute;
			if (attr != null)
				return attr.Value;
			return "";
		}

		private static int GetXMLIntegerAttribute(XmlNode n, string strAttr)
		{
			XmlAttribute attr = n.Attributes.GetNamedItem(strAttr) as XmlAttribute;
			if (attr != null)
				return ParseInteger(attr.Value);
			return 0;
		}

		private static int ParseInteger(string str)
		{
			int nVal = 0;
			try
			{
				nVal = Int32.Parse(str);
			}
			catch
			{
			}
			return nVal;
		}

		static bool m_fFoundSpritePalettes;
		static bool m_fFoundSprites;
		static bool m_fFoundBackgroundPalettes;
		static bool m_fFoundBackgroundSprites;
		static bool m_fFoundBackgroundMap;

		private bool LoadXML(XmlNodeList xnl)
		{
			m_fFoundSpritePalettes = false;
			m_fFoundSprites = false;
			m_fFoundBackgroundPalettes = false;
			m_fFoundBackgroundSprites = false;
			m_fFoundBackgroundMap = false;

			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "gba_tileset"
					// Obsolete name for gba_tileset.
					// No longer used - included for backwards compatibility.
					|| xn.Name == "gba_sprite_collection"
					)
				{
					if (!LoadXML_gba_tileset(xn.ChildNodes))
						return false;
				}
			}

			if (!m_fFoundSpritePalettes)
			{
				//m_Owner.Error("Unable to load sprite palettes.");
				PaletteMgr palettes = m_doc.GetSpritePalettes(MainForm.Tab.Sprites);
				palettes.AddDefaultPalette("0", Palette.DefaultColorSet.Color1);
				palettes.AddMissingPalettes();
			}
			if (!m_fFoundSprites)
			{
				//TODO remove string
				//m_Owner.Error("Unable to load sprites.");
			}
			if (!m_fFoundBackgroundPalettes)
			{
				//m_Owner.Error("Unable to load background palettes.");
				PaletteMgr palettes = m_doc.GetSpritePalettes(MainForm.Tab.BackgroundSprites);
				palettes.AddDefaultPalette("0", Palette.DefaultColorSet.Color1);
				palettes.AddMissingPalettes();
			}
			if (!m_fFoundBackgroundSprites)
			{
				//m_Owner.Error("Unable to load background sprites.");
				m_doc.GetSprites(MainForm.Tab.BackgroundSprites).AddSprite(1, 1, "", "");
			}
			if (!m_fFoundBackgroundMap)
			{
				//m_Owner.Error("Unable to load background map.");
				//TODO: add default bg map?
			}

			// Remove any UndoActions since we just loaded from a file.
			m_doc.ResetUndo();
			return true;
		}

		private bool LoadXML_gba_tileset(XmlNodeList xnl)
		{
			foreach (XmlNode xn in xnl)
			{
				switch (xn.Name)
				{
					case "palettes":
						if (!LoadXML_palettes(xn.ChildNodes, MainForm.Tab.Sprites))
							return false;
						m_fFoundSpritePalettes = true;
						break;
					case "sprites":
						if (!LoadXML_sprites(xn.ChildNodes, MainForm.Tab.Sprites))
							return false;
						m_fFoundSprites = true;
						break;
					case "bgpalettes":
						if (!LoadXML_palettes(xn.ChildNodes, MainForm.Tab.BackgroundSprites))
							return false;
						m_fFoundBackgroundPalettes = true;
						break;
					case "bgsprites":
						if (!LoadXML_sprites(xn.ChildNodes, MainForm.Tab.BackgroundSprites))
							return false;
						m_fFoundBackgroundSprites = true;
						break;
					case "map":
						if (!LoadXML_map(xn.ChildNodes))
							return false;
						m_fFoundBackgroundMap = true;
						break;
				}
			}
			return true;
		}

		private bool LoadXML_palettes(XmlNodeList xnl, MainForm.Tab tab)
		{
			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "palette")
				{
					string strID = GetXMLAttribute(xn, "id");

					uint[] uiPalette = new uint[16];
					int nCurrEntry = 0;

					Regex rxPalette = new Regex(@"\s*0x([0-9A-Fa-f]{4})\s*,");
					Match mxPalette = rxPalette.Match(xn.InnerText);
					if (mxPalette.Success)
					{
						while (mxPalette.Success)
						{
							GroupCollection matchGroups = mxPalette.Groups;

							if (nCurrEntry < 16)
								uiPalette[nCurrEntry] = Convert.ToUInt32(matchGroups[1].Value, 16);
							nCurrEntry++;
							if (nCurrEntry == 16)
							{
								// After we've read 16 colors, create the palette.
								PaletteMgr palettes = m_doc.GetSpritePalettes(tab);
								if (!palettes.ImportPalette(strID, uiPalette))
								{
									// Warning/Error message already displayed.
									return false;
								}

								// Since we just loaded from a file, update the snapshot without creating an UndoAction
								palettes.GetNamedPalette(strID).RecordSnapshot();
							}

							// Get the next match.
							mxPalette = mxPalette.NextMatch();
						}

						if (nCurrEntry != 16)
						{
							// Wrong number of colors in palette.
							m_Owner.Warning(String.Format(ResourceMgr.GetString("ErrorNumColorsInPalette"), strID, nCurrEntry));
							return false;
						}
					}
				}
			}
			m_doc.GetSpritePalettes(tab).AddMissingPalettes();
			return true;
		}

		private bool LoadXML_sprites(XmlNodeList xnl, MainForm.Tab tab)
		{
			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "sprite")
				{
					string strName = GetXMLAttribute(xn, "name");
					string strDesc = GetXMLAttribute(xn, "desc");
					string strPalette = GetXMLAttribute(xn, "palette");
					string strSize = GetXMLAttribute(xn, "size");
					int nID = GetXMLIntegerAttribute(xn, "id");
					int nFirstTileID = GetXMLIntegerAttribute(xn, "firsttileid");
					string[] aSize = strSize.Split('x');
					int nWidth = ParseInteger(aSize[0]);
					int nHeight = ParseInteger(aSize[1]);

					Sprite s = m_doc.GetSprites(tab).AddSprite(nWidth, nHeight, strName, strDesc);
					if (s == null)
					{
						// Invalid sprite size
						//m_Owner.Error(String.Format("Invalid sprite size ({0}) for sprite named '{1}'.", strSize, strName));
						m_Owner.Error(String.Format(ResourceMgr.GetString("ErrorInvalidSpriteSize"), strSize, strName));
						return false;
					}

					s.ExportID = nID;
					s.FirstTileID = nFirstTileID;

					s.PaletteID = m_doc.GetSpritePalettes(tab).GetNamedPaletteID(strPalette);
					if (!LoadXML_sprite(s, xn.ChildNodes))
						return false;

					// Since we just loaded from a file, update the snapshot without creating an UndoAction
					s.RecordSnapshot();
				}
			}
			return true;
		}

		private bool LoadXML_sprite(Sprite s, XmlNodeList xnl)
		{
			int nTileIndex = 0;

			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "tile")
				{
					string strPalette = GetXMLAttribute(xn, "palette");

					uint[] bTile = new uint[32];
					int nCurrByte = 0;

					Regex rxTile = new Regex(@"\s*0x([0-9A-Fa-f]{2})\s*,");
					Match mxTile = rxTile.Match(xn.InnerText);
					if (mxTile.Success)
					{
						while (mxTile.Success)
						{
							GroupCollection matchGroups = mxTile.Groups;

							if (nCurrByte < 32)
								bTile[nCurrByte] = Convert.ToUInt32(matchGroups[1].Value, 16);
							nCurrByte++;
							if (nCurrByte == 32)
							{
								// After we've read 32 bytes, import the tile into the sprite.
								if (!s.ImportTile(nTileIndex, bTile))
								{
									// Warning/Error message already displayed.
									return false;
								}
								nTileIndex++;
							}

							// Get the next match.
							mxTile = mxTile.NextMatch();
						}
					}
				}
			}
			return true;
		}

		private bool LoadXML_map(XmlNodeList xnl)
		{
			int x = 0;
			int y = 0;

			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "row")
				{
					x = 0;

					foreach (XmlNode xn2 in xn.ChildNodes)
					{
						//TODO: warn if x,y doesn't match value in file
						int nTileID = GetXMLIntegerAttribute(xn2, "tileid");
						int nPaletteID = 0; //TODO: = Int32.Parse(GetXMLAttribute(xn2, "palette"));
						m_doc.GetSprites(MainForm.Tab.BackgroundSprites).Map.SetBackgroundTile(x, y, nTileID, nPaletteID);
						//TODO: handle error from SetBackgroundTile
						x++;
					}

					y++;
				}
			}
			return true;
		}

		#endregion

		#region Export

		public class ExportFile
		{
			/// <summary>
			/// Filename to create.
			/// </summary>
			public string Filename;
			/// <summary>
			/// Template file used to create the file.
			/// </summary>
			public string Template;
			/// <summary>
			/// Is this a project file (as opposed to a sprite/background data file)?
			/// </summary>
			public bool ProjectFile;
			/// <summary>
			/// Is this a game_state file?
			/// </summary>
			public bool GameStateFile;
			/// <summary>
			/// Subdirectory where this file should be placed when writing out projects.
			/// </summary>
			public string SubDirectory;
			public ExportFile(string strFile, string strTemplate, bool fProject, bool fGameState, string strDir)
			{
				Filename = strFile;
				Template = strTemplate;
				ProjectFile = fProject;
				GameStateFile = fGameState;
				SubDirectory = strDir;
			}
		};
		private ExportFile[] ExportFiles = new ExportFile[]
		{
			// Makefile
			new ExportFile("Makefile",			"makefile.txt",			true,	false,	""),
			// Project files
			new ExportFile("animation.cpp",		"animation_cpp.txt",	true,	false,	"source"),
			new ExportFile("animation.h",		"animation_h.txt",		true,	false,	"source"),
			new ExportFile("game_state.h",		"game_state_h.txt",		true,	true,	"source"),
			new ExportFile("game_state.cpp",	"game_state_cpp.txt",	true,	true,	"source"),
			new ExportFile("collision.cpp",		"collision_cpp.txt",	true,	false,	"source"),
			new ExportFile("collision.h",		"collision_h.txt",		true,	false,	"source"),
			new ExportFile("main.cpp",			"main_cpp.txt",			true,	false,	"source"),
			//new ExportFile("oam_info.h",		"oam_info_h.txt",		true,	false,	"source"),
			new ExportFile("object_utils.cpp",	"object_utils_cpp.txt",	true,	false,	"source"),
			new ExportFile("object_utils.h",	"object_utils_h.txt",	true,	false,	"source"),
			// Sprite/Background files
			new ExportFile("backgrounds.cpp",	"backgrounds_cpp.txt",	false,	false,	"source"),
			new ExportFile("backgrounds.h",		"backgrounds_h.txt",	false,	false,	"source"),
			new ExportFile("sprites.cpp",		"sprites_cpp.txt",		false,	false,	"source"),
			new ExportFile("sprites.h",			"sprites_h.txt",		false,	false,	"source"),
		};

		public enum ExportResult
		{
			OK,
			Failed,
			Cancel,
		}

		public ExportResult Export(out string strProjectDir, out bool fNDS, out bool fExportProject)
		{
			strProjectDir = "";
			fNDS = false;
			fExportProject = false;

			Export ex = new Export();
			DialogResult result = ex.ShowDialog();
			if (result == DialogResult.Cancel)
				return ExportResult.Cancel;
			if (result != DialogResult.OK)
				return ExportResult.Failed;

			strProjectDir = ex.ExportLocation;
			fNDS = ex.NDS;
			fExportProject = ex.Project || ex.UpdateProject;
			bool fSkipGameState = ex.UpdateProject;

			// Figure out which directory we should use.
			// In general, we use the directory selected by the user.
			// But, if:
			//   (1) we're exporting just sprites/backgrounds (not a complete project), and
			//   (2) the sprite/background files already exist in the "source" subdirectory
			// then we export the files into the "source" subdirectory instead of the selected directory.
			//
			// We need to consider the following use cases:
			//   (1) a. User exports a complete project into a "project" directory
			//       b. User re-exports just the sprites/backgrounds in to the "project" directory
			//       This is the common case. We expect the files in the "project/source" dir to be updated
			//       even though the user selected the "project" directory.
			//   (2) a. User starts a new project by accidentally exporting just the sprites/backgrounds
			//          into the "project" directory
			//       b. User notices mistake (when "make" doesn't work), and re-exports entire project into
			//          the "project" directory. This creates the "project/source" directory.
			//       c. There are now 2 copies of the sprite/background files: one in "project" and
			//          another in "project/source".
			//       d. User re-exports just the sprites/backgrounds into the "project" directory
			//       We expect the "project/source" files to be updated since they are the only ones
			//       recognized by the project makefile. The sprite/background files in the "project"
			//       directory are ignored (and should be deleted by the user).
			//   (3) a. User creates their own project from scratch and wants to use exported sprites.
			//       b. User exports sprites/backgrounds into their project directory.
			//       c. There is no "source" subdir (or it doesn't have sprites/backgrounds files), so
			//          files are written into the "project" directory.
			//       A similar situation is where the user exports sprites/backgrounds directly into the
			//       "project/source" directory.
			//int nInDir = 0;
			int nInSubdir = 0;

			if (!fExportProject)
			{
				// Lookup locations of any already-existing non-project files.
				foreach (ExportFile f in ExportFiles)
				{
					// This is a project-only file - ignore.
					if (f.ProjectFile)
						continue;

					// Does the file already exist in the specified directory?
					//StringBuilder sbDirPath = new StringBuilder(strProjectDir);
					//sbDirPath.Append(Path.DirectorySeparatorChar);
					//sbDirPath.Append(f.Filename);
					//if (System.IO.File.Exists(sbDirPath.ToString()))
					//	nInDir++;

					// Does it already exist in the subdirectory?
					if (f.SubDirectory != "")
					{
						StringBuilder sbSubdirPath = new StringBuilder(strProjectDir);
						sbSubdirPath.Append(Path.DirectorySeparatorChar);
						sbSubdirPath.Append(f.SubDirectory);
						sbSubdirPath.Append(Path.DirectorySeparatorChar);
						sbSubdirPath.Append(f.Filename);
						if (System.IO.File.Exists(sbSubdirPath.ToString()))
							nInSubdir++;
					}
				}
			}

			// Should we ignore the subdir and write the files directly into the specified directory?
			bool fIgnoreSubdir = false;
			// Ignore the subdir if we're exporting just the sprite files and the sprite files don't
			// already exist in the "source" subdir
			if (!fExportProject && nInSubdir == 0)
				fIgnoreSubdir = true;

			foreach (ExportFile f in ExportFiles)
			{
				// Only export projet files if we're exporting a project.
				if (f.ProjectFile && !fExportProject)
					continue;

				// Don't overwrite the game_state files if we're updating a project.
				if (f.GameStateFile && fSkipGameState)
					continue;

				StringBuilder sb = new StringBuilder(strProjectDir);

				// Don't create the subdirectory unless we're
				if (!fIgnoreSubdir && f.SubDirectory != "")
				{
					sb.Append(Path.DirectorySeparatorChar);
					sb.Append(f.SubDirectory);
					
					if (!System.IO.Directory.Exists(sb.ToString()))
						System.IO.Directory.CreateDirectory(sb.ToString());
				}
				sb.Append(Path.DirectorySeparatorChar);
				sb.Append(f.Filename);

				// TODO: if this is a project file, and it already exists and it is different from the
				// default (eg: changes made to main.cpp), then warn the user before overwriting.

				if (!ExportFromTemplate(f.Template, sb.ToString(), fNDS))
					return ExportResult.Failed;
			}

			return ExportResult.OK;
		}

		private bool ExportFromTemplate(string strTemplateFilename, string strOutputFilename, bool fNDS)
		{
			TextReader tr;
			TextWriter tw;

			string strExeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

			string strTemplateDir = String.Format("{0}{1}{2}{3}", strExeDir,
													Path.DirectorySeparatorChar,
													"templates",
													Path.DirectorySeparatorChar);
			try
			{
				tr = new StreamReader(strTemplateDir + strTemplateFilename, Encoding.UTF8);
			}
			catch (Exception ex)
			{
				//m_Owner.Error("An exception was thrown while load the project template files: " + ex.Message);
				m_Owner.Error(ResourceMgr.GetString("ExceptionLoadTemplate") + ex.Message);
				return false;
			}

			try
			{
				tw = new StreamWriter(strOutputFilename);
			}
			catch (Exception ex)
			{
				//m_Owner.Error("An exception was thrown while opening the project file for writing: " + ex.Message);
				m_Owner.Error(ResourceMgr.GetString("ExceptionOpenProjectWrite") + ex.Message);
				return false;
			}

			SpriteList sprites = m_doc.GetSprites(MainForm.Tab.Sprites);
			SpriteList bgsprites = m_doc.GetSprites(MainForm.Tab.BackgroundSprites);
			PaletteMgr palettes = m_doc.GetSpritePalettes(MainForm.Tab.Sprites);
			PaletteMgr bgpalettes = m_doc.GetSpritePalettes(MainForm.Tab.BackgroundSprites);

			string strLine;
			while ((strLine = tr.ReadLine()) != null)
			{
				if (strLine.StartsWith("%%NDS:%%"))
				{
					if (fNDS)
						tw.WriteLine(strLine.Substring(8));
					continue;
				}
				if (strLine.StartsWith("%%GBA:%%"))
				{
					if (!fNDS)
						tw.WriteLine(strLine.Substring(8));
					continue;
				}
				if (strLine == "%%_SPRITE_INFO_%%")
				{
					sprites.ExportGBA_AssignIDs();
					sprites.ExportGBA_SpriteInfo(tw);
					continue;
				}
				if (strLine == "%%_SPRITE_PALETTES_%%")
				{
					palettes.ExportGBA_Palette(tw);
					continue;
				}
				if (strLine == "%%_SPRITE_TILES_%%")
				{
					sprites.ExportGBA_SpriteData(tw);
					continue;
				}
				if (strLine == "%%_SPRITE_IDS_%%")
				{
					sprites.ExportGBA_SpriteIDs(tw);
					continue;
				}
				if (strLine == "%%_BACKGROUND_PALETTES_%%")
				{
					bgpalettes.ExportGBA_Palette(tw);
					continue;
				}
				if (strLine == "%%_BACKGROUND_TILES_%%")
				{
					// Even though backgrounds don't have sprites, we allow background "sprites"
					// for the creation of the background map. So we need to "export" these sprites
					// so that they get assigned unique sprite ids for the tile export to work.
					bgsprites.ExportGBA_AssignIDs();
					bgsprites.ExportGBA_SpriteData(tw);
					continue;
				}
				if (strLine == "%%_BACKGROUND_TILEMAP_%%")
				{
					bgsprites.Map.ExportGBA_BackgroundTileMap(tw);
					continue;
				}

				strLine = strLine.Replace("%%_VERSION_%%", ResourceMgr.GetString("Version"));
				strLine = strLine.Replace("%%_PLATFORM_%%", fNDS ? "NDS" : "GBA");
				strLine = strLine.Replace("%%_NUM_SPRITE_PALETTES_%%", palettes.NumPalettes.ToString());
				strLine = strLine.Replace("%%_NUM_SPRITE_TILES_%%", sprites.NumTiles.ToString());
				strLine = strLine.Replace("%%_NUM_SPRITES_%%", sprites.NumSprites.ToString());
				strLine = strLine.Replace("%%_NUM_BACKGROUND_PALETTES_%%", bgpalettes.NumPalettes.ToString());
				strLine = strLine.Replace("%%_NUM_BACKGROUND_TILES_%%", bgsprites.NumTiles.ToString());

				tw.WriteLine(strLine);
			}

			tw.Close();
			tr.Close();

			return true;
		}

		#endregion

	}
}
