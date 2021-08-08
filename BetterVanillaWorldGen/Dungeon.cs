using Terraria;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class Dungeon
	{
		public static bool IsDungeon(int x, int y)
		{
			if (y < Main.worldSurface)
				return false;

			if (x < 0 || x > Main.maxTilesX)
				return false;

			return Main.wallDungeon[Main.tile[x, y].wall];
		}
	}
}