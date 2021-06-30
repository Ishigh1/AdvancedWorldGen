using Terraria;
using Terraria.GameContent.Creative;

namespace AdvancedWorldGen.Base
{
	public class DifficultyHelper
	{
		public static float GetDamageModifier()
		{
			float power = Main.GameModeInfo.EnemyDamageMultiplier;

			CreativePowers.DifficultySliderPower difficultySliderPower =
				CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();
			if (difficultySliderPower != null && difficultySliderPower.GetIsUnlocked())
				power *= difficultySliderPower.StrengthMultiplierToGiveNPCs;

			if (Main.getGoodWorld) power++;
			return power;
		}
	}
}