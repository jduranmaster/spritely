using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public abstract class Toolbox
	{
		protected const int k_nToolboxColumns = 2;
		protected const int k_nToolboxRows = 7;

		protected const int k_pxToolboxWidth = 54;
		protected const int k_pxToolboxHeight = 230;

		/// <summary>
		/// Tool indent from edge of toolbox.
		/// </summary>
		protected const int k_pxToolboxIndent = 2;
		/// <summary>
		/// Size of each tool (including border).
		/// </summary>
		protected const int k_pxToolboxToolSize = 26;
		/// <summary>
		/// Indent of tool image with the tool "button".
		/// </summary>
		protected const int k_pxToolImageOffset = 2;

		private int m_pxToolboxOffsetY = 0;

		public int ToolboxOffsetY
		{
			get { return m_pxToolboxOffsetY; }
			set { m_pxToolboxOffsetY = value; }
		}

		/// <summary>
		/// Valid tool types.
		/// </summary>
		public enum ToolType
		{
			Blank=0,
			// Sprite editing tools
			Select,
			Pencil,
			Eyedropper,
			FloodFill,
			Eraser,
			Line,
			Rect,
			RectFilled,
			Ellipse,
			EllipseFilled,
			// Map tools
			GBA,
			NDS,
			//FloodFill,
			RubberStamp,
		};

		private ToolType m_eSelectedTool = ToolType.Blank;

		public ToolType CurrentTool
		{
			get { return m_eSelectedTool; }
			set { m_eSelectedTool = value; }
		}

		public class Tool
		{
			public string Name;
			public ToolType Type;
			public int X;
			public int Y;
			public bool Show;
			public bool Enabled;
			public Bitmap ButtonBitmap;

			public Tool(string strName, ToolType type, int x, int y, bool fShow, bool fEnable, Bitmap bm)
			{
				Name = strName;
				Type = type;
				X = x;
				Y = y;
				Show = fShow;
				Enabled = fEnable;
				ButtonBitmap = bm;
			}
		};
		protected List<Tool> Tools;

		public Toolbox()
		{
			Tools = new List<Tool>();
		}

		/// <summary>
		/// Handle the mouse when the button is held down.
		/// </summary>
		/// <param name="pxX"></param>
		/// <param name="pxY"></param>
		/// <returns>True if a new command was selected</returns>
		public virtual bool HandleMouse(int pxX, int pxY)
		{
			pxY -= ToolboxOffsetY;

			if (pxX < k_pxToolboxIndent || pxY < k_pxToolboxIndent)
				return false;

			// Convert pixel (x,y) to tool (x,y).
			int nX = pxX / k_pxToolboxToolSize;
			int nY = pxY / k_pxToolboxToolSize;

			// Ignore if outside the Toolbox bounds.
			if (nX >= k_nToolboxColumns || nY >= k_nToolboxRows)
				return false;

			foreach (Tool t in Tools)
			{
				if (nX != t.X || nY != t.Y)
					continue;

				if (!t.Show || !t.Enabled)
					return false;

				// Same as the currently selected tool - nothing to do.
				if (m_eSelectedTool == t.Type)
					return false;

				m_eSelectedTool = t.Type;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Handle the mouse move events when the mouse button is not pressed.
		/// This needs to hilight/unhilight the shift arrows as appropriate.
		/// </summary>
		/// <param name="pxX"></param>
		/// <param name="pxY"></param>
		/// <returns>True if we need to redraw the screen</returns>
		public virtual bool HandleMouseMove(int pxX, int pxY)
		{
			return false;
		}

		protected Bitmap s_bmHilite = ResourceMgr.GetBitmap("tool_hilite");
		protected Bitmap s_bmHilite2 = ResourceMgr.GetBitmap("tool_hilite2");

		public virtual void Draw(Graphics g)
		{
			g.DrawRectangle(Pens.LightGray, 0, 0, k_pxToolboxWidth, k_pxToolboxHeight);
			int pxX0, pxY0;

			foreach (Tool t in Tools)
			{
				if (!t.Show)
					continue;

				int iColumn = t.X;
				int iRow = t.Y;

				pxX0 = k_pxToolboxIndent + iColumn * k_pxToolboxToolSize;
				pxY0 = ToolboxOffsetY + k_pxToolboxIndent + iRow * k_pxToolboxToolSize;

				if (t.Type == m_eSelectedTool)
					g.DrawImage(s_bmHilite, pxX0, pxY0);

				g.DrawImage(t.ButtonBitmap, pxX0 + k_pxToolImageOffset, pxY0 + k_pxToolImageOffset);
			}
		}

	}
}
