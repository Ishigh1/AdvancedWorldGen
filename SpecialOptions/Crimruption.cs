using System;
using System.Reflection;
using AdvancedWorldGen.Base;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using Terraria;
using ILWorldGen = IL.Terraria.WorldGen;

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
			//Two options to IL edit a worldgen pass : 

			//Option 1 : 
			/*Assembly assembly = typeof(Main).Assembly;

			Type type = assembly.GetType("Terraria.WorldGen+<>c__DisplayClass343_0");
			CorruptionGen = type.GetMethod("<GenerateWorld>b__31", BindingFlags.Instance | BindingFlags.NonPublic);*/

			//Option 2 : 
			ILWorldGen.GenerateWorld += GetMethodInfo;
			ILWorldGen.GenerateWorld -= GetMethodInfo;


			Drunk = typeof(WorldGen).GetField("drunkWorldGen");

			OnIlCorruptionGeneration += CrimruptionBiomes;
		}

		private void GetMethodInfo(ILContext il)
		{
			ILCursor cursor = new(il);
			if (!cursor.TryGotoNext(MoveType.After, instruction => instruction.MatchLdstr("Corruption"))) return;
			if (!cursor.TryGotoNext(instruction => instruction.OpCode == OpCodes.Ldftn)) return;
			MethodReference methodReference = (MethodReference) cursor.Next.Operand;

			Assembly assembly = typeof(Main).Assembly;
			Type type = assembly.GetType("Terraria.WorldGen+" + methodReference.DeclaringType.Name);
			CorruptionGen = type.GetMethod(methodReference.Name, BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public void Unload()
		{
			OnIlCorruptionGeneration -= CrimruptionBiomes;
		}

		//After IL_010b and IL_09f3 : || OptionContains("Crimruption")IL_19af
		public void CrimruptionBiomes(ILContext il)
		{
			ILCursor cursor = new(il);


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
			ILCursor cursor = new(il);

			for (int i = 0; i < 3; i++)
				if (!cursor.TryGotoNext(MoveType.After, instruction => instruction.MatchLdsfld(Drunk)))
					return;

			OrOptionContainsCrimruption(cursor);
		}

		public static void OrOptionContainsCrimruption(ILCursor cursor)
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