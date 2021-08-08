using System.Collections.Generic;
using Terraria.GameContent.Biomes.Desert;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using OnDesertHive = On.Terraria.GameContent.Biomes.Desert.DesertHive;
using OnWorldGen = On.Terraria.WorldGen;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class Replacer
	{
		public static bool Revamped => ModContent.GetInstance<WorldgenSettings>().FasterWorldgen;

		public static void Replace()
		{
			OnDesertHive.Place += ReplaceDesertHive;
			OnWorldGen.AddBuriedChest_int_int_int_bool_int_bool_ushort += ReplaceChest;
		}

		public static void UnReplace()
		{
			OnDesertHive.Place -= ReplaceDesertHive;
		}

		public static void ReplaceGenPasses(List<GenPass> genPasses)
		{
			if (!Revamped)
				return;
			genPasses.Insert(0, new ResetOverhauled("Reset Overhauled", 0));
			int index = genPasses.FindIndex(pass => pass.Name == "Mushroom Patches");
			if (index != -1)
			{
				genPasses.RemoveAt(index);
				genPasses.Insert(index, new MushroomPatches());
			}
		}

		public static void ReplaceDesertHive(OnDesertHive.orig_Place orig,
			DesertDescription description)
		{
			if (Revamped)
				DesertHive.Place(description);
			else
				orig(description);
		}

		private static bool ReplaceChest(OnWorldGen.orig_AddBuriedChest_int_int_int_bool_int_bool_ushort orig, int i,
			int j, int contain, bool notnearotherchests, int style, bool tryslope, ushort chesttiletype)
		{
			return Revamped
				? Chest.AddBuriedChest(i, j, contain, notnearotherchests, style, chesttiletype)
				: orig(i, j, contain, notnearotherchests, style, tryslope, chesttiletype);
		}
	}
}