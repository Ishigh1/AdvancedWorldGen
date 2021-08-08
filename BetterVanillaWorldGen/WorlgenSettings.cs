using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class WorldgenSettings : ModConfig
	{
		[Label("Faster Worldgen")]
		[Tooltip("Make the worldgen faster. It might slightly change the worldgen.")]
		[DefaultValue(false)]
		public bool FasterWorldgen;

		public override ConfigScope Mode => ConfigScope.ClientSide;
	}
}