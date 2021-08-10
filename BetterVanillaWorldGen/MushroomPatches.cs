using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedWorldGen.Helper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class MushroomPatches : GenPass
	{
		public FastRandom FastRandom;

		public MushroomPatches() : base("Mushroom Patches", 743.7686f) //Magic number from vanilla
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetText("LegacyWorldGen.13").Value;
			int mushroomBiomes = Math.Max(1, Main.maxTilesX / 700);
			FastRandom = new FastRandom(WorldGen.genRand.Next());

			List<Vector2> mushroomBiomesPosition = new();
			int tries = 0;
			const int spread = 100;
			int minTiles1 = (int) Math.Max(spread, Main.maxTilesX * 0.2);
			int maxTiles1 = (int) Math.Min(Main.maxTilesX - spread, Main.maxTilesX * 0.975);
			int minTiles2 = (int) Math.Max(spread, Main.maxTilesX * 0.25);
			int maxTiles2 = (int) Math.Min(Main.maxTilesX - spread, Main.maxTilesX * 0.8);
			for (int numBiome = 0; numBiome < mushroomBiomes; numBiome++)
			{
				GenPassHelper.SetProgress(progress, numBiome, mushroomBiomes, 0.5f);
				bool isValid = false;
				while (!isValid)
				{
					tries++;
					if (tries > Main.maxTilesX / 2)
						break;

					int x = tries < Main.maxTilesX / 4
						? FastRandom.Next(minTiles1, maxTiles1)
						: FastRandom.Next(minTiles2, maxTiles2);

					int y = FastRandom.Next((int) Main.rockLayer + 50, Main.maxTilesY - 300);
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
						int x2 = x + FastRandom.Next(-40, 41);
						int y2 = y + FastRandom.Next(-40, 41);
						ShroomPatch(x2, y2);
					}

					mushroomBiomesPosition.Add(new Vector2(x, y));
				}

				if (tries > Main.maxTilesX / 2)
					break;
			}

			for (int x = 50; x < Main.maxTilesX - 50; x++)
			{
				GenPassHelper.SetProgress(progress, x - 50, Main.maxTilesX - 100, 0.5f, 0.5f);
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

						if (FastRandom.Next(4) == 0)
						{
							int num814 = x + FastRandom.Next(-20, 21);
							int num815 = y + FastRandom.Next(-20, 21);
							if (Main.tile[num814, num815].type == 59)
								Main.tile[num814, num815].type = 70;
						}
					}
				}
			}
		}

		public void ShroomPatch(int i, int j)
		{
			double num = FastRandom.Next(80, 100);
			float num2 = FastRandom.Next(20, 26);
			float num3 = Main.maxTilesX / 4200f;
			if (WorldGen.getGoodWorldGen)
				num3 *= 2f;

			num *= num3;
			num2 *= num3;
			float num4 = num2 - 1f;
			Vector2 vector = default;
			vector.X = i;
			vector.Y = j - num2 * 0.3f;
			Vector2 vector2 = default;
			vector2.X = FastRandom.Next(-100, 101) * 0.005f;
			vector2.Y = FastRandom.Next(-200, -100) * 0.005f;
			while (num > 0.0 && num2 > 0f)
			{
				num -= FastRandom.Next(3);
				num2 -= 1f;
				int xMin = (int) (vector.X - num * 0.5);
				int xMax = (int) (vector.X + num * 0.5);
				int yMin = (int) (vector.Y - num * 0.5);
				int yMax = (int) (vector.Y + num * 0.5);
				if (xMin < 0)
					xMin = 0;

				if (xMax > Main.maxTilesX)
					xMax = Main.maxTilesX;

				if (yMin < 0)
					yMin = 0;

				if (yMax > Main.maxTilesY)
					yMax = Main.maxTilesY;

				double num5 = num * FastRandom.Next(80, 120) * 0.01;
				for (int x = xMin; x < xMax; x++)
				for (int y = yMin; y < yMax; y++)
				{
					float num10 = Math.Abs(x - vector.X);
					float num11 = Math.Abs((y - vector.Y) * 2.3f);
					double num12 = Math.Sqrt(num10 * num10 + num11 * num11);
					if (num12 < num5 * 0.8 && Main.tile[x, y].LiquidType == LiquidID.Lava)
						Main.tile[x, y].LiquidAmount = 0;

					if (num12 < num5 * 0.2 && y < vector.Y)
					{
						Main.tile[x, y].IsActive = false;
						if (Main.tile[x, y].wall > 0)
							Main.tile[x, y].wall = 80;
					}
					else if (num12 < num5 * 0.4 * (0.95 + FastRandom.NextFloat() * 0.1))
					{
						Main.tile[x, y].type = 59;
						if (num2 == num4 && y > vector.Y)
							Main.tile[x, y].IsActive = true;

						if (Main.tile[x, y].wall > 0)
							Main.tile[x, y].wall = 80;
					}
				}

				vector += vector2;
				vector.X += vector2.X;
				vector2.X += FastRandom.Next(-100, 110) * 0.005f;
				vector2.Y -= FastRandom.Next(110) * 0.005f;
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
					int x = (int) vector.X + FastRandom.Next(-20, 20);
					int y = (int) vector.Y + FastRandom.Next(0, 20);
					if (!Main.tile[x, y].IsActive)
					{
						const int stepSize = 2;
						int xStep = stepSize;
						int yStep = 0;

						int activeLength = 1;
						float length = 1;

						while (true)
						{
							x += xStep;
							y += yStep;
							if (Main.tile[x, y].IsActive)
							{
								break;
							}
							else if (--activeLength == 0)
							{
								int tmp = xStep;
								xStep = -yStep;
								yStep = tmp;
								length += 0.5f;
								activeLength = (int) length;
							}
						}
					}

					int strength = FastRandom.Next(10, 20);
					int steps = FastRandom.Next(10, 20);
					WorldGen.TileRunner(x, y, strength, steps, TileID.Mud, false, 0f, 2f, true);
				}
			}
		}
	}
}