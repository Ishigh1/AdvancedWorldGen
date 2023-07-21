namespace AdvancedWorldGen.SpecialOptions._100kSpecial.ColdAndHot;

public class BiomeExchanger : ControlledWorldGenPass
{
	public BiomeExchanger() : base("Exchanging desert and snow", 1f)
	{
	}

	protected override void ApplyPass()
	{
		int left = 0;
		int right = 0;
		for (int y = 0; y < Main.maxTilesY; y++)
		{
			if (GenVars.snowMinX[y] != 0)
			{
				left = GenVars.snowMinX[y] - 5;
				right = GenVars.snowMaxX[y] + 5;
			}
			for (int x = left; x < right; x++)
			{
				Tile tile = Main.tile[x, y];
				if (tile.HasTile)
				{
					ushort type = tile.TileType;
					if (TileID.Sets.Conversion.Ice[type] || TileID.Sets.Conversion.Snow[type])
					{
						WorldGen.Convert(x, y, BiomeConversionID.Sand, size:0);
					}
				}
			}
		}

		for (int x = GenVars.desertHiveLeft; x < GenVars.desertHiveRight; x++)
		{
			for (int y = (int)GenVars.worldSurfaceLow; y < GenVars.desertHiveLow; y++)
			{
				Tile tile = Main.tile[x, y];
				if (tile.HasTile)
				{
					ushort type = tile.TileType;
					if (TileID.Sets.Conversion.Sand[type] || TileID.Sets.Conversion.Sandstone[type] || TileID.Sets.Conversion.HardenedSand[type])
					{
						WorldGen.Convert(x, y, BiomeConversionID.Snow, size:0);
					}
				}
			}
		}
	}
}