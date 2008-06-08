using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class Export : Form
	{
		static string m_strLastExportDirectory = "";

		public Export()
		{
			InitializeComponent();

			this.DialogResult = DialogResult.Cancel;

			// Reset any invalid directories.
			if (m_strLastExportDirectory != "" && !System.IO.Directory.Exists(m_strLastExportDirectory))
				m_strLastExportDirectory = "";

			// Set the default directory to be the same as the application's directory.
			if (m_strLastExportDirectory == "")
				m_strLastExportDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

			tbLocation.Text = m_strLastExportDirectory;
			rbGBA.Checked = true;
			rbSprites.Checked = true;
		}

		private void bBrowse_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog SaveFolderDialog = new FolderBrowserDialog();
			//SaveFolderDialog.Description = "Select the directory where you want to store the exported files:";
			SaveFolderDialog.Description = ResourceMgr.GetString("SelectExportDir");
			SaveFolderDialog.SelectedPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
			if (SaveFolderDialog.ShowDialog() != DialogResult.OK)
				return;

			tbLocation.Text = SaveFolderDialog.SelectedPath;

			// TODO: verify that the path doesn't contain spaces or punctuation
			if (tbLocation.Text.IndexOfAny(new char[] { ' ' }) != -1)
			{
				//MessageBox.Show("The directory path that you've chosen contains at least one space (' ').\r\nWhile this is a valid Windows path, the spaces will cause problems for the Gameboy/Nintendo DS development tools.\r\nPlease select a path that does not contain these characters.", "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
				MessageBox.Show(ResourceMgr.GetString("InvalidPath"), ResourceMgr.GetString("InvalidPathTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void bExport_Click(object sender, EventArgs e)
		{
			m_strLastExportDirectory = tbLocation.Text;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		public string ExportLocation
		{
			get { return tbLocation.Text; }
		}

		public bool NDS
		{
			get { return rbNDS.Checked; }
		}

		public bool Project
		{
			get { return rbProject.Checked; }
		}

		public bool UpdateProject
		{
			get { return rbUpdateProject.Checked; }
		}
	}
}
