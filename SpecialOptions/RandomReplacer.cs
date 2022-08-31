namespace AdvancedWorldGen.SpecialOptions;

public class RandomReplacer : GenPass
{
	public RandomReplacer() : base("Random", 200f)
	{
	}
	
	protected override void ApplyPass(GenerationProgress progress, GameConfiguration gameConfiguration)
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
				if (tile.HasTile) RandomizeTile(tile, tileRandom, paintRandom);

				if (tile.WallType != 0) RandomizeWall(wallRandom, tile, paintWallRandom);
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
		if (OptionHelper.OptionsContains("Random"))
			if (Main.tileSolid[tile.TileType])
			{
				if (tileRandom.TryGetValue(tile.TileType, out ushort type))
				{
					tile.TileType = type;
				}
				else if (!TileReplacer.DontReplace(type))
				{
					do
					{
						type = (ushort)WorldGen.genRand.Next(TileLoader.TileCount);
					} while (TileReplacer.DontReplace(type) || tileRandom.ContainsValue(type));

					tileRandom[tile.TileType] = type;
					tile.TileType = type;
				}
			}

		if (OptionHelper.OptionsContains("Random.Painted"))
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
		if (OptionHelper.OptionsContains("Random"))
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

		if (OptionHelper.OptionsContains("Random.Painted"))
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