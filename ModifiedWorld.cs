using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvancedWorldGen.SeedUI;
using MonoMod.Cil;
using On.Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using static Terraria.ID.NPCID;
using NPC = Terraria.NPC;
using Utils = Terraria.Utils;
using WorldGen = Terraria.WorldGen;

namespace AdvancedWorldGen
{
	public class ModifiedWorld : ModSystem
	{
		public static OptionHelper OptionHelper;

		public override void OnModLoad()
		{
			OptionHelper = new OptionHelper();
		}

		public override void Unload()
		{
			OptionHelper = null;
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
				Terraria.Main.checkXMas();
			}
		}

		public override TagCompound SaveWorldData()
		{
			return new TagCompound
			{
				{"Options", OptionHelper.Options.ToList()}
			};
		}

		public override void NetReceive(BinaryReader reader)
		{
			List<int> list = new List<int>();
			OptionHelper.Options.Clear();

			int id;
			while ((id = reader.ReadInt16()) != 0) list.Add(id);

			foreach (KeyValuePair<string, Option> keyValuePair in OptionsSelector.OptionDict.Where(keyValuePair =>
				list.Remove(keyValuePair.Value.Id)))
			{
				OptionHelper.Options.Add(keyValuePair.Key);
				if (list.Count == 0) break;
			}

			Terraria.Main.checkXMas();
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
			Terraria.Main.getGoodWorld = OptionsContains("ForTheWorthy");
			WorldGen.getGoodWorldGen = Terraria.Main.getGoodWorld;
			Terraria.Main.drunkWorld = OptionsContains("Drunk");
			WorldGen.drunkWorldGen = Terraria.Main.drunkWorld;
			WorldGen.drunkWorldGenText = Terraria.Main.drunkWorld;
			if (!Terraria.Main.dayTime) Terraria.Main.time = 0;
		}

		//Deletes all the now-useless stuff about special seeds
		public static void OverrideWorldOptions(ILContext il)
		{
			ILCursor ilCursor = new ILCursor(il);
			if (ilCursor.TryGotoNext(instruction => instruction.MatchLdcI4(0)))
				for (int i = 0; i < 44; i++)
					ilCursor.Remove();
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			int npcIndex = tasks.FindIndex(pass => pass.Name == "Guide");
			if (npcIndex != -1)
			{
				tasks.RemoveAt(npcIndex);
				tasks.Insert(npcIndex, new PassLegacy("NPCs", HandleNpcs));
			}

			tasks.Add(new PassLegacy("Tile Switch", ReplaceTiles));
		}

		public void HandleNpcs(GenerationProgress progress, GameConfiguration configuration)
		{
			int npcs = 0;
			if (OptionHelper.OptionsContains("Painted"))
			{
				AddNpc(Painter);
				npcs++;
			}

			if (WorldGen.notTheBees)
			{
				AddNpc(Merchant);
				npcs++;
			}

			if (WorldGen.getGoodWorldGen)
			{
				AddNpc(Demolitionist);
				npcs++;
			}

			if (WorldGen.drunkWorldGen)
			{
				AddNpc(PartyGirl);
				npcs++;
			}

			if (OptionHelper.OptionsContains("Santa"))
			{
				AddNpc(SantaClaus);
				npcs++;
			}

			if (OptionHelper.OptionsContains("Random"))
			{
				AddNpc(RandomNpc());
				npcs++;
			}

			if (npcs == 0) AddNpc(Guide);
		}

		public static void AddNpc(int npc)
		{
			int npcId = NPC.NewNPC(Terraria.Main.spawnTileX * 16, Terraria.Main.spawnTileY * 16, npc);
			Terraria.Main.npc[npcId].homeTileX = Terraria.Main.spawnTileX;
			Terraria.Main.npc[npcId].homeTileY = Terraria.Main.spawnTileY;
			Terraria.Main.npc[npcId].direction = Terraria.Main.rand.Next(2) == 0 ? 1 : -1;
			Terraria.Main.npc[npcId].homeless = true;
		}

		public int RandomNpc()
		{
			List<int> npcs = new List<int>
			{
				Nurse, ArmsDealer, Dryad, Guide, Clothier, GoblinTinkerer, Wizard, Mechanic, Truffle, Steampunker,
				DyeTrader, Cyborg, WitchDoctor, Pirate, Stylist, Angler, TaxCollector, DD2Bartender, Golfer,
				BestiaryGirl, Princess
			};
			if (!WorldGen.notTheBees) npcs.Add(Merchant);

			if (!WorldGen.getGoodWorldGen) npcs.Add(Demolitionist);

			if (!WorldGen.drunkWorldGen) npcs.Add(PartyGirl);

			if (!OptionHelper.OptionsContains("Painted")) npcs.Add(Painter);

			return Utils.SelectRandom(Terraria.Main.rand, npcs.ToArray());
		}

		public void ReplaceTiles(GenerationProgress progress, GameConfiguration configuration)
		{
			if (OptionHelper.OptionsContains("Santa"))
				TileReplacer.Snow.ReplaceTiles(progress, "Making the world colder");

			if (OptionHelper.OptionsContains("Random", "Painted")) TileReplacer.RandomizeWorld(progress, OptionHelper);
		}

		public override void PostUpdateTime()
		{
			if (Terraria.Main.netMode != NetmodeID.MultiplayerClient) OptionHelper.OnTick();
		}

		public static void OnDawn(Main.orig_UpdateTime_StartDay orig, ref bool stopEvents)
		{
			orig(ref stopEvents);
			OptionHelper.OnDawn();
		}

		public static void OnDusk(Main.orig_UpdateTime_StartNight orig, ref bool stopEvents)
		{
			orig(ref stopEvents);
			OptionHelper.OnDusk();
		}

		public static bool OptionsContains(params string[] s)
		{
			return OptionHelper.OptionsContains(s);
		}
	}
}