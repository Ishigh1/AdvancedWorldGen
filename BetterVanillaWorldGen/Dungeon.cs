using System;
using System.Collections.Generic;
using System.Reflection;
using AdvancedWorldGen.Base;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class Dungeon
	{
		public static List<(int x, int y)> DungeonRoomPos;
		public static List<int> dRoomSize;
		public static List<bool> dRoomTreasure;
		public static List<int> dRoomL;
		public static List<int> dRoomR;
		public static List<int> dRoomT;
		public static List<int> dRoomB;

		public static List<(int x, int y, int pos)> Doors;

		public static List<(int x, int y)> DungeonPlatforms;

		public static int dEnteranceX;
		public static bool dSurface;
		public static double dxStrength1;
		public static double dyStrength1;
		public static double dxStrength2;
		public static double dyStrength2;
		public static int dMinX;
		public static int dMaxX;
		public static int dMinY;
		public static int dMaxY;
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
			dEnteranceX = 0;
			DungeonRoomPos = new List<(int x, int y)>();
			dRoomSize = new List<int>();
			dRoomTreasure = new List<bool>();
			dRoomL = new List<int>();
			dRoomR = new List<int>();
			dRoomT = new List<int>();
			dRoomB = new List<int>();
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
			typeof(WorldGen).GetField("crackedType", BindingFlags.NonPublic | BindingFlags.Static)
				.SetValue(null, CrackedType);
			WorldGen.dungeonLake = true;
			WorldGen.dungeonX = x;
			WorldGen.dungeonY = y;
			dMinX = x;
			dMaxX = x;
			dMinY = y;
			dMaxY = y;
			dxStrength1 = WorldGen.genRand.Next(25, 30);
			dyStrength1 = WorldGen.genRand.Next(20, 25);
			dxStrength2 = WorldGen.genRand.Next(35, 50);
			dyStrength2 = WorldGen.genRand.Next(10, 15);
			int roomsToMake = Main.maxTilesX / 60;
			roomsToMake += WorldGen.genRand.Next(roomsToMake / 3);
			float maxRooms = roomsToMake;
			int num6 = 5;
			DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
			while (roomsToMake > 0)
			{
				if (WorldGen.dungeonX < dMinX) dMinX = WorldGen.dungeonX;

				if (WorldGen.dungeonX > dMaxX) dMaxX = WorldGen.dungeonX;

				if (WorldGen.dungeonY > dMaxY) dMaxY = WorldGen.dungeonY;

				roomsToMake -= 1;
				Main.statusText = Lang.gen[58].Value + " " + (int) ((maxRooms - roomsToMake) / maxRooms * 60f) + "%";
				if (num6 > 0)
					num6--;

				if ((num6 == 0) & (WorldGen.genRand.Next(3) == 0))
				{
					num6 = 5;
					if (WorldGen.genRand.Next(2) == 0)
					{
						int num7 = WorldGen.dungeonX;
						int num8 = WorldGen.dungeonY;
						DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
						if (WorldGen.genRand.Next(2) == 0)
							DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);

						DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
						WorldGen.dungeonX = num7;
						WorldGen.dungeonY = num8;
					}
					else
					{
						DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
					}
				}
				else
				{
					DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
				}
			}

			DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
			(int dungeonX, int dungeonY) = DungeonRoomPos[0];
			for (int i = 0; i < DungeonRoomPos.Count; i++)
				if (DungeonRoomPos[i].y < dungeonY)
				{
					dungeonX = DungeonRoomPos[i].x;
					dungeonY = DungeonRoomPos[i].y;
				}

			WorldGen.dungeonX = dungeonX;
			WorldGen.dungeonY = dungeonY;
			dEnteranceX = dungeonX;
			dSurface = false;
			num6 = 5;
			if (WorldGen.drunkWorldGen) dSurface = true;

			while (!dSurface)
			{
				if (num6 > 0)
					num6--;

				if (num6 == 0 && WorldGen.genRand.Next(5) == 0 && WorldGen.dungeonY > Main.worldSurface + 100.0)
				{
					num6 = 10;
					int num11 = WorldGen.dungeonX;
					int num12 = WorldGen.dungeonY;
					DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3, true);
					DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
					WorldGen.dungeonX = num11;
					WorldGen.dungeonY = num12;
				}

				DungeonStairs(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
			}

			DungeonEnt(WorldGen.dungeonX, WorldGen.dungeonY, num2, num3);
			Main.statusText = Lang.gen[58].Value + " 65%";
			int num13 = Main.maxTilesX * 2;
			int num14;
			for (num14 = 0; num14 < num13; num14++)
			{
				int i2 = WorldGen.genRand.Next(dMinX, dMaxX);
				int num15 = dMinY;
				if (num15 < Main.worldSurface)
					num15 = (int) Main.worldSurface;

				int j = WorldGen.genRand.Next(num15, dMaxY);
				num14 = !DungeonPitTrap(i2, j, num2, num3) ? num14 + 1 : num14 + 1500;
			}

			for (int k = 0; k < DungeonRoomPos.Count; k++)
			{
				for (int l = dRoomL[k]; l <= dRoomR[k]; l++)
					if (!Main.tile[l, dRoomT[k] - 1].IsActive)
					{
						DungeonPlatforms.Add((l, dRoomT[k] - 1));
						break;
					}

				for (int m = dRoomL[k]; m <= dRoomR[k]; m++)
					if (!Main.tile[m, dRoomB[k] + 1].IsActive)
					{
						DungeonPlatforms.Add((m, dRoomT[k] + 1));
						break;
					}

				for (int n = dRoomT[k]; n <= dRoomB[k]; n++)
					if (!Main.tile[dRoomL[k] - 1, n].IsActive)
					{
						Doors.Add((dRoomL[k] - 1, n, -1));
						break;
					}

				for (int num16 = dRoomT[k]; num16 <= dRoomB[k]; num16++)
					if (!Main.tile[dRoomR[k] + 1, num16].IsActive)
					{
						Doors.Add((dRoomL[k] + 1, num16, 1));
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
				int num21 = WorldGen.genRand.Next(dMinX, dMaxX);
				int num22 = WorldGen.genRand.Next((int) Main.worldSurface + 25, dMaxY);
				if (WorldGen.drunkWorldGen)
					num22 = WorldGen.genRand.Next(WorldGen.dungeonY + 25, dMaxY);

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
				int num26 = WorldGen.genRand.Next(dMinX, dMaxX);
				int num27 = WorldGen.genRand.Next((int) Main.worldSurface + 25, dMaxY);
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
				int num58 = WorldGen.genRand.Next(dMinX, dMaxX);
				int num59 = WorldGen.genRand.Next(dMinY, dMaxY);
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
					int num84 = WorldGen.genRand.Next(dMinX, dMaxX);
					int num85 = WorldGen.genRand.Next((int) Main.worldSurface, dMaxY);
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
				int num86 = WorldGen.genRand.Next(dMinX, dMaxX);
				int num87 = WorldGen.genRand.Next(dMinY, dMaxY);
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
								num91 > dMinX && num91 < dMaxX && !Main.tile[num91, num87].IsActive &&
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
					int num98 = (int) (dRoomSize[num96] * 0.4);
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

			dMinX -= 25;
			dMaxX += 25;
			dMinY -= 25;
			dMaxY += 25;
			if (dMinX < 0) dMinX = 0;

			if (dMaxX > Main.maxTilesX) dMaxX = Main.maxTilesX;

			if (dMinY < 0) dMinY = 0;

			if (dMaxY > Main.maxTilesY) dMaxY = Main.maxTilesY;

			num17 = 0;
			num18 = 1000;
			num19 = 0;
			MakeDungeon_Lights(num2, ref num17, num18, ref num19, array);
			num17 = 0;
			num18 = 1000;
			num19 = 0;
			MakeDungeon_Traps(ref num17, num18, ref num19);
			float count = MakeDungeon_GroundFurniture(num3);
			count = MakeDungeon_Pictures(array, count);
			count = MakeDungeon_Banners(array, count);
		}

		public static void MakeDungeon_Traps(ref int failCount, int failMax, ref int numAdd)
		{
			while (numAdd < Main.maxTilesX / 500)
			{
				failCount++;
				int num = WorldGen.genRand.Next(dMinX, dMaxX);
				int num2 = WorldGen.genRand.Next(dMinY, dMaxY);
				while (num2 < Main.worldSurface) num2 = WorldGen.genRand.Next(dMinY, dMaxY);

				if (Main.wallDungeon[Main.tile[num, num2].wall] && WorldGen.placeTrap(num, num2, 0))
					failCount = failMax;

				if (failCount > failMax)
				{
					numAdd++;
					failCount = 0;
				}
			}
		}

		public static void MakeDungeon_Lights(ushort tileType, ref int failCount, int failMax, ref int numAdd,
			int[] roomWall)
		{
			int[] array = new int[3]
			{
				WorldGen.genRand.Next(7), WorldGen.genRand.Next(7),
				0
			};

			while (array[1] == array[0]) array[1] = WorldGen.genRand.Next(7);

			array[2] = WorldGen.genRand.Next(7);
			while (array[2] == array[0] || array[2] == array[1]) array[2] = WorldGen.genRand.Next(7);

			while (numAdd < Main.maxTilesX / 150)
			{
				failCount++;
				int num = WorldGen.genRand.Next(dMinX, dMaxX);
				int num2 = WorldGen.genRand.Next(dMinY, dMaxY);
				if (Main.wallDungeon[Main.tile[num, num2].wall])
					for (int num3 = num2; num3 > dMinY; num3--)
						if (Main.tile[num, num3 - 1].IsActive && Main.tile[num, num3 - 1].type == tileType)
						{
							bool flag = false;
							for (int i = num - 15; i < num + 15; i++)
							for (int j = num3 - 15; j < num3 + 15; j++)
								if (i > 0 && i < Main.maxTilesX && j > 0 && j < Main.maxTilesY &&
								    (Main.tile[i, j].type == 42 || Main.tile[i, j].type == 34))
								{
									flag = true;
									break;
								}

							if (Main.tile[num - 1, num3].IsActive || Main.tile[num + 1, num3].IsActive ||
							    Main.tile[num - 1, num3 + 1].IsActive || Main.tile[num + 1, num3 + 1].IsActive ||
							    Main.tile[num, num3 + 2].IsActive)
								flag = true;

							if (flag)
								break;

							bool flag2 = false;
							if (!flag2 && WorldGen.genRand.Next(7) == 0)
							{
								int style = 27;
								switch (roomWall[0])
								{
									case 7:
										style = 27;
										break;
									case 8:
										style = 28;
										break;
									case 9:
										style = 29;
										break;
								}

								bool flag3 = false;
								for (int k = 0; k < 15; k++)
									if (WorldGen.SolidTile(num, num3 + k))
									{
										flag3 = true;
										break;
									}

								if (!flag3) WorldGen.PlaceChand(num, num3, 34, style);

								if (Main.tile[num, num3].type == 34)
								{
									flag2 = true;
									failCount = 0;
									numAdd++;
									for (int l = 0; l < 1000; l++)
									{
										int num4 = num + WorldGen.genRand.Next(-12, 13);
										int num5 = num3 + WorldGen.genRand.Next(3, 21);
										if (Main.tile[num4, num5].IsActive || Main.tile[num4, num5 + 1].IsActive ||
										    !Main.tileDungeon[Main.tile[num4 - 1, num5].type] ||
										    !Main.tileDungeon[Main.tile[num4 + 1, num5].type] ||
										    !Collision.CanHit(new Vector2(num4 * 16, num5 * 16), 16, 16,
											    new Vector2(num * 16, num3 * 16 + 1), 16, 16))
											continue;

										if ((WorldGen.SolidTile(num4 - 1, num5) &&
										     Main.tile[num4 - 1, num5].type != 10 ||
										     WorldGen.SolidTile(num4 + 1, num5) &&
										     Main.tile[num4 + 1, num5].type != 10 ||
										     WorldGen.SolidTile(num4, num5 + 1)) &&
										    Main.wallDungeon[Main.tile[num4, num5].wall] &&
										    (Main.tileDungeon[Main.tile[num4 - 1, num5].type] ||
										     Main.tileDungeon[Main.tile[num4 + 1, num5].type]))
											WorldGen.PlaceTile(num4, num5, 136, true);

										if (!Main.tile[num4, num5].IsActive)
											continue;

										while (num4 != num || num5 != num3)
										{
											Main.tile[num4, num5].RedWire = true;
											if (num4 > num)
												num4--;

											if (num4 < num)
												num4++;

											Main.tile[num4, num5].RedWire = true;
											if (num5 > num3)
												num5--;

											if (num5 < num3)
												num5++;

											Main.tile[num4, num5].RedWire = true;
										}

										if (WorldGen.genRand.Next(3) > 0)
										{
											Main.tile[num, num3].frameX = 18;
											Main.tile[num, num3 + 1].frameX = 18;
										}

										break;
									}
								}
							}

							if (flag2)
								break;

							int style2 = array[0];
							if (Main.tile[num, num3].wall == roomWall[1])
								style2 = array[1];

							if (Main.tile[num, num3].wall == roomWall[2])
								style2 = array[2];

							WorldGen.Place1x2Top(num, num3, 42, style2);
							if (Main.tile[num, num3].type != 42)
								break;

							flag2 = true;
							failCount = 0;
							numAdd++;
							for (int m = 0; m < 1000; m++)
							{
								int num6 = num + WorldGen.genRand.Next(-12, 13);
								int num7 = num3 + WorldGen.genRand.Next(3, 21);
								if (Main.tile[num6, num7].IsActive || Main.tile[num6, num7 + 1].IsActive ||
								    Main.tile[num6 - 1, num7].type == 48 || Main.tile[num6 + 1, num7].type == 48 ||
								    !Collision.CanHit(new Vector2(num6 * 16, num7 * 16), 16, 16,
									    new Vector2(num * 16, num3 * 16 + 1), 16, 16))
									continue;

								if (WorldGen.SolidTile(num6 - 1, num7) && Main.tile[num6 - 1, num7].type != 10 ||
								    WorldGen.SolidTile(num6 + 1, num7) && Main.tile[num6 + 1, num7].type != 10 ||
								    WorldGen.SolidTile(num6, num7 + 1)) WorldGen.PlaceTile(num6, num7, 136, true);

								if (!Main.tile[num6, num7].IsActive)
									continue;

								while (num6 != num || num7 != num3)
								{
									Main.tile[num6, num7].RedWire = true;
									if (num6 > num)
										num6--;

									if (num6 < num)
										num6++;

									Main.tile[num6, num7].RedWire = true;
									if (num7 > num3)
										num7--;

									if (num7 < num3)
										num7++;

									Main.tile[num6, num7].RedWire = true;
								}

								if (WorldGen.genRand.Next(3) > 0)
								{
									Main.tile[num, num3].frameX = 18;
									Main.tile[num, num3 + 1].frameX = 18;
								}

								break;
							}

							break;
						}

				if (failCount > failMax)
				{
					numAdd++;
					failCount = 0;
				}
			}
		}

		public static float MakeDungeon_Banners(int[] roomWall, float count)
		{
			count = 840000f / Main.maxTilesX;
			for (int i = 0; (float) i < count; i++)
			{
				int num = WorldGen.genRand.Next(dMinX, dMaxX);
				int num2 = WorldGen.genRand.Next(dMinY, dMaxY);
				while (!Main.wallDungeon[Main.tile[num, num2].wall] || Main.tile[num, num2].IsActive)
				{
					num = WorldGen.genRand.Next(dMinX, dMaxX);
					num2 = WorldGen.genRand.Next(dMinY, dMaxY);
				}

				while (!WorldGen.SolidTile(num, num2) && num2 > 10) num2--;

				num2++;
				if (!Main.wallDungeon[Main.tile[num, num2].wall] || Main.tile[num, num2 - 1].type == 48 ||
				    Main.tile[num, num2].IsActive || Main.tile[num, num2 + 1].IsActive ||
				    Main.tile[num, num2 + 2].IsActive || Main.tile[num, num2 + 3].IsActive)
					continue;

				bool flag = true;
				for (int j = num - 1; j <= num + 1; j++)
				for (int k = num2; k <= num2 + 3; k++)
					if (Main.tile[j, k].IsActive && (Main.tile[j, k].type == 10 || Main.tile[j, k].type == 11 ||
					                                 Main.tile[j, k].type == 91))
						flag = false;

				if (flag)
				{
					int num3 = 10;
					if (Main.tile[num, num2].wall == roomWall[1])
						num3 = 12;

					if (Main.tile[num, num2].wall == roomWall[2])
						num3 = 14;

					num3 += WorldGen.genRand.Next(2);
					WorldGen.PlaceTile(num, num2, 91, true, false, -1, num3);
				}
			}

			return count;
		}

		public static float MakeDungeon_Pictures(int[] roomWall, float count)
		{
			count = 420000f / Main.maxTilesX;
			for (int i = 0; (float) i < count; i++)
			{
				int num = WorldGen.genRand.Next(dMinX, dMaxX);
				int num2 = WorldGen.genRand.Next((int) Main.worldSurface, dMaxY);
				while (!Main.wallDungeon[Main.tile[num, num2].wall] || Main.tile[num, num2].IsActive)
				{
					num = WorldGen.genRand.Next(dMinX, dMaxX);
					num2 = WorldGen.genRand.Next((int) Main.worldSurface, dMaxY);
				}

				int num3 = num;
				int num4 = num;
				int num5 = num2;
				int num6 = num2;
				int num7 = 0;
				int num8 = 0;
				for (int j = 0; j < 2; j++)
				{
					num3 = num;
					num4 = num;
					while (!Main.tile[num3, num2].IsActive && Main.wallDungeon[Main.tile[num3, num2].wall]) num3--;

					num3++;
					for (; !Main.tile[num4, num2].IsActive && Main.wallDungeon[Main.tile[num4, num2].wall]; num4++)
					{
					}

					num4--;
					num = (num3 + num4) / 2;
					num5 = num2;
					num6 = num2;
					while (!Main.tile[num, num5].IsActive && Main.wallDungeon[Main.tile[num, num5].wall]) num5--;

					num5++;
					for (; !Main.tile[num, num6].IsActive && Main.wallDungeon[Main.tile[num, num6].wall]; num6++)
					{
					}

					num6--;
					num2 = (num5 + num6) / 2;
				}

				num3 = num;
				num4 = num;
				while (!Main.tile[num3, num2].IsActive && !Main.tile[num3, num2 - 1].IsActive &&
				       !Main.tile[num3, num2 + 1].IsActive) num3--;

				num3++;
				for (;
					!Main.tile[num4, num2].IsActive && !Main.tile[num4, num2 - 1].IsActive &&
					!Main.tile[num4, num2 + 1].IsActive;
					num4++)
				{
				}

				num4--;
				num5 = num2;
				num6 = num2;
				while (!Main.tile[num, num5].IsActive && !Main.tile[num - 1, num5].IsActive &&
				       !Main.tile[num + 1, num5].IsActive) num5--;

				num5++;
				for (;
					!Main.tile[num, num6].IsActive && !Main.tile[num - 1, num6].IsActive &&
					!Main.tile[num + 1, num6].IsActive;
					num6++)
				{
				}

				num6--;
				num = (num3 + num4) / 2;
				num2 = (num5 + num6) / 2;
				num7 = num4 - num3;
				num8 = num6 - num5;
				if (num7 <= 7 || num8 <= 5)
					continue;

				bool[] array = new bool[3]
				{
					true,
					false,
					false
				};

				if (num7 > num8 * 3 && num7 > 21)
					array[1] = true;

				if (num8 > num7 * 3 && num8 > 21)
					array[2] = true;

				int num9 = WorldGen.genRand.Next(3);
				if (Main.tile[num, num2].wall == roomWall[0])
					num9 = 0;

				while (!array[num9]) num9 = WorldGen.genRand.Next(3);

				if (WorldGen.nearPicture2(num, num2))
					num9 = -1;

				switch (num9)
				{
					case 0:
					{
						Vector2 vector2 = randPictureTile();
						if (Main.tile[num, num2].wall != roomWall[0])
							vector2 = randBoneTile();

						int type2 = (int) vector2.X;
						int style2 = (int) vector2.Y;
						if (!WorldGen.nearPicture(num, num2))
							WorldGen.PlaceTile(num, num2, type2, true, false, -1, style2);

						break;
					}
					case 1:
					{
						Vector2 vector3 = randPictureTile();
						if (Main.tile[num, num2].wall != roomWall[0])
							vector3 = randBoneTile();

						int type3 = (int) vector3.X;
						int style3 = (int) vector3.Y;
						if (!Main.tile[num, num2].IsActive)
							WorldGen.PlaceTile(num, num2, type3, true, false, -1, style3);

						int num13 = num;
						int num14 = num2;
						int num15 = num2;
						for (int m = 0; m < 2; m++)
						{
							num += 7;
							num5 = num15;
							num6 = num15;
							while (!Main.tile[num, num5].IsActive && !Main.tile[num - 1, num5].IsActive &&
							       !Main.tile[num + 1, num5].IsActive) num5--;

							num5++;
							for (;
								!Main.tile[num, num6].IsActive && !Main.tile[num - 1, num6].IsActive &&
								!Main.tile[num + 1, num6].IsActive;
								num6++)
							{
							}

							num6--;
							num15 = (num5 + num6) / 2;
							vector3 = randPictureTile();
							if (Main.tile[num, num15].wall != roomWall[0])
								vector3 = randBoneTile();

							type3 = (int) vector3.X;
							style3 = (int) vector3.Y;
							if (Math.Abs(num14 - num15) >= 4 || WorldGen.nearPicture(num, num15))
								break;

							WorldGen.PlaceTile(num, num15, type3, true, false, -1, style3);
						}

						num15 = num2;
						num = num13;
						for (int n = 0; n < 2; n++)
						{
							num -= 7;
							num5 = num15;
							num6 = num15;
							while (!Main.tile[num, num5].IsActive && !Main.tile[num - 1, num5].IsActive &&
							       !Main.tile[num + 1, num5].IsActive) num5--;

							num5++;
							for (;
								!Main.tile[num, num6].IsActive && !Main.tile[num - 1, num6].IsActive &&
								!Main.tile[num + 1, num6].IsActive;
								num6++)
							{
							}

							num6--;
							num15 = (num5 + num6) / 2;
							vector3 = randPictureTile();
							if (Main.tile[num, num15].wall != roomWall[0])
								vector3 = randBoneTile();

							type3 = (int) vector3.X;
							style3 = (int) vector3.Y;
							if (Math.Abs(num14 - num15) >= 4 || WorldGen.nearPicture(num, num15))
								break;

							WorldGen.PlaceTile(num, num15, type3, true, false, -1, style3);
						}

						break;
					}
					case 2:
					{
						Vector2 vector = randPictureTile();
						if (Main.tile[num, num2].wall != roomWall[0])
							vector = randBoneTile();

						int type = (int) vector.X;
						int style = (int) vector.Y;
						if (!Main.tile[num, num2].IsActive) WorldGen.PlaceTile(num, num2, type, true, false, -1, style);

						int num10 = num2;
						int num11 = num;
						int num12 = num;
						for (int k = 0; k < 3; k++)
						{
							num2 += 7;
							num3 = num12;
							num4 = num12;
							while (!Main.tile[num3, num2].IsActive && !Main.tile[num3, num2 - 1].IsActive &&
							       !Main.tile[num3, num2 + 1].IsActive) num3--;

							num3++;
							for (;
								!Main.tile[num4, num2].IsActive && !Main.tile[num4, num2 - 1].IsActive &&
								!Main.tile[num4, num2 + 1].IsActive;
								num4++)
							{
							}

							num4--;
							num12 = (num3 + num4) / 2;
							vector = randPictureTile();
							if (Main.tile[num12, num2].wall != roomWall[0])
								vector = randBoneTile();

							type = (int) vector.X;
							style = (int) vector.Y;
							if (Math.Abs(num11 - num12) >= 4 || WorldGen.nearPicture(num12, num2))
								break;

							WorldGen.PlaceTile(num12, num2, type, true, false, -1, style);
						}

						num12 = num;
						num2 = num10;
						for (int l = 0; l < 3; l++)
						{
							num2 -= 7;
							num3 = num12;
							num4 = num12;
							while (!Main.tile[num3, num2].IsActive && !Main.tile[num3, num2 - 1].IsActive &&
							       !Main.tile[num3, num2 + 1].IsActive) num3--;

							num3++;
							for (;
								!Main.tile[num4, num2].IsActive && !Main.tile[num4, num2 - 1].IsActive &&
								!Main.tile[num4, num2 + 1].IsActive;
								num4++)
							{
							}

							num4--;
							num12 = (num3 + num4) / 2;
							vector = randPictureTile();
							if (Main.tile[num12, num2].wall != roomWall[0])
								vector = randBoneTile();

							type = (int) vector.X;
							style = (int) vector.Y;
							if (Math.Abs(num11 - num12) >= 4 || WorldGen.nearPicture(num12, num2))
								break;

							WorldGen.PlaceTile(num12, num2, type, true, false, -1, style);
						}

						break;
					}
				}
			}

			return count;
		}

		public static float MakeDungeon_GroundFurniture(int wallType)
		{
			int num = 2000 * Main.maxTilesX / 4200;
			int num2 = 1 + Main.maxTilesX / 4200;
			int num3 = 1 + Main.maxTilesX / 4200;
			for (int i = 0; i < num; i++)
			{
				int num4 = WorldGen.genRand.Next(dMinX, dMaxX);
				int j = WorldGen.genRand.Next((int) Main.worldSurface + 10, dMaxY);
				while (!Main.wallDungeon[Main.tile[num4, j].wall] || Main.tile[num4, j].IsActive)
				{
					num4 = WorldGen.genRand.Next(dMinX, dMaxX);
					j = WorldGen.genRand.Next((int) Main.worldSurface + 10, dMaxY);
				}

				if (!Main.wallDungeon[Main.tile[num4, j].wall] || Main.tile[num4, j].IsActive)
					continue;

				for (; !WorldGen.SolidTile(num4, j) && j < Main.UnderworldLayer; j++)
				{
				}

				j--;
				int num5 = num4;
				int k = num4;
				while (!Main.tile[num5, j].IsActive && WorldGen.SolidTile(num5, j + 1)) num5--;

				num5++;
				for (; !Main.tile[k, j].IsActive && WorldGen.SolidTile(k, j + 1); k++)
				{
				}

				k--;
				int num6 = k - num5;
				int num7 = (k + num5) / 2;
				if (Main.tile[num7, j].IsActive || !Main.wallDungeon[Main.tile[num7, j].wall] ||
				    !WorldGen.SolidTile(num7, j + 1) || Main.tile[num7, j + 1].type == 48)
					continue;

				int style = 13;
				int style2 = 10;
				int style3 = 11;
				int num8 = 1;
				int num9 = 46;
				int style4 = 1;
				int num10 = 5;
				int num11 = 11;
				int num12 = 5;
				int num13 = 6;
				int num14 = 21;
				int num15 = 22;
				int num16 = 24;
				int num17 = 30;
				switch (wallType)
				{
					case 8:
						style = 14;
						style2 = 11;
						style3 = 12;
						num8 = 2;
						num9 = 47;
						style4 = 2;
						num10 = 6;
						num11 = 12;
						num12 = 6;
						num13 = 7;
						num14 = 22;
						num15 = 23;
						num16 = 25;
						num17 = 31;
						break;
					case 9:
						style = 15;
						style2 = 12;
						style3 = 13;
						num8 = 3;
						num9 = 48;
						style4 = 3;
						num10 = 7;
						num11 = 13;
						num12 = 7;
						num13 = 8;
						num14 = 23;
						num15 = 24;
						num16 = 26;
						num17 = 32;
						break;
				}

				if (Main.tile[num7, j].wall >= 94 && Main.tile[num7, j].wall <= 105)
				{
					style = 17;
					style2 = 14;
					style3 = 15;
					num8 = -1;
					num9 = -1;
					style4 = 5;
					num10 = -1;
					num11 = -1;
					num12 = -1;
					num13 = -1;
					num14 = -1;
					num15 = -1;
					num16 = -1;
					num17 = -1;
				}

				int num18 = WorldGen.genRand.Next(13);
				if ((num18 == 10 || num18 == 11 || num18 == 12) && WorldGen.genRand.Next(4) != 0)
					num18 = WorldGen.genRand.Next(13);

				while (num18 == 2 && num9 == -1 || num18 == 5 && num10 == -1 || num18 == 6 && num11 == -1 ||
				       num18 == 7 && num12 == -1 || num18 == 8 && num13 == -1 || num18 == 9 && num14 == -1 ||
				       num18 == 10 && num15 == -1 || num18 == 11 && num16 == -1 ||
				       num18 == 12 && num17 == -1) num18 = WorldGen.genRand.Next(13);

				int num19 = 0;
				int num20 = 0;
				switch (num18)
				{
					case 0:
						num19 = 5;
						num20 = 4;
						break;
					case 1:
						num19 = 4;
						num20 = 3;
						break;
					case 2:
						num19 = 3;
						num20 = 5;
						break;
					case 3:
						num19 = 4;
						num20 = 6;
						break;
					case 4:
						num19 = 3;
						num20 = 3;
						break;
					case 5:
						num19 = 5;
						num20 = 3;
						break;
					case 6:
						num19 = 5;
						num20 = 4;
						break;
					case 7:
						num19 = 5;
						num20 = 4;
						break;
					case 8:
						num19 = 5;
						num20 = 4;
						break;
					case 9:
						num19 = 5;
						num20 = 3;
						break;
					case 10:
						num19 = 2;
						num20 = 4;
						break;
					case 11:
						num19 = 3;
						num20 = 3;
						break;
					case 12:
						num19 = 2;
						num20 = 5;
						break;
				}

				for (int l = num7 - num19; l <= num7 + num19; l++)
				for (int m = j - num20; m <= j; m++)
					if (Main.tile[l, m].IsActive)
					{
						num18 = -1;
						break;
					}

				if (num6 < num19 * 1.75)
					num18 = -1;

				if (num2 > 0)
				{
					WorldGen.PlaceTile(num7, j, TileID.AlchemyTable, true);
					if (Main.tile[num7, j].type == 355)
						num2--;
					continue;
				}
				else if (num3 > 0)
				{
					WorldGen.PlaceTile(num7, j, TileID.BewitchingTable, true);
					if (Main.tile[num7, j].type == 354)
						num3--;
					continue;
				}

				switch (num18)
				{
					case 0:
					{
						WorldGen.PlaceTile(num7, j, 14, true, false, -1, style2);
						if (Main.tile[num7, j].IsActive)
						{
							if (!Main.tile[num7 - 2, j].IsActive)
							{
								WorldGen.PlaceTile(num7 - 2, j, 15, true, false, -1, style);
								if (Main.tile[num7 - 2, j].IsActive)
								{
									Main.tile[num7 - 2, j].frameX += 18;
									Main.tile[num7 - 2, j - 1].frameX += 18;
								}
							}

							if (!Main.tile[num7 + 2, j].IsActive)
								WorldGen.PlaceTile(num7 + 2, j, 15, true, false, -1, style);
						}

						for (int num22 = num7 - 1; num22 <= num7 + 1; num22++)
							if (WorldGen.genRand.Next(2) == 0 && !Main.tile[num22, j - 2].IsActive)
							{
								int num23 = WorldGen.genRand.Next(5);
								if (num8 != -1 && num23 <= 1 && !Main.tileLighted[Main.tile[num22 - 1, j - 2].type])
									WorldGen.PlaceTile(num22, j - 2, 33, true, false, -1, num8);

								if (num23 == 2 && !Main.tileLighted[Main.tile[num22 - 1, j - 2].type])
									WorldGen.PlaceTile(num22, j - 2, 49, true);

								if (num23 == 3) WorldGen.PlaceTile(num22, j - 2, 50, true);

								if (num23 == 4) WorldGen.PlaceTile(num22, j - 2, 103, true);
							}

						break;
					}
					case 1:
					{
						WorldGen.PlaceTile(num7, j, 18, true, false, -1, style3);
						if (!Main.tile[num7, j].IsActive)
							break;

						if (WorldGen.genRand.Next(2) == 0)
						{
							if (!Main.tile[num7 - 1, j].IsActive)
							{
								WorldGen.PlaceTile(num7 - 1, j, 15, true, false, -1, style);
								if (Main.tile[num7 - 1, j].IsActive)
								{
									Main.tile[num7 - 1, j].frameX += 18;
									Main.tile[num7 - 1, j - 1].frameX += 18;
								}
							}
						}
						else if (!Main.tile[num7 + 2, j].IsActive)
						{
							WorldGen.PlaceTile(num7 + 2, j, 15, true, false, -1, style);
						}

						for (int n = num7; n <= num7 + 1; n++)
							if (WorldGen.genRand.Next(2) == 0 && !Main.tile[n, j - 1].IsActive)
							{
								int num21 = WorldGen.genRand.Next(5);
								if (num8 != -1 && num21 <= 1 && !Main.tileLighted[Main.tile[n - 1, j - 1].type])
									WorldGen.PlaceTile(n, j - 1, 33, true, false, -1, num8);

								if (num21 == 2 && !Main.tileLighted[Main.tile[n - 1, j - 1].type])
									WorldGen.PlaceTile(n, j - 1, 49, true);

								if (num21 == 3) WorldGen.PlaceTile(n, j - 1, 50, true);

								if (num21 == 4) WorldGen.PlaceTile(n, j - 1, 103, true);
							}

						break;
					}
					case 2:
						WorldGen.PlaceTile(num7, j, 105, true, false, -1, num9);
						break;
					case 3:
						WorldGen.PlaceTile(num7, j, 101, true, false, -1, style4);
						break;
					case 4:
						if (WorldGen.genRand.Next(2) == 0)
						{
							WorldGen.PlaceTile(num7, j, 15, true, false, -1, style);
							Main.tile[num7, j].frameX += 18;
							Main.tile[num7, j - 1].frameX += 18;
						}
						else
						{
							WorldGen.PlaceTile(num7, j, 15, true, false, -1, style);
						}

						break;
					case 5:
						if (WorldGen.genRand.Next(2) == 0)
							WorldGen.Place4x2(num7, j, 79, 1, num10);
						else
							WorldGen.Place4x2(num7, j, 79, -1, num10);
						break;
					case 6:
						WorldGen.PlaceTile(num7, j, 87, true, false, -1, num11);
						break;
					case 7:
						WorldGen.PlaceTile(num7, j, 88, true, false, -1, num12);
						break;
					case 8:
						WorldGen.PlaceTile(num7, j, 89, true, false, -1, num13);
						break;
					case 9:
						if (WorldGen.genRand.Next(2) == 0)
							WorldGen.Place4x2(num7, j, 90, 1, num14);
						else
							WorldGen.Place4x2(num7, j, 90, -1, num14);
						break;
					case 10:
						WorldGen.PlaceTile(num7, j, 93, true, false, -1, num16);
						break;
					case 11:
						WorldGen.PlaceTile(num7, j, 100, true, false, -1, num15);
						break;
					case 12:
						WorldGen.PlaceTile(num7, j, 104, true, false, -1, num17);
						break;
				}
			}

			return num;
		}

		public static Vector2 randBoneTile()
		{
			int num = WorldGen.genRand.Next(2);
			int num2 = 0;
			switch (num)
			{
				case 0:
					num = 240;
					num2 = WorldGen.genRand.Next(2);
					switch (num2)
					{
						case 0:
							num2 = 16;
							break;
						case 1:
							num2 = 17;
							break;
					}

					break;
				case 1:
					num = 241;
					num2 = WorldGen.genRand.Next(9);
					break;
			}

			return new Vector2(num, num2);
		}

		public static Vector2 randHellPicture()
		{
			int num = WorldGen.genRand.Next(4);
			int num2 = 0;
			if (num == 1)
				num = WorldGen.genRand.Next(4);

			switch (num)
			{
				case 0:
					num = 240;
					num2 = WorldGen.genRand.Next(5);
					switch (num2)
					{
						case 0:
							num2 = 27;
							break;
						case 1:
							num2 = 29;
							break;
						case 2:
							num2 = 30;
							break;
						case 3:
							num2 = 31;
							break;
						case 4:
							num2 = 32;
							break;
					}

					break;
				case 1:
					num = 242;
					num2 = 14;
					break;
				case 2:
					num = 245;
					num2 = WorldGen.genRand.Next(3);
					switch (num2)
					{
						case 0:
							num2 = 1;
							break;
						case 1:
							num2 = 2;
							break;
						case 2:
							num2 = 4;
							break;
					}

					break;
				default:
					num = 246;
					num2 = WorldGen.genRand.Next(3);
					switch (num2)
					{
						case 0:
							num2 = 0;
							break;
						case 1:
							num2 = 16;
							break;
						case 2:
							num2 = 17;
							break;
					}

					break;
			}

			return new Vector2(num, num2);
		}

		public static Vector2 RandHousePictureDesert()
		{
			int num = WorldGen.genRand.Next(4);
			int num2 = 0;
			if (num <= 1)
			{
				num = 240;
				int maxValue = 6;
				num2 = 63 + WorldGen.genRand.Next(maxValue);
			}
			else if (num == 2)
			{
				num = 245;
				int maxValue2 = 2;
				num2 = 7 + WorldGen.genRand.Next(maxValue2);
			}
			else
			{
				num = 242;
				int maxValue3 = 6;
				num2 = 37 + WorldGen.genRand.Next(maxValue3);
			}

			return new Vector2(num, num2);
		}

		public static Vector2 randHousePicture()
		{
			int num = WorldGen.genRand.Next(4);
			int num2 = 0;
			if (num >= 3 && WorldGen.genRand.Next(10) != 0)
				num = WorldGen.genRand.Next(3);

			if (num <= 1)
			{
				num = 240;
				int maxValue = 10;
				num2 = WorldGen.genRand.Next(maxValue);
				if (num2 == 9)
					num2 = WorldGen.genRand.Next(maxValue);

				if (num2 == 5)
					num2 = WorldGen.genRand.Next(maxValue);

				if (num2 == 6)
					num2 = WorldGen.genRand.Next(maxValue);

				switch (num2)
				{
					case 0:
						num2 = 26;
						break;
					case 1:
						num2 = 28;
						break;
					case 2:
						num2 = 20;
						break;
					case 3:
						num2 = 21;
						break;
					case 4:
						num2 = 22;
						break;
					case 5:
						num2 = 24;
						break;
					case 6:
						num2 = 25;
						break;
					case 7:
						num2 = 33;
						break;
					case 8:
						num2 = 34;
						break;
					case 9:
						num2 = 35;
						break;
				}
			}
			else if (num == 2)
			{
				int maxValue2 = 4;
				num = 245;
				num2 = WorldGen.genRand.Next(maxValue2);
				if (num2 == 2)
					num2 = WorldGen.genRand.Next(maxValue2);

				if (num2 == 0)
					num2 = WorldGen.genRand.Next(maxValue2);

				if (num2 == 0)
					num2 = WorldGen.genRand.Next(maxValue2);

				if (num2 == 0)
					num2 = WorldGen.genRand.Next(maxValue2);

				switch (num2)
				{
					case 0:
						num2 = 0;
						break;
					case 1:
						num2 = 3;
						break;
					case 2:
						num2 = 5;
						break;
					case 3:
						num2 = 6;
						break;
				}
			}
			else
			{
				num = 246;
				num2 = 1;
			}

			return new Vector2(num, num2);
		}

		public static Vector2 randPictureTile()
		{
			int num = WorldGen.genRand.Next(3);
			int num2 = 0;
			if (num <= 1)
			{
				int maxValue = 7;
				num = 240;
				num2 = WorldGen.genRand.Next(maxValue);
				if (num2 == 6)
					num2 = WorldGen.genRand.Next(maxValue);

				switch (num2)
				{
					case 0:
						num2 = 12;
						break;
					case 1:
						num2 = 13;
						break;
					case 2:
						num2 = 14;
						break;
					case 3:
						num2 = 15;
						break;
					case 4:
						num2 = 18;
						break;
					case 5:
						num2 = 19;
						break;
					case 6:
						num2 = 23;
						break;
				}
			}
			else if (num == 2)
			{
				num = 242;
				int maxValue2 = 17;
				num2 = WorldGen.genRand.Next(maxValue2);
				switch (num2)
				{
					case 14:
						num2 = 15;
						break;
					case 15:
						num2 = 16;
						break;
					case 16:
						num2 = 30;
						break;
				}
			}

			return new Vector2(num, num2);
		}

		public static void DungeonStairs(int x, int y, ushort tileType, int wallType)
		{
			int step = WorldGen.genRand.Next(5, 9);
			Vector2 currentPosition = new(x, y);

			int direction;
			if (x > Main.maxTilesX - 400)
				direction = -1;
			else if (x < 400)
				direction = 1;
			else if (x <= dEnteranceX)
				direction = 1;
			else
				direction = -1;

			Vector2 delta = new(-1, direction);
			if (WorldGen.genRand.Next(3) != 0)
				delta.X *= 1f + WorldGen.genRand.Next(0, 200) * 0.01f;
			else if (WorldGen.genRand.Next(3) == 0)
				delta.X *= WorldGen.genRand.Next(50, 76) * 0.01f;
			else if (WorldGen.genRand.Next(6) == 0)
				delta.Y *= 2f;

			if (WorldGen.dungeonX < Main.maxTilesX / 2 && delta.X < 0f && delta.X < 0.5 ||
			    WorldGen.dungeonX > Main.maxTilesX / 2 && delta.X > 0f && delta.X > 0.5)
				delta.X = -0.5f;

			if (WorldGen.drunkWorldGen)
			{
				direction *= -1;
				delta.X *= -1f;
			}

			for (int num3 = WorldGen.genRand.Next(10, 30); num3 > 0; num3--)
			{
				int xMin = (int) Math.Max(currentPosition.X - step - 4 - WorldGen.genRand.Next(6), 0);
				int xMax = (int) Math.Min(currentPosition.X + step + 4 + WorldGen.genRand.Next(6), Main.maxTilesX);
				int yMin = (int) Math.Max(currentPosition.Y - step - 4, 0);
				int yMax = (int) Math.Min(currentPosition.Y + step + 4 + WorldGen.genRand.Next(6), Main.maxTilesY);

				int num8 = currentPosition.X > Main.maxTilesX / 2f ? -1 : 1;

				int num9 = (int) (currentPosition.X + (float) dxStrength1 * 0.6f * num8 +
				                  (float) dxStrength2 * num8);
				int num10 = (int) (dyStrength2 * 0.5);
				if (currentPosition.Y < Main.worldSurface - 5.0 &&
				    Main.tile[num9, (int) (currentPosition.Y - step - 6.0 + num10)].wall == 0 &&
				    Main.tile[num9, (int) (currentPosition.Y - step - 7.0 + num10)].wall == 0 &&
				    Main.tile[num9, (int) (currentPosition.Y - step - 8.0 + num10)].wall == 0)
				{
					dSurface = true;
					WorldGen.TileRunner(num9, (int) (currentPosition.Y - step - 6.0 + num10),
						WorldGen.genRand.Next(25, 35),
						WorldGen.genRand.Next(10, 20), -1, false, 0f, -1f);
				}

				for (int x1 = xMin; x1 < xMax; x1++)
				for (int y1 = yMin; y1 < yMax; y1++)
				{
					Main.tile[x1, y1].LiquidAmount = 0;
					if (!Main.wallDungeon[Main.tile[x1, y1].wall])
					{
						Main.tile[x1, y1].wall = 0;
						Main.tile[x1, y1].IsActive = true;
						Main.tile[x1, y1].type = tileType;
					}
				}

				for (int x1 = xMin + 1; x1 < xMax - 1; x1++)
				for (int y1 = yMin + 1; y1 < yMax - 1; y1++)
					Main.tile[x1, y1].wall = (ushort) wallType;

				int num11 = 0;
				if (WorldGen.genRand.Next(step) == 0)
					num11 = WorldGen.genRand.Next(1, 3);

				xMin = (int) Math.Max(currentPosition.X - step * 0.5f - num11, 0);
				xMax = (int) Math.Min(currentPosition.X + step * 0.5f + num11, Main.maxTilesX);
				yMin = (int) Math.Max(currentPosition.Y - step * 0.5f - num11, 0);
				yMax = (int) Math.Min(currentPosition.Y + step * 0.5f + num11, Main.maxTilesY);

				for (int x1 = xMin; x1 < xMax; x1++)
				for (int y1 = yMin; y1 < yMax; y1++)
				{
					Main.tile[x1, y1].IsActive = false;
					WorldGen.PlaceWall(x1, y1, wallType, true);
				}

				currentPosition += delta;
				if (currentPosition.Y < Main.worldSurface)
					delta.Y *= 0.98f;

				if (dSurface)
					break;
			}

			WorldGen.dungeonX = (int) currentPosition.X;
			WorldGen.dungeonY = (int) currentPosition.Y;
		}

		public static bool DungeonPitTrap(int i, int j, ushort tileType, int wallType)
		{
			const int num = 30;
			int num2 = j;
			int num3 = num2;
			int num4 = WorldGen.genRand.Next(8, 19);
			int num5 = WorldGen.genRand.Next(19, 46);
			int num6 = num4 + WorldGen.genRand.Next(6, 10);
			int num7 = num5 + WorldGen.genRand.Next(6, 10);
			if (!Main.wallDungeon[Main.tile[i, num2].wall])
				return false;

			if (Main.tile[i, num2].IsActive)
				return false;

			for (int k = num2; k < Main.maxTilesY; k++)
			{
				if (k > Main.maxTilesY - 300)
					return false;

				if (Main.tile[i, k].IsActive && WorldGen.SolidTile(i, k))
				{
					if (Main.tile[i, k].type == 48)
						return false;

					num2 = k;
					break;
				}
			}

			if (!Main.wallDungeon[Main.tile[i - num4, num2].wall] || !Main.wallDungeon[Main.tile[i + num4, num2].wall])
				return false;

			for (int l = num2; l < num2 + num; l++)
			{
				bool flag = true;
				for (int m = i - num4; m <= i + num4; m++)
				{
					Tile tile = Main.tile[m, l];
					if (tile.IsActive && Main.tileDungeon[tile.type])
						flag = false;
				}

				if (flag)
				{
					num2 = l;
					break;
				}
			}

			for (int n = i - num4; n <= i + num4; n++)
			for (int num8 = num2; num8 <= num2 + num5; num8++)
			{
				Tile tile2 = Main.tile[n, num8];
				if (tile2.IsActive && (Main.tileDungeon[tile2.type] || tile2.type == CrackedType))
					return false;
			}

			bool flag2 = false;
			if (WorldGen.dungeonLake)
			{
				flag2 = true;
				WorldGen.dungeonLake = false;
			}
			else if (WorldGen.genRand.Next(8) == 0)
			{
				flag2 = true;
			}

			for (int num9 = i - num4; num9 <= i + num4; num9++)
			for (int num10 = num3; num10 <= num2 + num5; num10++)
				if (Main.tileDungeon[Main.tile[num9, num10].type])
				{
					Main.tile[num9, num10].type = CrackedType;
					Main.tile[num9, num10].wall = (ushort) wallType;
				}

			for (int num11 = i - num6; num11 <= i + num6; num11++)
			for (int num12 = num3; num12 <= num2 + num7; num12++)
			{
				Main.tile[num11, num12].LiquidType = LiquidID.Water;
				Main.tile[num11, num12].LiquidAmount = 0;
				if (!Main.wallDungeon[Main.tile[num11, num12].wall] &&
				    Main.tile[num11, num12].type != CrackedType)
				{
					Main.tile[num11, num12].Clear(TileDataType.Slope);
					Main.tile[num11, num12].type = tileType;
					Main.tile[num11, num12].IsActive = true;
					if (num11 > i - num6 && num11 < i + num6 && num12 < num2 + num7)
						Main.tile[num11, num12].wall = (ushort) wallType;
				}
			}

			for (int num13 = i - num4; num13 <= i + num4; num13++)
			for (int num14 = num3; num14 <= num2 + num5; num14++)
				if (Main.tile[num13, num14].type != CrackedType)
				{
					if (flag2)
						Main.tile[num13, num14].LiquidAmount = byte.MaxValue;

					if (num13 == i - num4 || num13 == i + num4 || num14 == num2 + num5)
						Main.tile[num13, num14].type = 48;
					else if (num13 == i - num4 + 1 && num14 % 2 == 0 || num13 == i + num4 - 1 && num14 % 2 == 0 ||
					         num14 == num2 + num5 - 1 && num13 % 2 == 0)
						Main.tile[num13, num14].type = 48;
					else
						Main.tile[num13, num14].IsActive = false;
				}

			return true;
		}

		public static void DungeonHalls(int i, int j, ushort tileType, int wallType, bool forceX = false)
		{
			Vector2 zero = Vector2.Zero;
			double num = WorldGen.genRand.Next(4, 6);
			double num2 = num;
			Vector2 zero2 = Vector2.Zero;
			Vector2 zero3 = Vector2.Zero;
			int num3 = 1;
			Vector2 vector = default;
			vector.X = i;
			vector.Y = j;
			int num4 = WorldGen.genRand.Next(35, 80);
			bool flag = WorldGen.genRand.Next(5) == 0;

			if (forceX)
			{
				num4 += 20;
				WorldGen.lastDungeonHall = Vector2.Zero;
			}
			else if (WorldGen.genRand.Next(5) == 0)
			{
				num *= 2.0;
				num4 /= 2;
			}

			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = true;
			while (!flag2)
			{
				bool flag5 = false;
				if (flag4 && !forceX)
				{
					bool flag6 = true;
					bool flag7 = true;
					bool flag8 = true;
					bool flag9 = true;
					int num5 = num4;
					bool flag10 = false;
					for (int num6 = j; num6 > j - num5; num6--)
						if (Main.tile[i, num6].wall == wallType)
						{
							if (flag10)
							{
								flag6 = false;
								break;
							}
						}
						else
						{
							flag10 = true;
						}

					flag10 = false;
					for (int k = j; k < j + num5; k++)
						if (Main.tile[i, k].wall == wallType)
						{
							if (flag10)
							{
								flag7 = false;
								break;
							}
						}
						else
						{
							flag10 = true;
						}

					flag10 = false;
					for (int num7 = i; num7 > i - num5; num7--)
						if (Main.tile[num7, j].wall == wallType)
						{
							if (flag10)
							{
								flag8 = false;
								break;
							}
						}
						else
						{
							flag10 = true;
						}

					flag10 = false;
					for (int l = i; l < i + num5; l++)
						if (Main.tile[l, j].wall == wallType)
						{
							if (flag10)
							{
								flag9 = false;
								break;
							}
						}
						else
						{
							flag10 = true;
						}

					if (!flag8 && !flag9 && !flag6 && !flag7)
					{
						num3 = WorldGen.genRand.Next(2) != 0 ? 1 : -1;
						if (WorldGen.genRand.Next(2) == 0)
							flag5 = true;
					}
					else
					{
						int num8 = WorldGen.genRand.Next(4);
						do
						{
							num8 = WorldGen.genRand.Next(4);
						} while (!(num8 == 0 && flag6) && !(num8 == 1 && flag7) && !(num8 == 2 && flag8) &&
						         !(num8 == 3 && flag9));

						switch (num8)
						{
							case 0:
								num3 = -1;
								break;
							case 1:
								num3 = 1;
								break;
							default:
								flag5 = true;
								num3 = num8 != 2 ? 1 : -1;
								break;
						}
					}
				}
				else
				{
					num3 = WorldGen.genRand.Next(2) != 0 ? 1 : -1;
					if (WorldGen.genRand.Next(2) == 0)
						flag5 = true;
				}

				flag4 = false;
				if (forceX)
					flag5 = true;

				if (flag5)
				{
					zero2.Y = 0f;
					zero2.X = num3;
					zero3.Y = 0f;
					zero3.X = -num3;
					zero.Y = 0f;
					zero.X = num3;
					if (WorldGen.genRand.Next(3) == 0)
					{
						if (WorldGen.genRand.Next(2) == 0)
							zero.Y = -0.2f;
						else
							zero.Y = 0.2f;
					}
				}
				else
				{
					num += 1.0;
					zero.Y = num3;
					zero.X = 0f;
					zero2.X = 0f;
					zero2.Y = num3;
					zero3.X = 0f;
					zero3.Y = -num3;
					if (WorldGen.genRand.Next(3) != 0)
					{
						flag3 = true;
						if (WorldGen.genRand.Next(2) == 0)
							zero.X = WorldGen.genRand.Next(10, 20) * 0.1f;
						else
							zero.X = -WorldGen.genRand.Next(10, 20) * 0.1f;
					}
					else if (WorldGen.genRand.Next(2) == 0)
					{
						if (WorldGen.genRand.Next(2) == 0)
							zero.X = WorldGen.genRand.Next(20, 40) * 0.01f;
						else
							zero.X = -WorldGen.genRand.Next(20, 40) * 0.01f;
					}
					else
					{
						num4 /= 2;
					}
				}

				if (WorldGen.lastDungeonHall != zero3)
					flag2 = true;
			}

			int num9 = 0;
			if (!forceX)
			{
				if (vector.X > Main.maxTilesX - 200)
				{
					num3 = -1;
					zero2.Y = 0f;
					zero2.X = num3;
					zero.Y = 0f;
					zero.X = num3;
					if (WorldGen.genRand.Next(3) == 0)
					{
						if (WorldGen.genRand.Next(2) == 0)
							zero.Y = -0.2f;
						else
							zero.Y = 0.2f;
					}
				}
				else if (vector.X < 200f)
				{
					num3 = 1;
					zero2.Y = 0f;
					zero2.X = num3;
					zero.Y = 0f;
					zero.X = num3;
					if (WorldGen.genRand.Next(3) == 0)
					{
						if (WorldGen.genRand.Next(2) == 0)
							zero.Y = -0.2f;
						else
							zero.Y = 0.2f;
					}
				}
				else if (vector.Y > Main.maxTilesY - 300)
				{
					num3 = -1;
					num += 1.0;
					zero.Y = num3;
					zero.X = 0f;
					zero2.X = 0f;
					zero2.Y = num3;
					if (WorldGen.genRand.Next(2) == 0)
					{
						if (WorldGen.genRand.Next(2) == 0)
							zero.X = WorldGen.genRand.Next(20, 50) * 0.01f;
						else
							zero.X = -WorldGen.genRand.Next(20, 50) * 0.01f;
					}
				}
				else if (vector.Y < Main.rockLayer + 100.0)
				{
					num3 = 1;
					num += 1.0;
					zero.Y = num3;
					zero.X = 0f;
					zero2.X = 0f;
					zero2.Y = num3;
					if (WorldGen.genRand.Next(3) != 0)
					{
						flag3 = true;
						if (WorldGen.genRand.Next(2) == 0)
							zero.X = WorldGen.genRand.Next(10, 20) * 0.1f;
						else
							zero.X = -WorldGen.genRand.Next(10, 20) * 0.1f;
					}
					else if (WorldGen.genRand.Next(2) == 0)
					{
						if (WorldGen.genRand.Next(2) == 0)
							zero.X = WorldGen.genRand.Next(20, 50) * 0.01f;
						else
							zero.X = WorldGen.genRand.Next(20, 50) * 0.01f;
					}
				}
				else if (vector.X < Main.maxTilesX / 2f && vector.X > Main.maxTilesX * 0.25)
				{
					num3 = -1;
					zero2.Y = 0f;
					zero2.X = num3;
					zero.Y = 0f;
					zero.X = num3;
					if (WorldGen.genRand.Next(3) == 0)
					{
						if (WorldGen.genRand.Next(2) == 0)
							zero.Y = -0.2f;
						else
							zero.Y = 0.2f;
					}
				}
				else if (vector.X > Main.maxTilesX / 2 && vector.X < Main.maxTilesX * 0.75)
				{
					num3 = 1;
					zero2.Y = 0f;
					zero2.X = num3;
					zero.Y = 0f;
					zero.X = num3;
					if (WorldGen.genRand.Next(3) == 0)
					{
						if (WorldGen.genRand.Next(2) == 0)
							zero.Y = -0.2f;
						else
							zero.Y = 0.2f;
					}
				}
			}

			if (zero2.Y == 0f)
				Doors.Add(((int x, int y, int pos)) (vector.X, vector.Y, 0));
			else
				DungeonPlatforms.Add(((int x, int y)) (vector.X, vector.Y));

			WorldGen.lastDungeonHall = zero2;
			if (Math.Abs(zero.X) > Math.Abs(zero.Y) && WorldGen.genRand.Next(3) != 0)
				num = (int) ((float) num2 * (WorldGen.genRand.Next(110, 150) * 0.01));

			while (num4 > 0)
			{
				num9++;
				if (zero2.X > 0f && vector.X > Main.maxTilesX - 100)
					num4 = 0;
				else if (zero2.X < 0f && vector.X < 100f)
					num4 = 0;
				else if (zero2.Y > 0f && vector.Y > Main.maxTilesY - 100)
					num4 = 0;
				else if (zero2.Y < 0f && vector.Y < Main.rockLayer + 50.0)
					num4 = 0;

				num4--;
				int num10 = (int) (vector.X - num - 4.0 - WorldGen.genRand.Next(6));
				int num11 = (int) (vector.X + num + 4.0 + WorldGen.genRand.Next(6));
				int num12 = (int) (vector.Y - num - 4.0 - WorldGen.genRand.Next(6));
				int num13 = (int) (vector.Y + num + 4.0 + WorldGen.genRand.Next(6));
				if (num10 < 0)
					num10 = 0;

				if (num11 > Main.maxTilesX)
					num11 = Main.maxTilesX;

				if (num12 < 0)
					num12 = 0;

				if (num13 > Main.maxTilesY)
					num13 = Main.maxTilesY;

				for (int m = num10; m < num11; m++)
				for (int n = num12; n < num13; n++)
				{
					if (m < dMinX) dMinX = m;

					if (m > dMaxX) dMaxX = m;

					if (n > dMaxY) dMaxY = n;

					Main.tile[m, n].LiquidAmount = 0;
					if (!Main.wallDungeon[Main.tile[m, n].wall])
					{
						Main.tile[m, n].IsActive = true;
						Main.tile[m, n].type = tileType;
						Main.tile[m, n].Clear(TileDataType.Slope);
					}
				}

				for (int num14 = num10 + 1; num14 < num11 - 1; num14++)
				for (int num15 = num12 + 1; num15 < num13 - 1; num15++)
					Main.tile[num14, num15].wall = (ushort) wallType;

				int num16 = 0;
				if (zero.Y == 0f && WorldGen.genRand.Next((int) num + 1) == 0)
					num16 = WorldGen.genRand.Next(1, 3);
				else if (zero.X == 0f && WorldGen.genRand.Next((int) num - 1) == 0)
					num16 = WorldGen.genRand.Next(1, 3);
				else if (WorldGen.genRand.Next((int) num * 3) == 0)
					num16 = WorldGen.genRand.Next(1, 3);

				num10 = (int) (vector.X - num * 0.5 - num16);
				num11 = (int) (vector.X + num * 0.5 + num16);
				num12 = (int) (vector.Y - num * 0.5 - num16);
				num13 = (int) (vector.Y + num * 0.5 + num16);
				if (num10 < 0)
					num10 = 0;

				if (num11 > Main.maxTilesX)
					num11 = Main.maxTilesX;

				if (num12 < 0)
					num12 = 0;

				if (num13 > Main.maxTilesY)
					num13 = Main.maxTilesY;

				for (int num17 = num10; num17 < num11; num17++)
				for (int num18 = num12; num18 < num13; num18++)
				{
					Main.tile[num17, num18].Clear(TileDataType.Slope);
					if (flag)
					{
						if (Main.tile[num17, num18].IsActive || Main.tile[num17, num18].wall != wallType)
						{
							Main.tile[num17, num18].IsActive = true;
							Main.tile[num17, num18].type = CrackedType;
						}
					}
					else
					{
						Main.tile[num17, num18].IsActive = false;
					}

					Main.tile[num17, num18].Clear(TileDataType.Slope);
					Main.tile[num17, num18].wall = (ushort) wallType;
				}

				vector += zero;
				if (flag3 && num9 > WorldGen.genRand.Next(10, 20))
				{
					num9 = 0;
					zero.X *= -1f;
				}
			}

			WorldGen.dungeonX = (int) vector.X;
			WorldGen.dungeonY = (int) vector.Y;
			if (zero2.Y == 0f)
				Doors.Add(((int x, int y, int pos)) (vector.X, vector.Y, 0));
			else
				DungeonPlatforms.Add(((int x, int y)) (vector.X, vector.Y));
		}

		public static void DungeonRoom(int i, int j, ushort tileType, int wallType)
		{
			double num = WorldGen.genRand.Next(15, 30);
			Vector2 vector = default;
			vector.X = WorldGen.genRand.Next(-10, 11) * 0.1f;
			vector.Y = WorldGen.genRand.Next(-10, 11) * 0.1f;
			Vector2 vector2 = default;
			vector2.X = i;
			vector2.Y = j - (float) num / 2f;
			int num2 = WorldGen.genRand.Next(10, 20);
			double num3 = vector2.X;
			double num4 = vector2.X;
			double num5 = vector2.Y;
			double num6 = vector2.Y;
			while (num2 > 0)
			{
				num2--;
				int num7 = (int) (vector2.X - num * 0.8 - 5.0);
				int num8 = (int) (vector2.X + num * 0.8 + 5.0);
				int num9 = (int) (vector2.Y - num * 0.8 - 5.0);
				int num10 = (int) (vector2.Y + num * 0.8 + 5.0);
				if (num7 < 0)
					num7 = 0;

				if (num8 > Main.maxTilesX)
					num8 = Main.maxTilesX;

				if (num9 < 0)
					num9 = 0;

				if (num10 > Main.maxTilesY)
					num10 = Main.maxTilesY;

				for (int k = num7; k < num8; k++)
				for (int l = num9; l < num10; l++)
				{
					if (k < dMinX) dMinX = k;

					if (k > dMaxX) dMaxX = k;

					if (l > dMaxY) dMaxY = l;

					Main.tile[k, l].LiquidAmount = 0;
					if (!Main.wallDungeon[Main.tile[k, l].wall])
					{
						Main.tile[k, l].Clear(TileDataType.Slope);
						Main.tile[k, l].IsActive = true;
						Main.tile[k, l].type = tileType;
					}
				}

				for (int m = num7 + 1; m < num8 - 1; m++)
				for (int n = num9 + 1; n < num10 - 1; n++)
					Main.tile[m, n].wall = (ushort) wallType;

				num7 = (int) (vector2.X - num * 0.5);
				num8 = (int) (vector2.X + num * 0.5);
				num9 = (int) (vector2.Y - num * 0.5);
				num10 = (int) (vector2.Y + num * 0.5);
				if (num7 < 0)
					num7 = 0;

				if (num8 > Main.maxTilesX)
					num8 = Main.maxTilesX;

				if (num9 < 0)
					num9 = 0;

				if (num10 > Main.maxTilesY)
					num10 = Main.maxTilesY;

				if (num7 < num3)
					num3 = num7;

				if (num8 > num4)
					num4 = num8;

				if (num9 < num5)
					num5 = num9;

				if (num10 > num6)
					num6 = num10;

				for (int num11 = num7; num11 < num8; num11++)
				for (int num12 = num9; num12 < num10; num12++)
				{
					Main.tile[num11, num12].IsActive = false;
					Main.tile[num11, num12].wall = (ushort) wallType;
				}

				vector2 += vector;
				vector.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
				vector.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
				if (vector.X > 1f)
					vector.X = 1f;

				if (vector.X < -1f)
					vector.X = -1f;

				if (vector.Y > 1f)
					vector.Y = 1f;

				if (vector.Y < -1f)
					vector.Y = -1f;
			}

			DungeonRoomPos.Add(((int x, int y)) (vector2.X, vector2.Y));
			dRoomSize.Add((int) num);
			dRoomL.Add((int) num3);
			dRoomR.Add((int) num4);
			dRoomT.Add((int) num5);
			dRoomB.Add((int) num6);
			dRoomTreasure.Add(false);
		}

		public static void DungeonEnt(int i, int j, ushort tileType, int wallType)
		{
			const int num = 60;
			for (int k = i - num; k < i + num; k++)
			for (int l = j - num; l < j + num; l++)
			{
				Main.tile[k, l].LiquidAmount = 0;
				Main.tile[k, l].LiquidType = LiquidID.Water;
				Main.tile[k, l].Clear(TileDataType.Slope);
			}

			double num2 = dxStrength1;
			double num3 = dyStrength1;
			Vector2 vector = default;
			vector.X = i;
			vector.Y = j - (float) num3 / 2f;
			dMinY = (int) vector.Y;
			int num4 = 1;
			if (i > Main.maxTilesX / 2)
				num4 = -1;

			if (WorldGen.drunkWorldGen || WorldGen.getGoodWorldGen)
				num4 *= -1;

			int num5 = (int) (vector.X - num2 * 0.6 - WorldGen.genRand.Next(2, 5));
			int num6 = (int) (vector.X + num2 * 0.6 + WorldGen.genRand.Next(2, 5));
			int num7 = (int) (vector.Y - num3 * 0.6 - WorldGen.genRand.Next(2, 5));
			int num8 = (int) (vector.Y + num3 * 0.6 + WorldGen.genRand.Next(8, 16));
			if (num5 < 0)
				num5 = 0;

			if (num6 > Main.maxTilesX)
				num6 = Main.maxTilesX;

			if (num7 < 0)
				num7 = 0;

			if (num8 > Main.maxTilesY)
				num8 = Main.maxTilesY;

			for (int m = num5; m < num6; m++)
			for (int n = num7; n < num8; n++)
			{
				Main.tile[m, n].LiquidAmount = 0;
				if (Main.tile[m, n].wall != wallType)
				{
					Main.tile[m, n].wall = 0;
					if (m > num5 + 1 && m < num6 - 2 && n > num7 + 1 && n < num8 - 2)
						Main.tile[m, n].wall = (ushort) wallType;

					Main.tile[m, n].IsActive = true;
					Main.tile[m, n].type = tileType;
					Main.tile[m, n].Clear(TileDataType.Slope);
				}
			}

			int num9 = num5;
			int num10 = num5 + 5 + WorldGen.genRand.Next(4);
			int num11 = num7 - 3 - WorldGen.genRand.Next(3);
			int num12 = num7;
			for (int num13 = num9; num13 < num10; num13++)
			for (int num14 = num11; num14 < num12; num14++)
			{
				Main.tile[num13, num14].LiquidAmount = 0;
				if (Main.tile[num13, num14].wall != wallType)
				{
					Main.tile[num13, num14].IsActive = true;
					Main.tile[num13, num14].type = tileType;
					Main.tile[num13, num14].Clear(TileDataType.Slope);
				}
			}

			num9 = num6 - 5 - WorldGen.genRand.Next(4);
			num10 = num6;
			num11 = num7 - 3 - WorldGen.genRand.Next(3);
			num12 = num7;
			for (int num15 = num9; num15 < num10; num15++)
			for (int num16 = num11; num16 < num12; num16++)
			{
				Main.tile[num15, num16].LiquidAmount = 0;
				if (Main.tile[num15, num16].wall != wallType)
				{
					Main.tile[num15, num16].IsActive = true;
					Main.tile[num15, num16].type = tileType;
					Main.tile[num15, num16].Clear(TileDataType.Slope);
				}
			}

			int num17 = 1 + WorldGen.genRand.Next(2);
			int num18 = 2 + WorldGen.genRand.Next(4);
			int num19 = 0;
			for (int num20 = num5; num20 < num6; num20++)
			{
				for (int num21 = num7 - num17; num21 < num7; num21++)
				{
					Main.tile[num20, num21].LiquidAmount = 0;
					if (Main.tile[num20, num21].wall != wallType)
					{
						Main.tile[num20, num21].IsActive = true;
						Main.tile[num20, num21].type = tileType;
						Main.tile[num20, num21].Clear(TileDataType.Slope);
					}
				}

				num19++;
				if (num19 >= num18)
				{
					num20 += num18;
					num19 = 0;
				}
			}

			for (int num22 = num5; num22 < num6; num22++)
			for (int num23 = num8; (double) num23 < Main.worldSurface; num23++)
			{
				Main.tile[num22, num23].LiquidAmount = 0;
				if (!Main.wallDungeon[Main.tile[num22, num23].wall])
				{
					Main.tile[num22, num23].IsActive = true;
					Main.tile[num22, num23].type = tileType;
				}

				if (num22 > num5 && num22 < num6 - 1)
					Main.tile[num22, num23].wall = (ushort) wallType;

				Main.tile[num22, num23].Clear(TileDataType.Slope);
			}

			num5 = (int) (vector.X - num2 * 0.6);
			num6 = (int) (vector.X + num2 * 0.6);
			num7 = (int) (vector.Y - num3 * 0.6);
			num8 = (int) (vector.Y + num3 * 0.6);
			if (num5 < 0)
				num5 = 0;

			if (num6 > Main.maxTilesX)
				num6 = Main.maxTilesX;

			if (num7 < 0)
				num7 = 0;

			if (num8 > Main.maxTilesY)
				num8 = Main.maxTilesY;

			for (int num24 = num5; num24 < num6; num24++)
			for (int num25 = num7; num25 < num8; num25++)
			{
				Main.tile[num24, num25].LiquidAmount = 0;
				Main.tile[num24, num25].wall = (ushort) wallType;
				Main.tile[num24, num25].Clear(TileDataType.Slope);
			}

			num5 = (int) (vector.X - num2 * 0.6 - 1.0);
			num6 = (int) (vector.X + num2 * 0.6 + 1.0);
			num7 = (int) (vector.Y - num3 * 0.6 - 1.0);
			num8 = (int) (vector.Y + num3 * 0.6 + 1.0);
			if (num5 < 0)
				num5 = 0;

			if (num6 > Main.maxTilesX)
				num6 = Main.maxTilesX;

			if (num7 < 0)
				num7 = 0;

			if (num8 > Main.maxTilesY)
				num8 = Main.maxTilesY;

			if (WorldGen.drunkWorldGen)
				num5 -= 4;

			for (int num26 = num5; num26 < num6; num26++)
			for (int num27 = num7; num27 < num8; num27++)
			{
				Main.tile[num26, num27].LiquidAmount = 0;
				Main.tile[num26, num27].wall = (ushort) wallType;
				Main.tile[num26, num27].Clear(TileDataType.Slope);
			}

			num5 = (int) (vector.X - num2 * 0.5);
			num6 = (int) (vector.X + num2 * 0.5);
			num7 = (int) (vector.Y - num3 * 0.5);
			num8 = (int) (vector.Y + num3 * 0.5);
			if (num5 < 0)
				num5 = 0;

			if (num6 > Main.maxTilesX)
				num6 = Main.maxTilesX;

			if (num7 < 0)
				num7 = 0;

			if (num8 > Main.maxTilesY)
				num8 = Main.maxTilesY;

			for (int num28 = num5; num28 < num6; num28++)
			for (int num29 = num7; num29 < num8; num29++)
			{
				Main.tile[num28, num29].LiquidAmount = 0;
				Main.tile[num28, num29].IsActive = false;
				Main.tile[num28, num29].wall = (ushort) wallType;
			}

			int num30 = (int) vector.X;
			int num31 = num8;
			for (int num32 = 0; num32 < 20; num32++)
			{
				num30 = (int) vector.X - num32;
				if (!Main.tile[num30, num31].IsActive && Main.wallDungeon[Main.tile[num30, num31].wall])
				{
					DungeonPlatforms.Add((num30, num31));
					break;
				}

				num30 = (int) vector.X + num32;
				if (!Main.tile[num30, num31].IsActive && Main.wallDungeon[Main.tile[num30, num31].wall])
				{
					DungeonPlatforms.Add((num30, num31));
					break;
				}
			}

			vector.X += (float) num2 * 0.6f * num4;
			vector.Y += (float) num3 * 0.5f;
			num2 = dxStrength2;
			num3 = dyStrength2;
			vector.X += (float) num2 * 0.55f * num4;
			vector.Y -= (float) num3 * 0.5f;
			num5 = (int) (vector.X - num2 * 0.6 - WorldGen.genRand.Next(1, 3));
			num6 = (int) (vector.X + num2 * 0.6 + WorldGen.genRand.Next(1, 3));
			num7 = (int) (vector.Y - num3 * 0.6 - WorldGen.genRand.Next(1, 3));
			num8 = (int) (vector.Y + num3 * 0.6 + WorldGen.genRand.Next(6, 16));
			if (num5 < 0)
				num5 = 0;

			if (num6 > Main.maxTilesX)
				num6 = Main.maxTilesX;

			if (num7 < 0)
				num7 = 0;

			if (num8 > Main.maxTilesY)
				num8 = Main.maxTilesY;

			for (int num33 = num5; num33 < num6; num33++)
			for (int num34 = num7; num34 < num8; num34++)
			{
				Main.tile[num33, num34].LiquidAmount = 0;
				if (Main.tile[num33, num34].wall == wallType)
					continue;

				bool flag = true;
				if (num4 < 0)
				{
					if (num33 < vector.X - num2 * 0.5)
						flag = false;
				}
				else if (num33 > vector.X + num2 * 0.5 - 1.0)
				{
					flag = false;
				}

				if (flag)
				{
					Main.tile[num33, num34].wall = 0;
					Main.tile[num33, num34].IsActive = true;
					Main.tile[num33, num34].type = tileType;
					Main.tile[num33, num34].Clear(TileDataType.Slope);
				}
			}

			for (int num35 = num5; num35 < num6; num35++)
			for (int num36 = num8; (double) num36 < Main.worldSurface; num36++)
			{
				Main.tile[num35, num36].LiquidAmount = 0;
				if (!Main.wallDungeon[Main.tile[num35, num36].wall])
				{
					Main.tile[num35, num36].IsActive = true;
					Main.tile[num35, num36].type = tileType;
				}

				Main.tile[num35, num36].wall = (ushort) wallType;
				Main.tile[num35, num36].Clear(TileDataType.Slope);
			}

			num5 = (int) (vector.X - num2 * 0.5);
			num6 = (int) (vector.X + num2 * 0.5);
			num9 = num5;
			if (num4 < 0)
				num9++;

			num10 = num9 + 5 + WorldGen.genRand.Next(4);
			num11 = num7 - 3 - WorldGen.genRand.Next(3);
			num12 = num7;
			for (int num37 = num9; num37 < num10; num37++)
			for (int num38 = num11; num38 < num12; num38++)
			{
				Main.tile[num37, num38].LiquidAmount = 0;
				if (Main.tile[num37, num38].wall != wallType)
				{
					Main.tile[num37, num38].IsActive = true;
					Main.tile[num37, num38].type = tileType;
					Main.tile[num37, num38].Clear(TileDataType.Slope);
				}
			}

			num9 = num6 - 5 - WorldGen.genRand.Next(4);
			num10 = num6;
			num11 = num7 - 3 - WorldGen.genRand.Next(3);
			num12 = num7;
			for (int num39 = num9; num39 < num10; num39++)
			for (int num40 = num11; num40 < num12; num40++)
			{
				Main.tile[num39, num40].LiquidAmount = 0;
				if (Main.tile[num39, num40].wall != wallType)
				{
					Main.tile[num39, num40].IsActive = true;
					Main.tile[num39, num40].type = tileType;
					Main.tile[num39, num40].Clear(TileDataType.Slope);
				}
			}

			num17 = 1 + WorldGen.genRand.Next(2);
			num18 = 2 + WorldGen.genRand.Next(4);
			num19 = 0;
			if (num4 < 0)
				num6++;

			for (int num41 = num5 + 1; num41 < num6 - 1; num41++)
			{
				for (int num42 = num7 - num17; num42 < num7; num42++)
				{
					Main.tile[num41, num42].LiquidAmount = 0;
					if (Main.tile[num41, num42].wall != wallType)
					{
						Main.tile[num41, num42].IsActive = true;
						Main.tile[num41, num42].type = tileType;
						Main.tile[num41, num42].Clear(TileDataType.Slope);
					}
				}

				num19++;
				if (num19 >= num18)
				{
					num41 += num18;
					num19 = 0;
				}
			}

			if (!WorldGen.drunkWorldGen)
			{
				num5 = (int) (vector.X - num2 * 0.6);
				num6 = (int) (vector.X + num2 * 0.6);
				num7 = (int) (vector.Y - num3 * 0.6);
				num8 = (int) (vector.Y + num3 * 0.6);
				if (num5 < 0)
					num5 = 0;

				if (num6 > Main.maxTilesX)
					num6 = Main.maxTilesX;

				if (num7 < 0)
					num7 = 0;

				if (num8 > Main.maxTilesY)
					num8 = Main.maxTilesY;

				for (int num43 = num5; num43 < num6; num43++)
				for (int num44 = num7; num44 < num8; num44++)
				{
					Main.tile[num43, num44].LiquidAmount = 0;
					Main.tile[num43, num44].wall = 0;
				}
			}

			num5 = (int) (vector.X - num2 * 0.5);
			num6 = (int) (vector.X + num2 * 0.5);
			num7 = (int) (vector.Y - num3 * 0.5);
			num8 = (int) (vector.Y + num3 * 0.5);
			if (num5 < 0)
				num5 = 0;

			if (num6 > Main.maxTilesX)
				num6 = Main.maxTilesX;

			if (num7 < 0)
				num7 = 0;

			if (num8 > Main.maxTilesY)
				num8 = Main.maxTilesY;

			for (int num45 = num5; num45 < num6; num45++)
			for (int num46 = num7; num46 < num8; num46++)
			{
				Main.tile[num45, num46].LiquidAmount = 0;
				Main.tile[num45, num46].IsActive = false;
				Main.tile[num45, num46].wall = 0;
			}

			Main.dungeonX = (int) vector.X;
			Main.dungeonY = num8;
			int num47 = NPC.NewNPC(Main.dungeonX * 16 + 8, Main.dungeonY * 16, 37);
			Main.npc[num47].homeless = false;
			Main.npc[num47].homeTileX = Main.dungeonX;
			Main.npc[num47].homeTileY = Main.dungeonY;
			if (WorldGen.drunkWorldGen)
			{
				int num48 = (int) Main.worldSurface;
				while (Main.tile[WorldGen.dungeonX, num48].IsActive || Main.tile[WorldGen.dungeonX, num48].wall > 0 ||
				       Main.tile[WorldGen.dungeonX, num48 - 1].IsActive ||
				       Main.tile[WorldGen.dungeonX, num48 - 1].wall > 0 ||
				       Main.tile[WorldGen.dungeonX, num48 - 2].IsActive ||
				       Main.tile[WorldGen.dungeonX, num48 - 2].wall > 0 ||
				       Main.tile[WorldGen.dungeonX, num48 - 3].IsActive ||
				       Main.tile[WorldGen.dungeonX, num48 - 3].wall > 0 ||
				       Main.tile[WorldGen.dungeonX, num48 - 4].IsActive ||
				       Main.tile[WorldGen.dungeonX, num48 - 4].wall > 0)
				{
					num48--;
					if (num48 < 50)
						break;
				}

				if (num48 > 50) WorldGen.GrowDungeonTree(WorldGen.dungeonX, num48);
			}

			if (!WorldGen.drunkWorldGen)
			{
				int num49 = 100;
				if (num4 == 1)
				{
					int num50 = 0;
					for (int num51 = num6; num51 < num6 + num49; num51++)
					{
						num50++;
						for (int num52 = num8 + num50; num52 < num8 + num49; num52++)
						{
							Main.tile[num51, num52].LiquidAmount = 0;
							Main.tile[num51, num52 - 1].LiquidAmount = 0;
							Main.tile[num51, num52 - 2].LiquidAmount = 0;
							Main.tile[num51, num52 - 3].LiquidAmount = 0;
							if (!Main.wallDungeon[Main.tile[num51, num52].wall] && Main.tile[num51, num52].wall != 3 &&
							    Main.tile[num51, num52].wall != 83)
							{
								Main.tile[num51, num52].IsActive = true;
								Main.tile[num51, num52].type = tileType;
								Main.tile[num51, num52].Clear(TileDataType.Slope);
							}
						}
					}
				}
				else
				{
					int num53 = 0;
					for (int num54 = num5; num54 > num5 - num49; num54--)
					{
						num53++;
						for (int num55 = num8 + num53; num55 < num8 + num49; num55++)
						{
							Main.tile[num54, num55].LiquidAmount = 0;
							Main.tile[num54, num55 - 1].LiquidAmount = 0;
							Main.tile[num54, num55 - 2].LiquidAmount = 0;
							Main.tile[num54, num55 - 3].LiquidAmount = 0;
							if (!Main.wallDungeon[Main.tile[num54, num55].wall] && Main.tile[num54, num55].wall != 3 &&
							    Main.tile[num54, num55].wall != 83)
							{
								Main.tile[num54, num55].IsActive = true;
								Main.tile[num54, num55].type = tileType;
								Main.tile[num54, num55].Clear(TileDataType.Slope);
							}
						}
					}
				}
			}

			num17 = 1 + WorldGen.genRand.Next(2);
			num18 = 2 + WorldGen.genRand.Next(4);
			num19 = 0;
			num5 = (int) (vector.X - num2 * 0.5);
			num6 = (int) (vector.X + num2 * 0.5);
			if (WorldGen.drunkWorldGen)
			{
				if (num4 == 1)
				{
					num6--;
					num5--;
				}
				else
				{
					num5++;
					num6++;
				}
			}
			else
			{
				num5 += 2;
				num6 -= 2;
			}

			for (int num56 = num5; num56 < num6; num56++)
			{
				for (int num57 = num7; num57 < num8 + 1; num57++) WorldGen.PlaceWall(num56, num57, wallType, true);

				if (!WorldGen.drunkWorldGen)
				{
					num19++;
					if (num19 >= num18)
					{
						num56 += num18 * 2;
						num19 = 0;
					}
				}
			}

			if (WorldGen.drunkWorldGen)
			{
				num5 = (int) (vector.X - num2 * 0.5);
				num6 = (int) (vector.X + num2 * 0.5);
				if (num4 == 1)
					num5 = num6 - 3;
				else
					num6 = num5 + 3;

				for (int num58 = num5; num58 < num6; num58++)
				for (int num59 = num7; num59 < num8 + 1; num59++)
				{
					Main.tile[num58, num59].IsActive = true;
					Main.tile[num58, num59].type = tileType;
					Main.tile[num58, num59].Clear(TileDataType.Slope);
				}
			}

			vector.X -= (float) num2 * 0.6f * num4;
			vector.Y += (float) num3 * 0.5f;
			num2 = 15.0;
			num3 = 3.0;
			vector.Y -= (float) num3 * 0.5f;
			num5 = (int) (vector.X - num2 * 0.5);
			num6 = (int) (vector.X + num2 * 0.5);
			num7 = (int) (vector.Y - num3 * 0.5);
			num8 = (int) (vector.Y + num3 * 0.5);
			if (num5 < 0)
				num5 = 0;

			if (num6 > Main.maxTilesX)
				num6 = Main.maxTilesX;

			if (num7 < 0)
				num7 = 0;

			if (num8 > Main.maxTilesY)
				num8 = Main.maxTilesY;

			for (int num60 = num5; num60 < num6; num60++)
			for (int num61 = num7; num61 < num8; num61++)
				Main.tile[num60, num61].IsActive = false;

			if (num4 < 0)
				vector.X -= 1f;

			WorldGen.PlaceTile((int) vector.X, (int) vector.Y + 1, 10, true, false, -1, 13);
		}
	}
}