namespace AdvancedWorldGen.SpecialOptions._100k_special;

public class _100kWorld : ModSystem
{
	public static bool Enabled => false;

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
	{
		if (!Enabled)
			return;
		int index = tasks.FindIndex(pass => pass.Name == "Pyramids");
		if (index != -1 && tasks[index].GetType().Assembly == typeof(PassLegacy).Assembly)
		{
			tasks[index] = new Dimaryp();
			int index2 = tasks.FindIndex(pass => pass.Name == "Floating Islands");
			if (index2 != -1)
			{
				GenPass genPass = tasks[index2];
				tasks.RemoveAt(index2);
				tasks.Insert(++index, genPass);
			}
			index2 = tasks.FindIndex(pass => pass.Name == "Spawn Point");
			if (index2 != -1)
			{
				tasks.RemoveAt(index2);
			}
		}
		
	}

	public static void Replace()
	{
		if (!Enabled)
			return;
		if (!WorldgenSettings.Instance.FasterWorldgen)
			On_WorldGen.FloatingIsland += FloatingIslandOutsideOfDesert.IDontHaveAnyIdeaAsTheClassNameSaysAll;
	}

	public static void Unreplace()
	{
		if (!Enabled)
			return;
		if (!WorldgenSettings.Instance.FasterWorldgen)
			On_WorldGen.FloatingIsland += FloatingIslandOutsideOfDesert.IDontHaveAnyIdeaAsTheClassNameSaysAll;
	}
}