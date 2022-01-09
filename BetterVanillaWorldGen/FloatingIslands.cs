using System;
using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
using AdvancedWorldGen.Helper;
using Terraria;
using Terraria.Localization;
using static Terraria.WorldGen;

namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class FloatingIslands : ControlledWorldGenPass
{
	public FloatingIslands() : base("Floating Islands", 1504.831f)
	{
	}

	protected override void ApplyPass()
	{
		int numIslandHouses = 0;
		Progress.Message = Language.GetTextValue("LegacyWorldGen.12");
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
				int x = genRand.Next((int)(Main.maxTilesX * 0.1),
					(int)(Main.maxTilesX * 0.9));
				while (x > Main.maxTilesX / 2 - 150 && x < Main.maxTilesX / 2 + 150)
					x = genRand.Next((int)(Main.maxTilesX * 0.1),
						(int)(Main.maxTilesX * 0.9));

				for (int num822 = 0; num822 < numIslandHouses; num822++)
					if (x > VanillaInterface.FloatingIslandHouseX.Value[num822] - 180 &&
					    x < VanillaInterface.FloatingIslandHouseX.Value[num822] + 180)
					{
						flag54 = false;
						break;
					}

				if (flag54)
				{
					flag54 = false;
					int num823 = 0;
					for (int num824 = 200; num824 < Main.worldSurface; num824++)
						if (Main.tile[x, num824].IsActive)
						{
							num823 = num824;
							flag54 = true;
							break;
						}

					if (flag54)
					{
						int num825 = 0;
						num820 = -1;
						int y = genRand.Next(Math.Max(50, Math.Min(90, (int)worldSurfaceLow - 50)), num823 - 100);
						if (islandsMade >= skyIslands)
						{
							VanillaInterface.SkyLake.Value[numIslandHouses] = true;
							CloudLake(x, y);
						}
						else
						{
							VanillaInterface.SkyLake.Value[numIslandHouses] = false;
							if (drunkWorldGen)
							{
								if (genRand.Next(2) == 0)
								{
									num825 = 3;
									SnowCloudIsland(x, y);
								}
								else
								{
									num825 = 1;
									DesertCloudIsland(x, y);
								}
							}
							else
							{
								if (getGoodWorldGen)
									num825 = !crimson ? 4 : 5;

								if (Main.tenthAnniversaryWorld)
									num825 = 6;

								CloudIsland(x, y);
							}
						}

						VanillaInterface.FloatingIslandHouseX.Value[numIslandHouses] = x;
						VanillaInterface.FloatingIslandHouseY.Value[numIslandHouses] = y;
						VanillaInterface.FloatingIslandStyle.Value[numIslandHouses] = num825;
						numIslandHouses++;
						islandsMade++;
					}
				}
			}
		}

		VanillaInterface.NumIslandHouses.Value = numIslandHouses;
	}
}