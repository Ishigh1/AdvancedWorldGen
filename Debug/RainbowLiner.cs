#if SPECIALDEBUG
namespace AdvancedWorldGen.Debug
{
	public static class RainbowLiner
	{
		public static void CreateColumn(int x)
		{
			for (int y = 0; y < Main.maxTilesY; y++) WorldGen.PlaceTile(x, y, TileID.RainbowBrick, forced: true);
		}

		public static void CreateLine(int y)
		{
			for (int x = 0; x < Main.maxTilesX; x++) WorldGen.PlaceTile(x, y, TileID.RainbowBrick, forced: true);
		}
	}
}
#endif