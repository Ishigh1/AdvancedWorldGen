namespace AdvancedWorldGen.BetterVanillaWorldGen.FloatingSky;

public class FloatingIslands : ControlledWorldGenPass
{
	public FloatingIslands() : base("Floating Islands", 1364.3461f)
	{
	}

	protected override void ApplyPass()
	{
		Progress.Message = Language.GetTextValue("LegacyWorldGen.12");
		WorldGen.numIslandHouses = 0;

		int skyIslands = (int)OverhauledWorldGenConfigurator.Configuration.Next("SkyIslands").Get<JsonRange>("HouseAmount").GetRandom(WorldGen.genRand);

		int skyLakes = (int)OverhauledWorldGenConfigurator.Configuration.Next("SkyIslands").Get<JsonRange>("LakeAmount").GetRandom(WorldGen.genRand);
		int totalSkyBiomes = skyIslands + skyLakes;
		List<(int left, int right)> placedBiomes = new() { (Main.maxTilesX / 2 - 150, Main.maxTilesX / 2 + 150) };
		int malus = 301;
		int xMin = Math.Max(WorldGen.oceanWaterStartRandomMax, (int)(Main.maxTilesX * 0.1f));
		int xMax = Main.maxTilesX - xMin;
		int maxMalus = xMax - xMin;
		for (int currentSkyItem = 0; currentSkyItem < totalSkyBiomes; currentSkyItem++)
		{
			if (malus > maxMalus)
			{
				AdvancedWorldGenMod.Instance.Logger.Warn("Too many sky islands requested, reseting the placed island list to place more, islands may collide.");
				placedBiomes.Add((Main.maxTilesX / 2 - 150, Main.maxTilesX / 2 + 150));
				malus = 301;
				placedBiomes.Clear();
			}
			Progress.Set(currentSkyItem, totalSkyBiomes);
			int tries = Main.maxTilesX;
			while (--tries > 0)
			{
				bool flag54 = true;
				int x = WorldGen.genRand.Next(xMin, xMax - malus);
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
						tries = -1;
						int y = WorldGen.genRand.Next(Math.Max(50, Math.Min(90, (int)WorldGen.worldSurfaceLow - 50)),
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
								if (WorldGen.getGoodWorldGen)
									floatingIslandInfo.Style = !WorldGen.crimson ? 4 : 5;

								if (Main.tenthAnniversaryWorld)
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
						WorldGen.numIslandHouses++;
					}
				}
			}
		}
	}
}