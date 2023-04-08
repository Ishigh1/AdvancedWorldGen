namespace AdvancedWorldGen.BetterVanillaWorldGen.Traps;

public static class PlaceTraps
{
	public static bool ReplaceTraps(On_WorldGen.orig_placeTrap orig, int x, int y, int type)
	{
		if (Vector2D.Distance(new Vector2D(x, y), GenVars.shimmerPosition) < 100.0)
			return false;

		bool flag = false;
		bool flag2 = false;
		if (WorldGen.noTrapsWorldGen)
			Main.tileSolid[138] = false;

		while (!WorldGen.SolidTile(x, y))
		{
			y++;
			if (y > Main.maxTilesY - 10)
				return false;

			if (y >= Main.maxTilesY - 300)
				flag2 = true;
		}

		if (WorldGen.noTrapsWorldGen)
			Main.tileSolid[138] = true;

		y--;
		if (!WorldGen.noTrapsWorldGen && WorldGen.IsTileNearby(x, y, 70, 20))
			return false;

		if (Main.tile[x, y].WallType == 87)
			return false;

		if (Main.tile[x, y].LiquidAmount > 0 && Main.tile[x, y].LiquidType == LiquidID.Lava)
			flag = true;

		if (Main.remixWorld)
		{
			type = type switch
			{
				-1 when WorldGen.genRand.Next(20) == 0 => 2,
				-1 when y < Main.rockLayer - 30.0 && WorldGen.genRand.Next(3) != 0 => 3,
				-1 => WorldGen.genRand.Next(2),
				_ => type
			};
		}
		else
			type = type switch
			{
				-1 when WorldGen.genRand.Next(20) == 0 => 2,
				-1 when y > GenVars.lavaLine + 30 && WorldGen.genRand.Next(6) != 0 => 3,
				-1 => WorldGen.genRand.Next(2),
				_ => type
			};

		if (!WorldGen.InWorld(x, y, 3))
			return false;

		if (flag && type != 3)
			return false;

		if (flag2 && type != 3)
			return false;

		if (Main.tile[x, y].HasTile || Main.tile[x - 1, y].HasTile || Main.tile[x + 1, y].HasTile ||
		    Main.tile[x, y - 1].HasTile || Main.tile[x - 1, y - 1].HasTile ||
		    Main.tile[x + 1, y - 1].HasTile || Main.tile[x, y - 2].HasTile ||
		    Main.tile[x - 1, y - 2].HasTile || Main.tile[x + 1, y - 2].HasTile)
			return false;

		switch (Main.tile[x, y + 1].TileType)
		{
			case 48:
			case 232:
				return false;
		}

		if (type == 1)
		{
			for (int i = x - 3; i <= x + 3; i++)
			{
				for (int j = y - 3; j <= y + 3; j++)
				{
					if (Main.tile[i, j].TileType is 147 or 161)
						type = 0;
				}
			}
		}

		if (WorldGen.noTrapsWorldGen)
			Main.tileSolid[138] = false;

		switch (type)
		{
			case 0:
			{
				int num20 = x;
				int num21 = y;
				num21 -= WorldGen.genRand.Next(3);
				while (!WorldGen.SolidTile(num20, num21) && !Main.tileCracked[Main.tile[num20, num21].TileType])
				{
					num20--;
					if (num20 < 0)
						return false;
				}

				int num22 = num20;
				num20 = x;
				while (!WorldGen.SolidTile(num20, num21) && !Main.tileCracked[Main.tile[num20, num21].TileType])
				{
					num20++;
					if (num20 >= Main.maxTilesX)
						return false;
				}

				int num23 = num20;
				int num24 = x - num22;
				int num25 = num23 - x;
				bool flag5 = false;
				bool flag6 = false;
				if (num24 is > 5 and < 50)
					flag5 = true;

				if (num25 is > 5 and < 50)
					flag6 = true;

				if (flag5 && !WorldGen.SolidTile(num22, num21 + 1))
					flag5 = false;

				if (flag6 && !WorldGen.SolidTile(num23, num21 + 1))
					flag6 = false;

				if (flag5 && (Main.tile[num22, num21].TileType is 10 or 48 ||
				              Main.tile[num22, num21 + 1].TileType is 10 or 48))
					flag5 = false;

				if (flag6 && (Main.tile[num23, num21].TileType is 10 or 48 ||
				              Main.tile[num23, num21 + 1].TileType is 10 or 48))
					flag6 = false;

				int num26;
				if (flag5 && flag6)
				{
					num26 = 1;
					num20 = num22;
					if (WorldGen.genRand.Next(2) == 0)
					{
						num20 = num23;
						num26 = -1;
					}
				}
				else if (flag6)
				{
					num20 = num23;
					num26 = -1;
				}
				else
				{
					if (!flag5)
					{
						return false;
					}

					num20 = num22;
					num26 = 1;
				}

				if (Main.tile[num20, num21].TileType == 190)
				{
					return false;
				}

				if (Main.tile[x, y].WallType > 0)
					WorldGen.PlaceTile(x, y, 135, mute: true, forced: true, -1, 2);
				else
					WorldGen.PlaceTile(x, y, 135, mute: true, forced: true, -1, WorldGen.genRand.Next(2, 4));

				WorldGen.KillTile(num20, num21);
				WorldGen.PlaceTile(num20, num21, 137, mute: true, forced: true);
				if (num26 == 1)
					Main.tile[num20, num21].TileFrameX += 18;

				RunWire(x, y, num20, num21);

				return true;
			}
			case 1:
			{
				if (WorldGen.noTrapsWorldGen)
					Main.tileSolid[138] = true;

				int num3 = x;
				int num4 = y - 8;
				num3 += WorldGen.genRand.Next(-1, 2);
				if (WorldGen.noTrapsWorldGen)
				{
					if (WorldGen.IsTileNearby(num3, num4, 138, 6))
						return false;

					if (WorldGen.IsTileNearby(num3, num4, 664, 6))
						return false;
				}
				else
				{
					if (WorldGen.IsTileNearby(num3, num4, 138, 10))
						return false;

					if (WorldGen.IsTileNearby(num3, num4, 664, 10))
						return false;

					if (WorldGen.IsTileNearby(num3, num4, 665, 10))
						return false;
				}

				bool flag3 = true;
				while (flag3)
				{
					bool flag4 = true;
					int num5 = 0;
					for (int m = num3 - 2; m <= num3 + 3; m++)
					{
						for (int n = num4; n <= num4 + 3; n++)
						{
							if (!WorldGen.SolidTile(m, n))
								flag4 = false;

							if (Main.tile[m, n].HasTile)
							{
								switch (Main.tile[m, n].TileType)
								{
									case 226:
										return false;
									case 0 or 1 or 59:
										num5++;
										break;
								}
							}
						}
					}

					num4--;
					if (num4 < Main.worldSurface)
					{
						return false;
					}

					if (flag4 && num5 > 2)
						flag3 = false;
				}

				if (y - num4 <= 5 || y - num4 >= 40)
				{
					return false;
				}

				for (int num6 = num3; num6 <= num3 + 1; num6++)
				{
					for (int num7 = num4; num7 <= y; num7++)
					{
						WorldGen.KillTile(num6, num7);
					}
				}

				for (int num8 = num3 - 2; num8 <= num3 + 3; num8++)
				{
					for (int num9 = num4 - 2; num9 <= num4 + 3; num9++)
					{
						if (WorldGen.SolidTile(num8, num9))
							Main.tile[num8, num9].TileType = 1;
					}
				}

				if (WorldGen.IsTileNearby(num3, num4, 21, 4) || WorldGen.IsTileNearby(num3, num4, 467, 4))
				{
					return false;
				}

				WorldGen.PlaceTile(x, y, 135, mute: true, forced: true, -1, 7);
				WorldGen.PlaceTile(num3, num4 + 2, 130, mute: true);
				WorldGen.PlaceTile(num3 + 1, num4 + 2, 130, mute: true);
				if ((WorldGen.tenthAnniversaryWorldGen || WorldGen.noTrapsWorldGen) && WorldGen.genRand.Next(3) == 0)
					WorldGen.PlaceTile(num3 + 1, num4 + 1, 664, mute: true);
				else
					WorldGen.PlaceTile(num3 + 1, num4 + 1, 138, mute: true);

				num4 += 2;
				Tile tile = Main.tile[num3, num4];
				Tile tile1 = Main.tile[num3 + 1, num4];
				tile.RedWire = true;
				tile1.RedWire = true;
				num4++;
				WorldGen.PlaceTile(num3, num4, 130, mute: true);
				WorldGen.PlaceTile(num3 + 1, num4, 130, mute: true);
				tile.RedWire = true;
				tile1.RedWire = true;
				WorldGen.PlaceTile(num3, num4 + 1, 130, mute: true);
				WorldGen.PlaceTile(num3 + 1, num4 + 1, 130, mute: true);
				Tile tile2 = Main.tile[num3, num4 + 1];
				tile2.RedWire = true;
				Tile tile3 = Main.tile[num3 + 1, num4 + 1];
				tile3.RedWire = true;
				RunWire(x, y, num3, num4);

				return true;
			}
			case 2:
			{
				int num12 = WorldGen.genRand.Next(4, 7);
				int num13 = x;
				num13 += WorldGen.genRand.Next(-1, 2);
				int num14 = y;
				for (int num15 = 0; num15 < num12; num15++)
				{
					num14++;
					if (!WorldGen.SolidTile(num13, num14))
					{
						return false;
					}
				}

				for (int num16 = num13 - 2; num16 <= num13 + 2; num16++)
				{
					for (int num17 = num14 - 2; num17 <= num14 + 2; num17++)
					{
						if (!WorldGen.SolidTile(num16, num17))
							return false;
					}
				}

				WorldGen.KillTile(num13, num14);
				Tile tile = Main.tile[num13, num14];
				tile.HasTile = true;
				tile.TileType = 141;
				tile.TileFrameX = 0;
				tile.TileFrameY = (short)(18 * WorldGen.genRand.Next(2));
				WorldGen.PlaceTile(x, y, 135, mute: true, forced: true, -1, WorldGen.genRand.Next(2, 4));
				RunWire(x, y, num13, num14);

				break;
			}
			case 3:
			{
				if (Main.tile[x + 1, y].HasTile)
					return false;

				if (Main.tile[x, y].LiquidAmount > 0 && Main.tile[x, y].LiquidType != LiquidID.Lava)
					return false;

				if (WorldGen.noTrapsWorldGen && (WorldGen.tenthAnniversaryWorldGen || WorldGen.notTheBees))
				{
					if (WorldGen.genRand.Next(3) != 0)
						return false;

					if (WorldGen.IsTileNearby(x, y, 443, 30))
						return false;
				}

				for (int k = x; k <= x + 1; k++)
				{
					int j2 = y + 1;
					if (!WorldGen.SolidTile(k, j2))
						return false;
				}

				int num2 = WorldGen.genRand.Next(2);
				for (int l = 0; l < 2; l++)
				{
					Tile tile = Main.tile[x + l, y];
					tile.HasTile = true;
					tile.TileType = 443;
					tile.TileFrameX = (short)(18 * l + 36 * num2);
					tile.TileFrameY = 0;
				}

				return true;
			}
		}

		if (WorldGen.noTrapsWorldGen)
			Main.tileSolid[138] = true;

		return false;
	}

	private static void RunWire(int x, int y, int targetX, int targetY)
	{
		Tile tile = Main.tile[x, y];
		tile.RedWire = true;
		int dx = Math.Sign(targetX - x);
		int dy = Math.Sign(targetY - y);
		while (x != targetX || y != targetY)
		{
			if (x != targetX)
			{
				x += dx;
				tile = Main.tile[x, y];
				tile.RedWire = true;
			}

			if (y != targetY)
			{
				y += dy;
				tile = Main.tile[x, y];
				tile.RedWire = true;
			}
		}
	}
}