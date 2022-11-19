namespace AdvancedWorldGen.SpecialOptions;

public class EvilReplacer : GenPass
{
	public EvilReplacer() : base("Evil", 200f)
	{
	}

	protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
	{
		progress.Message = Language.GetTextValue("Mods.AdvancedWorldGen.WorldGenMessage.Evil");
		bool isDrunk = OptionHelper.OptionsContains("Drunk.Crimruption");
		bool corruptOnLeft = isDrunk;
		if (isDrunk)
			for (int x = WorldGen.beachDistance; x < Main.maxTilesX - WorldGen.beachDistance; x++)
			for (int y = (int)Main.worldSurface; y < Main.rockLayer; y++)
			{
				Tile tile = Main.tile[x, y];
				if (!tile.HasTile)
					continue;
				ushort tileType = tile.TileType;
				if (TileID.Sets.Corrupt[tileType])
				{
					corruptOnLeft = x < Main.maxTilesX / 2;
					break;
				}

				if (TileID.Sets.Crimson[tileType])
				{
					corruptOnLeft = x >= Main.maxTilesX / 2;
					break;
				}
			}

		int conversionType;
		if (isDrunk)
			conversionType = corruptOnLeft ? 1 : 4;
		else
			conversionType = WorldGen.crimson ? 4 : 1;
		for (int x = 0; x < Main.maxTilesX; x++)
		{
			progress.Set(x, Main.maxTilesX);
			if (x == Main.maxTilesX / 2 && isDrunk)
				conversionType = corruptOnLeft ? 4 : 1;
			for (int y = 0; y < Main.maxTilesY; y++) WorldGen.Convert(x, y, conversionType, 0);
		}
	}
}