using System.Reflection;
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
			FieldInfo drunk = typeof(WorldGen).GetField("drunkWorldGen");

			ILCursor cursor = new(il);

			for (int i = 0; i < 3; i++)
				if (!cursor.TryGotoNext(MoveType.Before, instruction => instruction.MatchLdsfld(drunk)))
					return;

			OrOptionContainsCrimruption(cursor);
		}

		public void OrOptionContainsCrimruption(ILCursor cursor)
		{
			cursor.Remove();
			ILHelper.OptionContains(cursor, "Crimruption", "Drunk");
		}

		public static void Crimruption1(GenerationProgress progress, GameConfiguration configuration)
		{
			WasDrunk = WorldGen.drunkWorldGen;
			WorldGen.drunkWorldGen = true;
		}

		public static void Crimruption2(GenerationProgress progress, GameConfiguration configuration)
		{
			WorldGen.drunkWorldGen = WasDrunk;
		}
	}
}