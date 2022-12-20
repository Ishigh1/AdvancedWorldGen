using static Terraria.ID.ProjectileID;
using static Terraria.ID.TileID;

namespace AdvancedWorldGen.SpecialOptions.Snow;

public class SnowWorld
{
	public static void FallSnow()
	{
		if (!OptionHelper.OptionsContains("Santa")) return;
		if (Main.rand.Next((int)(10000 / (Main.maxRaining + .01))) >= Main.maxTilesX)
			return;
		int x = Main.rand.Next(Main.maxTilesX);
		int y = Main.rand.Next((int)Main.worldSurface);
		Tile tile = Main.tile[x, y];
		if (tile.HasTile || tile.WallType != 0) return;
		if (tile.LiquidAmount > 0)
		{
			if (tile.LiquidType != LiquidID.Water) return;
			tile.LiquidAmount = 0;
			tile.HasTile = true;
			tile.TileType = BreakableIce;

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

		int projectile = Projectile.NewProjectile(Entity.GetSource_None(), x * 16 + 8, y * 16 + 8,
			speedX, speedY, SnowBallHostile, (int)damages, (float)knockback, Main.myPlayer);
		Main.projectile[projectile].friendly = true;
		Main.projectile[projectile].netUpdate = true;
	}

	private static void ComputeSnowBall(out double damages, out double knockback)
	{
		damages = 10;
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

	public static void MainOnCheckXMas(On_Main.orig_checkXMas orig)
	{
		if (OptionHelper.OptionsContains("Santa"))
			Main.xMas = true;
		else
			orig();
	}

	/*
	 * after IL_2356e, && !OptionHelper.OptionsContains("Santa")
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