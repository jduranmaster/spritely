namespace Spritely
{
	partial class ProjectTreeViewForm
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
			this.components = new System.ComponentModel.Container();
			this.treeView = new System.Windows.Forms.TreeView();
			this.cmenuSpriteset = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.cmenuSprites_Open = new System.Windows.Forms.ToolStripMenuItem();
			this.cmenuSprites_Duplicate = new System.Windows.Forms.ToolStripMenuItem();
			this.cmenuSprites_Delete = new System.Windows.Forms.ToolStripMenuItem();
			this.cmenuSpritesetTitle = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.cmenuSpritesTitle_AddNewSprites = new System.Windows.Forms.ToolStripMenuItem();
			this.cmenuSpriteset.SuspendLayout();
			this.cmenuSpritesetTitle.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.Indent = 16;
			this.treeView.Location = new System.Drawing.Point(0, 0);
			this.treeView.Name = "treeView";
			this.treeView.ShowNodeToolTips = true;
			this.treeView.Size = new System.Drawing.Size(308, 318);
			this.treeView.TabIndex = 0;
			this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
			// 
			// cmenuSpriteset
			// 
			this.cmenuSpriteset.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmenuSprites_Open,
            this.cmenuSprites_Duplicate,
            this.cmenuSprites_Delete});
			this.cmenuSpriteset.Name = "contextMenuStrip1";
			this.cmenuSpriteset.Size = new System.Drawing.Size(119, 70);
			// 
			// cmenuSprites_Open
			// 
			this.cmenuSprites_Open.Name = "cmenuSprites_Open";
			this.cmenuSprites_Open.Size = new System.Drawing.Size(118, 22);
			this.cmenuSprites_Open.Text = "Open";
			// 
			// cmenuSprites_Duplicate
			// 
			this.cmenuSprites_Duplicate.Enabled = false;
			this.cmenuSprites_Duplicate.Name = "cmenuSprites_Duplicate";
			this.cmenuSprites_Duplicate.Size = new System.Drawing.Size(118, 22);
			this.cmenuSprites_Duplicate.Text = "Duplicate";
			// 
			// cmenuSprites_Delete
			// 
			this.cmenuSprites_Delete.Enabled = false;
			this.cmenuSprites_Delete.Name = "cmenuSprites_Delete";
			this.cmenuSprites_Delete.Size = new System.Drawing.Size(118, 22);
			this.cmenuSprites_Delete.Text = "Delete";
			// 
			// cmenuSpritesetTitle
			// 
			this.cmenuSpritesetTitle.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmenuSpritesTitle_AddNewSprites});
			this.cmenuSpritesetTitle.Name = "cmenuSpritesTitle";
			this.cmenuSpritesetTitle.Size = new System.Drawing.Size(183, 26);
			// 
			// cmenuSpritesTitle_AddNewSprites
			// 
			this.cmenuSpritesTitle_AddNewSprites.Enabled = false;
			this.cmenuSpritesTitle_AddNewSprites.Name = "cmenuSpritesTitle_AddNewSprites";
			this.cmenuSpritesTitle_AddNewSprites.Size = new System.Drawing.Size(182, 22);
			this.cmenuSpritesTitle_AddNewSprites.Text = "Add new set of sprites";
			// 
			// ProjectTreeViewForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(308, 318);
			this.Controls.Add(this.treeView);
			this.MinimizeBox = false;
			this.Name = "ProjectTreeViewForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "ProjectTreeForm";
			this.Load += new System.EventHandler(this.ProjectTreeViewForm_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProjectTreeViewForm_FormClosing);
			this.cmenuSpriteset.ResumeLayout(false);
			this.cmenuSpritesetTitle.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.ContextMenuStrip cmenuSpriteset;
		private System.Windows.Forms.ToolStripMenuItem cmenuSprites_Open;
		private System.Windows.Forms.ContextMenuStrip cmenuSpritesetTitle;
		private System.Windows.Forms.ToolStripMenuItem cmenuSpritesTitle_AddNewSprites;
		private System.Windows.Forms.ToolStripMenuItem cmenuSprites_Duplicate;
		private System.Windows.Forms.ToolStripMenuItem cmenuSprites_Delete;
	}
}