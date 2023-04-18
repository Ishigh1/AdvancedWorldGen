namespace AdvancedWorldGen.SpecialOptions._100k_special.Entities;

public class EyeOfCthulhu : GlobalNPC
{
	public static bool CurrentlyFighting;
	
	public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
	{
		return entity.type == NPCID.EyeofCthulhu;
	}

	public override bool InstancePerEntity => true;

	public override void OnSpawn(NPC npc, IEntitySource source)
	{
	}
}