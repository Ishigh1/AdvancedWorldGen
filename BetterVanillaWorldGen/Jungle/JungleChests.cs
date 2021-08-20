using System;
using System.Reflection;
using AdvancedWorldGen.Helper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Jungle
{
	public class JungleChests : GenPass
	{
		public JungleChests() : base("Jungle Chests", 0.5896f)
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration passConfig)
		{
			ushort jungleHut = _random.Next(5) switch
			{
				0 => 119,
				1 => 120,
				2 => 158,
				3 => 175,
				4 => 45
			};

			float num536 = _random.Next(7, 12);
			num536 *= Main.maxTilesX / 4200f;
			for (int num538 = 0; (float) num538 < num536; num538++)
			{
				GenPassHelper.SetProgress(progress, num538, num536);
				int minX = Math.Max(10, JunglePass.JungleX - Main.maxTilesX / 8);
				int maxX = Math.Min(Main.maxTilesX - 10, JunglePass.JungleX + Main.maxTilesX / 8);
				int x = _random.Next(minX, maxX);

				int y;
				if (Main.maxTilesY - 400 - (Main.worldSurface + Main.rockLayer) / 2 > 100)
					y = _random.Next((int) (Main.worldSurface + Main.rockLayer) / 2, Main.maxTilesY - 400);
				else if ((Main.worldSurface + Main.rockLayer) / 2 - Main.worldSurface > 100)
					y = _random.Next((int) (Main.worldSurface + Main.rockLayer) / 2 - 100,
						Main.maxTilesY - 400);
				else
					y = _random.Next((int) Main.worldSurface, Main.UnderworldLayer);


				(x, y) = TileFinder.SpiralSearch(x, y, IsValid);

				int num542 = _random.Next(2, 4);
				int num543 = _random.Next(2, 4);
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
					int num549 = _random.Next(x - num542, x + num542 + 1);
					int num550 = _random.Next(y - num543, y + num543 - 2);
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

				num542 -= _random.Next(1, 3);
				int num558 = y - num543 - 2;
				while (num542 > -1)
				{
					for (int num559 = x - num542 - 1; num559 <= x + num542 + 1; num559++)
					{
						Main.tile[num559, num558].IsActive = true;
						Main.tile[num559, num558].type = jungleHut;
					}

					num542 -= _random.Next(1, 3);
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

			Main.tileSolid[TileID.Traps] = false;
		}

		public static bool IsValid(int x, int y)
		{
			if (Main.tile[x, y].type == 60)
			{
				const int spread = 30;
				int xMin = Math.Min(x - spread, 10);
				int xMax = Math.Min(x + spread, Main.maxTilesX - 10);
				int yMin = Math.Min(y - spread, 10);
				int yMax = Math.Min(y + spread, Main.maxTilesY - 10);
				for (int x1 = xMin; x1 < xMax; x1 += 3)
				for (int y1 = yMin; y1 < yMax; y1 += 3)
				{
					if (Main.tile[x1, y1].IsActive && (Main.tile[x1, y1].type == 225 ||
					                                           Main.tile[x1, y1].type == 229 ||
					                                           Main.tile[x1, y1].type == 226 ||
					                                           Main.tile[x1, y1].type == 119 ||
					                                           Main.tile[x1, y1].type == 120))
						return true;

					if (Main.tile[x1, y1].wall == 86 || Main.tile[x1, y1].wall == 87)
						return true;
				}
			}

			return false;
		}
	}
}