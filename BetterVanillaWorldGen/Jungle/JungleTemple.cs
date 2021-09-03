using System;
using System.Collections.Generic;
using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
using AdvancedWorldGen.Helper;
using AdvancedWorldGen.SpecialOptions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Jungle
{
	public class JungleTemple : ControlledWorldGenPass
	{
		public JungleTemple() : base("Jungle Temple", 595.8422f)
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration passConfig)
		{
			progress.Message = Language.GetTextValue("LegacyWorldGen.70");
			int jungleX = Replacer.VanillaInterface.JungleX;
			int minX = Math.Max(10, jungleX - Main.maxTilesX / 8);
			int maxX = Math.Min(Main.maxTilesX - 10, jungleX + Main.maxTilesX / 8);
			int x = Random.Next(minX, maxX);
			int y = Random.Next((int) WorldGen.rockLayer, Main.UnderworldLayer);
			(x, _) = TileFinder.SpiralSearch(x, y, IsValid);

			MakeTemple(progress, x);
		}

		public static bool IsValid(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			if (tile.IsActive && tile.type == TileID.JungleGrass)
				return true;

			return tile.wall is WallID.Jungle or WallID.JungleUnsafe or WallID.JungleUnsafe1 or WallID.JungleUnsafe2 or WallID.JungleUnsafe3 or WallID.JungleUnsafe4;
		}

		public void MakeTemple(GenerationProgress generationProgress, int templeX)
		{
			List<Rectangle> rooms = new();
			float worldSize = Main.maxTilesX / 4200f;
			int ignored = 0;
			int templeRoomCount = ClassicOptions.GetTempleRooms(ref ignored, worldSize);

			int direction = 1;
			if (Random.NextBool(2))
				direction = -1;

			int num4 = direction;
			int num7 = templeX;
			int height = 0;
			int num9 = Random.Next(1, 3);
			int num10 = 0;
			for (int i = 0; i < templeRoomCount; i++)
			{
				generationProgress.SetProgress(i, templeRoomCount, 1 / 12f);
				num10++;
				int num11 = direction;
				int num12 = num7;
				int tempHeight = height;
				bool flag = true;
				int num14 = 0;
				int num15 = 0;
				int num16 = -10;
				Rectangle rectangle = new(num12 - num14 / 2, tempHeight - num15 / 2, num14, num15);
				while (flag)
				{
					num12 = num7;
					tempHeight = height;
					num14 = Random.Next(25, 50);
					num15 = Random.Next(20, 35);
					if (num15 > num14)
						num15 = num14;

					if (i == templeRoomCount - 1)
					{
						num14 = Random.Next(55, 65);
						num15 = Random.Next(45, 50);
						if (num15 > num14)
							num15 = num14;

						num14 = (int) (num14 * 1.6);
						num15 = (int) (num15 * 1.35);
						tempHeight += Random.Next(5, 10);
					}

					if (num10 > num9)
					{
						tempHeight += Random.Next(num15 + 1, num15 + 3) + num16;
						num12 += Random.Next(-5, 6);
						num11 = direction * -1;
					}
					else
					{
						num12 += (Random.Next(num14 + 1, num14 + 3) + num16) * num11;
						tempHeight += Random.Next(-5, 6);
					}

					flag = false;
					rectangle = new Rectangle(num12 - num14 / 2, tempHeight - num15 / 2, num14, num15);
					for (int j = 0; j < i; j++)
					{
						if (rectangle.Intersects(rooms[j]))
							flag = true;

						if (Random.NextBool(100))
							num16++;
					}
				}

				if (num10 > num9)
				{
					num9++;
					num10 = 1;
				}

				rooms.Add(rectangle);
				direction = num11;
				num7 = num12;
				height = tempHeight;
			}

			int templeY;
			if (height > Main.UnderworldLayer - WorldGen.rockLayer)
				templeY = Main.UnderworldLayer - height - Random.Next(Main.UnderworldLayer - WorldGen.lavaLine);
			else
				templeY = Random.Next((int) WorldGen.rockLayer, Main.UnderworldLayer - height);
			for (int index = 0; index < rooms.Count; index++)
			{
				generationProgress.SetProgress(index, rooms.Count, 1 / 12f, 1 / 12f);
				Rectangle rectangle = rooms[index];
				rectangle.Y += templeY;
				rooms[index] = rectangle;
			}

			generationProgress.Value = 0.2f;

			for (int k = 0; k < templeRoomCount; k++)
			{
				generationProgress.SetProgress(k, templeRoomCount, 1 / 12f, 2 / 12f);
				for (int l = 0; l < 2; l++)
				for (int m = 0; m < templeRoomCount; m++)
				for (int n = 0; n < 2; n++)
				{
					int num17 = rooms[k].X;
					if (l == 1)
						num17 += rooms[k].Width - 1;

					int num18 = rooms[k].Y;
					int num19 = num18 + rooms[k].Height;
					int num20 = rooms[m].X;
					if (n == 1)
						num20 += rooms[m].Width - 1;

					int y2 = rooms[m].Y;
					int num21 = y2 + rooms[m].Height;
					while (num17 != num20 || num18 != y2 || num19 != num21)
					{
						if (num17 < num20)
							num17++;

						if (num17 > num20)
							num17--;

						if (num18 < y2)
							num18++;

						if (num18 > y2)
							num18--;

						if (num19 < num21)
							num19++;

						if (num19 > num21)
							num19--;

						int num22 = num17;
						for (int num23 = num18; num23 < num19; num23++)
						{
							Main.tile[num22, num23].IsActive = true;
							Main.tile[num22, num23].type = TileID.LihzahrdBrick;
							Main.tile[num22, num23].LiquidAmount = 0;
							Main.tile[num22, num23].Slope = SlopeType.Solid;
							Main.tile[num22, num23].IsHalfBlock = false;
						}
					}
				}
			}

			for (int num24 = 0; num24 < templeRoomCount; num24++)
			{
				generationProgress.SetProgress(num24, templeRoomCount, 1 / 12f, 3 / 12f);
				for (int num25 = rooms[num24].X; num25 < rooms[num24].X + rooms[num24].Width; num25++)
				for (int num26 = rooms[num24].Y; num26 < rooms[num24].Y + rooms[num24].Height; num26++)
				{
					Main.tile[num25, num26].IsActive = true;
					Main.tile[num25, num26].type = TileID.LihzahrdBrick;
					Main.tile[num25, num26].LiquidAmount = 0;
					Main.tile[num25, num26].Slope = SlopeType.Solid;
					Main.tile[num25, num26].IsHalfBlock = false;
				}

				int x2 = rooms[num24].X;
				int num27 = x2 + rooms[num24].Width;
				int y3 = rooms[num24].Y;
				int num28 = y3 + rooms[num24].Height;
				x2 += Random.Next(3, 8);
				num27 -= Random.Next(3, 8);
				y3 += Random.Next(3, 8);
				num28 -= Random.Next(3, 8);
				int num29 = x2;
				int num30 = num27;
				int num31 = y3;
				int num32 = num28;
				int num33 = (x2 + num27) / 2;
				int num34 = (y3 + num28) / 2;
				for (int num35 = x2; num35 < num27; num35++)
				for (int num36 = y3; num36 < num28; num36++)
				{
					if (Random.NextBool(20))
						num31 += Random.Next(-1, 2);

					if (Random.NextBool(20))
						num32 += Random.Next(-1, 2);

					if (Random.NextBool(20))
						num29 += Random.Next(-1, 2);

					if (Random.NextBool(20))
						num30 += Random.Next(-1, 2);

					if (num29 < x2)
						num29 = x2;

					if (num30 > num27)
						num30 = num27;

					if (num31 < y3)
						num31 = y3;

					if (num32 > num28)
						num32 = num28;

					if (num29 > num33)
						num29 = num33;

					if (num30 < num33)
						num30 = num33;

					if (num31 > num34)
						num31 = num34;

					if (num32 < num34)
						num32 = num34;

					if (num35 >= num29 && num35 < num30 && num36 >= num31 && num36 <= num32)
					{
						Main.tile[num35, num36].IsActive = false;
						Main.tile[num35, num36].wall = 87;
					}
				}

				for (int num37 = num28; num37 > y3; num37--)
				for (int num38 = num27; num38 > x2; num38--)
				{
					if (Random.NextBool(20))
						num31 += Random.Next(-1, 2);

					if (Random.NextBool(20))
						num32 += Random.Next(-1, 2);

					if (Random.NextBool(20))
						num29 += Random.Next(-1, 2);

					if (Random.NextBool(20))
						num30 += Random.Next(-1, 2);

					if (num29 < x2)
						num29 = x2;

					if (num30 > num27)
						num30 = num27;

					if (num31 < y3)
						num31 = y3;

					if (num32 > num28)
						num32 = num28;

					if (num29 > num33)
						num29 = num33;

					if (num30 < num33)
						num30 = num33;

					if (num31 > num34)
						num31 = num34;

					if (num32 < num34)
						num32 = num34;

					if (num38 >= num29 && num38 < num30 && num37 >= num31 && num37 <= num32)
					{
						Main.tile[num38, num37].IsActive = false;
						Main.tile[num38, num37].wall = 87;
					}
				}
			}

			Vector2 templePath = new(templeX, templeY);
			for (int num39 = 0; num39 < templeRoomCount; num39++)
			{
				generationProgress.SetProgress(num39, templeRoomCount, 1 / 12f, 4 / 12f);
				Rectangle rectangle2 = rooms[num39];
				rectangle2.X += 8;
				rectangle2.Y += 8;
				rectangle2.Width -= 16;
				rectangle2.Height -= 16;
				bool flag2 = true;
				while (flag2)
				{
					int num40 = Random.Next(rectangle2.X, rectangle2.X + rectangle2.Width);
					int num41 = Random.Next(rectangle2.Y, rectangle2.Y + rectangle2.Height);
					if (num39 == templeRoomCount - 1)
					{
						num40 = rectangle2.X + rectangle2.Width / 2 + Random.Next(-10, 10);
						num41 = rectangle2.Y + rectangle2.Height / 2 + Random.Next(-10, 10);
					}

					templePath = WorldGen.templePather(templePath, num40, num41);
					if (templePath.X == num40 && templePath.Y == num41)
						flag2 = false;
				}

				if (num39 >= templeRoomCount - 1)
					continue;

				if (Random.Next(3) != 0)
				{
					int num42 = num39 + 1;
					if (rooms[num42].Y >= rooms[num39].Y + rooms[num39].Height)
					{
						rectangle2.X = rooms[num42].X;
						if (num39 == 0)
						{
							if (direction > 0)
								rectangle2.X += (int) (rooms[num42].Width * 0.8);
							else
								rectangle2.X += (int) (rooms[num42].Width * 0.2);
						}
						else if (rooms[num42].X < rooms[num39].X)
						{
							rectangle2.X += (int) (rooms[num42].Width * 0.2);
						}
						else
						{
							rectangle2.X += (int) (rooms[num42].Width * 0.8);
						}

						rectangle2.Y = rooms[num42].Y;
					}
					else
					{
						rectangle2.X = (rooms[num39].X + rooms[num39].Width / 2 + rooms[num42].X +
						                rooms[num42].Width / 2) / 2;
						rectangle2.Y = (int) (rooms[num42].Y + rooms[num42].Height * 0.8);
					}

					int x3 = rectangle2.X;
					int y4 = rectangle2.Y;
					flag2 = true;
					while (flag2)
					{
						int num43 = Random.Next(x3 - 6, x3 + 7);
						int num44 = Random.Next(y4 - 6, y4 + 7);
						templePath = WorldGen.templePather(templePath, num43, num44);
						if (templePath.X == num43 && templePath.Y == num44)
							flag2 = false;
					}

					continue;
				}

				int num45 = num39 + 1;
				int num46 = (rooms[num39].X + rooms[num39].Width / 2 + rooms[num45].X + rooms[num45].Width / 2) / 2;
				int num47 = (rooms[num39].Y + rooms[num39].Height / 2 + rooms[num45].Y + rooms[num45].Height / 2) / 2;
				flag2 = true;
				while (flag2)
				{
					int num48 = Random.Next(num46 - 6, num46 + 7);
					int num49 = Random.Next(num47 - 6, num47 + 7);
					templePath = WorldGen.templePather(templePath, num48, num49);
					if (templePath.X == num48 && templePath.Y == num49)
						flag2 = false;
				}
			}

			int templeLeft = Main.maxTilesX - 20;
			int templeRight = 20;
			int templeTop = Main.maxTilesY - 20;
			int templeBottom = 20;
			for (int num54 = 0; num54 < templeRoomCount; num54++)
			{
				if (rooms[num54].X < templeLeft)
					templeLeft = rooms[num54].X;

				if (rooms[num54].X + rooms[num54].Width > templeRight)
					templeRight = rooms[num54].X + rooms[num54].Width;

				if (rooms[num54].Y < templeTop)
					templeTop = rooms[num54].Y;

				if (rooms[num54].Y + rooms[num54].Height > templeBottom)
					templeBottom = rooms[num54].Y + rooms[num54].Height;
			}

			templeLeft -= 10;
			templeRight += 10;
			templeTop -= 10;
			templeBottom += 10;
			for (int num55 = templeLeft; num55 < templeRight; num55++)
			{
				generationProgress.SetProgress(num55, templeRight, 1 / 12f, 5 / 12f);
				for (int num56 = templeTop; num56 < templeBottom; num56++) WorldGen.outerTempled(num55, num56);
			}

			for (int num57 = templeRight; num57 >= templeLeft; num57--)
			{
				generationProgress.SetProgress(num57, templeLeft, 1 / 12f, 6 / 12f);
				for (int num58 = templeTop; num58 < templeBottom / 2; num58++) WorldGen.outerTempled(num57, num58);
			}

			for (int num59 = templeTop; num59 < templeBottom; num59++)
			{
				generationProgress.SetProgress(num59, templeBottom, 1 / 12f, 7 / 12f);
				for (int num60 = templeLeft; num60 < templeRight; num60++) WorldGen.outerTempled(num60, num59);
			}

			for (int num61 = templeBottom; num61 >= templeTop; num61--)
			{
				generationProgress.SetProgress(num61, templeTop, 1 / 12f, 8 / 12f);
				for (int num62 = templeLeft; num62 < templeRight; num62++) WorldGen.outerTempled(num62, num61);
			}

			direction = -num4;
			Vector2 vector = new(templeX, templeY);
			int num63 = Random.Next(2, 5);
			bool flag3 = true;
			int num64 = 0;
			int num65 = Random.Next(9, 14);
			while (flag3)
			{
				num64++;
				if (num64 >= num65)
				{
					num64 = 0;
					vector.Y -= 1f;
				}

				vector.X += direction;
				int num66 = (int) vector.X;
				flag3 = false;
				for (int num67 = (int) vector.Y - num63; num67 < vector.Y + num63; num67++)
				{
					if (Main.tile[num66, num67].wall == 87 ||
					    Main.tile[num66, num67].IsActive && Main.tile[num66, num67].type == 226)
						flag3 = true;

					if (Main.tile[num66, num67].IsActive && Main.tile[num66, num67].type == 226)
					{
						Main.tile[num66, num67].IsActive = false;
						Main.tile[num66, num67].wall = 87;
					}
				}
			}

			int num68 = templeX;
			int num69;
			for (num69 = templeY; !Main.tile[num68, num69].IsActive; num69++)
			{
			}

			num69 -= 4;
			int num70 = num69;
			while (Main.tile[num68, num70].IsActive && Main.tile[num68, num70].type == 226 ||
			       Main.tile[num68, num70].wall == 87) num70--;

			num70 += 2;
			for (int num71 = num68 - 1; num71 <= num68 + 1; num71++)
			for (int num72 = num70; num72 <= num69; num72++)
			{
				Main.tile[num71, num72].IsActive = true;
				Main.tile[num71, num72].type = TileID.LihzahrdBrick;
				Main.tile[num71, num72].LiquidAmount = 0;
				Main.tile[num71, num72].Slope = SlopeType.Solid;
				Main.tile[num71, num72].IsHalfBlock = false;
			}

			for (int num73 = num68 - 4; num73 <= num68 + 4; num73++)
			for (int num74 = num69 - 1; num74 < num69 + 3; num74++)
			{
				Main.tile[num73, num74].IsActive = false;
				Main.tile[num73, num74].wall = 87;
			}

			for (int num75 = num68 - 1; num75 <= num68 + 1; num75++)
			for (int num76 = num69 - 5; num76 <= num69 + 8; num76++)
			{
				Main.tile[num75, num76].IsActive = true;
				Main.tile[num75, num76].type = TileID.LihzahrdBrick;
				Main.tile[num75, num76].LiquidAmount = 0;
				Main.tile[num75, num76].Slope = SlopeType.Solid;
				Main.tile[num75, num76].IsHalfBlock = false;
			}

			for (int num77 = num68 - 3; num77 <= num68 + 3; num77++)
			for (int num78 = num69 - 2; num78 < num69 + 3; num78++)
				if (num78 >= num69 || num77 < templeX - 1 || num77 > templeX + 1)
				{
					Main.tile[num77, num78].IsActive = false;
					Main.tile[num77, num78].wall = 87;
				}

			WorldGen.PlaceTile(num68, num69, 10, true, false, -1, 11);
			for (int num79 = templeLeft; num79 < templeRight; num79++)
			{
				generationProgress.SetProgress(num79, templeRight, 1 / 12f, 9 / 12f);
				for (int num80 = templeTop; num80 < templeBottom; num80++) WorldGen.templeCleaner(num79, num80);
			}

			for (int num81 = templeBottom; num81 >= templeTop; num81--)
			{
				generationProgress.SetProgress(num81, templeTop, 1 / 12f, 10 / 12f);
				for (int num82 = templeRight; num82 >= templeLeft; num82--) WorldGen.templeCleaner(num82, num81);
			}

			for (int num83 = templeLeft; num83 < templeRight; num83++)
			{
				generationProgress.SetProgress(num83, templeRight, 1 / 12f, 11 / 12f);
				for (int num84 = templeTop; num84 < templeBottom; num84++)
				{
					bool flag4 = true;
					for (int num85 = num83 - 1; flag4 && num85 <= num83 + 1; num85++)
					for (int num86 = num84 - 1; num86 <= num84 + 1; num86++)
						if ((!Main.tile[num85, num86].IsActive || Main.tile[num85, num86].type != 226) &&
						    Main.tile[num85, num86].wall != 87)
						{
							flag4 = false;
							break;
						}

					if (flag4)
						Main.tile[num83, num84].wall = 87;
				}
			}

			int tries = 0;
			Rectangle rectangle3 = rooms[templeRoomCount - 1];
			int num88 = rectangle3.Width / 2;
			int num89 = rectangle3.Height / 2;
			while (true)
			{
				tries++;
				int x = rectangle3.X + num88 + 15 - Random.Next(30);
				int y = rectangle3.Y + num89 + 15 - Random.Next(30);
				WorldGen.PlaceTile(x, y, TileID.LihzahrdAltar);
				if (Main.tile[x, y].type == TileID.LihzahrdAltar)
				{
					int lAltarX = x - Main.tile[x, y].frameX / 18;
					int lAltarY = y - Main.tile[x, y].frameY / 18;
					VanillaInterface.LAltarX.Value = lAltarX;
					VanillaInterface.LAltarY.Value = lAltarY;
					break;
				}

				if (tries < 1000)
					continue;

				x = rectangle3.X + num88;
				y = rectangle3.Y + num89;
				x += Random.Next(-10, 11);
				for (y += Random.Next(-10, 11); !Main.tile[x, y].IsActive; y++)
				{
				}

				Main.tile[x - 1, y].IsActive = true;
				Main.tile[x - 1, y].Slope = SlopeType.Solid;
				Main.tile[x - 1, y].IsHalfBlock = false;
				Main.tile[x - 1, y].type = TileID.LihzahrdBrick;
				Main.tile[x, y].IsActive = true;
				Main.tile[x, y].Slope = SlopeType.Solid;
				Main.tile[x, y].IsHalfBlock = false;
				Main.tile[x, y].type = TileID.LihzahrdBrick;
				Main.tile[x + 1, y].IsActive = true;
				Main.tile[x + 1, y].Slope = SlopeType.Solid;
				Main.tile[x + 1, y].IsHalfBlock = false;
				Main.tile[x + 1, y].type = TileID.LihzahrdBrick;
				y -= 2;
				x--;
				for (int num92 = -1; num92 <= 3; num92++)
				for (int num93 = -1; num93 <= 1; num93++)
				{
					templeX = x + num92;
					y += num93;
					Main.tile[templeX, y].IsActive = false;
				}

				int lAltarX2 = x;
				int lAltarY2 = y;
				VanillaInterface.LAltarX.Value = lAltarX2;
				VanillaInterface.LAltarY.Value = lAltarY2;
				for (int num94 = 0; num94 <= 2; num94++)
				for (int num95 = 0; num95 <= 1; num95++)
				{
					templeX = x + num94;
					y += num95;
					Main.tile[templeX, y].IsActive = true;
					Main.tile[templeX, y].type = TileID.LihzahrdAltar;
					Main.tile[templeX, y].frameX = (short) (num94 * 18);
					Main.tile[templeX, y].frameY = (short) (num95 * 18);
				}

				for (int num96 = 0; num96 <= 2; num96++)
				for (int num97 = 0; num97 <= 1; num97++)
				{
					templeX = x + num96;
					y += num97;
					WorldGen.SquareTileFrame(templeX, y);
				}

				break;
			}

			float num98 = templeRoomCount * 1.1f;
			num98 *= 1f + Random.Next(-25, 26) * 0.01f;
			if (WorldGen.drunkWorldGen)
				num98 *= 1.5f;

			int num99 = 0;
			while (num98 > 0f)
			{
				num99++;
				int roomIndex = Random.Next(templeRoomCount);
				int x = Random.Next(rooms[roomIndex].X, rooms[roomIndex].X + rooms[roomIndex].Width);
				int y = Random.Next(rooms[roomIndex].Y, rooms[roomIndex].Y + rooms[roomIndex].Height);
				if (Main.tile[x, y].wall == WallID.LihzahrdBrickUnsafe && !Main.tile[x, y].IsActive)
				{
					bool flag5 = false;
					if (Random.NextBool(2))
					{
						int directionY = 1;
						if (Random.NextBool(2))
							directionY = -1;

						for (; !Main.tile[x, y].IsActive; y += directionY)
						{
						}

						y -= directionY;
						bool num104 = Random.NextBool(2);
						int range = Random.Next(3, 10);
						bool found = true;
						for (int xx = x - range; xx < x + range; xx++)
						{
							for (int yy = y - range; yy < y + range; yy++)
								if (Main.tile[xx, yy].IsActive &&
								    Main.tile[xx, yy].type is TileID.ClosedDoor or TileID.LihzahrdAltar)
								{
									found = false;
									break;
								}

							if (found)
								break;
						}

						if (found)
							for (int xx = x - range; xx < x + range; xx++)
							for (int yy = y - range; yy < y + range; yy++)
							{
								if (!WorldGen.SolidTile(xx, yy) ||
								    Main.tile[xx, yy].type == TileID.WoodenSpikes ||
								    WorldGen.SolidTile(xx, yy - directionY))
									continue;

								Main.tile[xx, yy].type = TileID.WoodenSpikes;
								flag5 = true;
								if (num104)
								{
									Main.tile[xx, yy - 1].type = TileID.WoodenSpikes;
									Main.tile[xx, yy - 1].IsActive = true;
									if (WorldGen.drunkWorldGen)
									{
										Main.tile[xx, yy - 2].type = TileID.WoodenSpikes;
										Main.tile[xx, yy - 2].IsActive = true;
									}
								}
								else
								{
									Main.tile[xx, yy + 1].type = TileID.WoodenSpikes;
									Main.tile[xx, yy + 1].IsActive = true;
									if (WorldGen.drunkWorldGen)
									{
										Main.tile[xx, yy + 2].type = TileID.WoodenSpikes;
										Main.tile[xx, yy + 2].IsActive = true;
									}
								}

								num104 = !num104;
							}

						if (flag5)
						{
							num99 = 0;
							num98 -= 1f;
						}
					}
					else
					{
						int directionX = 1;
						if (Random.NextBool(2))
							directionX = -1;

						for (; !Main.tile[x, y].IsActive; x += directionX)
						{
						}

						x -= directionX;
						bool num111 = Random.NextBool(2);
						int range = Random.Next(3, 10);
						bool flag7 = true;
						for (int xx = x - range; xx < x + range; xx++)
						for (int yy = y - range; yy < y + range; yy++)
							if (Main.tile[xx, yy].IsActive && Main.tile[xx, yy].type == 10)
							{
								flag7 = false;
								break;
							}

						if (flag7)
							for (int xx = x - range; xx < x + range; xx++)
							for (int yy = y - range; yy < y + range; yy++)
							{
								if (!WorldGen.SolidTile(xx, yy) ||
								    Main.tile[xx, yy].type == TileID.WoodenSpikes ||
								    WorldGen.SolidTile(xx - directionX, yy))
									continue;

								Main.tile[xx, yy].type = TileID.WoodenSpikes;
								flag5 = true;
								if (num111)
								{
									Main.tile[xx - 1, yy].type = TileID.WoodenSpikes;
									Main.tile[xx - 1, yy].IsActive = true;
									if (WorldGen.drunkWorldGen)
									{
										Main.tile[xx - 2, yy].type = TileID.WoodenSpikes;
										Main.tile[xx - 2, yy].IsActive = true;
									}
								}
								else
								{
									Main.tile[xx + 1, yy].type = TileID.WoodenSpikes;
									Main.tile[xx + 1, yy].IsActive = true;
									if (WorldGen.drunkWorldGen)
									{
										Main.tile[xx - 2, yy].type = TileID.WoodenSpikes;
										Main.tile[xx - 2, yy].IsActive = true;
									}
								}

								num111 = !num111;
							}

						if (flag5)
						{
							num99 = 0;
							num98 -= 1f;
						}
					}
				}

				if (num99 > 1000)
				{
					num99 = 0;
					num98 -= 1f;
				}
			}

			WorldGen.tLeft = templeLeft;
			WorldGen.tRight = templeRight;
			WorldGen.tTop = templeTop;
			WorldGen.tBottom = templeBottom;
			WorldGen.tRooms = templeRoomCount;
		}
	}
}