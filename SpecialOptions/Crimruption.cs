using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using Terraria;

namespace AdvancedWorldGen.SpecialOptions
{
	public class Crimruption
	{
		public MethodInfo CorruptionGen;
		public FieldInfo Drunk;

		public event ILContext.Manipulator OnIlCorruptionGeneration
		{
			add => HookEndpointManager.Modify(CorruptionGen, value);
			remove => HookEndpointManager.Unmodify(CorruptionGen, value);
		}

		public void Load()
		{
			Assembly assembly = typeof(Main).Assembly;

			Type type = assembly.GetType("Terraria.WorldGen+<>c__DisplayClass343_0");
			CorruptionGen = type.GetMethod("<GenerateWorld>b__31", BindingFlags.Instance | BindingFlags.NonPublic);

			Drunk = typeof(WorldGen).GetField("drunkWorldGen");

			OnIlCorruptionGeneration += CrimruptionBiomes;
		}

		public void Unload()
		{
			OnIlCorruptionGeneration -= CrimruptionBiomes;
		}

		//After IL_010b and IL_09f3 : || OptionContains("Crimruption")IL_19af
		public void CrimruptionBiomes(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);


			for (int i = 0; i < 3; i++)
			{
				if (!cursor.TryGotoNext(MoveType.After, instruction => instruction.MatchLdsfld(Drunk))) return;
				if (i == 1) continue;

				OrOptionContainsCrimruption(cursor);
			}
		}

		//After IL_19af : || OptionContains("Crimruption")
		public void CrimruptionChest(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			for (int i = 0; i < 3; i++)
				if (!cursor.TryGotoNext(MoveType.After, instruction => instruction.MatchLdsfld(Drunk)))
					return;

			OrOptionContainsCrimruption(cursor);
		}

		private static void OrOptionContainsCrimruption(ILCursor cursor)
		{
			ILLabel label = cursor.DefineLabel();
			cursor.Emit(OpCodes.Brtrue_S, label);
			ILHelper.OptionContains(cursor, "Crimruption");

			cursor.GotoNext();
			cursor.GotoNext();
			cursor.MarkLabel(label);
		}
	}
}