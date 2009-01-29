using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class MapForm : Form
	{
		private ProjectMainForm m_parent;

		private Map m_map;
		private Spriteset m_ss;
		private Sprite m_sprite;

		private Toolbox_Map m_toolbox;

		private const int k_nMaxMapTilesX = 32;
		private const int k_nMaxMapTilesY = 32;
		private const int k_nGBAScreenTilesX = 30;
		private const int k_nGBAScreenTilesY = 20;
		private const int k_nNDSScreenTilesX = 32;
		private const int k_nNDSScreenTilesY = 24;

		// Tiles to highlight under the cursor in the Background Map.
		private int m_tileSpriteX;
		private int m_tileSpriteY;

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

		/// <summary>
		/// The width of a sprite window that is best suited for allowing
		/// the editing of a single tile. This is used on the Background Map
		/// editing tab to size the sprite window and to calculate the x-offset
		/// for the map window.
		/// </summary>
		public const int SingleTileWindowWidth = 206;

		private static Pen m_penHilight = new Pen(Color.FromArgb(128, Color.Red), 3);
		private static Pen m_penHilight2 = new Pen(Color.FromArgb(128, Color.Black), 1);

		public enum ZoomLevel
		{
			Zoom_1x = 0,
			Zoom_2x,
			Zoom_4x,
			Zoom_8x,
			Zoom_16x,
			Zoom_32x,
		};

		public MapForm(ProjectMainForm parent, Map m, Spriteset ss, Sprite s)
		{
			m_parent = parent;

			m_ss = ss;

			InitializeComponent();

			SetMap(m);
			m_toolbox = new Toolbox_Map();

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

		public void SetMap(Map m)
		{
			m_map = m;
			pbMap.Invalidate();

			// Update title with map name.
			if (m != null)
				Text = "Map '" + m.Name + "'";
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
		}

		/// <summary>
		/// The data for one of the sprites has changed.
		/// </summary>
		public void SpriteDataChanged()
		{
			pbMap.Invalidate();
		}

		/// <summary>
		/// The currently subpalette selection has changed.
		/// </summary>
		public void SubpaletteSelectChanged()
		{
			pbMap.Invalidate();
		}

		/// <summary>
		/// The data for the map has changed.
		/// </summary>
		public void MapDataChanged()
		{
			pbMap.Invalidate();
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

		#region Map

		private bool m_fEditSprite_Selecting = false;
		private bool m_fEditSprite_Erase = false;

		private void pbMap_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_sprite == null)
				return;

			m_fEditSprite_Selecting = true;

			// Right click is handled the same as the Eraser tool
			m_fEditSprite_Erase = (e.Button == MouseButtons.Right);

			pbMap_MouseMove(sender, e);
		}

		private void pbMap_MouseMove(object sender, MouseEventArgs e)
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
						m_parent.HandleMapDataChange(m_map);
					}
				}
			}
		}

		private void pbMap_MouseUp(object sender, MouseEventArgs e)
		{
			if (m_fEditSprite_Selecting)
			{
				Toolbox.ToolType tool = m_toolbox.CurrentTool;

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

			// Remove the old bitmaps.
			m_sprite.FlushBitmaps();
			return true;
		}

		private void pbMap_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			int pxX = 0;
			int pxY = 0;
			for (int ix = 0; ix < k_nMaxMapTilesX; ix++)
			{
				pxY = 0;
				for (int iy = 0; iy < k_nMaxMapTilesY; iy++)
				{
					bool fDrawn = false;
					int nTileId, nSubpalette;
					m_map.GetBackgroundTile(ix, iy, out nTileId, out nSubpalette);
					Sprite s = m_map.Spriteset.FindSprite(nTileId);
					if (s != null)
					{
						Tile t = s.GetTile(nTileId - s.FirstTileID);
						if (t != null)
						{
							t.DrawSmallTile(g, pxX, pxY);
							fDrawn = true;
						}
					}

					if (!fDrawn)
					{
						int pxInset = Tile.SmallBitmapScreenSize / 4;
						int pxX0i = pxX + pxInset;
						int pxY0i = pxY + pxInset;
						int pxX1i = pxX + Tile.SmallBitmapScreenSize - pxInset;
						int pxY1i = pxY + Tile.SmallBitmapScreenSize - pxInset;
						g.DrawLine(Pens.Firebrick, pxX0i, pxY0i, pxX1i, pxY1i);
						g.DrawLine(Pens.Firebrick, pxX0i, pxY1i, pxX1i, pxY0i);
					}
					pxY += Tile.SmallBitmapScreenSize;
				}
				pxX += Tile.SmallBitmapScreenSize;
			}

			// Draw the grid and border.
			int pxTileSize = Tile.SmallBitmapScreenSize;
			pxX = 0;
			pxY = 0;
			int pxWidth = pxTileSize * k_nMaxMapTilesX;
			int pxHeight = pxTileSize * k_nMaxMapTilesY;

			if (Options.BackgroundMap_ShowGrid)
			{
				Pen penTileBorder = Pens.LightGray;

				// Draw a border around each tile.
				for (int i = pxX + pxTileSize; i < pxWidth; i += pxTileSize)
					g.DrawLine(penTileBorder, i, pxY, i, pxHeight);
				for (int i = pxY + pxTileSize; i < pxHeight; i += pxTileSize)
					g.DrawLine(penTileBorder, pxX, i, pxWidth, i);
			}

			// Draw the outer border.
			g.DrawRectangle(Pens.Black, pxX, pxY, pxWidth, pxHeight);

			if (Options.BackgroundMap_ShowScreen)
			{
				if (Options.Platform == Options.PlatformType.GBA)
				{
					pxWidth = pxTileSize * k_nGBAScreenTilesX;
					pxHeight = pxTileSize * k_nGBAScreenTilesY;
				}
				else
				{
					pxWidth = pxTileSize * k_nNDSScreenTilesX;
					pxHeight = pxTileSize * k_nNDSScreenTilesY;
				}
				g.DrawRectangle(m_penHilight, pxX, pxY, pxWidth, pxHeight);
				g.DrawRectangle(m_penHilight2, pxX, pxY, pxWidth, pxHeight);
			}

			// Draw a border around the current background "sprite".
			Sprite spriteSelected = m_ss.CurrentSprite;
			if (m_tileSpriteX != -1 && m_tileSpriteY != -1 && spriteSelected != null)
			{
				pxX = m_tileSpriteX * pxTileSize;
				pxY = m_tileSpriteY * pxTileSize;
				pxWidth = spriteSelected.TileWidth * pxTileSize;
				pxHeight = spriteSelected.TileHeight * pxTileSize;
				g.DrawRectangle(m_penHilight, pxX, pxY, pxWidth, pxHeight);
				g.DrawRectangle(m_penHilight2, pxX, pxY, pxWidth, pxHeight);
			}
		}

		#endregion

		private void cbZoom_SelectedIndexChanged(object sender, EventArgs e)
		{
			BigBitmapPixelSize = 1 << cbZoom.SelectedIndex;
			pbMap.Invalidate();
		}

	}
}
