using AdvancedWorldGen.BetterVanillaWorldGen.DesertStuff;
using AdvancedWorldGen.BetterVanillaWorldGen.DungeonStuff;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Biomes.Desert;
using DesertHive = AdvancedWorldGen.BetterVanillaWorldGen.DesertStuff.DesertHive;
using OnJunglePass = On.Terraria.GameContent.Biomes.JunglePass;
using OnDesertHive = On.Terraria.GameContent.Biomes.Desert.DesertHive;
using OnDesertDescription = On.Terraria.GameContent.Biomes.Desert.DesertDescription;
using OnWorldGen = On.Terraria.WorldGen;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public partial class Replacer
	{
		public static void Replace()
		{
			OnDesertHive.Place += ReplaceDesertHive;
			OnWorldGen.AddBuriedChest_int_int_int_bool_int_bool_ushort += ReplaceChest;
			OnWorldGen.MakeDungeon += ReplaceDungeon;
			OnDesertDescription.CreateFromPlacement += ReplaceDesertDescriptionCreation;
		}

		public static void UnReplace()
		{
			VanillaInterface = null;

			OnDesertHive.Place -= ReplaceDesertHive;
			OnWorldGen.AddBuriedChest_int_int_int_bool_int_bool_ushort -= ReplaceChest;
			OnWorldGen.MakeDungeon -= ReplaceDungeon;
			OnDesertDescription.CreateFromPlacement -= ReplaceDesertDescriptionCreation;
		}

		public static void ReplaceDesertHive(OnDesertHive.orig_Place orig,
			DesertDescription description)
		{
			if (WorldgenSettings.Revamped)
				DesertHive.Place(description);
			else
				orig(description);
		}

		public static bool ReplaceChest(OnWorldGen.orig_AddBuriedChest_int_int_int_bool_int_bool_ushort orig, int i,
			int j, int contain, bool notNearOtherChests, int style, bool trySlope, ushort chestTileType)
		{
			return WorldgenSettings.Revamped
				? Chest.AddBuriedChest(i, j, contain, notNearOtherChests, style, chestTileType)
				: orig(i, j, contain, notNearOtherChests, style, trySlope, chestTileType);
		}

		public static void ReplaceDungeon(OnWorldGen.orig_MakeDungeon orig, int x, int y)
		{
			if (WorldgenSettings.Revamped)
				Dungeon.MakeDungeon(x, y);
			else
				orig(x, y);
		}

		public static DesertDescription ReplaceDesertDescriptionCreation(
			OnDesertDescription.orig_CreateFromPlacement orig, Point origin)
		{
			return WorldgenSettings.Revamped ? Desert.CreateFromPlacement(origin) : orig(origin);
		}
	}
}