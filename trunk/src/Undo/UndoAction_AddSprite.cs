using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	public class UndoAction_AddSprite : UndoAction
	{
		UndoMgr m_mgr;
		Spriteset m_ss;
		Sprite m_sprite;
		bool m_fAdd;

		public UndoAction_AddSprite(UndoMgr mgr, Spriteset ss, Sprite sprite, bool fAdd)
		{
			m_mgr = mgr;
			m_ss = ss;
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
				m_ss.SpriteList.RemoveSprite(m_sprite, false);
				m_ss.CurrentSprite = m_mgr.FindMostRecentSprite();
				if (m_ss.CurrentSprite == null)
					m_ss.SelectFirstSprite();
			}
			else
			{
				m_ss.SpriteList.AddSprite(m_sprite, null);
				m_ss.CurrentSprite = m_sprite;
			}
		}

		public override void ApplyRedo()
		{
			if (m_fAdd)
			{
				m_ss.SpriteList.AddSprite(m_sprite, null);
				m_ss.CurrentSprite = m_sprite;
			}
			else
			{
				m_ss.SpriteList.RemoveSprite(m_sprite, false);
				m_ss.CurrentSprite = m_mgr.FindMostRecentSprite();
				if (m_ss.CurrentSprite == null)
					m_ss.SelectFirstSprite();
			}
		}

		#region Tests

		[TestFixture]
		public class UndoAction_AddSprite_Test
		{
			Document m_doc;
			Palette m_palette;
			Spriteset m_ss;

			UndoMgr m_mgr;

			[SetUp]
			public void TestInit()
			{
				m_doc = new Document(null);
				Assert.IsNotNull(m_doc);

				m_palette = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
				Assert.IsNotNull(m_palette);

				m_ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", m_palette);
				Assert.IsNotNull(m_ss);

				m_mgr = m_doc.Undo(TabMgr.TabId.Sprites);
				Assert.IsNotNull(m_mgr);
			}

			[Test]
			public void Test_AddSprite_noundo()
			{
				Sprite s = m_ss.AddSprite(1, 1, "sample", 0, "", 0, null);
				Assert.IsNotNull(s);
				Assert.AreEqual(0, m_mgr.Count);
				Assert.IsFalse(m_mgr.CanUndo());
				Assert.IsFalse(m_mgr.CanRedo());
			}

			[Test]
			public void Test_AddSprite()
			{
				Sprite s = m_ss.AddSprite(1, 1, "sample", 0, "", 0, m_mgr);
				Assert.IsNotNull(s);
				Assert.AreEqual(1, m_mgr.Count);
				Assert.IsTrue(m_mgr.CanUndo());
				Assert.IsFalse(m_mgr.CanRedo());
			}

		}

		#endregion

	}
}
