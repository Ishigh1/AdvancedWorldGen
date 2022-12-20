namespace AdvancedWorldGen.SpecialOptions;

public class DrunkOptions
{
	private static bool WasDrunk;

	//After IL_19af : OptionContains("Drunk.Crimruption")
	public static void CrimruptionChest(ILContext il)
	{
		ILCursor cursor = new(il);

		for (int i = 0; i < 3; i++)
			cursor.GotoNext(MoveType.Before, instruction => instruction.MatchLdsfld<WorldGen>("drunkWorldGen"));

		OrOptionContainsCrimruption(cursor);
	}

	private static void OrOptionContainsCrimruption(ILCursor cursor)
	{
		cursor.Remove();
		cursor.OptionContains("Drunk.Crimruption");
	}

	public static void AddDrunkEdits(List<GenPass> tasks)
	{
		if (OptionHelper.OptionsContains("Drunk.BothOres"))
		{
			int passIndex = tasks.FindIndex(pass => pass.Name == "Shinies");
			if (passIndex != -1)
			{
				tasks.Insert(passIndex, new PassLegacy("BothOres1", BothOres1));
				tasks.Insert(passIndex + 2, new PassLegacy("BothOres2", BothOres2));
			}
		}

		if (OptionHelper.OptionsContains("Drunk.Crimruption"))
		{
			int passIndex;
			if (!WorldgenSettings.Revamped)
			{
				passIndex = tasks.FindIndex(pass => pass.Name == "Corruption");
				if (passIndex != -1)
				{
					tasks.Insert(passIndex, new PassLegacy("Crimruption1", Crimruption1));
					tasks.Insert(passIndex + 2, new PassLegacy("Crimruption2", Crimruption2));
				}
			}

			passIndex = tasks.FindIndex(pass => pass.Name == "Tile Cleanup");
			if (passIndex != -1)
			{
				tasks.Insert(passIndex, new PassLegacy("Crimruption3", Crimruption1));
				tasks.Insert(passIndex + 2, new PassLegacy("Crimruption4", Crimruption2));
			}
		}

		if (OptionHelper.OptionsContains("Drunk.MiddleLavaOcean"))
		{
			int passIndex = tasks.FindIndex(pass => pass.Name == "Underworld");
			if (passIndex != -1)
			{
				tasks.Insert(passIndex, new PassLegacy("Crimruption1", Hell1));
				tasks.Insert(passIndex + 2, new PassLegacy("Crimruption2", Hell2));
			}
		}
	}

	private static void Crimruption1(GenerationProgress progress, GameConfiguration configuration)
	{
		WasDrunk = WorldGen.drunkWorldGen;
		WorldGen.drunkWorldGen = OptionHelper.OptionsContains("Drunk.Crimruption");
	}

	private static void Crimruption2(GenerationProgress progress, GameConfiguration configuration)
	{
		WorldGen.drunkWorldGen = WasDrunk;
	}

	private static void Hell1(GenerationProgress progress, GameConfiguration configuration)
	{
		WasDrunk = WorldGen.drunkWorldGen;
		WorldGen.drunkWorldGen = OptionHelper.OptionsContains("Drunk.MiddleLavaOcean");
	}

	private static void Hell2(GenerationProgress progress, GameConfiguration configuration)
	{
		WorldGen.drunkWorldGen = WasDrunk;
	}

	private static void BothOres1(GenerationProgress progress, GameConfiguration configuration)
	{
		WasDrunk = WorldGen.drunkWorldGen;
		WorldGen.drunkWorldGen = OptionHelper.OptionsContains("Drunk.BothOres");
	}

	private static void BothOres2(GenerationProgress progress, GameConfiguration configuration)
	{
		WorldGen.drunkWorldGen = WasDrunk;
	}
}