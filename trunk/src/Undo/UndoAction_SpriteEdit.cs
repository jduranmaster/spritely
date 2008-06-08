using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	public class UndoAction_SpriteEdit : UndoAction
	{
		UndoMgr m_mgr;
		SpriteList m_spritelist;
		Sprite m_sprite;
		Sprite.UndoData m_before;
		Sprite.UndoData m_after;

		public UndoAction_SpriteEdit(UndoMgr mgr, SpriteList slist, Sprite sprite, Sprite.UndoData before, Sprite.UndoData after, string strDesc)
		{
			m_mgr = mgr;
			m_spritelist = slist;
			m_sprite = sprite;
			m_before = new Sprite.UndoData(before);
			m_after = new Sprite.UndoData(after);

			Description = "SpriteEdit " + sprite.Name + " " + strDesc;
			if (IsPaletteChange())
				Description += " " + before.palette + " to " + after.palette;
		}

		public Sprite GetSprite
		{
			get { return m_sprite; }
		}

		public bool IsPaletteChange()
		{
			return m_before.palette != m_after.palette;
		}

		public bool IsPixelChange()
		{
			if (m_before.width != m_after.width || m_before.height != m_after.height)
				return true;

			int nTiles = m_before.width * m_before.height;
			for (int i = 0; i < nTiles; i++)
			{
				if (!m_before.tiles[i].Equals(m_after.tiles[i]))
					return true;
			}
			return false;
		}

		public override void ApplyUndo()
		{
			m_sprite.ApplyUndoData(m_before);
			m_spritelist.CurrentSprite = m_sprite;
		}

		public override void ApplyRedo()
		{
			m_sprite.ApplyUndoData(m_after);
			m_spritelist.CurrentSprite = m_sprite;
		}
	}
}
	