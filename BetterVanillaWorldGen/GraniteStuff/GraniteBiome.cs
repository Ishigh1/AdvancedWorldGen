namespace AdvancedWorldGen.BetterVanillaWorldGen.GraniteStuff;

public class GraniteBiome : MicroBiome
{
	private struct Magma
	{
		public readonly float Pressure;
		public readonly float Resistance;
		public readonly bool IsActive;

		private Magma(float pressure, float resistance, bool active)
		{
			Pressure = pressure;
			Resistance = resistance;
			IsActive = active;
		}

		public Magma ToFlow() => new Magma(Pressure, Resistance, active: true);

		public static Magma CreateFlow(float pressure, float resistance = 0f) =>
			new Magma(pressure, resistance, active: true);

		public static Magma CreateEmpty(float resistance = 0f) => new Magma(0f, resistance, active: false);
	}

	private const int MAX_MAGMA_ITERATIONS = 300;
	private Magma[,] _sourceMagmaMap = new Magma[200, 200];
	private Magma[,] _targetMagmaMap = new Magma[200, 200];

	private static Vector2[] _normalisedVectors = new Vector2[9]
	{
		Vector2.Normalize(new Vector2(-1f, -1f)),
		Vector2.Normalize(new Vector2(-1f, 0f)),
		Vector2.Normalize(new Vector2(-1f, 1f)),
		Vector2.Normalize(new Vector2(0f, -1f)),
		new Vector2(0f, 0f),
		Vector2.Normalize(new Vector2(0f, 1f)),
		Vector2.Normalize(new Vector2(1f, -1f)),
		Vector2.Normalize(new Vector2(1f, 0f)),
		Vector2.Normalize(new Vector2(1f, 1f))
	};

	public static bool CanPlace(Point origin, StructureMap structures)
	{
		if (WorldGen.BiomeTileCheck(origin.X, origin.Y))
			return false;

		return !Main.tile[origin.X, origin.Y].HasTile;
	}

	public override bool Place(Point origin, StructureMap structures)
	{
		if (Main.tile[origin.X, origin.Y].HasTile)
			return false;

		origin.X -= _sourceMagmaMap.GetLength(0) / 2;
		origin.Y -= _sourceMagmaMap.GetLength(1) / 2;
		BuildMagmaMap(origin);
		SimulatePressure(out Rectangle effectedMapArea);
		PlaceGranite(origin, effectedMapArea);
		CleanupTiles(origin, effectedMapArea);
		PlaceDecorations(origin, effectedMapArea);
		structures.AddStructure(effectedMapArea, 8);
		return true;
	}

	private void BuildMagmaMap(Point tileOrigin)
	{
		_sourceMagmaMap = new Magma[200, 200];
		_targetMagmaMap = new Magma[200, 200];
		for (int i = 0; i < _sourceMagmaMap.GetLength(0); i++)
		{
			for (int j = 0; j < _sourceMagmaMap.GetLength(1); j++)
			{
				int i2 = i + tileOrigin.X;
				int j2 = j + tileOrigin.Y;
				_sourceMagmaMap[i, j] = Magma.CreateEmpty(WorldGen.SolidTile(i2, j2) ? 4f : 1f);
				_targetMagmaMap[i, j] = _sourceMagmaMap[i, j];
			}
		}
	}

	private void SimulatePressure(out Rectangle effectedMapArea)
	{
		int length = _sourceMagmaMap.GetLength(0);
		int length2 = _sourceMagmaMap.GetLength(1);
		int num = length / 2;
		int num2 = length2 / 2;
		int num3 = num;
		int num4 = num3;
		int num5 = num2;
		int num6 = num5;
		for (int i = 0; i < MAX_MAGMA_ITERATIONS; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				for (int k = num5; k <= num6; k++)
				{
					Magma magma = _sourceMagmaMap[j, k];
					if (!magma.IsActive)
						continue;

					float num7 = 0f;
					Vector2 zero = Vector2.Zero;
					for (int l = -1; l <= 1; l++)
					{
						for (int m = -1; m <= 1; m++)
						{
							if (l == 0 && m == 0)
								continue;

							Vector2 value = _normalisedVectors[(l + 1) * 3 + (m + 1)];
							Magma magma2 = _sourceMagmaMap[j + l, k + m];
							if (magma.Pressure > 0.01f && !magma2.IsActive)
							{
								if (l == -1)
									num3 = Utils.Clamp(j + l, 1, num3);
								else
									num4 = Utils.Clamp(j + l, num4, length - 2);

								if (m == -1)
									num5 = Utils.Clamp(k + m, 1, num5);
								else
									num6 = Utils.Clamp(k + m, num6, length2 - 2);

								_targetMagmaMap[j + l, k + m] = magma2.ToFlow();
							}

							float pressure = magma2.Pressure;
							num7 += pressure;
							zero += pressure * value;
						}
					}

					num7 /= 8f;
					if (num7 > magma.Resistance)
					{
						float num8 = zero.Length() / 8f;
						float val = Math.Max(num7 - num8 - magma.Pressure, 0f) + num8 + magma.Pressure * 0.875f -
						            magma.Resistance;
						val = Math.Max(0f, val);
						_targetMagmaMap[j, k] = Magma.CreateFlow(val, Math.Max(0f, magma.Resistance - val * 0.02f));
					}
				}
			}

			if (i < 2)
				_targetMagmaMap[num, num2] = Magma.CreateFlow(25f);

			Utils.Swap(ref _sourceMagmaMap, ref _targetMagmaMap);
		}

		effectedMapArea = new Rectangle(num3, num5, num4 - num3 + 1, num6 - num5 + 1);
	}

	private bool ShouldUseLava(Point tileOrigin)
	{
		int length = _sourceMagmaMap.GetLength(0);
		int length2 = _sourceMagmaMap.GetLength(1);
		int num = length / 2;
		int num2 = length2 / 2;
		if (tileOrigin.Y + num2 <= GenVars.lavaLine - 30)
			return false;

		for (int i = -50; i < 50; i++)
		{
			for (int j = -50; j < 50; j++)
			{
				if (Main.tile[tileOrigin.X + num + i, tileOrigin.Y + num2 + j].HasTile)
				{
					ushort type = _tiles[tileOrigin.X + num + i, tileOrigin.Y + num2 + j].TileType;
					if (type == 147 || (uint)(type - 161) <= 2u || type == 200)
						return false;
				}
			}
		}

		return true;
	}

	private void PlaceGranite(Point tileOrigin, Rectangle magmaMapArea)
	{
		bool flag = ShouldUseLava(tileOrigin);
		ushort type = 368;
		ushort wall = 180;
		if (WorldGen.drunkWorldGen)
		{
			type = 367;
			wall = 178;
		}

		for (int i = magmaMapArea.Left; i < magmaMapArea.Right; i++)
		{
			for (int j = magmaMapArea.Top; j < magmaMapArea.Bottom; j++)
			{
				Magma magma = _sourceMagmaMap[i, j];
				if (!magma.IsActive)
					continue;

				Tile tile = _tiles[tileOrigin.X + i, tileOrigin.Y + j];
				float num = (float)Math.Sin((tileOrigin.Y + j) * 0.4f) * 0.7f + 1.2f;
				float num2 = 0.2f + 0.5f / (float)Math.Sqrt(Math.Max(0f, magma.Pressure - magma.Resistance));
				if (Math.Max(1f - Math.Max(0f, num * num2), magma.Pressure / 15f) >
				    0.35f + (WorldGen.SolidTile(tileOrigin.X + i, tileOrigin.Y + j) ? 0f : 0.5f))
				{
					if (TileID.Sets.Ore[tile.TileType])
						tile.ResetToType(tile.TileType);
					else
						tile.ResetToType(type);

					tile.WallType = wall;
				}
				else if (magma.Resistance < 0.01f)
				{
					WorldUtils.ClearTile(tileOrigin.X + i, tileOrigin.Y + j);
					tile.WallType = wall;
				}

				if (tile.LiquidAmount > 0 && flag)
					tile.LiquidType = 1;
			}
		}
	}

	private void CleanupTiles(Point tileOrigin, Rectangle magmaMapArea)
	{
		ushort wall = 180;
		if (WorldGen.drunkWorldGen)
			wall = 178;

		List<Point> list = new();
		for (int i = magmaMapArea.Left; i < magmaMapArea.Right; i++)
		{
			for (int j = magmaMapArea.Top; j < magmaMapArea.Bottom; j++)
			{
				if (!_sourceMagmaMap[i, j].IsActive)
					continue;

				int num = 0;
				int num2 = i + tileOrigin.X;
				int num3 = j + tileOrigin.Y;
				if (!WorldGen.SolidTile(num2, num3))
					continue;

				for (int k = -1; k <= 1; k++)
				{
					for (int l = -1; l <= 1; l++)
					{
						if (WorldGen.SolidTile(num2 + k, num3 + l))
							num++;
					}
				}

				if (num < 3)
					list.Add(new Point(num2, num3));
			}
		}

		foreach (Point item in list)
		{
			int x = item.X;
			int y = item.Y;
			WorldUtils.ClearTile(x, y, frameNeighbors: true);
			Main.tile[x, y].WallType = wall;
		}

		list.Clear();
	}

	private void PlaceDecorations(Point tileOrigin, Rectangle magmaMapArea)
	{
		FastRandom fastRandom = new FastRandom(Main.ActiveWorldFileData.Seed).WithModifier(65440uL);
		for (int i = magmaMapArea.Left; i < magmaMapArea.Right; i++)
		{
			for (int j = magmaMapArea.Top; j < magmaMapArea.Bottom; j++)
			{
				Magma magma = _sourceMagmaMap[i, j];
				int num = i + tileOrigin.X;
				int num2 = j + tileOrigin.Y;
				if (!magma.IsActive)
					continue;

				WorldUtils.TileFrame(num, num2);
				WorldGen.SquareWallFrame(num, num2);
				FastRandom fastRandom2 = fastRandom.WithModifier(num, num2);
				if (fastRandom2.Next(8) == 0 && Main.tile[num, num2].HasTile)
				{
					if (!Main.tile[num, num2 + 1].HasTile)
						WorldGen.PlaceUncheckedStalactite(num, num2 + 1, fastRandom2.Next(2) == 0, fastRandom2.Next(3),
							spiders: false);

					if (!Main.tile[num, num2 - 1].HasTile)
						WorldGen.PlaceUncheckedStalactite(num, num2 - 1, fastRandom2.Next(2) == 0, fastRandom2.Next(3),
							spiders: false);
				}

				if (fastRandom2.Next(2) == 0)
					Tile.SmoothSlope(num, num2);
			}
		}
	}
}