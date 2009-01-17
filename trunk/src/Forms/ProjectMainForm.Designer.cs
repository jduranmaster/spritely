namespace Spritely
{
	partial class ProjectMainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.menuBar = new System.Windows.Forms.MenuStrip();
			this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
			this.menuFile_Exit = new System.Windows.Forms.ToolStripMenuItem();
			this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.menuWindow = new System.Windows.Forms.ToolStripMenuItem();
			this.menuWindow_ProjectList = new System.Windows.Forms.ToolStripMenuItem();
			this.menuBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuBar
			// 
			this.menuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuEdit,
            this.menuWindow});
			this.menuBar.Location = new System.Drawing.Point(0, 0);
			this.menuBar.Name = "menuBar";
			this.menuBar.Size = new System.Drawing.Size(742, 24);
			this.menuBar.TabIndex = 0;
			this.menuBar.Text = "menuStrip1";
			// 
			// menuFile
			// 
			this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile_Exit});
			this.menuFile.Name = "menuFile";
			this.menuFile.Size = new System.Drawing.Size(35, 20);
			this.menuFile.Text = "&File";
			// 
			// menuFile_Exit
			// 
			this.menuFile_Exit.Name = "menuFile_Exit";
			this.menuFile_Exit.Size = new System.Drawing.Size(92, 22);
			this.menuFile_Exit.Text = "E&xit";
			// 
			// menuEdit
			// 
			this.menuEdit.Name = "menuEdit";
			this.menuEdit.Size = new System.Drawing.Size(37, 20);
			this.menuEdit.Text = "&Edit";
			// 
			// menuWindow
			// 
			this.menuWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuWindow_ProjectList});
			this.menuWindow.Name = "menuWindow";
			this.menuWindow.Size = new System.Drawing.Size(57, 20);
			this.menuWindow.Text = "&Window";
			// 
			// menuWindow_ProjectList
			// 
			this.menuWindow_ProjectList.Checked = true;
			this.menuWindow_ProjectList.CheckState = System.Windows.Forms.CheckState.Checked;
			this.menuWindow_ProjectList.Name = "menuWindow_ProjectList";
			this.menuWindow_ProjectList.Size = new System.Drawing.Size(127, 22);
			this.menuWindow_ProjectList.Text = "&Project List";
			this.menuWindow_ProjectList.Click += new System.EventHandler(this.menuWindow_ProjectList_Click);
			// 
			// ProjectMainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(742, 546);
			this.Controls.Add(this.menuBar);
			this.IsMdiContainer = true;
			this.MainMenuStrip = this.menuBar;
			this.Name = "ProjectMainForm";
			this.Text = "ProjectForm";
			this.menuBar.ResumeLayout(false);
			this.menuBar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuBar;
		private System.Windows.Forms.ToolStripMenuItem menuFile;
		private System.Windows.Forms.ToolStripMenuItem menuEdit;
		private System.Windows.Forms.ToolStripMenuItem menuFile_Exit;
		private System.Windows.Forms.ToolStripMenuItem menuWindow;
		private System.Windows.Forms.ToolStripMenuItem menuWindow_ProjectList;
	}
}