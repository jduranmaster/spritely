using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	public class UndoAction_AddSprite : UndoAction
	{
		UndoMgr m_mgr;
		SpriteList m_spritelist;
		Sprite m_sprite;
		bool m_fAdd;

		public UndoAction_AddSprite(UndoMgr mgr, SpriteList spritelist, Sprite sprite, bool fAdd)
		{
			m_mgr = mgr;
			m_spritelist = spritelist;
			m_sprite = sprite;
			m_fAdd = fAdd;

			Description = (fAdd ? "AddSprite " : "RemoveSprite ") + sprite.Name;
		}

		public Sprite GetSprite
		{
			get { return m_sprite; }
		}

		public bool Add
		{
			get { return m_fAdd; }
		}

		public override void ApplyUndo()
		{
			if (m_fAdd)
			{
				m_spritelist.RemoveSprite(m_sprite, false);
				m_spritelist.CurrentSprite = m_mgr.FindMostRecentSprite();
				if (m_spritelist.CurrentSprite == null)
					m_spritelist.SelectFirstSprite();
			}
			else
			{
				m_spritelist.AddSprite(m_sprite, false);
				m_spritelist.CurrentSprite = m_sprite;
			}
		}

		public override void ApplyRedo()
		{
			if (m_fAdd)
			{
				m_spritelist.AddSprite(m_sprite, false);
				m_spritelist.CurrentSprite = m_sprite;
			}
			else
			{
				m_spritelist.RemoveSprite(m_sprite, false);
				m_spritelist.CurrentSprite = m_mgr.FindMostRecentSprite();
				if (m_spritelist.CurrentSprite == null)
					m_spritelist.SelectFirstSprite();
			}
		}
	}
}
