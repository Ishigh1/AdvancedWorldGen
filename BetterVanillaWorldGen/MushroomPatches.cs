using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class MushroomPatches : GenPass
	{
		public MushroomPatches() : base("Mushroom Patches", 743.7686f) //Magic number from vanilla
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetText("LegacyWorldGen.13").Value;
			int mushroomBiomes = Math.Max(1, Main.maxTilesX / 700);

			List<Vector2> mushroomBiomesPosition = new();
			int tries = 0;
			const int spread = 100;
			int minTiles1 = (int) Math.Max(spread, Main.maxTilesX * 0.2);
			int maxTiles1 = (int) Math.Min(Main.maxTilesX - spread, Main.maxTilesX * 0.8); 
			int minTiles2 = (int) Math.Max(spread, Main.maxTilesX * 0.25);
			int maxTiles2 = (int) Math.Min(Main.maxTilesX - spread, Main.maxTilesX * 0.975);
			for (int numBiome = 0; numBiome < mushroomBiomes; numBiome++)
			{
				progress.Set(numBiome / (float) mushroomBiomes / 2f);
				bool isValid = false;
				while (!isValid)
				{
					tries++;
					if (tries > Main.maxTilesX / 2)
						break;

					int x = tries < Main.maxTilesX / 4
						? WorldGen.genRand.Next(minTiles1, maxTiles1)
						: WorldGen.genRand.Next(minTiles2, maxTiles2);

					int y = WorldGen.genRand.Next((int) Main.rockLayer + 50, Main.maxTilesY - 300);
					const int distanceBetweenBiomes = 500;

					Vector2 current = new(x, y);
					isValid = mushroomBiomesPosition.All(position =>
						current.Distance(position) > distanceBetweenBiomes);

					for (int x2 = x - spread; x2 < x + spread && isValid; x2 += 10)
					for (int y2 = y - spread; y2 < y + spread && isValid; y2 += 10)
						if (Main.tile[x2, y2].type == TileID.SnowBlock || Main.tile[x2, y2].type == TileID.IceBlock ||
						    Main.tile[x2, y2].type == TileID.IceBlock || Main.tile[x2, y2].type == TileID.JungleGrass ||
						    Main.tile[x2, y2].type == TileID.Granite || Main.tile[x2, y2].type == TileID.Marble)
							isValid = false;
						else if (WorldGen.UndergroundDesertLocation.Contains(new Point(x2, y2))) isValid = false;

					if (!isValid)
						continue;

					ShroomPatch(x, y);
					for (int it = 0; it < 5; it++)
					{
						int x2 = x + WorldGen.genRand.Next(-40, 41);
						int y2 = y + WorldGen.genRand.Next(-40, 41);
						ShroomPatch(x2, y2);
					}

					mushroomBiomesPosition.Add(new Vector2(x, y));
				}

				if (tries > Main.maxTilesX / 2)
					break;
			}

			for (int x = 50; x < Main.maxTilesX - 50; x++)
			{
				progress.Set(x / (float) Main.maxTilesX / 2f + 0.5f);
				for (int y = (int) Main.worldSurface; y < Main.maxTilesY - 50; y++)
				{
					if (!Main.tile[x, y].IsActive)
						continue;
					WorldGen.SpreadGrass(x, y, TileID.Mud, TileID.MushroomGrass, false);
					if (Main.tile[x, y].type == TileID.MushroomGrass)
					{
						const int type = TileID.Mud;
						for (int x2 = x - 1; x2 <= x + 1; x2++)
						for (int y2 = y - 1; y2 <= y + 1; y2++)
							if (Main.tile[x2, y2].IsActive)
							{
								if (!Main.tile[x2 - 1, y2].IsActive &&
								    !Main.tile[x2 + 1, y2].IsActive)
									WorldGen.KillTile(x2, y2);
								else if (!Main.tile[x2, y2 - 1].IsActive &&
								         !Main.tile[x2, y2 + 1].IsActive)
									WorldGen.KillTile(x2, y2);
							}
							else if (Main.tile[x2 - 1, y2].IsActive &&
							         Main.tile[x2 + 1, y2].IsActive)
							{
								WorldGen.PlaceTile(x2, y2, type);
								if (Main.tile[x2 - 1, y].type == 70)
									Main.tile[x2 - 1, y].type = 59;

								if (Main.tile[x2 + 1, y].type == 70)
									Main.tile[x2 + 1, y].type = 59;
							}
							else if (Main.tile[x2, y2 - 1].IsActive &&
							         Main.tile[x2, y2 + 1].IsActive)
							{
								WorldGen.PlaceTile(x2, y2, type);
								if (Main.tile[x2, y - 1].type == 70)
									Main.tile[x2, y - 1].type = 59;

								if (Main.tile[x2, y + 1].type == 70)
									Main.tile[x2, y + 1].type = 59;
							}

						if (WorldGen.genRand.Next(4) == 0)
						{
							int num814 = x + WorldGen.genRand.Next(-20, 21);
							int num815 = y + WorldGen.genRand.Next(-20, 21);
							if (Main.tile[num814, num815].type == 59)
								Main.tile[num814, num815].type = 70;
						}
					}
				}
			}
		}

		public static void ShroomPatch(int i, int j)
		{
			double num = WorldGen.genRand.Next(80, 100);
			float num2 = WorldGen.genRand.Next(20, 26);
			float num3 = Main.maxTilesX / 4200;
			if (WorldGen.getGoodWorldGen)
				num3 *= 2f;

			num *= num3;
			num2 *= num3;
			float num4 = num2 - 1f;
			double num5 = num;
			Vector2 vector = default;
			vector.X = i;
			vector.Y = j - num2 * 0.3f;
			Vector2 vector2 = default;
			vector2.X = WorldGen.genRand.Next(-100, 101) * 0.005f;
			vector2.Y = WorldGen.genRand.Next(-200, -100) * 0.005f;
			while (num > 0.0 && num2 > 0f)
			{
				num -= WorldGen.genRand.Next(3);
				num2 -= 1f;
				int num6 = (int) (vector.X - num * 0.5);
				int num7 = (int) (vector.X + num * 0.5);
				int num8 = (int) (vector.Y - num * 0.5);
				int num9 = (int) (vector.Y + num * 0.5);
				if (num6 < 0)
					num6 = 0;

				if (num7 > Main.maxTilesX)
					num7 = Main.maxTilesX;

				if (num8 < 0)
					num8 = 0;

				if (num9 > Main.maxTilesY)
					num9 = Main.maxTilesY;

				num5 = num * WorldGen.genRand.Next(80, 120) * 0.01;
				for (int k = num6; k < num7; k++)
				for (int l = num8; l < num9; l++)
				{
					float num10 = Math.Abs(k - vector.X);
					float num11 = Math.Abs((l - vector.Y) * 2.3f);
					double num12 = Math.Sqrt(num10 * num10 + num11 * num11);
					if (num12 < num5 * 0.8 && Main.tile[k, l].LiquidType == LiquidID.Lava)
						Main.tile[k, l].LiquidAmount = 0;

					if (num12 < num5 * 0.2 && l < vector.Y)
					{
						Main.tile[k, l].IsActive = false;
						if (Main.tile[k, l].wall > 0)
							Main.tile[k, l].wall = 80;
					}
					else if (num12 < num5 * 0.4 * (0.95 + WorldGen.genRand.NextFloat() * 0.1))
					{
						Main.tile[k, l].type = 59;
						if (num2 == num4 && l > vector.Y)
							Main.tile[k, l].IsActive = true;

						if (Main.tile[k, l].wall > 0)
							Main.tile[k, l].wall = 80;
					}
				}

				vector += vector2;
				vector.X += vector2.X;
				vector2.X += WorldGen.genRand.Next(-100, 110) * 0.005f;
				vector2.Y -= WorldGen.genRand.Next(110) * 0.005f;
				if (vector2.X > -0.5 && vector2.X < 0.5)
				{
					if (vector2.X < 0f)
						vector2.X = -0.5f;
					else
						vector2.X = 0.5f;
				}

				if (vector2.X > 0.5)
					vector2.X = 0.5f;

				if (vector2.X < -0.5)
					vector2.X = -0.5f;

				if (vector2.Y > 0.5)
					vector2.Y = 0.5f;

				if (vector2.Y < -0.5)
					vector2.Y = -0.5f;

				for (int m = 0; m < 2; m++)
				{
					int x = (int) vector.X + WorldGen.genRand.Next(-20, 20);
					int y = (int) vector.Y + WorldGen.genRand.Next(0, 20);
					while (!Main.tile[x, y].IsActive)
					{
						x = (int) vector.X + WorldGen.genRand.Next(-20, 20);
						y = (int) vector.Y + WorldGen.genRand.Next(0, 20);
					}

					int strength = WorldGen.genRand.Next(10, 20);
					int steps = WorldGen.genRand.Next(10, 20);
					WorldGen.TileRunner(x, y, strength, steps, TileID.Mud, false, 0f, 2f, true);
				}
			}
		}
	}
}