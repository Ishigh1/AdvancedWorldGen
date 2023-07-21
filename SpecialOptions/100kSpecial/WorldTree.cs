namespace AdvancedWorldGen.SpecialOptions._100kSpecial;

public class WorldTree : ControlledWorldGenPass
{
	public WorldTree() : base("World Tree", 0)
	{
	}

	protected override void ApplyPass()
	{
		int i = Main.maxTilesX / 2;
		int j = (int)GenVars.worldSurfaceLow;

		while (true)
		{
			bool flag1 = true;
			for (int k = 0; k < 5; k++)
			{
				if (Main.tile[i, j - k].HasTile)
				{
					j--;
					flag1 = false;
					break;
				}
			}

			if (flag1)
				break;
		}
		
		while (true)
		{
			if (!Main.tile[i, j + 1].HasTile)
			{
				j++;
			}
			else
			{
				break;
			}

		}

		int num = 0;
		int[] array = new int[1000];
		int[] array2 = new int[1000];
		int[] array3 = new int[1000];
		int[] array4 = new int[1000];
		int num2 = 0;
		int[] array5 = new int[2000];
		int[] array6 = new int[2000];
		bool[] array7 = new bool[2000];

		int num3 = i - WorldGen.genRand.Next(2, 3);
		int num4 = i + WorldGen.genRand.Next(2, 3);
		if (WorldGen.genRand.Next(5) == 0)
		{
			if (WorldGen.genRand.Next(2) == 0)
				num3--;
			else
				num4++;
		}

		int num5 = num4 - num3;
		bool flag = num5 >= 4;

		int num8 = num3;
		int num9 = num4;
		int minl = num3;
		int minr = num4;
		bool flag2 = true;
		int num10 = WorldGen.genRand.Next(-8, -4);
		int num11 = WorldGen.genRand.Next(2);
		int num12 = j;
		int num13 = WorldGen.genRand.Next(5, 15);
		Main.tileSolid[48] = false;
		while (flag2)
		{
			num10++;
			if (num10 > num13)
			{
				num13 = WorldGen.genRand.Next(5, 15);
				num10 = 0;
				array2[num] = num12 + WorldGen.genRand.Next(5);
				if (WorldGen.genRand.Next(5) == 0)
					num11 = num11 == 0 ? 1 : 0;

				if (num11 == 0)
				{
					array3[num] = -1;
					array[num] = num3;
					array4[num] = num4 - num3;
					if (WorldGen.genRand.Next(2) == 0)
						num3++;

					num8++;
					num11 = 1;
				}
				else
				{
					array3[num] = 1;
					array[num] = num4;
					array4[num] = num4 - num3;
					if (WorldGen.genRand.Next(2) == 0)
						num4--;

					num9--;
					num11 = 0;
				}

				if (num8 == num9)
					flag2 = false;

				num++;
			}

			for (int m = num3; m <= num4; m++)
			{
				Tile tile = Main.tile[m, num12];
				tile.TileType = 191;
				tile.HasTile = true;
				tile.IsHalfBlock = false;
			}

			num12--;
		}

		for (int n = 0; n < num - 1; n++)
		{
			int num14 = array[n] + array3[n];
			int num15 = array2[n];
			int num16 = (int)(array4[n] * (1.0 + WorldGen.genRand.Next(20, 30) * 0.1));
			Tile tile = Main.tile[num14, num15 + 1];
			tile.TileType = 191;
			tile.HasTile = true;
			tile.IsHalfBlock = false;
			int num17 = WorldGen.genRand.Next(3, 5);
			while (num16 > 0)
			{
				num16--;
				Tile tile1 = Main.tile[num14, num15];
				tile1.TileType = 191;
				tile1.HasTile = true;
				tile1.IsHalfBlock = false;
				if (WorldGen.genRand.Next(10) == 0)
					num15 = WorldGen.genRand.Next(2) != 0 ? num15 + 1 : num15 - 1;
				else
					num14 += array3[n];

				tile1 = Main.tile[num14, num15];

				if (num17 > 0)
				{
					num17--;
				}
				else if (WorldGen.genRand.Next(2) == 0)
				{
					num17 = WorldGen.genRand.Next(2, 5);
					if (WorldGen.genRand.Next(2) == 0)
					{
						tile1.TileType = 191;
						tile1.HasTile = true;
						tile1.IsHalfBlock = false;
						Tile tile2 = Main.tile[num14, num15 - 1];
						tile2.TileType = 191;
						tile2.HasTile = true;
						tile2.IsHalfBlock = false;
						array5[num2] = num14;
						array6[num2] = num15;
						num2++;
					}
					else
					{
						tile1.TileType = 191;
						tile1.HasTile = true;
						tile1.IsHalfBlock = false;
						tile = Main.tile[num14, num15 + 1];
						tile.TileType = 191;
						tile.HasTile = true;
						tile.IsHalfBlock = false;
						array5[num2] = num14;
						array6[num2] = num15;
						num2++;
					}
				}

				if (num16 == 0)
				{
					array5[num2] = num14;
					array6[num2] = num15;
					num2++;
				}
			}
		}

		int num18 = (num3 + num4) / 2;
		int num19 = num12;
		int num20 = WorldGen.genRand.Next(num5 * 3, num5 * 5);
		int num21 = 0;
		int num22 = 0;
		while (num20 > 0)
		{
			Tile tile = Main.tile[num18, num19];
			tile.TileType = 191;
			tile.HasTile = true;
			tile.IsHalfBlock = false;
			if (num21 > 0)
				num21--;

			if (num22 > 0)
				num22--;

			for (int num23 = -1; num23 < 2; num23++)
			{
				if (num23 == 0 || ((num23 >= 0 || num21 != 0) && (num23 <= 0 || num22 != 0)) ||
				    WorldGen.genRand.Next(2) != 0)
					continue;

				int num24 = num18;
				int num25 = num19;
				int num26 = WorldGen.genRand.Next(num5, num5 * 3);
				if (num23 < 0)
					num21 = WorldGen.genRand.Next(3, 5);

				if (num23 > 0)
					num22 = WorldGen.genRand.Next(3, 5);

				int num27 = 0;
				while (num26 > 0)
				{
					num26--;
					num24 += num23;
					Tile tile2 = Main.tile[num24, num25];
					tile2.TileType = 191;
					tile2.HasTile = true;
					tile2.IsHalfBlock = false;
					if (num26 == 0)
					{
						array5[num2] = num24;
						array6[num2] = num25;
						array7[num2] = true;
						num2++;
					}

					if (WorldGen.genRand.Next(5) == 0)
					{
						num25 = WorldGen.genRand.Next(2) != 0 ? num25 + 1 : num25 - 1;
						tile2 = Main.tile[num24, num25];
						tile2.TileType = 191;
						tile2.HasTile = true;
						tile2.IsHalfBlock = false;
					}

					if (num27 > 0)
					{
						num27--;
					}
					else if (WorldGen.genRand.Next(3) == 0)
					{
						num27 = WorldGen.genRand.Next(2, 4);
						int num29 = num25;
						num29 = WorldGen.genRand.Next(2) != 0 ? num29 + 1 : num29 - 1;
						Tile tile1 = Main.tile[num24, num29];
						tile1.TileType = 191;
						tile1.HasTile = true;
						tile1.IsHalfBlock = false;
						array5[num2] = num24;
						array6[num2] = num29;
						array7[num2] = true;
						num2++;
						array5[num2] = num24 + WorldGen.genRand.Next(-5, 6);
						array6[num2] = num29 + WorldGen.genRand.Next(-5, 6);
						array7[num2] = true;
						num2++;
					}
				}
			}

			array5[num2] = num18;
			array6[num2] = num19;
			num2++;
			if (WorldGen.genRand.Next(4) == 0)
			{
				num18 = WorldGen.genRand.Next(2) != 0 ? num18 + 1 : num18 - 1;
				tile = Main.tile[num18, num19];
				tile.TileType = 191;
				tile.HasTile = true;
				tile.IsHalfBlock = false;
			}

			num19--;
			num20--;
		}

		for (int num30 = minl; num30 <= minr; num30++)
		{
			int num31 = WorldGen.genRand.Next(1, 6);
			int num32 = j + 1;
			while (num31 > 0)
			{
				if (WorldGen.SolidTile(num30, num32))
					num31--;

				Tile tile = Main.tile[num30, num32];
				tile.TileType = 191;
				tile.HasTile = true;
				tile.IsHalfBlock = false;
				num32++;
			}

			int num33 = num32;
			int num34 = WorldGen.genRand.Next(2, num5 + 1);
			for (int num35 = 0; num35 < num34; num35++)
			{
				num32 = num33;
				int num36 = (minl + minr) / 2;
				int num38 = 1;
				int num37 = num30 >= num36 ? 1 : -1;
				if (num30 == num36 || (num5 > 6 && (num30 == num36 - 1 || num30 == num36 + 1)))
					num37 = 0;

				int num39 = num37;
				int num40 = num30;
				num31 = WorldGen.genRand.Next((int)(num5 * 3.5), num5 * 6);
				while (num31 > 0)
				{
					num31--;
					num40 += num37;
					Tile tile = Main.tile[num40, num32];
					if (tile.WallType != 244)
					{
						tile.TileType = 191;
						tile.HasTile = true;
						tile.IsHalfBlock = false;
					}

					num32 += num38;
					tile = Main.tile[num40, num32];
					if (tile.WallType != 244)
					{
						tile.TileType = 191;
						tile.HasTile = true;
						tile.IsHalfBlock = false;
					}

					if (!Main.tile[num40, num32 + 1].HasTile)
					{
						num37 = 0;
						num38 = 1;
					}

					if (WorldGen.genRand.Next(3) == 0)
						num37 = num39 < 0
							? num37 == 0 ? -1 : 0
							: num39 <= 0
								? WorldGen.genRand.Next(-1, 2)
								: num37 == 0
									? 1
									: 0;

					if (WorldGen.genRand.Next(3) == 0)
						num38 = num38 == 0 ? 1 : 0;
				}
			}
		}

		if (WorldGen.remixWorldGen)
			num2 = 0;

		for (int num41 = 0; num41 < num2; num41++)
		{
			int num42 = WorldGen.genRand.Next(5, 8);
			num42 = (int)(num42 * (1.0 + num5 * 0.05));
			if (array7[num41])
				num42 = WorldGen.genRand.Next(6, 12) + num5;

			int num43 = array5[num41] - num42 * 2;
			int num44 = array5[num41] + num42 * 2;
			int num45 = array6[num41] - num42 * 2;
			int num46 = array6[num41] + num42 * 2;
			double num47 = 2.0 - WorldGen.genRand.Next(5) * 0.1;
			for (int num48 = num43; num48 <= num44; num48++)
			{
				for (int num49 = num45; num49 <= num46; num49++)
				{
					Tile tile = Main.tile[num48, num49];
					if (tile.TileType == 191)
						continue;

					if (array7[num41])
					{
						if ((new Vector2D(array5[num41], array6[num41]) - new Vector2D(num48, num49)).Length() <
						    num42 * 0.9)
						{
							tile.TileType = 192;
							tile.HasTile = true;
							tile.IsHalfBlock = false;
						}
					}
					else if (Math.Abs(array5[num41] - num48) + Math.Abs(array6[num41] - num49) * num47 <
					         num42)
					{
						tile.TileType = 192;
						tile.HasTile = true;
						tile.IsHalfBlock = false;
					}
				}

				if (WorldGen.genRand.Next(30) == 0)
				{
					int num50 = num45;
					if (!Main.tile[num48, num50].HasTile)
					{
						for (; !Main.tile[num48, num50 + 1].HasTile && num50 < num46; num50++)
						{
						}

						if (Main.tile[num48, num50 + 1].TileType == 192)
							WorldGen.PlaceTile(num48, num50, 187, mute: true, forced: false, -1,
								WorldGen.genRand.Next(50, 52));
					}
				}

				if (array7[num41] || WorldGen.genRand.Next(15) != 0)
					continue;

				int num51 = num46;
				int num52 = num51 + 100;
				if (Main.tile[num48, num51].HasTile)
					continue;

				for (; !Main.tile[num48, num51 + 1].HasTile && num51 < num52; num51++)
				{
				}

				if (Main.tile[num48, num51 + 1].TileType == 192)
					continue;

				if (WorldGen.genRand.Next(2) == 0)
				{
					WorldGen.PlaceTile(num48, num51, 187, mute: true, forced: false, -1, WorldGen.genRand.Next(47, 50));
					continue;
				}

				int num53 = WorldGen.genRand.Next(2);
				int x = 72;
				if (num53 == 1)
					x = WorldGen.genRand.Next(59, 62);

				WorldGen.PlaceSmallPile(num48, num51, x, num53);
			}
		}

		if (flag)
		{
			bool flag3 = false;
			for (int num54 = j; num54 < j + 20 && !(num54 >= Main.worldSurface - 2.0); num54++)
			{
				for (int num55 = minl; num55 <= minr; num55++)
				{
					if (Main.tile[num55, num54].WallType == 0 && !WorldGen.SolidTile(num55, num54))
						flag3 = true;
				}
			}

			if (!flag3) GrowLivingTree_MakePassage(j, num5, ref minl, ref minr);
		}

		Main.tileSolid[48] = true;
	}

	private static void GrowLivingTree_MakePassage(int j, int w, ref int minl, ref int minr)
	{
		int num = minl;
		int num2 = minr;
		int y = j - 6;
		int num5 = 50;
		int num7 = 0;
		int shift = _random.NextBool(2) ? 1 : -1;
		bool first = true;
		int lastLeft = 0;
		int lastRight = 0;
		while (true)
		{
			int middle = (minl + minr) / 2;
			y++;
			num5--;

			int num10 = 1;
			if (y > j && w <= 4)
				num10++;

			for (int i = minl - num10; i <= minr + num10; i++)
			{
				Tile tile1 = Main.tile[i, y];

				if (i >= middle - 1 && i <= middle + 1)
				{
					if (y > j - 4)
					{
						Tile tile = Main.tile[i, y + 1];
						tile1 = Main.tile[i, y];
						if (tile1.TileType != 19 && tile1.TileType != 15 &&
						    tile1.TileType != 304 && tile1.TileType != 21 &&
						    tile1.TileType != 10 && Main.tile[i, y - 1].TileType != 15 &&
						    Main.tile[i, y - 1].TileType != 304 && Main.tile[i, y - 1].TileType != 21 &&
						    Main.tile[i, y - 1].TileType != 10 && tile.TileType != 10)
							tile1.HasTile = false;

						if (!Main.wallDungeon[tile1.WallType])
							tile1.WallType = 244;

						if (!Main.wallDungeon[Main.tile[i - 1, y].WallType] &&
						    (Main.tile[i - 1, y].WallType > 0 || y >= Main.worldSurface))
							Main.tile[i - 1, y].WallType = 244;

						if (!Main.wallDungeon[Main.tile[i + 1, y].WallType] &&
						    (Main.tile[i + 1, y].WallType > 0 || y >= Main.worldSurface))
							Main.tile[i + 1, y].WallType = 244;

						if (y == j && i > middle - 2 && i <= middle + 1)
						{
							tile.HasTile = false;
							WorldGen.PlaceTile(i, y + 1, 19, mute: true, forced: false, -1, 23);
						}
					}
				}
				else
				{
					if (tile1.TileType != 15 && tile1.TileType != 304 &&
					    tile1.TileType != 21 && tile1.TileType != 10 &&
					    Main.tile[i - 1, y].TileType != 10 && Main.tile[i + 1, y].TileType != 10)
					{
						if (!Main.wallDungeon[tile1.WallType])
						{
							tile1.TileType = 191;
							tile1.HasTile = true;
							tile1.IsHalfBlock = false;
						}

						if (Main.tile[i - 1, y].TileType == 40)
							Main.tile[i - 1, y].TileType = 0;

						if (Main.tile[i + 1, y].TileType == 40)
							Main.tile[i + 1, y].TileType = 0;
					}

					if (y <= j && y > j - 4 && i > minl - num10 && i <= minr + num10 - 1)
						tile1.WallType = 244;
				}
			}

			num7++;
			if (num7 >= 6)
			{
				num7 = 0;

				if (first)
					first = false;
				else
				{
					minl += shift;
					minr += shift;
					shift = -shift;

					if (num5 <= 0 && 
					    ((shift == 1 && lastLeft < y - 6) || (shift == -1 && lastRight < y - 12))
					      && _random.NextBool(10))
					{
						if (shift == 1)
							lastLeft = y;
						else
							lastRight = y;
						GrowLivingTreePassageRoom(minl, minr, y);
					}
				}
			}

			if (y > Main.UnderworldLayer)
			{
				bool flag1 = true;
				for (int k = 0; k < 5; k++)
				{
					if (Main.tile[middle, y + k].HasTile)
					{
						j--;
						flag1 = false;
						break;
					}
				}

				if (flag1)
					break;
			}
		}

		minl = num;
		minr = num2;
		for (int num13 = minl; num13 <= minr; num13++)
		{
			for (int num14 = j - 3; num14 <= j; num14++)
			{
				Tile tile = Main.tile[num13, num14];
				tile.HasTile = false;
				bool flag6 = true;
				for (int num15 = num13 - 1; num15 <= num13 + 1; num15++)
				{
					for (int num16 = num14 - 1; num16 <= num14 + 1; num16++)
					{
						if (!Main.tile[num15, num16].HasTile && Main.tile[num15, num16].WallType == 0)
							flag6 = false;
					}
				}

				if (flag6 && !Main.wallDungeon[tile.WallType])
					tile.WallType = 244;
			}
		}
	}

	private static void GrowLivingTreePassageRoom(int minl, int minr, int y)
	{
		int num = WorldGen.genRand.Next(2);
		if (num == 0)
			num = -1;

		int num2 = y - 2;
		int num3 = (minl + minr) / 2;
		switch (num)
		{
			case < 0:
				num3--;
				break;
			case > 0:
				num3++;
				break;
		}

		int num4 = WorldGen.genRand.Next(15, 30);
		int num5 = num3 + num4;
		if (num < 0)
		{
			num5 = num3;
			num3 -= num4;
		}

		for (int i = num3; i < num5; i++)
		{
			for (int j = y - 20; j < y + 10; j++)
			{
				if (Main.tile[i, j].WallType == 0 && !Main.tile[i, j].HasTile && j < Main.worldSurface)
					return;
			}
		}

		GenVars.dMinX = num3;
		GenVars.dMaxX = num5;
		if (num < 0)
			GenVars.dMinX -= 40;
		else
			GenVars.dMaxX += 40;

		for (int k = num3; k <= num5; k++)
		{
			for (int l = num2 - 2; l <= y + 2; l++)
			{
				if (Main.tile[k - 1, l].TileType == 40)
					Main.tile[k - 1, l].TileType = 0;

				if (Main.tile[k + 1, l].TileType == 40)
					Main.tile[k + 1, l].TileType = 0;

				if (Main.tile[k, l - 1].TileType == 40)
					Main.tile[k, l - 1].TileType = 0;

				if (Main.tile[k, l + 1].TileType == 40)
					Main.tile[k, l + 1].TileType = 0;

				Tile tile = Main.tile[k, l];
				if (tile.WallType != 244 && tile.TileType != 19)
				{
					tile.HasTile = true;
					tile.TileType = 191;
					tile.IsHalfBlock = false;
				}

				if (l >= num2 && l <= y)
				{
					tile.LiquidAmount = 0;
					tile.WallType = 244;
					tile.HasTile = false;
				}
			}
		}

		int i2 = (minl + minr) / 2 + 3 * num;
		WorldGen.PlaceTile(i2, y, 10, mute: true, forced: false, -1, 7);
		int num6 = WorldGen.genRand.Next(5, 9);
		int num7 = WorldGen.genRand.Next(4, 6);
		if (num < 0)
		{
			num5 = num3 + num6;
			num3 -= num6;
		}
		else
		{
			num3 = num5 - num6;
			num5 += num6;
		}

		num2 = y - num7;
		for (int m = num3 - 2; m <= num5 + 2; m++)
		{
			for (int n = num2 - 2; n <= y + 2; n++)
			{
				if (Main.tile[m - 1, n].TileType == 40)
					Main.tile[m - 1, n].TileType = 40;

				if (Main.tile[m + 1, n].TileType == 40)
					Main.tile[m + 1, n].TileType = 40;

				if (Main.tile[m, n - 1].TileType == 40)
					Main.tile[m, n - 1].TileType = 40;

				if (Main.tile[m, n + 1].TileType == 40)
					Main.tile[m, n + 1].TileType = 40;

				Tile tile = Main.tile[m, n];
				if (tile.WallType != 244 && tile.TileType != 19)
				{
					tile.HasTile = true;
					tile.TileType = 191;
					tile.IsHalfBlock = false;
				}

				if (n >= num2 && n <= y && m >= num3 && m <= num5)
				{
					tile.LiquidAmount = 0;
					tile.WallType = 244;
					tile.HasTile = false;
				}
			}
		}

		if (num < 0)
			i2 = num5 + 2;
		else
			i2 = num3 - 2;

		WorldGen.PlaceTile(i2, y, 10, mute: true, forced: false, -1, 7);
		int num8 = num < 0 ? num3 : num5;

		int num9 = 2;
		if (WorldGen.genRand.Next(num9) == 0)
		{
			num9 += 2;
			WorldGen.PlaceTile(num8, y, 15, mute: true, forced: false, -1, 5);
			if (num < 0)
			{
				Main.tile[num8, y - 1].TileFrameX += 18;
				Main.tile[num8, y].TileFrameX += 18;
			}
		}

		num8 = num5 - 2;
		if (num < 0)
			num8 = num3 + 2;

		WorldGen.PlaceTile(num8, y, 304, mute: true);
		num8 = num5 - 4;
		if (num < 0)
			num8 = num3 + 4;

		if (WorldGen.genRand.Next(num9) == 0)
		{
			WorldGen.PlaceTile(num8, y, 15, mute: true, forced: false, -1, 5);
			if (num > 0)
			{
				Main.tile[num8, y - 1].TileFrameX += 18;
				Main.tile[num8, y].TileFrameX += 18;
			}
		}

		num8 = num5 - 7;
		if (num < 0)
			num8 = num3 + 8;

		int contain = 832;
		if (WorldGen.genRand.Next(3) == 0)
			contain = 4281;

		if (WorldGen.remixWorldGen)
		{
			int num10 = WorldGen.genRand.Next(1, 3);
			for (int num11 = 0; num11 < num10; num11++)
			{
				bool flag = false;
				while (!flag)
				{
					int num12 = WorldGen.genRand.Next(Main.maxTilesX / 8, Main.maxTilesX - Main.maxTilesX / 8);
					int num13 = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 350);
					if (!WorldGen.IsTileNearby(num12, num13, 53, 20) && !WorldGen.IsTileNearby(num12, num13, 147, 20) &&
					    !WorldGen.IsTileNearby(num12, num13, 59, 20))
						flag = GenerationChests.AddBuriedChest(num12, num13, contain, notNearOtherChests: false, 12, 0);
				}
			}

			if (WorldGen.crimson)
				GenerationChests.AddBuriedChest(num8, y, 0, notNearOtherChests: false, 14, 0);
			else
				GenerationChests.AddBuriedChest(num8, y, 0, notNearOtherChests: false, 7, 0);
		}
		else
		{
			GenerationChests.AddBuriedChest(num8, y, contain, notNearOtherChests: false, 12, 0);
		}
	}
}