using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace AdvancedWorldGen.Base
{
	public class DifficultyHelper
	{
		public static float GetDamageModifier()
		{
			if (ModLoader.GetMod("CreativeFix") != null)
				return Main.GameModeInfo.EnemyDamageMultiplier;
			float power = Main.GameModeInfo.EnemyDamageMultiplier;

			if (Main.GameModeInfo.IsJourneyMode)
			{
				CreativePowers.DifficultySliderPower difficultySliderPower =
					CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();
				power *= difficultySliderPower.StrengthMultiplierToGiveNPCs;
			}

			if (Main.getGoodWorld) power++;
			return power;
		}
	}
}