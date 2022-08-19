namespace AdvancedWorldGen.Base;

public class OptionHelper
{
	public static Dictionary<string, Option> OptionDict = null!;
	public static WorldSettings WorldSettings;
	
	public static void InitializeDict(Mod mod)
	{
		WorldSettings = new WorldSettings();
		OptionDict = JsonConvert.DeserializeObject<Dictionary<string, Option>>(
			Encoding.UTF8.GetString(mod.GetFileBytes("Options.json")));

		for (int index = 0; index < OptionDict.Count; index++)
		{
			(_, Option? option) = OptionDict.ElementAt(index);
			if (option.Children.Count == 0)
				continue;
			Option baseOption = new()
			{
				Children = new List<Option>(),
				Conflicts = option.Conflicts,
				Name = "Base"
			};
			option.Conflicts = new List<string>();
			option.Children.Insert(0, baseOption);
			foreach (Option optionChild in option.Children)
			{
				optionChild.Parent = option;
				OptionDict.Add(optionChild.FullName, optionChild);
			}
		}

		foreach ((_, Option? option) in OptionDict)
			for (int index = 0; index < option.Conflicts.Count; index++)
			{
				string optionConflict = option.Conflicts[index];
				Option conflict = OptionDict[optionConflict];
				if (conflict.Children.Count != 0)
					option.Conflicts[index] = conflict.Children[0].Name;
			}
	}

	public static void ClearAll()
	{
		foreach ((string? _, Option? option) in OptionDict) option.Disable();
	}

	public static void Import(ICollection<string> optionNames)
	{
        ClearAll();
		Legacy.ReplaceOldOptions(optionNames);
		foreach (string optionName in optionNames)
			if (OptionDict.TryGetValue(optionName, out Option? option))
				option.WeakEnable();
	}

	public static List<string> Export()
	{
		List<string> list = new();
		foreach ((string? _, Option? option) in OptionDict)
			if (option.Enabled && option.Children.Count == 0)
				list.Add(option.FullName);
		return list;
	}

	public static bool OptionsContains(string optionName)
	{
		if (!OptionDict.TryGetValue(optionName, out Option? option))
			return false;
		return option.Children.Count == 0 ? option.Enabled : option.Children[0].Enabled;
	}

	public static void OnTick()
	{
		SnowWorld.FallSnow();
	}

	public static void OnDawn()
	{
		Entropy.StartEntropy();
		if (OptionsContains("Santa") &&
		    Main.hardMode && Main.invasionType == 0 && ((!NPC.downedFrost && Main.rand.NextBool(20)) ||
		                                                (NPC.downedFrost && Main.rand.NextBool(60))))
		{
			Main.invasionDelay = 0;
			Main.StartInvasion(InvasionID.SnowLegion);
			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.InvasionProgressReport, -1, -1, null, 0, 1f, Main.invasionType + 3);
		}


		if (Main.drunkWorld ^ OptionsContains("Drunk.Crimruption") && Main.netMode != NetmodeID.MultiplayerClient)
		{
			WorldGen.crimson = !WorldGen.crimson;
			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.WorldData);
		}
	}

	public static void OnDusk()
	{
		Entropy.StartEntropy();
		if (OptionsContains("Santa") && NPC.downedFishron &&
		    ((!NPC.downedChristmasIceQueen && Main.rand.NextBool(20)) ||
		     (NPC.downedChristmasIceQueen && Main.rand.NextBool(60))))
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				Main.NewText(Language.GetTextValue("LegacyMisc.34"), 50, 255, 130);
				Main.startSnowMoon();
			}
			else
			{
				ChatHelper.BroadcastChatMessage(NetworkText.FromKey("LegacyMisc.34"),
					new Color(50, 255, 130));

				Main.startSnowMoon();
				NetMessage.SendData(MessageID.WorldData);
				NetMessage.SendData(MessageID.InvasionProgressReport, -1, -1, null, 0, 1f, 1f, 1f);
			}
		}
	}

	public static void HandlePacket(BinaryReader reader)
	{
		PacketId packetId = (PacketId)reader.ReadByte();
		switch (packetId)
		{
			case PacketId.SantaWaterFreezing:
				int x = reader.Read();
				int y = reader.Read();
				Tile tile = Main.tile[x, y];
				tile.LiquidAmount = 0;
				tile.HasTile = true;
				tile.TileType = TileID.BreakableIce;
				break;
			case PacketId.EntropyHappening:
				new Entropy(500, reader).TreatTiles();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(packetId), packetId, null);
		}
	}
}