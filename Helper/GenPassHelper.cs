using Terraria.WorldBuilding;

namespace AdvancedWorldGen.Helper
{
	public static class GenPassHelper
	{
		public static void SetProgress(this GenerationProgress generationProgress, int currentValue, float maxValue,
			float weight = 1f, float pastWeight = 0f)
		{
			generationProgress.Set(currentValue / maxValue * weight + pastWeight);
		}

		public static void SetProgress(this GenerationProgress generationProgress, float progress)
		{
			generationProgress.Set(progress);
		}
	}
}