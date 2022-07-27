namespace AdvancedWorldGen.BetterVanillaWorldGen.DungeonStuff;

public partial class DungeonPass
{
	public static List<(int x, int y)> DungeonRoomPos = null!;
	public static List<int> DungeonRoomSize = null!;
	public static List<int> DungeonRoomL = null!;
	public static List<int> DungeonRoomR = null!;
	public static List<int> DungeonRoomT = null!;
	public static List<int> DungeonRoomB = null!;

	public static List<(int x, int y, int pos)> Doors = null!;

	public static List<(int x, int y)> DungeonPlatforms = null!;

	public static int DungeonEntranceX;
	public static bool DungeonSurface;
	public static double DungeonXStrength1;
	public static double DungeonYStrength1;
	public static double DungeonXStrength2;
	public static double DungeonYStrength2;

	public static bool IsDungeon(int x, int y)
	{
		if (y < Main.worldSurface)
			return false;

		if (x < 0 || x > Main.maxTilesX)
			return false;

		return Main.wallDungeon[Main.tile[x, y].WallType];
	}

	public static void DungeonInit()
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

	public void MakeDungeon(int dungeonX, int dungeonY)
	{
		DungeonInit();

		ushort tileType;
		ushort wallType;
		switch (WorldGen.genRand.Next(3))
		{
			case 0:
				tileType = TileID.BlueDungeonBrick;
				wallType = WallID.BlueDungeonUnsafe;
				WorldGen.crackedType = TileID.CrackedBlueDungeonBrick;
				break;
			case 1:
				tileType = TileID.GreenDungeonBrick;
				wallType = WallID.GreenDungeonUnsafe;
				WorldGen.crackedType = TileID.CrackedGreenDungeonBrick;
				break;
			default:
				tileType = TileID.PinkDungeonBrick;
				wallType = WallID.PinkDungeonUnsafe;
				WorldGen.crackedType = TileID.CrackedPinkDungeonBrick;
				break;
		}

		Main.tileSolid[WorldGen.crackedType] = false;
		WorldGen.dungeonLake = true;
		WorldGen.dungeonX = dungeonX;
		WorldGen.dungeonY = dungeonY;
		WorldGen.dMinX = dungeonX;
		WorldGen.dMaxX = dungeonX;
		WorldGen.dMinY = dungeonY;
		WorldGen.dMaxY = dungeonY;
		DungeonXStrength1 = WorldGen.genRand.Next(25, 30);
		DungeonYStrength1 = WorldGen.genRand.Next(20, 25);
		DungeonXStrength2 = WorldGen.genRand.Next(35, 50);
		DungeonYStrength2 = WorldGen.genRand.Next(10, 15);

		int maxRooms = (int)(Main.maxTilesX * Main.maxTilesY * ModifiedWorld.Instance.OptionHelper.WorldSettings.Params.DungeonMultiplier / (1200 * 60));
		maxRooms += WorldGen.genRand.Next(maxRooms / 3);

		int num6 = 5;
		DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
		int x00 = WorldGen.dungeonX;
		int y00 = WorldGen.dungeonY;
		for (int room = 0; room < maxRooms; room++)
		{
			WorldGen.dMinX = Math.Min(WorldGen.dMinX, WorldGen.dungeonX);
			WorldGen.dMaxX = Math.Max(WorldGen.dMaxX, WorldGen.dungeonX);
			WorldGen.dMaxY = Math.Max(WorldGen.dMaxY, WorldGen.dungeonY);

			Progress.Set(room, maxRooms, 0.6f);
			if (--num6 <= 0 && WorldGen.genRand.NextBool(3))
			{
				num6 = 5;
				if (WorldGen.genRand.NextBool(2))
				{
					int x = WorldGen.dungeonX;
					int y = WorldGen.dungeonY;
					DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
					if (WorldGen.genRand.NextBool(2))
						DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);

					DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
					WorldGen.dungeonX = x;
					WorldGen.dungeonY = y;
				}
				else
				{
					DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
				}
			}
			else
			{
				DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
			}
		}

		DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
		(int dX, int dY) = DungeonRoomPos[0];
		WorldGen.dungeonX = dX;
		WorldGen.dungeonY = dY;
		DungeonEntranceX = dX;
		DungeonSurface = false;
		num6 = 5;
		if (WorldGen.drunkWorldGen) DungeonSurface = true;

		while (!DungeonSurface)
		{
			if (--num6 <= 0 && WorldGen.genRand.NextBool(5) && WorldGen.dungeonY > Main.worldSurface + 100.0)
			{
				num6 = 10;
				int num11 = WorldGen.dungeonX;
				int num12 = WorldGen.dungeonY;
				DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, true);
				DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
				WorldGen.dungeonX = num11;
				WorldGen.dungeonY = num12;
			}

			DungeonStairs(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
		}

		DungeonEntrance(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);

		Progress.Set(65, 100);
		int num13 = Main.maxTilesX * 2;
		for (int num14 = 0; num14 < num13; num14++)
		{
			int x1 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
			int y1 = WorldGen.genRand.Next((int)Math.Max(WorldGen.dMinY, Main.worldSurface), WorldGen.dMaxY);
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
		int num18 = 1000;
		int num19 = 0;
		int num20 = Main.maxTilesX / 100;
		if (WorldGen.getGoodWorldGen)
			num20 *= 3;

		while (num19 < num20)
		{
			num17++;
			int x0 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
			int y0;
			if (WorldGen.drunkWorldGen && WorldGen.dungeonY + 25 >= WorldGen.dMaxY)
				y0 = WorldGen.dMaxY;
			else
				y0 = WorldGen.genRand.Next((int)Main.worldSurface + 25, WorldGen.dMaxY);

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
				    Main.tile[x0 - 1, y0].TileType != WorldGen.crackedType &&
				    !Main.tile[x0 - 1, y0 - num24].HasTile && !Main.tile[x0 + 1, y0 - num24].HasTile)
				{
					num19++;
					int num25 = WorldGen.genRand.Next(5, 13);
					Tile tile = Main.tile[x0, y0 - num24];
					Tile tile1 = Main.tile[x0, y0 - num24 * 2];

					while (Main.tile[x0 - 1, y0].HasTile &&
					       Main.tile[x0 - 1, y0].TileType != WorldGen.crackedType &&
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
					       Main.tile[x0 + 1, y0].TileType != WorldGen.crackedType &&
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

			if (num17 > num18)
			{
				num17 = 0;
				num19++;
			}
		}

		num17 = 0;
		num18 = 1000;
		num19 = 0;
		Progress.Set(75, 100);
		while (num19 < num20)
		{
			num17++;
			int x = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
			int y = WorldGen.genRand.Next((int)Main.worldSurface + 25, WorldGen.dMaxY);
			int num28 = y;
			if (Main.tile[x, y].WallType == wallType && !Main.tile[x, y].HasTile)
			{
				int num29 = 1;
				if (WorldGen.genRand.NextBool(2))
					num29 = -1;

				for (; x > 5 && x < Main.maxTilesX - 5 && !Main.tile[x, y].HasTile; x += num29)
				{
				}

				if (Main.tile[x, y - 1].HasTile && Main.tile[x, y + 1].HasTile &&
				    Main.tile[x, y - 1].TileType != WorldGen.crackedType &&
				    !Main.tile[x - num29, y - 1].HasTile && !Main.tile[x - num29, y + 1].HasTile)
				{
					num19++;
					int num30 = WorldGen.genRand.Next(5, 13);
					Tile tile = Main.tile[x - num29, y];
					Tile tile1 = Main.tile[x - num29 * 2, y];

					while (Main.tile[x, y - 1].HasTile &&
					       Main.tile[x, y - 1].TileType != WorldGen.crackedType &&
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
					       Main.tile[x, y + 1].TileType != WorldGen.crackedType &&
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

			if (num17 > num18)
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
			Tile tile = Main.tile[x0, y0];
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
				}

			y0++;
			x0--;
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
			int randX = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
			int randY = WorldGen.genRand.Next(WorldGen.dMinY, WorldGen.dMaxY);
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

		int evilChests = 5;
		if (API.OptionsContains("Drunk.Crimruption"))
			evilChests = 6;

		for (int numChest = 0; numChest < evilChests; numChest++)
		{
			bool chestPlaced = false;
			while (!chestPlaced)
			{
				int randX = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
				int randY = WorldGen.genRand.Next((int)Main.worldSurface, WorldGen.dMaxY);
				if (!Main.wallDungeon[Main.tile[randX, randY].WallType] || Main.tile[randX, randY].HasTile)
					continue;

				ushort chestTileType = TileID.Containers;
				int contain = 0;
				int style2 = 0;
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
		num18 = 1000;
		num19 = 0;
		while (num19 < Main.maxTilesX / 20)
		{
			num17++;
			int x = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
			int y = WorldGen.genRand.Next(WorldGen.dMinY, WorldGen.dMaxY);
			bool flag6 = true;
			if (Main.wallDungeon[Main.tile[x, y].WallType] && !Main.tile[x, y].HasTile)
			{
				int direction = 1;
				if (WorldGen.genRand.NextBool(2))
					direction = -1;

				while (flag6 && !Main.tile[x, y].HasTile)
				{
					x -= direction;
					if (x < 5 || x > Main.maxTilesX - 5)
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
						     distX > WorldGen.dMinX && distX < WorldGen.dMaxX &&
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
								{
									if (WorldGen.genRand.NextBool(2))
										Main.tile[x, y].TileFrameX = 18;
									else
										Main.tile[x, y].TileFrameX = 36;
								}
							}
						}
					}
				}
			}

			if (num17 > num18)
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
						itemType = ItemID.AquaScepter;
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
					tries += 1000;
					num95++;
				}

				tries++;
			}
		}

		WorldGen.dMinX -= 25;
		WorldGen.dMaxX += 25;
		WorldGen.dMinY -= 25;
		WorldGen.dMaxY += 25;
		if (WorldGen.dMinX < 0) WorldGen.dMinX = 0;

		if (WorldGen.dMaxX > Main.maxTilesX) WorldGen.dMaxX = Main.maxTilesX;

		if (WorldGen.dMinY < 0) WorldGen.dMinY = 0;

		if (WorldGen.dMaxY > Main.maxTilesY) WorldGen.dMaxY = Main.maxTilesY;

		MakeDungeon_Lights(tileType, wallTypes);
		MakeDungeon_Traps();
		MakeDungeon_GroundFurniture(wallType);
		MakeDungeon_Pictures(wallTypes);
		MakeDungeon_Banners(wallTypes);
	}
}