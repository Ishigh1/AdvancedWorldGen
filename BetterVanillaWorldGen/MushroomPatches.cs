namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class MushroomPatches : ControlledWorldGenPass
{
	public MushroomPatches() : base("Mushroom Patches", 743.7686f)
	{
	}

	protected override void ApplyPass()
	{
		Progress.Message = Language.GetTextValue("LegacyWorldGen.13");
		int mushroomBiomes = Math.Max(1, Main.maxTilesX / 700);

		RTree mushroomBiomesRectangles = RTree.Root();

		const int spread = 100;
		int jungleMinX = Math.Max(VanillaInterface.JungleLeft - spread, spread);
		int jungleSpread = Math.Min(VanillaInterface.JungleRight + spread, Main.maxTilesX - spread) -
		                   jungleMinX;
		int xMax = Main.maxTilesX - jungleSpread - spread;

		for (int numBiome = 0; numBiome < mushroomBiomes; numBiome++)
		{
			Progress.Set(numBiome, mushroomBiomes, 0.5f);
			TryToShroomPatch(spread, xMax, jungleMinX, jungleSpread, mushroomBiomesRectangles);
		}

		for (int x = 50; x < Main.maxTilesX - 50; x++)
		{
			Progress.Set(x - 50, Main.maxTilesX - 100, 0.5f, 0.5f);
			for (int y = (int)Main.worldSurface; y < Main.maxTilesY - 50; y++)
			{
				if (!Main.tile[x, y].HasTile)
					continue;
				WorldGen.grassSpread = 0;
				WorldGen.SpreadGrass(x, y, TileID.Mud, TileID.MushroomGrass, false);
				if (Main.tile[x, y].TileType == TileID.MushroomGrass)
				{
					const int type = TileID.Mud;
					for (int x2 = x - 1; x2 <= x + 1; x2++)
					for (int y2 = y - 1; y2 <= y + 1; y2++)
						if (Main.tile[x2, y2].HasTile)
						{
							if (!Main.tile[x2 - 1, y2].HasTile &&
							    !Main.tile[x2 + 1, y2].HasTile)
								WorldGen.KillTile(x2, y2);
							else if (!Main.tile[x2, y2 - 1].HasTile &&
							         !Main.tile[x2, y2 + 1].HasTile)
								WorldGen.KillTile(x2, y2);
						}
						else if (Main.tile[x2 - 1, y2].HasTile &&
						         Main.tile[x2 + 1, y2].HasTile)
						{
							WorldGen.PlaceTile(x2, y2, type);
							if (Main.tile[x2 - 1, y].TileType == 70)
								Main.tile[x2 - 1, y].TileType = 59;

							if (Main.tile[x2 + 1, y].TileType == 70)
								Main.tile[x2 + 1, y].TileType = 59;
						}
						else if (Main.tile[x2, y2 - 1].HasTile &&
						         Main.tile[x2, y2 + 1].HasTile)
						{
							WorldGen.PlaceTile(x2, y2, type);
							if (Main.tile[x2, y - 1].TileType == 70)
								Main.tile[x2, y - 1].TileType = 59;

							if (Main.tile[x2, y + 1].TileType == 70)
								Main.tile[x2, y + 1].TileType = 59;
						}

					if (WorldGen.genRand.NextBool(4))
					{
						int num814 = x + WorldGen.genRand.Next(-20, 21);
						int num815 = y + WorldGen.genRand.Next(-20, 21);
						if (Main.tile[num814, num815].TileType == 59)
							Main.tile[num814, num815].TileType = 70;
					}
				}
			}
		}
	}

	private static void TryToShroomPatch(int spread, int xMax, int jungleMinX, int jungleSpread,
		RTree mushroomBiomesRectangles)
	{
		int tries = 0;
		while (true)
		{
			if (tries++ > 350)
				break;

			int x = WorldGen.genRand.Next(spread, xMax);
			if (x >= jungleMinX)
				x += jungleSpread;

			int y;
			y = Main.rockLayer + 200 < Main.UnderworldLayer
				? WorldGen.genRand.Next((int)Main.rockLayer + 50, Main.UnderworldLayer - 100)
				: WorldGen.genRand.Next((int)Main.rockLayer, Main.UnderworldLayer);

			bool isValid = !mushroomBiomesRectangles.Contains(x, y);
			if (!isValid)
				continue;

			for (int x2 = x - spread; x2 < x + spread && isValid; x2 += 10)
			for (int y2 = y - spread; y2 < y + spread && isValid; y2 += 10)
				if (Main.tile[x2, y2].TileType is TileID.SnowBlock or TileID.IceBlock or TileID.BreakableIce or
				    TileID.JungleGrass or TileID.Granite or TileID.Marble)
					isValid = false;
				else if (WorldGen.UndergroundDesertLocation.Contains(new Point(x2, y2))) isValid = false;

			if (!isValid)
				continue;

			ShroomPatch(x, y);
			for (int it = 0; it < 5; it++)
			{
				int x2 = x + WorldGen.genRand.Next(-40, 41);
				int y2 = y + WorldGen.genRand.Next(-40, 41);
				ShroomPatch(x2, y2);
			}

			const int distanceBetweenBiomes = 500;
			mushroomBiomesRectangles.Insert(new Rectangle(x - distanceBetweenBiomes, y - distanceBetweenBiomes,
				2 * distanceBetweenBiomes, 2 * distanceBetweenBiomes));
			break;
		}
	}

	private static void ShroomPatch(int baseX, int baseY)
	{
		int num = WorldGen.genRand.Next(80, 100);
		int num2 = WorldGen.genRand.Next(20, 26);
		float multiplier = Main.maxTilesX / 4200f;
		if (WorldGen.getGoodWorldGen)
			multiplier *= 2f;

		num = (int)(num * multiplier);
		num2 = (int)(num2 * multiplier);
		float num4 = num2 - 1f;

		float centerX = baseX;
		float centerY = baseY - num2 * 0.3f;
		Vector2 vector2 = new(WorldGen.genRand.Next(-100, 101) * 0.005f,
			WorldGen.genRand.Next(-200, -100) * 0.005f);

		while (num > 0 && num2 > 0)
		{
			num -= WorldGen.genRand.Next(3);
			num2 -= 1;
			int xMin = (int)Math.Max(0, centerX - num * 0.5);
			int xMax = (int)Math.Min(Main.maxTilesX, centerX + num * 0.5);
			int yMin = (int)Math.Max(0, centerY - num * 0.5);
			int yMax = (int)Math.Min(Main.maxTilesY, centerY + num * 0.5);

			double num5 = num * WorldGen.genRand.Next(80, 120) * 0.01;
			for (baseX = xMin; baseX < xMax; baseX++)
			for (baseY = yMin; baseY < yMax; baseY++)
			{
				float num10 = Math.Abs(baseX - centerX);
				float num11 = Math.Abs((baseY - centerY) * 2.3f);
				double num12 = Math.Sqrt(num10 * num10 + num11 * num11);
				Tile tile = Main.tile[baseX, baseY];
				if (num12 < num5 * 0.8 && tile.LiquidType == LiquidID.Lava)
					tile.LiquidAmount = 0;

				if (num12 < num5 * 0.2 && baseY < centerY)
				{
					tile.HasTile = false;
					if (tile.WallType is not WallID.None)
						tile.WallType = WallID.MushroomUnsafe;
				}
				else if (num12 < num5 * 0.4 * (0.95 + WorldGen.genRand.NextFloat() * 0.1))
				{
					tile.TileType = TileID.Mud;
					if (Math.Abs(num2 - num4) < 0.1f && baseY > centerY)
						tile.HasTile = true;

					if (tile.WallType is not WallID.None)
						tile.WallType = WallID.MushroomUnsafe;
				}
			}

			centerX += 2 * vector2.X;
			centerY += vector2.Y;
			vector2.X += WorldGen.genRand.Next(-100, 110) * 0.005f;
			vector2.Y -= WorldGen.genRand.Next(110) * 0.005f;

			if (vector2.X < 0f)
				vector2.X = -0.5f;
			else
				vector2.X = 0.5f;
			vector2.Y = Utils.Clamp(vector2.Y, -0.5f, 0.5f);

			for (int m = 0; m < 2; m++)
			{
				int x = (int)centerX + WorldGen.genRand.Next(-20, 20);
				int y = (int)centerY + WorldGen.genRand.Next(0, 20);
				(x, y) = TileFinder.SpiralSearch(x, y, (i1, i2) => Main.tile[i1, i2].HasTile);
				if ((x, y) is (-1, -1))
					throw new Exception("Solid tile not found for mushroom biome !");

				int strength = WorldGen.genRand.Next(10, 20);
				int steps = WorldGen.genRand.Next(10, 20);
				WorldGen.TileRunner(x, y, strength, steps, TileID.Mud, false, 0f, 2f, true);
			}
		}
	}
}