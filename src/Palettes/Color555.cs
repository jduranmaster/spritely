using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	public class Color555
	{
		public int Encoded;

		public Color555(int encoded)
		{
			Encoded = encoded;
		}

		public Color555(int r, int g, int b)
		{
			Encoded = Encode(r, g, b);
		}

		public static int Encode(int r, int g, int b)
		{
			return r | (g << 5) | (b << 10);
		}
	}
}
