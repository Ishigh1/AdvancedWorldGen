namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class Gems : ControlledWorldGenPass
{
	public Gems() : base("Gems", 895.426f)
	{
	}

	protected override void ApplyPass()
	{
		Progress.Message = Lang.gen[23].Value;
		Main.tileSolid[484] = false;
		for (int gemType = 63; gemType <= 68; gemType++)
		{
			double value13 = (gemType - 63) / 6.0;
			Progress.Set(value13);
			double gemClusters = gemType switch
			{
				67 => Main.maxTilesX * 0.5,
				66 => Main.maxTilesX * 0.45,
				63 => Main.maxTilesX * 0.3,
				65 => Main.maxTilesX * 0.25,
				64 => Main.maxTilesX * 0.1,
				68 => Main.maxTilesX * 0.05,
				_ => 0.0
			};

			gemClusters *= 0.2;
			for (int num720 = 0; num720 < gemClusters; num720++)
			{
				int num721 = WorldGen.genRand.Next(0, Main.maxTilesX);
				int num722 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
				while (Main.tile[num721, num722].TileType != 1)
				{
					num721 = WorldGen.genRand.Next(0, Main.maxTilesX);
					num722 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
				}

				WorldGen.TileRunner(num721, num722, WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(3, 7), gemType);
			}
		}

		int yBorder = Main.maxTilesY - 5;
		for (int directionId = 0; directionId < 2; directionId++)
		{
			int direction;
			int startX;
			int border;
			if (directionId == 1)
			{
				direction = -1;
				startX = Main.maxTilesX - 5;
				border = 5;
			}
			else
			{
				direction = 1;
				startX = 5;
				border = Main.maxTilesX - 5;
			}

			for (int x = startX; x != border; x += direction)
			{
				if (x <= GenVars.UndergroundDesertLocation.Left ||
				    x >= GenVars.UndergroundDesertLocation.Right)
				{
					for (int y = 10; y < Main.maxTilesY - 10; y++)
					{
						if (Main.tile[x, y].HasTile && Main.tile[x, y + 1].HasTile &&
						    Main.tileSand[Main.tile[x, y].TileType] &&
						    Main.tileSand[Main.tile[x, y + 1].TileType])
						{
							int exchangeX = x + direction;
							int exchangeY = y + 1;
							if (!Main.tile[exchangeX, y].HasTile && !Main.tile[exchangeX, exchangeY].HasTile)
							{
								for (; y < yBorder && !Main.tile[exchangeX, exchangeY].HasTile; exchangeY++)
								{
								}

								Tile tile = Main.tile[x, y];
								tile.HasTile = false;
								if (y != yBorder)
								{
									Tile tile1 = Main.tile[exchangeX, exchangeY - 1];
									tile1.HasTile = true;
									tile1.TileType = tile.TileType;
								}
							}
						}
					}
				}
				else if (direction == -1)
					x = GenVars.UndergroundDesertLocation.Left + 1;
				else
					x = GenVars.UndergroundDesertLocation.Right - 1;
			}
		}
	}
}