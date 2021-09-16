using System;
using Terraria;

namespace AdvancedWorldGen.Helper
{
	public static class TileFinder
	{
		public static (int x, int y) SpiralSearch(int x, int y, Func<int, int, bool> check)
		{
			const int stepSize = 2;
			int xStep = stepSize;
			int yStep = 0;

			int activeLength = 1;
			float length = 1;

			while (true)
			{
				if (WorldGen.InWorld(x, y, 10) && check(x, y))
				{
					break;
				}
				
				x += xStep;
				y += yStep;
				if (--activeLength == 0)
				{
					int tmp = xStep;
					xStep = -yStep;
					yStep = tmp;
					length += 0.5f;
					activeLength = (int) length;
				}
			}

			return (x, y);
		}
	}
}