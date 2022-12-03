namespace AdvancedWorldGen.SpecialOptions;

public class ClassicOptions
{
	public static void SmallNotTheBees(On_WorldGen.orig_NotTheBees orig)
	{
		if (OptionHelper.OptionsContains("NotTheBees.JungleWorld"))
		{
			bool wasNotTheBees = WorldGen.notTheBees;
			WorldGen.notTheBees = true;
			orig();
			WorldGen.notTheBees = wasNotTheBees;
		}
	}
}