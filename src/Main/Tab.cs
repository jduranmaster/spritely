using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public class OldTab
	{
		public enum Type
		{
			Sprites = 0,
			BackgroundSprites,
			BackgroundMap,
			MAX,
			Unknown,
		};

		private OldMainForm m_owner;

		public Type TabType;

		public PictureBox PaletteWindow;
		public PictureBox PaletteSelectWindow;
		public PictureBox PaletteSwatchWindow;
		public Label RedLabel;
		public Label GreenLabel;
		public Label BlueLabel;
		public HScrollBar RedScrollbar;
		public HScrollBar GreenScrollbar;
		public HScrollBar BlueScrollbar;

		public PictureBox SpriteListWindow;
		public VScrollBar SpriteListScrollbar;
		public PictureBox EditSpriteWindow;

		public PictureBox EditMapWindow;

		public Toolbox Toolbox;
		public PictureBox ToolboxWindow;
		public ComboBox ToolboxZoomCombobox;

		public OldTab(Type type, OldMainForm owner)
		{
			m_owner = owner;
			TabType = type;
		}

		public Palettes Palettes
		{
			get
			{
				if (TabType == Type.Sprites)
					return m_owner.Doc.Palettes;
				else
					return m_owner.Doc.BackgroundPalettes;
			}
		}

		public Spritesets Spritesets
		{
			get
			{
				if (TabType == Type.Sprites)
					return m_owner.Doc.Spritesets;
				else
					return m_owner.Doc.BackgroundSpritesets;
			}
		}

		public SpriteList SpriteList
		{
			get
			{
				if (TabType == Type.Sprites)
					return m_owner.Doc.Spritesets.Current.SpriteList;
				else
					return m_owner.Doc.BackgroundSpritesets.Current.SpriteList;
			}
		}

	}
}
