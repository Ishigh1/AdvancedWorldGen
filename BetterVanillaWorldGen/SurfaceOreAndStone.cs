using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedWorldGen.Helper;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class SurfaceOreAndStone : GenPass
	{
		public SurfaceOreAndStone() : base("Surface Ore and Stone", 0)
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			int num360 = WorldGen.genRand.Next(Main.maxTilesX / (4200 / 5), Main.maxTilesX / (4200 / 10));
			List<int> orePatchesX = new();

			for (int num361 = 0; num361 < num360; num361++)
			{
				GenPassHelper.SetProgress(progress, num361, num360, 0.5f);
				int num362 = Main.maxTilesX / 420;
				while (num362 > 0)
				{
					num362--;
					int num363 = WorldGen.genRand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);
					while (num363 >= Main.maxTilesX * 0.48 &&
					       num363 <= Main.maxTilesX * 0.52)
						num363 = WorldGen.genRand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);

					int y3 = WorldGen.genRand.Next((int) WorldGen.worldSurfaceLow, (int) WorldGen.worldSurface);
					bool flag19 = orePatchesX.Any(x => Math.Abs(num363 - x) < 200);

					if (!flag19 && WorldGen.OrePatch(num363, y3))
					{
						orePatchesX.Add(num363);

						break;
					}
				}
			}

			num360 = Main.maxTilesX > 2 * 4200 / 7 ? WorldGen.genRand.Next(1, Main.maxTilesX / (4200 / 7)) : 1;
			for (int num365 = 0; num365 < num360; num365++)
			{
				GenPassHelper.SetProgress(progress, num365, num360, 0.5f, 0.5f);
				int num366 = Main.maxTilesX / 420;
				while (num366 > 0)
				{
					num366--;
					int num367 = WorldGen.genRand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);
					while (num367 >= Main.maxTilesX * 0.47 &&
					       num367 <= Main.maxTilesX * 0.53)
						num367 = WorldGen.genRand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);

					int y4 = WorldGen.genRand.Next((int) WorldGen.worldSurfaceLow, (int) WorldGen.worldSurface);
					bool flag20 = orePatchesX.Any(x => Math.Abs(num367 - x) < 100);

					if (!flag20 && WorldGen.StonePatch(num367, y4))
						break;
				}
			}
		}
	}
}