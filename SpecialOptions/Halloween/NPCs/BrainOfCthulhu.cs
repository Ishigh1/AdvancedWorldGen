using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace AdvancedWorldGen.SpecialOptions.Halloween.NPCs
{
	public class BrainOfCthulhu
	{
		public const int CreeperServerAI = -126;
		public const int CreeperInitAI = -127;
		public const int CreeperAI = -128;
		public static DifficultyValue<int> NumCreepers = new(5, 10, 10);

		public static bool GhostActive;

		public static bool PreAI(NPC npc)
		{
			if (npc.type == NPCID.Creeper)
				switch (npc.ai[3])
				{
					case CreeperServerAI:
						npc.ai[3] = CreeperInitAI;
						break;
					case CreeperInitAI:
						npc.ai[3] = CreeperAI;

						if (Main.masterMode)
							npc.dontTakeDamage = true;
						else
							npc.immortal = true;

						npc.alpha = 120;
						break;
				}

			return true;
		}

		public static void PostAI(NPC npc)
		{
			if (npc.type == NPCID.BrainofCthulhu && Main.netMode != NetmodeID.MultiplayerClient)
				if (npc.ai[0] < 0 && !GhostActive)
				{
					GhostActive = true;
					for (int creepers = 0; creepers < NumCreepers.GetCurrentValue(); creepers++)
					{
						float x = npc.Center.X;
						float y = npc.Center.Y;
						x += Main.rand.Next(-npc.width, npc.width);
						y += Main.rand.Next(-npc.height, npc.height);
						int id = NPC.NewNPC((int) x, (int) y, 267,
							ai3: Main.netMode == NetmodeID.Server ? CreeperServerAI : CreeperInitAI);
						Main.npc[id].velocity = new Vector2(Main.rand.Next(-30, 31) * 0.1f,
							Main.rand.Next(-30, 31) * 0.1f);
						Main.npc[id].netUpdate = true;
					}
				}
		}

		public static void OnKill(NPC npc)
		{
			if (npc.type == NPCID.BrainofCthulhu) GhostActive = false;
		}
	}
}