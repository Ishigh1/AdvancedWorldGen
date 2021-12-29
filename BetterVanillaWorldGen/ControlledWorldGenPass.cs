using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
using Terraria;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen;

public abstract class ControlledWorldGenPass : GenPass
{
	public GameConfiguration Configuration = null!;
	public GenerationProgress Progress = null!;
	public UnifiedRandom Random;
	public VanillaInterface VanillaInterface;

	protected ControlledWorldGenPass(string name, float loadWeight) : base(name, loadWeight)
	{
		Random = new UnifiedRandom(WorldGen.genRand.Next());
		VanillaInterface = Replacer.VanillaInterface;
	}

	protected abstract void ApplyPass();

	protected sealed override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
	{
		Progress = progress;
		Configuration = configuration;
		UnifiedRandom random = WorldGen.genRand;
		WorldGen._genRand = Random;
		ApplyPass();
		WorldGen._genRand = random;
	}
}