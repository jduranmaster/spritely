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
		#region Palette Select

		private bool m_fPaletteSelect_Selecting = false;
		private int m_fPaletteSelect_OriginalPalette = 0;

		private void PaletteSelect_MouseDown(object sender, MouseEventArgs e)
		{
			PictureBox pbPaletteSelect = sender as PictureBox;
			Tab tab = GetTab_PaletteSelect(pbPaletteSelect);

			Sprite s = m_doc.GetCurrentSprite(tab);
			if (s == null)
				return;
			m_fPaletteSelect_OriginalPalette = s.PaletteID;
			m_fPaletteSelect_Selecting = true;
			if (m_doc.GetSpritePalette(tab).HandleSelectorMouse(e.X, e.Y))
			{
				UpdatePaletteSelect(tab);
				m_doc.HasUnsavedChanges = true;
			}
		}

		private void PaletteSelect_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fPaletteSelect_Selecting)
			{
				PictureBox pbPaletteSelect = sender as PictureBox;
				Tab tab = GetTab_PaletteSelect(pbPaletteSelect);

				if (m_doc.GetSpritePalette(tab).HandleSelectorMouse(e.X, e.Y))
				{
					UpdatePaletteSelect(tab);
					m_doc.HasUnsavedChanges = true;
				}
			}
		}

		private void PaletteSelect_MouseUp(object sender, MouseEventArgs e)
		{
			PictureBox pbPaletteSelect = sender as PictureBox;
			Tab tab = GetTab_PaletteSelect(pbPaletteSelect);

			if (!m_fPaletteSelect_Selecting)
				return;
			m_fPaletteSelect_Selecting = false;

			// Record an undo action if the current palette selection has changed
			Sprite s = m_doc.GetCurrentSprite(tab);
			if (s != null && m_fPaletteSelect_OriginalPalette != s.PaletteID)
			{
				s.RecordUndoAction("palette select");
			}
		}

		private void PaletteSelect_Paint(object sender, PaintEventArgs e)
		{
			PictureBox pbPaletteSelect = sender as PictureBox;
			Tab tab = GetTab_PaletteSelect(pbPaletteSelect);

			m_doc.GetSpritePalette(tab).DrawSelector(e.Graphics);
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
		private void UpdatePaletteSelect(Tab tab)
		{
			UpdatePaletteColor(tab);
			if (tab == Tab.Sprites || tab == Tab.BackgroundSprites)
			{
				Sprite s = m_doc.GetCurrentSprite(tab);
				if (s != null)
					s.PaletteID = m_doc.GetSpritePalette(tab).CurrentSubpaletteID;
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
		private void UpdatePaletteColor(Tab tab)
		{
			Subpalette p = m_doc.GetSpritePalette(tab).CurrentSubpalette;

			// Setting the scrollbar values will fire a Palette_ColorScrollbar_ValueChanged
			// message which will (in turn) call us again. Set a flag to prevent this
			// infinite recursion.
			m_fUpdatePalette = false;

			HScrollBar sbRed = GetRedScrollbar(tab);
			HScrollBar sbGreen = GetGreenScrollbar(tab);
			HScrollBar sbBlue = GetBlueScrollbar(tab);

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

		/// <summary>
		/// This is called when:
		///   The current palette has changed (indirectly from S_AdjustPaletteScrollbars)
		///   The current color in the palette has changed (indirectly from S_AdjustPaletteScrollbars)
		///   The scrollbars are used to edit the current color (from the scrollbar event handler)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Palette_ColorScrollbar_ValueChanged(object sender, EventArgs e)
		{
			// Don't try to update the palettes if we're already in the middle of
			// an update.
			if (m_fUpdatePalette)
			{
				// This section is entered only when the scrollbar handler is called as a result of
				// the user clicking on the scrollbar.
				Subpalette p = m_doc.GetSpritePalette(m_eCurrentTab).CurrentSubpalette;
				HScrollBar sbRed = GetRedScrollbar(m_eCurrentTab);
				HScrollBar sbGreen = GetGreenScrollbar(m_eCurrentTab);
				HScrollBar sbBlue = GetBlueScrollbar(m_eCurrentTab);
				p.UpdateColor(sbRed.Value, sbGreen.Value, sbBlue.Value);

				UpdatePalette(m_eCurrentTab);
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
		private void UpdatePalette(Tab tab)
		{
			PictureBox pbox;

			m_doc.FlushBitmaps();
			pbox = GetSpriteListWindow(tab);
			if (pbox != null)
				pbox.Invalidate();
			pbox = GetEditSpriteWindow(tab);
			if (pbox != null)
				pbox.Invalidate();
			pbox = GetEditMapWindow(tab);
			if (pbox != null)
				pbox.Invalidate();
			pbox = GetPaletteWindow(tab);
			if (pbox != null)
				pbox.Invalidate();
			pbox = GetPaletteSelectWindow(tab);
			if (pbox != null)
				pbox.Invalidate();
			pbox = GetPaletteSwatchWindow(tab);
			if (pbox != null)
				pbox.Invalidate();
			UpdatePaletteHexValues(tab);
		}

		private void UpdatePaletteHexValues(Tab tab)
		{
			UpdatePaletteHexValue(GetRedLabel(tab), GetRedScrollbar(tab));
			UpdatePaletteHexValue(GetGreenLabel(tab), GetGreenScrollbar(tab));
			UpdatePaletteHexValue(GetBlueLabel(tab), GetBlueScrollbar(tab));
		}

		private void UpdatePaletteHexValue(Label l, HScrollBar sb)
		{
			if (l == null || sb == null)
				return;
			l.Text = String.Format("{0:X2}", sb.Value);
		}

		private bool m_fPalette_Selecting = false;
		private int m_fPalette_OriginalColor = 0;

		// This is called when the user presses the mouse in the palette.
		private void Palette_MouseDown(object sender, MouseEventArgs e)
		{
			PictureBox pbPalette = sender as PictureBox;
			Tab tab = GetTab_Palette(pbPalette);
			Subpalette palette = m_doc.GetSpritePalette(tab).CurrentSubpalette;

			m_fPalette_OriginalColor = palette.CurrentColor;
			m_fPalette_Selecting = true;
			if (palette.HandleMouse(e.X, e.Y))
			{
				UpdatePaletteColor(tab);
			}
		}

		private void Palette_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fPalette_Selecting)
			{
				PictureBox pbPalette = sender as PictureBox;
				Tab tab = GetTab_Palette(pbPalette);
				Subpalette palette = m_doc.GetSpritePalette(tab).CurrentSubpalette;

				if (palette.HandleMouse(e.X, e.Y))
				{
					UpdatePaletteColor(tab);
				}
			}
		}

		// This is called when the user releases the mouse after pressing it within the palette
		private void Palette_MouseUp(object sender, MouseEventArgs e)
		{
			PictureBox pbPalette = sender as PictureBox;
			Tab tab = GetTab_Palette(pbPalette);
			Subpalette palette = m_doc.GetSpritePalette(tab).CurrentSubpalette;

			m_fPalette_Selecting = false;

			// Record an undo action if the current color selection has changed
			if (m_fPalette_OriginalColor != palette.CurrentColor)
			{
				palette.RecordUndoAction("select color");
			}
		}

		private void Palette_Paint(object sender, PaintEventArgs e)
		{
			PictureBox pbPalette = sender as PictureBox;
			Tab tab = GetTab_Palette(pbPalette);
			Subpalette palette = m_doc.GetSpritePalette(tab).CurrentSubpalette;

			palette.Draw(e.Graphics);
		}

		#endregion

		#region Palette Swatch

		private void PaletteSwatch_Paint(object sender, PaintEventArgs e)
		{
			PictureBox pbSwatch = sender as PictureBox;
			Tab tab = GetTab_PaletteSwatch(pbSwatch);
			m_doc.GetSpritePalette(tab).CurrentSubpalette.DrawSwatch(e.Graphics);
		}

		#endregion
	}
}
