namespace AdvancedWorldGen.BetterVanillaWorldGen;

public static partial class Replacer
{
	public static VanillaInterface VanillaInterface = null!;

	public static void ReplaceGenPasses(List<GenPass> genPasses)
	{
		if (!WorldgenSettings.Revamped)
			return;
		int index = genPasses.FindIndex(pass => pass.Name == "Reset");
		if (index != -1)
		{
			VanillaInterface = new VanillaInterface();
			genPasses[index] = new ResetPass();
		}
		else
		{
			return;
		}

		index = genPasses.FindIndex(pass => pass.Name == "Terrain");
		if (index != -1) genPasses[index] = new TerrainPass();

		index = genPasses.FindIndex(pass => pass.Name == "Generate Ice Biome");
		if (index != -1) genPasses[index] = new GenerateIceBiome();

		index = genPasses.FindIndex(pass => pass.Name == "Jungle");
		if (index != -1) genPasses[index] = new ModifiedJunglePass();

		index = genPasses.FindIndex(pass => pass.Name == "Floating Islands");
		if (index != -1) genPasses[index] = new FloatingIslands();

		index = genPasses.FindIndex(pass => pass.Name == "Mushroom Patches");
		if (index != -1) genPasses[index] = new MushroomPatches();

		index = genPasses.FindIndex(pass => pass.Name == "Corruption");
		if (index != -1) genPasses[index] = new Corruption();

		index = genPasses.FindIndex(pass => pass.Name == "Dungeon");
		if (index != -1) genPasses[index] = new DungeonPass();

		index = genPasses.FindIndex(pass => pass.Name == "Jungle Temple");
		if (index != -1) genPasses[index] = new JungleTemple();

		index = genPasses.FindIndex(pass => pass.Name == "Jungle Chests");
		if (index != -1) genPasses[index] = new JungleChests();

		index = genPasses.FindIndex(pass => pass.Name == "Shell Piles");
		if (index != -1) genPasses[index] = new ShellPiles();

		index = genPasses.FindIndex(pass => pass.Name == "Surface Ore and Stone");
		if (index != -1) genPasses[index] = new SurfaceOreAndStone();

		index = genPasses.FindIndex(pass => pass.Name == "Micro Biomes");
		if (index != -1)
		{
			for (int i = 1; i <= 8; i++)
			{
				MicroBiomes microBiomes = new(i);
				genPasses.Insert(index + i, microBiomes);
			}

			genPasses.Insert(index + 9, new MicroBiomes(-1)); //For mod compatibility sake
			genPasses.RemoveAt(index);
		}

		GenPasses = genPasses;
	}
}