namespace AdvancedWorldGen.SpecialOptions.Snow;

public class SnowReplacer : TileReplacer
{
	public SnowReplacer() : base("SnowReplace")
	{
		UpdateDictionary(TileID.SnowBlock, TileID.Dirt, TileID.Grass, TileID.CorruptGrass,
			TileID.ClayBlock, TileID.CrimsonGrass);
		UpdateDictionary(TileID.IceBlock, TileID.Stone, TileID.GreenMoss, TileID.BrownMoss,
			TileID.RedMoss, TileID.BlueMoss, TileID.PurpleMoss, TileID.LavaMoss, TileID.KryptonMoss,
			TileID.XenonMoss, TileID.ArgonMoss);
		UpdateDictionary(TileID.CorruptIce, TileID.Ebonstone);
		UpdateDictionary(TileID.FleshIce, TileID.Crimstone);
		UpdateDictionary(TileID.BorealWood, TileID.WoodBlock);
		UpdateDictionary(TileID.BreakableIce, Water);
		UpdateDictionary(TileID.Slush, TileID.Silt);
		UpdateDictionary(TileID.Trees, TileID.VanityTreeSakura, TileID.VanityTreeYellowWillow);
		UpdateDictionary(None, TileID.Plants, TileID.CorruptPlants, TileID.Sunflower, TileID.Vines,
			TileID.Plants2, TileID.CrimsonPlants, TileID.CrimsonVines, TileID.VineFlowers, TileID.CorruptThorns,
			TileID.CrimsonThorns);
		SpecialCases.Add(TileID.ImmatureHerbs, new SpecialCase(None,
			(_, _, tile) => tile.TileFrameX is 0 or 32 or 32 * 2 or 32 * 3 or 32 * 6));
		SpecialCases.Add(TileID.MatureHerbs, new SpecialCase(None,
			(_, _, tile) => tile.TileFrameX is 0 or 32 or 32 * 2 or 32 * 3 or 32 * 6));
		SpecialCases.Add(TileID.BloomingHerbs, new SpecialCase(None,
			(_, _, tile) => tile.TileFrameX is 0 or 32 or 32 * 2 or 32 * 3 or 32 * 6));
		SpecialCases.Add(TileID.Cattail, new SpecialCase(None,
			(_, _, tile) => tile.TileFrameY is 0 or 32 * 3 or 32 * 4));
		SpecialCases.Add(TileID.LilyPad, new SpecialCase(None,
			(_, _, tile) => tile.TileFrameY is 0 or 32 * 3 or 32 * 4));
		SpecialCases.Add(TileID.DyePlants, new SpecialCase(None,
			(_, _, tile) => tile.TileFrameX is 32 * 3 or 32 * 4 or 32 * 7));
	}
}