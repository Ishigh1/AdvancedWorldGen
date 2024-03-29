namespace AdvancedWorldGen.BetterVanillaWorldGen.DungeonStuff;

public partial class DungeonPass
{
	private static List<(int x, int y)> DungeonRoomPos = null!;
	private static List<int> DungeonRoomSize = null!;
	private static List<int> DungeonRoomL = null!;
	private static List<int> DungeonRoomR = null!;
	private static List<int> DungeonRoomT = null!;
	private static List<int> DungeonRoomB = null!;

	private static List<(int x, int y, int pos)> Doors = null!;

	private static List<(int x, int y)> DungeonPlatforms = null!;

	private static int DungeonEntranceX;
	private static bool DungeonSurface;
	private static double DungeonXStrength1;
	private static double DungeonYStrength1;
	private static double DungeonXStrength2;
	private static double DungeonYStrength2;

	public static bool IsDungeon(int x, int y)
	{
		if (y < Main.worldSurface)
			return false;

		if (x < 0 || x > Main.maxTilesX)
			return false;

		return Main.wallDungeon[Main.tile[x, y].WallType];
	}

	private static void DungeonInit()
	{
		DungeonEntranceX = 0;
		DungeonRoomPos = new List<(int x, int y)>();
		DungeonRoomSize = new List<int>();
		DungeonRoomL = new List<int>();
		DungeonRoomR = new List<int>();
		DungeonRoomT = new List<int>();
		DungeonRoomB = new List<int>();
		Doors = new List<(int x, int y, int pos)>();
		DungeonPlatforms = new List<(int x, int y)>();
	}

	private void MakeDungeon(int dungeonX, int dungeonY)
	{
		DungeonInit();

		ushort tileType;
		ushort wallType;
		switch (WorldGen.genRand.Next(3))
		{
			case 0:
				tileType = TileID.BlueDungeonBrick;
				wallType = WallID.BlueDungeonUnsafe;
				GenVars.crackedType = TileID.CrackedBlueDungeonBrick;
				break;
			case 1:
				tileType = TileID.GreenDungeonBrick;
				wallType = WallID.GreenDungeonUnsafe;
				GenVars.crackedType = TileID.CrackedGreenDungeonBrick;
				break;
			default:
				tileType = TileID.PinkDungeonBrick;
				wallType = WallID.PinkDungeonUnsafe;
				GenVars.crackedType = TileID.CrackedPinkDungeonBrick;
				break;
		}

		Main.tileSolid[GenVars.crackedType] = false;
		GenVars.dungeonLake = true;
		if (VanillaInterface.Calamity.Enabled)
		{
			GenVars.dungeonX = Utils.Clamp(dungeonX, VanillaInterface.Calamity.SulphurousSeaBiomeWidth + 100,
				Main.maxTilesX - VanillaInterface.Calamity.SulphurousSeaBiomeWidth - 101);
			WorldUtils.Find(new Point(dungeonX, dungeonY),
				Searches.Chain(new Searches.Down(9001), new Conditions.IsSolid()), out Point result);
			GenVars.dungeonY = result.Y - 10;
		}
		else
		{
			GenVars.dungeonX = dungeonX;
			GenVars.dungeonY = dungeonY;
		}

		GenVars.dMinX = dungeonX;
		GenVars.dMaxX = dungeonX;
		GenVars.dMinY = dungeonY;
		GenVars.dMaxY = dungeonY;
		DungeonXStrength1 = WorldGen.genRand.Next(25, 30);
		DungeonYStrength1 = WorldGen.genRand.Next(20, 25);
		DungeonXStrength2 = WorldGen.genRand.Next(35, 50);
		DungeonYStrength2 = WorldGen.genRand.Next(10, 15);

		int maxRooms = (int)(Main.maxTilesX * Main.maxTilesY * Params.DungeonMultiplier / (1200 * 60));
		maxRooms += WorldGen.genRand.Next(maxRooms / 3);

		int num6 = 5;
		DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, tileType, wallType);
		for (int room = 0; room < maxRooms; room++)
		{
			GenVars.dMinX = Math.Min(GenVars.dMinX, GenVars.dungeonX);
			GenVars.dMaxX = Math.Max(GenVars.dMaxX, GenVars.dungeonX);
			GenVars.dMaxY = Math.Max(GenVars.dMaxY, GenVars.dungeonY);

			Progress.Set(room, maxRooms, 0.6f);
			if (--num6 <= 0 && WorldGen.genRand.NextBool(3))
			{
				num6 = 5;
				if (WorldGen.genRand.NextBool(2))
				{
					int x = GenVars.dungeonX;
					int y = GenVars.dungeonY;
					DungeonHalls(GenVars.dungeonX, GenVars.dungeonY, tileType, wallType);
					if (WorldGen.genRand.NextBool(2))
						DungeonHalls(GenVars.dungeonX, GenVars.dungeonY, tileType, wallType);

					DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, tileType, wallType);
					GenVars.dungeonX = x;
					GenVars.dungeonY = y;
				}
				else
				{
					DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, tileType, wallType);
				}
			}
			else
			{
				DungeonHalls(GenVars.dungeonX, GenVars.dungeonY, tileType, wallType);
			}
		}

		DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, tileType, wallType);
		(int dX, int dY) = DungeonRoomPos[0];
		GenVars.dungeonX = dX;
		GenVars.dungeonY = dY;
		DungeonEntranceX = dX;
		DungeonSurface = false;
		num6 = 5;
		if (WorldGen.drunkWorldGen) DungeonSurface = true;

		while (!DungeonSurface)
		{
			if (--num6 <= 0 && WorldGen.genRand.NextBool(5) && GenVars.dungeonY > Main.worldSurface + 100.0)
			{
				num6 = 10;
				int num11 = GenVars.dungeonX;
				int num12 = GenVars.dungeonY;
				DungeonHalls(GenVars.dungeonX, GenVars.dungeonY, tileType, wallType, true);
				DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, tileType, wallType);
				GenVars.dungeonX = num11;
				GenVars.dungeonY = num12;
			}

			DungeonStairs(GenVars.dungeonX, GenVars.dungeonY, tileType, wallType);
		}

		DungeonEntrance(GenVars.dungeonX, GenVars.dungeonY, tileType, wallType);

		Progress.Set(65, 100);
		int num13 = Main.maxTilesX * 2;
		for (int num14 = 0; num14 < num13; num14++)
		{
			int x1 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
			int y1 = WorldGen.genRand.Next((int)Math.Max(GenVars.dMinY, Main.worldSurface), GenVars.dMaxY);
			num14 = !DungeonPitTrap(x1, y1, tileType, wallType) ? num14 + 1 : num14 + 1500;
		}

		for (int k = 0; k < DungeonRoomPos.Count; k++)
		{
			for (int l = DungeonRoomL[k]; l <= DungeonRoomR[k]; l++)
				if (!Main.tile[l, DungeonRoomT[k] - 1].HasTile)
				{
					DungeonPlatforms.Add((l, DungeonRoomT[k] - 1));
					break;
				}

			for (int m = DungeonRoomL[k]; m <= DungeonRoomR[k]; m++)
				if (!Main.tile[m, DungeonRoomB[k] + 1].HasTile)
				{
					DungeonPlatforms.Add((m, DungeonRoomT[k] + 1));
					break;
				}

			for (int n = DungeonRoomT[k]; n <= DungeonRoomB[k]; n++)
				if (!Main.tile[DungeonRoomL[k] - 1, n].HasTile)
				{
					Doors.Add((DungeonRoomL[k] - 1, n, -1));
					break;
				}

			for (int num16 = DungeonRoomT[k]; num16 <= DungeonRoomB[k]; num16++)
				if (!Main.tile[DungeonRoomR[k] + 1, num16].HasTile)
				{
					Doors.Add((DungeonRoomL[k] + 1, num16, 1));
					break;
				}
		}

		Progress.Set(70, 100);
		int num17 = 0;
		const int maxTries = 1000;
		int num19 = 0;
		int num20 = Main.maxTilesX / 100;
		if (WorldGen.getGoodWorldGen)
			num20 *= 3;

		while (num19 < num20)
		{
			num17++;
			int x0 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
			int y0;
			if (WorldGen.drunkWorldGen && GenVars.dungeonY + 25 >= GenVars.dMaxY)
				y0 = GenVars.dMaxY;
			else
				y0 = WorldGen.genRand.Next((int)Main.worldSurface + 25, GenVars.dMaxY);

			int num23 = x0;
			if (Main.tile[x0, y0].WallType == wallType && !Main.tile[x0, y0].HasTile)
			{
				int num24 = 1;
				if (WorldGen.genRand.NextBool(2))
					num24 = -1;

				for (; !Main.tile[x0, y0].HasTile; y0 += num24)
				{
				}

				if (Main.tile[x0 - 1, y0].HasTile && Main.tile[x0 + 1, y0].HasTile &&
				    Main.tile[x0 - 1, y0].TileType != GenVars.crackedType &&
				    !Main.tile[x0 - 1, y0 - num24].HasTile && !Main.tile[x0 + 1, y0 - num24].HasTile)
				{
					num19++;
					int num25 = WorldGen.genRand.Next(5, 13);
					Tile tile = Main.tile[x0, y0 - num24];
					Tile tile1 = Main.tile[x0, y0 - num24 * 2];

					while (Main.tile[x0 - 1, y0].HasTile &&
					       Main.tile[x0 - 1, y0].TileType != GenVars.crackedType &&
					       Main.tile[x0, y0 + num24].HasTile && Main.tile[x0, y0].HasTile &&
					       !tile.HasTile && num25 > 0)
					{
						Main.tile[x0, y0].TileType = TileID.Spikes;
						if (!Main.tile[x0 - 1, y0 - num24].HasTile &&
						    !Main.tile[x0 + 1, y0 - num24].HasTile)
						{
							tile.Clear(TileDataType.Slope);
							tile.TileType = TileID.Spikes;
							tile.HasTile = true;
							tile1.Clear(TileDataType.Slope);
							tile1.TileType = TileID.Spikes;
							tile1.HasTile = true;
						}

						x0--;
						num25--;
						tile = Main.tile[x0, y0 - num24];
						tile1 = Main.tile[x0, y0 - num24 * 2];
					}

					num25 = WorldGen.genRand.Next(5, 13);
					x0 = num23 + 1;
					tile = Main.tile[x0, y0 - num24];
					tile1 = Main.tile[x0, y0 - num24 * 2];

					while (Main.tile[x0 + 1, y0].HasTile &&
					       Main.tile[x0 + 1, y0].TileType != GenVars.crackedType &&
					       Main.tile[x0, y0 + num24].HasTile && Main.tile[x0, y0].HasTile &&
					       !tile.HasTile && num25 > 0)
					{
						Main.tile[x0, y0].TileType = TileID.Spikes;
						if (!Main.tile[x0 - 1, y0 - num24].HasTile &&
						    !Main.tile[x0 + 1, y0 - num24].HasTile)
						{
							tile.Clear(TileDataType.Slope);
							tile.TileType = TileID.Spikes;
							tile.HasTile = true;
							tile1.Clear(TileDataType.Slope);
							tile1.TileType = TileID.Spikes;
							tile1.HasTile = true;
						}

						x0++;
						num25--;
						tile = Main.tile[x0, y0 - num24];
						tile1 = Main.tile[x0, y0 - num24 * 2];
					}
				}
			}

			if (num17 > maxTries)
			{
				num17 = 0;
				num19++;
			}
		}

		num17 = 0;
		num19 = 0;
		Progress.Set(75, 100);
		while (num19 < num20)
		{
			num17++;
			int x = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
			int y = WorldGen.genRand.Next((int)Main.worldSurface + 25, GenVars.dMaxY);
			int num28 = y;
			if (Main.tile[x, y].WallType == wallType && !Main.tile[x, y].HasTile)
			{
				int num29 = 1;
				if (WorldGen.genRand.NextBool(2))
					num29 = -1;

				for (; x > 5 && x < Main.maxTilesX - 6 && !Main.tile[x, y].HasTile; x += num29)
				{
				}

				if (Main.tile[x, y - 1].HasTile && Main.tile[x, y + 1].HasTile &&
				    Main.tile[x, y - 1].TileType != GenVars.crackedType &&
				    !Main.tile[x - num29, y - 1].HasTile && !Main.tile[x - num29, y + 1].HasTile)
				{
					num19++;
					int num30 = WorldGen.genRand.Next(5, 13);
					Tile tile = Main.tile[x - num29, y];
					Tile tile1 = Main.tile[x - num29 * 2, y];

					while (Main.tile[x, y - 1].HasTile &&
					       Main.tile[x, y - 1].TileType != GenVars.crackedType &&
					       Main.tile[x + num29, y].HasTile && Main.tile[x, y].HasTile &&
					       !tile.HasTile && num30 > 0)
					{
						Main.tile[x, y].TileType = TileID.Spikes;
						if (!Main.tile[x - num29, y - 1].HasTile &&
						    !Main.tile[x - num29, y + 1].HasTile)
						{
							tile.TileType = TileID.Spikes;
							tile.HasTile = true;
							tile.Clear(TileDataType.Slope);
							tile1.TileType = TileID.Spikes;
							tile1.HasTile = true;
							tile1.Clear(TileDataType.Slope);
						}

						y--;
						num30--;
						tile = Main.tile[x - num29, y];
						tile1 = Main.tile[x - num29 * 2, y];
					}

					num30 = WorldGen.genRand.Next(5, 13);
					y = num28 + 1;
					tile = Main.tile[x - num29, y];
					tile1 = Main.tile[x - num29 * 2, y];
					while (Main.tile[x, y + 1].HasTile &&
					       Main.tile[x, y + 1].TileType != GenVars.crackedType &&
					       Main.tile[x + num29, y].HasTile && Main.tile[x, y].HasTile &&
					       !tile.HasTile && num30 > 0)
					{
						Main.tile[x, y].TileType = TileID.Spikes;
						if (!Main.tile[x - num29, y - 1].HasTile &&
						    !Main.tile[x - num29, y + 1].HasTile)
						{
							tile.TileType = TileID.Spikes;
							tile.HasTile = true;
							tile.Clear(TileDataType.Slope);
							tile1.TileType = TileID.Spikes;
							tile1.HasTile = true;
							tile1.Clear(TileDataType.Slope);
						}

						y++;
						num30--;
						tile = Main.tile[x - num29, y];
						tile1 = Main.tile[x - num29 * 2, y];
					}
				}
			}

			if (num17 > maxTries)
			{
				num17 = 0;
				num19++;
			}
		}

		Progress.Set(80, 100);
		foreach ((int doorX, int doorY, int pos) in Doors)
		{
			int num34 = 100;
			int num35 = 0;
			for (int x = doorX - 10; x < doorX + 10; x++)
			{
				bool flag = true;
				int y = doorY;
				while (y > 10 && !Main.tile[x, y].HasTile) y--;

				if (!Main.tileDungeon[Main.tile[x, y].TileType])
					flag = false;

				int oldY = y;
				for (y = doorY; !Main.tile[x, y].HasTile; y++)
				{
				}

				if (!Main.tileDungeon[Main.tile[x, y].TileType])
					flag = false;

				if (y - oldY < 3)
					continue;

				for (int xx = x - 20; xx < x + 20; xx++)
				for (int yy = y - 10; yy < y + 10; yy++)
					if (Main.tile[xx, yy].HasTile && Main.tile[xx, yy].TileType == TileID.ClosedDoor)
					{
						flag = false;
						break;
					}

				if (flag)
					for (int yy = y - 3; yy < y; yy++)
					for (int xx = x - 3; xx <= x + 3; xx++)
						if (Main.tile[xx, yy].HasTile)
						{
							flag = false;
							break;
						}

				if (flag && y - oldY < 20)
					if ((pos == 0 && y - oldY < num34) || (pos == -1 && x > num35) ||
					    (pos == 1 && (x < num35 || num35 == 0)))
					{
						num35 = x;
						num34 = y - oldY;
					}
			}

			if (num34 >= 20)
				continue;

			int x0 = num35;
			int y0 = doorY;
			int num50 = y0;
			Tile tile;
			for (; !Main.tile[x0, y0].HasTile; y0++)
			{
				tile = Main.tile[x0, y0];
				tile.HasTile = false;
			}

			while (!Main.tile[x0, num50].HasTile) num50--;

			y0--;
			num50++;
			for (int num51 = num50; num51 < y0 - 2; num51++)
			{
				Tile tile1 = Main.tile[x0, num51];
				tile1.Clear(TileDataType.Slope);
				tile1.HasTile = true;
				tile1.TileType = tileType;

				for (int i = -2; i <= 2; i++)
				{
					if (i == 0) continue;
					tile1 = Main.tile[x0 + i, num51];
					if (tile1.TileType == tileType)
					{
						tile1.HasTile = false;
						tile1.ClearEverything();
						tile1.WallType = wallType;
					}
				}
			}

			int style = 13;
			if (WorldGen.genRand.NextBool(3))
				style = wallType switch
				{
					7 => 16,
					8 => 17,
					9 => 18,
					_ => style
				};

			WorldGen.PlaceTile(x0, y0, TileID.ClosedDoor, true, false, -1, style);
			x0--;
			int num52 = y0 - 3;
			while (!Main.tile[x0, num52].HasTile) num52--;

			if (y0 - num52 < y0 - num50 + 5 && Main.tileDungeon[Main.tile[x0, num52].TileType])
				for (int num53 = y0 - 4 - WorldGen.genRand.Next(3); num53 > num52; num53--)
				{
					Tile tile1 = Main.tile[x0, num53];
					tile1.Clear(TileDataType.Slope);
					tile1.HasTile = true;
					tile1.TileType = tileType;

					for (int i = -2; i < 0; i++)
					{
						tile1 = Main.tile[x0 + i, num53];
						if (tile1.TileType == tileType)
						{
							tile1.HasTile = false;
							tile1.ClearEverything();
							tile1.WallType = wallType;
						}
					}
				}

			x0 += 2;
			num52 = y0 - 3;
			while (!Main.tile[x0, num52].HasTile) num52--;

			if (y0 - num52 < y0 - num50 + 5 && Main.tileDungeon[Main.tile[x0, num52].TileType])
				for (int num54 = y0 - 4 - WorldGen.genRand.Next(3); num54 > num52; num54--)
				{
					Tile tile1 = Main.tile[x0, num54];
					tile1.HasTile = true;
					tile1.Clear(TileDataType.Slope);
					tile1.TileType = tileType;

					for (int i = 1; i <= 2; i++)
					{
						tile1 = Main.tile[x0 + i, num54];
						if (tile1.TileType == tileType)
						{
							tile1.HasTile = false;
							tile1.ClearEverything();
							tile1.WallType = wallType;
						}
					}
				}

			y0++;
			x0--;

			for (int num51 = y0 - 8; num51 < y0; num51++)
			for (int i = -3; i <= 3; i++)
			{
				if (i is >= -1 and <= 1) continue;
				Tile tile1 = Main.tile[x0 + i, num51];
				if (tile1.TileType == tileType)
				{
					tile1.HasTile = false;
					tile1.ClearEverything();
					tile1.WallType = wallType;
				}
			}

			Tile tile2 = Main.tile[x0 - 1, y0];
			tile2.HasTile = true;
			tile2.TileType = tileType;
			tile2.Clear(TileDataType.Slope);
			Tile tile3 = Main.tile[x0 + 1, y0];
			tile3.HasTile = true;
			tile3.TileType = tileType;
			tile3.Clear(TileDataType.Slope);
		}

		int[] wallTypes = wallType switch
		{
			WallID.BlueDungeonUnsafe => new int[]
			{
				WallID.BlueDungeonUnsafe,
				WallID.BlueDungeonSlabUnsafe,
				WallID.BlueDungeonTileUnsafe
			},
			WallID.PinkDungeonUnsafe => new int[]
			{
				WallID.PinkDungeonUnsafe,
				WallID.PinkDungeonSlabUnsafe,
				WallID.PinkDungeonTileUnsafe
			},
			_ => new int[]
			{
				WallID.GreenDungeonUnsafe,
				WallID.GreenDungeonSlabUnsafe,
				WallID.GreenDungeonTileUnsafe
			}
		};

		for (int _ = 0; _ < 5; _++)
		for (int i = 0; i < 3; i++)
		{
			int range = WorldGen.genRand.Next(40, 240);
			int randX = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
			int randY = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
			for (int x = randX - range; x < randX + range; x++)
			for (int y = randY - range; y < randY + range; y++)
				if (y > Main.worldSurface)
				{
					float distX = Math.Abs(randX - x);
					float distY = Math.Abs(randY - y);
					if (Math.Sqrt(distX * distX + distY * distY) < range * 0.4 &&
					    Main.wallDungeon[Main.tile[x, y].WallType])
						WorldGen.Spread.WallDungeon(x, y, wallTypes[i]);
				}
		}

		Progress.Set(85, 100);
		foreach ((int platformX, int platformY) in DungeonPlatforms)
		{
			int y = int.MaxValue;
			int offX = 10;
			if (platformY < Main.worldSurface + 50.0)
				offX = 20;

			for (int y1 = platformY - 5; y1 <= platformY + 5; y1++)
			{
				int x1 = platformX;
				int x2 = platformX;
				if (!Main.tile[x1, y1].HasTile)
				{
					bool flag3 = false;
					while (!Main.tile[x1, y1].HasTile)
					{
						x1--;
						if (!Main.tileDungeon[Main.tile[x1, y1].TileType] || x1 == 0)
						{
							flag3 = true;
							break;
						}
					}

					if (flag3)
						continue;

					while (!Main.tile[x2, y1].HasTile)
					{
						x2++;
						if (!Main.tileDungeon[Main.tile[x2, y1].TileType] || x2 == Main.maxTilesX - 1)
						{
							flag3 = true;
							break;
						}
					}

					if (flag3)
						continue;
				}
				else
				{
					continue;
				}

				if (x2 - x1 > offX)
					continue;

				bool flag4 = true;
				for (int xx = platformX - offX / 2 - 2; xx <= platformX + offX / 2 + 2; xx++)
				{
					for (int yy = y1 - 5; yy <= y1 + 5; yy++)
						if (Main.tile[xx, yy].HasTile && Main.tile[xx, yy].TileType == TileID.Platforms)
						{
							flag4 = false;
							break;
						}

					if (!flag4)
						break;
				}

				for (int yy = y1 + 3; yy >= y1 - 5; yy--)
					if (Main.tile[platformX, yy].HasTile)
					{
						flag4 = false;
						break;
					}

				if (flag4)
				{
					y = y1;
					break;
				}
			}

			if (y <= platformY - 10 || y >= platformY + 10)
				continue;

			int x = platformX;
			// Place to the left
			Tile tile = Main.tile[x, y];
			while (!tile.HasTile)
			{
				tile.HasTile = true;
				tile.TileType = TileID.Platforms;
				tile.Clear(TileDataType.Slope);
				tile.TileFrameY = wallType switch
				{
					WallID.BlueDungeonUnsafe => 6 * 18,
					WallID.GreenDungeonUnsafe => 8 * 18,
					_ => 7 * 18
				};

				WorldGen.TileFrame(x, y);
				x--;
				tile = Main.tile[x, y];
			}

			x = platformX + 1;
			tile = Main.tile[x, y];
			// Place to the right
			for (; !tile.HasTile; x++)
			{
				tile = Main.tile[x, y];
				tile.HasTile = true;
				tile.TileType = TileID.Platforms;
				tile.Clear(TileDataType.Slope);
				tile.TileFrameY = wallType switch
				{
					WallID.BlueDungeonUnsafe => 108,
					WallID.GreenDungeonUnsafe => 144,
					_ => 126
				};

				WorldGen.TileFrame(x, y);
			}
		}

		int evilChests = OptionHelper.OptionsContains("Drunk.Crimruption") ? 6 : 5;

		for (int numChest = 0; numChest < evilChests; numChest++)
		{
			int contain = 0;
			int style2 = 0;
			ushort chestTileType = TileID.Containers;
			switch (numChest)
			{
				case 0:
					style2 = 23;
					contain = ItemID.PiranhaGun;
					break;
				case 1:
					if (!WorldGen.crimson)
					{
						style2 = 24;
						contain = ItemID.ScourgeoftheCorruptor;
					}
					else
					{
						style2 = 25;
						contain = ItemID.VampireKnives;
					}

					break;
				case 5:
					if (WorldGen.crimson)
					{
						style2 = 24;
						contain = ItemID.ScourgeoftheCorruptor;
					}
					else
					{
						style2 = 25;
						contain = ItemID.VampireKnives;
					}

					break;
				case 2:
					style2 = 26;
					contain = ItemID.RainbowGun;
					break;
				case 3:
					style2 = 27;
					contain = ItemID.StaffoftheFrostHydra;
					break;
				case 4:
					chestTileType = TileID.Containers2;
					style2 = 13;
					contain = ItemID.StormTigerStaff;
					break;
			}

			bool chestPlaced = false;
			while (!chestPlaced)
			{
				int randX = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
				int randY = WorldGen.genRand.Next((int)Main.worldSurface, GenVars.dMaxY);
				if (!Main.wallDungeon[Main.tile[randX, randY].WallType] || Main.tile[randX, randY].HasTile)
					continue;

				chestPlaced = GenerationChests.AddBuriedChest(randX, randY, contain, false, style2, chestTileType);
			}
		}

		int[] frameMult =
		{
			WorldGen.genRand.Next(9, 13),
			WorldGen.genRand.Next(9, 13),
			WorldGen.genRand.Next(9, 13)
		};

		while (frameMult[1] == frameMult[0])
			frameMult[1] = WorldGen.genRand.Next(9, 13);

		while (frameMult[2] == frameMult[0] || frameMult[2] == frameMult[1])
			frameMult[2] = WorldGen.genRand.Next(9, 13);

		Progress.Set(90, 100);
		num17 = 0;
		num19 = 0;
		while (num19 < Main.maxTilesX / 20)
		{
			num17++;
			int x = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
			int y = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
			bool flag6 = true;
			if (Main.wallDungeon[Main.tile[x, y].WallType] && !Main.tile[x, y].HasTile)
			{
				int direction = 1;
				if (WorldGen.genRand.NextBool(2))
					direction = -1;

				while (flag6 && !Main.tile[x, y].HasTile)
				{
					x -= direction;
					if (x < 5 || x >= Main.maxTilesX - 5)
						flag6 = false;
					else if (Main.tile[x, y].HasTile && !Main.tileDungeon[Main.tile[x, y].TileType])
						flag6 = false;
				}

				if (flag6 &&
				    Main.tile[x, y].HasTile && Main.tileDungeon[Main.tile[x, y].TileType] &&
				    Main.tile[x, y - 1].HasTile && Main.tileDungeon[Main.tile[x, y - 1].TileType] &&
				    Main.tile[x, y + 1].HasTile && Main.tileDungeon[Main.tile[x, y + 1].TileType])
				{
					x += direction;
					for (int xx = x - 3; xx <= x + 3; xx++)
					for (int yy = y - 3; yy <= y + 3; yy++)
						if (Main.tile[xx, yy].HasTile && Main.tile[xx, yy].TileType == TileID.Platforms)
						{
							flag6 = false;
							break;
						}

					if (flag6 && !Main.tile[x, y - 1].HasTile && !Main.tile[x, y - 2].HasTile &&
					    !Main.tile[x, y - 3].HasTile)
					{
						int distX = x;
						int oldX = x;
						for (;
						     distX > GenVars.dMinX && distX < GenVars.dMaxX &&
						     !Main.tile[distX, y].HasTile &&
						     !Main.tile[distX, y - 1].HasTile &&
						     !Main.tile[distX, y + 1].HasTile;
						     distX += direction)
						{
						}

						distX = Math.Abs(x - distX);
						bool books = WorldGen.genRand.NextBool(2);

						if (distX > 5)
						{
							for (int _ = WorldGen.genRand.Next(1, 4); _ > 0; _--)
							{
								Tile tile = Main.tile[x, y];
								tile.HasTile = true;
								tile.Clear(TileDataType.Slope);
								tile.TileType = TileID.Platforms;
								if (tile.WallType == wallTypes[0])
									tile.TileFrameY = (short)(18 * frameMult[0]);
								else if (tile.WallType == wallTypes[1])
									tile.TileFrameY = (short)(18 * frameMult[1]);
								else
									tile.TileFrameY = (short)(18 * frameMult[2]);

								WorldGen.TileFrame(x, y);
								if (books)
								{
									WorldGen.PlaceTile(x, y - 1, TileID.Books, true);
									if (WorldGen.genRand.NextBool(50) &&
									    y > (Main.worldSurface + Main.rockLayer) / 2.0 &&
									    Main.tile[x, y - 1].TileType == TileID.Books)
										Main.tile[x, y - 1].TileFrameX = 90;
								}

								x += direction;
							}

							num17 = 0;
							num19++;
							if (!books && WorldGen.genRand.NextBool(2))
							{
								x = oldX;
								y--;
								int type = 0;
								if (WorldGen.genRand.NextBool(4))
									type = 1;

								type = type switch
								{
									0 => TileID.Bottles,
									1 => TileID.WaterCandle,
									_ => type // Impossible
								};

								WorldGen.PlaceTile(x, y, type, true);
								if (Main.tile[x, y].TileType == TileID.Bottles)
									Main.tile[x, y].TileFrameX = WorldGen.genRand.NextBool(2) ? (short)18 : (short)36;
							}
						}
					}
				}
			}

			if (num17 > maxTries)
			{
				num17 = 0;
				num19++;
			}
		}

		Progress.Set(95, 100);
		int num95 = 1;
		for (int index = 0; index < DungeonRoomPos.Count; index++)
		{
			int tries = 0;
			while (tries < 1000)
			{
				int offset = (int)(DungeonRoomSize[index] * 0.4);
				(int x, int y) = DungeonRoomPos[index];
				x += WorldGen.genRand.Next(-offset, offset + 1);
				y += WorldGen.genRand.Next(-offset, offset + 1);
				int style3 = 2;
				if (num95 == 1)
					num95++;

				int itemType;
				switch (num95)
				{
					case 2:
						itemType = ItemID.Muramasa;
						break;
					case 3:
						itemType = ItemID.CobaltShield;
						break;
					case 4:
						itemType = WorldGen.remixWorldGen ? ItemID.BubbleGun : ItemID.AquaScepter;
						break;
					case 5:
						itemType = ItemID.BlueMoon;
						break;
					case 6:
						itemType = ItemID.MagicMissile;
						break;
					case 7:
						itemType = ItemID.Valor;
						break;
					case 8:
						itemType = ItemID.GoldenKey;
						style3 = 0;
						break;
					default:
						itemType = ItemID.Handgun;
						num95 = 0;
						break;
				}

				if (y < Main.worldSurface + 50.0)
				{
					itemType = 327;
					style3 = 0;
				}

				if (GenerationChests.AddBuriedChest(x, y, itemType, false, style3))
				{
					num95++;
					break;
				}

				tries++;
			}
		}

		GenVars.dMinX = Math.Max(GenVars.dMinX - 25, 0);
		GenVars.dMaxX = Math.Min(GenVars.dMaxX + 25, Main.maxTilesX);
		GenVars.dMinY = Math.Max(GenVars.dMinY - 25, 0);
		GenVars.dMaxY = Math.Min(GenVars.dMaxY + 25, Main.maxTilesY);

		MakeDungeon_Lights(tileType, wallTypes);
		MakeDungeon_Traps();
		MakeDungeon_GroundFurniture(wallType);
		MakeDungeon_Pictures(wallTypes);
		MakeDungeon_Banners(wallTypes);
	}
}