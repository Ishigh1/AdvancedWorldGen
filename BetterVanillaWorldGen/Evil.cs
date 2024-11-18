namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class Corruption : ControlledWorldGenPass
{
	private RandomPointInLine OtherBiomes = null!;
	private List<Point> HeartPos = [];
	private int HeartCount = 0;

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
		OtherBiomes.AddBlock(false, GenVars.jungleMinX, GenVars.jungleMaxX); //Jungle
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

			CrimStart(crimsonCenter, minY - 10);
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

		CrimPlaceHearts();
	}
	
	private void CrimStart(int i, int j)
	{
		int crimDir = 1;
		int k = j;
		if (k > Main.worldSurface)
			k = (int)Main.worldSurface;

		for (; !WorldGen.SolidTile(i, k); k++) {
		}

		int num = k;
		Vector2D position = new(i, k);
		Vector2D vector2D = new(WorldGen.genRand.Next(-20, 21) * 0.1, WorldGen.genRand.Next(20, 201) * 0.01);
		if (vector2D.X < 0.0)
			crimDir = -1;

		double num2 = WorldGen.genRand.Next(15, 26);
		bool flag = true;
		int num3 = 0;
		while (flag) {
			num2 += WorldGen.genRand.Next(-50, 51) * 0.01;
			if (num2 < 15.0)
				num2 = 15.0;

			if (num2 > 25.0)
				num2 = 25.0;

			for (int l = (int)(position.X - num2 / 2.0); l < position.X + num2 / 2.0; l++) {
				for (int m = (int)(position.Y - num2 / 2.0); m < position.Y + num2 / 2.0; m++)
				{
					Tile tile = Main.tile[l, m];
					if (m > num) {
						if (Math.Abs(l - position.X) + Math.Abs(m - position.Y) < num2 * 0.3) {
							tile.HasTile = false;
							tile.WallType = 83;
						}
						else if (Math.Abs(l - position.X) + Math.Abs(m - position.Y) < num2 * 0.8 && tile.WallType != 83) {
							tile.HasTile = true;
							tile.TileType = 203;
							if (Math.Abs(l - position.X) + Math.Abs(m - position.Y) < num2 * 0.6)
								tile.WallType = 83;
						}
					}
					else if (Math.Abs(l - position.X) + Math.Abs(m - position.Y) < num2 * 0.3 && tile.HasTile) {
						tile.HasTile = false;
						tile.WallType = 83;
					}
				}
			}

			if (position.X > i + 50)
				num3 = -100;

			if (position.X < i - 50)
				num3 = 100;

			if (num3 < 0)
				vector2D.X -= WorldGen.genRand.Next(20, 51) * 0.01;
			else if (num3 > 0)
				vector2D.X += WorldGen.genRand.Next(20, 51) * 0.01;
			else
				vector2D.X += WorldGen.genRand.Next(-50, 51) * 0.01;

			vector2D.Y += WorldGen.genRand.Next(-50, 51) * 0.01;
			if (vector2D.Y < 0.25)
				vector2D.Y = 0.25;

			if (vector2D.Y > 2.0)
				vector2D.Y = 2.0;

			if (vector2D.X < -2.0)
				vector2D.X = -2.0;

			if (vector2D.X > 2.0)
				vector2D.X = 2.0;

			position += vector2D;
			if (position.Y > Main.worldSurface + 100.0)
				flag = false;
		}

		num2 = WorldGen.genRand.Next(40, 55);
		for (int n = 0; n < 50; n++) {
			int num4 = (int)position.X + WorldGen.genRand.Next(-20, 21);
			int num5 = (int)position.Y + WorldGen.genRand.Next(-20, 21);
			for (int num6 = (int)(num4 - num2 / 2.0); num6 < num4 + num2 / 2.0; num6++) {
				for (int num7 = (int)(num5 - num2 / 2.0); num7 < num5 + num2 / 2.0; num7++) {
					double num8 = Math.Abs(num6 - num4);
					double num9 = Math.Abs(num7 - num5);
					double num10 = 1.0 + WorldGen.genRand.Next(-20, 21) * 0.01;
					double num11 = 1.0 + WorldGen.genRand.Next(-20, 21) * 0.01;
					double num12 = num8 * num10;
					num9 *= num11;
					double num13 = Math.Sqrt(num12 * num12 + num9 * num9);
					Tile tile = Main.tile[num6, num7];
					if (num13 < num2 * 0.25) {
						tile.HasTile = false;
						tile.WallType = 83;
					}
					else if (num13 < num2 * 0.4 && tile.WallType != 83) {
						tile.HasTile = true;
						tile.TileType = 203;
						if (num13 < num2 * 0.35)
							tile.WallType = 83;
					}
				}
			}
		}

		int num14 = WorldGen.genRand.Next(5, 9);
		Vector2D[] array = new Vector2D[num14];
		for (int num15 = 0; num15 < num14; num15++) {
			int num16 = (int)position.X;
			int num17 = (int)position.Y;
			int num18 = 0;
			bool flag2 = true;
			Vector2D vector2D2 = new Vector2D(WorldGen.genRand.Next(-20, 21) * 0.15, WorldGen.genRand.Next(0, 21) * 0.15);
			while (flag2) {
				vector2D2 = new Vector2D(WorldGen.genRand.Next(-20, 21) * 0.15, WorldGen.genRand.Next(0, 21) * 0.15);
				while (Math.Abs(vector2D2.X) + Math.Abs(vector2D2.Y) < 1.5) {
					vector2D2 = new Vector2D(WorldGen.genRand.Next(-20, 21) * 0.15, WorldGen.genRand.Next(0, 21) * 0.15);
				}

				flag2 = false;
				for (int num19 = 0; num19 < num15; num19++) {
					if (vector2D.X > array[num19].X - 0.75 && vector2D.X < array[num19].X + 0.75 && vector2D.Y > array[num19].Y - 0.75 && vector2D.Y < array[num19].Y + 0.75) {
						flag2 = true;
						num18++;
						break;
					}
				}

				if (num18 > 10000)
					break;
			}

			array[num15] = vector2D2;
			CrimVein(new Vector2D(num16, num17), vector2D2);
		}

		int num20 = Main.maxTilesX;
		int num21 = 0;
		position.X = i;
		position.Y = num;
		num2 = WorldGen.genRand.Next(25, 35);
		double num22 = WorldGen.genRand.Next(0, 6);
		for (int num23 = 0; num23 < 50; num23++) {
			if (num22 > 0.0) {
				double num24 = WorldGen.genRand.Next(10, 30) * 0.01;
				num22 -= num24;
				position.Y -= num24;
			}

			int num25 = (int)position.X + WorldGen.genRand.Next(-2, 3);
			int num26 = (int)position.Y + WorldGen.genRand.Next(-2, 3);
			for (int num27 = (int)(num25 - num2 / 2.0); num27 < num25 + num2 / 2.0; num27++) {
				for (int num28 = (int)(num26 - num2 / 2.0); num28 < num26 + num2 / 2.0; num28++) {
					double num29 = Math.Abs(num27 - num25);
					double num30 = Math.Abs(num28 - num26);
					double num31 = 1.0 + WorldGen.genRand.Next(-20, 21) * 0.005;
					double num32 = 1.0 + WorldGen.genRand.Next(-20, 21) * 0.005;
					double num33 = num29 * num31;
					num30 *= num32;
					double num34 = Math.Sqrt(num33 * num33 + num30 * num30);
					Tile tile = Main.tile[num27, num28];
					if (num34 < num2 * 0.2 * (WorldGen.genRand.Next(90, 111) * 0.01)) {
						tile.HasTile = false;
						tile.WallType = 83;
					}
					else {
						if (!(num34 < num2 * 0.45))
							continue;

						if (num27 < num20)
							num20 = num27;

						if (num27 > num21)
							num21 = num27;

						if (tile.WallType != 83) {
							tile.HasTile = true;
							tile.TileType = 203;
							if (num34 < num2 * 0.35)
								tile.WallType = 83;
						}
					}
				}
			}
		}

		for (int num35 = num20; num35 <= num21; num35++) {
			int num36;
			for (num36 = num; (Main.tile[num35, num36].TileType == 203 && Main.tile[num35, num36].HasTile) || Main.tile[num35, num36].WallType == 83; num36++) {
			}

			int num37 = WorldGen.genRand.Next(15, 20);
			for (; !Main.tile[num35, num36].HasTile; num36++) {
				if (num37 <= 0)
					break;

				Tile tile = Main.tile[num35, num36];
				if (tile.WallType == 83)
					break;

				num37--;
				tile.TileType = 203;
				tile.HasTile = true;
			}
		}

		WorldGen.CrimEnt(position, crimDir);
	}
	
	private void CrimPlaceHearts()
	{
		int num;
		foreach (Point heart in HeartPos)
		{
			int x = heart.X;
			int y = heart.Y;
			num = WorldGen.genRand.Next(16, 21);
			for (int j = x - num / 2; j < x + num / 2; j++) {
				for (int k = y - num / 2; k < y + num / 2; k++) {
					double num2 = Math.Abs(j - x);
					double num3 = Math.Abs(k - y);
					if (Math.Sqrt(num2 * num2 + num3 * num3) < num * 0.4) {
						Tile tile = Main.tile[j, k];
						tile.HasTile = true;
						tile.TileType = 203;
						tile.WallType = 83;
					}
				}
			}
		}

		foreach (Point heart in HeartPos)
		{
			int x = heart.X;
			int y = heart.Y;
			num = WorldGen.genRand.Next(10, 14);
			for (int m = x - num / 2; m < x + num / 2; m++) {
				for (int n = y - num / 2; n < y + num / 2; n++) {
					double num4 = Math.Abs(m - x);
					double num5 = Math.Abs(n - y);
					if (Math.Sqrt(num4 * num4 + num5 * num5) < num * 0.3) {
						Tile tile = Main.tile[m, n];
						tile.HasTile = false;
						tile.WallType = 83;
					}
				}
			}
		}

		foreach (Point heart in HeartPos)
		{
			WorldGen.AddShadowOrb(heart.X, heart.Y);
		}
	}
	
	private void CrimVein(Vector2D position, Vector2D velocity)
	{
		double num = WorldGen.genRand.Next(15, 26);
		bool flag = true;
		Vector2D vector2D = velocity;
		Vector2D vector2D2 = position;
		int num2 = WorldGen.genRand.Next(100, 150);
		if (velocity.Y < 0.0)
			num2 -= 25;

		while (flag) {
			num += WorldGen.genRand.Next(-50, 51) * 0.02;
			if (num < 15.0)
				num = 15.0;

			if (num > 25.0)
				num = 25.0;

			for (int i = (int)(position.X - num / 2.0); i < position.X + num / 2.0; i++) {
				for (int j = (int)(position.Y - num / 2.0); j < position.Y + num / 2.0; j++) {
					double num3 = Math.Abs(i - position.X);
					double num4 = Math.Abs(j - position.Y);
					double num5 = Math.Sqrt(num3 * num3 + num4 * num4);
					Tile tile = Main.tile[i, j];
					if (num5 < num * 0.2) {
						tile.HasTile = false;
						tile.WallType = 83;
					}
					else if (num5 < num * 0.5 && tile.WallType != 83) {
						tile.HasTile = true;
						tile.TileType = 203;
						if (num5 < num * 0.4)
							tile.WallType = 83;
					}
				}
			}

			velocity.X += WorldGen.genRand.Next(-50, 51) * 0.05;
			velocity.Y += WorldGen.genRand.Next(-50, 51) * 0.05;
			if (velocity.Y < vector2D.Y - 0.75)
				velocity.Y = vector2D.Y - 0.75;

			if (velocity.Y > vector2D.Y + 0.75)
				velocity.Y = vector2D.Y + 0.75;

			if (velocity.X < vector2D.X - 0.75)
				velocity.X = vector2D.X - 0.75;

			if (velocity.X > vector2D.X + 0.75)
				velocity.X = vector2D.X + 0.75;

			position += velocity;
			if (Math.Abs(position.X - vector2D2.X) + Math.Abs(position.Y - vector2D2.Y) > num2)
				flag = false;
		}

		HeartPos.Add(position.ToPoint());
		HeartCount++;
	}
}