namespace AdvancedWorldGen.SpecialOptions;

public class DrunkOptions
{
	public static bool WasDrunk;

	//After IL_19af : OptionContains("Drunk.Crimruption")
	public static void CrimruptionChest(ILContext il)
	{
		ILCursor cursor = new(il);

		for (int i = 0; i < 3; i++)
			cursor.GotoNext(MoveType.Before, instruction => instruction.MatchLdsfld<WorldGen>("drunkWorldGen"));

		OrOptionContainsCrimruption(cursor);
	}

	public static void OrOptionContainsCrimruption(ILCursor cursor)
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
				tasks[passIndex].OnBegin(BothOres1);
				tasks[passIndex].OnComplete(BothOres2);
			}
		}

		if (!WorldgenSettings.Revamped && OptionHelper.OptionsContains("Drunk.Crimruption"))
		{
			int passIndex = tasks.FindIndex(pass => pass.Name == "Corruption");
			if (passIndex != -1)
			{
				tasks[passIndex].OnBegin(Crimruption1);
				tasks[passIndex].OnComplete(Crimruption2);
			}

			passIndex = tasks.FindIndex(pass => pass.Name == "Tile Cleanup");
			if (passIndex != -1)
			{
				tasks[passIndex].OnBegin(Crimruption3);
				tasks[passIndex].OnComplete(Crimruption4);
			}
		}

		if (OptionHelper.OptionsContains("Drunk.MiddleLavaOcean"))
		{
			int passIndex = tasks.FindIndex(pass => pass.Name == "Underworld");
			if (passIndex != -1)
			{
				tasks[passIndex].OnBegin(Hell1);
				tasks[passIndex].OnComplete(Hell2);
			}
		}
	}

	public static void Crimruption1(GenPass genPass)
	{
		WasDrunk = WorldGen.drunkWorldGen;
		WorldGen.drunkWorldGen = OptionHelper.OptionsContains("Drunk.Crimruption");
	}

	public static void Crimruption2(GenPass genPass)
	{
		WorldGen.drunkWorldGen = WasDrunk;
	}

	public static void Crimruption3(GenPass genPass)
	{
		WasDrunk = WorldGen.drunkWorldGen;
		WorldGen.drunkWorldGen = OptionHelper.OptionsContains("Drunk.Crimruption");
	}

	public static void Crimruption4(GenPass genPass)
	{
		WorldGen.drunkWorldGen = WasDrunk;
	}

	public static void Hell1(GenPass genPass)
	{
		WasDrunk = WorldGen.drunkWorldGen;
		WorldGen.drunkWorldGen = OptionHelper.OptionsContains("Drunk.MiddleLavaOcean");
	}

	public static void Hell2(GenPass genPass)
	{
		WorldGen.drunkWorldGen = WasDrunk;
	}

	public static void BothOres1(GenPass genPass)
	{
		WasDrunk = WorldGen.drunkWorldGen;
		WorldGen.drunkWorldGen = OptionHelper.OptionsContains("Drunk.BothOres");
	}

	public static void BothOres2(GenPass genPass)
	{
		WorldGen.drunkWorldGen = WasDrunk;
	}
}