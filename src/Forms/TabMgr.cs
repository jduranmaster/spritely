using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public class TabMgr
	{
		private ProjectMainForm m_owner;
		private TabId m_eId;
		private List<Form> m_winSpritesets;
		private List<Form> m_winSprites;
		private List<Form> m_winPalettes;
		private List<Form> m_winMaps;

		public enum TabId
		{
			Sprites=0,
			Backgrounds,
			MAX,
		};

		public TabMgr(ProjectMainForm owner, TabId id)
		{
			m_owner = owner;
			m_eId = id;
			m_winSpritesets = new List<Form>();
			m_winSprites = new List<Form>();
			m_winPalettes = new List<Form>();
			m_winMaps = new List<Form>();
		}

		public TabId Id
		{
			get { return m_eId; }
		}

		public void AddSpritesetWindow(Form f)
		{
			if (!m_winSpritesets.Contains(f))
				m_winSpritesets.Add(f);
		}

		public void AddSpriteWindow(Form f)
		{
			if (!m_winSprites.Contains(f))
				m_winSprites.Add(f);
		}

		public void AddPaletteWindow(Form f)
		{
			if (!m_winPalettes.Contains(f))
				m_winPalettes.Add(f);
		}

		public void AddMapWindow(Form f)
		{
			if (!m_winMaps.Contains(f))
				m_winMaps.Add(f);
		}

		public void RemoveWindow(Form f)
		{
			if (m_winSpritesets.Contains(f))
				m_winSpritesets.Remove(f);
			if (m_winSprites.Contains(f))
				m_winSprites.Remove(f);
			if (m_winPalettes.Contains(f))
				m_winPalettes.Remove(f);
			if (m_winMaps.Contains(f))
				m_winMaps.Remove(f);
		}

		public void RemoveAllWindows()
		{
			foreach (Form f in m_winSpritesets)
				f.Dispose();
			m_winSpritesets.Clear();
			foreach (Form f in m_winSprites)
				f.Dispose();
			m_winSprites.Clear();
			foreach (Form f in m_winPalettes)
				f.Dispose();
			m_winPalettes.Clear();
			foreach (Form f in m_winMaps)
				f.Dispose();
			m_winMaps.Clear();
		}

		public void ShowWindows()
		{
			foreach (Form f in m_winSpritesets)
				f.Show();
			foreach (Form f in m_winSprites)
				f.Show();
			foreach (Form f in m_winPalettes)
				f.Show();
			foreach (Form f in m_winMaps)
				f.Show();
		}

		public void HideWindows()
		{
			foreach (Form f in m_winSpritesets)
				f.Hide();
			foreach (Form f in m_winSprites)
				f.Hide();
			foreach (Form f in m_winPalettes)
				f.Hide();
			foreach (Form f in m_winMaps)
				f.Hide();
		}

		public void CloseWindows()
		{
			foreach (Form f in m_winSpritesets)
				f.Close();
			foreach (Form f in m_winSprites)
				f.Close();
			foreach (Form f in m_winPalettes)
				f.Close();
			foreach (Form f in m_winMaps)
				f.Close();
		}

		/// <summary>
		/// The width of a sprite window that is best suited for allowing
		/// the editing of a single tile. This is used on the Background Map
		/// editing tab to size the sprite window and to calculate the x-offset
		/// for the map window.
		/// </summary>
		const int k_pxSprite1IdealWidth = 206;
		const int k_pxSprite2IdealWidth = 330;
		const int k_pxSprite4IdealWidth = 591;
		const int k_pxMapIdealWidth = 591;

		public void ArrangeWindows()
		{
			if (m_winSpritesets.Count == 0
					|| m_winSprites.Count == 0
					|| m_winPalettes.Count == 0
					|| (m_eId == TabId.Backgrounds && m_winMaps.Count == 0)
				)
				return;

			Form ss = m_winSpritesets[0];
			ss.Top = 0;
			ss.Left = 0;

			Form p = m_winPalettes[0];
			p.Top = ss.Height;
			p.Left = 0;

			Form s = m_winSprites[0];
			s.Top = 0;
			s.Left = ss.Right;
			s.Height = m_owner.ContentHeight;
			if (m_eId == TabId.Backgrounds)
			{
				Form m = m_winMaps[0];
				m.Top = 0;
				m.Height = m_owner.ContentHeight;

				// Balance width of sprite and map windows in the remaining space.
				int pxSpace = m_owner.ContentWidth - s.Left;

				// Is there room for a map and 2-tile wide sprite?
				if (pxSpace >= k_pxSprite2IdealWidth + k_pxMapIdealWidth)
				{
					s.Width = k_pxSprite2IdealWidth;
					m.Left = s.Right;
					m.Width = k_pxMapIdealWidth;
				}
				else
				{
					// Make bgsprite window = 1 tile width.
					s.Width = k_pxSprite1IdealWidth;
					m.Left = s.Right;
					// Make map window as big as possible.
					if (pxSpace >= k_pxSprite1IdealWidth + k_pxMapIdealWidth)
						m.Width = k_pxMapIdealWidth;
					else
						m.Width = m_owner.ContentWidth - s.Right;
				}
			}
			else
			{
				// Expand window to fill all remaining space.
				s.Width = m_owner.ContentWidth - s.Left;
			}

			if (m_eId == TabId.Backgrounds)
			{
			}
		}
	}
}
