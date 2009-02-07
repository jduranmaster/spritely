namespace Spritely
{
	partial class OptionsEdit
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
			this.bOK = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.tcOptions = new System.Windows.Forms.TabControl();
			this.tabSprite = new System.Windows.Forms.TabPage();
			this.cbSprite_TileGrid = new System.Windows.Forms.CheckBox();
			this.cbSprite_PixelGrid = new System.Windows.Forms.CheckBox();
			this.cbSprite_ShowPaletteIndex = new System.Windows.Forms.CheckBox();
			this.tabPalette = new System.Windows.Forms.TabPage();
			this.cbPalette_ShowPaletteIndex = new System.Windows.Forms.CheckBox();
			this.tabMap = new System.Windows.Forms.TabPage();
			this.cbMap_ShowGrid = new System.Windows.Forms.CheckBox();
			this.cbMap_ShowScreen = new System.Windows.Forms.CheckBox();
			this.tcOptions.SuspendLayout();
			this.tabSprite.SuspendLayout();
			this.tabPalette.SuspendLayout();
			this.tabMap.SuspendLayout();
			this.SuspendLayout();
			// 
			// bOK
			// 
			this.bOK.Location = new System.Drawing.Point(343, 220);
			this.bOK.Name = "bOK";
			this.bOK.Size = new System.Drawing.Size(92, 23);
			this.bOK.TabIndex = 2;
			this.bOK.Text = "OK";
			this.bOK.UseVisualStyleBackColor = true;
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// bCancel
			// 
			this.bCancel.Location = new System.Drawing.Point(245, 220);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(92, 23);
			this.bCancel.TabIndex = 1;
			this.bCancel.Text = "Cancel";
			this.bCancel.UseVisualStyleBackColor = true;
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// tcOptions
			// 
			this.tcOptions.Controls.Add(this.tabSprite);
			this.tcOptions.Controls.Add(this.tabPalette);
			this.tcOptions.Controls.Add(this.tabMap);
			this.tcOptions.Location = new System.Drawing.Point(12, 12);
			this.tcOptions.Name = "tcOptions";
			this.tcOptions.SelectedIndex = 0;
			this.tcOptions.Size = new System.Drawing.Size(423, 202);
			this.tcOptions.TabIndex = 0;
			// 
			// tabSprite
			// 
			this.tabSprite.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabSprite.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tabSprite.Controls.Add(this.cbSprite_TileGrid);
			this.tabSprite.Controls.Add(this.cbSprite_PixelGrid);
			this.tabSprite.Controls.Add(this.cbSprite_ShowPaletteIndex);
			this.tabSprite.Location = new System.Drawing.Point(4, 22);
			this.tabSprite.Name = "tabSprite";
			this.tabSprite.Padding = new System.Windows.Forms.Padding(3);
			this.tabSprite.Size = new System.Drawing.Size(415, 176);
			this.tabSprite.TabIndex = 0;
			this.tabSprite.Text = "Sprite Edit";
			this.tabSprite.UseVisualStyleBackColor = true;
			// 
			// cbSprite_TileGrid
			// 
			this.cbSprite_TileGrid.AutoSize = true;
			this.cbSprite_TileGrid.Location = new System.Drawing.Point(20, 43);
			this.cbSprite_TileGrid.Name = "cbSprite_TileGrid";
			this.cbSprite_TileGrid.Size = new System.Drawing.Size(89, 17);
			this.cbSprite_TileGrid.TabIndex = 3;
			this.cbSprite_TileGrid.Text = "Show tile grid";
			this.cbSprite_TileGrid.UseVisualStyleBackColor = true;
			// 
			// cbSprite_PixelGrid
			// 
			this.cbSprite_PixelGrid.AutoSize = true;
			this.cbSprite_PixelGrid.Location = new System.Drawing.Point(20, 20);
			this.cbSprite_PixelGrid.Name = "cbSprite_PixelGrid";
			this.cbSprite_PixelGrid.Size = new System.Drawing.Size(97, 17);
			this.cbSprite_PixelGrid.TabIndex = 2;
			this.cbSprite_PixelGrid.Text = "Show pixel grid";
			this.cbSprite_PixelGrid.UseVisualStyleBackColor = true;
			// 
			// cbSprite_ShowPaletteIndex
			// 
			this.cbSprite_ShowPaletteIndex.AutoSize = true;
			this.cbSprite_ShowPaletteIndex.Location = new System.Drawing.Point(20, 66);
			this.cbSprite_ShowPaletteIndex.Name = "cbSprite_ShowPaletteIndex";
			this.cbSprite_ShowPaletteIndex.Size = new System.Drawing.Size(271, 17);
			this.cbSprite_ShowPaletteIndex.TabIndex = 1;
			this.cbSprite_ShowPaletteIndex.Text = "Show the palette index on each pixel (16x, 32x only)";
			this.cbSprite_ShowPaletteIndex.UseVisualStyleBackColor = true;
			// 
			// tabPalette
			// 
			this.tabPalette.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tabPalette.Controls.Add(this.cbPalette_ShowPaletteIndex);
			this.tabPalette.Location = new System.Drawing.Point(4, 22);
			this.tabPalette.Name = "tabPalette";
			this.tabPalette.Size = new System.Drawing.Size(415, 176);
			this.tabPalette.TabIndex = 1;
			this.tabPalette.Text = "Palette Edit";
			this.tabPalette.UseVisualStyleBackColor = true;
			// 
			// cbPalette_ShowPaletteIndex
			// 
			this.cbPalette_ShowPaletteIndex.AutoSize = true;
			this.cbPalette_ShowPaletteIndex.Location = new System.Drawing.Point(20, 20);
			this.cbPalette_ShowPaletteIndex.Name = "cbPalette_ShowPaletteIndex";
			this.cbPalette_ShowPaletteIndex.Size = new System.Drawing.Size(239, 17);
			this.cbPalette_ShowPaletteIndex.TabIndex = 1;
			this.cbPalette_ShowPaletteIndex.Text = "Show the palette index on each color swatch";
			this.cbPalette_ShowPaletteIndex.UseVisualStyleBackColor = true;
			// 
			// tabMap
			// 
			this.tabMap.Controls.Add(this.cbMap_ShowGrid);
			this.tabMap.Controls.Add(this.cbMap_ShowScreen);
			this.tabMap.Location = new System.Drawing.Point(4, 22);
			this.tabMap.Name = "tabMap";
			this.tabMap.Size = new System.Drawing.Size(415, 176);
			this.tabMap.TabIndex = 2;
			this.tabMap.Text = "Map Edit";
			this.tabMap.UseVisualStyleBackColor = true;
			// 
			// cbMap_ShowGrid
			// 
			this.cbMap_ShowGrid.AutoSize = true;
			this.cbMap_ShowGrid.Location = new System.Drawing.Point(20, 43);
			this.cbMap_ShowGrid.Name = "cbMap_ShowGrid";
			this.cbMap_ShowGrid.Size = new System.Drawing.Size(89, 17);
			this.cbMap_ShowGrid.TabIndex = 2;
			this.cbMap_ShowGrid.Text = "Show tile grid";
			this.cbMap_ShowGrid.UseVisualStyleBackColor = true;
			// 
			// cbMap_ShowScreen
			// 
			this.cbMap_ShowScreen.AutoSize = true;
			this.cbMap_ShowScreen.Location = new System.Drawing.Point(20, 20);
			this.cbMap_ShowScreen.Name = "cbMap_ShowScreen";
			this.cbMap_ShowScreen.Size = new System.Drawing.Size(267, 17);
			this.cbMap_ShowScreen.TabIndex = 1;
			this.cbMap_ShowScreen.Text = "Show a red rectangle marking the screen boundary";
			this.cbMap_ShowScreen.UseVisualStyleBackColor = true;
			// 
			// OptionsEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(447, 255);
			this.Controls.Add(this.tcOptions);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionsEdit";
			this.Text = "Spritely Options";
			this.tcOptions.ResumeLayout(false);
			this.tabSprite.ResumeLayout(false);
			this.tabSprite.PerformLayout();
			this.tabPalette.ResumeLayout(false);
			this.tabPalette.PerformLayout();
			this.tabMap.ResumeLayout(false);
			this.tabMap.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.TabControl tcOptions;
		private System.Windows.Forms.TabPage tabSprite;
		private System.Windows.Forms.CheckBox cbSprite_ShowPaletteIndex;
		private System.Windows.Forms.TabPage tabPalette;
		private System.Windows.Forms.CheckBox cbPalette_ShowPaletteIndex;
		private System.Windows.Forms.TabPage tabMap;
		private System.Windows.Forms.CheckBox cbMap_ShowGrid;
		private System.Windows.Forms.CheckBox cbMap_ShowScreen;
		private System.Windows.Forms.CheckBox cbSprite_TileGrid;
		private System.Windows.Forms.CheckBox cbSprite_PixelGrid;
	}
}