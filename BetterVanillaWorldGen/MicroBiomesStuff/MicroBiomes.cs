namespace AdvancedWorldGen.BetterVanillaWorldGen.MicroBiomesStuff;

public class MicroBiomes : ControlledWorldGenPass
{
	private readonly int Index;
	private readonly string? Variation;

	public MicroBiomes(int index) : base("Micro Biomes", 0)
	{
		Index = index;
		if (index != -1)
		{
			Variation = GetVariationFromIndex();
			Name = "Micro Biomes " + Variation;
			Weight = 3547.4304f / 8;
		}
	}

	protected override void ApplyPass()
	{
		WorldGenConfiguration configuration = GenVars.configuration;
		Configuration = configuration.GetPassConfiguration("Micro Biomes");

		Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + ".." + Variation;
		switch (Index)
		{
			case 1:
				MakeDeadManChests(configuration);
				break;
			case 2:
				MakeThinIcePatches(configuration);
				break;
			case 3:
				MakeEnchantedSwordShrines(configuration);
				break;
			case 4:
				MakeCampsites(configuration);
				break;
			case 5:
				MakeExplosiveTraps(configuration);
				break;
			case 6:
				MakeMahoganyTrees(configuration);
				break;
			case 7:
				Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Long Minecart Tracks";

				JsonRange worldGenLongRange = Configuration.Get<JsonRange>("LongTrackLength");
				JsonRange worldGenShortRange = Configuration.Get<JsonRange>("StandardTrackLength");

				ModifiedTrackGenerator trackGenerator = new((int)worldGenShortRange.ScaledMinimum);
				int longTracks = (int)Configuration.Get<JsonRange>("LongTrackCount").GetRandom(WorldGen.genRand);
				MakeMinecartTracks(trackGenerator, worldGenLongRange, longTracks);

				Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Standard Minecart Tracks";
				int shortTracks = (int)Configuration.Get<JsonRange>("StandardTrackCount").GetRandom(WorldGen.genRand);
				MakeMinecartTracks(trackGenerator, worldGenShortRange, shortTracks);
				break;
			case 8:
				MakeLavaTraps();
				break;
		}
	}

	private string GetVariationFromIndex()
	{
		return Index switch
		{
			1 => "Dead Man's Chests",
			2 => "Thin Ice",
			3 => "Sword Shrines",
			4 => "Campsites",
			5 => "Explosive Traps",
			6 => "Living Trees",
			7 => "Minecart Tracks",
			8 => "Lava Traps",
			_ => throw new ArgumentException("Invalid index")
		};
	}

	private void MakeDeadManChests(WorldGenConfiguration configuration)
	{
		DeadMansChestBiome deadMansChestBiome = configuration.CreateBiome<DeadMansChestBiome>();
		List<int> possibleChestsToTrapify = deadMansChestBiome.GetPossibleChestsToTrapify(GenVars.structures);
		int random = (int)Configuration.Get<JsonRange>("DeadManChests").GetRandom(WorldGen.genRand);
		int num31 = 0;
		while (num31 < random && possibleChestsToTrapify.Count > 0)
		{
			int num32 = possibleChestsToTrapify[WorldGen.genRand.Next(possibleChestsToTrapify.Count)];
			Point origin = new(Main.chest[num32].x, Main.chest[num32].y);
			deadMansChestBiome.Place(origin, GenVars.structures);
			num31++;
			possibleChestsToTrapify.Remove(num32);
		}
	}

	private void MakeThinIcePatches(WorldGenConfiguration configuration)
	{
		if (!WorldGen.notTheBees || WorldGen.everythingWorldGen)
		{
			ThinIceBiome thinIceBiome = configuration.CreateBiome<ThinIceBiome>();
			int random2 = (int)Configuration.Get<JsonRange>("ThinIcePatchCount").GetRandom(WorldGen.genRand);
			int num33 = 0;
			const int iceMaxAttempts = 1000;
			int num35 = 0;
			while (num35 < random2)
				if (thinIceBiome.Place(
					    RandomUnderSurfaceWorldPoint((int)Main.worldSurface + 20, 200, 50, 50),
					    GenVars.structures))
				{
					num35++;
					num33 = 0;
				}
				else
				{
					num33++;
					if (num33 > iceMaxAttempts)
					{
						num35++;
						num33 = 0;
					}
				}
		}
	}

	private void MakeEnchantedSwordShrines(WorldGenConfiguration configuration)
	{
		EnchantedSwordBiome enchantedSwordBiome = configuration.CreateBiome<EnchantedSwordBiome>();
		int swordShrines = (int)Configuration.Get<JsonRange>("SwordShrineAttempts").GetRandom(WorldGen.genRand);
		float shrineChance = Configuration.Get<float>("SwordShrinePlacementChance");
		if (WorldGen.tenthAnniversaryWorldGen)
		{
			swordShrines *= 2;
			shrineChance /= 2;
		}

		Point origin2 = default;
		for (int num37 = 0; num37 < swordShrines; num37++)
			if ((num37 == 0 && WorldGen.tenthAnniversaryWorldGen) || !(WorldGen.genRand.NextFloat() > shrineChance))
			{
				int num38 = 0;
				while (num38++ <= Main.maxTilesX)
				{
					origin2.Y = (int)GenVars.worldSurface + WorldGen.genRand.Next(50, 100);
					origin2.X = WorldGen.genRand.NextBool(2)
						? WorldGen.genRand.Next(50, (int)(Main.maxTilesX * 0.3f))
						: WorldGen.genRand.Next((int)(Main.maxTilesX * 0.7f), Main.maxTilesX - 50);

					if (enchantedSwordBiome.Place(origin2, GenVars.structures))
						break;
				}
			}
	}

	private void MakeCampsites(WorldGenConfiguration configuration)
	{
		if (!WorldGen.notTheBees || WorldGen.everythingWorldGen)
		{
			CampsiteBiome campsiteBiome = configuration.CreateBiome<CampsiteBiome>();
			int random4 = (int)Configuration.Get<JsonRange>("CampsiteCount").GetRandom(WorldGen.genRand);
			int num39 = 0;
			while (num39 < random4)
				if (campsiteBiome.Place(
					    RandomUnderSurfaceWorldPoint((int)Main.worldSurface, 200,
						    WorldGen.beachDistance, WorldGen.beachDistance), GenVars.structures))
					num39++;
		}
	}

	private void MakeExplosiveTraps(WorldGenConfiguration configuration)
	{
		if (!WorldGen.notTheBees || WorldGen.everythingWorldGen)
		{
			MiningExplosivesBiome miningExplosivesBiome = configuration.CreateBiome<MiningExplosivesBiome>();
			int num40 = (int)Configuration.Get<JsonRange>("ExplosiveTrapCount").GetRandom(WorldGen.genRand);
			if ((WorldGen.getGoodWorldGen || WorldGen.noTrapsWorldGen) && !WorldGen.everythingWorldGen)
				num40 = (int)(num40 * 1.5);

			int num41 = 0;
			while (num41 < num40)
				if (WorldGen.remixWorldGen)
				{
					if (miningExplosivesBiome.Place(
						    WorldGen.RandomWorldPoint((int)Main.worldSurface, WorldGen.beachDistance,
							    Main.maxTilesY - (int)GenVars.rockLayer,
							    WorldGen.beachDistance), GenVars.structures))
						num41++;
				}
				else
				{
					if (miningExplosivesBiome.Place(
						    WorldGen.RandomWorldPoint((int)GenVars.rockLayer, WorldGen.beachDistance, 200,
							    WorldGen.beachDistance), GenVars.structures))
						num41++;
				}
		}
	}

	private void MakeMahoganyTrees(WorldGenConfiguration configuration)
	{
		MahoganyTreeBiome mahoganyTreeBiome = configuration.CreateBiome<MahoganyTreeBiome>();
		int treeNumber = (int)Configuration.Get<JsonRange>("LivingTreeCount").GetRandom(WorldGen.genRand);
		int placed = 0;

		int tries = 0;
		int top = (int)Main.worldSurface + 50;
		const int bottom = 500;
		int left = GenVars.jungleMinX;
		int right = Main.maxTilesX - GenVars.jungleMaxX;
		const int mahoganyMaxAttempts = 20000;

		while (placed < treeNumber && tries < mahoganyMaxAttempts)
			if (mahoganyTreeBiome.Place(RandomUnderSurfaceWorldPoint(top, bottom, left, right), GenVars.structures))
				placed++;
			else
				tries++;
	}

	private void MakeMinecartTracks(ModifiedTrackGenerator trackGenerator, JsonRange worldGenLongRange, int tracks)
	{
		int attempts = 0;
		int longTrackGenerated = 0;
		int maxAttempts = Main.maxTilesX / 10;

		while (longTrackGenerated < tracks && attempts < maxAttempts)
			if (trackGenerator.Place(
				    RandomUnderSurfaceWorldPoint((int)Main.worldSurface, 200, 10, 10),
				    (int)worldGenLongRange.ScaledMinimum, (int)worldGenLongRange.ScaledMaximum))
			{
				Progress.Add(1, tracks, 0.5f);
				longTrackGenerated++;
			}
			else
			{
				attempts++;
			}
	}

	private static void MakeLavaTraps()
	{
		if (WorldGen.notTheBees) return;

		const int maxAttempts = 20000;
		int attempts = 0;
		double lavaTraps = Main.maxTilesX * 0.02;
		if (WorldGen.noTrapsWorldGen)
			lavaTraps *= 5;
		else if (WorldGen.getGoodWorldGen)
			lavaTraps *= 2;

		int generatedLavaTraps = 0;
		while (generatedLavaTraps < lavaTraps && attempts < maxAttempts)
		{
			int x = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
			int y = WorldGen.genRand.Next(GenVars.lavaLine - 100, Main.maxTilesY - 210);
			if (WorldGen.placeLavaTrap(x, y))
				generatedLavaTraps++;
			else
				attempts++;
		}
	}

	private static Point RandomUnderSurfaceWorldPoint(int top = 0, int bottom = 0, int left = 0, int right = 0)
	{
		while (left + right > Main.maxTilesX)
		{
			left /= 2;
			right /= 2;
		}

		while (top + bottom > Main.maxTilesY)
		{
			top -= (int)(top - Main.worldSurface) / 2;
			bottom -= (Main.UnderworldLayer - Main.maxTilesY + bottom) / 2;
		}

		left = Utils.Clamp(left, 0, Main.maxTilesX - 1);
		right = Utils.Clamp(right, 0, Main.maxTilesX - 1);
		top = Utils.Clamp(top, 0, Main.maxTilesY - 1);
		bottom = Utils.Clamp(bottom, 0, Main.maxTilesY - 1);

		return new Point(WorldGen.genRand.Next(left, Main.maxTilesX - 1 - right),
			WorldGen.genRand.Next(top, Main.maxTilesY - 1 - bottom));
	}
}