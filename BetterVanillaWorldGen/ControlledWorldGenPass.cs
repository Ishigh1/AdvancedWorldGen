using Terraria;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public abstract class ControlledWorldGenPass : GenPass
	{
		public UnifiedRandom Random;
		public GenerationProgress Progress = null!;
		public GameConfiguration Configuration = null!;
		
		protected ControlledWorldGenPass(string name, float loadWeight) : base(name, loadWeight)
		{
			Random = new UnifiedRandom(WorldGen.genRand.Next());
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
}