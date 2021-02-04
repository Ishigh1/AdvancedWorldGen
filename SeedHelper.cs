using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvancedSeedGen.SpecialSeeds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AdvancedSeedGen
{
	public class SeedHelper
	{
		public static Dictionary<string, List<string>> SeedTranslator;
		public AdvancedSeedGen AdvancedSeedGen;
		public List<string> Options;
		public SnowWorld SnowWorld;

		public SeedHelper(List<string> options)
		{
			Options = options;
			AdvancedSeedGen = (AdvancedSeedGen) ModLoader.GetMod("AdvancedSeedGen");
			SnowWorld = new SnowWorld(this);
			AdvancedSeedGen.SeedHelper = this;
		}

		public SeedHelper(string seedText) : this(ExtractOptions(seedText))
		{
		}

		public bool OptionsContains(params string[] value)
		{
			return Options != null && value.Any(s => Options.Contains(s));
		}

		public static List<string> ExtractOptions(string seed)
		{
			List<string> list = new List<string>();
			if (seed == null) return list;
			string[] strings = seed.Split(':');

			strings = strings[0].Split(',');

			foreach (string s in strings)
				if (SeedTranslator.TryGetValue(s.ToLower(), out List<string> collection))
					list.AddRange(collection);

			return list;
		}

		public static string TweakSeedText(string seedText)
		{
			SeedHelper seedHelper = new SeedHelper(seedText);
			string[] strings = seedText.Split(':');
			if (seedHelper.Options.Count != 0)
				if (strings.Length != 2)
				{
					UnifiedRandom rand = new UnifiedRandom();
					int seed = rand.Next(999999999);
					return seedText + ":" + seed;
				}

			return seedText;
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
					NetMessage.BroadcastChatMessage(NetworkText.FromKey("LegacyMisc.34"),
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
					tile.liquid = 0;
					tile.active(true);
					tile.type = TileID.BreakableIce;
					break;
				case ServerChangeId.Entropy:
					new Entropy(500, reader).TreatTiles();
					break;
			}
		}

		public static List<string> GetWorldOptions()
		{
			List<string> options = new List<string>();
			foreach (string s in SeedTranslator.Values.SelectMany(seedTranslatorValue =>
				seedTranslatorValue.Where(s => !options.Contains(s))))
				options.Add(s);

			return options;
		}
	}
}