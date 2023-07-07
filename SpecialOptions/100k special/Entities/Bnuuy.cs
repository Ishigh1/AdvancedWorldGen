using Terraria.ID;

namespace AdvancedWorldGen.SpecialOptions._100k_special.Entities;

public class Bnuuy : GlobalNPC
{
	public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
	{
		return NPCID.Search.TryGetName(entity.type, out string name) && name.Contains("Bunny");
	}

	public override void SetDefaults(NPC entity)
	{
		if (_100kWorld.Enabled)
		{
			entity.SpawnedFromStatue = true;
			if (entity.damage == 0)
				entity.GivenName = "Baba";
			else
				entity.GivenName = "Baba òxó";
		}
	}

	public override bool CanBeHitByNPC(NPC npc, NPC aggressor)
	{
		return !_100kWorld.Enabled;
	}

	public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
	{
		return !_100kWorld.Enabled || projectile.hostile;
	}

	public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
	{
		if (!_100kWorld.Enabled) return;
		if (!npc.lavaWet)
		{
			if (npc.type != NPCID.ExplosiveBunny)
			{
				int npcIndex = NPC.NewNPC(new EntitySource_Death(npc), (int)npc.Center.X, (int)npc.Center.Y,
					NPCID.GiantTortoise);
				Main.npc[npcIndex].SpawnedFromStatue = true;
			}
			else
			{
				NPC.SpawnBoss((int)npc.Center.X, (int)npc.Center.Y, NPCID.MoonLordCore, npc.target);
			}
		}

		npc.active = false;
		if (Main.netMode == NetmodeID.Server)
		{
			npc.netSkip = -1;
			npc.life = 0;
			NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
			ModPacket? modPacket = AdvancedWorldGenMod.Instance.GetPacket();
			modPacket.Write(npc.Center.X);
			modPacket.Write(npc.Center.Y);
		}
		else
		{
			Puff(npc.Center);
		}
	}

	public static void Puff(Vector2 center)
	{
		for (int num674 = 0; num674 < 50; num674++)
		{
			double angle = Main.rand.NextDouble() * Math.Tau;
			Dust.NewDust(center, 18, 20, DustID.Smoke, 2.5f * (float)Math.Sin(angle), (float)(2.5f * Math.Cos(angle)));
		}
	}
}