using System;
using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
using AdvancedWorldGen.Helper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Jungle;

public class JungleChests : ControlledWorldGenPass
{
	public JungleChests() : base("Jungle Chests", 195.6581f)
	{
	}

	protected override void ApplyPass()
	{
		int rand = WorldGen.genRand.Next(5);
		ushort tileType = rand switch
		{
			0 => TileID.IridescentBrick,
			1 => TileID.Mudstone,
			2 => TileID.RichMahogany,
			3 => TileID.TinBrick,
			_ => TileID.GoldBrick
		};
		ushort wallType = rand switch
		{
			0 => WallID.IridescentBrick,
			1 => WallID.MudstoneBrick,
			2 => WallID.RichMaogany,
			3 => WallID.TinBrick,
			_ => WallID.GoldBrick
		};

		float stepCount = WorldGen.genRand.Next(7 * Main.maxTilesX / 4200, 12 * Main.maxTilesX / 4200);
		for (int step = 0; step < stepCount; step++)
		{
			Progress.Set(step, stepCount);
			int minX = VanillaInterface.JungleMinX;
			int maxX = VanillaInterface.JungleMaxX;
			int x = WorldGen.genRand.Next(minX, maxX);

			int y;
			if (Main.maxTilesY - 400 - (Main.worldSurface + Main.rockLayer) / 2 > 100)
				y = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, Main.maxTilesY - 400);
			else if ((Main.worldSurface + Main.rockLayer) / 2 - Main.worldSurface > 100)
				y = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2 - 100, Main.maxTilesY - 400);
			else
				y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);


			(x, y) = TileFinder.SpiralSearch(x, y, IsValid);
			if ((x, y) is (-1, -1))
				throw new Exception("Jungle grass not found for jungle chest !");

			int width = WorldGen.genRand.Next(2, 4);
			int height = WorldGen.genRand.Next(2, 4);
			Rectangle area = new(x - width - 1, y - height - 1, width + 1, height + 1);

			for (int xx = x - width - 1; xx <= x + width + 1; xx++)
			for (int yy = y - height - 1; yy <= y + height + 1; yy++)
			{
				Tile tile = Main.tile[xx, yy];
				tile.HasTile = true;
				tile.TileType = tileType;
				tile.LiquidAmount = 0;
				tile.LiquidType = LiquidID.Water;
			}

			for (int xx = x - width; xx <= x + width; xx++)
			for (int yy = y - height; yy <= y + height; yy++)
			{
				Tile tile = Main.tile[xx, yy];
				tile.HasTile = false;
				tile.WallType = wallType;
			}

			bool torchPlaced = false;
			int tries = 0;
			while (!torchPlaced && tries < 100)
			{
				tries++;
				int xx = WorldGen.genRand.Next(x - width, x + width + 1);
				int yy = WorldGen.genRand.Next(y - height, y + height - 2);
				WorldGen.PlaceTile(xx, yy, TileID.Torches, true, false, -1, 3);
				if (TileID.Sets.Torch[Main.tile[xx, yy].TileType])
					torchPlaced = true;
			}

			for (int xx = x - width - 1; xx <= x + width + 1; xx++)
			for (int yy = y + height - 2; yy <= y + height; yy++)
			{
				Tile tile = Main.tile[xx, yy];
				tile.HasTile = false;
			}

			for (int xx = x - width - 1; xx <= x + width + 1; xx++)
			for (int yy = y + height - 2; yy <= y + height - 1; yy++)
			{
				Tile tile = Main.tile[xx, yy];
				tile.HasTile = false;
			}

			for (int xx = x - width - 1; xx <= x + width + 1; xx++)
			{
				int num556 = 4;
				int yy = y + height + 2;
				Tile tile = Main.tile[xx, yy];
				while (!tile.HasTile && yy < Main.maxTilesY && num556 > 0)
				{
					tile.HasTile = true;
					tile.TileType = 59;
					yy++;
					num556--;
					tile = Main.tile[xx, yy];
				}
			}

			width -= WorldGen.genRand.Next(1, 3);
			int j = y - height - 2;
			while (width > -1)
			{
				for (int i = x - width - 1; i <= x + width + 1; i++)
				{
					Tile tile = Main.tile[i, j];
					tile.HasTile = true;
					tile.TileType = tileType;
				}

				width -= WorldGen.genRand.Next(1, 3);
				j--;
			}


			int[] jChestX = VanillaInterface.JChestX.Value;
			int[] jChestY = VanillaInterface.JChestY.Value;
			int numJChests = VanillaInterface.NumJChests.Value;
			jChestX[numJChests] = x;
			jChestY[numJChests] = y;
			WorldGen.structures.AddProtectedStructure(area);
			VanillaInterface.NumJChests.Value = numJChests + 1;
		}

		Main.tileSolid[TileID.Traps] = false;
	}

	public static bool IsValid(int x, int y)
	{
		if (Main.tile[x, y].TileType == TileID.JungleGrass)
		{
			const int spread = 30;
			int xMin = Math.Max(x - spread, 10);
			int xMax = Math.Min(x + spread, Main.maxTilesX - 10);
			int yMin = Math.Max(y - spread, 10);
			int yMax = Math.Min(y + spread, Main.maxTilesY - 10);
			for (int x1 = xMin; x1 < xMax; x1 += 3)
			for (int y1 = yMin; y1 < yMax; y1 += 3)
			{
				if (Main.tile[x1, y1].HasTile && Main.tile[x1, y1].TileType is 225 or 229 or 226 or 119 or 120)
					return true;

				if (Main.tile[x1, y1].WallType is WallID.HiveUnsafe or WallID.LihzahrdBrickUnsafe)
					return true;
			}
		}

		return false;
	}
}