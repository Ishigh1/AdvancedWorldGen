using Terraria.WorldBuilding;

namespace AdvancedWorldGen.Helper
{
	public class GenPassHelper
	{
		public static void SetProgress(GenerationProgress generationProgress, int currentValue, float maxValue,
			float weight, float pastWeight = 0f)
		{
			generationProgress.Set(currentValue / maxValue * weight + pastWeight);
		}

		public static void SetProgress(GenerationProgress generationProgress, float progress)
		{
			generationProgress.Set(progress);
		}
	}
}