using System;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using On.Terraria;

namespace AdvancedWorldGen.SpecialOptions
{
	public class ClassicOptions
	{
		public static void SmallNotTheBees(WorldGen.orig_NotTheBees orig)
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
			ILCursor ilCursor = new ILCursor(il);
			if (!ilCursor.TryGotoNext(instruction => instruction.MatchLdcI4(100))) return;

			while (ilCursor.Next.OpCode != OpCodes.Ldsfld) ilCursor.Remove();

			ilCursor.Emit(OpCodes.Ldarga_S, (byte) 1);

			ilCursor.GotoNext(instruction => instruction.MatchStloc(1));
			ilCursor.Emit(OpCodes.Call, typeof(ClassicOptions).GetMethod("GetTempleRooms"));
			ilCursor.Emit(OpCodes.Stloc_2);
			ilCursor.Emit(OpCodes.Ldloc_2);
			ilCursor.Emit(OpCodes.Newarr, typeof(Rectangle));
			ilCursor.Emit(OpCodes.Stloc_0);
			int c = 0;
			while (ilCursor.Next.OpCode != OpCodes.Stloc_2 || c++ < 2) ilCursor.Remove();

			ilCursor.Remove();
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
			else if (Terraria.WorldGen.getGoodWorldGen && Terraria.WorldGen.drunkWorldGen)
				templeSize = 6;
			else if (Terraria.WorldGen.getGoodWorldGen || Terraria.WorldGen.drunkWorldGen) templeSize = 3;

			if (templeSize > 10)
			{
				y -= (int) (200 * worldSize * (templeSize - 10) / 5);
				y = Math.Max(y, 50);
			}

			return Terraria.WorldGen.genRand.Next((int) (10 * worldSize * templeSize),
				(int) (16 * worldSize * templeSize));
		}
	}
}