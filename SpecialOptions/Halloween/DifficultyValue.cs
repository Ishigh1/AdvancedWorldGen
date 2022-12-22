namespace AdvancedWorldGen.SpecialOptions.Halloween;

public readonly struct DifficultyValue<T>
{
	private readonly T ExpertMode;
	private readonly T MasterMode;
	private readonly T NormalMode;

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