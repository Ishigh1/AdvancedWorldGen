namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class Corruption : ControlledWorldGenPass
{
	private RandomPointInLine OtherBiomes = null!;

	public Corruption() : base("Corruption", 1094.237f)
	{
	}

	protected override void ApplyPass()
	{
		int biomeNumber = (int)OverhauledWorldGenConfigurator.Configuration.Next("Evil")
			.Get<JsonRange>("BiomeAmount").GetRandom(WorldGen.genRand);
		if (WorldGen.remixWorldGen)
			biomeNumber *= 2;
		bool oldCrimson = WorldGen.crimson;

		if (OptionHelper.OptionsContains("Drunk.Crimruption"))
		{
			bool isOdd = biomeNumber % 2 == 0;
			biomeNumber /= 2;
			int crimsonNumber = biomeNumber;
			int corruptionNumber = biomeNumber;
			if (isOdd)
			{
				if (WorldGen.genRand.NextBool(2))
					crimsonNumber += 1;
				else
					corruptionNumber += 1;
			}

			bool left = GenVars.crimsonLeft;
			InitializeBiomes(left);
			GenerateCrimson(crimsonNumber);
			InitializeBiomes(!left);
			GenerateCorruption(corruptionNumber);
		}
		else if (oldCrimson)
		{
			InitializeBiomes(false);
			GenerateCrimson(biomeNumber);
		}
		else
		{
			InitializeBiomes(false);
			GenerateCorruption(biomeNumber);
		}

		WorldGen.crimson = oldCrimson;
	}

	private void InitializeBiomes(bool? left = null)
	{
		int middlePadding = OptionHelper.OptionsContains("Drunk.Crimruption") ? 100 : 200;
		JsonRange jsonRange = OverhauledWorldGenConfigurator.Configuration.Next("Evil")
			.Get<JsonRange>("EvilBiomeSizeAroundCenter");
		int beachAvoidance = GenVars.evilBiomeBeachAvoidance;
		int dungeonAvoidance = 100;

		if (WorldGen.remixWorldGen)
		{
			middlePadding = 0;
		}
		else if (WorldGen.tenthAnniversaryWorldGen)
		{
			beachAvoidance *= 2;
			dungeonAvoidance *= 2;
			middlePadding = 0;
		}

		if (left == null)
		{
			OtherBiomes = new RandomPointInLine((int)jsonRange.ScaledMaximum, beachAvoidance,
				Main.maxTilesX - beachAvoidance - 1);
			OtherBiomes.AddBlock(true, Main.maxTilesX / 2 - middlePadding, Main.maxTilesX / 2 + middlePadding); //Center
		}
		else
		{
			int half = Main.maxTilesX / 2;
			int min = left == true ? beachAvoidance : half + middlePadding;
			int max = left == true ? half - middlePadding : Main.maxTilesX - beachAvoidance - 1;

			OtherBiomes = new RandomPointInLine((int)jsonRange.ScaledMaximum, min, max);
		}

		if (!WorldGen.remixWorldGen)
			OtherBiomes.AddBlock(true, GenVars.UndergroundDesertLocation.Left,
				GenVars.UndergroundDesertLocation.Right); //Desert
		OtherBiomes.AddBlock(false, GenVars.snowOriginLeft, GenVars.snowOriginRight); //Snow
		OtherBiomes.AddBlock(false, VanillaInterface.JungleLeft, VanillaInterface.JungleRight); //Jungle
		OtherBiomes.AddBlock(false, GenVars.dungeonLocation - dungeonAvoidance,
			GenVars.dungeonLocation + dungeonAvoidance); // Dungeon
	}

	private void GenerateCorruption(double biomeNumber)
	{
		Progress.Message = Lang.gen[20].Value;

		WorldGen.crimson = false;
		for (int biome = 0; biome < biomeNumber; biome++)
		{
			Progress.Set(biome, (float)biomeNumber);
			(int corruptionLeft, int corruptionCenter, int corruptionRight) = FindSuitableCenter();

			int minY = (from floatingIslandInfo in VanillaInterface.FloatingIslandInfos
				let islandX = floatingIslandInfo.X
				where corruptionLeft - 100 < islandX && corruptionRight + 100 > islandX
				select floatingIslandInfo.Y + 50).Prepend((int)GenVars.worldSurfaceLow - 50).Max();

			minY = Math.Max(minY, 10);
			MakeSingleCorruptionBiome(corruptionLeft, corruptionRight, corruptionCenter, minY);
		}
	}

	public static void MakeSingleCorruptionBiome(int corruptionLeft, int corruptionRight, int corruptionCenter,
		int minY)
	{
		for (int y = minY; y < Main.worldSurface - 1; y++)
			if (Main.tile[corruptionCenter, y].HasTile || Main.tile[corruptionCenter, y].WallType > 0)
			{
				WorldGen.ChasmRunner(corruptionCenter, y, WorldGen.genRand.Next(150, 300), true);
				break;
			}

		int pitSpacing = 20;
		for (int x = corruptionCenter; x > corruptionLeft; x--)
		{
			CorruptColumn(corruptionLeft, corruptionRight, minY, x, ref pitSpacing);
			pitSpacing--;
		}

		pitSpacing = 19;
		for (int x = corruptionCenter + 1; x < corruptionRight; x++)
		{
			CorruptColumn(corruptionLeft, corruptionRight, minY, x, ref pitSpacing);
			pitSpacing--;
		}

		double deepEnough = GenVars.worldSurfaceHigh + 60.0;

		for (int x = corruptionLeft; x < corruptionRight; x++)
		{
			bool flag52 = false;
			for (int y = minY; y < deepEnough; y++)
				if (Main.tile[x, y].HasTile)
				{
					if (Main.tile[x, y].TileType == 53 && x >= corruptionLeft + WorldGen.genRand.Next(5) &&
					    x <= corruptionRight - WorldGen.genRand.Next(5))
						Main.tile[x, y].TileType = 112;

					if (y < Main.worldSurface - 1.0 && !flag52)
						switch (Main.tile[x, y].TileType)
						{
							case 0:
								WorldGen.grassSpread = 0;
								WorldGen.SpreadGrass(x, y, 0, 23);
								break;
							case 59:
								WorldGen.grassSpread = 0;
								WorldGen.SpreadGrass(x, y, 59, TileID.CorruptJungleGrass);
								break;
						}

					flag52 = true;
					if (Main.tile[x, y].TileType == 1 && x >= corruptionLeft + WorldGen.genRand.Next(5) &&
					    x <= corruptionRight - WorldGen.genRand.Next(5))
						Main.tile[x, y].TileType = 25;

					Main.tile[x, y].WallType = Main.tile[x, y].WallType switch
					{
						216 => 217,
						187 => 220,
						_ => Main.tile[x, y].WallType
					};

					Main.tile[x, y].TileType = Main.tile[x, y].TileType switch
					{
						2 => 23,
						59 => TileID.CorruptJungleGrass,
						161 => 163,
						396 => 400,
						397 => 398,
						_ => Main.tile[x, y].TileType
					};
				}
		}

		#region protecc the orbs

		for (int x1 = corruptionLeft; x1 < corruptionRight; x1 += 2)
		for (int y1 = 0; y1 < Main.maxTilesY - 50; y1 += 2) // Main.maxTilesY - 50 is too deep
			if (Main.tile[x1, y1].HasTile && Main.tile[x1, y1].TileType == TileID.ShadowOrbs)
			{
				int xMin = Math.Max(x1 - 13, 10);
				int xMax = Math.Min(x1 + 13, Main.maxTilesX - 10);
				int yMin = Math.Max(y1 - 13, 10);
				int yMax = Math.Min(y1 + 13, Main.maxTilesY - 10);
				for (int x2 = xMin; x2 <= xMax; x2++)
				for (int y2 = yMin; y2 <= yMax; y2++)
				{
					Tile tile = Main.tile[x2, y2];
					int xDiff = Math.Abs(x2 - x1);
					int yDiff = Math.Abs(y2 - y1);
					if (tile.HasTile && tile.TileType == TileID.ShadowOrbs)
					{
					}
					else if (xDiff <= 2 + WorldGen.genRand.Next(3) &&
					         yDiff <= 2 + WorldGen.genRand.Next(3))
					{
						WorldGen.KillTile(x2, y2);
					}
					else if (xDiff + yDiff < 9 + WorldGen.genRand.Next(11) &&
					         WorldGen.genRand.NextBool(2, 3))
					{
						WorldGen.PlaceTile(x2, y2, TileID.Ebonstone, true);
					}
				}
			}

		#endregion
	}

	private static void CorruptColumn(int corruptionLeft, int corruptionRight, int minY, int x,
		ref int pitSpacing)
	{
		if (pitSpacing <= 0 && WorldGen.genRand.NextBool(35))
			for (int y = minY; y < Main.worldSurface - 1; y++)
				if (Main.tile[x, y].HasTile || Main.tile[x, y].WallType > 0)
				{
					pitSpacing = 30;
					WorldGen.ChasmRunner(x, y, WorldGen.genRand.Next(50, 100), true);
					break;
				}

		for (int y = (int)GenVars.worldSurfaceLow; y < Main.worldSurface - 1.0; y++)
			if (Main.tile[x, y].HasTile)
			{
				int num741 = y + WorldGen.genRand.Next(10, 14);
				for (int num742 = y; num742 < num741; num742++)
					if (Main.tile[x, num742].TileType is TileID.JungleGrass &&
					    x > corruptionLeft + WorldGen.genRand.Next(5) &&
					    x < corruptionRight - WorldGen.genRand.Next(5))
						Main.tile[x, num742].TileType = TileID.CorruptJungleGrass;

				break;
			}
	}

	private (int left, int center, int right) FindSuitableCenter()
	{
		JsonRange biomeSideSize = OverhauledWorldGenConfigurator.Configuration.Next("Evil")
			.Get<JsonRange>("EvilBiomeSizeAroundCenter");
		while (true)
		{
			double doubleCenter = biomeSideSize.GetRandom(WorldGen.genRand);
			int center = (int)Math.Round(doubleCenter);
			int biomeSize = (int)(doubleCenter + biomeSideSize.GetRandom(WorldGen.genRand));

			int x = OtherBiomes.GetRandomPoint();
			if (x == -1)
			{
				OtherBiomes.WeakMalus += 10;
				continue;
			}

			x += ((int) biomeSideSize.ScaledMaximum - biomeSize) / 2;
			return (x, x + center, x + biomeSize);
		}
	}

	private void GenerateCrimson(double biomeNumber)
	{
		Progress.Message = Lang.gen[72].Value;
		WorldGen.crimson = true;

		for (int biome = 0; biome < biomeNumber; biome++)
		{
			Progress.Set(biome, (float)biomeNumber);
			(int crimsonLeft, int crimsonCenter, int crimsonRight) = FindSuitableCenter();

			int minY = (from floatingIslandInfo in VanillaInterface.FloatingIslandInfos
				let islandX = floatingIslandInfo.X
				where crimsonLeft - 100 < islandX && crimsonRight + 100 > islandX
				select floatingIslandInfo.Y + 50).Prepend((int)GenVars.worldSurfaceLow - 50).Max();

			WorldGen.CrimStart(crimsonCenter, minY - 10);
			for (int x = crimsonLeft; x < crimsonRight; x++)
			for (int y = minY; y < Main.worldSurface - 1.0; y++)
				if (Main.tile[x, y].HasTile)
				{
					int num716 = y + WorldGen.genRand.Next(10, 14);
					for (int num717 = y; num717 < num716; num717++)
						if (Main.tile[x, num717].TileType is 60 && x >= crimsonLeft + WorldGen.genRand.Next(5) &&
						    x < crimsonRight - WorldGen.genRand.Next(5))
							Main.tile[x, num717].TileType = TileID.CrimsonJungleGrass;

					break;
				}

			double worldTop = GenVars.worldSurfaceHigh + 60.0;

			for (int x = crimsonLeft; x < crimsonRight; x++)
			{
				bool flag49 = false;
				for (int y = minY; y < worldTop; y++)
					if (Main.tile[x, y].HasTile)
					{
						if (Main.tile[x, y].TileType == 53 && x >= crimsonLeft + WorldGen.genRand.Next(5) &&
						    x <= crimsonRight - WorldGen.genRand.Next(5))
							Main.tile[x, y].TileType = 234;

						if (y < Main.worldSurface - 1.0 && !flag49)
							switch (Main.tile[x, y].TileType)
							{
								case 0:
									WorldGen.grassSpread = 0;
									WorldGen.SpreadGrass(x, y, 0, TileID.CrimsonGrass);
									break;
								case 59:
									WorldGen.grassSpread = 0;
									WorldGen.SpreadGrass(x, y, 59, TileID.CrimsonJungleGrass);
									break;
							}

						flag49 = true;
						Main.tile[x, y].WallType = Main.tile[x, y].WallType switch
						{
							216 => 218,
							187 => 221,
							_ => Main.tile[x, y].WallType
						};

						switch (Main.tile[x, y].TileType)
						{
							case 1:
							{
								if (x >= crimsonLeft + WorldGen.genRand.Next(5) &&
								    x <= crimsonRight - WorldGen.genRand.Next(5))
									Main.tile[x, y].TileType = 203;
								break;
							}
							case 2:
								Main.tile[x, y].TileType = 199;
								break;
							case 59:
								Main.tile[x, y].TileType = TileID.CrimsonJungleGrass;
								break;
							case 161:
								Main.tile[x, y].TileType = 200;
								break;
							case 396:
								Main.tile[x, y].TileType = 401;
								break;
							case 397:
								Main.tile[x, y].TileType = 399;
								break;
						}
					}
			}

			int num721 = WorldGen.genRand.Next(10, 15);
			for (int num722 = 0; num722 < num721; num722++)
			{
				int num723 = 0;
				bool flag50 = false;
				int num724 = 0;
				while (!flag50)
				{
					num723++;
					int num725 = WorldGen.genRand.Next(crimsonLeft - num724, crimsonRight + num724);
					int num726 = WorldGen.genRand.Next((int)(Main.worldSurface - num724 / 2f),
						(int)(Main.worldSurface + 100 + num724));
					while (WorldGen.oceanDepths(num725, num726))
					{
						num725 = WorldGen.genRand.Next(crimsonLeft - num724, crimsonRight + num724);
						num726 = WorldGen.genRand.Next((int)(Main.worldSurface - num724 / 2f),
							(int)(Main.worldSurface + 100 + num724));
					}

					if (num723 > 100)
					{
						num724++;
						num723 = 0;
					}

					if (!Main.tile[num725, num726].HasTile)
					{
						for (; !Main.tile[num725, num726].HasTile; num726++)
						{
						}

						num726--;
					}
					else
					{
						while (Main.tile[num725, num726].HasTile && num726 > Main.worldSurface) num726--;
					}

					if ((num724 > 10 || (Main.tile[num725, num726 + 1].HasTile &&
					                     Main.tile[num725, num726 + 1].TileType == 203)) &&
					    !WorldGen.IsTileNearby(num725, num726, 26, 3))
					{
						WorldGen.Place3x2(num725, num726, 26, 1);
						if (Main.tile[num725, num726].TileType == 26)
							flag50 = true;
					}

					if (num724 > 100)
						flag50 = true;
				}
			}
		}

		WorldGen.CrimPlaceHearts();
	}
}