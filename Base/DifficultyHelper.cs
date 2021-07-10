using Terraria;
using Terraria.GameContent.Creative;

namespace AdvancedWorldGen.Base
{
	public class DifficultyHelper
	{
		public static float GetDamageModifier()
		{
			float power = Main.GameModeInfo.EnemyDamageMultiplier;

			if (Main.getGoodWorld) power++;
			CreativePowers.DifficultySliderPower difficultySliderPower =
				CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();
			if (difficultySliderPower != null && difficultySliderPower.GetIsUnlocked())
				power *= difficultySliderPower.StrengthMultiplierToGiveNPCs;

			return power;
		}
	}
}