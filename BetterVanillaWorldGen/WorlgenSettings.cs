using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class WorldgenSettings : ModConfig
	{
		[DefaultValue(false)] public bool FasterWorldgen;

		public static bool Revamped => ModContent.GetInstance<WorldgenSettings>().FasterWorldgen;

		public override ConfigScope Mode => ConfigScope.ClientSide;
	}
}