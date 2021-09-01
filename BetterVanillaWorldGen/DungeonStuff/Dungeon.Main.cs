using System;
using System.Collections.Generic;
using AdvancedWorldGen.Base;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace AdvancedWorldGen.BetterVanillaWorldGen.DungeonStuff
{
	public static partial class Dungeon
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
		public static int DungeonMinX;
		public static int DungeonMaxX;
		public static int DungeonMinY;
		public static int DungeonMaxY;
		public static ushort CrackedType;

		public static bool IsDungeon(int x, int y)
		{
			if (y < Main.worldSurface)
				return false;

			if (x < 0 || x > Main.maxTilesX)
				return false;

			return Main.wallDungeon[Main.tile[x, y].wall];
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

		public static void MakeDungeon(int dungeonX, int dungeonY)
		{
			DungeonInit();

			ushort tileType;
			ushort wallType;
			switch (WorldGen.genRand.Next(3))
			{
				case 0:
					tileType = TileID.BlueDungeonBrick;
					wallType = WallID.BlueDungeonUnsafe;
					CrackedType = TileID.CrackedBlueDungeonBrick;
					break;
				case 1:
					tileType = TileID.GreenDungeonBrick;
					wallType = WallID.GreenDungeonUnsafe;
					CrackedType = TileID.CrackedGreenDungeonBrick;
					break;
				default:
					tileType = TileID.PinkDungeonBrick;
					wallType = WallID.PinkDungeonUnsafe;
					CrackedType = TileID.CrackedPinkDungeonBrick;
					break;
			}

			Main.tileSolid[CrackedType] = false;
			Replacer.VanillaInterface.CrackedType.Set(CrackedType);
			WorldGen.dungeonLake = true;
			WorldGen.dungeonX = dungeonX;
			WorldGen.dungeonY = dungeonY;
			DungeonMinX = dungeonX;
			DungeonMaxX = dungeonX;
			DungeonMinY = dungeonY;
			DungeonMaxY = dungeonY;
			DungeonXStrength1 = WorldGen.genRand.Next(25, 30);
			DungeonYStrength1 = WorldGen.genRand.Next(20, 25);
			DungeonXStrength2 = WorldGen.genRand.Next(35, 50);
			DungeonYStrength2 = WorldGen.genRand.Next(10, 15);
			int maxRooms = Main.maxTilesX / 60;
			maxRooms += WorldGen.genRand.Next(maxRooms / 3);
			int num6 = 5;
			DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
			for (int room = 0; room < maxRooms; room++)
			{
				if (WorldGen.dungeonX < DungeonMinX) DungeonMinX = WorldGen.dungeonX;

				if (WorldGen.dungeonX > DungeonMaxX) DungeonMaxX = WorldGen.dungeonX;

				if (WorldGen.dungeonY > DungeonMaxY) DungeonMaxY = WorldGen.dungeonY;

				if ((--num6 == 0) && (WorldGen.genRand.Next(3) == 0))
				{
					num6 = 5;
					if (WorldGen.genRand.Next(2) == 0)
					{
						int num7 = WorldGen.dungeonX;
						int num8 = WorldGen.dungeonY;
						DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
						if (WorldGen.genRand.Next(2) == 0)
							DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);

						DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
						WorldGen.dungeonX = num7;
						WorldGen.dungeonY = num8;
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
			for (int index = 0; index < DungeonRoomPos.Count; index++)
				if (DungeonRoomPos[index].y < dY)
				{
					dX = DungeonRoomPos[index].x;
					dY = DungeonRoomPos[index].y;
				}

			WorldGen.dungeonX = dX;
			WorldGen.dungeonY = dY;
			DungeonEntranceX = dX;
			DungeonSurface = false;
			num6 = 5;
			if (WorldGen.drunkWorldGen) DungeonSurface = true;

			while (!DungeonSurface)
			{
				num6--;

				if (num6 == 0 && WorldGen.genRand.Next(5) == 0 && WorldGen.dungeonY > Main.worldSurface + 100.0)
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
			Main.statusText = Language.GetTextValue("LegacyWorldGen.58") + " 65%";
			int num13 = Main.maxTilesX * 2;
			for (int num14 = 0; num14 < num13; num14++)
			{
				int x1 = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int y1 = WorldGen.genRand.Next((int) Math.Max(DungeonMinY, Main.worldSurface), DungeonMaxY);
				num14 = !DungeonPitTrap(x1, y1, tileType, wallType) ? num14 + 1 : num14 + 1500;
			}

			for (int k = 0; k < DungeonRoomPos.Count; k++)
			{
				for (int l = DungeonRoomL[k]; l <= DungeonRoomR[k]; l++)
					if (!Main.tile[l, DungeonRoomT[k] - 1].IsActive)
					{
						DungeonPlatforms.Add((l, DungeonRoomT[k] - 1));
						break;
					}

				for (int m = DungeonRoomL[k]; m <= DungeonRoomR[k]; m++)
					if (!Main.tile[m, DungeonRoomB[k] + 1].IsActive)
					{
						DungeonPlatforms.Add((m, DungeonRoomT[k] + 1));
						break;
					}

				for (int n = DungeonRoomT[k]; n <= DungeonRoomB[k]; n++)
					if (!Main.tile[DungeonRoomL[k] - 1, n].IsActive)
					{
						Doors.Add((DungeonRoomL[k] - 1, n, -1));
						break;
					}

				for (int num16 = DungeonRoomT[k]; num16 <= DungeonRoomB[k]; num16++)
					if (!Main.tile[DungeonRoomR[k] + 1, num16].IsActive)
					{
						Doors.Add((DungeonRoomL[k] + 1, num16, 1));
						break;
					}
			}

			Main.statusText = Language.GetTextValue("LegacyWorldGen.58") + " 70%";
			int num17 = 0;
			int num18 = 1000;
			int num19 = 0;
			int num20 = Main.maxTilesX / 100;
			if (WorldGen.getGoodWorldGen)
				num20 *= 3;

			while (num19 < num20)
			{
				num17++;
				int x0 = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int y0;
				if (WorldGen.drunkWorldGen && WorldGen.dungeonY + 25 >= DungeonMaxY)
					y0 = DungeonMaxY;
				else
					y0 = WorldGen.genRand.Next((int) Main.worldSurface + 25, DungeonMaxY);

				int num23 = x0;
				if (Main.tile[x0, y0].wall == wallType && !Main.tile[x0, y0].IsActive)
				{
					int num24 = 1;
					if (WorldGen.genRand.Next(2) == 0)
						num24 = -1;

					for (; !Main.tile[x0, y0].IsActive; y0 += num24)
					{
					}

					if (Main.tile[x0 - 1, y0].IsActive && Main.tile[x0 + 1, y0].IsActive &&
					    Main.tile[x0 - 1, y0].type != CrackedType &&
					    !Main.tile[x0 - 1, y0 - num24].IsActive && !Main.tile[x0 + 1, y0 - num24].IsActive)
					{
						num19++;
						int num25 = WorldGen.genRand.Next(5, 13);
						while (Main.tile[x0 - 1, y0].IsActive &&
						       Main.tile[x0 - 1, y0].type != CrackedType &&
						       Main.tile[x0, y0 + num24].IsActive && Main.tile[x0, y0].IsActive &&
						       !Main.tile[x0, y0 - num24].IsActive && num25 > 0)
						{
							Main.tile[x0, y0].type = TileID.Spikes;
							if (!Main.tile[x0 - 1, y0 - num24].IsActive &&
							    !Main.tile[x0 + 1, y0 - num24].IsActive)
							{
								Main.tile[x0, y0 - num24].Clear(TileDataType.Slope);
								Main.tile[x0, y0 - num24].type = TileID.Spikes;
								Main.tile[x0, y0 - num24].IsActive = true;
								Main.tile[x0, y0 - num24 * 2].Clear(TileDataType.Slope);
								Main.tile[x0, y0 - num24 * 2].type = TileID.Spikes;
								Main.tile[x0, y0 - num24 * 2].IsActive = true;
							}

							x0--;
							num25--;
						}

						num25 = WorldGen.genRand.Next(5, 13);
						x0 = num23 + 1;
						while (Main.tile[x0 + 1, y0].IsActive &&
						       Main.tile[x0 + 1, y0].type != CrackedType &&
						       Main.tile[x0, y0 + num24].IsActive && Main.tile[x0, y0].IsActive &&
						       !Main.tile[x0, y0 - num24].IsActive && num25 > 0)
						{
							Main.tile[x0, y0].type = TileID.Spikes;
							if (!Main.tile[x0 - 1, y0 - num24].IsActive &&
							    !Main.tile[x0 + 1, y0 - num24].IsActive)
							{
								Main.tile[x0, y0 - num24].Clear(TileDataType.Slope);
								Main.tile[x0, y0 - num24].type = TileID.Spikes;
								Main.tile[x0, y0 - num24].IsActive = true;
								Main.tile[x0, y0 - num24 * 2].Clear(TileDataType.Slope);
								Main.tile[x0, y0 - num24 * 2].type = TileID.Spikes;
								Main.tile[x0, y0 - num24 * 2].IsActive = true;
							}

							x0++;
							num25--;
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
			Main.statusText = Language.GetTextValue("LegacyWorldGen.58") + " 75%";
			while (num19 < num20)
			{
				num17++;
				int x = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int y = WorldGen.genRand.Next((int) Main.worldSurface + 25, DungeonMaxY);
				int num28 = y;
				if (Main.tile[x, y].wall == wallType && !Main.tile[x, y].IsActive)
				{
					int num29 = 1;
					if (WorldGen.genRand.Next(2) == 0)
						num29 = -1;

					for (; x > 5 && x < Main.maxTilesX - 5 && !Main.tile[x, y].IsActive; x += num29)
					{
					}

					if (Main.tile[x, y - 1].IsActive && Main.tile[x, y + 1].IsActive &&
					    Main.tile[x, y - 1].type != CrackedType &&
					    !Main.tile[x - num29, y - 1].IsActive && !Main.tile[x - num29, y + 1].IsActive)
					{
						num19++;
						int num30 = WorldGen.genRand.Next(5, 13);
						while (Main.tile[x, y - 1].IsActive &&
						       Main.tile[x, y - 1].type != CrackedType &&
						       Main.tile[x + num29, y].IsActive && Main.tile[x, y].IsActive &&
						       !Main.tile[x - num29, y].IsActive && num30 > 0)
						{
							Main.tile[x, y].type = TileID.Spikes;
							if (!Main.tile[x - num29, y - 1].IsActive &&
							    !Main.tile[x - num29, y + 1].IsActive)
							{
								Main.tile[x - num29, y].type = TileID.Spikes;
								Main.tile[x - num29, y].IsActive = true;
								Main.tile[x - num29, y].Clear(TileDataType.Slope);
								Main.tile[x - num29 * 2, y].type = TileID.Spikes;
								Main.tile[x - num29 * 2, y].IsActive = true;
								Main.tile[x - num29 * 2, y].Clear(TileDataType.Slope);
							}

							y--;
							num30--;
						}

						num30 = WorldGen.genRand.Next(5, 13);
						y = num28 + 1;
						while (Main.tile[x, y + 1].IsActive &&
						       Main.tile[x, y + 1].type != CrackedType &&
						       Main.tile[x + num29, y].IsActive && Main.tile[x, y].IsActive &&
						       !Main.tile[x - num29, y].IsActive && num30 > 0)
						{
							Main.tile[x, y].type = TileID.Spikes;
							if (!Main.tile[x - num29, y - 1].IsActive &&
							    !Main.tile[x - num29, y + 1].IsActive)
							{
								Main.tile[x - num29, y].type = TileID.Spikes;
								Main.tile[x - num29, y].IsActive = true;
								Main.tile[x - num29, y].Clear(TileDataType.Slope);
								Main.tile[x - num29 * 2, y].type = TileID.Spikes;
								Main.tile[x - num29 * 2, y].IsActive = true;
								Main.tile[x - num29 * 2, y].Clear(TileDataType.Slope);
							}

							y++;
							num30--;
						}
					}
				}

				if (num17 > num18)
				{
					num17 = 0;
					num19++;
				}
			}

			Main.statusText = Language.GetTextValue("LegacyWorldGen.58") + " 80%";
			foreach ((int doorX, int doorY, int pos) in Doors)
			{
				int num34 = 100;
				int num35 = 0;
				for (int x = doorX - 10; x < doorX + 10; x++)
				{
					bool flag = true;
					int y = doorY;
					while (y > 10 && !Main.tile[x, y].IsActive) y--;

					if (!Main.tileDungeon[Main.tile[x, y].type])
						flag = false;

					int oldY = y;
					for (y = doorY; !Main.tile[x, y].IsActive; y++)
					{
					}

					if (!Main.tileDungeon[Main.tile[x, y].type])
						flag = false;

					if (y - oldY < 3)
						continue;
					
					for (int xx = x - 20; xx < x + 20; xx++)
					for (int yy = y - 10; yy < y + 10; yy++)
						if (Main.tile[xx, yy].IsActive && Main.tile[xx, yy].type == TileID.ClosedDoor)
						{
							flag = false;
							break;
						}

					if (flag)
						for (int yy = y - 3; yy < y; yy++)
						for (int xx = x - 3; xx <= x + 3; xx++)
							if (Main.tile[xx, yy].IsActive)
							{
								flag = false;
								break;
							}

					if (flag && y - oldY < 20)
						if (pos == 0 && y - oldY < num34 || pos == -1 && x > num35 ||
						    pos == 1 && (x < num35 || num35 == 0))
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
				for (; !Main.tile[x0, y0].IsActive; y0++) Main.tile[x0, y0].IsActive = false;

				while (!Main.tile[x0, num50].IsActive) num50--;

				y0--;
				num50++;
				for (int num51 = num50; num51 < y0 - 2; num51++)
				{
					Main.tile[x0, num51].Clear(TileDataType.Slope);
					Main.tile[x0, num51].IsActive = true;
					Main.tile[x0, num51].type = tileType;
				}

				int style = 13;
				if (WorldGen.genRand.Next(3) == 0)
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
				while (!Main.tile[x0, num52].IsActive) num52--;

				if (y0 - num52 < y0 - num50 + 5 && Main.tileDungeon[Main.tile[x0, num52].type])
					for (int num53 = y0 - 4 - WorldGen.genRand.Next(3); num53 > num52; num53--)
					{
						Main.tile[x0, num53].Clear(TileDataType.Slope);
						Main.tile[x0, num53].IsActive = true;
						Main.tile[x0, num53].type = tileType;
					}

				x0 += 2;
				num52 = y0 - 3;
				while (!Main.tile[x0, num52].IsActive) num52--;

				if (y0 - num52 < y0 - num50 + 5 && Main.tileDungeon[Main.tile[x0, num52].type])
					for (int num54 = y0 - 4 - WorldGen.genRand.Next(3); num54 > num52; num54--)
					{
						Main.tile[x0, num54].IsActive = true;
						Main.tile[x0, num54].Clear(TileDataType.Slope);
						Main.tile[x0, num54].type = tileType;
					}

				y0++;
				x0--;
				Main.tile[x0 - 1, y0].IsActive = true;
				Main.tile[x0 - 1, y0].type = tileType;
				Main.tile[x0 - 1, y0].Clear(TileDataType.Slope);
				Main.tile[x0 + 1, y0].IsActive = true;
				Main.tile[x0 + 1, y0].type = tileType;
				Main.tile[x0 + 1, y0].Clear(TileDataType.Slope);
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
				int randX = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int randY = WorldGen.genRand.Next(DungeonMinY, DungeonMaxY);
				for (int x = randX - range; x < randX + range; x++)
				for (int y = randY - range; y < randY + range; y++)
					if (y > Main.worldSurface)
					{
						float distX = Math.Abs(randX - x);
						float distY = Math.Abs(randY - y);
						if (Math.Sqrt(distX * distX + distY * distY) < range * 0.4 && Main.wallDungeon[Main.tile[x, y].wall])
							WorldGen.Spread.WallDungeon(x, y, wallTypes[i]);
					}
			}

			Main.statusText = Language.GetTextValue("LegacyWorldGen.58") + " 85%";
			foreach ((int platformX, int platformY) in DungeonPlatforms)
			{
				int y = Main.maxTilesX; // TODO: Is this supposed to be 'Main.maxTilesY'?
				int offX = 10;
				if (platformY < Main.worldSurface + 50.0)
					offX = 20;

				for (int y1 = platformY - 5; y1 <= platformY + 5; y1++)
				{
					int x1 = platformX;
					int x2 = platformX;
					if (!Main.tile[x1, y1].IsActive)
					{
						bool flag3 = false;
						while (!Main.tile[x1, y1].IsActive)
						{
							x1--;
							if (!Main.tileDungeon[Main.tile[x1, y1].type] || x1 == 0)
							{
								flag3 = true;
								break;
							}
						}
						if (flag3)
							continue;

						while (!Main.tile[x2, y1].IsActive)
						{
							x2++;
							if (!Main.tileDungeon[Main.tile[x2, y1].type] || x2 == Main.maxTilesX - 1)
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
							if (Main.tile[xx, yy].IsActive && Main.tile[xx, yy].type == TileID.Platforms)
							{
								flag4 = false;
								break;
							}

						if (!flag4)
							break;
					}

					for (int yy = y1 + 3; yy >= y1 - 5; yy--)
						if (Main.tile[platformX, yy].IsActive)
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
				while (!Main.tile[x, y].IsActive)
				{
					Main.tile[x, y].IsActive = true;
					Main.tile[x, y].type = TileID.Platforms;
					Main.tile[x, y].Clear(TileDataType.Slope);
					Main.tile[x, y].frameY = wallType switch
					{
						WallID.BlueDungeonUnsafe => 108,
						WallID.GreenDungeonUnsafe => 144,
						_ => 126
					};

					WorldGen.TileFrame(x, y);
					x--;
				}

				x = platformX + 1;
				// Place to the right
				for (; !Main.tile[x, y].IsActive; x++)
				{
					Main.tile[x, y].IsActive = true;
					Main.tile[x, y].type = TileID.Platforms;
					Main.tile[x, y].Clear(TileDataType.Slope);
					Main.tile[x, y].frameY = wallType switch
					{
						WallID.BlueDungeonUnsafe => 108,
						WallID.GreenDungeonUnsafe => 144,
						_ => 126
					};

					WorldGen.TileFrame(x, y);
				}
			}

			int evilChests = 5;
			if (WorldGen.drunkWorldGen || ModifiedWorld.OptionsContains("Crimruption"))
				evilChests = 6;

			for (int numChest = 0; numChest < evilChests; numChest++)
			{
				bool chestPlaced = false;
				while (!chestPlaced)
				{
					int randX = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
					int randY = WorldGen.genRand.Next((int) Main.worldSurface, DungeonMaxY);
					if (!Main.wallDungeon[Main.tile[randX, randY].wall] || Main.tile[randX, randY].IsActive)
						continue;

					ushort chestTileType = 21;
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

					chestPlaced = Chest.AddBuriedChest(randX, randY, contain, false, style2, chestTileType);
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

			Main.statusText = Language.GetTextValue("LegacyWorldGen.58") + " 90%";
			num17 = 0;
			num18 = 1000;
			num19 = 0;
			while (num19 < Main.maxTilesX / 20)
			{
				num17++;
				int x = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int y = WorldGen.genRand.Next(DungeonMinY, DungeonMaxY);
				bool flag6 = true;
				if (Main.wallDungeon[Main.tile[x, y].wall] && !Main.tile[x, y].IsActive)
				{
					int direction = 1;
					if (WorldGen.genRand.Next(2) == 0)
						direction = -1;

					while (flag6 && !Main.tile[x, y].IsActive)
					{
						x -= direction;
						if (x < 5 || x > Main.maxTilesX - 5)
							flag6 = false;
						else if (Main.tile[x, y].IsActive && !Main.tileDungeon[Main.tile[x, y].type])
							flag6 = false;
					}

					if (flag6 &&
					    Main.tile[x, y].IsActive && Main.tileDungeon[Main.tile[x, y].type] &&
					    Main.tile[x, y - 1].IsActive && Main.tileDungeon[Main.tile[x, y - 1].type] &&
					    Main.tile[x, y + 1].IsActive && Main.tileDungeon[Main.tile[x, y + 1].type])
					{
						x += direction;
						for (int xx = x - 3; xx <= x + 3; xx++)
						for (int yy = y - 3; yy <= y + 3; yy++)
							if (Main.tile[xx, yy].IsActive && Main.tile[xx, yy].type == TileID.Platforms)
							{
								flag6 = false;
								break;
							}

						if (flag6 && !Main.tile[x, y - 1].IsActive && !Main.tile[x, y - 2].IsActive && !Main.tile[x, y - 3].IsActive)
						{
							int distX = x;
							int oldX = x;
							for (;
								distX > DungeonMinX && distX < DungeonMaxX &&
								!Main.tile[distX, y].IsActive &&
								!Main.tile[distX, y - 1].IsActive &&
								!Main.tile[distX, y + 1].IsActive;
								distX += direction)
							{
							}

							distX = Math.Abs(x - distX);
							bool books = WorldGen.genRand.Next(2) == 0;

							if (distX > 5)
							{
								for (int _ = WorldGen.genRand.Next(1, 4); _ > 0; _--)
								{
									Main.tile[x, y].IsActive = true;
									Main.tile[x, y].Clear(TileDataType.Slope);
									Main.tile[x, y].type = TileID.Platforms;
									if (Main.tile[x, y].wall == wallTypes[0])
										Main.tile[x, y].frameY = (short) (18 * frameMult[0]);
									else if (Main.tile[x, y].wall == wallTypes[1])
										Main.tile[x, y].frameY = (short) (18 * frameMult[1]);
									else
										Main.tile[x, y].frameY = (short) (18 * frameMult[2]);

									WorldGen.TileFrame(x, y);
									if (books)
									{
										WorldGen.PlaceTile(x, y - 1, TileID.Books, true);
										if (WorldGen.genRand.Next(50) == 0 &&
										    y > (Main.worldSurface + Main.rockLayer) / 2.0 &&
										    Main.tile[x, y - 1].type == TileID.Books)
											Main.tile[x, y - 1].frameX = 90;
									}

									x += direction;
								}

								num17 = 0;
								num19++;
								if (!books && WorldGen.genRand.Next(2) == 0)
								{
									x = oldX;
									y--;
									int type = 0;
									if (WorldGen.genRand.Next(4) == 0)
										type = 1;

									type = type switch
									{
										0 => TileID.Bottles,
										1 => TileID.WaterCandle,
										_ => type // Impossible
									};

									WorldGen.PlaceTile(x, y, type, true);
									if (Main.tile[x, y].type == TileID.Bottles)
									{
										if (WorldGen.genRand.Next(2) == 0)
											Main.tile[x, y].frameX = 18;
										else
											Main.tile[x, y].frameX = 36;
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

			Main.statusText = Language.GetTextValue("LegacyWorldGen.58") + " 95%";
			int num95 = 1;
			for (int index = 0; index < DungeonRoomPos.Count; index++)
			{
				int tries = 0;
				while (tries < 1000)
				{
					int offset = (int) (DungeonRoomSize[index] * 0.4);
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

					if (Chest.AddBuriedChest(x, y, itemType, false, style3))
					{
						tries += 1000;
						num95++;
					}

					tries++;
				}
			}

			DungeonMinX -= 25;
			DungeonMaxX += 25;
			DungeonMinY -= 25;
			DungeonMaxY += 25;
			if (DungeonMinX < 0) DungeonMinX = 0;

			if (DungeonMaxX > Main.maxTilesX) DungeonMaxX = Main.maxTilesX;

			if (DungeonMinY < 0) DungeonMinY = 0;

			if (DungeonMaxY > Main.maxTilesY) DungeonMaxY = Main.maxTilesY;

			MakeDungeon_Lights(tileType, wallTypes);
			MakeDungeon_Traps();
			MakeDungeon_GroundFurniture(wallType);
			MakeDungeon_Pictures(wallTypes);
			MakeDungeon_Banners(wallTypes);
		}
	}
}