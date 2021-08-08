using Terraria;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class Desert
	{
		public static bool IsUndergroundDesert(int x, int y) {
			if (y < Main.worldSurface)
				return false;

			if (x < Main.maxTilesX * 0.15 || x > Main.maxTilesX * 0.85)
				return false;

			const int spread = 15;
			for (int i = x - spread; i <= x + spread; i += 10)
			for (int j = y - spread; j <= y + spread; j += 10)
				if (Main.tile[i, j].wall == 187 || Main.tile[i, j].wall == 216)
					return true;

			return false;
		}
	}
}