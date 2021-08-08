using Terraria.IO;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class ResetOverhauled : GenPass
	{
		public ResetOverhauled(string name, float loadWeight) : base(name, loadWeight)
		{}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			Chest.ShuffleChests();
		}
	}
}