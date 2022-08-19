using static Terraria.ID.NPCID;

namespace AdvancedWorldGen.SpecialOptions.Halloween;

public static class HalloweenCommon
{
	public static void Setup()
	{
		OnMain.checkHalloween += MainOnCheckHalloween;
		OnNPC.SetDefaults += HalloweenSwap;
		OnPlayer.KillMe += SpawnGhostOnPlayerDeath;
		OnNPC.DoesntDespawnToInactivity += NoGhostDespawn;
		ILPlayer.UpdateGraveyard += PermanentGraveyard;
	}

	public static void MainOnCheckHalloween(OnMain.orig_checkHalloween orig)
	{
		if (OptionHelper.OptionsContains("Spooky"))
			Main.halloween = true;
		else
			orig();
	}

	public static void HalloweenSwap(OnNPC.orig_SetDefaults orig, NPC self, int type,
		NPCSpawnParams spawnParams)
	{
		orig(self, type, spawnParams);
		if (!OptionHelper.OptionsContains("Spooky")) return;
		switch (self.aiStyle)
		{
			case 1 when Main.hardMode && self.type != HoppinJack:
				orig(self, HoppinJack, spawnParams);
				break;
			case 24:
				orig(self, Raven, spawnParams);
				break;
		}

		self.netUpdate = true;
	}

	public static void SpawnGhostOnPlayerDeath(OnPlayer.orig_KillMe orig, Player self,
		PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp)
	{
		orig(self, damageSource, dmg, hitDirection, pvp);
		if (Main.netMode == NetmodeID.MultiplayerClient ||
		    !OptionHelper.OptionsContains("Spooky")) return;
		NPC npc = Main.npc[NPC.NewNPC(Entity.GetSource_NaturalSpawn(), 0, 0, Ghost)];
		npc.position.X = self.position.X;
		npc.position.Y = self.position.Y;
		npc.netUpdate = true;
	}

	public static bool NoGhostDespawn(OnNPC.orig_DoesntDespawnToInactivity orig, NPC self)
	{
		if (self.type == Ghost && OptionHelper.OptionsContains("Spooky")) return true;
		return orig(self);
	}

	public static void PermanentGraveyard(ILContext il)
	{
		ILCursor cursor = new(il);
		cursor.GotoNext(MoveType.After, instruction => instruction.MatchStloc(0));

		cursor.OptionContains("Spooky");
		ILLabel label = cursor.DefineLabel();

		cursor.Emit(OpCodes.Brfalse_S, label);
		cursor.Emit(OpCodes.Ldc_R4, 1f);
		cursor.Emit(OpCodes.Stloc_0);

		cursor.MarkLabel(label);
	}

	public static void InsertTasks(List<GenPass> tasks, ref int passIndex)
	{
		if (!OptionHelper.OptionsContains("Spooky"))
			return;
		tasks.Insert(++passIndex, new PassLegacy("Graveyards", Graveyards.GenerateStructures));
		tasks.Insert(++passIndex, new PassLegacy("HalloweenTraps", Traps.PlaceTraps));
	}
}