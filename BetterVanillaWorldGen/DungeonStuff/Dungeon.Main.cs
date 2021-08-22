using System;
using System.Collections.Generic;
using AdvancedWorldGen.Base;
using Terraria;
using Terraria.DataStructures;

namespace AdvancedWorldGen.BetterVanillaWorldGen.DungeonStuff
{
	public partial class Dungeon
	{
		public static List<(int x, int y)> DungeonRoomPos;
		public static List<int> DungeonRoomSize;
		public static List<int> DungeonRoomL;
		public static List<int> DungeonRoomR;
		public static List<int> DungeonRoomT;
		public static List<int> DungeonRoomB;

		public static List<(int x, int y, int pos)> Doors;

		public static List<(int x, int y)> DungeonPlatforms;

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

		public static void MakeDungeon(int x, int y)
		{
			DungeonInit();

			int num = WorldGen.genRand.Next(3);
			ushort num2;
			int num3;
			switch (num)
			{
				case 0:
					num2 = 41;
					num3 = 7;
					CrackedType = 481;
					break;
				case 1:
					num2 = 43;
					num3 = 8;
					CrackedType = 482;
					break;
				default:
					num2 = 44;
					num3 = 9;
					CrackedType = 483;
					break;
			}

			Main.tileSolid[CrackedType] = false;
			Replacer.VanillaInterface.CrackedType.Set(CrackedType);
			WorldGen.dungeonLake = true;
			WorldGen.dungeonX = x;
			WorldGen.dungeonY = y;
			DungeonMinX = x;
			DungeonMaxX = x;
			DungeonMinY = y;
			DungeonMaxY = y;
			DungeonXStrength1 = WorldGen.genRand.Next(25, 30);
			DungeonYStrength1 = WorldGen.genRand.Next(20, 25);
			DungeonXStrength2 = WorldGen.genRand.Next(35, 50);
			DungeonYStrength2 = WorldGen.genRand.Next(10, 15);
			int maxRooms = Main.maxTilesX / 60;
			maxRooms += WorldGen.genRand.Next(maxRooms / 3);
			int num6 = 5;
			Dungeon.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
			for (int room = 0; room < maxRooms; room++)
			{
				if (WorldGen.dungeonX < DungeonMinX) DungeonMinX = WorldGen.dungeonX;

				if (WorldGen.dungeonX > DungeonMaxX) DungeonMaxX = WorldGen.dungeonX;

				if (WorldGen.dungeonY > DungeonMaxY) DungeonMaxY = WorldGen.dungeonY;

				if (num6 > 0)
					num6--;

				if ((num6 == 0) & (WorldGen.genRand.Next(3) == 0))
				{
					num6 = 5;
					if (WorldGen.genRand.Next(2) == 0)
					{
						int num7 = WorldGen.dungeonX;
						int num8 = WorldGen.dungeonY;
						Dungeon.DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
						if (WorldGen.genRand.Next(2) == 0)
							Dungeon.DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);

						Dungeon.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
						WorldGen.dungeonX = num7;
						WorldGen.dungeonY = num8;
					}
					else
					{
						Dungeon.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
					}
				}
				else
				{
					Dungeon.DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
				}
			}

			Dungeon.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
			(int dungeonX, int dungeonY) = DungeonRoomPos[0];
			for (int i = 0; i < DungeonRoomPos.Count; i++)
				if (DungeonRoomPos[i].y < dungeonY)
				{
					dungeonX = DungeonRoomPos[i].x;
					dungeonY = DungeonRoomPos[i].y;
				}

			WorldGen.dungeonX = dungeonX;
			WorldGen.dungeonY = dungeonY;
			DungeonEntranceX = dungeonX;
			DungeonSurface = false;
			num6 = 5;
			if (WorldGen.drunkWorldGen) DungeonSurface = true;

			while (!DungeonSurface)
			{
				if (num6 > 0)
					num6--;

				if (num6 == 0 && WorldGen.genRand.Next(5) == 0 && WorldGen.dungeonY > Main.worldSurface + 100.0)
				{
					num6 = 10;
					int num11 = WorldGen.dungeonX;
					int num12 = WorldGen.dungeonY;
					Dungeon.DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3, true);
					Dungeon.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
					WorldGen.dungeonX = num11;
					WorldGen.dungeonY = num12;
				}

				Dungeon.DungeonStairs(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
			}

			Dungeon.DungeonEntrance(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
			Main.statusText = Lang.gen[58].Value + " 65%";
			int num13 = Main.maxTilesX * 2;
			int num14;
			for (num14 = 0; num14 < num13; num14++)
			{
				int i2 = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int num15 = DungeonMinY;
				if (num15 < Main.worldSurface)
					num15 = (int) Main.worldSurface;

				int j = WorldGen.genRand.Next(num15, DungeonMaxY);
				num14 = !Dungeon.DungeonPitTrap(i2, j, num2, num3) ? num14 + 1 : num14 + 1500;
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

			Main.statusText = Lang.gen[58].Value + " 70%";
			int num17 = 0;
			int num18 = 1000;
			int num19 = 0;
			int num20 = Main.maxTilesX / 100;
			if (WorldGen.getGoodWorldGen)
				num20 *= 3;

			while (num19 < num20)
			{
				num17++;
				int num21 = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int num22 = WorldGen.genRand.Next((int) Main.worldSurface + 25, DungeonMaxY);
				if (WorldGen.drunkWorldGen)
					num22 = WorldGen.genRand.Next(WorldGen.dungeonY + 25, DungeonMaxY);

				int num23 = num21;
				if (Main.tile[num21, num22].wall == num3 && !Main.tile[num21, num22].IsActive)
				{
					int num24 = 1;
					if (WorldGen.genRand.Next(2) == 0)
						num24 = -1;

					for (; !Main.tile[num21, num22].IsActive; num22 += num24)
					{
					}

					if (Main.tile[num21 - 1, num22].IsActive && Main.tile[num21 + 1, num22].IsActive &&
					    Main.tile[num21 - 1, num22].type != CrackedType &&
					    !Main.tile[num21 - 1, num22 - num24].IsActive && !Main.tile[num21 + 1, num22 - num24].IsActive)
					{
						num19++;
						int num25 = WorldGen.genRand.Next(5, 13);
						while (Main.tile[num21 - 1, num22].IsActive &&
						       Main.tile[num21 - 1, num22].type != CrackedType &&
						       Main.tile[num21, num22 + num24].IsActive && Main.tile[num21, num22].IsActive &&
						       !Main.tile[num21, num22 - num24].IsActive && num25 > 0)
						{
							Main.tile[num21, num22].type = 48;
							if (!Main.tile[num21 - 1, num22 - num24].IsActive &&
							    !Main.tile[num21 + 1, num22 - num24].IsActive)
							{
								Main.tile[num21, num22 - num24].Clear(TileDataType.Slope);
								Main.tile[num21, num22 - num24].type = 48;
								Main.tile[num21, num22 - num24].IsActive = true;
								Main.tile[num21, num22 - num24 * 2].Clear(TileDataType.Slope);
								Main.tile[num21, num22 - num24 * 2].type = 48;
								Main.tile[num21, num22 - num24 * 2].IsActive = true;
							}

							num21--;
							num25--;
						}

						num25 = WorldGen.genRand.Next(5, 13);
						num21 = num23 + 1;
						while (Main.tile[num21 + 1, num22].IsActive &&
						       Main.tile[num21 + 1, num22].type != CrackedType &&
						       Main.tile[num21, num22 + num24].IsActive && Main.tile[num21, num22].IsActive &&
						       !Main.tile[num21, num22 - num24].IsActive && num25 > 0)
						{
							Main.tile[num21, num22].type = 48;
							if (!Main.tile[num21 - 1, num22 - num24].IsActive &&
							    !Main.tile[num21 + 1, num22 - num24].IsActive)
							{
								Main.tile[num21, num22 - num24].Clear(TileDataType.Slope);
								Main.tile[num21, num22 - num24].type = 48;
								Main.tile[num21, num22 - num24].IsActive = true;
								Main.tile[num21, num22 - num24 * 2].Clear(TileDataType.Slope);
								Main.tile[num21, num22 - num24 * 2].type = 48;
								Main.tile[num21, num22 - num24 * 2].IsActive = true;
							}

							num21++;
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
			Main.statusText = Lang.gen[58].Value + " 75%";
			while (num19 < num20)
			{
				num17++;
				int num26 = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int num27 = WorldGen.genRand.Next((int) Main.worldSurface + 25, DungeonMaxY);
				int num28 = num27;
				if (Main.tile[num26, num27].wall == num3 && !Main.tile[num26, num27].IsActive)
				{
					int num29 = 1;
					if (WorldGen.genRand.Next(2) == 0)
						num29 = -1;

					for (; num26 > 5 && num26 < Main.maxTilesX - 5 && !Main.tile[num26, num27].IsActive; num26 += num29)
					{
					}

					if (Main.tile[num26, num27 - 1].IsActive && Main.tile[num26, num27 + 1].IsActive &&
					    Main.tile[num26, num27 - 1].type != CrackedType &&
					    !Main.tile[num26 - num29, num27 - 1].IsActive && !Main.tile[num26 - num29, num27 + 1].IsActive)
					{
						num19++;
						int num30 = WorldGen.genRand.Next(5, 13);
						while (Main.tile[num26, num27 - 1].IsActive &&
						       Main.tile[num26, num27 - 1].type != CrackedType &&
						       Main.tile[num26 + num29, num27].IsActive && Main.tile[num26, num27].IsActive &&
						       !Main.tile[num26 - num29, num27].IsActive && num30 > 0)
						{
							Main.tile[num26, num27].type = 48;
							if (!Main.tile[num26 - num29, num27 - 1].IsActive &&
							    !Main.tile[num26 - num29, num27 + 1].IsActive)
							{
								Main.tile[num26 - num29, num27].type = 48;
								Main.tile[num26 - num29, num27].IsActive = true;
								Main.tile[num26 - num29, num27].Clear(TileDataType.Slope);
								Main.tile[num26 - num29 * 2, num27].type = 48;
								Main.tile[num26 - num29 * 2, num27].IsActive = true;
								Main.tile[num26 - num29 * 2, num27].Clear(TileDataType.Slope);
							}

							num27--;
							num30--;
						}

						num30 = WorldGen.genRand.Next(5, 13);
						num27 = num28 + 1;
						while (Main.tile[num26, num27 + 1].IsActive &&
						       Main.tile[num26, num27 + 1].type != CrackedType &&
						       Main.tile[num26 + num29, num27].IsActive && Main.tile[num26, num27].IsActive &&
						       !Main.tile[num26 - num29, num27].IsActive && num30 > 0)
						{
							Main.tile[num26, num27].type = 48;
							if (!Main.tile[num26 - num29, num27 - 1].IsActive &&
							    !Main.tile[num26 - num29, num27 + 1].IsActive)
							{
								Main.tile[num26 - num29, num27].type = 48;
								Main.tile[num26 - num29, num27].IsActive = true;
								Main.tile[num26 - num29, num27].Clear(TileDataType.Slope);
								Main.tile[num26 - num29 * 2, num27].type = 48;
								Main.tile[num26 - num29 * 2, num27].IsActive = true;
								Main.tile[num26 - num29 * 2, num27].Clear(TileDataType.Slope);
							}

							num27++;
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

			Main.statusText = Lang.gen[58].Value + " 80%";
			foreach ((int i, int i1, int pos) in Doors)
			{
				int num32 = i - 10;
				int num33 = i + 10;
				int num34 = 100;
				int num35 = 0;
				int num36 = 0;
				int num37 = 0;
				for (int num38 = num32; num38 < num33; num38++)
				{
					bool flag = true;
					int num39 = i1;
					while (num39 > 10 && !Main.tile[num38, num39].IsActive) num39--;

					if (!Main.tileDungeon[Main.tile[num38, num39].type])
						flag = false;

					num36 = num39;
					for (num39 = i1; !Main.tile[num38, num39].IsActive; num39++)
					{
					}

					if (!Main.tileDungeon[Main.tile[num38, num39].type])
						flag = false;

					num37 = num39;
					if (num37 - num36 < 3)
						continue;

					int num40 = num38 - 20;
					int num41 = num38 + 20;
					int num42 = num37 - 10;
					int num43 = num37 + 10;
					for (int num44 = num40; num44 < num41; num44++)
					for (int num45 = num42; num45 < num43; num45++)
						if (Main.tile[num44, num45].IsActive && Main.tile[num44, num45].type == 10)
						{
							flag = false;
							break;
						}

					if (flag)
						for (int num46 = num37 - 3; num46 < num37; num46++)
						for (int num47 = num38 - 3; num47 <= num38 + 3; num47++)
							if (Main.tile[num47, num46].IsActive)
							{
								flag = false;
								break;
							}

					if (flag && num37 - num36 < 20)
						if (pos == 0 && num37 - num36 < num34 || pos == -1 && num38 > num35 ||
						    pos == 1 && (num38 < num35 || num35 == 0))
						{
							num35 = num38;
							num34 = num37 - num36;
						}
				}

				if (num34 >= 20)
					continue;

				int num48 = num35;
				int num49 = i1;
				int num50 = num49;
				for (; !Main.tile[num48, num49].IsActive; num49++) Main.tile[num48, num49].IsActive = false;

				while (!Main.tile[num48, num50].IsActive) num50--;

				num49--;
				num50++;
				for (int num51 = num50; num51 < num49 - 2; num51++)
				{
					Main.tile[num48, num51].Clear(TileDataType.Slope);
					Main.tile[num48, num51].IsActive = true;
					Main.tile[num48, num51].type = num2;
				}

				int style = 13;
				if (WorldGen.genRand.Next(3) == 0)
					switch (num3)
					{
						case 7:
							style = 16;
							break;
						case 8:
							style = 17;
							break;
						case 9:
							style = 18;
							break;
					}

				WorldGen.PlaceTile(num48, num49, 10, true, false, -1, style);
				num48--;
				int num52 = num49 - 3;
				while (!Main.tile[num48, num52].IsActive) num52--;

				if (num49 - num52 < num49 - num50 + 5 && Main.tileDungeon[Main.tile[num48, num52].type])
					for (int num53 = num49 - 4 - WorldGen.genRand.Next(3); num53 > num52; num53--)
					{
						Main.tile[num48, num53].Clear(TileDataType.Slope);
						Main.tile[num48, num53].IsActive = true;
						Main.tile[num48, num53].type = num2;
					}

				num48 += 2;
				num52 = num49 - 3;
				while (!Main.tile[num48, num52].IsActive) num52--;

				if (num49 - num52 < num49 - num50 + 5 && Main.tileDungeon[Main.tile[num48, num52].type])
					for (int num54 = num49 - 4 - WorldGen.genRand.Next(3); num54 > num52; num54--)
					{
						Main.tile[num48, num54].IsActive = true;
						Main.tile[num48, num54].Clear(TileDataType.Slope);
						Main.tile[num48, num54].type = num2;
					}

				num49++;
				num48--;
				Main.tile[num48 - 1, num49].IsActive = true;
				Main.tile[num48 - 1, num49].type = num2;
				Main.tile[num48 - 1, num49].Clear(TileDataType.Slope);
				Main.tile[num48 + 1, num49].IsActive = true;
				Main.tile[num48 + 1, num49].type = num2;
				Main.tile[num48 + 1, num49].Clear(TileDataType.Slope);
			}

			int[] array = new int[3];
			switch (num3)
			{
				case 7:
					array[0] = 7;
					array[1] = 94;
					array[2] = 95;
					break;
				case 9:
					array[0] = 9;
					array[1] = 96;
					array[2] = 97;
					break;
				default:
					array[0] = 8;
					array[1] = 98;
					array[2] = 99;
					break;
			}

			for (int num55 = 0; num55 < 5; num55++)
			for (int num56 = 0; num56 < 3; num56++)
			{
				int num57 = WorldGen.genRand.Next(40, 240);
				int num58 = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int num59 = WorldGen.genRand.Next(DungeonMinY, DungeonMaxY);
				for (int num60 = num58 - num57; num60 < num58 + num57; num60++)
				for (int num61 = num59 - num57; num61 < num59 + num57; num61++)
					if (num61 > Main.worldSurface)
					{
						float num62 = Math.Abs(num58 - num60);
						float num63 = Math.Abs(num59 - num61);
						if (Math.Sqrt(num62 * num62 + num63 * num63) < num57 * 0.4 &&
						    Main.wallDungeon[Main.tile[num60, num61].wall])
							WorldGen.Spread.WallDungeon(num60, num61, array[num56]);
					}
			}

			Main.statusText = Lang.gen[58].Value + " 85%";
			foreach ((int num65, int num66) in DungeonPlatforms)
			{
				int num67 = Main.maxTilesX;
				int num68 = 10;
				if (num66 < Main.worldSurface + 50.0)
					num68 = 20;

				for (int num69 = num66 - 5; num69 <= num66 + 5; num69++)
				{
					int num70 = num65;
					int num71 = num65;
					bool flag3 = false;
					if (Main.tile[num70, num69].IsActive)
					{
						flag3 = true;
					}
					else
					{
						while (!Main.tile[num70, num69].IsActive)
						{
							num70--;
							if (!Main.tileDungeon[Main.tile[num70, num69].type] || num70 == 0)
							{
								flag3 = true;
								break;
							}
						}

						while (!Main.tile[num71, num69].IsActive)
						{
							num71++;
							if (!Main.tileDungeon[Main.tile[num71, num69].type] || num71 == Main.maxTilesX - 1)
							{
								flag3 = true;
								break;
							}
						}
					}

					if (flag3 || num71 - num70 > num68)
						continue;

					bool flag4 = true;
					int num72 = num65 - num68 / 2 - 2;
					int num73 = num65 + num68 / 2 + 2;
					int num74 = num69 - 5;
					int num75 = num69 + 5;
					for (int num76 = num72; num76 <= num73; num76++)
					for (int num77 = num74; num77 <= num75; num77++)
						if (Main.tile[num76, num77].IsActive && Main.tile[num76, num77].type == 19)
						{
							flag4 = false;
							break;
						}

					for (int num78 = num69 + 3; num78 >= num69 - 5; num78--)
						if (Main.tile[num65, num78].IsActive)
						{
							flag4 = false;
							break;
						}

					if (flag4)
					{
						num67 = num69;
						break;
					}
				}

				if (num67 <= num66 - 10 || num67 >= num66 + 10)
					continue;

				int num79 = num65;
				int num80 = num67;
				int num81 = num65 + 1;
				while (!Main.tile[num79, num80].IsActive)
				{
					Main.tile[num79, num80].IsActive = true;
					Main.tile[num79, num80].type = 19;
					Main.tile[num79, num80].Clear(TileDataType.Slope);
					switch (num3)
					{
						case 7:
							Main.tile[num79, num80].frameY = 108;
							break;
						case 8:
							Main.tile[num79, num80].frameY = 144;
							break;
						default:
							Main.tile[num79, num80].frameY = 126;
							break;
					}

					WorldGen.TileFrame(num79, num80);
					num79--;
				}

				for (; !Main.tile[num81, num80].IsActive; num81++)
				{
					Main.tile[num81, num80].IsActive = true;
					Main.tile[num81, num80].type = 19;
					Main.tile[num81, num80].Clear(TileDataType.Slope);
					switch (num3)
					{
						case 7:
							Main.tile[num81, num80].frameY = 108;
							break;
						case 8:
							Main.tile[num81, num80].frameY = 144;
							break;
						default:
							Main.tile[num81, num80].frameY = 126;
							break;
					}

					WorldGen.TileFrame(num81, num80);
				}
			}

			int evilChests = 5;
			if (WorldGen.drunkWorldGen || ModifiedWorld.OptionsContains("Crimruption"))
				evilChests = 6;

			for (int numChest = 0; numChest < evilChests; numChest++)
			{
				bool flag5 = false;
				while (!flag5)
				{
					int num84 = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
					int num85 = WorldGen.genRand.Next((int) Main.worldSurface, DungeonMaxY);
					if (!Main.wallDungeon[Main.tile[num84, num85].wall] || Main.tile[num84, num85].IsActive)
						continue;

					ushort chestTileType = 21;
					int contain = 0;
					int style2 = 0;
					switch (numChest)
					{
						case 0:
							style2 = 23;
							contain = 1156;
							break;
						case 1:
							if (!WorldGen.crimson)
							{
								style2 = 24;
								contain = 1571;
							}
							else
							{
								style2 = 25;
								contain = 1569;
							}

							break;
						case 5:
							if (WorldGen.crimson)
							{
								style2 = 24;
								contain = 1571;
							}
							else
							{
								style2 = 25;
								contain = 1569;
							}

							break;
						case 2:
							style2 = 26;
							contain = 1260;
							break;
						case 3:
							style2 = 27;
							contain = 1572;
							break;
						case 4:
							chestTileType = 467;
							style2 = 13;
							contain = 4607;
							break;
					}

					flag5 = WorldGen.AddBuriedChest(num84, num85, contain, false, style2, false, chestTileType);
				}
			}

			int[] array2 = new int[3]
			{
				WorldGen.genRand.Next(9, 13), WorldGen.genRand.Next(9, 13),
				0
			};

			while (array2[1] == array2[0]) array2[1] = WorldGen.genRand.Next(9, 13);

			array2[2] = WorldGen.genRand.Next(9, 13);
			while (array2[2] == array2[0] || array2[2] == array2[1]) array2[2] = WorldGen.genRand.Next(9, 13);

			Main.statusText = Lang.gen[58].Value + " 90%";
			num17 = 0;
			num18 = 1000;
			num19 = 0;
			while (num19 < Main.maxTilesX / 20)
			{
				num17++;
				int num86 = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int num87 = WorldGen.genRand.Next(DungeonMinY, DungeonMaxY);
				bool flag6 = true;
				if (Main.wallDungeon[Main.tile[num86, num87].wall] && !Main.tile[num86, num87].IsActive)
				{
					int num88 = 1;
					if (WorldGen.genRand.Next(2) == 0)
						num88 = -1;

					while (flag6 && !Main.tile[num86, num87].IsActive)
					{
						num86 -= num88;
						if (num86 < 5 || num86 > Main.maxTilesX - 5)
							flag6 = false;
						else if (Main.tile[num86, num87].IsActive && !Main.tileDungeon[Main.tile[num86, num87].type])
							flag6 = false;
					}

					if (flag6 && Main.tile[num86, num87].IsActive && Main.tileDungeon[Main.tile[num86, num87].type] &&
					    Main.tile[num86, num87 - 1].IsActive && Main.tileDungeon[Main.tile[num86, num87 - 1].type] &&
					    Main.tile[num86, num87 + 1].IsActive && Main.tileDungeon[Main.tile[num86, num87 + 1].type])
					{
						num86 += num88;
						for (int num89 = num86 - 3; num89 <= num86 + 3; num89++)
						for (int num90 = num87 - 3; num90 <= num87 + 3; num90++)
							if (Main.tile[num89, num90].IsActive && Main.tile[num89, num90].type == 19)
							{
								flag6 = false;
								break;
							}

						if (flag6 && !Main.tile[num86, num87 - 1].IsActive & !Main.tile[num86, num87 - 2].IsActive &
							!Main.tile[num86, num87 - 3].IsActive)
						{
							int num91 = num86;
							int num92 = num86;
							for (;
								num91 > DungeonMinX && num91 < DungeonMaxX && !Main.tile[num91, num87].IsActive &&
								!Main.tile[num91, num87 - 1].IsActive && !Main.tile[num91, num87 + 1].IsActive;
								num91 += num88)
							{
							}

							num91 = Math.Abs(num86 - num91);
							bool flag7 = WorldGen.genRand.Next(2) == 0;

							if (num91 > 5)
							{
								for (int num93 = WorldGen.genRand.Next(1, 4); num93 > 0; num93--)
								{
									Main.tile[num86, num87].IsActive = true;
									Main.tile[num86, num87].Clear(TileDataType.Slope);
									Main.tile[num86, num87].type = 19;
									if (Main.tile[num86, num87].wall == array[0])
										Main.tile[num86, num87].frameY = (short) (18 * array2[0]);
									else if (Main.tile[num86, num87].wall == array[1])
										Main.tile[num86, num87].frameY = (short) (18 * array2[1]);
									else
										Main.tile[num86, num87].frameY = (short) (18 * array2[2]);

									WorldGen.TileFrame(num86, num87);
									if (flag7)
									{
										WorldGen.PlaceTile(num86, num87 - 1, 50, true);
										if (WorldGen.genRand.Next(50) == 0 &&
										    num87 > (Main.worldSurface + Main.rockLayer) / 2.0 &&
										    Main.tile[num86, num87 - 1].type == 50)
											Main.tile[num86, num87 - 1].frameX = 90;
									}

									num86 += num88;
								}

								num17 = 0;
								num19++;
								if (!flag7 && WorldGen.genRand.Next(2) == 0)
								{
									num86 = num92;
									num87--;
									int num94 = 0;
									if (WorldGen.genRand.Next(4) == 0)
										num94 = 1;

									switch (num94)
									{
										case 0:
											num94 = 13;
											break;
										case 1:
											num94 = 49;
											break;
									}

									WorldGen.PlaceTile(num86, num87, num94, true);
									if (Main.tile[num86, num87].type == 13)
									{
										if (WorldGen.genRand.Next(2) == 0)
											Main.tile[num86, num87].frameX = 18;
										else
											Main.tile[num86, num87].frameX = 36;
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

			Main.statusText = Lang.gen[58].Value + " 95%";
			int num95 = 1;
			for (int num96 = 0; num96 < DungeonRoomPos.Count; num96++)
			{
				int num97 = 0;
				while (num97 < 1000)
				{
					int num98 = (int) (DungeonRoomSize[num96] * 0.4);
					(int i3, int num99) = DungeonRoomPos[num96];
					i3 += WorldGen.genRand.Next(-num98, num98 + 1);
					num99 += WorldGen.genRand.Next(-num98, num98 + 1);
					int num100 = 0;
					int style3 = 2;
					if (num95 == 1)
						num95++;

					switch (num95)
					{
						case 2:
							num100 = 155;
							break;
						case 3:
							num100 = 156;
							break;
						case 4:
							num100 = 157;
							break;
						case 5:
							num100 = 163;
							break;
						case 6:
							num100 = 113;
							break;
						case 7:
							num100 = 3317;
							break;
						case 8:
							num100 = 327;
							style3 = 0;
							break;
						default:
							num100 = 164;
							num95 = 0;
							break;
					}

					if (num99 < Main.worldSurface + 50.0)
					{
						num100 = 327;
						style3 = 0;
					}

					if (WorldGen.AddBuriedChest(i3, num99, num100, false, style3))
					{
						num97 += 1000;
						num95++;
					}

					num97++;
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

			num17 = 0;
			num18 = 1000;
			num19 = 0;
			Dungeon.MakeDungeon_Lights(num2, ref num17, num18, ref num19, array);
			num17 = 0;
			num18 = 1000;
			num19 = 0;
			Dungeon.MakeDungeon_Traps(ref num17, num18, ref num19);
			float count = Dungeon.MakeDungeon_GroundFurniture(num3);
			count = Dungeon.MakeDungeon_Pictures(array, count);
			count = Dungeon.MakeDungeon_Banners(array, count);
		}
	}
}