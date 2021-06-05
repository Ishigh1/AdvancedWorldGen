using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvancedWorldGen.OptionUI;
using AdvancedWorldGen.SpecialOptions.Halloween;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using static Terraria.ID.NPCID;
using OnMain = On.Terraria.Main;

namespace AdvancedWorldGen.Base
{
	public class ModifiedWorld : ModSystem
	{
		public static OptionHelper OptionHelper;

		public static List<int> NPCs = new()
		{
			Merchant, Nurse, ArmsDealer, Dryad, Guide, Demolitionist, Clothier, GoblinTinkerer, Wizard, Mechanic,
			Truffle, Steampunker, DyeTrader, PartyGirl, Cyborg, Painter, WitchDoctor, Pirate, Stylist, Angler,
			TaxCollector, DD2Bartender, Golfer, BestiaryGirl, Princess, TownBunny, TownDog
		};

		public override void OnModLoad()
		{
			OptionHelper = new OptionHelper();
		}

		public override void Unload()
		{
			OptionHelper = null;
		}

		public override void OnWorldLoad()
		{
			OptionHelper.Options.Clear();
		}

		public override void LoadWorldData(TagCompound tag)
		{
			IList<string> list = tag.GetList<string>("Options");
			if (list != null)
			{
				OptionHelper.Options = new HashSet<string>(list);
				Main.checkHalloween();
				Main.checkXMas();
			}
		}

		public override TagCompound SaveWorldData()
		{
			return new()
			{
				{"Options", OptionHelper.Options.ToList()}
			};
		}

		public override void NetReceive(BinaryReader reader)
		{
			List<int> list = new();
			OptionHelper.Options.Clear();

			int id;
			while ((id = reader.ReadInt16()) != 0) list.Add(id);

			foreach (KeyValuePair<string, Option> keyValuePair in OptionsSelector.OptionDict.Where(keyValuePair =>
				list.Remove(keyValuePair.Value.Id)))
			{
				OptionHelper.Options.Add(keyValuePair.Key);
				if (list.Count == 0) break;
			}

			Main.checkHalloween();
			Main.checkXMas();
		}

		public override void NetSend(BinaryWriter writer)
		{
			foreach (string seedHelperOption in OptionHelper.Options)
				writer.Write(OptionsSelector.OptionDict[seedHelperOption].Id);

			writer.Write(0);
		}

		public override void PreWorldGen()
		{
			WorldGen.notTheBees = OptionsContains("NotTheBees", "SmallNotTheBees");
			Main.getGoodWorld = OptionsContains("ForTheWorthy");
			WorldGen.getGoodWorldGen = Main.getGoodWorld;
			Main.drunkWorld = OptionsContains("Drunk");
			WorldGen.drunkWorldGen = Main.drunkWorld;
			WorldGen.drunkWorldGenText = Main.drunkWorld;
			Main.tenthAnniversaryWorld = OptionsContains("Celebrationmk10");
			if (!Main.dayTime) Main.time = 0;
		}

		//Deletes all the now-useless stuff about special seeds
		public static void OverrideWorldOptions(ILContext il)
		{
			ILCursor cursor = new(il);
			if (cursor.TryGotoNext(instruction => instruction.MatchLdcI4(0)))
				while (!cursor.Next.MatchLdstr(
					"Creating world - Seed: {0} Width: {1}, Height: {2}, Evil: {3}, IsExpert: {4}"))
					cursor.Remove();
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			int passIndex = tasks.FindIndex(pass => pass.Name == "Guide");
			if (passIndex != -1)
			{
				tasks.RemoveAt(passIndex);
				tasks.Insert(passIndex, new PassLegacy("NPCs", HandleNpcs));
			}

			passIndex = tasks.FindIndex(passIndex, pass => pass.Name == "Micro Biomes");
			if (passIndex != -1)
				passIndex = HalloweenCommon.InsertTasks(tasks, passIndex);

			tasks.Add(new PassLegacy("Tile Switch", ReplaceTiles));
		}

		public static void HandleNpcs(GenerationProgress progress, GameConfiguration configuration)
		{
			List<int> availableNPCs = NPCs.ToList();
			if (OptionHelper.OptionsContains("Painted")) AddNpc(Painter, availableNPCs);

			if (WorldGen.notTheBees) AddNpc(Merchant, availableNPCs);

			if (WorldGen.getGoodWorldGen) AddNpc(Demolitionist, availableNPCs);

			if (WorldGen.drunkWorldGen) AddNpc(PartyGirl, availableNPCs);

			if (Main.tenthAnniversaryWorld)
			{
				AddNpc(Princess, availableNPCs);
				AddNpc(Steampunker, availableNPCs);
				AddNpc(Guide, availableNPCs);
				AddNpc(PartyGirl, availableNPCs);
				AddNpc(TownBunny, availableNPCs);
			}

			if (OptionHelper.OptionsContains("Santa")) AddNpc(SantaClaus, availableNPCs);

			if (OptionHelper.OptionsContains("Random")) AddNpc(RandomNpc(availableNPCs), availableNPCs);

			if (availableNPCs.Count == NPCs.Count) AddNpc(Guide, availableNPCs);
		}

		public static void AddNpc(int npc, List<int> availableNPCs)
		{
			if(!availableNPCs.Contains(npc)) return;
			int npcId = NPC.NewNPC(Main.spawnTileX * 16, Main.spawnTileY * 16, npc);
			Main.npc[npcId].homeTileX = Main.spawnTileX;
			Main.npc[npcId].homeTileY = Main.spawnTileY;
			Main.npc[npcId].direction = Main.rand.Next(2) == 0 ? 1 : -1;
			Main.npc[npcId].homeless = true;
			availableNPCs.RemoveAll(i => i == npc);
		}

		public static int RandomNpc(List<int> availableNPCs)
		{
			return Utils.SelectRandom(Main.rand, availableNPCs.ToArray());
		}

		public static void ReplaceTiles(GenerationProgress progress, GameConfiguration configuration)
		{
			if (OptionHelper.OptionsContains("Santa"))
				TileReplacer.Snow.ReplaceTiles(progress, "Making the world colder");

			if (OptionHelper.OptionsContains("Random", "Painted")) TileReplacer.RandomizeWorld(progress, OptionHelper);
		}

		public override void PostUpdateTime()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) OptionHelper.OnTick();
		}

		public static void OnDawn(OnMain.orig_UpdateTime_StartDay orig, ref bool stopEvents)
		{
			orig(ref stopEvents);
			OptionHelper.OnDawn();
		}

		public static void OnDusk(OnMain.orig_UpdateTime_StartNight orig, ref bool stopEvents)
		{
			orig(ref stopEvents);
			OptionHelper.OnDusk();
		}

		public static bool OptionsContains(params string[] s)
		{
			return OptionHelper.OptionsContains(s);
		}
	}
}