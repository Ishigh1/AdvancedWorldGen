namespace AdvancedWorldGen.BetterVanillaWorldGen.Jungle;

public class ModifiedJunglePass : ControlledWorldGenPass
{
	private int DungeonSide;
	private int JungleOriginX;
	private int LeftBeachEnd;
	private int RightBeachStart;
	private float WorldScaleX;
	private float WorldScaleY;

	public ModifiedJunglePass() : base("Jungle", 10154.652f)
	{
	}

	protected override void ApplyPass()
	{
		Progress.Message = Language.GetTextValue("LegacyWorldGen.11");

		JungleOriginX = GenVars.jungleOriginX;
		DungeonSide = GenVars.dungeonSide;
		LeftBeachEnd = GenVars.leftBeachEnd;
		RightBeachStart = GenVars.rightBeachStart;

		WorldScaleY = Main.maxTilesY * (1.5f / 1200f);
		WorldScaleX = Main.maxTilesX * (1.5f / 4200f);
		int meanX = 0;
		int meanY = 0;

		Progress.Add(1, 11);

		int mudPasses = (int)Math.Max(3, 3 * WorldScaleY / WorldScaleX);
		for (int i = 0; i < mudPasses; i++)
		{
			int xRange = 100 + 300 * i / mudPasses;

			(int x, int y) = RandomPoint(xRange);
			meanX += x;
			meanY += y;
			PlaceFirstPassMud(x, y, 3);
			Progress.Add(1, mudPasses, 5 / 11f / 2f);

			ushort baseGem;
			if (i < mudPasses / 3)
				baseGem = 63;
			else if (i < 2 * mudPasses / 3)
				baseGem = 65;
			else
				baseGem = 67;
			PlaceGemsAt(x, y, baseGem, 2);
			Progress.Add(1, mudPasses, 5 / 11f / 2f);
		}

		meanX /= mudPasses;
		meanY /= mudPasses;

		int num = WorldGen.genRand.Next((int)(400f * WorldScaleX), (int)(600f * WorldScaleX));
		meanX = (int)Utils.Clamp(meanX, LeftBeachEnd + num / 2 + 25f * WorldScaleX,
			RightBeachStart - num / 2f - 25f * WorldScaleX);

		GenVars.mudWall = true;
		WorldGen.TileRunner(meanX, meanY, num, (int)(5000 * WorldScaleY), 59, false, 0f, -20f, true);
		Progress.Set(7, 11);

		GenerateTunnelToSurface(meanX, meanY);
		Progress.Set(8, 11);

		GenVars.mudWall = false;
		DelimitJungle((int)((GenVars.rockLayer + Main.UnderworldLayer) / 2));
		DelimitJungle((int)((GenVars.rockLayer + Main.UnderworldLayer) / 3), true);
		DelimitJungle((int)((GenVars.rockLayer + Main.UnderworldLayer) * 2 / 3), true);
		Progress.Set(9, 11);

		GenerateHolesInMudWalls();
		Progress.Set(10, 11);

		GenerateFinishingTouches(meanX, meanY);
		Progress.Set(11, 11);
	}

	private void PlaceGemsAt(int x, int y, ushort baseGem, int gemVariants)
	{
		for (int _ = 0; _ < 6f * Math.Sqrt(WorldScaleX * WorldScaleY); _++)
		{
			int i = x + WorldGen.genRand.Next(-(int)(125f * WorldScaleX), (int)(125f * WorldScaleX));
			int j = y + WorldGen.genRand.Next(-(int)(125f * WorldScaleY), (int)(125f * WorldScaleY));
			int strength = WorldGen.genRand.Next(3, 7);
			int steps = WorldGen.genRand.Next(3, 8);
			int type = baseGem + WorldGen.genRand.Next(0, gemVariants);
			WorldGen.TileRunner(i, j, strength, steps, type);
		}
	}

	private void PlaceFirstPassMud(int x, int y, int xSpeedScale)
	{
		GenVars.mudWall = true;
		float mul = WorldScaleY / WorldScaleX;
		WorldGen.TileRunner(x, y, WorldGen.genRand.Next((int)(250f * WorldScaleX), (int)(500f * WorldScaleX)),
			WorldGen.genRand.Next((int)(50f * WorldScaleY * mul), (int)(150f * WorldScaleY * mul)), 59, false,
			DungeonSide * xSpeedScale);
		GenVars.mudWall = false;
	}

	public (int x, int y) CreateStartPoint()
	{
		return (JungleOriginX, (int)(Main.maxTilesY + Main.rockLayer) / 2);
	}

	public void ApplyRandomMovement(ref int x, ref int y, int xRange, int yRange)
	{
		x += WorldGen.genRand.Next((int)(-xRange * WorldScaleX), 1 + (int)(xRange * WorldScaleX));
		y += WorldGen.genRand.Next((int)(-yRange * WorldScaleY), 1 + (int)(yRange * WorldScaleY));
		y = Utils.Clamp(y, (int)Main.rockLayer, Main.UnderworldLayer);
	}

	private (int x, int y) RandomPoint(int xDeviation)
	{
		return (JungleOriginX + WorldGen.genRand.Next(-xDeviation, xDeviation),
			WorldGen.genRand.Next((int)GenVars.rockLayer, Main.UnderworldLayer));
	}

	private static void GenerateTunnelToSurface(int x, int y)
	{
		double num = WorldGen.genRand.Next(5, 11);
		Vector2 vector = new(x, y);
		Vector2 vector2 = new(WorldGen.genRand.Next(-10, 11) * 0.1f,
			WorldGen.genRand.Next(10, 20) * 0.1f);
		int num2 = 0;
		while (true)
		{
			num += WorldGen.genRand.Next(-20, 21) * 0.1f;
			num = Utils.Clamp(num, 5, 10);

			int value3 = (int)(vector.X - num * 0.5);
			int value4 = (int)(vector.X + num * 0.5);
			int value5 = (int)(vector.Y - num * 0.5);
			int value6 = (int)(vector.Y + num * 0.5);
			value3 = Utils.Clamp(value3, 10, Main.maxTilesX - 10);
			value4 = Utils.Clamp(value4, 10, Main.maxTilesX - 10);
			value5 = Utils.Clamp(value5, 10, Main.maxTilesY - 10);
			value6 = Utils.Clamp(value6, 10, Main.maxTilesY - 10);
			for (int k = value3; k < value4; k++)
			for (int l = value5; l < value6; l++)
				if (Math.Abs(k - vector.X) + Math.Abs(l - vector.Y) <
				    num * 0.5 * (1.0 + WorldGen.genRand.Next(-10, 11) * 0.015))
					WorldGen.KillTile(k, l);

			num2++;
			if (num2 > 10 && WorldGen.genRand.Next(50) < num2)
			{
				num2 = 0;
				int num4 = WorldGen.genRand.NextBool(2) ? 2 : -2;

				WorldGen.TileRunner((int)vector.X, (int)vector.Y, WorldGen.genRand.Next(3, 20),
					WorldGen.genRand.Next(10, 100), -1,
					false, num4);
			}

			vector += vector2;
			vector2.Y += WorldGen.genRand.Next(-10, 11) * 0.01f;
			vector2.Y = Utils.Clamp(vector2.Y, -2, 0);

			vector2.X += WorldGen.genRand.Next(-10, 11) * 0.1f;
			if (vector.X < x - 200)
				vector2.X += WorldGen.genRand.Next(5, 21) * 0.1f;

			if (vector.X > x + 200)
				vector2.X -= WorldGen.genRand.Next(5, 21) * 0.1f;

			vector2.X = Utils.Clamp(vector2.X, -1.5f, 1.5f);

			int x1 = Utils.Clamp((int)vector.X, 10, Main.maxTilesX - 10);
			int y1 = Utils.Clamp((int)vector.Y, 10, Main.maxTilesY - 10);
			if (y1 < Main.worldSurface)
			{
				if (WorldGen.drunkWorldGen)
					return;

				if (Main.tile[x1, y1].WallType == 0 && !Main.tile[x1, y1].HasTile &&
				    Main.tile[x1, y1 - 3].WallType == 0 && !Main.tile[x1, y1 - 3].HasTile &&
				    Main.tile[x1, y1 - 1].WallType == 0 && !Main.tile[x1, y1 - 1].HasTile &&
				    Main.tile[x1, y1 - 4].WallType == 0 && !Main.tile[x1, y1 - 4].HasTile &&
				    Main.tile[x1, y1 - 2].WallType == 0 && !Main.tile[x1, y1 - 2].HasTile &&
				    Main.tile[x1, y1 - 5].WallType == 0 && !Main.tile[x1, y1 - 5].HasTile)
					return;
			}
		}
	}

	public void GenerateHolesInMudWalls()
	{
		int minX = VanillaInterface.JungleLeft;
		int maxX = VanillaInterface.JungleRight;
		for (int i = 0; i < Main.maxTilesX / 4; i++)
		{
			int x = WorldGen.genRand.Next(minX, maxX);
			int y = WorldGen.genRand.Next((int)Main.worldSurface + 10, Main.UnderworldLayer);
			if (Main.tile[x, y].WallType is WallID.JungleUnsafe or WallID.MudUnsafe)
				WorldGen.MudWallRunner(x, y);
		}
	}

	public void DelimitJungle(int y, bool secondTry = false)
	{
		int importantX = JungleOriginX;
		int currentX = JungleOriginX;
		int lastSeen = 0;
		Tile tile;
		while (lastSeen < 7 && currentX > 10)
		{
			tile = Main.tile[currentX, y];
			if (tile.HasTile && tile.TileType is TileID.Mud)
			{
				importantX = currentX;
				lastSeen = 0;
			}
			else
			{
				lastSeen += 1;
			}

			currentX -= 3;
		}

		VanillaInterface.JungleLeft = secondTry ? Math.Min(VanillaInterface.JungleLeft, importantX) : importantX;

		importantX = JungleOriginX;
		currentX = JungleOriginX;
		lastSeen = 0;
		while (lastSeen < 7 && currentX < Main.maxTilesX - 10)
		{
			tile = Main.tile[currentX, y];
			if (tile.HasTile && tile.TileType is TileID.Mud)
			{
				importantX = currentX;
				lastSeen = 0;
			}
			else
			{
				lastSeen += 1;
			}

			currentX += 3;
		}

		VanillaInterface.JungleRight = secondTry ? Math.Max(VanillaInterface.JungleRight, importantX) : importantX;
	}

	public void GenerateFinishingTouches(int middleX, int middleY)
	{
		int x = middleX;
		int y = middleY;

		float iterations = 20f * WorldScaleX;
		for (int i = 0; i <= iterations; i++)
		{
			Progress.Set(i, iterations, 1 / 11f / 3f, 10 / 11f);
			x += WorldGen.genRand.Next((int)(-5f * WorldScaleX), (int)(6f * WorldScaleX));
			y += WorldGen.genRand.Next((int)(-5f * WorldScaleY), (int)(6f * WorldScaleY));
			WorldGen.TileRunner(x, y, WorldGen.genRand.Next(40, 100), WorldGen.genRand.Next(300, 500), 59);
		}

		iterations = 10f * WorldScaleX;
		int xMin = Math.Max(1, (int)(middleX - 600f * WorldScaleX));
		int xMax = Math.Min(Main.maxTilesX - 1, (int)(middleX + 600f * WorldScaleX));
		int yMin = Math.Max(1, (int)(middleY - 200f * WorldScaleY));
		int yMax = Math.Min(Main.maxTilesY - 1, (int)(middleY + 200f * WorldScaleY));
		for (int i = 0; i <= iterations; i++)
		{
			Progress.Set(i, iterations, 1 / 11f / 3f, 10 / 11f + 1 / 11f / 3f);

			do
			{
				x = WorldGen.genRand.Next(xMin, xMax);
				y = WorldGen.genRand.Next(yMin, yMax);
			} while (Main.tile[x, y].TileType != TileID.Mud);

			for (int k = 0; k < 8f * WorldScaleX; k++)
			{
				x += WorldGen.genRand.Next(-30, 31);
				y += WorldGen.genRand.Next(-30, 31);
				int type = -1;
				if (WorldGen.genRand.NextBool(7))
					type = -2;

				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(30, 70), type);
			}
		}

		iterations = 300f * WorldScaleX;
		for (int i = 0; i <= iterations; i++)
		{
			Progress.Set(i, iterations, 1 / 11f / 3f, 10 / 11f + 2 / 11f / 3f);

			do
			{
				x = WorldGen.genRand.Next(xMin, xMax);
				y = WorldGen.genRand.Next(yMin, yMax);
			} while (Main.tile[x, y].HasTile && Main.tile[x, y].TileType != TileID.Mud);

			WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 30), 1);
			if (WorldGen.genRand.NextBool(4))
			{
				int type = WorldGen.genRand.Next(TileID.Sapphire, TileID.Diamond + 1);
				WorldGen.TileRunner(x + WorldGen.genRand.Next(-1, 2), y + WorldGen.genRand.Next(-1, 2),
					WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(4, 8), type);
			}
		}
	}
}