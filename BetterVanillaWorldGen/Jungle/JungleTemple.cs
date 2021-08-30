using System;
using System.Collections.Generic;
using System.Reflection;
using AdvancedWorldGen.Helper;
using AdvancedWorldGen.SpecialOptions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
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
			progress.Message = Lang.gen[70].Value;
			int jungleX = Replacer.VanillaInterface.JungleX;
			int minX = Math.Max(10, jungleX - Main.maxTilesX / 8);
			int maxX = Math.Min(Main.maxTilesX - 10, jungleX + Main.maxTilesX / 8);
			int x = Random.Next(minX, maxX);
			int y = Random.Next((int) WorldGen.rockLayer, Main.UnderworldLayer);
			(x, y) = TileFinder.SpiralSearch(x, y, IsValid);

			makeTemple(progress, x);
		}

		public static bool IsValid(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			if (tile.IsActive && tile.type == TileID.JungleGrass)
				return true;
			else if (tile.wall == WallID.Jungle || tile.wall == WallID.JungleUnsafe ||
			         tile.wall == WallID.JungleUnsafe1 || tile.wall == WallID.JungleUnsafe2 ||
			         tile.wall == WallID.JungleUnsafe3 || tile.wall == WallID.JungleUnsafe4)
				return true;
			return false;
		}

		public void makeTemple(GenerationProgress generationProgress, int x)
		{
			List<Rectangle> rooms = new();
			float worldSize = Main.maxTilesX / 4200f;
			int ignored = 0;
			int num2 = ClassicOptions.GetTempleRooms(ref ignored, worldSize);

			int direction = 1;
			if (Random.Next(2) == 0)
				direction = -1;

			int num4 = direction;
			int num7 = x;
			int height = 0;
			int num9 = Random.Next(1, 3);
			int num10 = 0;
			for (int i = 0; i < num2; i++)
			{
				GenPassHelper.SetProgress(generationProgress, i, num2, 1 / 12f);
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

					if (i == num2 - 1)
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

						if (Random.Next(100) == 0)
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

			int y;
			if (height > Main.UnderworldLayer - WorldGen.rockLayer)
				y = Main.UnderworldLayer - height - Random.Next(Main.UnderworldLayer - WorldGen.lavaLine);
			else
				y = Random.Next((int) WorldGen.rockLayer, Main.UnderworldLayer - height);
			for (int index = 0; index < rooms.Count; index++)
			{
				GenPassHelper.SetProgress(generationProgress, index, rooms.Count, 1 / 12f, 1 / 12f);
				Rectangle rectangle = rooms[index];
				rectangle.Y += y;
				rooms[index] = rectangle;
			}

			generationProgress.Value = 0.2f;

			for (int k = 0; k < num2; k++)
			{
				GenPassHelper.SetProgress(generationProgress, k, num2, 1 / 12f, 2 / 12f);
				for (int l = 0; l < 2; l++)
				for (int m = 0; m < num2; m++)
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
							Main.tile[num22, num23].type = 226;
							Main.tile[num22, num23].LiquidAmount = 0;
							Main.tile[num22, num23].Slope = SlopeType.Solid;
							Main.tile[num22, num23].IsHalfBlock = false;
						}
					}
				}
			}

			for (int num24 = 0; num24 < num2; num24++)
			{
				GenPassHelper.SetProgress(generationProgress, num24, num2, 1 / 12f, 3 / 12f);
				for (int num25 = rooms[num24].X; num25 < rooms[num24].X + rooms[num24].Width; num25++)
				for (int num26 = rooms[num24].Y; num26 < rooms[num24].Y + rooms[num24].Height; num26++)
				{
					Main.tile[num25, num26].IsActive = true;
					Main.tile[num25, num26].type = 226;
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
					if (Random.Next(20) == 0)
						num31 += Random.Next(-1, 2);

					if (Random.Next(20) == 0)
						num32 += Random.Next(-1, 2);

					if (Random.Next(20) == 0)
						num29 += Random.Next(-1, 2);

					if (Random.Next(20) == 0)
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
					if (Random.Next(20) == 0)
						num31 += Random.Next(-1, 2);

					if (Random.Next(20) == 0)
						num32 += Random.Next(-1, 2);

					if (Random.Next(20) == 0)
						num29 += Random.Next(-1, 2);

					if (Random.Next(20) == 0)
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

			Vector2 templePath = new(x, y);
			for (int num39 = 0; num39 < num2; num39++)
			{
				GenPassHelper.SetProgress(generationProgress, num39, num2, 1 / 12f, 4 / 12f);
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
					if (num39 == num2 - 1)
					{
						num40 = rectangle2.X + rectangle2.Width / 2 + Random.Next(-10, 10);
						num41 = rectangle2.Y + rectangle2.Height / 2 + Random.Next(-10, 10);
					}

					templePath = WorldGen.templePather(templePath, num40, num41);
					if (templePath.X == num40 && templePath.Y == num41)
						flag2 = false;
				}

				if (num39 >= num2 - 1)
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

			int num50 = Main.maxTilesX - 20;
			int num51 = 20;
			int num52 = Main.maxTilesY - 20;
			int num53 = 20;
			for (int num54 = 0; num54 < num2; num54++)
			{
				if (rooms[num54].X < num50)
					num50 = rooms[num54].X;

				if (rooms[num54].X + rooms[num54].Width > num51)
					num51 = rooms[num54].X + rooms[num54].Width;

				if (rooms[num54].Y < num52)
					num52 = rooms[num54].Y;

				if (rooms[num54].Y + rooms[num54].Height > num53)
					num53 = rooms[num54].Y + rooms[num54].Height;
			}

			num50 -= 10;
			num51 += 10;
			num52 -= 10;
			num53 += 10;
			for (int num55 = num50; num55 < num51; num55++)
			{
				GenPassHelper.SetProgress(generationProgress, num55, num51, 1 / 12f, 5 / 12f);
				for (int num56 = num52; num56 < num53; num56++) WorldGen.outerTempled(num55, num56);
			}

			for (int num57 = num51; num57 >= num50; num57--)
			{
				GenPassHelper.SetProgress(generationProgress, num57, num50, 1 / 12f, 6 / 12f);
				for (int num58 = num52; num58 < num53 / 2; num58++) WorldGen.outerTempled(num57, num58);
			}

			for (int num59 = num52; num59 < num53; num59++)
			{
				GenPassHelper.SetProgress(generationProgress, num59, num53, 1 / 12f, 7 / 12f);
				for (int num60 = num50; num60 < num51; num60++) WorldGen.outerTempled(num60, num59);
			}

			for (int num61 = num53; num61 >= num52; num61--)
			{
				GenPassHelper.SetProgress(generationProgress, num61, num52, 1 / 12f, 8 / 12f);
				for (int num62 = num50; num62 < num51; num62++) WorldGen.outerTempled(num62, num61);
			}

			direction = -num4;
			Vector2 vector = new(x, y);
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
				for (int num67 = (int) vector.Y - num63; (float) num67 < vector.Y + (float) num63; num67++)
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

			int num68 = x;
			int num69;
			for (num69 = y; !Main.tile[num68, num69].IsActive; num69++)
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
				Main.tile[num71, num72].type = 226;
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
				Main.tile[num75, num76].type = 226;
				Main.tile[num75, num76].LiquidAmount = 0;
				Main.tile[num75, num76].Slope = SlopeType.Solid;
				Main.tile[num75, num76].IsHalfBlock = false;
			}

			for (int num77 = num68 - 3; num77 <= num68 + 3; num77++)
			for (int num78 = num69 - 2; num78 < num69 + 3; num78++)
				if (num78 >= num69 || num77 < x - 1 || num77 > x + 1)
				{
					Main.tile[num77, num78].IsActive = false;
					Main.tile[num77, num78].wall = 87;
				}

			WorldGen.PlaceTile(num68, num69, 10, true, false, -1, 11);
			for (int num79 = num50; num79 < num51; num79++)
			{
				GenPassHelper.SetProgress(generationProgress, num79, num51, 1 / 12f, 9 / 12f);
				for (int num80 = num52; num80 < num53; num80++) WorldGen.templeCleaner(num79, num80);
			}

			for (int num81 = num53; num81 >= num52; num81--)
			{
				GenPassHelper.SetProgress(generationProgress, num81, num52, 1 / 12f, 10 / 12f);
				for (int num82 = num51; num82 >= num50; num82--) WorldGen.templeCleaner(num82, num81);
			}

			for (int num83 = num50; num83 < num51; num83++)
			{
				GenPassHelper.SetProgress(generationProgress, num83, num51, 1 / 12f, 11 / 12f);
				for (int num84 = num52; num84 < num53; num84++)
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

			int num87 = 0;
			Rectangle rectangle3 = rooms[num2 - 1];
			int num88 = rectangle3.Width / 2;
			int num89 = rectangle3.Height / 2;
			while (true)
			{
				num87++;
				int num90 = rectangle3.X + num88 + 15 - Random.Next(30);
				int num91 = rectangle3.Y + num89 + 15 - Random.Next(30);
				WorldGen.PlaceTile(num90, num91, 237);
				if (Main.tile[num90, num91].type == 237)
				{
					int lAltarX = num90 - Main.tile[num90, num91].frameX / 18;
					int lAltarY = num91 - Main.tile[num90, num91].frameY / 18;
					typeof(WorldGen).GetField("lAltarX", BindingFlags.NonPublic | BindingFlags.Static)
						.SetValue(null, lAltarX);
					typeof(WorldGen).GetField("lAltarY", BindingFlags.NonPublic | BindingFlags.Static)
						.SetValue(null, lAltarY);
					break;
				}

				if (num87 < 1000)
					continue;

				num90 = rectangle3.X + num88;
				num91 = rectangle3.Y + num89;
				num90 += Random.Next(-10, 11);
				for (num91 += Random.Next(-10, 11); !Main.tile[num90, num91].IsActive; num91++)
				{
				}

				Main.tile[num90 - 1, num91].IsActive = true;
				Main.tile[num90 - 1, num91].Slope = SlopeType.Solid;
				Main.tile[num90 - 1, num91].IsHalfBlock = false;
				Main.tile[num90 - 1, num91].type = 226;
				Main.tile[num90, num91].IsActive = true;
				Main.tile[num90, num91].Slope = SlopeType.Solid;
				Main.tile[num90, num91].IsHalfBlock = false;
				Main.tile[num90, num91].type = 226;
				Main.tile[num90 + 1, num91].IsActive = true;
				Main.tile[num90 + 1, num91].Slope = SlopeType.Solid;
				Main.tile[num90 + 1, num91].IsHalfBlock = false;
				Main.tile[num90 + 1, num91].type = 226;
				num91 -= 2;
				num90--;
				for (int num92 = -1; num92 <= 3; num92++)
				for (int num93 = -1; num93 <= 1; num93++)
				{
					x = num90 + num92;
					y = num91 + num93;
					Main.tile[x, y].IsActive = false;
				}

				int lAltarX2 = num90;
				int lAltarY2 = num91;
				typeof(WorldGen).GetField("lAltarX", BindingFlags.NonPublic | BindingFlags.Static)
					.SetValue(null, lAltarX2);
				typeof(WorldGen).GetField("lAltarY", BindingFlags.NonPublic | BindingFlags.Static)
					.SetValue(null, lAltarY2);
				for (int num94 = 0; num94 <= 2; num94++)
				for (int num95 = 0; num95 <= 1; num95++)
				{
					x = num90 + num94;
					y = num91 + num95;
					Main.tile[x, y].IsActive = true;
					Main.tile[x, y].type = 237;
					Main.tile[x, y].frameX = (short) (num94 * 18);
					Main.tile[x, y].frameY = (short) (num95 * 18);
				}

				for (int num96 = 0; num96 <= 2; num96++)
				for (int num97 = 0; num97 <= 1; num97++)
				{
					x = num90 + num96;
					y = num91 + num97;
					WorldGen.SquareTileFrame(x, y);
				}

				break;
			}

			float num98 = num2 * 1.1f;
			num98 *= 1f + Random.Next(-25, 26) * 0.01f;
			if (WorldGen.drunkWorldGen)
				num98 *= 1.5f;

			int num99 = 0;
			while (num98 > 0f)
			{
				num99++;
				int num100 = Random.Next(num2);
				int num101 = Random.Next(rooms[num100].X, rooms[num100].X + rooms[num100].Width);
				int num102 = Random.Next(rooms[num100].Y, rooms[num100].Y + rooms[num100].Height);
				if (Main.tile[num101, num102].wall == 87 && !Main.tile[num101, num102].IsActive)
				{
					bool flag5 = false;
					if (Random.Next(2) == 0)
					{
						int num103 = 1;
						if (Random.Next(2) == 0)
							num103 = -1;

						for (; !Main.tile[num101, num102].IsActive; num102 += num103)
						{
						}

						num102 -= num103;
						int num104 = Random.Next(2);
						int num105 = Random.Next(3, 10);
						bool flag6 = true;
						for (int num106 = num101 - num105; num106 < num101 + num105; num106++)
						for (int num107 = num102 - num105; num107 < num102 + num105; num107++)
							if (Main.tile[num106, num107].IsActive && (Main.tile[num106, num107].type == 10 ||
							                                           Main.tile[num106, num107].type == 237))
							{
								flag6 = false;
								break;
							}

						if (flag6)
							for (int num108 = num101 - num105; num108 < num101 + num105; num108++)
							for (int num109 = num102 - num105; num109 < num102 + num105; num109++)
							{
								if (!WorldGen.SolidTile(num108, num109) || Main.tile[num108, num109].type == 232 ||
								    WorldGen.SolidTile(num108, num109 - num103))
									continue;

								Main.tile[num108, num109].type = 232;
								flag5 = true;
								if (num104 == 0)
								{
									Main.tile[num108, num109 - 1].type = 232;
									Main.tile[num108, num109 - 1].IsActive = true;
									if (WorldGen.drunkWorldGen)
									{
										Main.tile[num108, num109 - 2].type = 232;
										Main.tile[num108, num109 - 2].IsActive = true;
									}
								}
								else
								{
									Main.tile[num108, num109 + 1].type = 232;
									Main.tile[num108, num109 + 1].IsActive = true;
									if (WorldGen.drunkWorldGen)
									{
										Main.tile[num108, num109 + 2].type = 232;
										Main.tile[num108, num109 + 2].IsActive = true;
									}
								}

								num104++;
								if (num104 > 1)
									num104 = 0;
							}

						if (flag5)
						{
							num99 = 0;
							num98 -= 1f;
						}
					}
					else
					{
						int num110 = 1;
						if (Random.Next(2) == 0)
							num110 = -1;

						for (; !Main.tile[num101, num102].IsActive; num101 += num110)
						{
						}

						num101 -= num110;
						int num111 = Random.Next(2);
						int num112 = Random.Next(3, 10);
						bool flag7 = true;
						for (int num113 = num101 - num112; num113 < num101 + num112; num113++)
						for (int num114 = num102 - num112; num114 < num102 + num112; num114++)
							if (Main.tile[num113, num114].IsActive && Main.tile[num113, num114].type == 10)
							{
								flag7 = false;
								break;
							}

						if (flag7)
							for (int num115 = num101 - num112; num115 < num101 + num112; num115++)
							for (int num116 = num102 - num112; num116 < num102 + num112; num116++)
							{
								if (!WorldGen.SolidTile(num115, num116) || Main.tile[num115, num116].type == 232 ||
								    WorldGen.SolidTile(num115 - num110, num116))
									continue;

								Main.tile[num115, num116].type = 232;
								flag5 = true;
								if (num111 == 0)
								{
									Main.tile[num115 - 1, num116].type = 232;
									Main.tile[num115 - 1, num116].IsActive = true;
									if (WorldGen.drunkWorldGen)
									{
										Main.tile[num115 - 2, num116].type = 232;
										Main.tile[num115 - 2, num116].IsActive = true;
									}
								}
								else
								{
									Main.tile[num115 + 1, num116].type = 232;
									Main.tile[num115 + 1, num116].IsActive = true;
									if (WorldGen.drunkWorldGen)
									{
										Main.tile[num115 - 2, num116].type = 232;
										Main.tile[num115 - 2, num116].IsActive = true;
									}
								}

								num111++;
								if (num111 > 1)
									num111 = 0;
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

			WorldGen.tLeft = num50;
			WorldGen.tRight = num51;
			WorldGen.tTop = num52;
			WorldGen.tBottom = num53;
			WorldGen.tRooms = num2;
		}
	}
}