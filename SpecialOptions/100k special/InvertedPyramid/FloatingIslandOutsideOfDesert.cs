namespace AdvancedWorldGen.SpecialOptions._100k_special.InvertedPyramid;

public class FloatingIslandOutsideOfDesert : ControlledWorldGenPass
{
	public FloatingIslandOutsideOfDesert() : base("Remove Floating Islands Houses", 0f)
	{
	}

	protected override void ApplyPass()
	{
		int numIslandHouses = GenVars.numIslandHouses;

		int n = 0;
		bool[] skyLake = new bool[30];
		int[] floatingIslandHouseX = new int[30];
		int[] floatingIslandHouseY = new int[30];
		int[] floatingIslandStyle = new int[30];
		for (int i = 0; i < numIslandHouses; i++)
		{
			int x = GenVars.floatingIslandHouseX[i];
			if (x < GenVars.UndergroundDesertLocation.Left - 100 || x > GenVars.UndergroundDesertLocation.Right + 100)
			{
				skyLake[n] = GenVars.skyLake[i];
				floatingIslandHouseX[n] = GenVars.floatingIslandHouseX[i];
				floatingIslandHouseY[n] = GenVars.floatingIslandHouseY[i];
				floatingIslandStyle[n] = GenVars.floatingIslandStyle[i];
				n++;
			}
		}
		GenVars.skyLake = skyLake;
		GenVars.floatingIslandHouseX = floatingIslandHouseX;
		GenVars.floatingIslandHouseY = floatingIslandHouseY;
		GenVars.floatingIslandStyle = floatingIslandStyle;
		GenVars.numIslandHouses = n;
	}

	public static void IDontHaveAnyIdeaAsTheClassNameSaysAll(On_WorldGen.orig_FloatingIsland orig, int i, int j)
	{
		if (i < GenVars.UndergroundDesertLocation.Left - 100 || i > GenVars.UndergroundDesertLocation.Right + 100)
		{
			orig(i, j);
		}
	}
}