using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using AdvancedWorldGen.BetterVanillaWorldGen;
using AdvancedWorldGen.CustomSized;
using AdvancedWorldGen.Helper;
using AdvancedWorldGen.SpecialOptions;
using AdvancedWorldGen.SpecialOptions.Halloween;
using AdvancedWorldGen.SpecialOptions.Snow;
using AdvancedWorldGen.UI;
using MonoMod.Cil;
using On.Terraria.GameContent.UI.States;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameContent.Generation;
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
using UIWorldSelect = Terraria.GameContent.UI.States.UIWorldSelect;

namespace AdvancedWorldGen.Base;

public class ModifiedWorld : ModSystem
{
	public static readonly List<int> NPCs = new()
	{
		Merchant, Nurse, ArmsDealer, Dryad, Guide, Demolitionist, Clothier, GoblinTinkerer, Wizard, Mechanic,
		Truffle, Steampunker, DyeTrader, PartyGirl, Cyborg, Painter, WitchDoctor, Pirate, Stylist, Angler,
		TaxCollector, DD2Bartender, Golfer, BestiaryGirl, Princess, TownBunny, TownDog
	};

	public OptionHelper OptionHelper = null!;

	public static ModifiedWorld Instance => ModContent.GetInstance<ModifiedWorld>();

	public override void OnModLoad()
	{
		OptionHelper = new OptionHelper(Mod);
	}

	public override void Unload()
	{
		OptionHelper = null!;
	}

	public override void OnWorldLoad()
	{
		OptionHelper.ClearAll();
	}

	public override void LoadWorldData(TagCompound tag)
	{
		if (tag.TryGet("Options", out List<string> options))
		{
			OptionHelper.Import(options);
			Main.checkHalloween();
			Main.checkXMas();
		}
	}

	public override void SaveWorldData(TagCompound tagCompound)
	{
		List<string> options = OptionHelper.Export();
		if (options.Count > 0)
			tagCompound.Add("Options", options);
	}

	public override void PreWorldGen()
	{
		Main.notTheBeesWorld = API.OptionsContains("NotTheBees");
		WorldGen.notTheBees = Main.notTheBeesWorld;
		Main.getGoodWorld = API.OptionsContains("ForTheWorthy");
		WorldGen.getGoodWorldGen = Main.getGoodWorld;
		Main.drunkWorld = API.OptionsContains("Drunk");
		WorldGen.drunkWorldGen = Main.drunkWorld;
		WorldGen.drunkWorldGenText = Main.drunkWorld;
		Main.tenthAnniversaryWorld = API.OptionsContains("Celebrationmk10");
		WorldGen.tenthAnniversaryWorldGen = Main.tenthAnniversaryWorld;
		Main.dontStarveWorld = API.OptionsContains("TheConstant");
		WorldGen.dontStarveWorldGen = Main.dontStarveWorld;
		if (!Main.dayTime) Main.time = 0;
	}

	public override void NetReceive(BinaryReader reader)
	{
		List<string> options = new();
		string optionName;
		while ((optionName = reader.ReadString()) != "") options.Add(optionName);

		OptionHelper.Import(options);

		Main.checkHalloween();
		Main.checkXMas();
	}

	public override void NetSend(BinaryWriter writer)
	{
		List<string> optionNames = OptionHelper.Export();
		foreach (string optionName in optionNames)
			writer.Write(optionName);

		writer.Write("");
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

		DrunkOptions.AddDrunkEdits(tasks);

		int passIndex = tasks.FindIndex(pass => pass.Name == "Guide");
		if (passIndex != -1)
			tasks[passIndex] = new PassLegacy("NPCs", HandleNpcs);

		passIndex = tasks.FindIndex(pass => pass.Name == "Micro Biomes");
		if (passIndex != -1)
			HalloweenCommon.InsertTasks(tasks, ref passIndex);

		if (API.OptionsContains("Santa", "Evil", "Random", "Random.Painted"))
		{
			tasks.Add(new PassLegacy("Tile Switch", ReplaceTiles));
			GenPass? liquidSettle = null;
			passIndex = tasks.FindIndex(pass => pass.Name == "Settle Liquids Again");
			if (passIndex != -1)
				liquidSettle = tasks[passIndex];
			if (liquidSettle != null)
				tasks.Add(liquidSettle);
		}
	}

	public void HandleNpcs(GenerationProgress progress, GameConfiguration configuration)
	{
		List<int> availableNPCs = NPCs.ToList();
		int alreadyPlaced = 0;
		if (API.OptionsContains("Random.Painted")) TryAddNpc(availableNPCs, Painter, ref alreadyPlaced, out _);

		if (WorldGen.notTheBees) TryAddNpc(availableNPCs, Merchant, ref alreadyPlaced, out _);

		if (WorldGen.getGoodWorldGen) TryAddNpc(availableNPCs, Demolitionist, ref alreadyPlaced, out _);

		if (WorldGen.drunkWorldGen) TryAddNpc(availableNPCs, PartyGirl, ref alreadyPlaced, out _);

		if (WorldGen.tenthAnniversaryWorldGen)
		{
			BirthdayParty.GenuineParty = true;
			BirthdayParty.PartyDaysOnCooldown = 5;

			if (TryAddNpc(availableNPCs, Princess, ref alreadyPlaced, out NPC? princess))
			{
				princess.GivenName = Language.GetTextValue("PrincessNames.Yorai");
				BirthdayParty.CelebratingNPCs.Add(princess.whoAmI);
			}

			if (TryAddNpc(availableNPCs, Steampunker, ref alreadyPlaced, out NPC? steampunker))
			{
				steampunker.GivenName = Language.GetTextValue("SteampunkerNames.Whitney");
				BirthdayParty.CelebratingNPCs.Add(steampunker.whoAmI);
			}

			if (TryAddNpc(availableNPCs, Guide, ref alreadyPlaced, out NPC? guide))
			{
				guide.GivenName = Language.GetTextValue("GuideNames.Andrew");
				BirthdayParty.CelebratingNPCs.Add(guide.whoAmI);
			}

			TryAddNpc(availableNPCs, PartyGirl, ref alreadyPlaced, out _);

			if (TryAddNpc(availableNPCs, TownBunny, ref alreadyPlaced, out NPC? bunny))
			{
				bunny.townNpcVariationIndex = 1;
				NPC.boughtBunny = true;
			}
		}

		if (API.OptionsContains("Santa")) TryAddNpc(availableNPCs, SantaClaus, ref alreadyPlaced, out _);

		if (API.OptionsContains("Random")) TryAddNpc(availableNPCs, RandomNpc(availableNPCs), ref alreadyPlaced, out _);

		if (alreadyPlaced == 0) TryAddNpc(availableNPCs, Guide, ref alreadyPlaced, out _);
	}

	public static bool TryAddNpc(List<int> availableNPCs, int npcType,
		ref int alreadyPlaced, [NotNullWhen(true)] out NPC? npc)
	{
		if (!availableNPCs.Contains(npcType))
		{
			npc = Main.npc.FirstOrDefault(n => n.type == npcType);
			return npc != null;
		}

		int spawnPointX = Main.spawnTileX + (alreadyPlaced % 2 == 0 ? alreadyPlaced : -(alreadyPlaced + 1));
		npc = Main.npc[NPC.NewNPC(new EntitySource_WorldGen(), spawnPointX * 16, Main.spawnTileY * 16, npcType)];
		npc.homeTileX = spawnPointX;
		npc.homeTileY = Main.spawnTileY;
		npc.direction = alreadyPlaced % 2 == 0 ? 1 : -1;
		npc.homeless = true;
		availableNPCs.Remove(npcType);

		alreadyPlaced++;

		return true;
	}

	public static int RandomNpc(List<int> availableNPCs)
	{
		return WorldGen.genRand.NextFromList(availableNPCs.ToArray());
	}

	public static void ReplaceTiles(GenerationProgress progress, GameConfiguration configuration)
	{
		if (API.OptionsContains("Santa")) new SnowReplacer().ReplaceTiles(progress, "SnowReplace");

		if (API.OptionsContains("Evil")) EvilReplacer.CorruptWorld(progress);

		if (API.OptionsContains("Random", "Random.Painted")) TileReplacer.RandomizeWorld(progress);
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

	public void ResetSettings(OnUserInterface.orig_SetState orig, UserInterface self, UIState state)
	{
		orig(self, state);
		if (state is UIWorldSelect)
		{
			OptionHelper.ClearAll();
			OptionHelper.WorldSettings.Params.Wipe();
		}
	}

	public void LastMinuteChecks(UIWorldCreation.orig_FinishCreatingWorld orig, Terraria.GameContent.UI.States.UIWorldCreation self)
	{
		Params worldSettingsParams = OptionHelper.WorldSettings.Params;

		void OrigWithLog()
		{
			Mod.Logger.Info($"Overhauled : {WorldgenSettings.Revamped}");
			Mod.Logger.Info("Options : " + OptionsParser.GetJsonText());
			orig(self);
		}

		if (ModLoader.TryGetMod("CalamityMod", out Mod _))
		{
			UIState currentState = UserInterface.ActiveInstance.CurrentState;

			UIState? Prev()
			{
				return currentState;
			}

			UIState? Next()
			{
				OrigWithLog();
				return null;
			}

			switch (worldSettingsParams.SizeX)
			{
				case < KnownLimits.ClamityMinX:
					Main.MenuUI.SetState(new WarningUI(Language.GetTextValue(
						"Mods.AdvancedWorldGen.InvalidSizes.ClamityMinX"), Prev, Next));
					return;
				case > KnownLimits.ClamityMaxX:
					Main.MenuUI.SetState(new WarningUI(Language.GetTextValue(
						"Mods.AdvancedWorldGen.InvalidSizes.ClamityMaxX"), Prev, Next));
					return;
			}
		}

		OrigWithLog();
	}
}