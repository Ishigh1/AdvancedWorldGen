using AdvancedWorldGen.Helper.WorldGenHelpers;

namespace AdvancedWorldGen.BetterVanillaWorldGen.DesertStuff;

public static class ModifiedDesertHive
{
	public static void Place(DesertDescription description)
	{
		ClusterGroup clusters = ClusterGroup.FromDescription(description);
		PlaceClusters(clusters);
		AddTileVariance(description);
	}

	public static void PlaceClusters(ClusterGroup clusters)
	{
		Dictionary<(int, int), List<int>> hive = RegisterInterestingTiles(clusters);
		PlaceClustersArea(clusters, hive);
		foreach (((int x, int y), List<int> _) in hive)
		{
			Tile.SmoothSlope(x, y, false);
			WorldGenerator.CurrentGenerationProgress.Add(1, hive.Count, 1 / 6f);
		}
	}

	public static Dictionary<(int, int), List<int>> RegisterInterestingTiles(ClusterGroup clusters) // Weight : 1/3
	{
		Dictionary<(int, int), List<int>> registerInterestingTiles = new();
		int spreadX = (int)(10 * clusters.SpreadX);
		int spreadY = (int)(10 * clusters.SpreadY);
		int listSize = (int)(clusters.SpreadX * clusters.SpreadY * 3);
		for (int index = 0; index < clusters.Count; index++)
		{
			Cluster cluster = clusters[index];
			int minX = Math.Max(cluster[0].x - spreadX, 5);
			int maxX = Math.Min(cluster[0].x + spreadX, Main.maxTilesX - 6);
			int minY = Math.Max(cluster[0].y - spreadY, 5);
			int maxY = Math.Min(cluster[0].y + spreadY, Main.UnderworldLayer);
			for (int x = minX; x <= maxX; x++)
			for (int y = minY; y <= maxY; y++)
			{
				if (!registerInterestingTiles.TryGetValue((x, y), out List<int>? clusterList))
				{
					clusterList = new List<int>(listSize);
					registerInterestingTiles[(x, y)] = clusterList;
					WorldGen.UpdateDesertHiveBounds(x, y);
				}

				clusterList.Add(index);
			}

			WorldGenerator.CurrentGenerationProgress.Set(index, clusters.Count, 1 / 3f);
		}

		return registerInterestingTiles;
	}

	private static void
		PlaceClustersArea(ClusterGroup clusterGroup, Dictionary<(int, int), List<int>> hive) // Weight : 1/3
	{
		float spreadX = clusterGroup.SpreadX;
		float spreadY = clusterGroup.SpreadY;
		foreach (((int x, int y), List<int> interestingClusters) in hive)
		{
			float distanceToClosestCenter = 0f;
			int closestCluster = -1;
			float distanceToSecondClosestCenter = 0f;
			ushort type = WorldGen.genRand.NextBool(3) ? (ushort)397 : (ushort)53;

			foreach (int k in interestingClusters)
			{
				Cluster cluster = clusterGroup[k];

				float distanceScore = 0;
				foreach ((int x, int y) item in cluster)
				{
					float dx = (item.x - x) / spreadX;
					float dy = (item.y - y) / spreadY;
					float squaredDistance = dx * dx + dy * dy;
					distanceScore += 1f / squaredDistance;
				}

				if (distanceScore > distanceToClosestCenter)
				{
					distanceToSecondClosestCenter = distanceToClosestCenter;

					distanceToClosestCenter = distanceScore;
					closestCluster = k;
				}
				else if (distanceScore > distanceToSecondClosestCenter)
				{
					distanceToSecondClosestCenter = distanceScore;
				}
			}

			float score = distanceToClosestCenter + distanceToSecondClosestCenter;
			Tile tile = Main.tile[x, y];
			switch (score)
			{
				case > 3.5f:
					tile.ClearEverything();
					if (!WorldGen.remixWorldGen || !(y > Main.rockLayer + WorldGen.genRand.Next(-1, 2)))
					{
						if (closestCluster % 15 == 2)
							tile.ResetToType(404);
						tile.WallType = 187;
					}

					break;
				case > 1.8f:
					if (tile.HasTile) tile.ResetToType(TileID.Sandstone);
					if (!WorldGen.remixWorldGen || !(y > Main.rockLayer + WorldGen.genRand.Next(-1, 2)))
						tile.WallType = 187;
					if (!WorldGen.remixWorldGen)
						if (y < Main.worldSurface)
							tile.LiquidAmount = 0;
						else
							tile.LiquidType = LiquidID.Lava;
					break;
				case > 0.7f:
					if (tile.HasTile) tile.ResetToType(type);
					if (!WorldGen.remixWorldGen)
					{
						tile.WallType = 216;
						tile.LiquidAmount = 0;
					}
					else if (!(y > Main.rockLayer + WorldGen.genRand.Next(-1, 2)))
					{
						tile.WallType = 216;
					}

					break;
				case > 0.25f:
					float num8 = (score - 0.25f) / 0.45f;
					if (WorldGen.genRand.NextFloat() < num8)
					{
						if (!WorldGen.remixWorldGen || !(y > Main.rockLayer + WorldGen.genRand.Next(-1, 2)))
							tile.WallType = 187;
						if (!WorldGen.remixWorldGen)
							if (y < Main.worldSurface)
								tile.LiquidAmount = 0;
							else
								tile.LiquidType = LiquidID.Lava;

						if (tile.HasTile) tile.ResetToType(type);
					}

					break;
			}

			WorldGenerator.CurrentGenerationProgress.Add(1, hive.Count, 1 / 3f);
		}
	}

	private static void AddTileVariance(DesertDescription description) //Weight : 1/6
	{
		int xMin = Math.Max(description.Hive.X - 20, 5);
		int xMax = Math.Max(description.Hive.X + description.Hive.Width + 20, Main.maxTilesX - 5);
		int yMin = Math.Max(description.Hive.Y - 20, 5);
		int yMax = Math.Max(description.Hive.Y + description.Hive.Height + 20, Main.maxTilesY - 5);

		int size = (xMax - xMin) * (yMax - yMin);
		for (int x = xMin; x < xMax; x++)
		for (int y = yMin; y < yMax; y++)
		{
			Tile tile = Main.tile[x, y];
			Tile testTile = Main.tile[x, y + 1];
			Tile testTile2 = Main.tile[x, y + 2];
			if (tile.TileType == 53 && (!WorldGen.SolidTile(testTile) || !WorldGen.SolidTile(testTile2)))
				tile.TileType = 397;
		}

		WorldGenerator.CurrentGenerationProgress.Add(1, 12);

		for (int y = yMin; y < yMax; y++)
		{
			ushort spaceAbove = 0;
			for (int x = xMax; x >= xMin; x--)
			{
				Tile tile2 = Main.tile[x, y];
				if (tile2.HasTile && tile2.TileType == 396)
				{
					if (tile2.IsHalfBlock || tile2.TopSlope)
						spaceAbove = 0;
					else
					{
						spaceAbove++;
						for (int num5 = -1; num5 >= -3; num5--)
							if (Main.tile[x, y + num5].HasTile)
							{
								spaceAbove = 0;
								break;
							}
					}

					bool spaceBelow;
					if (tile2.BottomSlope)
						spaceBelow = false;
					else
					{
						spaceBelow = true;
						for (int m = 1; m <= 3; m++)
							if (Main.tile[x, y + m].HasTile)
							{
								spaceBelow = false;
								break;
							}
					}

					if (!(WorldGen.remixWorldGen && y > Main.rockLayer + WorldGen.genRand.Next(-1, 2)))
						if (spaceAbove > 1 && WorldGen.genRand.NextBool(20))
						{
							TilePlacer.QuickPlaceFurniture(x, y - 2, TileID.AntlionLarva, 2, 2,
								styleX: WorldGen.genRand.Next(4));
							spaceAbove = 0;
						}
						else if (spaceAbove > 1 && WorldGen.genRand.NextBool(5))
						{
							TilePlacer.QuickPlaceFurniture(x, y - 2, TileID.RollingCactus, 2, 2);
							spaceAbove = 0;
						}
						else if (spaceAbove > 0 && WorldGen.genRand.NextBool(5))
						{
							StalactiteHelper.PlaceAnyStalagmite(x, y - 2, StalactiteHelper.Sand);
							spaceAbove = 0;
						}
						else if (spaceAbove > 2 && WorldGen.genRand.NextBool(5))
						{
							TilePlacer.QuickPlaceFurniture(x, y - 2, TileID.LargePiles2, 3, 2,
								styleX: 29 + WorldGen.genRand.Next(6));
							spaceAbove = 0;
						}
						else if (spaceBelow && WorldGen.genRand.NextBool(5))
							StalactiteHelper.PlaceAnyStalactite(x, y + 1, StalactiteHelper.Sand);
				}
				else
				{
					spaceAbove = 0;
				}
			}
		}

		WorldGenerator.CurrentGenerationProgress.Add(1, size, 1 / 12f);
	}
}

public class Cluster : List<(int x, int y)>
{
}

public class ClusterGroup : List<Cluster>
{
	public readonly int Height;
	public readonly float SpreadX;
	public readonly float SpreadY;
	public readonly int Width;

	public ClusterGroup(int width, int height, DesertDescription description)
	{
		Width = width;
		Height = height;
		SpreadX = description.Hive.Width / (float)Width;
		SpreadY = description.Hive.Height / (float)Height;
		Generate(description);
	}

	public static ClusterGroup FromDescription(DesertDescription description)
	{
		return new ClusterGroup(description.BlockColumnCount, description.BlockRowCount, description);
	}

	public static void SearchForCluster(bool[,] blockMap, List<Point> pointCluster, int x, int y,
		int level = 2)
	{
		pointCluster.Add(new Point(x, y));
		blockMap[x, y] = false;
		level--;
		if (level != -1)
		{
			if (x > 0 && blockMap[x - 1, y])
				SearchForCluster(blockMap, pointCluster, x - 1, y, level);

			if (x < blockMap.GetLength(0) - 1 && blockMap[x + 1, y])
				SearchForCluster(blockMap, pointCluster, x + 1, y, level);

			if (y > 0 && blockMap[x, y - 1])
				SearchForCluster(blockMap, pointCluster, x, y - 1, level);

			if (y < blockMap.GetLength(1) - 1 && blockMap[x, y + 1])
				SearchForCluster(blockMap, pointCluster, x, y + 1, level);
		}
	}

	public static void AttemptClaim(int x, int y, int[,] clusterIndexMap, List<List<Point>> pointClusters,
		int index)
	{
		int num = clusterIndexMap[x, y];
		if (num == -1 || num == index)
			return;

		int num2 = WorldGen.genRand.NextBool(2) ? -1 : index;
		foreach (Point item in pointClusters[num]) clusterIndexMap[item.X, item.Y] = num2;
	}

	public void Generate(DesertDescription description)
	{
		Clear();
		bool[,] array = new bool[Width, Height];
		int num = Width / 2 - 1;
		int num2 = Height / 2 - 1;
		int num3 = (num + 1) * (num + 1);
		Point point = new(num, num2);
		for (int i = point.Y - num2; i <= point.Y + num2; i++)
		{
			float num4 = num / (float)num2 * (i - point.Y);
			int num5 = Math.Min(num, (int)Math.Sqrt(num3 - num4 * num4));
			for (int j = point.X - num5; j <= point.X + num5; j++) array[j, i] = WorldGen.genRand.NextBool(2);
		}

		List<List<Point>> pointClusters = new();
		for (int k = 0; k < array.GetLength(0); k++)
		for (int l = 0; l < array.GetLength(1); l++)
			if (array[k, l] && WorldGen.genRand.NextBool(2))
			{
				List<Point> list2 = new();
				SearchForCluster(array, list2, k, l);
				if (list2.Count > 2)
					pointClusters.Add(list2);
			}

		int[,] clusterIndexMap = new int[array.GetLength(0), array.GetLength(1)];
		for (int m = 0; m < clusterIndexMap.GetLength(0); m++)
		for (int n = 0; n < clusterIndexMap.GetLength(1); n++)
			clusterIndexMap[m, n] = -1;

		for (int num6 = 0; num6 < pointClusters.Count; num6++)
			foreach (Point item in pointClusters[num6])
				clusterIndexMap[item.X, item.Y] = num6;

		foreach (List<Point> pointCluster in pointClusters)
		foreach (Point item2 in pointCluster)
		{
			int x = item2.X;
			int y = item2.Y;
			if (clusterIndexMap[x, y] == -1)
				break;

			int index = clusterIndexMap[x, y];
			if (x > 0)
				AttemptClaim(x - 1, y, clusterIndexMap, pointClusters, index);

			if (x < clusterIndexMap.GetLength(0) - 1)
				AttemptClaim(x + 1, y, clusterIndexMap, pointClusters, index);

			if (y > 0)
				AttemptClaim(x, y - 1, clusterIndexMap, pointClusters, index);

			if (y < clusterIndexMap.GetLength(1) - 1)
				AttemptClaim(x, y + 1, clusterIndexMap, pointClusters, index);
		}

		foreach (List<Point> item3 in pointClusters) item3.Clear();

		for (int num8 = 0; num8 < clusterIndexMap.GetLength(0); num8++)
		for (int num9 = 0; num9 < clusterIndexMap.GetLength(1); num9++)
			if (clusterIndexMap[num8, num9] != -1)
				pointClusters[clusterIndexMap[num8, num9]].Add(new Point(num8, num9));

		foreach (List<Point> item5 in pointClusters)
		{
			if (item5.Count < 4)
				continue;

			Cluster cluster = new();
			cluster.AddRange(item5.Select(item6 => (
				(int)(item6.X * SpreadX) + description.Hive.X,
				(int)(item6.Y * SpreadY) + description.Hive.Y)));

			Add(cluster);
		}
	}
}