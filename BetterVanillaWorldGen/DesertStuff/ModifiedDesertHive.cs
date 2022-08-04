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
		foreach (((int x, int y), List<int> _) in hive) Tile.SmoothSlope(x, y, false);
	}

	public static Dictionary<(int, int), List<int>> RegisterInterestingTiles(ClusterGroup clusters)
	{
		Dictionary<(int, int), List<int>> registerInterestingTiles = new();
		int spreadX = (int)(10 * clusters.SpreadX);
		int spreadY = (int)(10 * clusters.SpreadY);
		for (int index = 0; index < clusters.Count; index++)
		{
			Cluster cluster = clusters[index];
			int minX = Math.Max(cluster[0].x - spreadX, 5);
			int maxX = Math.Min(cluster[0].x + spreadX, Main.maxTilesX - 5);
			int minY = Math.Max(cluster[0].y - spreadY, 5);
			int maxY = Math.Min(cluster[0].y + spreadY, Main.UnderworldLayer);
			for (int x = minX; x <= maxX; x++)
			for (int y = minY; y <= maxY; y++)
			{
				if (!registerInterestingTiles.TryGetValue((x, y), out List<int>? clusterList))
				{
					clusterList = new List<int>();
					registerInterestingTiles[(x, y)] = clusterList;
				}

				clusterList.Add(index);
				WorldGen.UpdateDesertHiveBounds(x, y);
			}
		}

		return registerInterestingTiles;
	}

	public static void PlaceClustersArea(ClusterGroup clusterGroup, Dictionary<(int, int), List<int>> hive)
	{
		foreach (((int x, int y), List<int> interestingClusters) in hive)
		{
			float distanceToClosestCenter = 0f;
			int closestCluster = -1;
			float distanceToSecondClosestCenter = 0f;
			ushort type = 53;
			if (WorldGen.genRand.NextBool(3))
				type = 397;

			foreach (int k in interestingClusters)
			{
				Cluster cluster = clusterGroup[k];
				if (Math.Abs(cluster[0].x - x) > 10f * clusterGroup.SpreadX ||
				    Math.Abs(cluster[0].y - y) > 10f * clusterGroup.SpreadY)
					continue;

				float distanceScore = cluster.Sum(item =>
					1f / Vector2.DistanceSquared(
						new Vector2(item.x / clusterGroup.SpreadX, item.y / clusterGroup.SpreadY),
						new Vector2(x / clusterGroup.SpreadX, y / clusterGroup.SpreadY)));

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
					tile.WallType = 187;
					if (closestCluster % 15 == 2)
						tile.ResetToType(404);
					break;
				case > 1.8f:
					tile.WallType = 187;
					if (y < Main.worldSurface)
						tile.LiquidAmount = 0;
					else
						tile.LiquidType = LiquidID.Lava;

					if (tile.HasTile) tile.ResetToType(TileID.Sandstone);
					break;
				case > 0.7f:
					tile.WallType = 216;
					tile.LiquidAmount = 0;
					if (tile.HasTile) tile.ResetToType(type);
					break;
				case > 0.25f:
					float num8 = (score - 0.25f) / 0.45f;
					if (WorldGen.genRand.NextFloat() < num8)
					{
						tile.WallType = 187;
						if (y < Main.worldSurface)
							tile.LiquidAmount = 0;
						else
							tile.LiquidType = LiquidID.Lava;

						if (tile.HasTile) tile.ResetToType(type);
					}

					break;
			}
		}
	}

	public static void AddTileVariance(DesertDescription description)
	{
		int xMin = Math.Max(description.Hive.X - 20, 5);
		int xMax = Math.Max(description.Hive.X + description.Hive.Width + 20, Main.maxTilesX - 5);
		int yMin = Math.Max(description.Hive.Y - 20, 5);
		int yMax = Math.Max(description.Hive.Y + description.Hive.Height + 20, Main.maxTilesY - 5);
		for (int x = xMin; x < xMax; x++)
		for (int y = yMin; y < yMax; y++)
		{
			Tile tile = Main.tile[x, y];
			Tile testTile = Main.tile[x, y + 1];
			Tile testTile2 = Main.tile[x, y + 2];
			if (tile.TileType == 53 && (!WorldGen.SolidTile(testTile) || !WorldGen.SolidTile(testTile2)))
				tile.TileType = 397;
		}

		for (int x = xMin; x < xMax; x++)
		for (int y = yMin; y < yMax; y++)
		{
			Tile tile2 = Main.tile[x, y];
			if (!tile2.HasTile || tile2.TileType != 396)
				continue;

			bool flag = true;
			for (int num5 = -1; num5 >= -3; num5--)
				if (Main.tile[x, y + num5].HasTile)
				{
					flag = false;
					break;
				}

			bool flag2 = true;
			for (int m = 1; m <= 3; m++)
				if (Main.tile[x, y + m].HasTile)
				{
					flag2 = false;
					break;
				}

			switch (flag)
			{
				case true when WorldGen.genRand.NextBool(20):
					WorldGen.PlaceTile(x, y - 1, 485, true, true, -1, WorldGen.genRand.Next(4));
					break;
				case true when WorldGen.genRand.NextBool(5):
					WorldGen.PlaceTile(x, y - 1, 484, true, true);
					break;
				default:
				{
					if (flag ^ flag2 && WorldGen.genRand.NextBool(5))
						WorldGen.PlaceTile(x, y + (!flag ? 1 : -1), 165, true, true);
					else if (flag && WorldGen.genRand.NextBool(5))
						WorldGen.PlaceTile(x, y - 1, 187, true, true, -1, 29 + WorldGen.genRand.Next(6));
					break;
				}
			}
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
}