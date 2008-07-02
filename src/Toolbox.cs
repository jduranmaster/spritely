using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class Toolbox
	{
		private MainForm m_Owner;

		private const int k_nToolboxColumns = 2;
		private const int k_nToolboxRows = 7;

		private const int k_pxToolboxWidth = 54;
		private const int k_pxToolboxHeight = 230;

		/// <summary>
		/// Tool indent from edge of toolbox.
		/// </summary>
		private const int k_pxToolboxIndent = 2;
		/// <summary>
		/// Size of each tool (including border).
		/// </summary>
		private const int k_pxToolboxToolSize = 26;
		/// <summary>
		/// Indent of tool image with the tool "button".
		/// </summary>
		private const int k_pxToolImageOffset = 2;

		private const int k_pxShiftArrowBoxOriginX = k_pxToolboxIndent;
		private const int k_pxShiftArrowBoxOriginY = k_pxToolboxIndent + k_nToolboxRows * k_pxToolboxToolSize;
		private const int k_pxShiftArrowBoxWidth = 51;
		private const int k_pxShiftArrowBoxHeight = 45;
		private const int k_pxShiftArrowInsetX = 5;
		private const int k_pxShiftArrowInsetY = 2;
		private const int k_pxShiftArrowWidth = 18;
		private const int k_pxShiftArrowHeight = 14;

		public enum ShiftArrow
		{
			None,
			Left,
			Up,
			Right,
			Down
		};
		private ShiftArrow m_eHilightedShiftArrow = ShiftArrow.None;

		/// <summary>
		/// Valid tool types.
		/// The order needs to be kept in sync with the Tools array.
		/// </summary>
		public enum ToolType
		{
			Select = 0,
			Pencil = 1,
			Eyedropper = 2,
			FloodFill = 3,
			Eraser = 4,
			Line = 5,
			Rect = 6,
			RectFilled = 7,
			Ellipse = 8,
			EllipseFilled = 9,
			RotateCCW = 10,
			RotateCW = 11,
			Blank,
		};

		class Tool
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
		private Tool[] Tools = new Tool[]
		{
			new Tool("Select",			ToolType.Select,		0,0,	true,	false,	ResourceMgr.GetBitmap("tool_select")),
			new Tool("Pencil",			ToolType.Pencil,		1,0,	true,	true,	ResourceMgr.GetBitmap("tool_pencil")),
			new Tool("Eyedropper",		ToolType.Eyedropper,	0,1,	true,	true,	ResourceMgr.GetBitmap("tool_eyedropper")),
			new Tool("FloodFill",		ToolType.FloodFill,		1,1,	true,	true,	ResourceMgr.GetBitmap("tool_floodfill")),
			new Tool("Eraser",			ToolType.Eraser,		0,2,	true,	true,	ResourceMgr.GetBitmap("tool_eraser")),
			new Tool("Line",			ToolType.Line,			1,2,	false,	false,	ResourceMgr.GetBitmap("tool_line")),
			new Tool("Rect",			ToolType.Rect,			0,3,	false,	false,	ResourceMgr.GetBitmap("tool_rect")),
			new Tool("RectFilled",		ToolType.RectFilled,	1,3,	false,	false,	ResourceMgr.GetBitmap("tool_rectfilled")),
			new Tool("Ellipse",			ToolType.Ellipse,		0,4,	false,	false,	ResourceMgr.GetBitmap("tool_ellipse")),
			new Tool("EllipseFilled",	ToolType.EllipseFilled,	1,4,	false,	false,	ResourceMgr.GetBitmap("tool_ellipsefilled")),
		};

		private ToolType m_eSelectedTool = ToolType.Pencil;

		class ShiftArrowInfo
		{
			public ShiftArrow Type;
			public bool Hilite;
			public bool MouseDown;
			public Bitmap ArrowBitmap;

			public ShiftArrowInfo(ShiftArrow type, bool hilite, bool mousedown, Bitmap bm)
			{
				Type = type;
				Hilite = hilite;
				MouseDown = mousedown;
				ArrowBitmap = bm;
			}
		};

		private ShiftArrowInfo[] ShiftArrowInfoList = new ShiftArrowInfo[]
		{
			// No highlighting.
			new ShiftArrowInfo(ShiftArrow.None, false, false, ResourceMgr.GetBitmap("tool_shift")),
			// Normal highlighting.
			new ShiftArrowInfo(ShiftArrow.Left, true, false, ResourceMgr.GetBitmap("tool_shift_left")),
			new ShiftArrowInfo(ShiftArrow.Right, true, false, ResourceMgr.GetBitmap("tool_shift_right")),
			new ShiftArrowInfo(ShiftArrow.Up, true, false, ResourceMgr.GetBitmap("tool_shift_up")),
			new ShiftArrowInfo(ShiftArrow.Down, true, false, ResourceMgr.GetBitmap("tool_shift_down")),
			// MouseDown highlighting.
			new ShiftArrowInfo(ShiftArrow.Left, true, true, ResourceMgr.GetBitmap("tool_shift_left_hilite")),
			new ShiftArrowInfo(ShiftArrow.Right, true, true, ResourceMgr.GetBitmap("tool_shift_right_hilite")),
			new ShiftArrowInfo(ShiftArrow.Up, true, true, ResourceMgr.GetBitmap("tool_shift_up_hilite")),
			new ShiftArrowInfo(ShiftArrow.Down, true, true, ResourceMgr.GetBitmap("tool_shift_down_hilite")),
		};

		static bool s_fAlreadyNotifiedUserAboutToolboxDef = false;

		public Toolbox(MainForm owner)
		{
			m_Owner = owner;

			// Make sure that the Tools array and the ToolType enum are consistent.
			int nIndex = 0;
			foreach (Tool t in Tools)
			{
				int iColumn = t.X;
				int iRow = t.Y;
				int nToolIndex = iRow * k_nToolboxColumns + iColumn;
				if (nToolIndex != nIndex)
				{
					if (!s_fAlreadyNotifiedUserAboutToolboxDef)
						m_Owner.Error(ResourceMgr.GetString("InconsistentToolboxDef"));
					s_fAlreadyNotifiedUserAboutToolboxDef = true;
					return;
				}
				nIndex++;
				ToolType eTool = (ToolType)nToolIndex;
				if (t.Type != eTool)
				{
					if (!s_fAlreadyNotifiedUserAboutToolboxDef)
						m_Owner.Error(ResourceMgr.GetString("InconsistentToolboxDef"));
					s_fAlreadyNotifiedUserAboutToolboxDef = true;
					return;
				}
			}
		}

		public ToolType CurrentTool
		{
			get { return m_eSelectedTool; }
			set { m_eSelectedTool = value; }
		}

		/// <summary>
		/// Handle the mouse when the button is held down.
		/// </summary>
		/// <param name="pxX"></param>
		/// <param name="pxY"></param>
		/// <returns>True if a new command was selected</returns>
		public bool HandleMouse(int pxX, int pxY)
		{
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
		/// Return true if one of the shift arrows is currently highlighted.
		/// </summary>
		/// <returns></returns>
		public ShiftArrow HilightedShiftArrow()
		{
			return m_eHilightedShiftArrow;
		}

		static bool m_fMouseDownShiftArrow = false;
		public void SetMouseDownShiftArrow(bool fMouseDown)
		{
			m_fMouseDownShiftArrow = fMouseDown;
		}

		/// <summary>
		/// Handle the mouse move events when the mouse button is not pressed.
		/// This needs to hilight/unhilight the shift arrows as appropriate.
		/// </summary>
		/// <param name="pxX"></param>
		/// <param name="pxY"></param>
		/// <returns>True if we need to redraw the screen</returns>
		public bool HandleMouseMove(int pxX, int pxY)
		{
			ShiftArrow eNewSelection = ShiftArrow.None;
			
			// Make sure the mouse is in the shift-arrow area
			if (pxX >= k_pxToolboxIndent
				&& pxY >= k_pxToolboxIndent + k_nToolboxRows * k_pxToolboxToolSize
				&& pxX < k_pxToolboxWidth
				&& pxY < k_pxToolboxHeight)
			{
				pxX -= k_pxShiftArrowBoxOriginX;
				pxY -= k_pxShiftArrowBoxOriginY;

				// If we just use the bounding box rects, we'll get strange results
				// because the rects overlap and some rects will take precedence
				// over the others.
				//                                                      \ t /
				// To avoid this, we use the diagonals to divide         \ /
				// up the area into top,left,bottom,right triangles    l  X  r
				// before applying the rectangle check.                  / \
				//                                                      / b \
				int pxX0 = pxX - k_pxShiftArrowInsetX;
				int pxY0 = pxY - k_pxShiftArrowInsetY;
				int pxX2 = k_pxShiftArrowBoxWidth - k_pxShiftArrowInsetX - pxX;
				// Bottom: pxX0 < pxY0 && pxX2 < pxY0
				// Left: pxX0 < pxY0 && pxX2 > pxY0
				// Right: pxX0 > pxY0 && pxX2 < pxY0
				// Top: pxX0 > pxY0 && pxX2 > pxY0

				if ((pxX0 < pxY0 && pxX2 > pxY0)	// Left triangle
					|| (pxX0 > pxY0 && pxX2 < pxY0)	// Right triangle
					)
				{
					// Note that k_pxShiftArrowWidth/Height are reversed because the
					// left/right arrows are rotated.
					int pxLeft = k_pxShiftArrowInsetX;
					int pxRight = pxLeft + k_pxShiftArrowHeight;
					int pxTop = (k_pxShiftArrowBoxHeight - k_pxShiftArrowWidth) / 2;
					int pxBottom = pxTop + k_pxShiftArrowWidth;
					if (pxX >= pxLeft && pxX < pxRight && pxY >= pxTop && pxY < pxBottom)
						eNewSelection = ShiftArrow.Left;

					pxRight = k_pxShiftArrowBoxWidth - k_pxShiftArrowInsetX;
					pxLeft = pxRight - k_pxShiftArrowHeight;
					if (pxX >= pxLeft && pxX < pxRight && pxY >= pxTop && pxY < pxBottom)
						eNewSelection = ShiftArrow.Right;
				} else {
					int pxLeft = (k_pxShiftArrowBoxWidth - k_pxShiftArrowWidth) / 2;
					int pxRight = pxLeft + k_pxShiftArrowWidth;
					int pxTop = k_pxShiftArrowInsetY;
					int pxBottom = pxTop + k_pxShiftArrowHeight;
					if (pxX >= pxLeft && pxX < pxRight && pxY >= pxTop && pxY < pxBottom)
						eNewSelection = ShiftArrow.Up;

					pxBottom = k_pxShiftArrowBoxHeight - k_pxShiftArrowInsetY;
					pxTop = pxBottom - k_pxShiftArrowHeight;
					if (pxX >= pxLeft && pxX < pxRight && pxY >= pxTop && pxY < pxBottom)
						eNewSelection = ShiftArrow.Down;
				}
			}
			
			// If the new selection is the same as the current selection, we're done.
			if (m_eHilightedShiftArrow == eNewSelection)
				return false;
			else
			{
				// Remove current selection.
				m_eHilightedShiftArrow = eNewSelection;
				return true;
			}
		}

		private Bitmap s_bmHilite = ResourceMgr.GetBitmap("tool_hilite");

		public void Draw(Graphics g)
		{
			g.DrawRectangle(Pens.LightGray, 0, 0, k_pxToolboxWidth, k_pxToolboxHeight);
			Font f = new Font("Arial Black", 10);
			int pxX0, pxY0;

			foreach (Tool t in Tools)
			{
				if (!t.Show)
					continue;

				int iColumn = t.X;
				int iRow = t.Y;
				int nToolIndex = iRow * k_nToolboxColumns + iColumn;
				ToolType eTool = (ToolType)nToolIndex;

				pxX0 = k_pxToolboxIndent + iColumn * k_pxToolboxToolSize;
				pxY0 = k_pxToolboxIndent + iRow * k_pxToolboxToolSize;

				if (eTool == m_eSelectedTool)
					g.DrawImage(s_bmHilite, pxX0, pxY0);

				g.DrawImage(t.ButtonBitmap, pxX0 + k_pxToolImageOffset, pxY0 + k_pxToolImageOffset);
			}	

			Bitmap bmShift = ShiftArrowInfoList[0].ArrowBitmap;
			if (m_eHilightedShiftArrow != ShiftArrow.None)
			{
				foreach (ShiftArrowInfo info in ShiftArrowInfoList)
				{
					if (info.Type == m_eHilightedShiftArrow
						&& info.MouseDown == m_fMouseDownShiftArrow)
					{
						bmShift = info.ArrowBitmap;
						break;
					}
				}
			}

			g.DrawImage(bmShift, k_pxShiftArrowBoxOriginX, k_pxShiftArrowBoxOriginY);
		}

	}
}
