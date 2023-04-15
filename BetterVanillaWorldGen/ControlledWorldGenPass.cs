namespace AdvancedWorldGen.BetterVanillaWorldGen;

public abstract class ControlledWorldGenPass : GenPass
{
	public GameConfiguration Configuration = null!;
	public GenerationProgress Progress = null!;
	public Stopwatch? Stopwatch;
	public VanillaInterface VanillaInterface;

	protected ControlledWorldGenPass(string name, float loadWeight) : base(name, loadWeight)
	{
		VanillaInterface = Replacer.VanillaInterface;
	}

	protected abstract void ApplyPass();

	protected sealed override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
	{
		Progress = progress;
		Configuration = configuration;
		ApplyPass();
	}
}