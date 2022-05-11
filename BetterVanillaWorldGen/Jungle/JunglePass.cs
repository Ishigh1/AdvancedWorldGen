using System;
using AdvancedWorldGen.Helper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using VanillaJunglePass = Terraria.GameContent.Biomes.JunglePass;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Jungle;

public class JunglePass : ControlledWorldGenPass
{
	public int DungeonSide;
	public int JungleOriginX;
	public int LeftBeachEnd;
	public int RightBeachStart;
	public float WorldScale;

	public JunglePass() : base("Jungle", 4304.6303f)
	{
	}

	protected override void ApplyPass()
	{
		Progress.Message = Language.GetTextValue("LegacyWorldGen.11");

		JungleOriginX = VanillaInterface.JungleOriginX.Value;
		DungeonSide = VanillaInterface.DungeonSide.Value;
		LeftBeachEnd = VanillaInterface.LeftBeachEnd.Value;
		RightBeachStart = VanillaInterface.RightBeachStart.Value;

		WorldScale = Main.maxTilesX * (1.5f / 4200f);
		(int x, int y) = CreateStartPoint();
		ApplyRandomMovement(ref x, ref y, 100, 100);
		Point mean = new(x, y);

		PlaceFirstPassMud(x, y, 3);
		Progress.Set(1, 11);

		PlaceGemsAt(x, y, 63, 2);
		Progress.Set(2, 11);

		ApplyRandomMovement(ref x, ref y, 250, 150);
		mean.X += x;
		mean.Y += y;

		PlaceFirstPassMud(x, y, 0);
		Progress.Set(3, 11);

		PlaceGemsAt(x, y, 65, 2);
		Progress.Set(4, 11);

		int oldX = x;
		int oldY = y;
		ApplyRandomMovement(ref x, ref y, 400, 150);
		mean.X += x;
		mean.Y += y;

		PlaceFirstPassMud(x, y, -3);
		Progress.Set(5, 11);

		PlaceGemsAt(x, y, 67, 2);
		Progress.Set(6, 11);

		x = mean.X / 3;
		y = mean.Y / 3;
		int num = WorldGen.genRand.Next((int)(400f * WorldScale), (int)(600f * WorldScale));
		x = (int)Utils.Clamp(x, LeftBeachEnd + num / 2 + 25f * WorldScale,
			RightBeachStart - num / 2f - 25f * WorldScale);

		WorldGen.mudWall = true;
		WorldGen.TileRunner(x, y, num, 10000, 59, false, 0f, -20f, true);
		Progress.Set(7, 11);

        GenerateTunnelToSurface(x, y);
		Progress.Set(8, 11);

		WorldGen.mudWall = false;
		DelimitJungle();
		Progress.Set(9, 11);

		GenerateHolesInMudWalls();
		Progress.Set(10, 11);

		GenerateFinishingTouches(oldX, oldY);
		Progress.Set(11, 11);
	}

	public void PlaceGemsAt(int x, int y, ushort baseGem, int gemVariants)
	{
		for (int _ = 0; _ < 6f * WorldScale; _++)
		{
			int i = x + WorldGen.genRand.Next(-(int)(125f * WorldScale), (int)(125f * WorldScale));
			int j = y + WorldGen.genRand.Next(-(int)(125f * WorldScale), (int)(125f * WorldScale));
			int strength = WorldGen.genRand.Next(3, 7);
			int steps = WorldGen.genRand.Next(3, 8);
			int type = baseGem + WorldGen.genRand.Next(0, gemVariants);
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

	public (int x, int y) CreateStartPoint()
	{
		return (JungleOriginX, (int)(Main.maxTilesY + Main.rockLayer) / 2);
	}

	public void ApplyRandomMovement(ref int x, ref int y, int xRange, int yRange)
	{
		x += WorldGen.genRand.Next((int)(-xRange * WorldScale), 1 + (int)(xRange * WorldScale));
		y += WorldGen.genRand.Next((int)(-yRange * WorldScale), 1 + (int)(yRange * WorldScale));
		y = Utils.Clamp(y, (int)Main.rockLayer, Main.UnderworldLayer);
	}

	public static void GenerateTunnelToSurface(int x, int y)
	{
		double num = WorldGen.genRand.Next(5, 11);
		Vector2 vector = new(x, y);
		Vector2 vector2 = new(WorldGen.genRand.Next(-10, 11) * 0.1f,
			WorldGen.genRand.Next(10, 20) * 0.1f);
		int num2 = 0;
		while (true)
		{
			num += WorldGen.genRand.Next(-20, 21) * 0.1f;
			num = Utils.Clamp(num, 5, 10);

			int value3 = (int)(vector.X - num * 0.5);
			int value4 = (int)(vector.X + num * 0.5);
			int value5 = (int)(vector.Y - num * 0.5);
			int value6 = (int)(vector.Y + num * 0.5);
			value3 = Utils.Clamp(value3, 10, Main.maxTilesX - 10);
			value4 = Utils.Clamp(value4, 10, Main.maxTilesX - 10);
			value5 = Utils.Clamp(value5, 10, Main.maxTilesY - 10);
			value6 = Utils.Clamp(value6, 10, Main.maxTilesY - 10);
			for (int k = value3; k < value4; k++)
			for (int l = value5; l < value6; l++)
				if (Math.Abs(k - vector.X) + Math.Abs(l - vector.Y) <
				    num * 0.5 * (1.0 + WorldGen.genRand.Next(-10, 11) * 0.015))
					WorldGen.KillTile(k, l);

			num2++;
			if (num2 > 10 && WorldGen.genRand.Next(50) < num2)
			{
				num2 = 0;
				int num4 = WorldGen.genRand.NextBool(2) ? 2 : -2;

				WorldGen.TileRunner((int)vector.X, (int)vector.Y, WorldGen.genRand.Next(3, 20),
					WorldGen.genRand.Next(10, 100), -1,
					false, num4);
			}

			vector += vector2;
			vector2.Y += WorldGen.genRand.Next(-10, 11) * 0.01f;
			vector2.Y = Utils.Clamp(vector2.Y, -2, 0);

			vector2.X += WorldGen.genRand.Next(-10, 11) * 0.1f;
			if (vector.X < x - 200)
				vector2.X += WorldGen.genRand.Next(5, 21) * 0.1f;

			if (vector.X > x + 200)
				vector2.X -= WorldGen.genRand.Next(5, 21) * 0.1f;

			vector2.X = Utils.Clamp(vector2.X, -1.5f, 1.5f);

			int x1 = Utils.Clamp((int)vector.X, 10, Main.maxTilesX - 10);
			int y1 = Utils.Clamp((int)vector.Y, 10, Main.maxTilesY - 10);
			if (y1 < Main.worldSurface)
			{
				if (WorldGen.drunkWorldGen)
					return;


				if (Main.tile[x1, y1].WallType == 0 && !Main.tile[x1, y1].HasTile &&
				    Main.tile[x1, y1 - 3].WallType == 0 && !Main.tile[x1, y1 - 3].HasTile &&
				    Main.tile[x1, y1 - 1].WallType == 0 && !Main.tile[x1, y1 - 1].HasTile &&
				    Main.tile[x1, y1 - 4].WallType == 0 && !Main.tile[x1, y1 - 4].HasTile &&
				    Main.tile[x1, y1 - 2].WallType == 0 && !Main.tile[x1, y1 - 2].HasTile &&
				    Main.tile[x1, y1 - 5].WallType == 0 && !Main.tile[x1, y1 - 5].HasTile)
					return;
			}
		}
	}

	public void GenerateHolesInMudWalls()
	{
		int minX = VanillaInterface.JungleMinX;
		int maxX = VanillaInterface.JungleMaxX;
		for (int i = 0; i < Main.maxTilesX / 4; i++)
		{
			int x = WorldGen.genRand.Next(minX, maxX);
			int y = WorldGen.genRand.Next((int)Main.worldSurface + 10, Main.UnderworldLayer);
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

	public void GenerateFinishingTouches(int middleX, int middleY)
	{
		int x = middleX;
		int y = middleY;
		float worldScale = WorldScale;

		float iterations = 20f * worldScale;
		for (int i = 0; i <= iterations; i++)
		{
			Progress.Set(i, iterations, 1 / 11f / 3f, 10 / 11f);
			x += WorldGen.genRand.Next((int)(-5f * worldScale), (int)(6f * worldScale));
			y += WorldGen.genRand.Next((int)(-5f * worldScale), (int)(6f * worldScale));
			WorldGen.TileRunner(x, y, WorldGen.genRand.Next(40, 100), WorldGen.genRand.Next(300, 500), 59);
		}

		iterations = 10f * worldScale;
		int xMin = Math.Max(1, (int)(middleX - 600f * worldScale));
		int xMax = Math.Min(Main.maxTilesX - 1, (int)(middleX + 600f * worldScale));
		int yMin = Math.Max(1, (int)(middleY - 200f * worldScale));
		int yMax = Math.Min(Main.maxTilesY - 1, (int)(middleY + 200f * worldScale));
		for (int i = 0; i <= iterations; i++)
		{
			Progress.Set(i, iterations, 1 / 11f / 3f, 10 / 11f + 1 / 11f / 3f);

			do
			{
				x = WorldGen.genRand.Next(xMin, xMax);
				y = WorldGen.genRand.Next(yMin, yMax);
			} while (Main.tile[x, y].TileType != TileID.Mud);

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

		iterations = 300f * worldScale;
		for (int i = 0; i <= iterations; i++)
		{
			Progress.Set(i, iterations, 1 / 11f / 3f, 10 / 11f + 2 / 11f / 3f);

			do
			{
				x = WorldGen.genRand.Next(xMin, xMax);
				y = WorldGen.genRand.Next(yMin, yMax);
			} while (Main.tile[x, y].HasTile && Main.tile[x, y].TileType != TileID.Mud);

			WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 30), 1);
			if (WorldGen.genRand.NextBool(4))
			{
				int type = WorldGen.genRand.Next(TileID.Sapphire, TileID.Diamond + 1);
				WorldGen.TileRunner(x + WorldGen.genRand.Next(-1, 2), y + WorldGen.genRand.Next(-1, 2),
					WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(4, 8), type);
			}
		}
	}
}