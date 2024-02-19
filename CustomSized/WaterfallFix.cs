namespace AdvancedWorldGen.CustomSized;

public static class WaterfallFix
{
	public static int Test()
	{
		if (Main.lo > Main.background - 1)
			Main.lo = Main.background - 1;
		int num = Main.lo;
		if (num < 0)
			num = 0;
		return num;
	}

	public static void FixWaterfalls(ILContext il)
	{
		ILCursor cursor = new(il);
		//Go at the start of the clamping
		cursor.GotoNext(MoveType.After, instruction => instruction.MatchLdcI4(20));

		//Clamp xMin to 1
		cursor.GotoNext(MoveType.Before, instruction => instruction.MatchLdcI4(0));
		cursor.Remove();
		cursor.EmitLdcI4(1);
		cursor.GotoNext(MoveType.Before, instruction => instruction.MatchLdcI4(0));
		cursor.Remove();
		cursor.EmitLdcI4(1);
		
		//Clamp xMax to Main.maxTilesX - 2
		cursor.GotoNext(MoveType.Before, instruction => instruction.MatchLdsfld<Main>(nameof(Main.maxTilesX)));
		cursor.GotoNext();
		cursor.EmitLdcI4(2);
		cursor.EmitSub();
		cursor.GotoNext(MoveType.Before, instruction => instruction.MatchLdsfld<Main>(nameof(Main.maxTilesX)));
		cursor.GotoNext();
		cursor.EmitLdcI4(2);
		cursor.EmitSub();
		
		//Clamp yMin to 1
		cursor.GotoNext(MoveType.Before, instruction => instruction.MatchLdcI4(0));
		cursor.Remove();
		cursor.EmitLdcI4(1);
		cursor.GotoNext(MoveType.Before, instruction => instruction.MatchLdcI4(0));
		cursor.Remove();
		cursor.EmitLdcI4(1);
		
		//Clamp xMax to Main.maxTilesY - 2
		cursor.GotoNext(MoveType.Before, instruction => instruction.MatchLdsfld<Main>(nameof(Main.maxTilesY)));
		cursor.GotoNext();
		cursor.EmitLdcI4(2);
		cursor.EmitSub();
		cursor.GotoNext(MoveType.Before, instruction => instruction.MatchLdsfld<Main>(nameof(Main.maxTilesY)));
		cursor.GotoNext();
		cursor.EmitLdcI4(2);
		cursor.EmitSub();
	}
}