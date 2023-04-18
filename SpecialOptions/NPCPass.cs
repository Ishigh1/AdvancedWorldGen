namespace AdvancedWorldGen.SpecialOptions;

public class NPCPass : ControlledWorldGenPass
{
	public static readonly List<int> NPCs = new()
	{
		NPCID.Merchant, NPCID.Nurse, NPCID.ArmsDealer, NPCID.Dryad, NPCID.Guide, NPCID.Demolitionist, NPCID.Clothier,
		NPCID.GoblinTinkerer, NPCID.Wizard, NPCID.Mechanic,
		NPCID.Truffle, NPCID.Steampunker, NPCID.DyeTrader, NPCID.PartyGirl, NPCID.Cyborg, NPCID.Painter,
		NPCID.WitchDoctor, NPCID.Pirate, NPCID.Stylist, NPCID.Angler,
		NPCID.TaxCollector, NPCID.DD2Bartender, NPCID.Golfer, NPCID.BestiaryGirl, NPCID.Princess, NPCID.TownBunny,
		NPCID.TownDog
	};

	public NPCPass() : base("Guide", 0.016f)
	{
	}

	protected override void ApplyPass()
	{
		HashSet<int> availableNPCs = NPCs.ToHashSet();
		int alreadyPlaced = 0;
		if (OptionHelper.OptionsContains("Random.Painted"))
			TryAddNpc(availableNPCs, NPCID.Painter, ref alreadyPlaced, out _);

		if (WorldGen.notTheBees) TryAddNpc(availableNPCs, NPCID.Merchant, ref alreadyPlaced, out _);

		if (WorldGen.getGoodWorldGen) TryAddNpc(availableNPCs, NPCID.Demolitionist, ref alreadyPlaced, out _);

		if (WorldGen.drunkWorldGen) TryAddNpc(availableNPCs, NPCID.PartyGirl, ref alreadyPlaced, out _);

		if (WorldGen.tenthAnniversaryWorldGen)
		{
			BirthdayParty.GenuineParty = true;
			BirthdayParty.PartyDaysOnCooldown = 5;

			if (TryAddNpc(availableNPCs, NPCID.Princess, ref alreadyPlaced, out NPC? princess))
			{
				princess.GivenName = Language.GetTextValue("PrincessNames.Yorai");
				BirthdayParty.CelebratingNPCs.Add(princess.whoAmI);
			}

			if (TryAddNpc(availableNPCs, NPCID.Steampunker, ref alreadyPlaced, out NPC? steampunker))
			{
				steampunker.GivenName = Language.GetTextValue("SteampunkerNames.Whitney");
				BirthdayParty.CelebratingNPCs.Add(steampunker.whoAmI);
			}

			if (TryAddNpc(availableNPCs, NPCID.Guide, ref alreadyPlaced, out NPC? guide))
			{
				guide.GivenName = Language.GetTextValue("GuideNames.Andrew");
				BirthdayParty.CelebratingNPCs.Add(guide.whoAmI);
			}

			TryAddNpc(availableNPCs, NPCID.PartyGirl, ref alreadyPlaced, out _);

			if (TryAddNpc(availableNPCs, NPCID.TownBunny, ref alreadyPlaced, out NPC? bunny))
			{
				bunny.townNpcVariationIndex = 1;
				NPC.boughtBunny = true;
			}
		}

		if (OptionHelper.OptionsContains("Santa")) TryAddNpc(availableNPCs, NPCID.SantaClaus, ref alreadyPlaced, out _);

		if (OptionHelper.OptionsContains("Random"))
			TryAddNpc(availableNPCs, RandomNpc(availableNPCs), ref alreadyPlaced, out _);

		if (alreadyPlaced == 0) TryAddNpc(availableNPCs, NPCID.Guide, ref alreadyPlaced, out _);
	}

	public static bool TryAddNpc(HashSet<int> availableNPCs, int npcType,
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
		availableNPCs.Remove(npcType);

		if (_100kWorld.Enabled && alreadyPlaced == 0)
		{
			npc.homeless = false;
			npc.homeTileX = Main.spawnTileX;
			npc.homeTileY = Main.spawnTileY;
		}
		else
		{
			npc.homeless = true;
		}

		alreadyPlaced++;

		return true;
	}

	public static int RandomNpc(HashSet<int> availableNPCs)
	{
		return WorldGen.genRand.NextFromList(availableNPCs.ToArray());
	}
}