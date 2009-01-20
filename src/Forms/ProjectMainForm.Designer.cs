namespace Spritely
{
	partial class ProjectMainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary
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
			this.tabSet = new System.Windows.Forms.TabControl();
			this.tabBackgrounds = new System.Windows.Forms.TabPage();
			this.tabSprites = new System.Windows.Forms.TabPage();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tabSet.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabSet
			// 
			this.tabSet.Controls.Add(this.tabSprites);
			this.tabSet.Controls.Add(this.tabBackgrounds);
			this.tabSet.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabSet.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			this.tabSet.ItemSize = new System.Drawing.Size(0, 20);
			this.tabSet.Location = new System.Drawing.Point(0, 24);
			this.tabSet.Margin = new System.Windows.Forms.Padding(6);
			this.tabSet.Name = "tabSet";
			this.tabSet.Padding = new System.Drawing.Point(15, 3);
			this.tabSet.SelectedIndex = 0;
			this.tabSet.Size = new System.Drawing.Size(742, 25);
			this.tabSet.TabIndex = 2;
			this.tabSet.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabSet_DrawItem);
			this.tabSet.SelectedIndexChanged += new System.EventHandler(this.tabSet_SelectedIndexChanged);
			// 
			// tabBackgrounds
			// 
			this.tabBackgrounds.Location = new System.Drawing.Point(4, 24);
			this.tabBackgrounds.Name = "tabBackgrounds";
			this.tabBackgrounds.Size = new System.Drawing.Size(734, 0);
			this.tabBackgrounds.TabIndex = 1;
			this.tabBackgrounds.Text = "Backgrounds";
			this.tabBackgrounds.UseVisualStyleBackColor = true;
			// 
			// tabSprites
			// 
			this.tabSprites.Location = new System.Drawing.Point(4, 24);
			this.tabSprites.Name = "tabSprites";
			this.tabSprites.Size = new System.Drawing.Size(734, 0);
			this.tabSprites.TabIndex = 0;
			this.tabSprites.Text = "Sprites";
			this.tabSprites.UseVisualStyleBackColor = true;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(742, 24);
			this.menuStrip1.TabIndex = 4;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// ProjectMainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(742, 546);
			this.Controls.Add(this.tabSet);
			this.Controls.Add(this.menuStrip1);
			this.IsMdiContainer = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "ProjectMainForm";
			this.Text = "ProjectForm";
			this.tabSet.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabSet;
		private System.Windows.Forms.TabPage tabSprites;
		private System.Windows.Forms.TabPage tabBackgrounds;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
	}
}