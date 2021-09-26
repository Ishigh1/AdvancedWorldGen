using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedWorldGen.Helper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class MushroomPatches : ControlledWorldGenPass
	{
		public MushroomPatches() : base("Mushroom Patches", 743.7686f)
		{
		}

		protected override void ApplyPass()
		{
			Progress.Message = Language.GetTextValue("LegacyWorldGen.13");
			int mushroomBiomes = Math.Max(1, Main.maxTilesX / 700);

			List<Vector2> mushroomBiomesPosition = new();
			const int spread = 100;
			int jungleMinX = Math.Max(VanillaInterface.JungleMinX - spread, spread);
			int jungleSpread = Math.Min(VanillaInterface.JungleMaxX + spread, Main.maxTilesX - spread) -
			                   jungleMinX;
			int xMax = Main.maxTilesX - jungleSpread - spread;

			int tries = 0;
			for (int numBiome = 0; numBiome < mushroomBiomes; numBiome++)
			{
				Progress.Set(numBiome, mushroomBiomes, 0.5f);
				bool isValid = false;
				while (!isValid)
				{
					tries++;
					if (tries > Main.maxTilesX / 2)
						break;

					int x = WorldGen.genRand.Next(spread, xMax);
					if (x >= jungleMinX)
						x += jungleSpread;

					int y;
					y = Main.rockLayer + 200 < Main.UnderworldLayer
						? WorldGen.genRand.Next((int) Main.rockLayer + 50, Main.UnderworldLayer - 100)
						: WorldGen.genRand.Next((int) Main.rockLayer, Main.UnderworldLayer);
					const int distanceBetweenBiomes = 500;

					Vector2 current = new(x, y);
					isValid = mushroomBiomesPosition.All(position =>
						current.Distance(position) > distanceBetweenBiomes);

					for (int x2 = x - spread; x2 < x + spread && isValid; x2 += 10)
					for (int y2 = y - spread; y2 < y + spread && isValid; y2 += 10)
						if (Main.tile[x2, y2].type is TileID.SnowBlock or TileID.IceBlock or TileID.BreakableIce or
							TileID.JungleGrass or TileID.Granite or TileID.Marble)
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
				Progress.Set(x - 50, Main.maxTilesX - 100, 0.5f, 0.5f);
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

						if (WorldGen.genRand.NextBool(4))
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

		public void ShroomPatch(int x, int y)
		{
			int num = WorldGen.genRand.Next(80, 100);
			int num2 = WorldGen.genRand.Next(20, 26);
			float worldSize = Main.maxTilesX / 4200f;
			if (WorldGen.getGoodWorldGen)
				worldSize *= 2f;

			num = (int) (num * worldSize);
			num2 = (int) (num2 * worldSize);
			float num4 = num2 - 1f;

			Vector2 vector = new(x, y - num2 * 0.3f);
			Vector2 vector2 = new(WorldGen.genRand.Next(-100, 101) * 0.005f,
				WorldGen.genRand.Next(-200, -100) * 0.005f);

			while (num > 0 && num2 > 0)
			{
				num -= WorldGen.genRand.Next(3);
				num2 -= 1;
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

				double num5 = num * WorldGen.genRand.Next(80, 120) * 0.01;
				for (x = xMin; x < xMax; x++)
				for (y = yMin; y < yMax; y++)
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
					else if (num12 < num5 * 0.4 * (0.95 + WorldGen.genRand.NextFloat() * 0.1))
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
					x = (int) vector.X + WorldGen.genRand.Next(-20, 20);
					y = (int) vector.Y + WorldGen.genRand.Next(0, 20);
					(x, y) = TileFinder.SpiralSearch(x, y, (i1, i2) => Main.tile[i1, i2].IsActive);

					int strength = WorldGen.genRand.Next(10, 20);
					int steps = WorldGen.genRand.Next(10, 20);
					WorldGen.TileRunner(x, y, strength, steps, TileID.Mud, false, 0f, 2f, true);
				}
			}
		}
	}
}