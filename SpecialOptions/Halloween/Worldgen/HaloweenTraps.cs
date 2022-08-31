using static Terraria.ID.TileID;

namespace AdvancedWorldGen.SpecialOptions.Halloween.Worldgen;

public class HaloweenTraps : GenPass
{
	public HaloweenTraps() : base("HalloweenTraps", 100f)
	{
	}

	public const int Left = -1;
	public const int Right = 1;

	protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
	{
		if (!OptionHelper.OptionsContains("Spooky")) return;
		progress.Message = "Placing Halloween traps";
		PlaceSpawnTrap(Left, Main.spawnTileX - 20 - Main.rand.Next(100));
		PlaceSpawnTrap(Right, Main.spawnTileX + 20 + Main.rand.Next(100));
		progress.Value = 0.2f;
		PlaceSurfaceTraps();
	}

	public static void PlaceSpawnTrap(int direction, int x)
	{
		int y;
		(x, y) = Utilities.FindSuitableSpotForSurfacePressurePlate(direction, x);
		int explosiveX = x + 11 * direction;

		int explosiveY = y + Main.rand.Next(2, 5);
		if (!Utilities.IsValidTop(explosiveX, explosiveY - 1) ||
		    !Utilities.IsSuitableForTraps(explosiveX, explosiveY + 1))
		{
			PlaceSpawnTrap(direction, x + 10 * direction);
			return;
		}

		WorldGen.KillTile(x, y);
		WorldGen.PlaceTile(x, y, PressurePlates, plr: -1, style: 3);
		WorldGen.KillTile(explosiveX, explosiveY);
		WorldGen.PlaceTile(explosiveX, explosiveY, Explosives, plr: -1);
		Tile tile = Main.tile[explosiveX, explosiveY];
		tile.TileColor = PaintID.ShadowPaint;
		WorldUtils.WireLine(new Point(x, y), new Point(explosiveX, explosiveY));
	}

	public static void PlaceSurfaceTraps()
	{
		int numTraps = (int)(Main.rand.Next(10, 21) * Utilities.WorldSize);
		for (int i = 0; i < numTraps; i++)
		{
			int x = Main.rand.Next(WorldGen.beachDistance, Main.maxTilesX * 2 / 5);
			if (Main.rand.NextBool(2)) x = Main.maxTilesX - x;
			switch (Main.rand.Next(4))
			{
				case 0:
				case 1:
					PlaceCloudTrap(x, (x1, y1) => WorldGen.PlaceTile(x1, y1, Boulder));
					break;
				case 2:
					PlaceCloudTrap(x, (x1, y1) =>
					{
						Tile tile = Main.tile[x1, y1];
						tile.LiquidAmount = byte.MaxValue;
						tile.LiquidType = LiquidID.Lava;
						Tile tile1 = Main.tile[x1, y1 - 1];
						tile1.LiquidAmount = byte.MaxValue;
						tile1.LiquidType = LiquidID.Lava;
						Tile tile2 = Main.tile[x1 - 1, y1];
						tile2.LiquidAmount = byte.MaxValue;
						tile2.LiquidType = LiquidID.Lava;
						Tile tile3 = Main.tile[x1 - 1, y1 - 1];
						tile3.LiquidAmount = byte.MaxValue;
						tile3.LiquidType = LiquidID.Lava;
					});
					break;
				case 3:
					PlaceGeyserTrap(x);
					break;
			}
		}
	}

	public static void PlaceCloudTrap(int x, Action<int, int> trap)
	{
		int y;
		int modifier = Main.rand.Next(2);
		int direction = modifier * 2 - 1;
		(x, y) = Utilities.FindSuitableSpotForSurfacePressurePlate(direction, x);

		int cloudDistanceY = Main.rand.Next(100, 150);
		if (!Utilities.IsValidStructure(x + modifier - 1, y - cloudDistanceY, 2, cloudDistanceY,
			    10)) return;

		WorldGen.KillTile(x, y);
		WorldGen.PlaceTile(x, y, PressurePlates, plr: -1, style: 7);
		if (y - cloudDistanceY <= 40)
		{
			WorldUtils.WireLine(new Point(x, y), new Point(x, y - cloudDistanceY));
			Tile tile = Main.tile[x, 40];
			tile.HasActuator = true;
			Tile tile1 = Main.tile[x + direction, 40];
			tile1.RedWire = true;
			tile1.HasActuator = true;
			WorldGen.PlaceTile(x, 40, TileID.Cloud);
			WorldGen.PlaceTile(x + direction, 40, TileID.Cloud);
			WorldGen.PlaceTile(x + 2 * direction, 39, TileID.Cloud);
			WorldGen.PlaceTile(x - direction, 39, TileID.Cloud);
			WorldGen.PlaceTile(x + 2 * direction, 38, TileID.Cloud);
			WorldGen.PlaceTile(x - direction, 38, TileID.Cloud);
			trap(x - modifier, 39);
		}
		else
		{
			bool enlarging = true;
			int width = 4;
			int cloudX = x - 1;
			int depth = 0;
			if (direction == -1) cloudX--;
			while (true)
			{
				//Randomize next cloud line
				int cloudY = y - cloudDistanceY - depth;
				if (enlarging)
				{
					if (Main.rand.Next(depth * depth) >= 9)
					{
						enlarging = false;
						WorldGen.KillTile(x, cloudY + 1);
						WorldGen.KillTile(x + direction, cloudY + 1);
						trap(x + modifier, cloudY + 1);
						WorldGen.PlaceWall(x, cloudY + 1, WallID.Cloud);
						WorldGen.PlaceWall(x + direction, cloudY + 1, WallID.Cloud);
						WorldGen.PlaceWall(x, cloudY, WallID.Cloud);
						WorldGen.PlaceWall(x + direction, cloudY, WallID.Cloud);

						WorldUtils.WireLine(new Point(x, y), new Point(x, y - cloudDistanceY));
						for (int i = 0; i < depth - 1; i++)
						{
							Tile tile = Main.tile[x, y - cloudDistanceY - i];
							tile.RedWire = true;
							tile.HasActuator = true;
							Tile tile1 = Main.tile[x + direction, y - cloudDistanceY - i];
							tile1.RedWire = true;
							tile1.HasActuator = true;
						}
					}
					else
					{
						int change = Main.rand.Next(6);
						width += change;
						cloudX -= Main.rand.Next(change);
					}
				}
				else
				{
					int change = Main.rand.Next(2, Math.Max(width / 2, 6));
					width -= change;
					cloudX += Main.rand.Next(change);
				}

				if (width <= 0) break;

				//Place cloud blocks
				PlaceCloudBlocks(width, cloudX, cloudY, enlarging);

				depth++;
			}
		}
	}

	public static void PlaceCloudBlocks(int width, int cloudX, int cloudY, bool enlarging)
	{
		for (int i = 0; i < width; i++)
		{
			int tempX = cloudX + i;
			Tile tile = Main.tile[tempX, cloudY];
			if (tile.HasTile || tile.LiquidAmount != 0) continue;
			WorldGen.PlaceTile(tempX, cloudY, TileID.Cloud);
			Tile tile1 = Main.tile[tempX, cloudY + 1];
			if (width <= 2)
			{
				tile.IsHalfBlock = true;
				tile1.Slope = SlopeType.Solid;
			}
			else if (i == 0)
			{
				if (enlarging)
				{
					if (!tile1.HasTile)
						tile.Slope = SlopeType.Solid;
				}
				else
				{
					tile.Slope = SlopeType.SlopeDownRight;
					tile1.Slope = SlopeType.Solid;
				}
			}
			else if (i == width - 1)
			{
				if (enlarging)
				{
					if (!tile1.HasTile)
						tile.Slope = SlopeType.SlopeUpLeft;
				}
				else
				{
					tile.Slope = SlopeType.SlopeDownLeft;
					tile1.Slope = SlopeType.Solid;
				}
			}
		}
	}

	public static void PlaceGeyserTrap(int x)
	{
		int y;
		int direction = Main.rand.Next(2) * 2 - 1;
		(x, y) = Utilities.FindSuitableSpotForSurfacePressurePlate(direction, x);

		int space = Main.rand.Next(3, 6);
		int geyserX = x - space;
		int geyser2X = x + space;
		int geyserY = y + Main.rand.Next(3, 6);
		if (!Utilities.IsValidTop(geyserX, geyserY - 1) || !Utilities.IsValidTop(geyserX - 1, geyserY - 1) ||
		    !Utilities.IsValidTop(geyser2X, geyserY - 1) || !Utilities.IsValidTop(geyser2X + 1, geyserY - 1) ||
		    !Utilities.IsSuitableForTraps(geyserX, geyserY + 1) ||
		    !Utilities.IsSuitableForTraps(geyserX - 1, geyserY + 1) ||
		    !Utilities.IsSuitableForTraps(geyser2X, geyserY + 1) ||
		    !Utilities.IsSuitableForTraps(geyser2X + 1, geyserY + 1))
		{
			PlaceGeyserTrap(x + direction);
			return;
		}

		WorldGen.KillTile(x, y);
		WorldGen.PlaceTile(x, y, PressurePlates, plr: -1, style: 3);

		WorldGen.KillTile(geyserX, geyserY);
		WorldGen.KillTile(geyserX - 1, geyserY);
		WorldGen.KillTile(geyser2X, geyserY);
		WorldGen.KillTile(geyser2X + 1, geyserY);
		WorldGen.Place2x1(geyserX - 1, geyserY, GeyserTrap, Main.rand.Next(2));
		WorldGen.Place2x1(geyser2X, geyserY, GeyserTrap, Main.rand.Next(2));
		WorldUtils.WireLine(new Point(x, y), new Point(geyserX, geyserY));
		WorldUtils.WireLine(new Point(x, y), new Point(geyser2X, geyserY));
	}
}