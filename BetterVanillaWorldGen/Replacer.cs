using System.Collections.Generic;
using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
using AdvancedWorldGen.BetterVanillaWorldGen.Jungle;
using Terraria.GameContent.Biomes.Desert;
using Terraria.WorldBuilding;
using AdvancedWorldGen.BetterVanillaWorldGen.DungeonStuff;
using DesertHive = AdvancedWorldGen.BetterVanillaWorldGen.DesertStuff.DesertHive;
using VanillaJunglePass = Terraria.GameContent.Biomes.JunglePass;
using OnJunglePass = On.Terraria.GameContent.Biomes.JunglePass;
using OnDesertHive = On.Terraria.GameContent.Biomes.Desert.DesertHive;
using OnWorldGen = On.Terraria.WorldGen;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class Replacer
	{
		public static VanillaInterface VanillaInterface;
		
		public static void Replace()
		{
			OnDesertHive.Place += ReplaceDesertHive;
			OnWorldGen.AddBuriedChest_int_int_int_bool_int_bool_ushort += ReplaceChest;
			OnWorldGen.MakeDungeon += ReplaceDungeon;
		}

		public static void UnReplace()
		{
			VanillaInterface = null;
			
			OnDesertHive.Place -= ReplaceDesertHive;
			OnWorldGen.AddBuriedChest_int_int_int_bool_int_bool_ushort -= ReplaceChest;
			OnWorldGen.MakeDungeon -= ReplaceDungeon;
		}

		public static void ReplaceGenPasses(List<GenPass> genPasses)
		{
			if (!WorldgenSettings.Revamped)
				return;
			int index = genPasses.FindIndex(pass => pass.Name == "Reset");
			if (index != -1)
			{
				VanillaInterface = new VanillaInterface(genPasses[index]);
				genPasses[index] = new Reset();
			}
			else
			{
				return;
			}

			index = genPasses.FindIndex(pass => pass.Name == "Terrain");
			if (index != -1) genPasses[index] = new TerrainPass();
			
			index = genPasses.FindIndex(index, pass => pass.Name == "Generate Ice Biome");
			if (index != -1) genPasses[index] = new GenerateIceBiome();

			index = genPasses.FindIndex(index, pass => pass.Name == "Jungle");
			if (index != -1) genPasses[index] = new JunglePass();
			
			index = genPasses.FindIndex(index, pass => pass.Name == "Mushroom Patches");
			if (index != -1) genPasses[index] = new MushroomPatches();

			index = genPasses.FindIndex(index, pass => pass.Name == "Jungle Temple");
			if (index != -1) genPasses[index] = new JungleTemple();

			index = genPasses.FindIndex(index, pass => pass.Name == "Jungle Chests");
			if (index != -1) genPasses[index] = new JungleChests();

			index = genPasses.FindIndex(index, pass => pass.Name == "Surface Ore and Stone");
			if (index != -1) genPasses[index] = new SurfaceOreAndStone();

			index = genPasses.FindIndex(index, pass => pass.Name == "Micro Biomes");
			if (index != -1) genPasses[index] = new MicroBiomes();
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
	}
}