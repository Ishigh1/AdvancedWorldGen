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

namespace AdvancedSeedGen
{
	public class SnowWorld
	{
		public SeedHelper SeedHelper;

		public SnowWorld(SeedHelper seedHelper)
		{
			SeedHelper = seedHelper;
		}

		public void FallSnow()
		{
			if (!SeedHelper.OptionsContains("Santa")) return;
			if (Terraria.Main.rand.Next((int) (10000 / (Terraria.Main.maxRaining + .1))) >=
			    Terraria.Main.maxTilesX) return;
			int x = Terraria.Main.rand.Next(Terraria.Main.maxTilesX);
			int y = Terraria.Main.rand.Next((int) Terraria.Main.worldSurface);
			Tile tile = Terraria.Main.tile[x, y];
			if (tile.active() || tile.wall != 0) return;
			if (tile.liquid > 0)
			{
				if (tile.liquidType() != 0) return;
				tile.liquid = 0;
				tile.active(true);
				tile.type = BreakableIce;

				if (Terraria.Main.netMode == NetmodeID.Server)
				{
					ModPacket modPacket = SeedHelper.AdvancedSeedGen.GetPacket();
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
			double damages = 10;
			double knockback = 4.5;
			damages *= Terraria.Main.ActiveWorldFileData.IsExpertMode ? Terraria.Main.expertDamage : 1;
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

			int projectile = Projectile.NewProjectile(x * 16 + 8, y * 16 + 8, speedX, speedY,
				SnowBallHostile, (int) damages,
				(float) knockback, Terraria.Main.myPlayer);
			Terraria.Main.projectile[projectile].friendly = true;
			Terraria.Main.projectile[projectile].netUpdate = true;
		}

		public static void MainOncheckXMas(Main.orig_checkXMas orig)
		{
			if (CustomSeededWorld.OptionsContains("Santa"))
				Terraria.Main.xMas = true;
			else
				orig();
		}

		/*
		 * after IL_2356e, && !CustomSeededWorld.CurrentCustomSeededWorld.SeedHelper.OptionsContains("Santa")
		 */
		public static void RemoveSnowDropDuringChristmas(ILContext il)
		{
			ILCursor ilCursor = new ILCursor(il);
			for (int i = 0; i < 4; i++)
				if (!ilCursor.TryGotoNext(instruction => instruction.MatchLdcI4(109)))
					return; // Instruction not found

			ilCursor.Index += 2;
			object label = ilCursor.Prev.Operand;
			
			ilCursor.Emit(OpCodes.Ldc_I4_1);
			ilCursor.Emit(OpCodes.Newarr, typeof(string));
			ilCursor.Emit(OpCodes.Dup);
			ilCursor.Emit(OpCodes.Ldc_I4_0);
			ilCursor.Emit(OpCodes.Ldstr, "Santa");
			ilCursor.Emit(OpCodes.Stelem_Ref);
			ilCursor.Emit(OpCodes.Call, typeof(CustomSeededWorld).GetMethod("OptionsContains"));
			ilCursor.Emit(OpCodes.Brtrue, label);
		}
	}
}