using Terraria;

namespace AdvancedWorldGen.SpecialOptions.Halloween;

public class DifficultyValue<T>
{
	public T ExpertMode;
	public T MasterMode;
	public T NormalMode;

	public DifficultyValue(T normalMode, T expertMode, T masterMode)
	{
		NormalMode = normalMode;
		ExpertMode = expertMode;
		MasterMode = masterMode;
	}

	public T GetCurrentValue()
	{
		if (Main.masterMode)
			return MasterMode;

		if (Main.expertMode)
			return ExpertMode;

		return NormalMode;
	}
}