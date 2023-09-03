using AdvancedWorldGen.SpecialOptions._100kSpecial.ColdAndHot;
using AdvancedWorldGen.SpecialOptions._100kSpecial.InvertedPyramid;

namespace AdvancedWorldGen.SpecialOptions._100kSpecial;

public class _100kWorld : ModSystem
{
#if SPECIALDEBUG
	public static bool Enabled => false;
#else
	public static bool Enabled => false;
#endif

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
	{
		if (!Enabled)
			return;
		int index = tasks.FindIndex(pass => pass.Name == "Pyramids");
		if (index != -1)
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

		index = tasks.FindIndex(pass => pass.Name == "Living Trees");
		if (index != -1)
		{
			tasks.Insert(index, new WorldTree());
		}

		index = tasks.FindIndex(pass => pass.Name == "Create Ocean Caves");
		if (index != -1)
		{
			tasks.Insert(index, new Oceans());
		}

		index = tasks.FindIndex(pass => pass.Name == "Corruption");
		if (index != -1)
		{
			tasks.Insert(index + 1, new ReplaceCorruption100k());
			totalWeight += 1;
		}

		index = tasks.FindIndex(pass => pass.Name == "Full Desert");
		if (index != -1)
		{
			tasks.Insert(index + 1, new BiomeExchanger());
			totalWeight += 1;
		}
	}

	public static void WorldgenReplace()
	{
		if (!Enabled)
			return;
		if (!WorldgenSettings.Instance.FasterWorldgen)
			On_WorldGen.FloatingIsland += FloatingIslandOutsideOfDesert.IDontHaveAnyIdeaAsTheClassNameSaysAll;
	}

	public static void WorldgenUnreplace()
	{
		if (!Enabled)
			return;
		if (!WorldgenSettings.Instance.FasterWorldgen)
			On_WorldGen.FloatingIsland -= FloatingIslandOutsideOfDesert.IDontHaveAnyIdeaAsTheClassNameSaysAll;
	}

	public static void IngameReplace()
	{
		if (!Enabled)
			return;
		On_WorldGen.CheckCactus += Cactus.CheckCactus;
		On_WorldGen.GrowCactus += Cactus.GrowCactus;
	}

	public static void IngameUnreplace()
	{
		if (!Enabled)
			return;
		On_WorldGen.CheckCactus -= Cactus.CheckCactus;
		On_WorldGen.GrowCactus -= Cactus.GrowCactus;
	}
}