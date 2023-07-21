using AdvancedWorldGen.SpecialOptions._100kSpecial;

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
		int skyIslands = (int)OverhauledWorldGenConfigurator.Configuration.Next("SkyIslands")
			.Get<JsonRange>("HouseAmount").GetRandom(WorldGen.genRand);

		int skyLakes = (int)OverhauledWorldGenConfigurator.Configuration.Next("SkyIslands").Get<JsonRange>("LakeAmount")
			.GetRandom(WorldGen.genRand);
		int totalSkyBiomes = skyIslands + skyLakes;

		int beachSize = Math.Max(GenVars.oceanWaterStartRandomMax, (int)(Main.maxTilesX * 0.1f));
		RandomPointInLine randomPointInLine = new(301, beachSize, Main.maxTilesX - beachSize - 1);
		randomPointInLine.AddBlock(false, Main.maxTilesX / 2 - 150, Main.maxTilesX / 2 + 150);
		
		if (_100kWorld.Enabled)
		{
			randomPointInLine.AddBlock(false, GenVars.UndergroundDesertLocation.Left - 100, GenVars.UndergroundDesertLocation.Right + 100);
		}
		
		for (int currentSkyItem = 0; currentSkyItem < totalSkyBiomes; currentSkyItem++)
		{
			Progress.Set(currentSkyItem, totalSkyBiomes);
			int tries = Main.maxTilesX;
			while (--tries > 0)
			{
				int x = randomPointInLine.GetRandomPoint();
				if (x == -1)
				{
					AdvancedWorldGenMod.Instance.Logger.Warn(
						"Too many sky islands requested, resetting the placed island list to place more, islands may collide.");
					randomPointInLine.WeakMalus = 301;
					x = randomPointInLine.GetRandomPoint();
				}

				bool flag54 = false;
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
						if (WorldGen.drunkWorldGen && !WorldGen.everythingWorldGen)
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
							if (WorldGen.everythingWorldGen)
								floatingIslandInfo.Style = (GenVars.crimsonLeft && x < Main.maxTilesX / 2) ||
								                           (!GenVars.crimsonLeft && x > Main.maxTilesX / 2)
									? 5
									: 4;
							else if (WorldGen.getGoodWorldGen || WorldGen.remixWorldGen)
								floatingIslandInfo.Style = !WorldGen.crimson ? 4 : 5;
							else if (Main.tenthAnniversaryWorld)
								floatingIslandInfo.Style = 6;

							WorldGen.CloudIsland(x, y);
						}
					}

					VanillaInterface.FloatingIslandInfos.Add(floatingIslandInfo);
						
					randomPointInLine.AddBlock(true, x - 180, x + 180);
					GenVars.numIslandHouses++;
				}
			}
		}
	}
}