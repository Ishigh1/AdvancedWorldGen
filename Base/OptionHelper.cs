using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdvancedWorldGen.CustomSized;
using AdvancedWorldGen.SpecialOptions;
using AdvancedWorldGen.SpecialOptions.Snow;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AdvancedWorldGen.Base;

public class OptionHelper
{
	public Dictionary<string, Option> OptionDict = null!;
	public SnowWorld SnowWorld;
	public WorldSettings WorldSettings;

	public OptionHelper(Mod mod)
	{
		WorldSettings = new WorldSettings();
		SnowWorld = new SnowWorld(this);
		InitializeDict(mod);
	}

	public void InitializeDict(Mod mod)
	{
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

	public void ClearAll()
	{
		foreach ((string? _, Option? option) in OptionDict) option.Disable();
	}

	public void Import(ICollection<string> optionNames)
	{
		ClearAll();
		Legacy.ReplaceOldOptions(optionNames);
		foreach (string optionName in optionNames)
			if (OptionDict.TryGetValue(optionName, out Option? option))
				option.WeakEnable();
	}

	public void Export(ICollection<string> collection)
	{
		foreach ((string? _, Option? option) in OptionDict)
			if(option.Enabled && option.Children.Count == 0)
				collection.Add(option.FullName);
	}
	
	public bool OptionsContains(params string[] optionNames)
	{
		return optionNames.Any(optionName =>
		{
			Option option = OptionDict[optionName];
			return option.Children.Count == 0 ? option.Enabled : option.Children[0].Enabled;
		});
	}

	public void OnTick()
	{
		SnowWorld.FallSnow();
	}

	public void OnDawn()
	{
		Entropy.StartEntropy();
		if (OptionsContains("Santa") &&
		    Main.hardMode && Main.invasionType == 0 && (!NPC.downedFrost && Main.rand.NextBool(20) ||
		                                                NPC.downedFrost && Main.rand.NextBool(60)))
		{
			Main.invasionDelay = 0;
			Main.StartInvasion(InvasionID.SnowLegion);
			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.InvasionProgressReport, -1, -1, null, 0, 1f, Main.invasionType + 3);
		}

		if (OptionsContains("Drunk.Crimruption") && !WorldGen.drunkWorldGen) WorldGen.crimson = !WorldGen.crimson;
	}

	public void OnDusk()
	{
		Entropy.StartEntropy();
		if (OptionsContains("Santa") && NPC.downedFishron &&
		    (!NPC.downedChristmasIceQueen && Main.rand.NextBool(20) ||
		     NPC.downedChristmasIceQueen && Main.rand.NextBool(60)))
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
				tile.IsActive = true;
				tile.type = TileID.BreakableIce;
				break;
			case PacketId.EntropyHappening:
				new Entropy(500, reader).TreatTiles();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(packetId));
		}
	}
}