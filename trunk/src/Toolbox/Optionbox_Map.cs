using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class Optionbox_Map : Optionbox
	{
		public Optionbox_Map() : base()
		{
			Tools.Add(new Tool("ShowScreen", ToolType.ShowScreen, 0, 0,
					ResourceMgr.GetBitmap("tool_screen"),
					Options.BoolOptionName.BackgroundMap_ShowScreen));

			Tools.Add(new Tool("ShowTileGrid", ToolType.ShowTileGrid, 1, 0,
					ResourceMgr.GetBitmap("tool_smallgrid"),
					Options.BoolOptionName.BackgroundMap_ShowGrid));

			ToolboxRows = 1;
		}

	}
}
