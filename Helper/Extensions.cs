namespace AdvancedWorldGen.Helper;

public static class Extensions
{
	#region GenerationProgress

	public static void Set(this GenerationProgress generationProgress, float currentValue, float maxValue,
		float weight = 1f, float pastWeight = 0f)
	{
		generationProgress.Set(currentValue / maxValue * weight + pastWeight);
	}

	public static void Add(this GenerationProgress generationProgress, float value, float maxValue,
		float weight = 1f)
	{
		generationProgress.Value += value / maxValue * weight;
	}

	#endregion

	#region ILHelper

	public static void OptionContains(this ILCursor cursor, string option)
	{
		cursor.Emit(OpCodes.Ldstr, option);
		cursor.Emit(OpCodes.Call, typeof(OptionHelper).GetMethod(nameof(OptionHelper.OptionsContains)));
	}

	#endregion

	#region CrossMod

	public static Type? GetType(this Mod mod, string typeName)
	{
		Assembly assembly = mod.GetType().Assembly;
		Type? type = assembly.GetType(typeName);
		if (type == null) AdvancedWorldGenMod.Instance.Logger.Info($"{typeName} not found");

		return type;
	}
	
	public static bool TryGetMethod(this Mod mod, string typeName, string methodName, BindingFlags bindingFlags, out MethodInfo? methodInfo)
	{
		Type? type = mod.GetType(typeName);
		if (type == null)
		{
			methodInfo = null;
			return false;
		}

		methodInfo = type.GetMethod(methodName, bindingFlags);
		if (methodInfo == null)
		{
			AdvancedWorldGenMod.Instance.Logger.Info($"{typeName}.{methodName} not found");
			return false;
		}

		return true;
	}

	#endregion
}