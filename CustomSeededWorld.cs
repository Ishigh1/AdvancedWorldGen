using System.Collections.Generic;
using On.Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using static Terraria.ID.NPCID;
using NPC = Terraria.NPC;
using Utils = Terraria.Utils;

namespace AdvancedWorldGen
{
	public class CustomSeededWorld : ModSystem
	{
		public SeedHelper SeedHelper;

		public override void LoadWorldData(TagCompound tag)
		{
			SeedHelper = new SeedHelper((List<string>) tag.GetList<string>("Options"));
			Terraria.Main.checkXMas();
		}

		public override TagCompound SaveWorldData()
		{
			return new TagCompound
			{
				{"Options", SeedHelper.Options}
			};
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			int npcIndex = tasks.FindIndex(pass => pass.Name == "Guide");
			if (npcIndex != -1)
			{
				tasks.RemoveAt(npcIndex);
				tasks.Insert(npcIndex, new PassLegacy("NPCs", HandleNpcs));
			}

			tasks.Add(new PassLegacy("Tile Switch", ReplaceTiles));
		}

		public void HandleNpcs(GenerationProgress progress, GameConfiguration configuration)
		{
			int npcs = 0;
			if (SeedHelper.OptionsContains("Random"))
			{
				AddNpc(RandomNpc());
				npcs++;
			}

			if (SeedHelper.OptionsContains("Painted"))
			{
				AddNpc(Painter);
				npcs++;
			}

			if (SeedHelper.OptionsContains("Santa"))
			{
				AddNpc(SantaClaus);
				npcs++;
			}

			if (npcs == 0) AddNpc(Guide);
		}

		public static void AddNpc(int npc)
		{
			int npcId = NPC.NewNPC(Terraria.Main.spawnTileX * 16, Terraria.Main.spawnTileY * 16, npc);
			Terraria.Main.npc[npcId].homeTileX = Terraria.Main.spawnTileX;
			Terraria.Main.npc[npcId].homeTileY = Terraria.Main.spawnTileY;
			Terraria.Main.npc[npcId].direction = 1;
			Terraria.Main.npc[npcId].homeless = true;
		}

		public int RandomNpc()
		{
			List<int> npcs = new List<int>
			{
				Nurse, ArmsDealer, Dryad, Guide, Clothier, GoblinTinkerer, Wizard, Mechanic, Truffle, Steampunker,
				DyeTrader, Cyborg, WitchDoctor, Pirate, Stylist, Angler, TaxCollector, DD2Bartender
			};
			if (!SeedHelper.OptionsContains("Painted")) npcs.Add(Painter);

			return Utils.SelectRandom(Terraria.Main.rand, npcs.ToArray());
		}

		public void ReplaceTiles(GenerationProgress progress, GameConfiguration configuration)
		{
			if (SeedHelper.OptionsContains("Santa"))
				TileReplacer.Snow.ReplaceTiles(progress, "Making the world colder");

			if (SeedHelper.OptionsContains("Random", "Painted")) TileReplacer.RandomizeWorld(progress, SeedHelper);
		}

		public override void PostUpdateTime()
		{
			if (Terraria.Main.netMode != NetmodeID.MultiplayerClient) SeedHelper.OnTick();
		}

		public static void OnDawn(Main.orig_UpdateTime_StartDay orig, ref bool stopEvents)
		{
			GetCurrentWorld().SeedHelper.OnDawn();
		}

		public static void OnDusk(Main.orig_UpdateTime_StartNight orig, ref bool stopEvents)
		{
			GetCurrentWorld().SeedHelper.OnDusk();
		}

		public static CustomSeededWorld GetCurrentWorld()
		{
			return (ModContent.Find<ModSystem>("AdvancedWorldGen", "CustomSeededWorld") as CustomSeededWorld);
		}

		public static bool OptionsContains(params string[] s)
		{
			SeedHelper seedHelper = GetCurrentWorld().SeedHelper;
			return seedHelper != null && seedHelper.OptionsContains(s);
		}
	}
}