using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Spritely
{
	public partial class OldMainForm : Form
	{
		/// <summary>
		/// The selection has changed, update the display.
		/// </summary>
		private void B_UpdateSelection()
		{
			m_doc.FlushBitmaps();
			pbBM_SpriteList.Invalidate();
			pbBM_EditBackgroundMap.Invalidate();
		}

		// Copy the BackgroundSprite palette into the BackgroundMap palette.
		// This is called when the user switches to the BackgroundMap tab.
		public void UpdateBackgroundMapPalette()
		{
			//TODO: fix this
			Palette bgpal = m_doc.GetBackgroundPalette(Options.DefaultBgPaletteId);
			//Palette bmpal = m_doc.GetBackgroundMapPalette();
			//bmpal.CopySubpalettes(bgpal);
		}

		#region Edit Background Map

		private bool m_fEditBackgroundMap_Selecting = false;

		private void EditBackgroundMap_MouseDown(object sender, MouseEventArgs e)
		{
			m_fEditBackgroundMap_Selecting = true;
			if (m_doc.BackgroundMaps.CurrentMap.HandleMouse_EditMap(e.X, e.Y))
			{
				pbBM_SpriteList.Invalidate();
				pbBM_EditBackgroundMap.Invalidate();
				m_doc.HasUnsavedChanges = true;
			}
		}

		private void EditBackgroundMap_MouseMove(object sender, MouseEventArgs e)
		{
			Map m = m_doc.BackgroundMaps.CurrentMap;
			if (m_fEditBackgroundMap_Selecting)
			{
				if (m.HandleMouse_EditMap(e.X, e.Y))
				{
					pbBM_EditBackgroundMap.Invalidate();
					m_doc.HasUnsavedChanges = true;
				}
			}
			if (m.HandleMouseMove_EditMap(e.X, e.Y))
			{
				pbBM_EditBackgroundMap.Invalidate();
			}
		}

		private void EditBackgroundMap_MouseLeave(object sender, EventArgs e)
		{
			//Map m = m_doc.BackgroundMaps.CurrentMap;
			//if (m.HandleMouseMove_EditMap(-10, -10))
			//{
			//	pbBM_EditBackgroundMap.Invalidate();
			//}
		}

		private void EditBackgroundMap_MouseUp(object sender, MouseEventArgs e)
		{
			//m_fEditBackgroundMap_Selecting = false;
		}

		private void EditBackgroundMap_Paint(object sender, PaintEventArgs e)
		{
			//m_doc.BackgroundMaps.CurrentMap.DrawBackgroundMap(e.Graphics);
		}

		#endregion

		private void pbBM_SpritePreview_Paint(object sender, PaintEventArgs e)
		{
			//Graphics g = e.Graphics;
			//g.DrawRectangle(Pens.Black, 0, 0, 64, 64);
		}

	}
}
