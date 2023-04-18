namespace AdvancedWorldGen.Base;

public partial class OptionHelper
{
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
			case PacketId.Puff:
				Bnuuy.Puff(new Vector2(reader.ReadSingle(), reader.ReadSingle()));
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(packetId), packetId, null);
		}
	}
}