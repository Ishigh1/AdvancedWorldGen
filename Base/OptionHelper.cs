using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvancedWorldGen.CustomSized;
using AdvancedWorldGen.SpecialOptions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AdvancedWorldGen.Base
{
	public class OptionHelper
	{
		public AdvancedWorldGen AdvancedWorldGen;
		public HashSet<string> Options;
		public SnowWorld SnowWorld;
		public WorldSettings WorldSettings;

		public OptionHelper()
		{
			Options = new HashSet<string>();
			WorldSettings = new WorldSettings();
			SnowWorld = new SnowWorld(this);
			AdvancedWorldGen = (AdvancedWorldGen) ModLoader.GetMod("AdvancedWorldGen");
		}

		public bool OptionsContains(params string[] value)
		{
			return value.Any(s => Options.Contains(s));
		}

		public void OnTick()
		{
			SnowWorld.FallSnow();
		}

		public void OnDawn()
		{
			Entropy.StartEntropy(this);
			if (OptionsContains("Santa") &&
			    Main.hardMode && Main.invasionType == 0 && (!NPC.downedFrost && Main.rand.Next(20) == 0 ||
			                                                NPC.downedFrost && Main.rand.Next(60) == 0))
			{
				Main.invasionDelay = 0;
				Main.StartInvasion(InvasionID.SnowLegion);
				if (Main.netMode == NetmodeID.Server)
					NetMessage.SendData(MessageID.InvasionProgressReport, -1, -1, null, 0, 1f, Main.invasionType + 3);
			}

			if (OptionsContains("Crimruption") && !WorldGen.drunkWorldGen) WorldGen.crimson = !WorldGen.crimson;
		}

		public void OnDusk()
		{
			Entropy.StartEntropy(this);
			if (OptionsContains("Santa") && NPC.downedFishron &&
			    (!NPC.downedChristmasIceQueen && Main.rand.Next(20) == 0 ||
			     NPC.downedChristmasIceQueen && Main.rand.Next(60) == 0))
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
			PacketId packetId = (PacketId) reader.ReadByte();
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
}