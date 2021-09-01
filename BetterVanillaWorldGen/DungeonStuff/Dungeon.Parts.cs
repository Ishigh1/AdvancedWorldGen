using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace AdvancedWorldGen.BetterVanillaWorldGen.DungeonStuff
{
	public static partial class Dungeon
	{
		public static void MakeDungeon_Traps()
		{
			int failCount = 0;
			const int failMax = 1000;
			int numAdd = 0;
			int yMin = (int) Math.Max(DungeonMinY, Main.worldSurface);
			while (numAdd < Main.maxTilesX / 500)
			{
				failCount++;
				int x1 = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int y1 = WorldGen.genRand.Next(yMin, DungeonMaxY);

				if (Main.wallDungeon[Main.tile[x1, y1].wall] && WorldGen.placeTrap(x1, y1, 0))
					failCount = failMax;

				if (failCount > failMax)
				{
					numAdd++;
					failCount = 0;
				}
			}
		}

		public static void MakeDungeon_Lights(ushort tileType, int[] roomWall)
		{
			int failCount = 0;
			int failMax = 1000;
			int numAdd = 0;
			int[] array =
			{
				WorldGen.genRand.Next(7), WorldGen.genRand.Next(6), WorldGen.genRand.Next(5)
			};

			if (array[1] >= array[0]) array[1]++;
			if (array[2] >= array[0]) array[2]++;
			if (array[2] >= array[1]) array[2]++;

			while (numAdd < Main.maxTilesX / 150)
			{
				failCount++;
				int num = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int num2 = WorldGen.genRand.Next(DungeonMinY, DungeonMaxY);
				if (Main.wallDungeon[Main.tile[num, num2].wall])
					for (int num3 = num2; num3 > DungeonMinY; num3--)
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
							if (WorldGen.genRand.NextBool(7))
							{
								int style = roomWall[0] switch
								{
									7 => 27,
									8 => 28,
									9 => 29,
									_ => 27
								};

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

							int style2;
							if (Main.tile[num, num3].wall == roomWall[1])
								style2 = array[0];
							else if (Main.tile[num, num3].wall == roomWall[2])
								style2 = array[1];
							else
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

		public static void MakeDungeon_Banners(int[] roomWall)
		{
			float count = 840000f / Main.maxTilesX;
			for (int i = 0; (float) i < count; i++)
			{
				int num = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int num2 = WorldGen.genRand.Next(DungeonMinY, DungeonMaxY);
				while (!Main.wallDungeon[Main.tile[num, num2].wall] || Main.tile[num, num2].IsActive)
				{
					num = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
					num2 = WorldGen.genRand.Next(DungeonMinY, DungeonMaxY);
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
		}

		public static void MakeDungeon_Pictures(int[] roomWall)
		{
			float count = 420000f / Main.maxTilesX;
			for (int i = 0; (float) i < count; i++)
			{
				int num = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int num2 = WorldGen.genRand.Next((int) Main.worldSurface, DungeonMaxY);
				while (!Main.wallDungeon[Main.tile[num, num2].wall] || Main.tile[num, num2].IsActive)
				{
					num = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
					num2 = WorldGen.genRand.Next((int) Main.worldSurface, DungeonMaxY);
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
		}

		public static void MakeDungeon_GroundFurniture(int wallType)
		{
			int num = (int) (2000 * Main.maxTilesX / 4200f);
			int num2 = 1 + Main.maxTilesX / 4200;
			int num3 = 1 + Main.maxTilesX / 4200;
			for (int i = 0; i < num; i++)
			{
				int num4 = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
				int j = WorldGen.genRand.Next((int) Main.worldSurface + 10, DungeonMaxY);
				while (!Main.wallDungeon[Main.tile[num4, j].wall] || Main.tile[num4, j].IsActive)
				{
					num4 = WorldGen.genRand.Next(DungeonMinX, DungeonMaxX);
					j = WorldGen.genRand.Next((int) Main.worldSurface + 10, DungeonMaxY);
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

				if (Main.tile[num7, j].wall is >= 94 and <= 105)
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
							if (WorldGen.genRand.NextBool(2) && !Main.tile[num22, j - 2].IsActive)
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

						if (WorldGen.genRand.NextBool(2))
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
							if (WorldGen.genRand.NextBool(2) && !Main.tile[n, j - 1].IsActive)
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
						if (WorldGen.genRand.NextBool(2))
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
						if (WorldGen.genRand.NextBool(2))
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
						if (WorldGen.genRand.NextBool(2))
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

				num2 = num2 switch
				{
					0 => 12,
					1 => 13,
					2 => 14,
					3 => 15,
					4 => 18,
					5 => 19,
					6 => 23,
					_ => num2
				};
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
			else if (x <= DungeonEntranceX)
				direction = 1;
			else
				direction = -1;

			Vector2 delta = new(direction, -1);
			if (WorldGen.genRand.Next(3) != 0)
				delta.X *= 1f + WorldGen.genRand.Next(0, 200) * 0.01f;
			else if (WorldGen.genRand.NextBool(3))
				delta.X *= WorldGen.genRand.Next(50, 76) * 0.01f;
			else if (WorldGen.genRand.NextBool(6))
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

				int num9 = (int) (currentPosition.X + (float) DungeonXStrength1 * 0.6f * num8 +
				                  (float) DungeonXStrength2 * num8);
				int num10 = (int) (DungeonYStrength2 * 0.5);
				if (currentPosition.Y < Main.worldSurface - 5.0 &&
				    Main.tile[num9, (int) (currentPosition.Y - step - 6.0 + num10)].wall == 0 &&
				    Main.tile[num9, (int) (currentPosition.Y - step - 7.0 + num10)].wall == 0 &&
				    Main.tile[num9, (int) (currentPosition.Y - step - 8.0 + num10)].wall == 0)
				{
					DungeonSurface = true;
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

				if (DungeonSurface)
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
			else if (WorldGen.genRand.NextBool(8))
			{
				flag2 = true;
			}

			for (int x = i - num4; x <= i + num4; x++)
			for (int y = num3; y <= num2 + num5; y++)
				if (Main.tileDungeon[Main.tile[x, y].type])
				{
					Main.tile[x, y].type = CrackedType;
					Main.tile[x, y].wall = (ushort) wallType;
				}

			for (int x = i - num6; x <= i + num6; x++)
			for (int y = num3; y <= num2 + num7; y++)
			{
				Main.tile[x, y].LiquidType = LiquidID.Water;
				Main.tile[x, y].LiquidAmount = 0;
				if (!Main.wallDungeon[Main.tile[x, y].wall] &&
				    Main.tile[x, y].type != CrackedType)
				{
					Main.tile[x, y].Clear(TileDataType.Slope);
					Main.tile[x, y].type = tileType;
					Main.tile[x, y].IsActive = true;
					if (x > i - num6 && x < i + num6 && y < num2 + num7)
						Main.tile[x, y].wall = (ushort) wallType;
				}
			}

			for (int num13 = i - num4; num13 <= i + num4; num13++)
			for (int num14 = num3; num14 <= num2 + num5; num14++)
				if (Main.tile[num13, num14].type != CrackedType)
				{
					if (flag2)
						Main.tile[num13, num14].LiquidAmount = byte.MaxValue;

					if (num13 == i - num4 || num13 == i + num4 || num14 == num2 + num5)
						Main.tile[num13, num14].type = TileID.Spikes;
					else if (num13 == i - num4 + 1 && num14 % 2 == 0 || num13 == i + num4 - 1 && num14 % 2 == 0 ||
					         num14 == num2 + num5 - 1 && num13 % 2 == 0)
						Main.tile[num13, num14].type = TileID.Spikes;
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
			Vector2 vector = new(i, j);
			int num4 = WorldGen.genRand.Next(35, 80);
			bool flag = WorldGen.genRand.NextBool(5);

			if (forceX)
			{
				num4 += 20;
				WorldGen.lastDungeonHall = Vector2.Zero;
			}
			else if (WorldGen.genRand.NextBool(5))
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
						if (WorldGen.genRand.NextBool(2))
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
					if (WorldGen.genRand.NextBool(2))
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
					if (WorldGen.genRand.NextBool(3))
					{
						if (WorldGen.genRand.NextBool(2))
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
						if (WorldGen.genRand.NextBool(2))
							zero.X = WorldGen.genRand.Next(10, 20) * 0.1f;
						else
							zero.X = -WorldGen.genRand.Next(10, 20) * 0.1f;
					}
					else if (WorldGen.genRand.NextBool(2))
					{
						if (WorldGen.genRand.NextBool(2))
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
					if (WorldGen.genRand.NextBool(3))
					{
						if (WorldGen.genRand.NextBool(2))
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
					if (WorldGen.genRand.NextBool(3))
					{
						if (WorldGen.genRand.NextBool(2))
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
					if (WorldGen.genRand.NextBool(2))
					{
						if (WorldGen.genRand.NextBool(2))
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
						if (WorldGen.genRand.NextBool(2))
							zero.X = WorldGen.genRand.Next(10, 20) * 0.1f;
						else
							zero.X = -WorldGen.genRand.Next(10, 20) * 0.1f;
					}
					else if (WorldGen.genRand.NextBool(2))
					{
						if (WorldGen.genRand.NextBool(2))
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
					if (WorldGen.genRand.NextBool(3))
					{
						if (WorldGen.genRand.NextBool(2))
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
					if (WorldGen.genRand.NextBool(3))
					{
						if (WorldGen.genRand.NextBool(2))
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
				int xMin = (int) Math.Max(vector.X - num - 4.0 - WorldGen.genRand.Next(6), 0);
				int xMax = (int) Math.Min(vector.X + num + 4.0 + WorldGen.genRand.Next(6), Main.maxTilesX);
				int yMin = (int) Math.Max(vector.Y - num - 4.0 - WorldGen.genRand.Next(6), 0);
				int yMax = (int) Math.Min(vector.Y + num + 4.0 + WorldGen.genRand.Next(6), Main.maxTilesY);

				DungeonMinX = Math.Min(xMin, DungeonMinX);
				DungeonMaxX = Math.Max(xMax, DungeonMaxX);
				DungeonMaxY = Math.Max(yMax, DungeonMaxY);
				for (int x = xMin; x < xMax; x++)
				for (int y = yMin; y < yMax; y++)
				{
					Main.tile[x, y].LiquidAmount = 0;
					if (!Main.wallDungeon[Main.tile[x, y].wall])
					{
						Main.tile[x, y].IsActive = true;
						Main.tile[x, y].type = tileType;
						Main.tile[x, y].Clear(TileDataType.Slope);
					}
				}

				for (int x = xMin + 1; x < xMax - 1; x++)
				for (int y = yMin + 1; y < yMax - 1; y++)
					Main.tile[x, y].wall = (ushort) wallType;

				int num16 = 0;
				if (zero.Y == 0f && WorldGen.genRand.Next((int) num + 1) == 0)
					num16 = WorldGen.genRand.Next(1, 3);
				else if (zero.X == 0f && WorldGen.genRand.Next((int) num - 1) == 0)
					num16 = WorldGen.genRand.Next(1, 3);
				else if (WorldGen.genRand.Next((int) num * 3) == 0)
					num16 = WorldGen.genRand.Next(1, 3);

				xMin = (int) Math.Max(0, vector.X - num * 0.5 - num16);
				xMax = (int) Math.Min(Main.maxTilesX, vector.X + num * 0.5 + num16);
				yMin = (int) Math.Max(0, vector.Y - num * 0.5 - num16);
				yMax = (int) Math.Min(Main.maxTilesY, vector.Y + num * 0.5 + num16);

				for (int x = xMin; x < xMax; x++)
				for (int y = yMin; y < yMax; y++)
				{
					Main.tile[x, y].Clear(TileDataType.Slope);
					if (flag)
					{
						if (Main.tile[x, y].IsActive || Main.tile[x, y].wall != wallType)
						{
							Main.tile[x, y].IsActive = true;
							Main.tile[x, y].type = CrackedType;
						}
					}
					else
					{
						Main.tile[x, y].IsActive = false;
					}

					Main.tile[x, y].Clear(TileDataType.Slope);
					Main.tile[x, y].wall = (ushort) wallType;
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

		public static void DungeonRoom(int i, int j, ushort tileType, ushort wallType)
		{
			int num = WorldGen.genRand.Next(15, 30);
			Vector2 vector = new()
			{
				X = WorldGen.genRand.Next(-10, 11) * 0.1f,
				Y = WorldGen.genRand.Next(-10, 11) * 0.1f
			};
			Vector2 vector2 = new()
			{
				X = i,
				Y = j - num / 2f
			};
			int num2 = WorldGen.genRand.Next(10, 20);
			double num3 = vector2.X;
			double num4 = vector2.X;
			double num5 = vector2.Y;
			double num6 = vector2.Y;
			while (num2 > 0)
			{
				num2--;
				int xMin = (int) Math.Max(vector2.X - num * 0.8 - 5.0, DungeonMinX);
				int xMax = (int) Math.Min(vector2.X + num * 0.8 + 5.0, DungeonMaxX);
				int yMin = (int) Math.Max(vector2.Y - num * 0.8 - 5.0, 0);
				int yMax = (int) Math.Min(vector2.Y + num * 0.8 + 5.0, DungeonMaxX);

				for (int x = xMin; x < xMax; x++)
				for (int y = yMin; y < yMax; y++)
				{
					Main.tile[x, y].LiquidAmount = 0;
					if (!Main.wallDungeon[Main.tile[x, y].wall])
					{
						Main.tile[x, y].Clear(TileDataType.Slope);
						Main.tile[x, y].IsActive = true;
						Main.tile[x, y].type = tileType;
					}
				}

				for (int m = xMin + 1; m < xMax - 1; m++)
				for (int n = yMin + 1; n < yMax - 1; n++)
					Main.tile[m, n].wall = wallType;

				xMin = (int) Math.Max(vector2.X - num * 0.5, xMin);
				xMax = (int) Math.Min(vector2.X + num * 0.5, xMax);
				yMin = (int) Math.Max(vector2.Y - num * 0.5, yMin);
				yMax = (int) Math.Min(vector2.Y + num * 0.5, yMax);
				for (int x = xMin; x < xMax; x++)
				for (int y = yMin; y < yMax; y++)
				{
					Main.tile[x, y].IsActive = false;
					Main.tile[x, y].wall = wallType;
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
			DungeonRoomSize.Add(num);
			DungeonRoomL.Add((int) num3);
			DungeonRoomR.Add((int) num4);
			DungeonRoomT.Add((int) num5);
			DungeonRoomB.Add((int) num6);
		}

		public static void DungeonEntrance(int x, int y, ushort tileType, int wallType)
		{
			const int spread = 60;
			for (int x1 = x - spread; x1 < x + spread; x1++)
			for (int y1 = y - spread; y1 < y + spread; y1++)
			{
				Main.tile[x1, y1].LiquidAmount = 0;
				Main.tile[x1, y1].LiquidType = LiquidID.Water;
				Main.tile[x1, y1].Clear(TileDataType.Slope);
			}

			double dungeonXStrength1 = DungeonXStrength1;
			double dungeonYStrength1 = DungeonYStrength1;
			DungeonMinY = (int) (y - (float) dungeonYStrength1 / 2f);
			Vector2 vector = new(x, DungeonMinY);
			int num4;
			if (x > Main.maxTilesX / 2)
				num4 = -1;
			else
				num4 = 1;

			if (WorldGen.drunkWorldGen || WorldGen.getGoodWorldGen)
				num4 *= -1;

			int xMin = (int) Math.Max(vector.X - dungeonXStrength1 * 0.6 - WorldGen.genRand.Next(2, 5), 0);
			int xMax = (int) Math.Min(vector.X + dungeonXStrength1 * 0.6 + WorldGen.genRand.Next(2, 5), Main.maxTilesX);
			int yMin = (int) Math.Max(vector.Y - dungeonYStrength1 * 0.6 - WorldGen.genRand.Next(2, 5), 0);
			int yMax = (int) Math.Min(vector.Y + dungeonYStrength1 * 0.6 + WorldGen.genRand.Next(8, 16),
				Main.maxTilesY);

			for (int x1 = xMin; x1 < xMax; x1++)
			for (int y1 = yMin; y1 < yMax; y1++)
			{
				Main.tile[x1, y1].LiquidAmount = 0;
				if (Main.tile[x1, y1].wall != wallType)
				{
					Main.tile[x1, y1].wall = 0;
					if (x1 > xMin + 1 && x1 < xMax - 2 && y1 > yMin + 1 && y1 < yMax - 2)
						Main.tile[x1, y1].wall = (ushort) wallType;

					Main.tile[x1, y1].IsActive = true;
					Main.tile[x1, y1].type = tileType;
					Main.tile[x1, y1].Clear(TileDataType.Slope);
				}
			}

			int num9 = xMin;
			int num10 = xMin + 5 + WorldGen.genRand.Next(4);
			int num11 = yMin - 3 - WorldGen.genRand.Next(3);
			int num12 = yMin;
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

			num9 = xMax - 5 - WorldGen.genRand.Next(4);
			num10 = xMax;
			num11 = yMin - 3 - WorldGen.genRand.Next(3);
			num12 = yMin;
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
			for (int num20 = xMin; num20 < xMax; num20++)
			{
				for (int num21 = yMin - num17; num21 < yMin; num21++)
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

			for (int num22 = xMin; num22 < xMax; num22++)
			for (int num23 = yMax; (double) num23 < Main.worldSurface; num23++)
			{
				Main.tile[num22, num23].LiquidAmount = 0;
				if (!Main.wallDungeon[Main.tile[num22, num23].wall])
				{
					Main.tile[num22, num23].IsActive = true;
					Main.tile[num22, num23].type = tileType;
				}

				if (num22 > xMin && num22 < xMax - 1)
					Main.tile[num22, num23].wall = (ushort) wallType;

				Main.tile[num22, num23].Clear(TileDataType.Slope);
			}

			xMin = (int) (vector.X - dungeonXStrength1 * 0.6);
			xMax = (int) (vector.X + dungeonXStrength1 * 0.6);
			yMin = (int) (vector.Y - dungeonYStrength1 * 0.6);
			yMax = (int) (vector.Y + dungeonYStrength1 * 0.6);
			if (xMin < 0)
				xMin = 0;

			if (xMax > Main.maxTilesX)
				xMax = Main.maxTilesX;

			if (yMin < 0)
				yMin = 0;

			if (yMax > Main.maxTilesY)
				yMax = Main.maxTilesY;

			for (int num24 = xMin; num24 < xMax; num24++)
			for (int num25 = yMin; num25 < yMax; num25++)
			{
				Main.tile[num24, num25].LiquidAmount = 0;
				Main.tile[num24, num25].wall = (ushort) wallType;
				Main.tile[num24, num25].Clear(TileDataType.Slope);
			}

			xMin = (int) (vector.X - dungeonXStrength1 * 0.6 - 1.0);
			xMax = (int) (vector.X + dungeonXStrength1 * 0.6 + 1.0);
			yMin = (int) (vector.Y - dungeonYStrength1 * 0.6 - 1.0);
			yMax = (int) (vector.Y + dungeonYStrength1 * 0.6 + 1.0);
			if (xMin < 0)
				xMin = 0;

			if (xMax > Main.maxTilesX)
				xMax = Main.maxTilesX;

			if (yMin < 0)
				yMin = 0;

			if (yMax > Main.maxTilesY)
				yMax = Main.maxTilesY;

			if (WorldGen.drunkWorldGen)
				xMin -= 4;

			for (int num26 = xMin; num26 < xMax; num26++)
			for (int num27 = yMin; num27 < yMax; num27++)
			{
				Main.tile[num26, num27].LiquidAmount = 0;
				Main.tile[num26, num27].wall = (ushort) wallType;
				Main.tile[num26, num27].Clear(TileDataType.Slope);
			}

			xMin = (int) (vector.X - dungeonXStrength1 * 0.5);
			xMax = (int) (vector.X + dungeonXStrength1 * 0.5);
			yMin = (int) (vector.Y - dungeonYStrength1 * 0.5);
			yMax = (int) (vector.Y + dungeonYStrength1 * 0.5);
			if (xMin < 0)
				xMin = 0;

			if (xMax > Main.maxTilesX)
				xMax = Main.maxTilesX;

			if (yMin < 0)
				yMin = 0;

			if (yMax > Main.maxTilesY)
				yMax = Main.maxTilesY;

			for (int num28 = xMin; num28 < xMax; num28++)
			for (int num29 = yMin; num29 < yMax; num29++)
			{
				Main.tile[num28, num29].LiquidAmount = 0;
				Main.tile[num28, num29].IsActive = false;
				Main.tile[num28, num29].wall = (ushort) wallType;
			}

			int num30 = (int) vector.X;
			int num31 = yMax;
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

			vector.X += (float) dungeonXStrength1 * 0.6f * num4;
			vector.Y += (float) dungeonYStrength1 * 0.5f;
			dungeonXStrength1 = DungeonXStrength2;
			dungeonYStrength1 = DungeonYStrength2;
			vector.X += (float) dungeonXStrength1 * 0.55f * num4;
			vector.Y -= (float) dungeonYStrength1 * 0.5f;
			xMin = (int) (vector.X - dungeonXStrength1 * 0.6 - WorldGen.genRand.Next(1, 3));
			xMax = (int) (vector.X + dungeonXStrength1 * 0.6 + WorldGen.genRand.Next(1, 3));
			yMin = (int) (vector.Y - dungeonYStrength1 * 0.6 - WorldGen.genRand.Next(1, 3));
			yMax = (int) (vector.Y + dungeonYStrength1 * 0.6 + WorldGen.genRand.Next(6, 16));
			if (xMin < 0)
				xMin = 0;

			if (xMax > Main.maxTilesX)
				xMax = Main.maxTilesX;

			if (yMin < 0)
				yMin = 0;

			if (yMax > Main.maxTilesY)
				yMax = Main.maxTilesY;

			for (int num33 = xMin; num33 < xMax; num33++)
			for (int num34 = yMin; num34 < yMax; num34++)
			{
				Main.tile[num33, num34].LiquidAmount = 0;
				if (Main.tile[num33, num34].wall == wallType)
					continue;

				bool flag = true;
				if (num4 < 0)
				{
					if (num33 < vector.X - dungeonXStrength1 * 0.5)
						flag = false;
				}
				else if (num33 > vector.X + dungeonXStrength1 * 0.5 - 1.0)
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

			for (int num35 = xMin; num35 < xMax; num35++)
			for (int num36 = yMax; (double) num36 < Main.worldSurface; num36++)
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

			xMin = (int) (vector.X - dungeonXStrength1 * 0.5);
			xMax = (int) (vector.X + dungeonXStrength1 * 0.5);
			num9 = xMin;
			if (num4 < 0)
				num9++;

			num10 = num9 + 5 + WorldGen.genRand.Next(4);
			num11 = yMin - 3 - WorldGen.genRand.Next(3);
			num12 = yMin;
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

			num9 = xMax - 5 - WorldGen.genRand.Next(4);
			num10 = xMax;
			num11 = yMin - 3 - WorldGen.genRand.Next(3);
			num12 = yMin;
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
				xMax++;

			for (int num41 = xMin + 1; num41 < xMax - 1; num41++)
			{
				for (int num42 = yMin - num17; num42 < yMin; num42++)
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
				xMin = (int) (vector.X - dungeonXStrength1 * 0.6);
				xMax = (int) (vector.X + dungeonXStrength1 * 0.6);
				yMin = (int) (vector.Y - dungeonYStrength1 * 0.6);
				yMax = (int) (vector.Y + dungeonYStrength1 * 0.6);
				if (xMin < 0)
					xMin = 0;

				if (xMax > Main.maxTilesX)
					xMax = Main.maxTilesX;

				if (yMin < 0)
					yMin = 0;

				if (yMax > Main.maxTilesY)
					yMax = Main.maxTilesY;

				for (int num43 = xMin; num43 < xMax; num43++)
				for (int num44 = yMin; num44 < yMax; num44++)
				{
					Main.tile[num43, num44].LiquidAmount = 0;
					Main.tile[num43, num44].wall = 0;
				}
			}

			xMin = (int) (vector.X - dungeonXStrength1 * 0.5);
			xMax = (int) (vector.X + dungeonXStrength1 * 0.5);
			yMin = (int) (vector.Y - dungeonYStrength1 * 0.5);
			yMax = (int) (vector.Y + dungeonYStrength1 * 0.5);
			if (xMin < 0)
				xMin = 0;

			if (xMax > Main.maxTilesX)
				xMax = Main.maxTilesX;

			if (yMin < 0)
				yMin = 0;

			if (yMax > Main.maxTilesY)
				yMax = Main.maxTilesY;

			for (int num45 = xMin; num45 < xMax; num45++)
			for (int num46 = yMin; num46 < yMax; num46++)
			{
				Main.tile[num45, num46].LiquidAmount = 0;
				Main.tile[num45, num46].IsActive = false;
				Main.tile[num45, num46].wall = 0;
			}

			Main.dungeonX = (int) vector.X;
			Main.dungeonY = yMax;
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
					for (int num51 = xMax; num51 < xMax + num49; num51++)
					{
						num50++;
						for (int num52 = yMax + num50; num52 < yMax + num49; num52++)
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
					for (int num54 = xMin; num54 > xMin - num49; num54--)
					{
						num53++;
						for (int num55 = yMax + num53; num55 < yMax + num49; num55++)
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
			xMin = (int) (vector.X - dungeonXStrength1 * 0.5);
			xMax = (int) (vector.X + dungeonXStrength1 * 0.5);
			if (WorldGen.drunkWorldGen)
			{
				if (num4 == 1)
				{
					xMax--;
					xMin--;
				}
				else
				{
					xMin++;
					xMax++;
				}
			}
			else
			{
				xMin += 2;
				xMax -= 2;
			}

			for (int num56 = xMin; num56 < xMax; num56++)
			{
				for (int num57 = yMin; num57 < yMax + 1; num57++) WorldGen.PlaceWall(num56, num57, wallType, true);

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
				xMin = (int) (vector.X - dungeonXStrength1 * 0.5);
				xMax = (int) (vector.X + dungeonXStrength1 * 0.5);
				if (num4 == 1)
					xMin = xMax - 3;
				else
					xMax = xMin + 3;

				for (int num58 = xMin; num58 < xMax; num58++)
				for (int num59 = yMin; num59 < yMax + 1; num59++)
				{
					Main.tile[num58, num59].IsActive = true;
					Main.tile[num58, num59].type = tileType;
					Main.tile[num58, num59].Clear(TileDataType.Slope);
				}
			}

			vector.X -= (float) dungeonXStrength1 * 0.6f * num4;
			vector.Y += (float) dungeonYStrength1 * 0.5f;
			dungeonXStrength1 = 15.0;
			dungeonYStrength1 = 3.0;
			vector.Y -= (float) dungeonYStrength1 * 0.5f;
			xMin = (int) Math.Max(vector.X - dungeonXStrength1 * 0.5, 0);
			xMax = (int) Math.Min(vector.X + dungeonXStrength1 * 0.5, Main.maxTilesX);
			yMin = (int) Math.Max(vector.Y - dungeonYStrength1 * 0.5, 0);
			yMax = (int) Math.Min(vector.Y + dungeonYStrength1 * 0.5, Main.maxTilesY);

			for (int num60 = xMin; num60 < xMax; num60++)
			for (int num61 = yMin; num61 < yMax; num61++)
				Main.tile[num60, num61].IsActive = false;

			if (num4 < 0)
				vector.X -= 1f;

			WorldGen.PlaceTile((int) vector.X, (int) vector.Y + 1, 10, true, false, -1, 13);
		}
	}
}