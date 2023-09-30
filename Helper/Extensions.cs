namespace AdvancedWorldGen.Helper;

public static class Extensions
{
	#region PassInsertion

	public static bool TryReplacePass(this List<GenPass> genPasses, string passName, GenPass newPass)
	{
		int index = genPasses.FindIndex(pass => pass.Name == passName);
		if (index == -1 || genPasses[index].GetType().Assembly != typeof(PassLegacy).Assembly)
			return false;
		genPasses[index] = newPass;
		return true;
	}

	#endregion

	public static GameConfiguration Next(this GameConfiguration gameConfiguration, string name)
	{
		return new GameConfiguration(gameConfiguration.Get<JObject>(name));
	}

	public static double Next(this UnifiedRandom random, double min, double max)
	{
		return random.NextDouble() * (max - min) + min;
	}

	#region ILHelper

	public static void OptionContains(this ILCursor cursor, string option)
	{
		cursor.Emit(OpCodes.Ldstr, option);
		cursor.Emit(OpCodes.Call, typeof(OptionHelper).GetMethod(nameof(OptionHelper.OptionsContains)));
	}

	public static void DeleteUntil(this ILCursor cursor, Func<Instruction, bool> validator)
	{
		List<ILLabel> labels = new();
		while (!validator(cursor.Next))
		{
			labels.AddRange(cursor.IncomingLabels);
			cursor.Remove();
		}
		foreach (ILLabel ilLabel in labels)
		{
			cursor.MarkLabel(ilLabel);
		}
	}

	#endregion

	#region Print

	public static string ToStringList<T>(this IEnumerable<T> list)
	{
		StringBuilder stringBuilder = new();
		int index = 0;
		foreach (T value in list) stringBuilder.AppendLine($"{index++} : {value}");

		return stringBuilder.ToString();
	}

	#endregion

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

	#region CrossMod

	public static Type? GetType(this Mod mod, string typeName)
	{
		Assembly assembly = mod.Code;
		Type? type = assembly.GetType(typeName);
		if (type == null) AdvancedWorldGenMod.Instance.Logger.Info($"{typeName} not found");

		return type;
	}

	public static object? GetFieldValue(this Type type, string fieldName, object? arg = null)
	{
		FieldInfo? fieldInfo = type.GetField(fieldName,
			BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (fieldInfo == null) AdvancedWorldGenMod.Instance.Logger.Info($"{fieldName} not found");

		return fieldInfo?.GetValue(arg);
	}

	public static object? GetPropertyValue(this Type type, string fieldName, object? arg = null)
	{
		PropertyInfo? propertyInfo = type.GetProperty(fieldName,
			BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (propertyInfo == null) AdvancedWorldGenMod.Instance.Logger.Info($"{fieldName} not found");

		return propertyInfo?.GetValue(arg);
	}

	public static bool TryGetMethod(this Mod mod, string typeName, string methodName, BindingFlags bindingFlags,
		out MethodInfo? methodInfo)
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