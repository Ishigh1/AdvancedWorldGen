namespace AdvancedWorldGen.BetterVanillaWorldGen.FloatingSky;

public class FloatingIslands : ControlledWorldGenPass
{
	public FloatingIslands() : base("Floating Islands", 1364.3461f)
	{
	}

	protected override void ApplyPass()
	{
		Progress.Message = Language.GetTextValue("LegacyWorldGen.12");
		GenVars.numIslandHouses = 0;
		int skyIslands = Main.maxTilesX / 1250;

		int skyLakes = Main.maxTilesX / 2500;
		int totalSkyBiomes = skyIslands + skyLakes;
		List<(int left, int right)> placedBiomes = new();
		int malus = 300;
		for (int currentSkyItem = 0; currentSkyItem < totalSkyBiomes; currentSkyItem++)
		{
			Progress.Set(currentSkyItem, totalSkyBiomes);
			int num820 = Main.maxTilesX;
			while (--num820 > 0)
			{
				bool flag54 = true;
				int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1),
					(int)(Main.maxTilesX * 0.9) - malus);
				if (x > Main.maxTilesX / 2 - 150)
					x += 300;
				int rightest = 0;
				foreach ((int left, int right) in placedBiomes)
				{
					if (x < left)
						break;
					x += right - Math.Max(left, rightest);
					rightest = right;
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
						num820 = -1;
						int y = WorldGen.genRand.Next(Math.Max(50, Math.Min(90, (int)GenVars.worldSurfaceLow - 50)),
							num823 - 100);

						FloatingIslandInfo floatingIslandInfo = new()
						{
							X = x,
							Y = y
						};
						bool lake = WorldGen.genRand.NextBool(skyLakes, skyLakes + skyIslands);
						if (lake)
						{
							skyLakes--;
							floatingIslandInfo.IsLake = true;
							WorldGen.CloudLake(x, y);
						}
						else
						{
							skyIslands--;
							if (WorldGen.drunkWorldGen)
							{
								if (WorldGen.genRand.NextBool(2))
								{
									floatingIslandInfo.Style = 3;
									WorldGen.SnowCloudIsland(x, y);
								}
								else
								{
									floatingIslandInfo.Style = 1;
									WorldGen.DesertCloudIsland(x, y);
								}
							}
							else
							{
								if (WorldGen.remixWorldGen && WorldGen.drunkWorldGen)
									floatingIslandInfo.Style = GenVars.crimsonLeft && x < Main.maxTilesX / 2 || !GenVars.crimsonLeft && x > Main.maxTilesX / 2 ? 5 : 4;
								else if (WorldGen.getGoodWorldGen || WorldGen.remixWorldGen)
									floatingIslandInfo.Style = !WorldGen.crimson ? 4 : 5;
								else if (Main.tenthAnniversaryWorld)
									floatingIslandInfo.Style = 6;

								WorldGen.CloudIsland(x, y);
							}
						}

						VanillaInterface.FloatingIslandInfos.Add(floatingIslandInfo);
						malus += 361;
						foreach ((int left, int right) in placedBiomes)
						{
							if (x - 180 < right && x + 180 > right)
								malus -= right - x + 180;
							if (x - 180 < left && x + 180 > left)
								malus -= x + 180 - left;
						}

						placedBiomes.Add((x - 180, x + 180));
						placedBiomes.Sort((biome1, biome2) => biome1.CompareTo(biome2));
						GenVars.numIslandHouses++;
					}
				}
			}
		}
	}
}