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

		//After IL_19af : || OptionContains("Crimruption")
		public void CrimruptionChest(ILContext il)
		{
			ILCursor cursor = new(il);

			for (int i = 0; i < 3; i++)
				if (!cursor.TryGotoNext(MoveType.Before, instruction => instruction.MatchLdsfld<WorldGen>("drunkWorldGen")))
					return;

			OrOptionContainsCrimruption(cursor);
		}

		public void OrOptionContainsCrimruption(ILCursor cursor)
		{
			cursor.Remove();
			cursor.OptionContains("Crimruption", "Drunk");
		}

		public static void MakeDrunk(GenerationProgress progress, GameConfiguration configuration)
		{
			WasDrunk = WorldGen.drunkWorldGen;
			WorldGen.drunkWorldGen = true;
		}

		public static void UnmakeDrunk(GenerationProgress progress, GameConfiguration configuration)
		{
			WorldGen.drunkWorldGen = WasDrunk;
		}
	}
}