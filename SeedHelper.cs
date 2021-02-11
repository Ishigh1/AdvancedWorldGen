using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvancedWorldGen.SpecialSeeds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AdvancedWorldGen
{
	public class SeedHelper
	{
		public AdvancedWorldGen AdvancedWorldGen;
		public List<string> Options;
		public SnowWorld SnowWorld;

		public SeedHelper(List<string> options)
		{
			Options = options;
			WorldGen.notTheBees = OptionsContains("NotTheBees", "SmallNotTheBees");
			WorldGen.getGoodWorldGen = OptionsContains("ForTheWorthy");

			SnowWorld = new SnowWorld(this);
		}

		public bool OptionsContains(params string[] value)
		{
			return Options != null && value.Any(s => Options.Contains(s));
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
					Main.NewText(Language.GetText("LegacyMisc.34").Value, 50, 255, 130);
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

		public static void HandlePacket(BinaryReader reader, int whoAmI)
		{
			ServerChangeId serverChangeId = (ServerChangeId) reader.ReadByte();
			switch (serverChangeId)
			{
				case ServerChangeId.Freezing:
					int x = reader.Read();
					int y = reader.Read();
					Tile tile = Main.tile[x, y];
					tile.LiquidAmount = 0;
					tile.IsActive = true;
					tile.type = TileID.BreakableIce;
					break;
				case ServerChangeId.Entropy:
					new Entropy(500, reader).TreatTiles();
					break;
			}
		}
	}
}