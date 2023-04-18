namespace AdvancedWorldGen.SpecialOptions._100k_special.Entities;

public class BoC : GlobalNPC
{
	private static int Previous;

	public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
	{
		return entity.type is NPCID.BrainofCthulhu or NPCID.Creeper;
	}

	public override void SetDefaults(NPC entity)
	{
		if (_100kWorld.Enabled)
		{
			entity.aiStyle = 6;
		}
	}

	public override bool PreAI(NPC npc)
	{
		if (!_100kWorld.Enabled)
			return true;
		if (npc.ai[0] == 0)
		{
			if (npc.type == NPCID.BrainofCthulhu)
			{
				npc.ai[2] = NPC.GetBrainOfCthuluCreepersCount();
				npc.ai[0] = NPC.NewNPC(new EntitySource_BossSpawn(npc), (int)(npc.position.X + npc.width / 2f),
					(int)(npc.position.Y + npc.height), NPCID.Creeper, npc.whoAmI);
				Main.npc[(int)npc.ai[0]].ai[1] = npc.whoAmI;
				Main.npc[(int)npc.ai[0]].ai[2] = npc.ai[2] - 1f;
				npc.netUpdate = true;
			}
			else if (npc.ai[2] >= 0)
			{
				npc.ai[0] = NPC.NewNPC(new EntitySource_BossSpawn(npc), (int)(npc.position.X + npc.width / 2f),
					(int)(npc.position.Y + npc.height), NPCID.Creeper, npc.whoAmI);
				Main.npc[(int)npc.ai[0]].ai[1] = npc.whoAmI;
				Main.npc[(int)npc.ai[0]].ai[2] = npc.ai[2] - 1f;
				npc.netUpdate = true;
			}
		}

		if (npc.type == NPCID.BrainofCthulhu)
		{
			if (!Main.npc[(int)npc.ai[0]].active)
			{
				npc.life = 0;
				npc.checkDead();
				npc.active = false;
			}
		}
		else if (npc.ai[2] < 0)
		{
			if (!Main.npc[(int)npc.ai[1]].active)
			{
				npc.life = 0;
				npc.checkDead();
				npc.active = false;
			}
		}
		else
		{
			if (!Main.npc[(int)npc.ai[0]].active)
			{
				npc.ai[2] = -1;
			}
			else if (!Main.npc[(int)npc.ai[1]].active)
			{
				float follower = npc.ai[0];
				npc.SetDefaults(NPCID.BrainofCthulhu);
				npc.ai[0] = follower;
			}
		}

		return true;
	}
}