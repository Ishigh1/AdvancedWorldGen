using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using AdvancedWorldGen.BetterVanillaWorldGen;
using AdvancedWorldGen.SpecialOptions;
using AdvancedWorldGen.SpecialOptions.Halloween;
using AdvancedWorldGen.SpecialOptions.Snow;
using AdvancedWorldGen.UI;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.Generation;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.WorldBuilding;
using static Terraria.ID.NPCID;
using OnMain = On.Terraria.Main;
using OnUserInterface = On.Terraria.UI.UserInterface;

namespace AdvancedWorldGen.Base
{
	public class ModifiedWorld : ModSystem
	{
		public static readonly List<int> NPCs = new()
		{
			Merchant, Nurse, ArmsDealer, Dryad, Guide, Demolitionist, Clothier, GoblinTinkerer, Wizard, Mechanic,
			Truffle, Steampunker, DyeTrader, PartyGirl, Cyborg, Painter, WitchDoctor, Pirate, Stylist, Angler,
			TaxCollector, DD2Bartender, Golfer, BestiaryGirl, Princess, TownBunny, TownDog
		};

		public CustomSizeUI CustomSizeUI = null!;

		public OptionHelper OptionHelper = null!;

		public static ModifiedWorld Instance => ModContent.GetInstance<ModifiedWorld>();

		public override void OnModLoad()
		{
			OptionHelper = new OptionHelper();
			if (!Main.dedServ)
				CustomSizeUI = new CustomSizeUI(OptionHelper.WorldSettings);
		}

		public override void Unload()
		{
			OptionHelper = null!;
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

		public override void SaveWorldData(TagCompound tagCompound)
		{
			if (OptionHelper.Options.Count != 0)
				tagCompound["Options"] = OptionHelper.Options.ToList();
		}

		public override void NetReceive(BinaryReader reader)
		{
			List<int> list = new();
			OptionHelper.Options.Clear();

			int id;
			while ((id = reader.ReadUInt16()) != 0) list.Add(id);

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
			cursor.GotoNext(instruction => instruction.MatchLdcI4(0));
			while (!cursor.Next.MatchLdstr(
				"Creating world - Seed: {0} Width: {1}, Height: {2}, Evil: {3}, IsExpert: {4}"))
				cursor.Remove();
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			Replacer.ReplaceGenPasses(tasks);
			int passIndex = tasks.FindIndex(pass => pass.Name == "Corruption");
			if (passIndex != -1 && OptionsContains("Crimruption"))
			{
				tasks.Insert(passIndex++, new PassLegacy("Crimruption1", Crimruption.Crimruption1));
				passIndex++;
				tasks.Insert(passIndex++, new PassLegacy("Crimruption2", Crimruption.Crimruption2));
			}

			passIndex = tasks.FindIndex(pass => pass.Name == "Guide");
			if (passIndex != -1)
				tasks[passIndex] = new PassLegacy("NPCs", HandleNpcs);

			GenPass? liquidSettle = null;
			passIndex = tasks.FindIndex(pass => pass.Name == "Settle Liquids Again");
			if (passIndex != -1)
				liquidSettle = tasks[passIndex];

			passIndex = tasks.FindIndex(pass => pass.Name == "Tile Cleanup");
			if (passIndex != -1 && OptionsContains("Crimruption"))
			{
				tasks.Insert(passIndex++, new PassLegacy("Crimruption3", Crimruption.Crimruption3));
				passIndex++;
				tasks.Insert(passIndex++, new PassLegacy("Crimruption4", Crimruption.Crimruption4));
			}

			passIndex = tasks.FindIndex(pass => pass.Name == "Micro Biomes");
			if (passIndex != -1)
				HalloweenCommon.InsertTasks(tasks, ref passIndex);

			if (OptionHelper.OptionsContains("Santa", "Evil", "Random", "Painted"))
			{
				tasks.Add(new PassLegacy("Tile Switch", ReplaceTiles));
				if (liquidSettle != null)
					tasks.Add(liquidSettle);
			}
		}

		public void HandleNpcs(GenerationProgress progress, GameConfiguration configuration)
		{
			List<int> availableNPCs = NPCs.ToList();
			if (OptionHelper.OptionsContains("Painted")) TryAddNpc(Painter, availableNPCs, out _);

			if (WorldGen.notTheBees) TryAddNpc(Merchant, availableNPCs, out _);

			if (WorldGen.getGoodWorldGen) TryAddNpc(Demolitionist, availableNPCs, out _);

			if (WorldGen.drunkWorldGen) TryAddNpc(PartyGirl, availableNPCs, out _);

			if (Main.tenthAnniversaryWorld)
			{
				BirthdayParty.GenuineParty = true;
				BirthdayParty.PartyDaysOnCooldown = 5;

				if (TryAddNpc(Princess, availableNPCs, out NPC? princess))
				{
					princess.GivenName = Language.GetTextValue("PrincessNames.Yorai");
					BirthdayParty.CelebratingNPCs.Add(princess.whoAmI);
				}

				if (TryAddNpc(Steampunker, availableNPCs, out NPC? steampunker))
				{
					steampunker.GivenName = Language.GetTextValue("SteampunkerNames.Whitney");
					BirthdayParty.CelebratingNPCs.Add(steampunker.whoAmI);
				}

				if (TryAddNpc(Guide, availableNPCs, out NPC? guide))
				{
					guide.GivenName = Language.GetTextValue("GuideNames.Andrew");
					BirthdayParty.CelebratingNPCs.Add(guide.whoAmI);
				}

				TryAddNpc(PartyGirl, availableNPCs, out _);

				if (TryAddNpc(TownBunny, availableNPCs, out NPC? bunny))
				{
					bunny.townNpcVariationIndex = 1;
					NPC.boughtBunny = true;
				}
			}

			if (OptionHelper.OptionsContains("Santa")) TryAddNpc(SantaClaus, availableNPCs, out _);

			if (OptionHelper.OptionsContains("Random")) TryAddNpc(RandomNpc(availableNPCs), availableNPCs, out _);

			if (availableNPCs.Count == NPCs.Count) TryAddNpc(Guide, availableNPCs, out _);
		}

		public static bool TryAddNpc(int npcType, List<int> availableNPCs, [NotNullWhen(true)] out NPC? npc)
		{
			if (!availableNPCs.Contains(npcType))
			{
				npc = Main.npc.FirstOrDefault(n => n.type == npcType);
				return npc != null;
			}

			npc = Main.npc[NPC.NewNPC(Main.spawnTileX * 16, Main.spawnTileY * 16, npcType)];
			npc.homeTileX = Main.spawnTileX;
			npc.homeTileY = Main.spawnTileY;
			npc.direction = WorldGen._genRand.NextBool(2) ? 1 : -1;
			npc.homeless = true;
			availableNPCs.RemoveAll(i => i == npcType);

			return true;
		}

		public static int RandomNpc(List<int> availableNPCs) => WorldGen._genRand.NextFromList(availableNPCs.ToArray());

		public void ReplaceTiles(GenerationProgress progress, GameConfiguration configuration)
		{
			if (OptionHelper.OptionsContains("Santa"))
				new SnowReplacer().ReplaceTiles(progress, "SnowReplace");

			if (OptionHelper.OptionsContains("Evil"))
				EvilReplacer.CorruptWorld(progress);

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

		public static bool OptionsContains(params string[] s) => Instance.OptionHelper.OptionsContains(s);

		public void ResetSettings(OnUserInterface.orig_SetState orig, UserInterface self, UIState state)
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