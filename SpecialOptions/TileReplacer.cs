namespace AdvancedWorldGen.SpecialOptions;

public class TileReplacer
{
	public const int None = -1;
	public const int Water = -2;
	public const int Lava = -3;
	public const int Honey = -4;

	public static HashSet<int> NotReplaced = null!;

	public Dictionary<int, int> DirectReplacements;
	public Dictionary<int, SpecialCase> SpecialCases;

	public TileReplacer()
	{
		DirectReplacements = new Dictionary<int, int>();
		SpecialCases = new Dictionary<int, SpecialCase>();
	}

	public static void Initialize()
	{
		NotReplaced = new HashSet<int>
		{
			TileID.ClosedDoor,
			TileID.MagicalIceBlock,
			TileID.Traps,
			TileID.Boulder,
			TileID.Teleporter,
			TileID.MetalBars,
			TileID.PlanterBox,
			TileID.TrapdoorClosed,
			TileID.TallGateClosed
		};
		IEnumerable<ModTile> modTiles = ModLoader.GetMod("ModLoader").GetContent<ModTile>();

		foreach (ModTile modTile in modTiles) NotReplaced.Add(modTile.Type);
	}

	public static bool DontReplace(int type)
	{
		return !Main.tileSolid[type] || TileID.Sets.Platforms[type] || NotReplaced.Contains(type) ||
		       Main.tileFrameImportant[type];
	}

	public void UpdateDictionary(int to, params int[] from)
	{
		foreach (int tile in from) DirectReplacements.Add(tile, to);
	}

	public void ReplaceTiles(GenerationProgress progress, string s)
	{
		progress.Message = Language.GetTextValue("Mods.AdvancedWorldGen.WorldGenMessage." + s);
		for (int x = 0; x < Main.maxTilesX; x++)
		{
			progress.Set(x, Main.maxTilesX);
			for (int y = 0; y < Main.maxTilesY; y++)
			{
				Tile tile = Main.tile[x, y];
				if (tile == null) continue;
				if (tile.HasTile) HandleReplacement(tile.TileType, x, y, tile);

				if (tile.LiquidAmount > 0)
					HandleReplacement(-tile.LiquidType - 2, x, y, tile);
			}
		}
	}

	public void HandleReplacement(int tileType, int x, int y, Tile tile)
	{
		if (!DirectReplacements.TryGetValue(tileType, out int type))
		{
			if (!SpecialCases.TryGetValue(tileType, out SpecialCase? specialCase) || !specialCase.IsValid(x, y, tile))
				return;
			type = specialCase.Type;
		}

		if (tileType < -1)
			tile.LiquidAmount = 0;
		else
			tile.HasTile = false;

		switch (type)
		{
			case > -1:
				if (!tile.HasTile)
				{
					tile.HasTile = true;
					tile.TileType = (ushort)type;
					WorldGen.DiamondTileFrame(x, y);
				}

				break;
			case < -1:
				tile.LiquidAmount = byte.MaxValue;
				tile.LiquidType = -type + 2;
				break;
		}
	}

	public static void RandomizeWorld(GenerationProgress progress)
	{
		Dictionary<ushort, ushort> tileRandom = new();
		Dictionary<ushort, ushort> wallRandom = new();
		Dictionary<ushort, byte> paintRandom = new();
		Dictionary<ushort, byte> paintWallRandom = new();

		progress.Message = Language.GetTextValue("Mods.AdvancedWorldGen.WorldGenMessage.Random");
		for (int x = 0; x < Main.maxTilesX; x++)
		{
			progress.Set(x, Main.maxTilesX, 0.5f);
			for (int y = 0; y < Main.maxTilesY; y++)
			{
				Tile tile = Main.tile[x, y];
				if (tile != null)
				{
					if (tile.HasTile) RandomizeTile(tile, tileRandom, paintRandom);

					if (tile.WallType != 0) RandomizeWall(wallRandom, tile, paintWallRandom);
				}
			}
		}

		for (int x = 0; x < Main.maxTilesX; x++)
		{
			progress.Set(x, Main.maxTilesX, 0.5f, 0.5f);
			int previousBlock = 0;
			for (int y = Main.maxTilesY - 1; y >= 1; y--)
			{
				Tile tile = Main.tile[x, y];
				if (!tile.HasTile)
					continue;
				if (tile.TileType == TileID.Cactus)
				{
					WorldGen.CheckCactus(x, y);
					continue;
				}

				if (previousBlock != 0 && y != previousBlock - 1 &&
				    TileID.Sets.Falling[tile.TileType])
				{
					Tile tileAbove = Main.tile[x, y - 1];
					if (tileAbove.HasTile && !Main.tileSolid[tileAbove.TileType])
					{
						WorldGen.KillTile(x, y - 1);
						if (!tileAbove.HasTile)
						{
							previousBlock = y;
							continue;
						}
					}

					Tile newPos = Main.tile[x, previousBlock - 1];
					newPos.HasTile = true;
					newPos.TileType = tile.TileType;
					newPos.IsHalfBlock = tile.IsHalfBlock;
					newPos.Slope = tile.Slope;
					tile.HasTile = false;
					previousBlock--;
				}
				else
				{
					previousBlock = y;
				}
			}
		}
	}

	public static void RandomizeTile(Tile tile, Dictionary<ushort, ushort> tileRandom,
		Dictionary<ushort, byte> paintRandom)
	{
		if (API.OptionsContains("Random"))
			if (Main.tileSolid[tile.TileType])
			{
				if (tileRandom.TryGetValue(tile.TileType, out ushort type))
				{
					tile.TileType = type;
				}
				else if (!DontReplace(type))
				{
					do
					{
						type = (ushort)WorldGen.genRand.Next(TileLoader.TileCount);
					} while (DontReplace(type) || tileRandom.ContainsValue(type));

					tileRandom[tile.TileType] = type;
					tile.TileType = type;
				}
			}

		if (API.OptionsContains("Random.Painted"))
		{
			if (!paintRandom.TryGetValue(tile.TileType, out byte paint))
			{
				paint = (byte)WorldGen.genRand.Next(PaintID.IlluminantPaint + 1);
				paintRandom[tile.TileType] = paint;
			}

			tile.TileColor = paint;
		}
	}

	public static void RandomizeWall(Dictionary<ushort, ushort> wallRandom, Tile tile,
		Dictionary<ushort, byte> paintWallRandom)
	{
		if (API.OptionsContains("Random"))
		{
			if (wallRandom.TryGetValue(tile.WallType, out ushort type))
			{
				tile.WallType = type;
			}
			else
			{
				do
				{
					type = (ushort)WorldGen.genRand.Next(1, WallLoader.WallCount);
				} while (wallRandom.ContainsValue(type));

				wallRandom[tile.WallType] = type;
				tile.WallType = type;
			}
		}

		if (API.OptionsContains("Random.Painted"))
		{
			if (!paintWallRandom.TryGetValue(tile.WallType, out byte paint))
			{
				paint = (byte)WorldGen.genRand.Next(PaintID.IlluminantPaint + 1);
				paintWallRandom[tile.WallType] = paint;
			}

			tile.WallColor = paint;
		}
	}
}