using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class WorldgenSettings : ModConfig
	{
		[Label("Overhauled Worldgen")]
		[Tooltip("Tweaks the worldgen so it is faster and more adapted to custom sized worlds. \n" + 
				"Worlds smaller than 2600 x ... can't be generated without it")]
		[DefaultValue(false)]
		public bool FasterWorldgen;

		public static bool Revamped => ModContent.GetInstance<WorldgenSettings>().FasterWorldgen;

		public override ConfigScope Mode => ConfigScope.ClientSide;
	}
}