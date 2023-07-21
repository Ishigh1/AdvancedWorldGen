namespace AdvancedWorldGen.SpecialOptions._100kSpecial;

public class ReplaceCorruption100k : ControlledWorldGenPass
{
	public ReplaceCorruption100k() : base("Change Orbs", 1f)
	{
	}

	protected override void ApplyPass()
	{
		for (int x = 5; x < Main.maxTilesX - 5; x++)
		for (int y = 5; y < Main.UnderworldLayer; y++)
		{
			Tile tile = Main.tile[x, y];
			if (tile.HasTile && tile.TileFrameY == 0)
			{
				ushort tileType = tile.TileType;
				switch (tileType)
				{
					case TileID.ShadowOrbs:
						if (tile.TileFrameX % 36 == 0)
						{
							int style = tile.TileFrameX / 36;
							if (_random.NextBool(2))
							{
								Delete3x2(x - 1, y);
								PlaceFloor(x - 1, y + 2, style);
								WorldGen.Place3x2(x, y + 1, TileID.DemonAltar, style);
							}
							else
							{
								Delete3x2(x, y);
								PlaceFloor(x, y + 2, style);
								WorldGen.Place3x2(x + 1, y + 1, TileID.DemonAltar, style);
							}

							x += 2;
						}

						break;
					case TileID.DemonAltar:
						if (tile.TileFrameX % 54 == 0)
						{
							Delete3x2(x, y);
							int style = tile.TileFrameX / 54;
							if (_random.NextBool(2))
							{
								WorldGen.Place2x2(x + 1, y + 1, TileID.ShadowOrbs, style);
							}
							else
							{
								WorldGen.Place2x2(x + 2, y + 1, TileID.ShadowOrbs, style);
							}

							x += 2;
						}

						break;
				}
			}
		}
	}

	private static void Delete3x2(int x, int y)
	{
		for (int i = 0; i < 3; i++)
		for (int j = 0; j < 2; j++)
		{
			WorldGen.KillTile(x + i, y + j);
		}
	}

	private static void PlaceFloor(int x, int y, int style)
	{
		int tileType = style == 0 ? TileID.Ebonstone : TileID.Crimstone;
		for (int i = 0; i < 3; i++)
		{
			WorldGen.PlaceTile(x + i, y, tileType);
		}
	}
}