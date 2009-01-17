namespace Spritely
{
	partial class SpriteForm
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
			this.pbTools = new System.Windows.Forms.PictureBox();
			this.pbSprite = new System.Windows.Forms.PictureBox();
			this.cbZoom = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.pbTools)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbSprite)).BeginInit();
			this.SuspendLayout();
			// 
			// pbTools
			// 
			this.pbTools.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbTools.Location = new System.Drawing.Point(5, 5);
			this.pbTools.Name = "pbTools";
			this.pbTools.Size = new System.Drawing.Size(55, 231);
			this.pbTools.TabIndex = 0;
			this.pbTools.TabStop = false;
			this.pbTools.MouseLeave += new System.EventHandler(this.pbTools_MouseLeave);
			this.pbTools.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbTools_MouseMove);
			this.pbTools.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbTools_MouseDown);
			this.pbTools.Paint += new System.Windows.Forms.PaintEventHandler(this.pbTools_Paint);
			this.pbTools.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbTools_MouseUp);
			// 
			// pbSprite
			// 
			this.pbSprite.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pbSprite.Location = new System.Drawing.Point(65, 5);
			this.pbSprite.Name = "pbSprite";
			this.pbSprite.Size = new System.Drawing.Size(259, 252);
			this.pbSprite.TabIndex = 1;
			this.pbSprite.TabStop = false;
			this.pbSprite.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbSprite_MouseMove);
			this.pbSprite.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbSprite_MouseDown);
			this.pbSprite.Paint += new System.Windows.Forms.PaintEventHandler(this.pbSprite_Paint);
			this.pbSprite.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbSprite_MouseUp);
			// 
			// cbZoom
			// 
			this.cbZoom.FormattingEnabled = true;
			this.cbZoom.Items.AddRange(new object[] {
            "1x",
            "2x",
            "4x",
            "8x",
            "16x",
            "32x"});
			this.cbZoom.Location = new System.Drawing.Point(5, 243);
			this.cbZoom.Name = "cbZoom";
			this.cbZoom.Size = new System.Drawing.Size(55, 21);
			this.cbZoom.TabIndex = 2;
			this.cbZoom.SelectedIndexChanged += new System.EventHandler(this.cbZoom_SelectedIndexChanged);
			// 
			// SpriteForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(329, 263);
			this.Controls.Add(this.cbZoom);
			this.Controls.Add(this.pbSprite);
			this.Controls.Add(this.pbTools);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(208, 297);
			this.Name = "SpriteForm";
			this.Text = "SpriteForm";
			((System.ComponentModel.ISupportInitialize)(this.pbTools)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbSprite)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pbTools;
		private System.Windows.Forms.PictureBox pbSprite;
		private System.Windows.Forms.ComboBox cbZoom;
	}
}