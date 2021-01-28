using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using static Terraria.ID.NPCID;

namespace AdvancedSeedGen
{
	public class CustomSeededWorld : ModWorld
	{
		public SeedHelper SeedHelper;

		public override void Initialize()
		{
			SeedHelper = null;
		}

		public override void Load(TagCompound tag)
		{
			SeedHelper = new SeedHelper((List<string>) tag.GetList<string>("Options"));
			Main.checkXMas();
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				{"Options", SeedHelper.Options}
			};
		}

		public override void PreWorldGen()
		{
			SeedHelper = new SeedHelper(Main.ActiveWorldFileData.SeedText);
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

		public void HandleNpcs(GenerationProgress progress)
		{
			int npcs = 0;
			if (SeedHelper.OptionsContains("Random"))
			{
				AddNpc(RandomNpc());
				npcs++;
			}

			if (SeedHelper.OptionsContains("Paint"))
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
			int npcId = NPC.NewNPC(Main.spawnTileX * 16, Main.spawnTileY * 16, npc);
			Main.npc[npcId].homeTileX = Main.spawnTileX;
			Main.npc[npcId].homeTileY = Main.spawnTileY;
			Main.npc[npcId].direction = 1;
			Main.npc[npcId].homeless = true;
		}

		public int RandomNpc()
		{
			List<int> npcs = new List<int>
			{
				Nurse, ArmsDealer, Dryad, Guide, Clothier, GoblinTinkerer, Wizard, Mechanic, Truffle, Steampunker,
				DyeTrader, Cyborg, WitchDoctor, Pirate, Stylist, Angler, TaxCollector, DD2Bartender
			};
			if (!SeedHelper.OptionsContains("Paint")) npcs.Add(Painter);

			return Utils.SelectRandom(Main.rand, npcs.ToArray());
		}

		private void ReplaceTiles(GenerationProgress progress)
		{
			if (SeedHelper.OptionsContains("Santa"))
				TileReplacer.Snow.ReplaceTiles(progress, "Making the world colder");

			if (SeedHelper.OptionsContains("Random", "Paint")) TileReplacer.RandomizeWorld(progress, SeedHelper);
		}

		public override void PostUpdate()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				SeedHelper.OnTick();
				if (Main.time == 0)
				{
					if (Main.dayTime)
						SeedHelper.OnDawn();
					else
						SeedHelper.OnDusk();
				}
			}
		}

		public static bool OptionsContains(params string[] s)
		{
			SeedHelper seedHelper =
				((ModLoader.GetMod("AdvancedSeedGen") as AdvancedSeedGen)?.GetModWorld("CustomSeededWorld") as
					CustomSeededWorld)?.SeedHelper;
			return seedHelper != null && seedHelper.OptionsContains(s);
		}
	}
}