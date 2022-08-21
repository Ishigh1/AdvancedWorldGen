namespace AdvancedWorldGen.ModHooks;

public class Calamity : ModSystem
{
	public override void Load()
	{
		if (ModLoader.TryGetMod("CalamityMod", out Mod mod) && mod.Version.Equals(new Version(2, 0, 0, 3)) && TryGetMethod(mod, out MethodInfo? methodInfo))
			HookEndpointManager.Modify(methodInfo, FixCalamIssue);
	}

	public override void Unload()
	{
		if (ModLoader.TryGetMod("CalamityMod", out Mod mod) && mod.Version.Equals(new Version(2, 0, 0, 3)) && TryGetMethod(mod, out MethodInfo? methodInfo))
			HookEndpointManager.Unmodify(methodInfo, FixCalamIssue);
	}

	private bool TryGetMethod(Mod mod, out MethodInfo? methodInfo)
	{
		Assembly assembly = mod.GetType().Assembly;
		Type? type = assembly.GetType("CalamityMod.World.MiscWorldgenRoutines");
		if (type == null)
		{
			Mod.Logger.Info("CalamityMod.World.MiscWorldgenRoutines not found");
			methodInfo = null;
			return false;
		}

		methodInfo = type.GetMethod("GenerateBiomeChests", BindingFlags.Public | BindingFlags.Static);
		if (methodInfo == null)
		{
			Mod.Logger.Info("CalamityMod.World.MiscWorldgenRoutines.GenerateBiomeChests not found");
			return false;
		}

		return true;
	}

	private static void FixCalamIssue(ILContext il)
	{
		ILCursor cursor = new(il);
		while (cursor.TryGotoNext(MoveType.Before, instruction => instruction.MatchLdcI4(40))) cursor.Next.Operand = 24;
	}
}