namespace AdvancedWorldGen.ModHooks;

public class WorldGenPreviewer : ModSystem
{
	public override void Load()
	{
		if (ModLoader.TryGetMod("WorldGenPreviewer", out Mod mod) && TryGetMethod(mod, out MethodInfo? methodInfo))
			HookEndpointManager.Add(methodInfo, CancelClick);
	}

	public override void Unload()
	{
		if (ModLoader.TryGetMod("WorldGenPreviewer", out Mod mod) && TryGetMethod(mod, out MethodInfo? methodInfo))
			HookEndpointManager.Remove(methodInfo, CancelClick);
	}

	private bool TryGetMethod(Mod mod, out MethodInfo? methodInfo)
	{
		Assembly assembly = mod.GetType().Assembly;
		Type? type = assembly.GetType("WorldGenPreviewer.UIWorldLoadSpecial");
		if (type == null)
		{
			Mod.Logger.Info("WorldgenPreviewer changed, UIWorldLoadSpecial not found");
			methodInfo = null;
			return false;
		}

		methodInfo = type.GetMethod("CancelClick", BindingFlags.NonPublic | BindingFlags.Instance);
		if (methodInfo == null)
		{
			Mod.Logger.Info("WorldgenPreviewer changed, UIWorldLoadSpecial.CancelClick not found");
			return false;
		}

		return true;
	}

	private static void Unpause()
	{
		Mod mod = ModLoader.GetMod("WorldGenPreviewer");
		Assembly assembly = mod.GetType().Assembly;
		Type type = assembly.GetType("WorldGenPreviewer.WorldGenPreviewerModWorld")!;
		FieldInfo fieldInfo = type.GetField("continueWorldGen", BindingFlags.NonPublic | BindingFlags.Static)!;
		fieldInfo.SetValue(null, true);
	}

	private static void CancelClick(Action<object, UIMouseEvent, UIElement> action, object self, UIMouseEvent evt, UIElement listeningElement)
	{
		Unpause();
		AdvancedWorldGenMod.Instance.UiChanger.Abort(evt, listeningElement);
	}
}