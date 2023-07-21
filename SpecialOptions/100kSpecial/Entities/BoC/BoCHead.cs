namespace AdvancedWorldGen.SpecialOptions._100kSpecial.Entities.BoC;

public class BoCHead : GlobalNPC
{

	public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
	{
		return entity.type is NPCID.BrainofCthulhu;
	}

	public override void SetDefaults(NPC entity)
	{
		if (_100kWorld.Enabled)
		{
			entity.aiStyle = NPCAIStyleID.Worm;
		}
	}

	public override bool PreAI(NPC npc)
	{
		if (!_100kWorld.Enabled)
			return true;
		if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[0] == 0)
		{
			npc.ai[2] = GetBrainOfCthuluCreepersCount();
			npc.ai[0] = NewNPC(new EntitySource_BossSpawn(npc), (int)(npc.position.X + npc.width / 2f),
				(int)(npc.position.Y + npc.height), NPCID.Creeper, npc.whoAmI);
			Main.npc[(int)npc.ai[0]].ai[1] = npc.whoAmI;
			Main.npc[(int)npc.ai[0]].ai[2] = npc.ai[2] - 1f;
			npc.netUpdate = true;
		}

		if (!Main.npc[(int)npc.ai[0]].active)
		{
			npc.life = 0;
			npc.checkDead();
			npc.active = false;
		}

		return true;
	}
}