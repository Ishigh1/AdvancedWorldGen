using System;
using AdvancedWorldGen.Helper;
using Terraria;
using Terraria.Localization;

namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class FloatingIslands : ControlledWorldGenPass
{
	public FloatingIslands() : base("Floating Islands", 2582.8511f)
	{
	}

	protected override void ApplyPass()
	{
		Progress.Message = Language.GetTextValue("LegacyWorldGen.12");
		int numIslandHouses = 0;
		int skyIslands = (int)(Main.maxTilesX * 0.0008);
		int islandsMade = 0;

		int skyLakes = Main.maxTilesX switch
		{
			> 8000 => 3,
			> 6000 => 2,
			_ => 1
		};
		int totalSkyBiomes = skyIslands + skyLakes;
		for (int currentSkyItem = 0; currentSkyItem < totalSkyBiomes; currentSkyItem++)
		{
			Progress.Set(currentSkyItem, totalSkyBiomes);
			int num820 = Main.maxTilesX;
			while (--num820 > 0)
			{
				bool flag54 = true;
				int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1),
					(int)(Main.maxTilesX * 0.9));
				while (x > Main.maxTilesX / 2 - 150 && x < Main.maxTilesX / 2 + 150)
					x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1),
						(int)(Main.maxTilesX * 0.9));

				for (int num822 = 0; num822 < numIslandHouses; num822++)
					if (x > WorldGen.floatingIslandHouseX[num822] - 180 &&
					    x < WorldGen.floatingIslandHouseX[num822] + 180)
					{
						flag54 = false;
						break;
					}

				if (flag54)
				{
					flag54 = false;
					int num823 = 0;
					for (int num824 = 200; num824 < Main.worldSurface; num824++)
						if (Main.tile[x, num824].HasTile)
						{
							num823 = num824;
							flag54 = true;
							break;
						}

					if (flag54)
					{
						int num825 = 0;
						num820 = -1;
						int y = WorldGen.genRand.Next(Math.Max(50, Math.Min(90, (int)WorldGen.worldSurfaceLow - 50)), num823 - 100);
						if (islandsMade >= skyIslands)
						{
							WorldGen.skyLake[numIslandHouses] = true;
							WorldGen.CloudLake(x, y);
						}
						else
						{
							WorldGen.skyLake[numIslandHouses] = false;
							if (WorldGen.drunkWorldGen)
							{
								if (WorldGen.genRand.Next(2) == 0)
								{
									num825 = 3;
									WorldGen.SnowCloudIsland(x, y);
								}
								else
								{
									num825 = 1;
									WorldGen.DesertCloudIsland(x, y);
								}
							}
							else
							{
								if (WorldGen.getGoodWorldGen)
									num825 = !WorldGen.crimson ? 4 : 5;

								if (Main.tenthAnniversaryWorld)
									num825 = 6;

								WorldGen.CloudIsland(x, y);
							}
						}

						WorldGen.floatingIslandHouseX[numIslandHouses] = x;
						WorldGen.floatingIslandHouseY[numIslandHouses] = y;
						WorldGen.floatingIslandStyle[numIslandHouses] = num825;
						numIslandHouses++;
						islandsMade++;
					}
				}
			}
		}

		numIslandHouses = numIslandHouses;
	}
}