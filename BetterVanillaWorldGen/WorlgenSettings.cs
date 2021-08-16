using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class WorldgenSettings : ModConfig
	{
		public static bool Revamped => ModContent.GetInstance<WorldgenSettings>().FasterWorldgen;
		
		[Label("Faster Worldgen")]
		[Tooltip("Make the worldgen a lot faster, especially the large ones. \n" +
		         "It also extends the generation of larger worlds.")]
		[DefaultValue(false)]
		public bool FasterWorldgen;

		public override ConfigScope Mode => ConfigScope.ClientSide;
	}
}