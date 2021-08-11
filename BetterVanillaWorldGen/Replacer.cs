using System.Collections.Generic;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Biomes.Desert;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using OnJunglePass = On.Terraria.GameContent.Biomes.JunglePass;
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
			OnWorldGen.MakeDungeon += ReplaceDungeon;
			OnJunglePass.GenerateHolesInMudWalls += ReplaceJungleHoles;
		}

		public static void UnReplace()
		{
			OnDesertHive.Place -= ReplaceDesertHive;
			OnWorldGen.AddBuriedChest_int_int_int_bool_int_bool_ushort -= ReplaceChest;
			OnWorldGen.MakeDungeon -= ReplaceDungeon;
			OnJunglePass.GenerateHolesInMudWalls -= ReplaceJungleHoles;
		}

		public static void ReplaceGenPasses(List<GenPass> genPasses)
		{
			if (!Revamped)
				return;
			genPasses.Insert(0, new ResetOverhauled("Reset Overhauled", 0));

			int index = genPasses.FindIndex(pass => pass.Name == "Jungle");
			JunglePass junglePass = null;
			if (index != -1) junglePass = (JunglePass) genPasses[index];
			index = genPasses.FindIndex(index, pass => pass.Name == "Mushroom Patches");
			if (index != -1)
			{
				genPasses.RemoveAt(index);
				genPasses.Insert(index, new MushroomPatches());
			}

			index = genPasses.FindIndex(index, pass => pass.Name == "Jungle Temple");
			if (index != -1 && junglePass != null)
			{
				genPasses.RemoveAt(index);
				genPasses.Insert(index, new JungleTemple(junglePass));
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

		public static bool ReplaceChest(OnWorldGen.orig_AddBuriedChest_int_int_int_bool_int_bool_ushort orig, int i,
			int j, int contain, bool notNearOtherChests, int style, bool trySlope, ushort chestTileType)
		{
			return Revamped
				? Chest.AddBuriedChest(i, j, contain, notNearOtherChests, style, chestTileType)
				: orig(i, j, contain, notNearOtherChests, style, trySlope, chestTileType);
		}

		public static void ReplaceDungeon(OnWorldGen.orig_MakeDungeon orig, int x, int y)
		{
			if (Revamped)
				Dungeon.MakeDungeon(x, y);
			else
				orig(x, y);
		}

		private static void ReplaceJungleHoles(OnJunglePass.orig_GenerateHolesInMudWalls orig, JunglePass self)
		{
			if (Revamped)
				Jungle.GenerateHolesInMudWalls(self);
			else
				orig(self);
		}
	}
}