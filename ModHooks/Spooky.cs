namespace AdvancedWorldGen.ModHooks;

public class Spooky : ModSystem
{
	private ILHook? Hook;

	private static ILContext.Manipulator GetManipulator(ILContext.Manipulator self)
	{
		return self;
	}

	public override void Load()
	{
		if (ModLoader.TryGetMod("Spooky", out Mod mod) &&
		    mod.TryGetMethod("Spooky.Content.Generation.SpookyHell", "ClearArea",
			    BindingFlags.Public | BindingFlags.Instance,
			    out MethodInfo? methodInfo))
			Hook = new ILHook(methodInfo, GetManipulator(AvoidRight));
	}

	public override void Unload()
	{
		Hook?.Dispose();
	}

	private static void AvoidRight(ILContext ilContext) //Will have to remove when it is implemented
	{
		ILCursor cursor = new(ilContext);
		cursor.GotoNext(MoveType.Before,
			instruction => instruction.OpCode == OpCodes.Ble_S && instruction.Next.MatchLdloc(0));
		cursor.Remove();
		cursor.Emit(OpCodes.Blt);
	}
}