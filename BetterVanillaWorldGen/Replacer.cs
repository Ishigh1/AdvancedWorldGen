using Terraria.GameContent.Biomes.Desert;
using Terraria.ModLoader;
using OnDesertHive = On.Terraria.GameContent.Biomes.Desert.DesertHive;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class Replacer
	{
		public static void Replace()
		{
			OnDesertHive.Place += ReplaceDesertHive;
		}
		public static void UnReplace()
		{
			OnDesertHive.Place -= ReplaceDesertHive;
		}

		private static void ReplaceDesertHive(On.Terraria.GameContent.Biomes.Desert.DesertHive.orig_Place orig,
			DesertDescription description)
		{
			bool revamped = ModContent.GetInstance<WorldgenSettings>().FasterWorldgen;
			if (revamped)
				DesertHive.Place(description);
			else
				orig(description);
		}
	}
}