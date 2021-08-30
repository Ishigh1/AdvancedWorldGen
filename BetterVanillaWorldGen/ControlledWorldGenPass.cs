using Terraria;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public abstract class ControlledWorldGenPass : GenPass
	{
		public UnifiedRandom Random;
		
		protected ControlledWorldGenPass(string name, float loadWeight) : base(name, loadWeight)
		{
			Random = new UnifiedRandom(WorldGen.genRand.Next());
		}
	}
}