using System;
using System.Reflection;
using AdvancedWorldGen.Helper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class JungleChests : GenPass
	{
		public JunglePass JunglePass;

		public JungleChests(JunglePass junglePass) : base("Jungle Chests", 0.5896f)
		{
			JunglePass = junglePass;
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			ushort jungleHut = WorldGen.genRand.Next(5) switch
			{
				0 => 119,
				1 => 120,
				2 => 158,
				3 => 175,
				4 => 45
			};

			int x;
			int y;
			float num536 = WorldGen.genRand.Next(7, 12);
			num536 *= Main.maxTilesX / 4200f;
			int tries = 0;
			for (int num538 = 0; (float) num538 < num536 && tries < Main.maxTilesX; num538++)
			{
				GenPassHelper.SetProgress(progress, num538, num536);
				bool flag35 = true;
				int minX = Math.Max(10, JunglePass.JungleX - Main.maxTilesX / 8);
				int maxX = Math.Min(Main.maxTilesX - 10, JunglePass.JungleX + Main.maxTilesX / 8);
				while (flag35)
				{
					tries++;
					x = WorldGen.genRand.Next(minX, maxX);

					if (Main.maxTilesY - 400 - (Main.worldSurface + Main.rockLayer) / 2 > 100)
						y = WorldGen.genRand.Next((int) (Main.worldSurface + Main.rockLayer) / 2, Main.maxTilesY - 400);
					else if ((Main.worldSurface + Main.rockLayer) / 2 - Main.worldSurface > 100)
						y = WorldGen.genRand.Next((int) (Main.worldSurface + Main.rockLayer) / 2 - 100, Main.maxTilesY - 400);
					else
						y = WorldGen.genRand.Next((int) Main.worldSurface, Main.UnderworldLayer);

					if (Main.tile[x, y].type == 60)
					{
						const int num539 = 30;
						flag35 = false;
						for (int num540 = x - num539; num540 < x + num539; num540 += 3)
						for (int num541 = y - num539; num541 < y + num539; num541 += 3)
						{
							if (Main.tile[num540, num541].IsActive && (Main.tile[num540, num541].type == 225 ||
							                                           Main.tile[num540, num541].type == 229 ||
							                                           Main.tile[num540, num541].type == 226 ||
							                                           Main.tile[num540, num541].type == 119 ||
							                                           Main.tile[num540, num541].type == 120))
								flag35 = true;

							if (Main.tile[num540, num541].wall == 86 || Main.tile[num540, num541].wall == 87)
								flag35 = true;
						}
					}

					if (!flag35)
					{
						int num542 = WorldGen.genRand.Next(2, 4);
						int num543 = WorldGen.genRand.Next(2, 4);
						Rectangle area = new(x - num542 - 1, y - num543 - 1, num542 + 1,
							num543 + 1);
						ushort wall2 = jungleHut switch
						{
							119 => 23,
							120 => 24,
							158 => 42,
							175 => 45,
							45 => 10
						};

						for (int num544 = x - num542 - 1; num544 <= x + num542 + 1; num544++)
						for (int num545 = y - num543 - 1; num545 <= y + num543 + 1; num545++)
						{
							Main.tile[num544, num545].IsActive = true;
							Main.tile[num544, num545].type = jungleHut;
							Main.tile[num544, num545].LiquidAmount = 0;
							Main.tile[num544, num545].LiquidType = LiquidID.Water;
						}

						for (int num546 = x - num542; num546 <= x + num542; num546++)
						for (int num547 = y - num543; num547 <= y + num543; num547++)
						{
							Main.tile[num546, num547].IsActive = false;
							Main.tile[num546, num547].wall = wall2;
						}

						bool flag36 = false;
						int num548 = 0;
						while (!flag36 && num548 < 100)
						{
							num548++;
							int num549 = WorldGen.genRand.Next(x - num542, x + num542 + 1);
							int num550 = WorldGen.genRand.Next(y - num543, y + num543 - 2);
							WorldGen.PlaceTile(num549, num550, 4, true, false, -1, 3);
							if (TileID.Sets.Torch[Main.tile[num549, num550].type])
								flag36 = true;
						}

						for (int num551 = x - num542 - 1; num551 <= x + num542 + 1; num551++)
						for (int num552 = y + num543 - 2; num552 <= y + num543; num552++)
							Main.tile[num551, num552].IsActive = false;

						for (int num553 = x - num542 - 1; num553 <= x + num542 + 1; num553++)
						for (int num554 = y + num543 - 2; num554 <= y + num543 - 1; num554++)
							Main.tile[num553, num554].IsActive = false;

						for (int num555 = x - num542 - 1; num555 <= x + num542 + 1; num555++)
						{
							int num556 = 4;
							int num557 = y + num543 + 2;
							while (!Main.tile[num555, num557].IsActive && num557 < Main.maxTilesY && num556 > 0)
							{
								Main.tile[num555, num557].IsActive = true;
								Main.tile[num555, num557].type = 59;
								num557++;
								num556--;
							}
						}

						num542 -= WorldGen.genRand.Next(1, 3);
						int num558 = y - num543 - 2;
						while (num542 > -1)
						{
							for (int num559 = x - num542 - 1; num559 <= x + num542 + 1; num559++)
							{
								Main.tile[num559, num558].IsActive = true;
								Main.tile[num559, num558].type = jungleHut;
							}

							num542 -= WorldGen.genRand.Next(1, 3);
							num558--;
						}


						int[] jChestX = (int[]) typeof(WorldGen)
							.GetField("JChestX", BindingFlags.NonPublic | BindingFlags.Static)
							.GetValue(null);
						int[] jChestY = (int[]) typeof(WorldGen)
							.GetField("JChestY", BindingFlags.NonPublic | BindingFlags.Static)
							.GetValue(null);
						int numJChests = (int) typeof(WorldGen)
							.GetField("numJChests", BindingFlags.NonPublic | BindingFlags.Static)
							.GetValue(null);
						jChestX[numJChests] = x;
						jChestY[numJChests] = y;
						WorldGen.structures.AddProtectedStructure(area);
						typeof(WorldGen).GetField("numJChests", BindingFlags.NonPublic | BindingFlags.Static)
							.SetValue(null, numJChests + 1);
					}
					else if (tries > Main.maxTilesX)
					{
						num538++;
						break;
					}
					else if (x < JunglePass.JungleX)
					{
						minX = x;
					}
					else
					{
						maxX = x;
					}
				}
			}

			Main.tileSolid[137] = false;
		}
	}
}