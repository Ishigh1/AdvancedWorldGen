using AdvancedWorldGen.Base;
using AdvancedWorldGen.Helper;
using MonoMod.Cil;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.SpecialOptions
{
	public class Crimruption
	{
		public static bool WasDrunk;

		//After IL_19af : OptionContains("Drunk.Crimruption")
		public void CrimruptionChest(ILContext il)
		{
			ILCursor cursor = new(il);

			for (int i = 0; i < 3; i++)
				cursor.GotoNext(MoveType.Before, instruction => instruction.MatchLdsfld<WorldGen>("drunkWorldGen"));

			OrOptionContainsCrimruption(cursor);
		}

		public void OrOptionContainsCrimruption(ILCursor cursor)
		{
			cursor.Remove();
			cursor.OptionContains("Drunk.Crimruption");
		}

		public static void Crimruption1(GenerationProgress progress, GameConfiguration configuration)
		{
			WasDrunk = WorldGen.drunkWorldGen;
			WorldGen.drunkWorldGen = ModifiedWorld.OptionsContains("Drunk.Crimruption");
		}

		public static void Crimruption2(GenerationProgress progress, GameConfiguration configuration)
		{
			WorldGen.drunkWorldGen = WasDrunk;
		}

		public static void Crimruption3(GenerationProgress progress, GameConfiguration configuration)
		{
			WasDrunk = WorldGen.drunkWorldGen;
			WorldGen.drunkWorldGen = ModifiedWorld.OptionsContains("Drunk.Crimruption");
		}

		public static void Crimruption4(GenerationProgress progress, GameConfiguration configuration)
		{
			WorldGen.drunkWorldGen = WasDrunk;
		}
	}
}