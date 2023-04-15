namespace AdvancedWorldGen.Secret._100k_special.InvertedPyramid;

public class Dimaryp : ControlledWorldGenPass
{
	public Dimaryp() : base("Dimaryp", 6.6884f)
	{
	}

	protected override void ApplyPass()
	{
		Rectangle undergroundDesertLocation = GenVars.UndergroundDesertLocation;
		int x = undergroundDesertLocation.Center.X;
		int y = undergroundDesertLocation.Top - 20;
		Pyramid(x, y);
	}

	private static void Pyramid(int x, int y)
	{
		#region structure
		int height = _random.Next(75, 125);
		int yMin = y - height;
		int width = 1 + height;
		int leftBorder = x - width;
		int rightBorder = x + width - 2;
		for (int y1 = yMin; y1 < y; y1++) {
			for (int l = leftBorder; l <= rightBorder; l++) {
				Tile tile = Main.tile[l, y1];
				tile.ResetToType(TileID.SandstoneBrick);
				if (y1 > yMin && y1 < y - 1 && l > leftBorder && l < rightBorder)
					tile.WallType = WallID.SandstoneBrick;
			}

			width--;
		}
		#endregion

		int direction = _random.Next(2) == 0 ? -1 : 1;

		#region entrance
		int playerHeight = _random.Next(5, 8);
		int entranceShift = _random.Next(9, 13);
		int entranceStartX = x - entranceShift * direction;
		int entranceStartY = y - entranceShift - playerHeight;
		bool flag2 = true;
		while (flag2) {
			flag2 = false;
			for (int num13 = entranceStartY; num13 <= entranceStartY + playerHeight; num13++) {
				Tile tile = Main.tile[entranceStartX, num13];
				if (tile.TileType == TileID.SandstoneBrick) {
					Main.tile[entranceStartX, num13 - 1].WallType = WallID.SandstoneBrick;
					Main.tile[entranceStartX + direction, num13].WallType = WallID.SandstoneBrick;
					tile.HasTile = false;
					flag2 = true;
				}
			}

			entranceStartX -= direction;
		}
		#endregion

		int num12 = _random.Next(20, 30);
		entranceStartX = x - entranceShift * direction;
		bool flag4 = true;
		bool flag5 = false;
		flag2 = true;
		while (flag2) {
			for (int num15 = entranceStartY; num15 <= entranceStartY + playerHeight; num15++)
			{
				Tile tile = Main.tile[entranceStartX, num15];
				tile.HasTile = false;
			}

			entranceStartX += direction;
			entranceStartY--;
			num12--;
			if (entranceStartY <= yMin + playerHeight * 2)
				num12 = 10;

			if (num12 <= 0) {
				bool flag6 = false;
				if (!flag4 && !flag5) {
					flag5 = true;
					flag6 = true;
					int roomHeight = _random.Next(7, 13);
					int roomWidth = _random.Next(23, 28);
					int num19 = roomWidth;
					int num20 = entranceStartX;
					int roomTop = entranceStartY + playerHeight - roomHeight;
					while (roomWidth > 0) {
						for (int num21 = roomTop; num21 <= entranceStartY + playerHeight; num21++)
						{
							Tile tile = Main.tile[entranceStartX, num21];
							if (roomWidth == num19 || roomWidth == 1) {
								if (num21 >= roomTop + 2)
									tile.HasTile = false;
							}
							else if (roomWidth == num19 - 1 || roomWidth == 2 || roomWidth == num19 - 2 || roomWidth == 3) {
								if (num21 >= roomTop + 1)
									tile.HasTile = false;
							}
							else {
								tile.HasTile = false;
							}
						}

						roomWidth--;
						entranceStartX += direction;
					}

					int num22 = entranceStartX - direction;
					int num23 = num22;
					int num24 = num20;
					if (num22 > num20) {
						num23 = num20;
						num24 = num22;
					}

					int num25 = _random.Next(3);
					if (num25 == 0)
						num25 = _random.Next(3);

					if (Main.tenthAnniversaryWorld && num25 == 0)
						num25 = 1;

					num25 = num25 switch
					{
						0 => 848,
						1 => 857,
						2 => 934,
						_ => num25
					};

					WorldGen.AddBuriedChest((num23 + num24) / 2, entranceStartY + playerHeight, num25, notNearOtherChests: false, 1, trySlope: false);
					int num26 = _random.Next(1, 10);
					for (int num27 = 0; num27 < num26; num27++) {
						int i2 = _random.Next(num23, num24);
						int j2 = entranceStartY + playerHeight;
						WorldGen.PlaceSmallPile(i2, j2, _random.Next(16, 19), 1);
					}

					WorldGen.PlaceTile(num23 + 2, roomTop - 1, 91, mute: true, forced: false, -1, _random.Next(4, 7));
					WorldGen.PlaceTile(num23 + 3, roomTop, 91, mute: true, forced: false, -1, _random.Next(4, 7));
					WorldGen.PlaceTile(num24 - 2, roomTop - 1, 91, mute: true, forced: false, -1, _random.Next(4, 7));
					WorldGen.PlaceTile(num24 - 3, roomTop, 91, mute: true, forced: false, -1, _random.Next(4, 7));
					for (int num28 = num23; num28 <= num24; num28++) {
						WorldGen.PlacePot(num28, entranceStartY + playerHeight, 28, _random.Next(25, 28));
					}
				}

				if (flag4) {
					flag4 = false;
					direction *= -1;
					num12 = _random.Next(15, 20);
				}
				else if (flag6) {
					num12 = _random.Next(10, 15);
				}
				else {
					direction *= -1;
					num12 = _random.Next(20, 40);
				}
			}

			if (entranceStartY < yMin - playerHeight)
				flag2 = false;
		}
	}
}