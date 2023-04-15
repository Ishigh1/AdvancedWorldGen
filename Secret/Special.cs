namespace AdvancedWorldGen.Secret;

public class Special : ModSystem
{
	public static bool TempleWorld;
	public static string? SecretString;

	public override void Load()
	{
		On_Main.UpdateTime_SpawnTownNPCs += HandleNotHousableWorlds;
	}

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
	{
		TempleWorld = float.IsInfinity(Params.TempleMultiplier);
		if (TempleWorld)
		{
			tasks.RemoveAll(pass =>
				pass.Name is not ("Reset" or "Smooth World" or "NPCs" or "Lihzahrd Altars" or "Final Cleanup"));
			tasks.Insert(1, new TempleWorld());
		}

		if (SecretString == "Spookypizza")
		{
			totalWeight = 0;
			tasks.Clear();
			Main.spawnTileX = 10;
			Main.spawnTileY = 10;
		}
	}

	public override void ModifyHardmodeTasks(List<GenPass> tasks)
	{
		if (TempleWorld) tasks.RemoveAll(pass => pass.Name is not "Hardmode Announcement");
	}

	public override void OnWorldUnload()
	{
		TempleWorld = false;
		SecretString = null;
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

	private static void HandleNotHousableWorlds(On_Main.orig_UpdateTime_SpawnTownNPCs orig)
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
				int id = NPC.NewNPC(Entity.GetSource_TownSpawn(), Main.spawnTileX * 16, Main.spawnTileY * 16,
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