using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Spritely
{
	public partial class MainForm : Form
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
			PaletteMgr bgpal = m_doc.GetSpritePalettes(Tab.BackgroundSprites);
			PaletteMgr bmpal = m_doc.GetSpritePalettes(Tab.BackgroundMap);
			bmpal.CopyPalettes(bgpal);
		}

		#region Edit Background Map

		private bool m_fEditBackgroundMap_Selecting = false;

		private void EditBackgroundMap_MouseDown(object sender, MouseEventArgs e)
		{
			m_fEditBackgroundMap_Selecting = true;
			if (m_doc.GetSprites(Tab.BackgroundSprites).Map.HandleMouse_EditMap(e.X, e.Y))
			{
				pbBM_SpriteList.Invalidate();
				pbBM_EditBackgroundMap.Invalidate();
				m_doc.HasUnsavedChanges = true;
			}
		}

		private void EditBackgroundMap_MouseMove(object sender, MouseEventArgs e)
		{
			SpriteList sl = m_doc.GetSprites(Tab.BackgroundSprites);
			if (m_fEditBackgroundMap_Selecting)
			{
				if (sl.Map.HandleMouse_EditMap(e.X, e.Y))
				{
					pbBM_EditBackgroundMap.Invalidate();
					m_doc.HasUnsavedChanges = true;
				}
			}
			if (sl.Map.HandleMouseMove_EditMap(e.X, e.Y))
			{
				pbBM_EditBackgroundMap.Invalidate();
			}
		}

		private void EditBackgroundMap_MouseLeave(object sender, EventArgs e)
		{
			SpriteList sl = m_doc.GetSprites(Tab.BackgroundSprites);
			if (sl.Map.HandleMouseMove_EditMap(-10, -10))
			{
				pbBM_EditBackgroundMap.Invalidate();
			}
		}

		private void EditBackgroundMap_MouseUp(object sender, MouseEventArgs e)
		{
			m_fEditBackgroundMap_Selecting = false;
		}

		private void EditBackgroundMap_Paint(object sender, PaintEventArgs e)
		{
			m_doc.GetSprites(Tab.BackgroundSprites).Map.DrawBackgroundMap(e.Graphics);
		}

		#endregion

		private void pbBM_SpritePreview_Paint(object sender, PaintEventArgs e)
		{
			//Graphics g = e.Graphics;
			//g.DrawRectangle(Pens.Black, 0, 0, 128, 128);
		}

	}
}
