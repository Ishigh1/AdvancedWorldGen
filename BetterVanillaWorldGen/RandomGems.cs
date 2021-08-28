using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class RandomGems : GenPass
	{
		public RandomGems() : base("Random Gems", 18.4925f)
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Set(1f);
			int yMax = Main.maxTilesY - 300 > Main.rockLayer ? Main.maxTilesY - 300 : Main.UnderworldLayer;
			for (int num161 = 0; num161 < Main.maxTilesX; num161++)
			{
				int num162 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
				int num163 = WorldGen.genRand.Next((int) Main.rockLayer, yMax);
				if (!Main.tile[num162, num163].IsActive && Main.tile[num162, num163].LiquidType != LiquidID.Lava &&
				    !Main.wallDungeon[Main.tile[num162, num163].wall] && Main.tile[num162, num163].wall != 27)
				{
					int num164 = WorldGen.genRand.Next(12);
					int num165 = 0;
					num165 = num164 >= 3 ? num164 < 6 ? 1 : num164 < 8 ? 2 : num164 < 10 ? 3 : num164 >= 11 ? 5 : 4 : 0;
					WorldGen.PlaceTile(num162, num163, 178, true, false, -1, num165);
				}
			}

			for (int num166 = 0; num166 < Main.maxTilesX; num166++)
			{
				int num167 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
				int num168 = WorldGen.genRand.Next((int) Main.worldSurface, yMax);
				if (!Main.tile[num167, num168].IsActive && Main.tile[num167, num168].LiquidType != LiquidID.Lava &&
				    (Main.tile[num167, num168].wall == 216 || Main.tile[num167, num168].wall == 187))
				{
					int num169 = WorldGen.genRand.Next(1, 4);
					int num170 = WorldGen.genRand.Next(1, 4);
					int num171 = WorldGen.genRand.Next(1, 4);
					int num172 = WorldGen.genRand.Next(1, 4);
					for (int num173 = num167 - num169; num173 < num167 + num170; num173++)
					for (int num174 = num168 - num171; num174 < num168 + num172; num174++)
						if (!Main.tile[num173, num174].IsActive)
							WorldGen.PlaceTile(num173, num174, 178, true, false, -1, 6);
				}
			}
		}
	}
}