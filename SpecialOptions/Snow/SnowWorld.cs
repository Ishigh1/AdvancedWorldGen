using System;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.Helper;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ID.ProjectileID;
using static Terraria.ID.TileID;
using OnMain = On.Terraria.Main;

namespace AdvancedWorldGen.SpecialOptions.Snow;

public class SnowWorld
{
	public OptionHelper OptionHelper;

	public SnowWorld(OptionHelper optionHelper)
	{
		OptionHelper = optionHelper;
	}

	public static void FallSnow()
	{
		if (!API.OptionsContains("Santa")) return;
		if (Main.rand.Next((int)(10000 / (Main.maxRaining + .01))) >=
		    Main.maxTilesX) return;
		int x = Main.rand.Next(Main.maxTilesX);
		int y = Main.rand.Next((int)Main.worldSurface);
		Tile tile = Main.tile[x, y];
		if (tile.IsActive || tile.wall != 0) return;
		if (tile.LiquidAmount > 0)
		{
			if (tile.LiquidType != LiquidID.Water) return;
			tile.LiquidAmount = 0;
			tile.IsActive = true;
			tile.type = BreakableIce;

			if (Main.netMode == NetmodeID.Server)
			{
				ModPacket modPacket = AdvancedWorldGenMod.Instance.GetPacket();
				modPacket.Write((byte)PacketId.SantaWaterFreezing);
				modPacket.Write(x);
				modPacket.Write(y);
				modPacket.Send();
			}

			return;
		}

		float speedX = Main.rand.Next(-100, 101);
		float speedY = Main.rand.Next(200) + 100;
		float modifier = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
		modifier = 12 / modifier;
		speedX *= modifier;
		speedY *= modifier;
		ComputeSnowBall(out double damages, out double knockback);

		int projectile = Projectile.NewProjectile(Projectile.GetNoneSource(), x * 16 + 8, y * 16 + 8,
			speedX, speedY, SnowBallHostile, (int)damages, (float)knockback, Main.myPlayer);
		Main.projectile[projectile].friendly = true;
		Main.projectile[projectile].netUpdate = true;
	}

	public static void ComputeSnowBall(out double damages, out double knockback)
	{
		damages = 10 * DifficultyHelper.GetDamageModifier();
		knockback = 4.5;
		if (NPC.downedBoss1)
		{
			damages *= 2;
			knockback *= 2;
		}

		if (Main.hardMode)
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

	public static void MainOnCheckXMas(OnMain.orig_checkXMas orig)
	{
		if (API.OptionsContains("Santa"))
			Main.xMas = true;
		else
			orig();
	}

	/*
	 * after IL_2356e, && !ModifiedWorld.CurrentCustomSeededWorld.OptionHelper.OptionsContains("Santa")
	 */
	public static void RemoveSnowDropDuringChristmas(ILContext il)
	{
		ILCursor cursor = new(il);
		for (int i = 0; i < 4; i++)
			cursor.GotoNext(instruction => instruction.MatchLdcI4(109));

		cursor.GotoNext(MoveType.After, instruction => instruction.OpCode == OpCodes.Brfalse_S);
		object label = cursor.Prev.Operand;

		cursor.OptionContains("Santa");
		cursor.Emit(OpCodes.Brtrue_S, label);
	}
}