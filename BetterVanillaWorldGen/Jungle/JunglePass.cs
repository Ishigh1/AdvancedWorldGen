using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.WorldBuilding;
using VanillaJunglePass = Terraria.GameContent.Biomes.JunglePass;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Jungle;

public class JunglePass : ControlledWorldGenPass
{
	public int DungeonSide;
	public int JungleOriginX;
	public int LeftBeachEnd;
	public int RightBeachStart;
	public float WorldScale;
	public double WorldSurface;

	public JunglePass() : base("Jungle", 4304.6303f)
	{
	}

	protected override void ApplyPass()
	{
		Progress.Message = Language.GetTextValue("LegacyWorldGen.11");

		JungleOriginX = VanillaInterface.JungleOriginX.Value;
		DungeonSide = VanillaInterface.DungeonSide.Value;
		WorldSurface = WorldGen.worldSurface;
		LeftBeachEnd = VanillaInterface.LeftBeachEnd.Value;
		RightBeachStart = VanillaInterface.RightBeachStart.Value;

		WorldScale = Main.maxTilesX / (4200 / 1.5f);
		Point point = CreateStartPoint();
		int x = point.X;
		int y = point.Y;
		Point zero = Point.Zero;
		ApplyRandomMovement(ref x, ref y, 100, 100);
		zero.X += x;
		zero.Y += y;
		PlaceFirstPassMud(x, y, 3);
		PlaceGemsAt(x, y, 63, 2);
		Progress.Set(0.15f);
		ApplyRandomMovement(ref x, ref y, 250, 150);
		zero.X += x;
		zero.Y += y;
		PlaceFirstPassMud(x, y, 0);
		PlaceGemsAt(x, y, 65, 2);
		Progress.Set(0.3f);
		int oldX = x;
		int oldY = y;
		ApplyRandomMovement(ref x, ref y, 400, 150);
		zero.X += x;
		zero.Y += y;
		PlaceFirstPassMud(x, y, -3);
		PlaceGemsAt(x, y, 67, 2);
		Progress.Set(0.45f);
		x = zero.X / 3;
		y = zero.Y / 3;
		int num = WorldGen.genRand.Next((int)(400f * WorldScale), (int)(600f * WorldScale));
		int num2 = (int)(25f * WorldScale);
		x = Utils.Clamp(x, LeftBeachEnd + num / 2 + num2, RightBeachStart - num / 2 - num2);
		WorldGen.mudWall = true;
		WorldGen.TileRunner(x, y, num, 10000, 59, false, 0f, -20f, true);
		GenerateTunnelToSurface(x, y);
		WorldGen.mudWall = false;
		DelimitJungle();
		Progress.Set(0.6f);
		GenerateHolesInMudWalls();
		GenerateFinishingTouches(Progress, oldX, oldY);
	}

	public void PlaceGemsAt(int x, int y, ushort baseGem, int gemVariants)
	{
		for (int _ = 0; _ < 6f * WorldScale; _++)
		{
			int i = x + WorldGen.genRand.Next(-(int)(125f * WorldScale), (int)(125f * WorldScale));
			int j = y + WorldGen.genRand.Next(-(int)(125f * WorldScale), (int)(125f * WorldScale));
			int strength = WorldGen.genRand.Next(3, 7);
			int steps = WorldGen.genRand.Next(3, 8);
			int type = WorldGen.genRand.Next(baseGem, baseGem + gemVariants);
			WorldGen.TileRunner(i, j, strength, steps, type);
		}
	}

	public void PlaceFirstPassMud(int x, int y, int xSpeedScale)
	{
		WorldGen.mudWall = true;
		WorldGen.TileRunner(x, y, WorldGen.genRand.Next((int)(250f * WorldScale), (int)(500f * WorldScale)),
			WorldGen.genRand.Next(50, 150), 59, false, DungeonSide * xSpeedScale);
		WorldGen.mudWall = false;
	}

	public Point CreateStartPoint()
	{
		return new Point(JungleOriginX, (int)(Main.maxTilesY + Main.rockLayer) / 2);
	}

	public void ApplyRandomMovement(ref int x, ref int y, int xRange, int yRange)
	{
		x += WorldGen.genRand.Next((int)(-xRange * WorldScale), 1 + (int)(xRange * WorldScale));
		y += WorldGen.genRand.Next((int)(-yRange * WorldScale), 1 + (int)(yRange * WorldScale));
		y = Utils.Clamp(y, (int)Main.rockLayer, Main.UnderworldLayer);
	}

	public void GenerateTunnelToSurface(int x, int y)
	{
		double num = WorldGen.genRand.Next(5, 11);
		Vector2 vector = new()
		{
			X = x,
			Y = y
		};
		Vector2 vector2 = new()
		{
			X = WorldGen.genRand.Next(-10, 11) * 0.1f,
			Y = WorldGen.genRand.Next(10, 20) * 0.1f
		};
		int num2 = 0;
		bool flag = true;
		while (flag)
		{
			if (vector.Y < Main.worldSurface)
			{
				if (WorldGen.drunkWorldGen)
					flag = false;

				int value = (int)vector.X;
				int value2 = (int)vector.Y;
				value = Utils.Clamp(value, 10, Main.maxTilesX - 10);
				value2 = Utils.Clamp(value2, 10, Main.maxTilesY - 10);
				if (value2 < 5)
					value2 = 5;

				if (Main.tile[value, value2].WallType == 0 && !Main.tile[value, value2].HasTile &&
				    Main.tile[value, value2 - 3].WallType == 0 && !Main.tile[value, value2 - 3].HasTile &&
				    Main.tile[value, value2 - 1].WallType == 0 && !Main.tile[value, value2 - 1].HasTile &&
				    Main.tile[value, value2 - 4].WallType == 0 && !Main.tile[value, value2 - 4].HasTile &&
				    Main.tile[value, value2 - 2].WallType == 0 && !Main.tile[value, value2 - 2].HasTile &&
				    Main.tile[value, value2 - 5].WallType == 0 && !Main.tile[value, value2 - 5].HasTile)
					flag = false;
			}

			num += WorldGen.genRand.Next(-20, 21) * 0.1f;
			if (num < 5.0)
				num = 5.0;

			if (num > 10.0)
				num = 10.0;

			int value3 = (int)(vector.X - num * 0.5);
			int value4 = (int)(vector.X + num * 0.5);
			int value5 = (int)(vector.Y - num * 0.5);
			int value6 = (int)(vector.Y + num * 0.5);
			int num3 = Utils.Clamp(value3, 10, Main.maxTilesX - 10);
			value4 = Utils.Clamp(value4, 10, Main.maxTilesX - 10);
			value5 = Utils.Clamp(value5, 10, Main.maxTilesY - 10);
			value6 = Utils.Clamp(value6, 10, Main.maxTilesY - 10);
			for (int k = num3; k < value4; k++)
			for (int l = value5; l < value6; l++)
				if (Math.Abs(k - vector.X) + Math.Abs(l - vector.Y) <
				    num * 0.5 * (1.0 + WorldGen.genRand.Next(-10, 11) * 0.015))
					WorldGen.KillTile(k, l);

			num2++;
			if (num2 > 10 && WorldGen.genRand.Next(50) < num2)
			{
				num2 = 0;
				int num4 = -2;
				if (WorldGen.genRand.NextBool(2))
					num4 = 2;

				WorldGen.TileRunner((int)vector.X, (int)vector.Y, WorldGen.genRand.Next(3, 20),
					WorldGen.genRand.Next(10, 100), -1,
					false, num4);
			}

			vector += vector2;
			vector2.Y += WorldGen.genRand.Next(-10, 11) * 0.01f;
			if (vector2.Y > 0f)
				vector2.Y = 0f;

			if (vector2.Y < -2f)
				vector2.Y = -2f;

			vector2.X += WorldGen.genRand.Next(-10, 11) * 0.1f;
			if (vector.X < x - 200)
				vector2.X += WorldGen.genRand.Next(5, 21) * 0.1f;

			if (vector.X > x + 200)
				vector2.X -= WorldGen.genRand.Next(5, 21) * 0.1f;

			if (vector2.X > 1.5)
				vector2.X = 1.5f;

			if (vector2.X < -1.5)
				vector2.X = -1.5f;
		}
	}

	public void GenerateHolesInMudWalls()
	{
		int minX = VanillaInterface.JungleMinX;
		int maxX = VanillaInterface.JungleMaxX;
		for (int i = 0; i < Main.maxTilesX / 4; i++)
		{
			int x = WorldGen.genRand.Next(minX, maxX);
			int y = WorldGen.genRand.Next((int)WorldSurface + 10, Main.UnderworldLayer);
			if (Main.tile[x, y].WallType is WallID.JungleUnsafe or WallID.MudUnsafe)
				WorldGen.MudWallRunner(x, y);
		}
	}

	public void DelimitJungle()
	{
		int y = (int)((WorldGen.rockLayer + Main.UnderworldLayer) / 2);
		int importantX = JungleOriginX;
		int currentX = JungleOriginX;
		Tile tile1;
		Tile tile2;
		Tile tile3;
		do
		{
			tile1 = Main.tile[currentX, y];
			tile2 = Main.tile[currentX, y - 10];
			tile3 = Main.tile[currentX, y + 10];
			if (tile1.HasTile || tile2.HasTile || tile3.HasTile) importantX = currentX;
			currentX--;
		} while (!tile1.HasTile || tile1.TileType is TileID.Mud ||
		         !tile2.HasTile || tile2.TileType is TileID.Mud ||
		         !tile3.HasTile || tile3.TileType is TileID.Mud);

		VanillaInterface.JungleMinX = importantX + 1;

		currentX = JungleOriginX;
		do
		{
			tile1 = Main.tile[currentX, y];
			tile2 = Main.tile[currentX, y - 10];
			tile3 = Main.tile[currentX, y + 10];
			if (tile1.HasTile || tile2.HasTile || tile3.HasTile) importantX = currentX;
			currentX++;
		} while (!tile1.HasTile || tile1.TileType is TileID.Mud ||
		         !tile2.HasTile || tile2.TileType is TileID.Mud ||
		         !tile3.HasTile || tile3.TileType is TileID.Mud);

		VanillaInterface.JungleMaxX = importantX - 1;
	}

	public void GenerateFinishingTouches(GenerationProgress progress, int oldX, int oldY)
	{
		int x = oldX;
		int y = oldY;
		float worldScale = WorldScale;
		for (int i = 0; i <= 20f * worldScale; i++)
		{
			progress.Set((60f + i / worldScale) * 0.01f);
			x += WorldGen.genRand.Next((int)(-5f * worldScale), (int)(6f * worldScale));
			y += WorldGen.genRand.Next((int)(-5f * worldScale), (int)(6f * worldScale));
			WorldGen.TileRunner(x, y, WorldGen.genRand.Next(40, 100), WorldGen.genRand.Next(300, 500), 59);
		}

		for (int j = 0; j <= 10f * worldScale; j++)
		{
			progress.Set((80f + j / worldScale * 2f) * 0.01f);
			x = oldX + WorldGen.genRand.Next((int)(-600f * worldScale), (int)(600f * worldScale));
			y = oldY + WorldGen.genRand.Next((int)(-200f * worldScale), (int)(200f * worldScale));
			while (x < 1 || x >= Main.maxTilesX - 1 || y < 1 || y >= Main.maxTilesY - 1 ||
			       Main.tile[x, y].TileType != 59)
			{
				x = oldX + WorldGen.genRand.Next((int)(-600f * worldScale), (int)(600f * worldScale));
				y = oldY + WorldGen.genRand.Next((int)(-200f * worldScale), (int)(200f * worldScale));
			}

			for (int k = 0; k < 8f * worldScale; k++)
			{
				x += WorldGen.genRand.Next(-30, 31);
				y += WorldGen.genRand.Next(-30, 31);
				int type = -1;
				if (WorldGen.genRand.NextBool(7))
					type = -2;

				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(30, 70), type);
			}
		}

		for (int _ = 0; _ <= 300f * worldScale; _++)
		{
			x = oldX + WorldGen.genRand.Next((int)(-600f * worldScale), (int)(600f * worldScale));
			y = oldY + WorldGen.genRand.Next((int)(-200f * worldScale), (int)(200f * worldScale));
			while (x < 1 || x >= Main.maxTilesX - 1 || y < 1 || y >= Main.maxTilesY - 1 ||
			       Main.tile[x, y].TileType != 59)
			{
				x = oldX + WorldGen.genRand.Next((int)(-600f * worldScale), (int)(600f * worldScale));
				y = oldY + WorldGen.genRand.Next((int)(-200f * worldScale), (int)(200f * worldScale));
			}

			WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 30), 1);
			if (WorldGen.genRand.NextBool(4))
			{
				int type = WorldGen.genRand.Next(TileID.Sapphire, TileID.JungleThorns);
				WorldGen.TileRunner(x + WorldGen.genRand.Next(-1, 2), y + WorldGen.genRand.Next(-1, 2),
					WorldGen.genRand.Next(3, 7),
					WorldGen.genRand.Next(4, 8), type);
			}
		}
	}
}