using System;
using AdvancedWorldGen.Base;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using OnWorldGen = On.Terraria.WorldGen;

namespace AdvancedWorldGen.SpecialOptions
{
	public class ClassicOptions
	{
		public static void SmallNotTheBees(OnWorldGen.orig_NotTheBees orig)
		{
			if (!ModifiedWorld.OptionsContains("SmallNotTheBees")) orig();
		}

		/*
		 * replace IL_0000 - IL_0045 by
		 * int num2 = WorldGen.getTempleRooms((float)(Main.maxTilesX / 4200));
		 * Rectangle[] array = new Rectangle[num2];
		 */
		public static void ReduceTemple(ILContext il)
		{
			ILCursor cursor = new(il);
			if (!cursor.TryGotoNext(instruction => instruction.MatchLdcI4(100))) return;

			while (cursor.Next.OpCode != OpCodes.Ldsfld) cursor.Remove();

			cursor.Emit(OpCodes.Ldarga_S, (byte) 1);

			cursor.GotoNext(instruction => instruction.MatchStloc(1));
			cursor.Emit(OpCodes.Call, typeof(ClassicOptions).GetMethod("GetTempleRooms"));
			cursor.Emit(OpCodes.Stloc_2);
			cursor.Emit(OpCodes.Ldloc_2);
			cursor.Emit(OpCodes.Newarr, typeof(Rectangle));
			cursor.Emit(OpCodes.Stloc_0);
			int c = 0;
			while (cursor.Next.OpCode != OpCodes.Stloc_2 || c++ < 2) cursor.Remove();

			cursor.Remove();
		}

		public static int GetTempleRooms(ref int y, float worldSize)
		{
			int templeSize = 1;
			if (ModifiedWorld.OptionsContains("StupidlyHugeTemple"))
				templeSize = 25;
			else if (ModifiedWorld.OptionsContains("StupidlyBigTemple"))
				templeSize = 20;
			else if (ModifiedWorld.OptionsContains("BigTemple"))
				templeSize = 15;
			else if (WorldGen.getGoodWorldGen && WorldGen.drunkWorldGen)
				templeSize = 6;
			else if (WorldGen.getGoodWorldGen || WorldGen.drunkWorldGen) templeSize = 3;

			if (templeSize > 10)
			{
				y -= (int) (200 * worldSize * (templeSize - 10) / 5);
				y = Math.Max(y, 50);
			}

			return WorldGen.genRand.Next((int) (10 * worldSize * templeSize),
				(int) (16 * worldSize * templeSize));
		}
	}
}