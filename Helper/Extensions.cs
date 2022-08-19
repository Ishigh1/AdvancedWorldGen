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
}