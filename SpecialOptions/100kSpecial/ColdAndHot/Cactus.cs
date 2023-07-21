using Terraria.ID;

namespace AdvancedWorldGen.SpecialOptions._100kSpecial.ColdAndHot;

public static class Cactus
{
	public static bool CheckCactus(On_WorldGen.orig_CheckCactus? orig, int i, int j)
	{
		int num = j;
		int num2 = i;
		ushort tileType = Main.tile[num2, num].TileType;
		while (Main.tile[num2, num].HasTile && tileType == TileID.Cactus)
		{
			num++;
			tileType = Main.tile[num2, num].TileType;

			if (!Main.tile[num2, num].HasTile || tileType != TileID.Cactus)
			{
				if (Main.tile[num2 - 1, num].HasTile && Main.tile[num2 - 1, num].TileType == TileID.Cactus &&
				    Main.tile[num2 - 1, num - 1].HasTile && Main.tile[num2 - 1, num - 1].TileType == TileID.Cactus &&
				    num2 >= i)
					num2--;

				if (Main.tile[num2 + 1, num].HasTile && Main.tile[num2 + 1, num].TileType == TileID.Cactus &&
				    Main.tile[num2 + 1, num - 1].HasTile && Main.tile[num2 + 1, num - 1].TileType == TileID.Cactus &&
				    num2 <= i)
					num2++;
			}
		}

		if (!Main.tile[num2, num].HasUnactuatedTile || Main.tile[num2, num].IsHalfBlock ||
		    Main.tile[num2, num].Slope != 0 ||
		    (tileType is not TileID.SnowBlock && !TileLoader.CanGrowModCactus(tileType)))
		{
			WorldGen.KillTile(i, j);
			return true;
		}

		if (i != num2)
		{
			if ((!Main.tile[i, j + 1].HasTile || Main.tile[i, j + 1].TileType != TileID.Cactus) &&
			    (!Main.tile[i - 1, j].HasTile || Main.tile[i - 1, j].TileType != TileID.Cactus) &&
			    (!Main.tile[i + 1, j].HasTile || Main.tile[i + 1, j].TileType != TileID.Cactus))
			{
				WorldGen.KillTile(i, j);
				return true;
			}
		}
		else if (i == num2 && (!Main.tile[i, j + 1].HasTile || (Main.tile[i, j + 1].TileType is not TileID.SnowBlock &&
		                                                        !TileLoader.CanGrowModCactus(Main.tile[i, j + 1]
			                                                        .TileType))))
		{
			WorldGen.KillTile(i, j);
			return true;
		}

		return false;
	}
	
	public static void GrowCactus(On_WorldGen.orig_GrowCactus? orig, int i, int j)
	{
		int num = j;
		int num2 = i;
		Tile tile = Main.tile[i, j];
		Tile tile2 = Main.tile[i, j - 1];
		if (!tile.HasUnactuatedTile || tile.IsHalfBlock || (!WorldGen.gen && tile.Slope != 0) || tile2.LiquidAmount > 0 || (tile.TileType != TileID.SnowBlock))
			return;

		int num3 = 0;
		for (int k = i - WorldGen.cactusWaterWidth; k < i + WorldGen.cactusWaterWidth; k++) {
			for (int l = j - WorldGen.cactusWaterHeight; l < j + WorldGen.cactusWaterHeight; l++) {
				num3 += Main.tile[k, l].LiquidAmount;
			}
		}

		if ((!Main.remixWorld || !(j > Main.worldSurface)) && num3 / 255 > WorldGen.cactusWaterLimit)
			return;

		if (tile.TileType is TileID.SnowBlock|| TileLoader.CanGrowModCactus(tile.TileType)) {
			if (tile2.HasTile || Main.tile[i - 1, j - 1].HasTile || Main.tile[i + 1, j - 1].HasTile)
				return;

			int num4 = 0;
			int num5 = 0;
			for (int m = i - 6; m <= i + 6; m++) {
				for (int n = j - 3; n <= j + 1; n++) {
					try {
						if (!Main.tile[m, n].HasTile)
							continue;

						if (Main.tile[m, n].TileType == 80) {
							num4++;
							if (num4 >= 4)
								return;
						}

						if (Main.tile[m, n].TileType is TileID.SnowBlock || TileLoader.CanGrowModCactus(Main.tile[m, n].TileType))
							num5++;
					}
					catch
					{
						// 
					}
				}
			}

			if (num5 > 10) {
				if (WorldGen.gen && WorldGen.genRand.Next(2) == 0)
					tile.Slope = 0;

				tile2.HasTile = true;
				tile2.TileType = 80;
				if (Main.netMode == NetmodeID.Server)
					NetMessage.SendTileSquare(-1, i, j - 1);

				WorldGen.SquareTileFrame(num2, num - 1);
			}
		}
		else {
			if (tile.TileType != 80)
				return;

			Tile tile3 = Main.tile[num2 - 1, num];
			Tile tile4 = Main.tile[num2 + 1, num];
			while (Main.tile[num2, num].HasTile && Main.tile[num2, num].TileType == 80) {
				num++;
				if (!Main.tile[num2, num].HasTile || Main.tile[num2, num].TileType != 80) {
					if (tile3.HasTile && tile3.TileType == 80 && Main.tile[num2 - 1, num - 1].HasTile && Main.tile[num2 - 1, num - 1].TileType == 80 && num2 >= i)
						num2--;

					if (tile4.HasTile && tile4.TileType == 80 && Main.tile[num2 + 1, num - 1].HasTile && Main.tile[num2 + 1, num - 1].TileType == 80 && num2 <= i)
						num2++;
				}
			}

			num--;
			int num6 = num - j;
			int num7 = i - num2;
			num2 = i - num7;
			num = j;
			int num8 = 11 - num6;
			int num9 = 0;
			for (int num10 = num2 - 2; num10 <= num2 + 2; num10++) {
				for (int num11 = num - num8; num11 <= num + num6; num11++) {
					if (Main.tile[num10, num11].HasTile && Main.tile[num10, num11].TileType == 80)
						num9++;
				}
			}

			if (Main.drunkWorld) {
				if (num9 >= WorldGen.genRand.Next(11, 20))
					return;
			}
			else if (num9 >= WorldGen.genRand.Next(11, 13)) {
				return;
			}

			num2 = i;
			num = j;
			Tile tile1 = Main.tile[num2, num - 1];
			tile3 = Main.tile[num2 - 1, num];
			tile4 = Main.tile[num2 + 1, num];
			if (num7 == 0) {
				if (num6 == 0) {
					if (!tile1.HasTile) {
						tile1.HasTile = true;
						tile1.TileType = 80;
						WorldGen.SquareTileFrame(num2, num - 1);
						if (Main.netMode == NetmodeID.Server)
							NetMessage.SendTileSquare(-1, num2, num - 1);
					}

					return;
				}

				bool flag = false;
				bool flag2 = false;
				if (tile1.HasTile && tile1.TileType == 80) {
					if (!tile3.HasTile && !Main.tile[num2 - 2, num + 1].HasTile && !Main.tile[num2 - 1, num - 1].HasTile && !Main.tile[num2 - 1, num + 1].HasTile && !Main.tile[num2 - 2, num].HasTile)
						flag = true;

					if (!tile4.HasTile && !Main.tile[num2 + 2, num + 1].HasTile && !Main.tile[num2 + 1, num - 1].HasTile && !Main.tile[num2 + 1, num + 1].HasTile && !Main.tile[num2 + 2, num].HasTile)
						flag2 = true;
				}

				int num12 = WorldGen.genRand.Next(3);
				if (num12 == 0 && flag) {
					tile3.HasTile = true;
					tile3.TileType = 80;
					WorldGen.SquareTileFrame(num2 - 1, num);
					if (Main.netMode == NetmodeID.Server)
						NetMessage.SendTileSquare(-1, num2 - 1, num);
				}
				else if (num12 == 1 && flag2) {
					tile4.HasTile = true;
					tile4.TileType = 80;
					WorldGen.SquareTileFrame(num2 + 1, num);
					if (Main.netMode == 2)
						NetMessage.SendTileSquare(-1, num2 + 1, num);
				}
				else {
					if (num6 >= WorldGen.genRand.Next(2, 8))
						return;

					if ((!Main.tile[num2 + 1, num - 1].HasTile || Main.tile[num2 + 1, num - 1].TileType != 80) && !tile1.HasTile) {
						tile1.HasTile = true;
						tile1.TileType = 80;
						WorldGen.SquareTileFrame(num2, num - 1);
						if (Main.netMode == NetmodeID.Server)
							NetMessage.SendTileSquare(-1, num2, num - 1);
					}
				}
			}
			else if (!tile1.HasTile && !Main.tile[num2, num - 2].HasTile && !Main.tile[num2 + num7, num - 1].HasTile && Main.tile[num2 - num7, num - 1].HasTile && Main.tile[num2 - num7, num - 1].TileType == 80) {
				tile1.HasTile = true;
				tile1.TileType = 80;
				WorldGen.SquareTileFrame(num2, num - 1);
				if (Main.netMode == NetmodeID.Server)
					NetMessage.SendTileSquare(-1, num2, num - 1);
			}
		}
	}
}