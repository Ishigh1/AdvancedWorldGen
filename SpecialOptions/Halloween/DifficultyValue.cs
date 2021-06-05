using Terraria;

namespace AdvancedWorldGen.SpecialOptions.Halloween
{
	public class DifficultyValue<T>
	{
		public T expertMode;
		public T masterMode;
		public T normalMode;

		public DifficultyValue(T normalMode, T expertMode, T masterMode)
		{
			this.normalMode = normalMode;
			this.expertMode = expertMode;
			this.masterMode = masterMode;
		}

		public T GetCurrentValue()
		{
			if (Main.masterMode)
				return masterMode;
			else if (Main.expertMode)
				return expertMode;
			else
				return normalMode;
		}
	}
}