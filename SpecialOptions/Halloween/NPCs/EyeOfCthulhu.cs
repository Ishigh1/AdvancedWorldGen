using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace AdvancedWorldGen.SpecialOptions.Halloween.NPCs
{
	public static class EyeOfCthulhu
	{
		public const int DashServerInitAI = -126;
		public const int DashInitAI = -127;
		public const int DashAI = -128;
		public const int RushDuration = 80;

		public static DifficultyValue<int> GhostDuration =
			new(RushDuration, RushDuration, RushDuration * 2);

		public static DifficultyValue<int> EyeGhostChances = new(600, 550, 500);
		public static DifficultyValue<int> EyeSpeed = new(10, 15, 18);

		public static bool PreAI(NPC npc)
		{
			if (npc.type == NPCID.EyeofCthulhu)
				switch (npc.ai[3])
				{
					case DashServerInitAI:
						npc.ai[3] = DashInitAI;
						return false;
					case DashInitAI:
						EyeRushPlayer(npc);
						npc.ai[3] = DashAI;
						return false;
					case DashAI:
					{
						npc.alpha = 20;
						if (--npc.ai[2] < 0f)
							npc.active = false;
						else if (npc.ai[2] == RushDuration) EyeRushPlayer(npc);

						return false;
					}
					default:
						return true;
				}

			return true;
		}

		public static void AI(NPC npc)
		{
			if (npc.type == NPCID.EyeofCthulhu && Main.netMode != NetmodeID.MultiplayerClient &&
			    Main.rand.Next(EyeGhostChances.GetCurrentValue()) == 0)
			{
				npc.FindClosestPlayer(out float distanceToPlayer);
				if (distanceToPlayer <= 300f) return;
				NPC newNPC =
					Main.npc[
						NPC.NewNPC(0, 0, NPCID.EyeofCthulhu, ai0: 3f, ai1: 4f, ai2: GhostDuration.GetCurrentValue(),
							ai3: Main.netMode == NetmodeID.Server ? DashServerInitAI : DashInitAI)];
				newNPC.position.X = npc.position.X;
				newNPC.position.Y = npc.position.Y;
				newNPC.netUpdate = true;
			}
		}

		public static void EyeRushPlayer(NPC npc)
		{
			npc.TargetClosest();
			float x = npc.position.X + npc.width / 2f - Main.player[npc.target].position.X -
			          Main.player[npc.target].width / 2f;
			float y = npc.position.Y + npc.height - 59f - Main.player[npc.target].position.Y -
			          Main.player[npc.target].height / 2f;
			float rotation = (float) Math.Atan2(x, -y);

			if (rotation < MathHelper.Pi)
				rotation += MathHelper.TwoPi;
			else if (rotation > MathHelper.Pi)
				rotation -= MathHelper.TwoPi;

			SoundEngine.PlaySound(SoundID.ForceRoar, (int) npc.position.X, (int) npc.position.Y, -1);
			npc.rotation = rotation;
			npc.velocity = (rotation + MathHelper.PiOver2).ToRotationVector2() * EyeSpeed.GetCurrentValue();
			npc.dontTakeDamage = true;
			npc.SpawnedFromStatue = true;
			npc.dontCountMe = true;
			npc.boss = false;
			npc.netUpdate = true;
		}

		public static void BossHeadSlot(NPC npc, ref int index)
		{
			if (npc.type == NPCID.EyeofCthulhu && npc.ai[3] == -128f) index = -1;
		}
	}
}