using System;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class TerrainPass : ControlledWorldGenPass
	{
		public enum TerrainFeatureType
		{
			Plateau,
			Hill,
			Dale,
			Mountain,
			Valley
		}

		public TerrainPass() : base("Terrain", 449.3722f)
		{
		}

		public override void ApplyPass()
		{
			int leftBeachSize = Replacer.VanillaInterface.LeftBeachEnd.Value;
			int rightBeachSize = Main.maxTilesX - Replacer.VanillaInterface.RightBeachStart.Value;

			int num = Configuration.Get<int>("FlatBeachPadding");
			Progress.Message = Language.GetTextValue("LegacyWorldGen.0");
			TerrainFeatureType terrainFeatureType = TerrainFeatureType.Plateau;
			double worldSurface = Main.maxTilesY * 0.3;
			worldSurface *= WorldGen.genRand.Next(90, 110) * 0.005;
			double rockLayer = Main.maxTilesY * 0.35;
			rockLayer *= WorldGen.genRand.Next(90, 110) * 0.01;
			if (rockLayer < worldSurface + Main.maxTilesY * 0.05)
			{
				if (worldSurface - rockLayer > Main.maxTilesY * 0.05)
					worldSurface -= 2 * (worldSurface - rockLayer);
				else
					worldSurface = rockLayer - Main.maxTilesY * 0.05;
			}

			if (worldSurface < Main.maxTilesY * 0.07)
				worldSurface = Main.maxTilesY * 0.07;

			double worldSurfaceLow = worldSurface;
			double worldSurfaceHigh = worldSurface;
			double rockLayerLow = rockLayer;
			double rockLayerHigh = rockLayer;
			double num9 = Main.maxTilesY * 0.23;
			SurfaceHistory surfaceHistory = new(500);
			int num2 = leftBeachSize + num;
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				Progress.Set(i / (float) Main.maxTilesX);
				worldSurfaceLow = Math.Min(worldSurface, worldSurfaceLow);
				worldSurfaceHigh = Math.Max(worldSurface, worldSurfaceHigh);
				rockLayerLow = Math.Min(rockLayer, rockLayerLow);
				rockLayerHigh = Math.Max(rockLayer, rockLayerHigh);
				if (num2 <= 0)
				{
					terrainFeatureType = (TerrainFeatureType) WorldGen.genRand.Next(0, 5);
					num2 = WorldGen.genRand.Next(5, 40);
					if (terrainFeatureType == TerrainFeatureType.Plateau)
						num2 *= (int) (WorldGen.genRand.Next(5, 30) * 0.2);
				}

				num2--;
				if (i > Main.maxTilesX * 0.45 && i < Main.maxTilesX * 0.55 &&
				    terrainFeatureType is TerrainFeatureType.Mountain or TerrainFeatureType.Valley)
					terrainFeatureType = (TerrainFeatureType) WorldGen.genRand.Next(3);

				if (i > Main.maxTilesX * 0.48 && i < Main.maxTilesX * 0.52)
					terrainFeatureType = TerrainFeatureType.Plateau;

				worldSurface += GenerateWorldSurfaceOffset(terrainFeatureType);
				float num10 = 0.17f;
				float num11 = 0.26f;
				if (WorldGen.drunkWorldGen)
				{
					num10 = 0.15f;
					num11 = 0.28f;
				}

				if (i < leftBeachSize + num || i > Main.maxTilesX - rightBeachSize - num)
				{
					worldSurface = Utils.Clamp(worldSurface, Main.maxTilesY * 0.17, num9);
				}
				else if (worldSurface < Main.maxTilesY * num10)
				{
					worldSurface = Main.maxTilesY * num10;
					num2 = 0;
				}
				else if (worldSurface > Main.maxTilesY * num11)
				{
					worldSurface = Main.maxTilesY * num11;
					num2 = 0;
				}

				while (WorldGen.genRand.Next(0, 3) == 0) rockLayer += WorldGen.genRand.Next(-2, 3);

				if (rockLayer < worldSurface + Main.maxTilesY * 0.06)
					rockLayer += 1.0;

				if (rockLayer > worldSurface + Main.maxTilesY * 0.35)
					rockLayer -= 1.0;

				surfaceHistory.Record(worldSurface);
				FillColumn(i, worldSurface, rockLayer);
				if (i == Main.maxTilesX - rightBeachSize - num)
				{
					if (worldSurface > num9)
						RetargetSurfaceHistory(surfaceHistory, i, num9);

					terrainFeatureType = TerrainFeatureType.Plateau;
					num2 = Main.maxTilesX - i;
				}
			}

			Main.worldSurface = (int) (worldSurfaceHigh + 25.0);
			Main.rockLayer = rockLayerHigh;
			int waterLine = (int) (Main.rockLayer + Main.maxTilesY) / 2;
			waterLine += WorldGen.genRand.Next(-100, 20);
			int lavaLine = waterLine + WorldGen.genRand.Next(50, 80);
			if (rockLayer > Main.UnderworldLayer)
				throw new RockUnderHellException();
			while (lavaLine > Main.UnderworldLayer)
			{
				waterLine -= (int) (waterLine - rockLayer) / 8;
				lavaLine -= (int) (lavaLine - rockLayer) / 8;
			}

			const int num14 = 20;
			if (rockLayerLow < worldSurfaceHigh + num14)
			{
				double num15 = (rockLayerLow + worldSurfaceHigh) / 2.0;
				double num16 = Math.Abs(rockLayerLow - worldSurfaceHigh);
				if (num16 < num14)
					num16 = num14;

				rockLayerLow = num15 + num16 / 2.0;
				worldSurfaceHigh = num15 - num16 / 2.0;
			}

			WorldGen.worldSurface = worldSurface;
			WorldGen.worldSurfaceHigh = worldSurfaceHigh;
			WorldGen.worldSurfaceLow = worldSurfaceLow;
			WorldGen.rockLayer = rockLayer;
			WorldGen.rockLayerHigh = rockLayerHigh;
			WorldGen.rockLayerLow = rockLayerLow;
			WorldGen.waterLine = waterLine;
			WorldGen.lavaLine = lavaLine;
		}

		public static void FillColumn(int x, double worldSurface, double rockLayer)
		{
			for (int i = 0; i < worldSurface; i++)
			{
				Main.tile[x, i].IsActive = false;
				Main.tile[x, i].frameX = -1;
				Main.tile[x, i].frameY = -1;
			}

			for (int j = (int) worldSurface; j < Main.maxTilesY; j++)
				if (j < rockLayer)
				{
					Main.tile[x, j].IsActive = true;
					Main.tile[x, j].type = 0;
					Main.tile[x, j].frameX = -1;
					Main.tile[x, j].frameY = -1;
				}
				else
				{
					Main.tile[x, j].IsActive = true;
					Main.tile[x, j].type = 1;
					Main.tile[x, j].frameX = -1;
					Main.tile[x, j].frameY = -1;
				}
		}

		public static void RetargetColumn(int x, double worldSurface)
		{
			for (int i = 0; i < worldSurface; i++)
			{
				Main.tile[x, i].IsActive = false;
				Main.tile[x, i].frameX = -1;
				Main.tile[x, i].frameY = -1;
			}

			for (int j = (int) worldSurface; j < Main.maxTilesY; j++)
				if (Main.tile[x, j].type != 1 || !Main.tile[x, j].IsActive)
				{
					Main.tile[x, j].IsActive = true;
					Main.tile[x, j].type = 0;
					Main.tile[x, j].frameX = -1;
					Main.tile[x, j].frameY = -1;
				}
		}

		public double GenerateWorldSurfaceOffset(TerrainFeatureType featureType)
		{
			double num = 0.0;
			if ((WorldGen.drunkWorldGen || WorldGen.getGoodWorldGen) && WorldGen.genRand.NextBool(2))
				switch (featureType)
				{
					case TerrainFeatureType.Plateau:
						while (WorldGen.genRand.Next(0, 6) == 0) num += WorldGen.genRand.Next(-1, 2);
						break;
					case TerrainFeatureType.Hill:
						while (WorldGen.genRand.Next(0, 3) == 0) num -= 1.0;
						while (WorldGen.genRand.Next(0, 10) == 0) num += 1.0;
						break;
					case TerrainFeatureType.Dale:
						while (WorldGen.genRand.Next(0, 3) == 0) num += 1.0;
						while (WorldGen.genRand.Next(0, 10) == 0) num -= 1.0;
						break;
					case TerrainFeatureType.Mountain:
						while (WorldGen.genRand.Next(0, 3) != 0) num -= 1.0;
						while (WorldGen.genRand.Next(0, 6) == 0) num += 1.0;
						break;
					case TerrainFeatureType.Valley:
						while (WorldGen.genRand.Next(0, 3) != 0) num += 1.0;
						while (WorldGen.genRand.Next(0, 5) == 0) num -= 1.0;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(featureType), featureType, null);
				}
			else
				switch (featureType)
				{
					case TerrainFeatureType.Plateau:
						while (WorldGen.genRand.Next(0, 7) == 0) num += WorldGen.genRand.Next(-1, 2);
						break;
					case TerrainFeatureType.Hill:
						while (WorldGen.genRand.Next(0, 4) == 0) num -= 1.0;
						while (WorldGen.genRand.Next(0, 10) == 0) num += 1.0;
						break;
					case TerrainFeatureType.Dale:
						while (WorldGen.genRand.Next(0, 4) == 0) num += 1.0;
						while (WorldGen.genRand.Next(0, 10) == 0) num -= 1.0;
						break;
					case TerrainFeatureType.Mountain:
						while (WorldGen.genRand.Next(0, 2) == 0) num -= 1.0;
						while (WorldGen.genRand.Next(0, 6) == 0) num += 1.0;
						break;
					case TerrainFeatureType.Valley:
						while (WorldGen.genRand.Next(0, 2) == 0) num += 1.0;
						while (WorldGen.genRand.Next(0, 5) == 0) num -= 1.0;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(featureType), featureType, null);
				}

			return num;
		}

		public static void RetargetSurfaceHistory(SurfaceHistory history, int targetX, double targetHeight)
		{
			for (int i = 0; i < history.Length / 2; i++)
			{
				if (history[^1] <= targetHeight)
					break;

				for (int j = 0; j < history.Length - i * 2; j++)
				{
					double num = history[history.Length - j - 1];
					num -= 1.0;
					history[history.Length - j - 1] = num;
					if (num <= targetHeight)
						break;
				}
			}

			for (int k = 0; k < history.Length; k++)
			{
				double worldSurface = history[history.Length - k - 1];
				RetargetColumn(targetX - k, worldSurface);
			}
		}

		public class SurfaceHistory
		{
			public readonly double[] Heights;
			public int Index;

			public SurfaceHistory(int size)
			{
				Heights = new double[size];
			}

			public double this[int index]
			{
				get => Heights[(index + Index) % Heights.Length];
				set => Heights[(index + Index) % Heights.Length] = value;
			}

			public int Length => Heights.Length;

			public void Record(double height)
			{
				Heights[Index] = height;
				Index = (Index + 1) % Heights.Length;
			}
		}
	}
}