using System.Collections.Generic;
using AdvancedWorldGen.Base;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using OnMain = On.Terraria.Main;

namespace AdvancedWorldGen.CustomSized.Secret;

public class Special : ModSystem
{
	public static bool TempleWorld;

	public override void Load()
	{
		OnMain.UpdateTime_SpawnTownNPCs += HandleNotHousableWorlds;
	}

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
	{
		TempleWorld = float.IsInfinity(ModifiedWorld.Instance.OptionHelper.WorldSettings.Params.TempleMultiplier);
		if (TempleWorld)
		{
			tasks.RemoveAll(pass =>
				pass.Name is not ("Reset" or "Smooth World" or "NPCs" or "Lihzahrd Altars" or "Final Cleanup"));
			tasks.Insert(1, new TempleWorld());
		}
	}

	public override void ModifyHardmodeTasks(List<GenPass> tasks)
	{
		if (TempleWorld) tasks.RemoveAll(pass => pass.Name is not "Hardmode Announcement");
	}

	public override void OnWorldUnload()
	{
		TempleWorld = false;
	}

	public override void SaveWorldData(TagCompound tag)
	{
		if (TempleWorld)
			tag["TempleWorld"] = true;
	}

	public override void LoadWorldData(TagCompound tag)
	{
		TempleWorld = tag.ContainsKey("TempleWorld");
	}

	public static void HandleNotHousableWorlds(OnMain.orig_UpdateTime_SpawnTownNPCs orig)
	{
		if (!TempleWorld)
		{
			orig();
		}
		else
		{
			WorldGen.prioritizedTownNPCType = 0;
			orig();
			if (WorldGen.prioritizedTownNPCType != 0)
			{
				int id = NPC.NewNPC(NPC.GetSpawnSourceForTownSpawn(), Main.spawnTileX * 16, Main.spawnTileY * 16,
					WorldGen.prioritizedTownNPCType);
				Main.npc[id].netUpdate = true;
				string fullName = Main.npc[id].FullName;
				switch (Main.netMode)
				{
					case NetmodeID.SinglePlayer:
						Main.NewText(Language.GetTextValue("Announcement.HasArrived", fullName), 50, 125);
						break;
					case NetmodeID.Server:
						ChatHelper.BroadcastChatMessage(
							NetworkText.FromKey("Announcement.HasArrived", Main.npc[id].GetFullNetName()),
							new Color(50, 125, 255));
						break;
				}

				WorldGen.prioritizedTownNPCType = 0;
			}
		}
	}
}