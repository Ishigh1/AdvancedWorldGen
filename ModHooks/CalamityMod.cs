namespace AdvancedWorldGen.ModHooks;

public class CalamityMod : ModSystem
{
	private static ILContext.Manipulator GetManipulator(ILContext.Manipulator self)
	{
		return self;
	}

	public override void Load()
	{
		if (ModLoader.TryGetMod("CalamityMod", out Mod mod) &&
		    mod.TryGetMethod("CalamityMod.World.SulphurousSea", "GetActualX", BindingFlags.Public | BindingFlags.Static,
			    out MethodInfo? methodInfo))
			HookEndpointManager.Modify(methodInfo, GetManipulator(ShiftRightSea));
	}

	public override void Unload()
	{
		if (ModLoader.TryGetMod("CalamityMod", out Mod mod) &&
		    mod.TryGetMethod("CalamityMod.World.SulphurousSea", "GetActualX", BindingFlags.Public | BindingFlags.Static,
			    out MethodInfo? methodInfo))
			HookEndpointManager.Unmodify(methodInfo, GetManipulator(ShiftRightSea));
	}

	private static void ShiftRightSea(ILContext ilContext) //Will have to remove when it is implemented
	{
		ILCursor cursor = new(ilContext);
		cursor.GotoNext(MoveType.After, instruction => instruction.MatchSub());
		cursor.Emit(OpCodes.Ldc_I4_0);
		cursor.Emit(OpCodes.Sub);
	}
}