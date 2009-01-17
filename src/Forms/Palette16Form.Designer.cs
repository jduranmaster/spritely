namespace Spritely
{
	partial class Palette16Form
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
			this.pbPaletteSwatch = new System.Windows.Forms.PictureBox();
			this.lBHex = new System.Windows.Forms.Label();
			this.lGHex = new System.Windows.Forms.Label();
			this.lRHex = new System.Windows.Forms.Label();
			this.pbPaletteSelect = new System.Windows.Forms.PictureBox();
			this.sbRed = new System.Windows.Forms.HScrollBar();
			this.sbGreen = new System.Windows.Forms.HScrollBar();
			this.sbBlue = new System.Windows.Forms.HScrollBar();
			this.lB = new System.Windows.Forms.Label();
			this.pbPalette = new System.Windows.Forms.PictureBox();
			this.lR = new System.Windows.Forms.Label();
			this.lG = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pbPaletteSwatch)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbPaletteSelect)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbPalette)).BeginInit();
			this.SuspendLayout();
			// 
			// pbPaletteSwatch
			// 
			this.pbPaletteSwatch.Location = new System.Drawing.Point(14, 97);
			this.pbPaletteSwatch.Name = "pbPaletteSwatch";
			this.pbPaletteSwatch.Size = new System.Drawing.Size(27, 27);
			this.pbPaletteSwatch.TabIndex = 36;
			this.pbPaletteSwatch.TabStop = false;
			this.pbPaletteSwatch.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPaletteSwatch_Paint);
			// 
			// lBHex
			// 
			this.lBHex.AutoSize = true;
			this.lBHex.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lBHex.ForeColor = System.Drawing.Color.Blue;
			this.lBHex.Location = new System.Drawing.Point(174, 122);
			this.lBHex.Name = "lBHex";
			this.lBHex.Size = new System.Drawing.Size(19, 13);
			this.lBHex.TabIndex = 35;
			this.lBHex.Text = "00";
			// 
			// lGHex
			// 
			this.lGHex.AutoSize = true;
			this.lGHex.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lGHex.ForeColor = System.Drawing.Color.Green;
			this.lGHex.Location = new System.Drawing.Point(174, 106);
			this.lGHex.Name = "lGHex";
			this.lGHex.Size = new System.Drawing.Size(19, 13);
			this.lGHex.TabIndex = 34;
			this.lGHex.Text = "00";
			// 
			// lRHex
			// 
			this.lRHex.AutoSize = true;
			this.lRHex.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lRHex.ForeColor = System.Drawing.Color.Red;
			this.lRHex.Location = new System.Drawing.Point(174, 89);
			this.lRHex.Name = "lRHex";
			this.lRHex.Size = new System.Drawing.Size(19, 13);
			this.lRHex.TabIndex = 33;
			this.lRHex.Text = "00";
			// 
			// pbPaletteSelect
			// 
			this.pbPaletteSelect.Location = new System.Drawing.Point(5, 5);
			this.pbPaletteSelect.Name = "pbPaletteSelect";
			this.pbPaletteSelect.Size = new System.Drawing.Size(193, 17);
			this.pbPaletteSelect.TabIndex = 32;
			this.pbPaletteSelect.TabStop = false;
			this.pbPaletteSelect.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbPaletteSelect_MouseMove);
			this.pbPaletteSelect.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPaletteSelect_MouseDown);
			this.pbPaletteSelect.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPaletteSelect_Paint);
			this.pbPaletteSelect.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPaletteSelect_MouseUp);
			// 
			// sbRed
			// 
			this.sbRed.LargeChange = 1;
			this.sbRed.Location = new System.Drawing.Point(71, 85);
			this.sbRed.Maximum = 31;
			this.sbRed.Name = "sbRed";
			this.sbRed.Size = new System.Drawing.Size(100, 17);
			this.sbRed.TabIndex = 26;
			// 
			// sbGreen
			// 
			this.sbGreen.LargeChange = 1;
			this.sbGreen.Location = new System.Drawing.Point(71, 102);
			this.sbGreen.Maximum = 31;
			this.sbGreen.Name = "sbGreen";
			this.sbGreen.Size = new System.Drawing.Size(100, 17);
			this.sbGreen.TabIndex = 27;
			// 
			// sbBlue
			// 
			this.sbBlue.LargeChange = 1;
			this.sbBlue.Location = new System.Drawing.Point(71, 119);
			this.sbBlue.Maximum = 31;
			this.sbBlue.Name = "sbBlue";
			this.sbBlue.Size = new System.Drawing.Size(100, 17);
			this.sbBlue.TabIndex = 28;
			// 
			// lB
			// 
			this.lB.AutoSize = true;
			this.lB.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lB.ForeColor = System.Drawing.Color.Blue;
			this.lB.Location = new System.Drawing.Point(53, 121);
			this.lB.Name = "lB";
			this.lB.Size = new System.Drawing.Size(15, 15);
			this.lB.TabIndex = 31;
			this.lB.Text = "B";
			this.lB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// pbPalette
			// 
			this.pbPalette.Location = new System.Drawing.Point(4, 28);
			this.pbPalette.Name = "pbPalette";
			this.pbPalette.Size = new System.Drawing.Size(195, 51);
			this.pbPalette.TabIndex = 25;
			this.pbPalette.TabStop = false;
			this.pbPalette.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbPalette_MouseMove);
			this.pbPalette.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPalette_MouseDown);
			this.pbPalette.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPalette_Paint);
			this.pbPalette.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPalette_MouseUp);
			// 
			// lR
			// 
			this.lR.AutoSize = true;
			this.lR.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lR.ForeColor = System.Drawing.Color.Red;
			this.lR.Location = new System.Drawing.Point(53, 87);
			this.lR.Name = "lR";
			this.lR.Size = new System.Drawing.Size(15, 15);
			this.lR.TabIndex = 29;
			this.lR.Text = "R";
			this.lR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lG
			// 
			this.lG.AutoSize = true;
			this.lG.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lG.ForeColor = System.Drawing.Color.Green;
			this.lG.Location = new System.Drawing.Point(53, 104);
			this.lG.Name = "lG";
			this.lG.Size = new System.Drawing.Size(15, 15);
			this.lG.TabIndex = 30;
			this.lG.Text = "G";
			this.lG.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// Palette16Form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(204, 145);
			this.Controls.Add(this.pbPaletteSwatch);
			this.Controls.Add(this.lBHex);
			this.Controls.Add(this.lGHex);
			this.Controls.Add(this.lRHex);
			this.Controls.Add(this.pbPaletteSelect);
			this.Controls.Add(this.sbRed);
			this.Controls.Add(this.sbGreen);
			this.Controls.Add(this.sbBlue);
			this.Controls.Add(this.lB);
			this.Controls.Add(this.pbPalette);
			this.Controls.Add(this.lR);
			this.Controls.Add(this.lG);
			this.MinimizeBox = false;
			this.Name = "Palette16Form";
			this.Text = "Palette16Form";
			((System.ComponentModel.ISupportInitialize)(this.pbPaletteSwatch)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbPaletteSelect)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbPalette)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pbPaletteSwatch;
		private System.Windows.Forms.Label lBHex;
		private System.Windows.Forms.Label lGHex;
		private System.Windows.Forms.Label lRHex;
		private System.Windows.Forms.PictureBox pbPaletteSelect;
		private System.Windows.Forms.HScrollBar sbGreen;
		private System.Windows.Forms.HScrollBar sbBlue;
		private System.Windows.Forms.Label lB;
		private System.Windows.Forms.PictureBox pbPalette;
		private System.Windows.Forms.Label lR;
		private System.Windows.Forms.Label lG;
		private System.Windows.Forms.HScrollBar sbRed;

	}
}