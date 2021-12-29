using AdvancedWorldGen.Base;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.SpecialOptions;

public class BothOres
{
	public static bool WasDrunk;

	public static void BothOres1(GenerationProgress progress, GameConfiguration configuration)
	{
		WasDrunk = WorldGen.drunkWorldGen;
		WorldGen.drunkWorldGen = ModifiedWorld.OptionsContains("Drunk.BothOres");
	}

	public static void BothOres2(GenerationProgress progress, GameConfiguration configuration)
	{
		WorldGen.drunkWorldGen = WasDrunk;
	}
}