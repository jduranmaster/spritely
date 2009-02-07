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

			m_tileSpriteX = -1;
			m_tileSpriteY = -1;
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
		
		private void MapForm_Resize(object sender, EventArgs e)
		{
			m_parent.ResizeSubwindow(this);
		}

		private void MapForm_FormClosing(object sender, FormClosingEventArgs e)
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
		/// One of the colors in the palette has changed.
		/// </summary>
		public void ColorDataChanged()
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

		private bool m_fToolbox_Selecting = false;

		private void pbTools_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_toolbox.HandleMouse(e.X, e.Y))
			{
				pbTools.Invalidate();
			}
			m_fToolbox_Selecting = true;
		}

		private void pbTools_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fToolbox_Selecting)
			{
				if (m_toolbox.HandleMouse(e.X, e.Y))
					pbTools.Invalidate();
			}
			else
			{
				if (m_toolbox.HandleMouseMove(e.X, e.Y))
					pbTools.Invalidate();
			}
		}

		private void pbTools_MouseUp(object sender, MouseEventArgs e)
		{
			m_fToolbox_Selecting = false;
			pbTools.Invalidate();
		}

		private void pbTools_MouseLeave(object sender, EventArgs e)
		{
			if (m_toolbox.HandleMouseMove(-10, -10))
			{
				pbTools.Invalidate();
			}
		}

		private void pbTools_Paint(object sender, PaintEventArgs e)
		{
			m_toolbox.Draw(e.Graphics);
		}

		#endregion

		#region Map

		private bool m_fEditBackgroundMap_Selecting = false;

		private void pbMap_MouseDown(object sender, MouseEventArgs e)
		{
			m_fEditBackgroundMap_Selecting = true;
			pbMap_MouseMove(sender, e);
		}

		private void pbMap_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fEditBackgroundMap_Selecting)
			{
				if (HandleMouse_EditMap(e.X, e.Y))
				{
					m_parent.HandleMapDataChange(m_map);
				}
			}

			// Update boundary rect for currently selected sprite.
			if (HandleMouseMove_EditMap(e.X, e.Y))
			{
				pbMap.Invalidate();
			}
		}

		private void pbMap_MouseUp(object sender, MouseEventArgs e)
		{
			if (HandleMouseMove_EditMap(-10, -10))
			{
				pbMap.Invalidate();
			}
			m_fEditBackgroundMap_Selecting = false;
		}

		public bool HandleMouse_EditMap(int pxX, int pxY)
		{
			Sprite spriteSelected = m_ss.CurrentSprite;
			if (spriteSelected == null)
				return false;

			if (pxX < 0 || pxY < 0)
				return false;

			// Convert screen pixel (x,y) to map coordinate (x,y).
			int x = pxX / Tile.SmallBitmapScreenSize;
			int y = pxY / Tile.SmallBitmapScreenSize;

			if (x >= k_nMaxMapTilesX || y >= k_nMaxMapTilesY)
				return false;

			bool fUpdate = false;
			int nIndex = 0;
			for (int iy = 0; iy < spriteSelected.TileHeight; iy++)
			{
				if (y + iy >= k_nMaxMapTilesY)
				{
					nIndex++;
					continue;
				}
				for (int ix = 0; ix < spriteSelected.TileWidth; ix++)
				{
					if (x + ix >= k_nMaxMapTilesX)
					{
						nIndex++;
						continue;
					}
					m_map.SetBackgroundTile(x + ix, y + iy,
							spriteSelected.FirstTileId + nIndex,
							spriteSelected.SubpaletteID);
					nIndex++;
					fUpdate = true;
				}
			}

			return fUpdate;
		}

		public bool HandleMouseMove_EditMap(int pxX, int pxY)
		{
			Sprite spriteSelected = m_ss.CurrentSprite;
			if (spriteSelected == null)
				return false;

			// Convert screen pixel (x,y) to map coordinate (x,y).
			int x = pxX / Tile.SmallBitmapScreenSize;
			int y = pxY / Tile.SmallBitmapScreenSize;

			if (pxX < 0 || pxY < 0 || x >= k_nMaxMapTilesX || y >= k_nMaxMapTilesY)
			{
				// Turn off the hilight if we currently have one.
				if (m_tileSpriteX != -1 || m_tileSpriteY != -1)
				{
					m_tileSpriteX = -1;
					m_tileSpriteY = -1;
					return true;
				}
				return false;
			}

			// Has the current mouse position changed?
			if (x != m_tileSpriteX || y != m_tileSpriteY)
			{
				m_tileSpriteX = x;
				m_tileSpriteY = y;
				return true;
			}

			// No change.
			return false;
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
						Tile t = s.GetTile(nTileId - s.FirstTileId);
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
