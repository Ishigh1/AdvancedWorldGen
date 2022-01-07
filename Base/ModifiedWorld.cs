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

namespace AdvancedWorldGen.Base;

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
			Legacy.ReplaceOldOptions(OptionHelper.Options);
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
		OptionHelper.Options.Clear();

		string optionName;
		while ((optionName = reader.ReadString()) != "") OptionHelper.Options.Add(optionName);

		Main.checkHalloween();
		Main.checkXMas();
	}

	public override void NetSend(BinaryWriter writer)
	{
		foreach (string seedHelperOption in OptionHelper.Options)
			writer.Write(seedHelperOption);

		writer.Write("");
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
		int passIndex = tasks.FindIndex(pass => pass.Name == "Shinies");
		if (passIndex != -1)
		{
			tasks.Insert(passIndex++, new PassLegacy("BothOres1", BothOres.BothOres1));
			passIndex++;
			tasks.Insert(passIndex++, new PassLegacy("BothOres2", BothOres.BothOres2));
		}

		passIndex = tasks.FindIndex(pass => pass.Name == "Corruption");
		if (passIndex != -1)
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
		if (passIndex != -1 && API.OptionsContains("Drunk.Crimruption"))
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
		int alreadyPlaced = 0;
		if (OptionHelper.OptionsContains("Painted")) TryAddNpc(availableNPCs, Painter, ref alreadyPlaced, out _);

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

		if (OptionHelper.OptionsContains("Santa")) TryAddNpc(availableNPCs, SantaClaus, ref alreadyPlaced, out _);

		if (OptionHelper.OptionsContains("Random")) TryAddNpc(availableNPCs, RandomNpc(availableNPCs), ref alreadyPlaced, out _);

		if (availableNPCs.Count == NPCs.Count) TryAddNpc(availableNPCs, Guide, ref alreadyPlaced, out _);
	}

	public static bool TryAddNpc(List<int> availableNPCs, int npcType,
		ref int alreadyPlaced, [NotNullWhen(true)] out NPC? npc)
	{
		if (!availableNPCs.Contains(npcType))
		{
			npc = Main.npc.FirstOrDefault(n => n.type == npcType);
			return npc != null;
		}

		int spawnPointX = Main.spawnTileX * 16 + (alreadyPlaced % 2 == 0 ? alreadyPlaced : -(alreadyPlaced + 1));
		npc = Main.npc[NPC.NewNPC(spawnPointX, Main.spawnTileY * 16, npcType)];
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
		return WorldGen._genRand.NextFromList(availableNPCs.ToArray());
	}

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