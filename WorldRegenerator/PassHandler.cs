using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using OnWorldGen = On.Terraria.WorldGen;
using ILWorldGen = IL.Terraria.WorldGen;

namespace AdvancedWorldGen.WorldRegenerator
{
	public static class PassHandler
	{
		public static List<GenPass> AvailablePasses = null!;
		public static bool ReplacePasses;

		public static void LoadPasses()
		{
			UnifiedRandom? rand = Main.rand;
			ILWorldGen.GenerateWorld += DontGenerate;
			WorldGen.GenerateWorld(0);
			ILWorldGen.GenerateWorld -= DontGenerate;
			Main._rand = rand;
			ReplacePasses = true;
		}

		public static void DontGenerate(ILContext il)
		{
			ILCursor ilCursor = new(il);
			MethodInfo modifyWorldGenTasksInfo =
				typeof(SystemLoader).GetMethod(nameof(SystemLoader.ModifyWorldGenTasks),
					BindingFlags.Public | BindingFlags.Static)!;
			ilCursor.GotoNext(MoveType.After, instruction => instruction.MatchCall(modifyWorldGenTasksInfo));
			var instructionStart = ilCursor.Prev.Previous.Previous.Previous.Previous;

			ilCursor.Emit(instructionStart.OpCode, instructionStart.Operand);
			ilCursor.Emit(instructionStart.Next.OpCode, instructionStart.Next.Operand);
			ilCursor.Emit(OpCodes.Stsfld,
				typeof(PassHandler).GetField(nameof(AvailablePasses), BindingFlags.Public | BindingFlags.Static));

			ilCursor.Emit(OpCodes.Ret);
		}

		public static void OverridePasses(ILContext il)
		{
			ILCursor ilCursor = new(il);
			MethodInfo preWorldGenInfo = typeof(SystemLoader).GetMethod(nameof(SystemLoader.PreWorldGen),
				BindingFlags.Public | BindingFlags.Static)!;
			MethodInfo modifyWorldGenTasksInfo =
				typeof(SystemLoader).GetMethod(nameof(SystemLoader.ModifyWorldGenTasks),
					BindingFlags.Public | BindingFlags.Static)!;

			FieldInfo replacePassesInfo =
				typeof(PassHandler).GetField(nameof(ReplacePasses), BindingFlags.Public | BindingFlags.Static)!;

			ilCursor.GotoNext(MoveType.Before, instruction => instruction.MatchCall(preWorldGenInfo));

			ILLabel label1 = ilCursor.DefineLabel();
			ilCursor.Emit(OpCodes.Ldsfld, replacePassesInfo);
			ilCursor.Emit(OpCodes.Brtrue_S, label1);

			ilCursor.GotoNext(MoveType.After, instruction => instruction.MatchCall(modifyWorldGenTasksInfo));

			Instruction instructionStart = ilCursor.Prev.Previous.Previous.Previous.Previous;

			ILLabel label2 = ilCursor.DefineLabel();
			ilCursor.Emit(OpCodes.Ldsfld, replacePassesInfo);
			ilCursor.Emit(OpCodes.Brfalse_S, label2);

			ilCursor.Emit(instructionStart.OpCode, instructionStart.Operand);
			ilCursor.GotoPrev();
			ilCursor.MarkLabel(label1);
			ilCursor.GotoNext();
			
			ilCursor.Emit(OpCodes.Ldsfld,
				typeof(PassHandler).GetField(nameof(AvailablePasses), BindingFlags.Public | BindingFlags.Static));
			ilCursor.Emit(OpCodes.Stfld, instructionStart.Next.Operand);

			ilCursor.MarkLabel(label2);
		}
	}
}