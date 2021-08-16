using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvancedWorldGen.BetterVanillaWorldGen;
using AdvancedWorldGen.SpecialOptions;
using AdvancedWorldGen.SpecialOptions.Halloween;
using AdvancedWorldGen.UI;
using MonoMod.Cil;
using On.Terraria.UI;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.Generation;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using static Terraria.ID.NPCID;
using OnMain = On.Terraria.Main;
using UIState = Terraria.UI.UIState;

namespace AdvancedWorldGen.Base
{
	public class ModifiedWorld : ModSystem
	{
		public static List<int> NPCs = new()
		{
			Merchant, Nurse, ArmsDealer, Dryad, Guide, Demolitionist, Clothier, GoblinTinkerer, Wizard, Mechanic,
			Truffle, Steampunker, DyeTrader, PartyGirl, Cyborg, Painter, WitchDoctor, Pirate, Stylist, Angler,
			TaxCollector, DD2Bartender, Golfer, BestiaryGirl, Princess, TownBunny, TownDog
		};

		public CustomSizeUI CustomSizeUI;

		public OptionHelper OptionHelper;
		public static ModifiedWorld Instance => ModContent.GetInstance<ModifiedWorld>();

		public override void OnModLoad()
		{
			OptionHelper = new OptionHelper();
			CustomSizeUI = new CustomSizeUI(OptionHelper.WorldSettings);
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
			Replacer.ReplaceGenPasses(tasks);
			int passIndex = tasks.FindIndex(pass => pass.Name == "Corruption");
			if (passIndex != -1 && !WorldgenSettings.Revamped && OptionsContains("Crimruption"))
			{
				tasks.Insert(passIndex++, new PassLegacy("Crimruption1", Crimruption.Crimruption1));
				passIndex++;
				tasks.Insert(passIndex++, new PassLegacy("Crimruption2", Crimruption.Crimruption2));
			}

			passIndex = tasks.FindIndex(passIndex, pass => pass.Name == "Guide");
			if (passIndex != -1)
			{
				tasks.RemoveAt(passIndex);
				tasks.Insert(passIndex, new PassLegacy("NPCs", HandleNpcs));
			}
			
			passIndex = tasks.FindIndex(passIndex, pass => pass.Name == "Tile Cleanup");
			if (passIndex != -1 && OptionsContains("Crimruption"))
			{
				tasks.Insert(passIndex++, new PassLegacy("Crimruption3", Crimruption.Crimruption3));
				passIndex++;
				tasks.Insert(passIndex++, new PassLegacy("Crimruption4", Crimruption.Crimruption4));
			}

			passIndex = tasks.FindIndex(passIndex, pass => pass.Name == "Micro Biomes");
			if (passIndex != -1)
				HalloweenCommon.InsertTasks(tasks, ref passIndex);

			tasks.Add(new PassLegacy("Tile Switch", ReplaceTiles));
		}

		public void HandleNpcs(GenerationProgress progress, GameConfiguration configuration)
		{
			List<int> availableNPCs = NPCs.ToList();
			if (OptionHelper.OptionsContains("Painted")) AddNpc(Painter, availableNPCs);

			if (WorldGen.notTheBees) AddNpc(Merchant, availableNPCs);

			if (WorldGen.getGoodWorldGen) AddNpc(Demolitionist, availableNPCs);

			if (WorldGen.drunkWorldGen) AddNpc(PartyGirl, availableNPCs);

			if (Main.tenthAnniversaryWorld)
			{
				BirthdayParty.GenuineParty = true;
				BirthdayParty.PartyDaysOnCooldown = 5;

				NPC princess = AddNpc(Princess, availableNPCs);
				princess.GivenName = Language.GetTextValue("PrincessNames.Yorai");
				BirthdayParty.CelebratingNPCs.Add(princess.whoAmI);

				NPC steampunker = AddNpc(Steampunker, availableNPCs);
				steampunker.GivenName = Language.GetTextValue("SteampunkerNames.Whitney");
				BirthdayParty.CelebratingNPCs.Add(steampunker.whoAmI);

				NPC guide = AddNpc(Guide, availableNPCs);
				guide.GivenName = Language.GetTextValue("GuideNames.Andrew");
				BirthdayParty.CelebratingNPCs.Add(guide.whoAmI);

				AddNpc(PartyGirl, availableNPCs);

				NPC bnnuy = AddNpc(TownBunny, availableNPCs);
				bnnuy.townNpcVariationIndex = 1;
				NPC.boughtBunny = true;
			}

			if (OptionHelper.OptionsContains("Santa")) AddNpc(SantaClaus, availableNPCs);

			if (OptionHelper.OptionsContains("Random")) AddNpc(RandomNpc(availableNPCs), availableNPCs);

			if (availableNPCs.Count == NPCs.Count) AddNpc(Guide, availableNPCs);
		}

		public static NPC AddNpc(int npcType, List<int> availableNPCs)
		{
			if (!availableNPCs.Contains(npcType)) return Main.npc.FirstOrDefault(npc => npc.type == npcType);

			NPC newNPC = Main.npc[NPC.NewNPC(Main.spawnTileX * 16, Main.spawnTileY * 16, npcType)];
			newNPC.homeTileX = Main.spawnTileX;
			newNPC.homeTileY = Main.spawnTileY;
			newNPC.direction = WorldGen._genRand.Next(2) == 0 ? 1 : -1;
			newNPC.homeless = true;
			availableNPCs.RemoveAll(i => i == npcType);
			return newNPC;
		}

		public static int RandomNpc(List<int> availableNPCs)
		{
			return Utils.SelectRandom(WorldGen._genRand, availableNPCs.ToArray());
		}

		public void ReplaceTiles(GenerationProgress progress, GameConfiguration configuration)
		{
			if (OptionHelper.OptionsContains("Santa"))
				TileReplacer.Snow.ReplaceTiles(progress, "Making the world colder");

			if (OptionHelper.OptionsContains("Random", "Painted")) TileReplacer.RandomizeWorld(progress, OptionHelper);
		}

		public override void PostUpdateTime()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) OptionHelper.OnTick();
		}

		public void OnDawn(OnMain.orig_UpdateTime_StartDay orig, ref bool stopEvents)
		{
			orig(ref stopEvents);
			OptionHelper.OnDawn();
		}

		public void OnDusk(OnMain.orig_UpdateTime_StartNight orig, ref bool stopEvents)
		{
			orig(ref stopEvents);
			OptionHelper.OnDusk();
		}

		public static bool OptionsContains(params string[] s)
		{
			return Instance.OptionHelper.OptionsContains(s);
		}

		public void ResetSettings(UserInterface.orig_SetState orig, Terraria.UI.UserInterface self, UIState state)
		{
			orig(self, state);
			if (state is UIWorldSelect)
			{
				OptionHelper.Options = new HashSet<string>();
				OptionHelper.WorldSettings.SetSizeTo(-1);
			}
		}
	}
}