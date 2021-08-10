using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class WorldgenSettings : ModConfig
	{
		[Label("Faster Worldgen")]
		[Tooltip("Make the worldgen a lot faster, especially the large ones. \nIt also extends the generation of larger worlds.")]
		[DefaultValue(false)]
		public bool FasterWorldgen;

		public override ConfigScope Mode => ConfigScope.ClientSide;
	}
}