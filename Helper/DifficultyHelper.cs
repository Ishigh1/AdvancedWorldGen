using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace AdvancedWorldGen.Helper
{
	public static class DifficultyHelper
	{
		public static float GetDamageModifier()
		{
			float power = Main.GameModeInfo.EnemyDamageMultiplier;

			if (Main.getGoodWorld)
			{
				power++;
				power *= 4 / 3f;
			}

			if (!ModLoader.TryGetMod("CreativeFix", out Mod _) && Main.GameModeInfo.IsJourneyMode)
			{
				CreativePowers.DifficultySliderPower difficultySliderPower =
					CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();
				power *= difficultySliderPower.StrengthMultiplierToGiveNPCs;
			}

			return power;
		}
	}
}