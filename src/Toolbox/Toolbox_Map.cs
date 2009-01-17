using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class Toolbox_Map : Toolbox
	{
		protected List<Tool> PlatformTools;

		private ToolType m_eSelectedPlatform = ToolType.GBA;

		public Toolbox_Map() : base()
		{
			ToolboxOffsetY = k_pxToolboxToolSize;

			PlatformTools = new List<Tool>();
			PlatformTools.Add(new Tool("GBA", ToolType.GBA, 0, 0, true, true,
							ResourceMgr.GetBitmap("tool_gba")));
			PlatformTools.Add(new Tool("NDS", ToolType.NDS, 1, 0, true, true,
							ResourceMgr.GetBitmap("tool_nds")));
		}

		public override bool HandleMouse(int pxX, int pxY)
		{
			// Handle the standard tools.
			if (base.HandleMouse(pxX, pxY))
				return true;

			return HandlePlatformToolsMouse(pxX, pxY);
		}

		private bool HandlePlatformToolsMouse(int pxX, int pxY)
		{
			// Handle the platform tools.
			if (pxX < k_pxToolboxIndent || pxY < k_pxToolboxIndent)
				return false;

			// Convert pixel (x,y) to tool (x,y).
			int nX = pxX / k_pxToolboxToolSize;
			int nY = pxY / k_pxToolboxToolSize;

			// Ignore if outside the Toolbox bounds.
			if (nX >= k_nToolboxColumns || nY >= 1)
				return false;

			foreach (Tool t in PlatformTools)
			{
				if (nX != t.X || nY != t.Y)
					continue;

				if (!t.Show || !t.Enabled)
					return false;

				// Same as the currently selected tool - nothing to do.
				if (m_eSelectedPlatform == t.Type)
					return false;

				m_eSelectedPlatform = t.Type;
				Options.Platform = (m_eSelectedPlatform == ToolType.GBA ? Options.PlatformType.GBA
																		: Options.PlatformType.NDS);
				return true;
			}

			return false;
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);

			DrawPlatformTools(g);
		}

		private void DrawPlatformTools(Graphics g)
		{
			int pxX0, pxY0;
			foreach (Tool t in PlatformTools)
			{
				if (!t.Show)
					continue;

				int iColumn = t.X;
				int iRow = t.Y;

				pxX0 = k_pxToolboxIndent + iColumn * k_pxToolboxToolSize;
				pxY0 = k_pxToolboxIndent + iRow * k_pxToolboxToolSize;

				if (t.Type == m_eSelectedPlatform)
					g.DrawImage(s_bmHilite2, pxX0, pxY0);

				g.DrawImage(t.ButtonBitmap, pxX0 + k_pxToolImageOffset, pxY0 + k_pxToolImageOffset);
			}
		}

	}
}
