using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class SpriteForm : Form
	{
		private ProjectMainForm m_parent;

		private Spriteset m_ss;
		private Sprite m_sprite;

		private Toolbox_Sprite m_toolbox;

		public static int m_pxBigBitmapSize = 4;
		public static int BigBitmapPixelSize
		{
			get { return m_pxBigBitmapSize; }
			set { m_pxBigBitmapSize = value; }
		}

		public static int BigBitmapScreenSize
		{
			get { return BigBitmapPixelSize * Tile.TileSize; }
		}

		public enum ZoomLevel
		{
			Zoom_1x = 0,
			Zoom_2x,
			Zoom_4x,
			Zoom_8x,
			Zoom_16x,
			Zoom_32x,
		};

		public SpriteForm(ProjectMainForm parent, Spriteset ss, Sprite s)
		{
			m_parent = parent;

			m_ss = ss;

			InitializeComponent();

			SetSprite(s);
			m_toolbox = new Toolbox_Sprite();

			MdiParent = parent;
			FormBorderStyle = FormBorderStyle.SizableToolWindow;
			StartPosition = FormStartPosition.Manual;
			Visible = false;
			ControlBox = false;

			// Set to 16x.
			cbZoom.SelectedIndex = (int)ZoomLevel.Zoom_16x;
		}

		public Sprite Sprite
		{
			get { return m_sprite; }
		}

		public void SetSprite(Sprite s)
		{
			m_sprite = s;
			pbSprite.Invalidate();

			// Update title with sprite name.
			if (s != null)
				Text = "Sprite '" + s.Name + "'";
		}

		#region Window events
		
		private void SpriteForm_Resize(object sender, EventArgs e)
		{
			m_parent.ResizeSubwindow(this);
		}

		private void SpriteForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_parent.CloseSubwindow(this);
			e.Cancel = true;
		}

		#endregion

		#region Subwindow updates

		/// <summary>
		/// The selected sprite has changed.
		/// </summary>
		public void SpriteSelectionChanged()
		{
			SetSprite(m_ss.CurrentSprite);
			pbSprite.Invalidate();
		}

		/// <summary>
		/// The data for one of the sprites has changed.
		/// </summary>
		public void SpriteDataChanged()
		{
			pbSprite.Invalidate();
		}

		/// <summary>
		/// The currently subpalette selection has changed.
		/// </summary>
		public void SubpaletteSelectChanged()
		{
			pbSprite.Invalidate();
		}

		#endregion

		#region Toolbox

		private void pbTools_Paint(object sender, PaintEventArgs e)
		{
			m_toolbox.Draw(e.Graphics);
		}

		private void pbTools_MouseDown(object sender, MouseEventArgs e)
		{
		
		}

		private void pbTools_MouseLeave(object sender, EventArgs e)
		{
		
		}

		private void pbTools_MouseMove(object sender, MouseEventArgs e)
		{
		
		}

		private void pbTools_MouseUp(object sender, MouseEventArgs e)
		{

		}

		#endregion

		#region Sprite

		private bool m_fEditSprite_Selecting = false;
		private bool m_fEditSprite_Erase = false;

		private void pbSprite_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_sprite == null)
				return;

			m_fEditSprite_Selecting = true;

			// Right click is handled the same as the Eraser tool
			m_fEditSprite_Erase = (e.Button == MouseButtons.Right);

			pbSprite_MouseMove(sender, e);
		}

		private void pbSprite_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fEditSprite_Selecting)
			{
				Toolbox.ToolType tool = m_toolbox.CurrentTool;
				if (m_fEditSprite_Erase)
					tool = Toolbox.ToolType.Eraser;

				if (HandleMouseMove(e.X, e.Y, tool))
				{
					if (tool == Toolbox.ToolType.Eyedropper)
					{
						//UpdatePaletteSelect(tab);
					}
					else
					{
						m_sprite.FlushBitmaps();
						m_parent.HandleSpriteDataChanged(m_ss);
					}
				}
			}
		}

		private void pbSprite_MouseUp(object sender, MouseEventArgs e)
		{
			if (m_fEditSprite_Selecting)
			{
				Toolbox.ToolType tool = m_toolbox.CurrentTool;

				// Close out the edit action.
				FinishEdit(tool);

				// Record the undo action.
				string strTool = "";
				bool fRecordUndo = false;
				switch (tool)
				{
					//case Toolbox.ToolType.Select - no  undo
					case Toolbox.ToolType.Pencil:
						strTool = "pencil";
						fRecordUndo = true;
						break;
					//case Toolbox.ToolType.Eyedropper - no undo
					case Toolbox.ToolType.FloodFill:
						strTool = "bucket";
						fRecordUndo = true;
						break;
					case Toolbox.ToolType.Eraser:
						strTool = "eraser";
						break;
				}

				if (fRecordUndo)
				{
					Sprite s = m_ss.CurrentSprite;
					if (s != null)
						s.RecordUndoAction(strTool);
				}
			}

			m_fEditSprite_Selecting = false;
		}

		private bool HandleMouseMove(int pxX, int pxY, Toolbox.ToolType tool)
		{
			if (m_sprite == null || pxX < 0 || pxY < 0)
				return false;

			// Convert screen pixel (x,y) to sprite pixel index (x,y).
			int pxSpriteX = pxX / BigBitmapPixelSize;
			int pxSpriteY = pxY / BigBitmapPixelSize;

			// Ignore if pixel is outside bounds of current sprite.
			if (pxSpriteX >= m_sprite.PixelWidth || pxSpriteY >= m_sprite.PixelHeight)
				return false;

			if (tool == Toolbox.ToolType.FloodFill)
				return FloodFillClick(pxSpriteX, pxSpriteY);

			// Convert sprite pixel coords (x,y) into tile index (x,y) and tile pixel coords (x,y).
			int tileX = pxSpriteX / Tile.TileSize;
			int pxTileX = pxSpriteX % Tile.TileSize;
			int tileY = pxSpriteY / Tile.TileSize;
			int pxTileY = pxSpriteY % Tile.TileSize;

			int nTileIndex = (tileY * m_sprite.TileWidth) + tileX;
			Palette p = m_parent.ActivePalette();
			Subpalette sp = p.GetCurrentSubpalette();

			if (tool == Toolbox.ToolType.Eyedropper)
			{
				int nCurrColor = m_sprite.GetPixel(pxSpriteX, pxSpriteY);
				if (nCurrColor == sp.CurrentColor)
					return false;
				sp.CurrentColor = nCurrColor;
				return true;
			}

			Tile t = m_sprite.GetTile(nTileIndex);
			int nColor = tool == Toolbox.ToolType.Eraser ? 0 : sp.CurrentColor;

			// Same color - no need to update.
			if (t.GetPixel(pxTileX, pxTileY) == nColor)
				return false;

			// Set the new color.
			t.SetPixel(pxTileX, pxTileY, nColor);

			return true;
		}

		/// <summary>
		/// Finish an editing operation.
		/// </summary>
		private void FinishEdit(Toolbox.ToolType tool)
		{
			switch (tool)
			{
				case Toolbox.ToolType.Eraser:
				case Toolbox.ToolType.Eyedropper:
				case Toolbox.ToolType.FloodFill:
				case Toolbox.ToolType.Pencil:
					// These are immediate tools - nothing to finish up.
					return;
			}

			//return m_Tiles[nTileIndex].Click(pxX, pxY, tool == Toolbox.ToolType.Eraser);
		}

		private void pbSprite_Paint(object sender, PaintEventArgs e)
		{
			if (m_sprite == null)
				return;

			Graphics g = e.Graphics;

			for (int iRow = 0; iRow < m_sprite.TileHeight; iRow++)
			{
				for (int iColumn = 0; iColumn < m_sprite.TileWidth; iColumn++)
				{
					int pxX = (iColumn * BigBitmapScreenSize);
					int pxY = (iRow * BigBitmapScreenSize);
					DrawTile(g, m_sprite.GetTile((iRow * m_sprite.TileWidth) + iColumn), pxX, pxY);
				}
			}

			// Draw the grid and border.
			int pxPixelSize = BigBitmapPixelSize;
			int pxTileSize = BigBitmapScreenSize;
			int pxX0 = 0;
			int pxY0 = 0;
			int pxX1 = m_sprite.TileWidth * pxTileSize;
			int pxY1 = m_sprite.TileHeight * pxTileSize;

			Pen penPixelBorder = Pens.LightGray;
			Pen penSpriteBorder = Pens.Gray;

			// Draw border around each pixel.
			if (BigBitmapPixelSize > 2 && Options.Sprite_ShowPixelGrid)
			{
				for (int i = pxX0 + pxPixelSize; i < pxX1; i += pxPixelSize)
					g.DrawLine(penPixelBorder, i, pxY0, i, pxY1);
				for (int i = pxY0 + pxPixelSize; i < pxY1; i += pxPixelSize)
					g.DrawLine(penPixelBorder, pxX0, i, pxX1, i);
			}
			else
			{
				penSpriteBorder = Pens.LightGray;
			}

			// Draw a border around each sprite.
			if (BigBitmapPixelSize > 1 && Options.Sprite_ShowTileGrid)
			{
				for (int i = pxX0 + pxTileSize; i < pxX1; i += pxTileSize)
					g.DrawLine(penSpriteBorder, i, pxY0, i, pxY1);
				for (int i = pxY0 + pxTileSize; i < pxY1; i += pxTileSize)
					g.DrawLine(penSpriteBorder, pxX0, i, pxX1, i);
			}

			// Draw the outer border.
			g.DrawLine(Pens.Black, pxX0, pxY0, pxX1, pxY0);	// Top
			g.DrawLine(Pens.Black, pxX0, pxY0, pxX0, pxY1);	// Left
			g.DrawLine(Pens.Black, pxX0, pxY1, pxX1, pxY1);	// Bottom
			g.DrawLine(Pens.Black, pxX1, pxY0, pxX1, pxY1);	// Right
		}

		public void DrawTile(Graphics g, Tile t, int pxOriginX, int pxOriginY)
		{
			// create a new bitmap
			int pxSize = BigBitmapPixelSize;
			int pxInset = pxSize / 4;
			Subpalette sp = m_sprite.Subpalette;

			Font f;
			int[] nXOffset;
			int nYOffset;
			if (pxSize == 16)
			{
				f = new Font("Arial", 9);
				nXOffset = new int[16] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 2, 2, 2, 3 };
				nYOffset = 1;
			}
			else
			{
				f = new Font("Arial Black", 10);
				nXOffset = new int[16] { 10, 9, 10, 10, 10, 10, 10, 10, 10, 10, 9, 9, 9, 9, 9, 10 };
				nYOffset = 6;
			}

			g.FillRectangle(Brushes.White, pxOriginX, pxOriginY, Tile.TileSize * pxSize, Tile.TileSize * pxSize);
			for (int iRow = 0; iRow < Tile.TileSize; iRow++)
			{
				for (int iColumn = 0; iColumn < Tile.TileSize; iColumn++)
				{
					int nPaletteIndex = t.GetPixel(iColumn, iRow);

					int pxX0 = pxOriginX + (iColumn * pxSize);
					int pxY0 = pxOriginY + (iRow * pxSize);
					g.FillRectangle(sp.Brush(nPaletteIndex), pxX0, pxY0, pxSize, pxSize);

					// Draw a red X over the transparent color (index 0).
					if (Options.Sprite_ShowRedXForTransparent && nPaletteIndex == 0 && BigBitmapPixelSize >= 8)
					{
						int pxX0i = pxX0 + pxInset;
						int pxY0i = pxY0 + pxInset;
						int pxX1i = pxX0 + pxSize - pxInset;
						int pxY1i = pxY0 + pxSize - pxInset;
						g.DrawLine(Pens.Firebrick, pxX0i, pxY0i, pxX1i, pxY1i);
						g.DrawLine(Pens.Firebrick, pxX0i, pxY1i, pxX1i, pxY0i);
					}

					if (Options.Sprite_ShowPaletteIndex && pxSize >= 16)
					{
						// Draw the palette index in each pixel.
						int pxLabelOffsetX = nXOffset[nPaletteIndex];
						int pxLabelOffsetY = nYOffset;
						g.DrawString(sp.Label(nPaletteIndex), f, sp.LabelBrush(nPaletteIndex), pxX0 + pxLabelOffsetX, pxY0 + pxLabelOffsetY);
					}
				}
			}
		}

		#endregion

		#region FloodFill tool

		public class PixelCoord
		{
			public int X, Y;

			public PixelCoord(int x, int y)
			{
				X = x;
				Y = y;
			}
		}

		/// <summary>
		/// Perform a floodfill click at the specified (x,y) pixel coords
		/// </summary>
		/// <param name="pxSpriteX">Click x-position (in pixels)</param>
		/// <param name="pxSpriteY">Click y-position (in pixels)</param>
		/// <returns>True if the sprite changes as a result of this click, false otherwise.</returns>
		public bool FloodFillClick(int pxSpriteX, int pxSpriteY)
		{
			int colorOld = m_sprite.GetPixel(pxSpriteX, pxSpriteY);
			int colorNew = 0;// p.CurrentColorIndex; //TODO Palette.CurrentColor;
			if (colorOld == colorNew)
				return false;

			// Stack of pixels to process.
			Stack<PixelCoord> stackPixels = new Stack<PixelCoord>();

			PixelCoord p = new PixelCoord(pxSpriteX, pxSpriteY);
			stackPixels.Push(p);

			while (stackPixels.Count != 0)
			{
				p = stackPixels.Pop();
				m_sprite.SetPixel(p.X, p.Y, colorNew);

				FloodFill_CheckPixel(p.X - 1, p.Y, colorOld, ref stackPixels);
				FloodFill_CheckPixel(p.X + 1, p.Y, colorOld, ref stackPixels);
				FloodFill_CheckPixel(p.X, p.Y - 1, colorOld, ref stackPixels);
				FloodFill_CheckPixel(p.X, p.Y + 1, colorOld, ref stackPixels);
			}

			m_sprite.FlushBitmaps();
			return true;
		}

		private void FloodFill_CheckPixel(int x, int y, int color, ref Stack<PixelCoord> stack)
		{
			if (x >= 0 && x < m_sprite.PixelWidth
				&& y >= 0 && y < m_sprite.PixelHeight
				&& m_sprite.GetPixel(x, y) == color)
			{
				stack.Push(new PixelCoord(x, y));
			}
		}

		#endregion

		private void cbZoom_SelectedIndexChanged(object sender, EventArgs e)
		{
			BigBitmapPixelSize = 1 << cbZoom.SelectedIndex;
			pbSprite.Invalidate();
		}

	}
}
