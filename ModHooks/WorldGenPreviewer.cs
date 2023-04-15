namespace AdvancedWorldGen.ModHooks;

public class WorldGenPreviewer : ModSystem
{
	private Hook? Hook;

	public override void Load()
	{
		if (ModLoader.TryGetMod("WorldGenPreviewer", out Mod mod) &&
		    mod.TryGetMethod("WorldGenPreviewer.UIWorldLoadSpecial", "CancelClick",
			    BindingFlags.NonPublic | BindingFlags.Instance, out MethodInfo? methodInfo))
			Hook = new Hook(methodInfo, CancelClick);
	}

	public override void Unload()
	{
		Hook?.Dispose();
	}

	private static void Unpause()
	{
		Mod mod = ModLoader.GetMod("WorldGenPreviewer");
		Assembly assembly = mod.GetType().Assembly;
		Type type = assembly.GetType("WorldGenPreviewer.WorldGenPreviewerModWorld")!;
		FieldInfo fieldInfo = type.GetField("continueWorldGen", BindingFlags.NonPublic | BindingFlags.Static)!;
		fieldInfo.SetValue(null, true);
	}

	private static void CancelClick(Action<object, UIMouseEvent, UIElement> action, object self, UIMouseEvent evt,
		UIElement listeningElement)
	{
		Unpause();
		AdvancedWorldGenMod.Instance.UiChanger.Abort(evt, listeningElement);
	}
}