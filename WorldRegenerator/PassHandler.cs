using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
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
			ILWorldGen.GenerateWorld += DontGenerate;
			WorldGen.GenerateWorld(0);
			ILWorldGen.GenerateWorld -= DontGenerate;
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
			MethodInfo modifyWorldGenTasksInfo =
				typeof(SystemLoader).GetMethod(nameof(SystemLoader.ModifyWorldGenTasks),
					BindingFlags.Public | BindingFlags.Static)!;
			ilCursor.GotoNext(MoveType.After, instruction => instruction.MatchCall(modifyWorldGenTasksInfo));

			Instruction instructionStart = ilCursor.Prev.Previous.Previous.Previous.Previous;
			
			ILLabel label = ilCursor.DefineLabel();
			ilCursor.Emit(OpCodes.Ldsfld,
				typeof(PassHandler).GetField(nameof(AvailablePasses), BindingFlags.Public | BindingFlags.Static));
			ilCursor.Emit(OpCodes.Brfalse, label);
			
			ilCursor.Emit(instructionStart.OpCode, instructionStart.Operand);
			ilCursor.Emit(OpCodes.Ldsfld,
				typeof(PassHandler).GetField(nameof(AvailablePasses), BindingFlags.Public | BindingFlags.Static));
			ilCursor.Emit(OpCodes.Stfld, instructionStart.Next.Operand);

			ilCursor.MarkLabel(label);
		}
	}
}