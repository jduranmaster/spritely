using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Spritely
{
	public partial class TutorialTestForm : Form
	{
		private Document m_doc = null;

		public TutorialTestForm(ProjectMainForm parent)
		{
			InitializeComponent();
		}

		private void UnitTestForm_Load(object sender, EventArgs e)
		{
			RunTests();
		}

		private void Print(string strMessage)
		{
			lbResults.Items.Add(strMessage);
			lbResults.SelectedIndex = lbResults.Items.Count - 1;
		}

		private void Print(string strFormat, params object[] args)
		{
			string strMessage = String.Format(strFormat, args);
			lbResults.Items.Add(strMessage);
			lbResults.SelectedIndex = lbResults.Items.Count - 1;
		}

		string CalcPath(string str1, string str2)
		{
			return String.Format("{0}{1}{2}", str1, Path.DirectorySeparatorChar.ToString(), str2);
		}

		string CalcPath(string[] path)
		{
			return String.Join(Path.DirectorySeparatorChar.ToString(), path);
		}

		void ClearDir(string strDir)
		{
			// Delete directory if it exists.
			if (Directory.Exists(strDir))
			{
				// Directory delete can fail if the directory is open in a window, so
				// we don't want to fail in this case.
				try
				{
					Directory.Delete(strDir, true);
				}
				catch { }
			}
			if (!Directory.Exists(strDir))
				Directory.CreateDirectory(strDir);
		}

		void CopyDir(string strSourceDir, string strTargetDir)
		{
			ClearDir(strTargetDir);

			foreach (string file in Directory.GetFiles(strSourceDir))
				File.Copy(file, CalcPath(strTargetDir, Path.GetFileName(file)), true);

			foreach (string dir in Directory.GetDirectories(strSourceDir))
			{
				string[] path = dir.Split(Path.DirectorySeparatorChar);
				CopyDir(dir, CalcPath(strTargetDir, path[path.Length-1]));
			}
		}

		void RunTests()
		{
			if (!System.IO.Directory.Exists(Options.Debug_TutorialPath))
			{
				Print("Unable to find tutorials in {0}.", Options.Debug_TutorialPath);
				return;
			}

			if (!System.IO.Directory.Exists(Options.Debug_TutorialRawPath))
			{
				Print("No tutorials to process in {0}.", Options.Debug_TutorialRawPath);
				return;
			}

			int nTuts = 0;
			int nBadGbaTests = 0;
			int nBadNdsTests = 0;
			lbResults.Items.Clear();

			foreach (string filename in Directory.GetFiles(Options.Debug_TutorialRawPath))
			{
				// Uncomment out all but one of the next lines to make it easier to debug a single tutorial.
				// Setup tutorials:
				//if (!filename.EndsWith("setup_windows.txt"))
				//if (!filename.EndsWith("using_pn.txt"))
				//if (!filename.EndsWith("build_gba_rom.txt"))
				//if (!filename.EndsWith("run_gba_rom.txt"))
				// Programming tutorials:
				if (!filename.EndsWith("add_title.txt"))
				//if (!filename.EndsWith("create_sprite.txt"))
				//if (!filename.EndsWith("creating_pong.txt"))
				//if (!filename.EndsWith("screen_bounds.txt"))
				//if (!filename.EndsWith("second_object.txt"))
					continue;

				nTuts++;
				Print("Processing {0}", filename);

				if (!RunTutorial(filename, false))
					nBadGbaTests++;
				if (!RunTutorial(filename, true))
					nBadNdsTests++;
			}

			Print("{0} tutorials processed. {1} GBA failed. {2} NDS failed.", nTuts, nBadGbaTests, nBadNdsTests);
		}

		bool RunTutorial(string strFilename, bool fNDS)
		{
			// Make a new document and initialize it.
			m_doc = new Document(null);
			m_doc.InitializeEmptyDocument();

			string strTutorialName = "unknown";
			string strWorkDir = "test_" + (fNDS ? "nds" : "gba");
			string strSourceFile = "";
			string strTitle = "";
			string strTempHtmlOutput = CalcPath(Options.Debug_TutorialPath, "tut.html");

			bool fGatherLines = false;
			List<string> Lines = new List<string>();

			Regex rxName = new Regex(@"^NAME\s+(.+)$");
			Regex rxTitle = new Regex(@"^TITLE\s+(.+)$");
			Regex rxTutorialLink = new Regex(@"^TUTORIAL_LINK\s+(\w+)\s+(.+)$");
			Regex rxLinkSourceFile = new Regex(@"^LINK_SOURCE_FILE\s+(.+)$");
			Regex rxLinkTargetFile = new Regex(@"^LINK_TARGET_FILE$");
			Regex rxCategory = new Regex(@"^CATEGORY\s+(.+)$");
			Regex rxImage = new Regex(@"^IMAGE\s+(.+)$");
			Regex rxFind = new Regex(@"^FIND\s+(.+)$");
			Regex rxUpdate = new Regex(@"^UPDATE\s+(.+)$");
			Regex rxAction = new Regex(@"^SPRITELY_ACTION\s+(.+)$");
			Match m;

			int nFind = 0;
			int nUpdate = 0;

			try
			{
				using (TextWriter tw = new StreamWriter(strTempHtmlOutput))
				{
					using (TextReader tr = new StreamReader(strFilename, Encoding.UTF8))
					{
						string strLine;
						while ((strLine = tr.ReadLine()) != null)
						{
							if (strLine.StartsWith("#"))
								continue;

							// NAME
							m = rxName.Match(strLine);
							if (m.Success)
							{
								strTutorialName = m.Groups[1].Value;
								continue;
							}

							// TITLE
							m = rxTitle.Match(strLine);
							if (m.Success)
							{
								strTitle = m.Groups[1].Value;
								WriteTutorialHeader(tw, strTitle);
								continue;
							}

							// VERIFIED
							if (strLine == "VERIFIED")
							{
								WriteCompatibility(tw);
								continue;
							}

							// CATEGORY
							m = rxCategory.Match(strLine);
							if (m.Success)
							{
								continue;
							}

							// IMAGE
							m = rxImage.Match(strLine);
							if (m.Success)
							{
								WriteImage(tw, strTutorialName, m.Groups[1].Value);
								continue;
							}

							// TUTORIAL_LINK
							m = rxTutorialLink.Match(strLine);
							if (m.Success)
							{
								WriteTutorialLink(tw, m.Groups[1].Value, m.Groups[2].Value);
								continue;
							}

							// LINK_SOURCE_FILE
							m = rxLinkSourceFile.Match(strLine);
							if (m.Success)
							{
								WriteSourceFileLink(tw, strTutorialName, strWorkDir, m.Groups[1].Value, fNDS);
								continue;
							}

							// LINK_TARGET_FILE
							m = rxLinkTargetFile.Match(strLine);
							if (m.Success)
							{
								WriteTargetFileLink(tw, strTutorialName, strWorkDir, fNDS);
								continue;
							}

							// FIND
							m = rxFind.Match(strLine);
							if (m.Success)
							{
								strSourceFile = m.Groups[1].Value;
								fGatherLines = true;
								Lines.Clear();
								nFind++;
								continue;
							}

							// END_FIND
							if (strLine == "END_FIND")
							{
								int nStartLine, nLines;
								if (!FindLines(strWorkDir, strSourceFile, Lines, out nStartLine, out nLines))
								{
									Print("Failed to match FIND #{0} in {1} at line {2}", nFind, strSourceFile, nLines);
									return false;
								}
								WriteSourceLines(tw, strSourceFile, nStartLine, nStartLine+nLines-1, Lines, false);
								fGatherLines = false;
								continue;
							}

							// UPDATE
							m = rxUpdate.Match(strLine);
							if (m.Success)
							{
								strSourceFile = m.Groups[1].Value;
								fGatherLines = true;
								Lines.Clear();
								nUpdate++;
								continue;
							}

							// END_UPDATE
							if (strLine == "END_UPDATE")
							{
								int nStartLine, nLines;
								if (!UpdateLines(strWorkDir, strSourceFile, Lines, out nStartLine, out nLines))
								{
									Print("Failed to match UPDATE #{0} in {1}", nUpdate, strSourceFile);
									return false;
								}
								WriteSourceLines(tw, strSourceFile, nStartLine, nStartLine+nLines-1, Lines, true);
								fGatherLines = false;
								continue;
							}

							// SPRITELY_ACTION
							m = rxAction.Match(strLine);
							if (m.Success)
							{
								string strCommand = m.Groups[1].Value;
								if (!HandleSpritelyAction(strWorkDir, strCommand, fNDS))
								{
									Print("Failed to process {0} ({1})", strCommand, fNDS ? "nds" : "gba");
									return false;
								}
								continue;
							}

							// VERIFY_BUILD
							if (strLine == "VERIFY_BUILD")
							{
								if (!VerifyBuild(strWorkDir))
								{
									Print("Build verify failed!");
									return false;
								}
								continue;
							}

							if (fGatherLines)
								Lines.Add(strLine);
							else
								tw.WriteLine(strLine);
						}
					}

					//TODO: Write out links to completed project files.
					//string strPlatform = fNDS ? "nds" : "gba";
					//string strTarget = strProjectName + "." + strPlatform;
					//string strTargetPath = CalcPath(new string[] { Options.Debug_TutorialPath, strName, strPlatform });
					//if (!Directory.Exists(strTargetPath))
					//	Directory.CreateDirectory(strTargetPath);

					//File.Copy(CalcPath(new string[] { Options.Debug_TutorialPath, strProjectName, strTarget }),
					//		CalcPath(strTargetPath, strTarget), true);

					tw.WriteLine("</body>");
					tw.WriteLine("");
					tw.WriteLine("</html>");
				}
			}
			catch (Exception ex)
			{
				Print("Unable to process tutorial file {0}", ex.Message);
				return false;
			}

			// Copy temp file over original and delete temp.
			string strHtmlOutput = CalcPath(Options.Debug_TutorialPath, strTutorialName + ".html");
			File.Copy(strTempHtmlOutput, strHtmlOutput, true);
			File.Delete(strTempHtmlOutput);

			return true;
		}

		void WriteTutorialHeader(TextWriter tw, string strTitle)
		{
			tw.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\"");
			tw.WriteLine("\"http://www.w3.org/TR/html4/loose.dtd\">");
			tw.WriteLine("");
			tw.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
			tw.WriteLine("");
			tw.WriteLine("<head>");
			tw.WriteLine(String.Format("<title>{0}</title>", strTitle));
			tw.WriteLine("");
			tw.WriteLine("<link href=\"css/tutorial.css\" type=\"text/css\" rel=\"stylesheet\" />");
			tw.WriteLine("<script type=\"text/javascript\" src=\"css/precodeformat.js\"></script>");
			tw.WriteLine("");
			tw.WriteLine("</head>");
			tw.WriteLine("");
			tw.WriteLine("<body onload=\"PreCodeFormat()\">");
			tw.WriteLine(String.Format("<h1>{0}</h1>", strTitle));
		}

		void WriteCompatibility(TextWriter tw)
		{
			tw.WriteLine("<p class=\"alert\">");
			tw.WriteLine("This tutorial has been tested with");
			tw.WriteLine(String.Format(" <a href=\"http://www.devkitpro.org\">devkitARM release {0}</a>",
					Options.Debug_DevkitArmVersion));
			tw.WriteLine(String.Format(" and <a href=\"http://code.google.com/p/spritely\">Spritely version {0}</a>",
					Options.VersionString));
			tw.WriteLine(" and verified to work for both GBA and NDS projects.");
			tw.WriteLine("</p>");
		}

		void WriteImage(TextWriter tw, string strTutorialName, string strImageName)
		{
			string strImagePath = CalcPath(strTutorialName, strImageName);
			if (!File.Exists(CalcPath(Options.Debug_TutorialPath, strImagePath)))
				Print("WARNING - Image {0} not found!", strImagePath);
			tw.WriteLine(String.Format("<p><img src=\"{0}\" /></p>", strImagePath));
		}

		void WriteTutorialLink(TextWriter tw, string strTutorial, string strTitle)
		{
			tw.WriteLine(String.Format("<a href=\"{0}.html\">{1}</a>", strTutorial, strTitle));
		}

		void WriteSourceFileLink(TextWriter tw, string strName, string strProjectName, string strFileName, bool fNDS)
		{
			string strPlatform = fNDS ? "nds" : "gba";
			tw.WriteLine(String.Format("<li><a href=\"{0}/{1}/{2}\">{3}</a></li>", strName, strPlatform, strFileName, strFileName));
		}

		void WriteTargetFileLink(TextWriter tw, string strName, string strProjectName, bool fNDS)
		{
			string strPlatform = fNDS ? "nds" : "gba";
			string strTarget = strProjectName + "." + strPlatform;			
			tw.WriteLine(String.Format("<li><a href=\"{0}/{1}/{2}\">{3}</a></li>", strName, strPlatform, strTarget, strTarget));
		}

		void WriteSourceLines(TextWriter tw, string strFilename, int nStartLine, int nEndLine,
			List<string> Lines, bool fUpdate)
		{
			tw.Write(String.Format("<p class=\"filename\"><code><b>{0}</b></code>&nbsp;&nbsp;&mdash;&nbsp;&nbsp;",
				strFilename));
			if (nStartLine == nEndLine)
				tw.WriteLine(String.Format("Line {0}:</p>", nStartLine));
			else
				tw.WriteLine(String.Format("Lines {0} - {1}:</p>", nStartLine, nEndLine));
			tw.WriteLine("<pre class=\"code\">");

			foreach (string line in Lines)
			{
				string strPrefix = line.Substring(0, 1);
				string strCode = FixupHtml(line.Substring(1));
				if (strPrefix == ".")
				{
					if (fUpdate)
					{
						// The disabled tag must come after any leading tabs
						// to ensure proper tab processing.
						string strTabs = "";
						while (strCode.Length > 1 && strCode[0] == '\t')
						{
							strTabs += strCode[0];
							strCode = strCode.Substring(1);
						}
						// Make sure strTabs and str are not both blank.
						// IE6/7 won't render them correctly.
						if (strTabs == "" && strCode == "")
							strTabs = "\t";
						tw.WriteLine(strTabs + "<disabled/>" + strCode);
					}
					else
						tw.WriteLine(strCode);
				}
				else if (strPrefix == "+")
					tw.WriteLine("<mark type=\"plus\"/>" + strCode);
				else if (strPrefix == "x")
					tw.WriteLine("<mark type=\"cross\"/>" + strCode);
				else if (strPrefix == ">" || strPrefix == "*")
					tw.WriteLine("<mark type=\"arrow\"/>" + strCode);
				//else starts with "-", which we don't display
			}

			tw.WriteLine("</pre>");
		}

		/// <summary>
		/// Fixup punct marks in the code so that they display correctly in HTML.
		/// </summary>
		/// <param name="strCode"></param>
		/// <returns></returns>
		string FixupHtml(string strCode)
		{
			strCode = strCode.Replace("&", "&amp;");
			strCode = strCode.Replace("<", "&lt;");
			strCode = strCode.Replace(">", "&gt;");
			return strCode;
		}

		bool FindLines(string strProjectName, string strFilename, List<string> Lines, out int nSourceStartLine, out int nSourceNumLines)
		{
			nSourceStartLine = 0;
			nSourceNumLines = 0;

			bool fScan = true;
			bool fForceMatch = false;
	
			int nLineNum = 0;
			int nLineMax = Lines.Count;
			int nSourceLine = 0;
	
			string strPath = CalcPath(new string[] { Options.Debug_TutorialPath, strProjectName, "source", strFilename });
			try
			{
				using (TextReader tr = new StreamReader(strPath, Encoding.UTF8))
				{
					string strLine;
					while ((strLine = tr.ReadLine()) != null)
					{
						strLine = strLine.TrimEnd();
						nSourceLine++;
						string strPrefix = Lines[nLineNum].Substring(0,1);
						string strMatchLine = Lines[nLineNum].Substring(1);
						if (strPrefix == "@")
						{
							// Set breakpoint at next line to break when "@" line is encoutered.
							nLineNum++;
							continue;
						}
						if ("+-x><".Contains(strPrefix))
						{
							Print("Error - '{0}' is an invalid prefix in a FIND block", strPrefix);
							return false;
						}
						if (fScan && strLine == strMatchLine)
						{
							fScan = false;
							fForceMatch = true;
							nSourceStartLine = nSourceLine;
							nLineNum++;
							if (nLineNum >= nLineMax)
							{
								nSourceNumLines = 1;
								return true;
							}
							continue;
						}
						if (fForceMatch)
						{
							if (strLine != strMatchLine)
							{
								Print("Error - ({0}) '{1}' != ({2}) '{3}'", nSourceLine, strLine, nLineNum, strMatchLine);
								nSourceNumLines = nSourceLine - nSourceStartLine + 1;
								return false;
							}
							else
							{
								nLineNum++;
								if (nLineNum >= nLineMax)
								{
									nSourceNumLines = nSourceLine - nSourceStartLine + 1;
									return true;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Print("Unable to load file {0} - {1}", strPath, ex.Message);
				return false;
			}

			Print("Error - ({0}) '{1}' != ({2}) '{3}'", nSourceLine, "EOF", nLineNum, nLineNum < nLineMax ? Lines[nLineNum].Substring(1) : "???");
			nSourceNumLines = nSourceLine - nSourceStartLine + 1;
			return false;
		}

		bool UpdateLines(string strProjectName, string strFilename, List<string> Lines, out int nSourceStartLine, out int nSourceNumLines)
		{
			nSourceStartLine = 0;
			nSourceNumLines = 0;

			bool fScan = true;
			bool fForceMatch = false;
			bool fSuccess = false;

			int nLineNum = 0;
			int nLineMax = Lines.Count;
			int nSourceLine = 0;
			int nInsertedLines = 0;
			int nDeletedLines = 0;

			string strTempfile = "temp.out";
			string strSourcePath = CalcPath(new string[] { Options.Debug_TutorialPath, strProjectName, "source", strFilename });
			string strTempPath = CalcPath(new string[] { Options.Debug_TutorialPath, strProjectName, strTempfile });

			try
			{
				using (TextReader tr = new StreamReader(strSourcePath, Encoding.UTF8))
				{
					using (TextWriter tw = new StreamWriter(strTempPath))
					{
						string strLine;
						while ((strLine = tr.ReadLine()) != null)
						{
							nSourceLine++;

							string strPrefix = "";
							string strMatchLine = "";
							if (nLineNum < nLineMax)
							{
								strPrefix = Lines[nLineNum].Substring(0, 1);
								strMatchLine = Lines[nLineNum].Substring(1);
							}
							if (strPrefix == "@")
							{
								// Set breakpoint at next line to break when "@" line is encoutered.
								nLineNum++;
								continue;
							}

							strLine = strLine.TrimEnd();
							if (fScan && strLine == strMatchLine)
							{
								fScan = false;
								fForceMatch = true;
								nSourceStartLine = nSourceLine;
								nLineNum++;
								if (strPrefix == "-" || strPrefix == "x")
									continue;
							}
							else if (fForceMatch)
							{
								// Handle deletes
								if (strPrefix == "-" || strPrefix == "x")
								{
									if (strLine == strMatchLine)
									{
										nLineNum++;
										continue;
									}
									else
									{
										Print("Error - ({0}) '{1}' != ({2}) '{3}'", nSourceLine, strLine, nLineNum, strMatchLine);
										return false;
									}
								}

								// Insert any lines.
								while (strPrefix == "+")
								{
									tw.WriteLine(strMatchLine);
									nInsertedLines++;
									nLineNum++;

									if (nLineNum < nLineMax)
									{
										strPrefix = Lines[nLineNum].Substring(0, 1);
										strMatchLine = Lines[nLineNum].Substring(1);
									}
									else
									{
										strPrefix = "";
										strMatchLine = "";
									}
								}

								// Handle line updates ('<' line followed by '>' line).
								if (strPrefix == "<" && strLine == strMatchLine)
								{
									nLineNum++;
									tw.WriteLine(Lines[nLineNum].Substring(1));
									nLineNum++;
									continue;
								}

								if (nLineNum >= nLineMax)
								{
									nSourceNumLines = nSourceLine - nSourceStartLine;
									fForceMatch = false;
									fSuccess = true;
								}
								else
								{
									// Match line.
									if (strLine != strMatchLine)
									{
										Print("Error - ({0}) '{1}' != ({2}) '{3}'", nSourceLine, strLine, nLineNum, strMatchLine);
										return false;
									}
									else
										nLineNum++;
								}
							}
							tw.WriteLine(strLine);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Print("Unable to process file {0}", ex.Message);
				return false;
			}

			// Check for match at end of file.
			if (!fSuccess && nLineNum >= nLineMax)
			{
				nSourceNumLines = nSourceLine - nSourceStartLine;
				fSuccess = true;
			}

			nSourceNumLines += nInsertedLines;
			nSourceNumLines -= nDeletedLines;

			if (fSuccess)
			{
				// Copy temp file over original and delete temp.
				File.Copy(strTempPath, strSourcePath, true);
				File.Delete(strTempPath);
			}
			return fSuccess;
		}

		bool VerifyBuild(string strProjectName)
		{
			string strDir = CalcPath(Options.Debug_TutorialPath, strProjectName);

			// Cleanup previous binaries.
			string[] gbaFiles = Directory.GetFiles(strDir, "*.gba");
			foreach (string file in gbaFiles)
				File.Delete(file);
			string[] ndsFiles = Directory.GetFiles(strDir, "*.nds");
			foreach (string file in ndsFiles)
				File.Delete(file);

			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = "make";
			psi.Arguments = "-B";	// Force complete rebuild.
			psi.WorkingDirectory = strDir;
			//psi.CreateNoWindow = false;
			//psi.WindowStyle = ProcessWindowStyle.Hidden;
			//psi.UseShellExecute = false;
			//psi.RedirectStandardOutput = true;
			using (Process p = Process.Start(psi))
			{
				p.WaitForExit();
			}

			// Did we successfully create a new binary?
			if (Directory.GetFiles(strDir, "*.gba").Length != 0
				|| Directory.GetFiles(strDir, "*.nds").Length != 0)
				return true;

			return false;
		}

		/// <summary>
		/// Draw a sample sprite for use in the tutorials.
		/// </summary>
		/// <param name="s"></param>
		private void DrawSampleSprite(Sprite s)
		{
			int color = s.Subpalette.CurrentColor;
			for (int i = 4; i < 12; i++)
			{
				for (int j = 4; j < 6; j++)
				{
					s.SetPixel(i, j, color);
					s.SetPixel(j, i, color);
				}
				for (int j = 10; j < 12; j++)
				{
					s.SetPixel(i, j, color);
					s.SetPixel(j, i, color);
				}
			}
			for (int i = 6; i < 10; i++)
			{
				s.SetPixel(3, i, color);
				s.SetPixel(12, i, color);
				s.SetPixel(i, 3, color);
				s.SetPixel(i, 12, color);
			}
			s.SetPixel(6, 6, color);
			s.SetPixel(6, 9, color);
			s.SetPixel(9, 6, color);
			s.SetPixel(9, 9, color);
		}

		private bool HandleSpritelyAction(string strProjectName, string strCommand, bool fNDS)
		{
			Match m;

			if (strCommand == "delete_sprite")
			{
				Spriteset ss = m_doc.Spritesets.Current;
				ss.RemoveSelectedSprite(null);
				return true;
			}
			m = Regex.Match(strCommand, "create_sprite\\s+(\\d)x(\\d)\\s+\"(.+)\"");
			if (m.Success)
			{
				int width = Int32.Parse(m.Groups[1].Value);
				int height = Int32.Parse(m.Groups[2].Value);
				string name = m.Groups[3].Value;
				Spriteset ss = m_doc.Spritesets.Current;
				ss.AddSprite(width, height, name, -1, "", 0, null);
				return true;
			}
			m = Regex.Match(strCommand, "rename_sprite\\s+(.+)");
			if (m.Success)
			{
				Sprite s = m_doc.Spritesets.Current.CurrentSprite;
				s.Name = m.Groups[1].Value;
				return true;
			}
			m = Regex.Match(strCommand, "select_color\\s+(\\d)");
			if (m.Success)
			{
				int color = Int32.Parse(m.Groups[1].Value);
				Spriteset ss = m_doc.Spritesets.Current;
				ss.Palette.GetCurrentSubpalette().CurrentColor = color;
				return true;
			}
			m = Regex.Match(strCommand, "fill_sprite\\s+(\\d),(\\d)");
			if (m.Success)
			{
				int x = Int32.Parse(m.Groups[1].Value);
				int y = Int32.Parse(m.Groups[2].Value);
				Sprite s = m_doc.Spritesets.Current.CurrentSprite;
				s.FloodFillClick(x,y);
				return true;
			}
			m = Regex.Match(strCommand, "draw_sample_2x2_sprite");
			if (m.Success)
			{
				Sprite s = m_doc.Spritesets.Current.CurrentSprite;
				DrawSampleSprite(s);
				return true;
			}
			m = Regex.Match(strCommand, "import_bgimage\\s+(.+)");
			if (m.Success)
			{
				string strImageFile = CalcPath(Options.Debug_TutorialRawDataPath, m.Groups[1].Value);
				System.Drawing.Bitmap b = new System.Drawing.Bitmap(strImageFile);
				m_doc.BackgroundImages.AddBgImage("Welcome", -1, "", b);
				return true;
			}
			m = Regex.Match(strCommand, "fill_bgsprite\\s+(\\d),(\\d)");
			if (m.Success)
			{
				int x = Int32.Parse(m.Groups[1].Value);
				int y = Int32.Parse(m.Groups[2].Value);
				Sprite s = m_doc.BackgroundSpritesets.Current.CurrentSprite;
				s.FloodFillClick(x, y);
				return true;
			}
			m = Regex.Match(strCommand, "export full");
			if (m.Success)
			{
				string strDir = CalcPath(Options.Debug_TutorialPath, strProjectName);
				if (!Directory.Exists(strDir))
					Directory.CreateDirectory(strDir);
				return m_doc.Filer.Export(strDir, fNDS, true, false);
			}

			Print("Unknown command: {0}", strCommand);
			return false;
		}

	}
}
