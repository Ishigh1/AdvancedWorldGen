using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using On.Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ID.ProjectileID;
using static Terraria.ID.TileID;
using NPC = Terraria.NPC;
using Projectile = Terraria.Projectile;
using Tile = Terraria.Tile;

namespace AdvancedWorldGen.SpecialOptions
{
	public class SnowWorld
	{
		public OptionHelper OptionHelper;

		public SnowWorld(OptionHelper optionHelper)
		{
			OptionHelper = optionHelper;
		}

		public void FallSnow()
		{
			if (!OptionHelper.OptionsContains("Santa")) return;
			if (Terraria.Main.rand.Next((int) (10000 / (Terraria.Main.maxRaining + .01))) >=
			    Terraria.Main.maxTilesX) return;
			int x = Terraria.Main.rand.Next(Terraria.Main.maxTilesX);
			int y = Terraria.Main.rand.Next((int) Terraria.Main.worldSurface);
			Tile tile = Terraria.Main.tile[x, y];
			if (tile.IsActive || tile.wall != 0) return;
			if (tile.LiquidAmount > 0)
			{
				if (tile.LiquidType != LiquidID.Water) return;
				tile.LiquidAmount = 0;
				tile.IsActive = true;
				tile.type = BreakableIce;

				if (Terraria.Main.netMode == NetmodeID.Server)
				{
					ModPacket modPacket = OptionHelper.AdvancedWorldGen.GetPacket();
					modPacket.Write((byte) ServerChangeId.Freezing);
					modPacket.Write(x);
					modPacket.Write(y);
					modPacket.Send();
				}

				return;
			}

			float speedX = Terraria.Main.rand.Next(-100, 101);
			float speedY = Terraria.Main.rand.Next(200) + 100;
			float modifier = (float) Math.Sqrt(speedX * speedX + speedY * speedY);
			modifier = 12 / modifier;
			speedX *= modifier;
			speedY *= modifier;
			ComputeSnowBall(out double damages, out double knockback);

			int projectile = Projectile.NewProjectile(x * 16 + 8, y * 16 + 8, speedX, speedY,
				SnowBallHostile, (int) damages,
				(float) knockback, Terraria.Main.myPlayer);
			Terraria.Main.projectile[projectile].friendly = true;
			Terraria.Main.projectile[projectile].netUpdate = true;
		}

		public static void ComputeSnowBall(out double damages, out double knockback)
		{
			damages = 10;
			knockback = 4.5;
			damages *= Terraria.Main.GameModeInfo.EnemyDamageMultiplier +
			           (Terraria.Main.getGoodWorld ? 1 : 0);
			if (NPC.downedBoss1)
			{
				damages *= 2;
				knockback *= 2;
			}

			if (Terraria.Main.hardMode)
			{
				damages *= 2;
				knockback *= 2;
			}

			if (NPC.downedFrost)
			{
				damages *= 3;
				knockback *= 3;
			}

			if (NPC.downedPlantBoss)
			{
				damages *= 2;
				knockback *= 2;
			}

			if (NPC.downedChristmasIceQueen)
			{
				damages *= 5;
				knockback *= 5;
			}
		}

		public static void MainOncheckXMas(Main.orig_checkXMas orig)
		{
			if (ModifiedWorld.OptionsContains("Santa"))
				Terraria.Main.xMas = true;
			else
				orig();
		}

		/*
		 * after IL_2356e, && !ModifiedWorld.CurrentCustomSeededWorld.OptionHelper.OptionsContains("Santa")
		 */
		public static void RemoveSnowDropDuringChristmas(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			for (int i = 0; i < 4; i++)
				if (!cursor.TryGotoNext(instruction => instruction.MatchLdcI4(109)))
					return; // Instruction not found

			cursor.Index += 2;
			object label = cursor.Prev.Operand;

			ILHelper.OptionContains(cursor, "Santa");
			cursor.Emit(OpCodes.Brtrue, label);
		}
	}
}