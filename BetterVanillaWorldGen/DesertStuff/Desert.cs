namespace AdvancedWorldGen.BetterVanillaWorldGen.DesertStuff;

public static class Desert
{
	private static readonly ConstructorInfo ConstructorInfo =
		typeof(DesertDescription).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
			null, Array.Empty<Type>(), null)!;

	private static readonly Vector2 DefaultBlockScale = new(4f, 2f);

	public static bool IsUndergroundDesert(int x, int y)
	{
		if (y < Main.worldSurface)
			return false;

		if (x < Main.maxTilesX * 0.15 || x > Main.maxTilesX * 0.85)
			return false;

		if (WorldGen.remixWorldGen && y > Main.rockLayer)
			return false;

		const int spread = 15;
		for (int i = x - spread; i <= x + spread; i += 10)
		for (int j = y - spread; j <= y + spread; j += 10)
			if (Main.tile[i, j].WallType is 187 or 216)
				return true;

		return false;
	}

	public static DesertDescription CreateFromPlacement(Point origin)
	{
		Vector2 defaultBlockScale = DefaultBlockScale;
		float worldSize = Main.maxTilesX / 4200f;
		float worldSizeY = Main.maxTilesY / 1200f;
		int width = (int)(80f * worldSize);
		int height = (int)Math.Min((WorldGen.genRand.NextFloat() + 1f) * 170f * worldSizeY,
			Main.UnderworldLayer - origin.Y);
		int scaledWidth = (int)(defaultBlockScale.X * width);
		int scaledHeight = (int)(defaultBlockScale.Y * height);
		origin.X -= scaledWidth / 2;
		SurfaceMap surfaceMap = SurfaceMap.FromArea(origin.X - 5, scaledWidth + 10);
		if (RowHasInvalidTiles(origin.X, surfaceMap.Bottom, scaledWidth))
			return DesertDescription.Invalid;

		int surfaceBottomStart = (int)(surfaceMap.Average + surfaceMap.Bottom) / 2;
		origin.Y = surfaceBottomStart + WorldGen.genRand.Next(40, 60);
		int specialSeedModifier = WorldGen.tenthAnniversaryWorldGen ? (int)(20f * worldSizeY) : 0;

		DesertDescription placement = (DesertDescription)ConstructorInfo.Invoke(Array.Empty<object>());
		foreach (PropertyInfo propertyInfo in typeof(DesertDescription).GetProperties())
		{
			object? value = propertyInfo.Name switch
			{
				"CombinedArea" => new Rectangle(origin.X, surfaceBottomStart, scaledWidth,
					origin.Y + scaledHeight - surfaceBottomStart),
				"Hive" => new Rectangle(origin.X, origin.Y + specialSeedModifier, scaledWidth,
					scaledHeight - specialSeedModifier),
				"Desert" => new Rectangle(origin.X, surfaceBottomStart, scaledWidth,
					origin.Y + scaledHeight / 2 - surfaceBottomStart + specialSeedModifier),
				"BlockScale" => defaultBlockScale,
				"BlockColumnCount" => width,
				"BlockRowCount" => height,
				"Surface" => surfaceMap,
				"IsValid" => true,
				_ => null
			};

			if (value != null)
				propertyInfo.SetValue(placement, value);
		}

		return placement;
	}

	private static bool RowHasInvalidTiles(int startX, int startY, int width)
	{
		if (GenVars.skipDesertTileCheck)
			return false;

		int xMin = Math.Max(startX, 0);
		int xMax = Math.Min(startX + width, Main.maxTilesX);
		for (int x = xMin; x < xMax; x += 3)
			switch (Main.tile[x, startY].TileType)
			{
				case TileID.Mud:
				case TileID.JungleGrass:
				case TileID.SnowBlock:
				case TileID.IceBlock:
					return true;
			}

		return false;
	}
}