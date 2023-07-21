namespace AdvancedWorldGen.SpecialOptions._100kSpecial.Entities.EoW;

public class EoWCreepers : GlobalNPC
{
	public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
	{
		return entity.type is NPCID.EaterofWorldsHead or NPCID.EaterofWorldsBody or NPCID.EaterofWorldsTail;
	}

	public override void SetDefaults(NPC entity)
	{
		if (_100kWorld.Enabled)
		{
			entity.aiStyle = NPCAIStyleID.Creeper;
		}
	}

	public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (!_100kWorld.Enabled)
			return true;
		Texture2D value7 = TextureAssets.Npc[npc.type].Value;
		Vector2 position2 = npc.Center - Main.screenPosition;
		Color npcColor = Lighting.GetColor((int)(npc.position.X + npc.width * 0.5) / 16,
			(int)((npc.position.Y + npc.height * 0.5) / 16.0));
		if (npc.behindTiles)
		{
			int num29 = (int)((npc.position.X - 8f) / 16f);
			int num30 = (int)((npc.position.X + npc.width + 8f) / 16f);
			int num31 = (int)((npc.position.Y - 8f) / 16f);
			int num32 = (int)((npc.position.Y + npc.height + 8f) / 16f);
			for (int l = num29; l <= num30; l++)
			{
				for (int m = num31; m <= num32; m++)
				{
					if (Lighting.Brightness(l, m) == 0f)
						npcColor = Color.Black;
				}
			}
		}

		spriteBatch.Draw(value7, position2, npcColor);
		return false;
	}

	// Healthbar is buggy rn but it's not that important
}