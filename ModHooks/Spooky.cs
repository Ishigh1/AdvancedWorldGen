namespace AdvancedWorldGen.ModHooks;

public class Spooky : ModSystem
{
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
			HookEndpointManager.Modify(methodInfo, GetManipulator(AvoidRight));
	}

	public override void Unload()
	{
		if (ModLoader.TryGetMod("Spooky", out Mod mod) &&
		    mod.TryGetMethod("Spooky.Content.Generation.SpookyHell", "ClearArea",
			    BindingFlags.Public | BindingFlags.Instance,
			    out MethodInfo? methodInfo))
			HookEndpointManager.Unmodify(methodInfo, GetManipulator(AvoidRight));
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