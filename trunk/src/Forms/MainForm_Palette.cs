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
		#region Palette Select

		private void S_PaletteSelect_MouseDown(object sender, MouseEventArgs e)
		{
		}

		private void BS_PaletteSelect_MouseDown(object sender, MouseEventArgs e)
		{
		}

		private void BM_PaletteSelect_MouseDown(object sender, MouseEventArgs e)
		{
		}

		private void PaletteSelect_MouseDown(OldTab tab, MouseEventArgs e)
		{
		}

		private void S_PaletteSelect_MouseMove(object sender, MouseEventArgs e)
		{
		}

		private void BS_PaletteSelect_MouseMove(object sender, MouseEventArgs e)
		{
		}

		private void BM_PaletteSelect_MouseMove(object sender, MouseEventArgs e)
		{
		}

		private void PaletteSelect_MouseMove(OldTab tab, MouseEventArgs e)
		{
		}

		private void S_PaletteSelect_MouseUp(object sender, MouseEventArgs e)
		{
		}

		private void BS_PaletteSelect_MouseUp(object sender, MouseEventArgs e)
		{
		}

		private void BM_PaletteSelect_MouseUp(object sender, MouseEventArgs e)
		{
		}

		private void PaletteSelect_MouseUp(OldTab tab)
		{
		}

		private void S_PaletteSelect_Paint(object sender, PaintEventArgs e)
		{
		}

		private void BS_PaletteSelect_Paint(object sender, PaintEventArgs e)
		{
		}

		private void BM_PaletteSelect_Paint(object sender, PaintEventArgs e)
		{
		}

		private void PaletteSelect_Paint(OldTab tab, PaintEventArgs e)
		{
		}

		#endregion

		#region Palette

		/// <summary>
		/// This is called when:
		///   The current palette has changed
		/// It is not called when:
		///   The current color in the palette has changed
		///   The scrollbars are used to edit the current color
		/// </summary>
		private void UpdatePaletteSelect(OldTab tab)
		{
			UpdatePaletteColor(tab);
			if (tab.TabType == OldTab.Type.Sprites || tab.TabType == OldTab.Type.BackgroundSprites)
			{
				//Sprite s = tab.Spritesets.Current.CurrentSprite;
				//if (s != null)
				//	s.PaletteID = tab.Palettes.CurrentPalette.CurrentSubpaletteID;
			}
		}

		private bool m_fUpdatePalette = true;

		/// <summary>
		/// This is called when:
		///   The current palette has changed
		///   The current color in the palette has changed
		/// It is not called when:
		///   The scrollbars are used to edit the current color
		/// </summary>
		private void UpdatePaletteColor(OldTab tab)
		{
			Subpalette p = null;// tab.Palettes.CurrentPalette.CurrentSubpalette;

			// Setting the scrollbar values will fire a Palette_ColorScrollbar_ValueChanged
			// message which will (in turn) call us again. Set a flag to prevent this
			// infinite recursion.
			m_fUpdatePalette = false;

			HScrollBar sbRed = tab.RedScrollbar;
			HScrollBar sbGreen = tab.GreenScrollbar;
			HScrollBar sbBlue = tab.BlueScrollbar;

			if (sbRed != null && sbGreen != null && sbBlue != null)
			{
				sbRed.Value = p.Red();
				sbGreen.Value = p.Green();
				sbBlue.Value = p.Blue();

				p.UpdateColor(sbRed.Value, sbGreen.Value, sbBlue.Value);
			}
			UpdatePalette(tab);
			m_fUpdatePalette = true;
		}

		private void S_Palette_ColorScrollbar_ValueChanged(object sender, EventArgs e)
		{
			Palette_ColorScrollbar_ValueChanged(GetTab(OldTab.Type.Sprites));
		}

		private void BS_Palette_ColorScrollbar_ValueChanged(object sender, EventArgs e)
		{
			Palette_ColorScrollbar_ValueChanged(GetTab(OldTab.Type.BackgroundSprites));
		}

		/// <summary>
		/// This is called when:
		///   The current palette has changed (indirectly from S_AdjustPaletteScrollbars)
		///   The current color in the palette has changed (indirectly from S_AdjustPaletteScrollbars)
		///   The scrollbars are used to edit the current color (from the scrollbar event handler)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Palette_ColorScrollbar_ValueChanged(OldTab tab)
		{
			// Don't try to update the palettes if we're already in the middle of
			// an update.
			if (m_fUpdatePalette)
			{
				// This section is entered only when the scrollbar handler is called as a result of
				// the user clicking on the scrollbar.
				Subpalette p = null;// tab.Palettes.GetPalette(0).CurrentSubpalette;
				HScrollBar sbRed = tab.RedScrollbar;
				HScrollBar sbGreen = tab.GreenScrollbar;
				HScrollBar sbBlue = tab.BlueScrollbar;
				p.UpdateColor(sbRed.Value, sbGreen.Value, sbBlue.Value);

				UpdatePalette(tab);
				m_doc.HasUnsavedChanges = true;

				p.RecordUndoAction("scrollbar palette change");
			}
		}

		/// <summary>
		/// The colors in the palette have changed, update the display.
		/// This is called when:
		///   The current palette has changed (from S_AdjustPaletteScrollbars)
		///   The current color in the palette has changed (from S_AdjustPaletteScrollbars)
		///   The scrollbars are used to edit the current color (from the scrollbar event handler)
		///   A new file has been loaded.
		/// </summary>
		private void UpdatePalette(OldTab tab)
		{
			PictureBox pbox;

			m_doc.FlushBitmaps();
			pbox = tab.SpriteListWindow;
			if (pbox != null)
				pbox.Invalidate();
			pbox = tab.EditSpriteWindow;
			if (pbox != null)
				pbox.Invalidate();
			pbox = tab.EditMapWindow;
			if (pbox != null)
				pbox.Invalidate();
			pbox = tab.PaletteWindow;
			if (pbox != null)
				pbox.Invalidate();
			pbox = tab.PaletteSelectWindow;
			if (pbox != null)
				pbox.Invalidate();
			pbox = tab.PaletteSwatchWindow;
			if (pbox != null)
				pbox.Invalidate();
			UpdatePaletteHexValues(tab);
		}

		private void UpdatePaletteHexValues(OldTab tab)
		{
			UpdatePaletteHexValue(tab.RedLabel, tab.RedScrollbar);
			UpdatePaletteHexValue(tab.GreenLabel, tab.GreenScrollbar);
			UpdatePaletteHexValue(tab.BlueLabel, tab.BlueScrollbar);
		}

		private void UpdatePaletteHexValue(Label l, HScrollBar sb)
		{
			if (l == null || sb == null)
				return;
			l.Text = String.Format("{0:X2}", sb.Value);
		}

		private void S_Palette_MouseDown(object sender, MouseEventArgs e)
		{
		}

		private void BS_Palette_MouseDown(object sender, MouseEventArgs e)
		{
		}

		// This is called when the user presses the mouse in the palette.
		private void Palette_MouseDown(OldTab tab, MouseEventArgs e)
		{
		}

		private void S_Palette_MouseMove(object sender, MouseEventArgs e)
		{
		}

		private void BS_Palette_MouseMove(object sender, MouseEventArgs e)
		{
		}

		private void Palette_MouseMove(OldTab tab, MouseEventArgs e)
		{
		}

		private void S_Palette_MouseUp(object sender, MouseEventArgs e)
		{
		}

		private void BS_Palette_MouseUp(object sender, MouseEventArgs e)
		{
		}

		// This is called when the user releases the mouse after pressing it within the palette
		private void Palette_MouseUp(OldTab tab)
		{
		}

		private void S_Palette_Paint(object sender, PaintEventArgs e)
		{
		}

		private void BS_Palette_Paint(object sender, PaintEventArgs e)
		{
		}

		private void BM_Palette_Paint(object sender, PaintEventArgs e)
		{
		}

		private void Palette_Paint(OldTab tab, PaintEventArgs e)
		{
		}

		#endregion

		#region Palette Swatch

		private void S_PaletteSwatch_Paint(object sender, PaintEventArgs e)
		{
		}

		private void BS_PaletteSwatch_Paint(object sender, PaintEventArgs e)
		{
		}

		private void PaletteSwatch_Paint(OldTab tab, PaintEventArgs e)
		{
		}

		#endregion
	}
}
