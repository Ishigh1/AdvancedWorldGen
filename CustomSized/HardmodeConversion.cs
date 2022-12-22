namespace AdvancedWorldGen.CustomSized;

public static class HardmodeConversion
{
	public static void ReplaceHardmodeConversion(On_WorldGen.orig_GERunner orig, int x, int y, double speedX,
		double speedY, bool good)
	{
		bool calamity = false;

		if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
		{
			Type type = calamityMod.GetType("CalamityMod.CalamityConfig")!;
			object config = type.GetFieldValue("Instance")!;
			calamity = (bool)type.GetPropertyValue("EarlyHardmodeProgressionRework", config)!;
		}

		if (!WorldgenSettings.Revamped || calamity)
		{
			orig(x, y, speedX, speedY, good);
			return;
		}


		float worldSize = Main.maxTilesX / 4200f;
		float num2 = WorldGen.genRand.Next(200, 250) * worldSize;
		Vector2D vector = default;
		vector.X = x;
		vector.Y = y;
		Vector2D vector2 = default;
		vector2.X = WorldGen.genRand.Next(-10, 11) * 0.1f;
		vector2.Y = WorldGen.genRand.Next(-10, 11) * 0.1f;
		if (speedX != 0f || speedY != 0f)
		{
			vector2.X = speedX;
			vector2.Y = speedY;
		}

		bool flag2 = true;
		while (flag2)
		{
			int num5 = Math.Max((int)(vector.X - num2 * 0.5), 0);
			int num6 = Math.Min((int)(vector.X + num2 * 0.5), Main.maxTilesX);
			int num7 = Math.Max((int)(vector.Y - num2 * 0.5), 0);
			int num8 = Math.Min((int)(vector.Y + num2 * 0.5), Main.maxTilesY - 5);

			for (int m = num5; m < num6; m++)
			for (int n = num7; n < num8; n++)
			{
				if (!(Math.Abs(m - vector.X) + Math.Abs(n - vector.Y) <
				      num2 * 0.5 * (1.0 + WorldGen.genRand.Next(-10, 11) * 0.015)))
					continue;

				if (good)
				{
					switch (Main.tile[m, n].WallType)
					{
						case 63:
						case 65:
						case 66:
						case 68:
						case 69:
						case 81:
							Main.tile[m, n].WallType = 70;
							break;
						case 216:
							Main.tile[m, n].WallType = 219;
							break;
						case 187:
							Main.tile[m, n].WallType = 222;
							break;
					}

					if (Main.tile[m, n].WallType is 3 or 83)
						Main.tile[m, n].WallType = 28;

					switch (Main.tile[m, n].TileType)
					{
						case 225 when WorldGen.notTheBees:
							Main.tile[m, n].TileType = 117;
							break;
						case 230 when WorldGen.notTheBees:
							Main.tile[m, n].TileType = 402;
							break;
						case 2:
							Main.tile[m, n].TileType = 109;
							break;
						case 1:
							case 25:
								case 203:
							Main.tile[m, n].TileType = 117;
							break;
						case 53:
						case 123:
							case 234:
							Main.tile[m, n].TileType = 116;
							break;
						case 661:
							case 662:
							Main.tile[m, n].TileType = 60;
							break;
						case 23:
						case 199:
							Main.tile[m, n].TileType = 109;
							break;
						case 161:
						case 163:
						case 200:
							Main.tile[m, n].TileType = 164;
							break;
						case 396:
							Main.tile[m, n].TileType = 403;
							break;
						case 397:
							Main.tile[m, n].TileType = 402;
							break;
						default:
							goto skipTileFrame;
					}
					WorldGen.SquareTileFrame(m, n);
				}
				else if (WorldGen.crimson)
				{
					switch (Main.tile[m, n].WallType)
					{
						case 63:
						case 65:
						case 66:
						case 68:
							Main.tile[m, n].WallType = 81;
							break;
						case 216:
							Main.tile[m, n].WallType = 218;
							break;
						case 187:
							Main.tile[m, n].WallType = 221;
							break;
					}

					switch (Main.tile[m, n].TileType)
					{
						case 225 when WorldGen.notTheBees:
							Main.tile[m, n].TileType = 203;
							break;
						case 230 when WorldGen.notTheBees:
							Main.tile[m, n].TileType = 399;
							break;
						case 60:
							case 661:
								
							Main.tile[m, n].TileType = 662;
							break;
						case 2:
							case 109:
							Main.tile[m, n].TileType = 199;
							break;
						case 1:
							case 117:
							Main.tile[m, n].TileType = 203;
							break;
						case 53:
						case 123:
						case 116:
							Main.tile[m, n].TileType = 234;
							break;
						case 161:
						case 164:
							Main.tile[m, n].TileType = 200;
							break;
						case 396:
							Main.tile[m, n].TileType = 401;
							break;
						case 397:
							Main.tile[m, n].TileType = 399;
							break;
						default:
							goto skipTileFrame;
					}
					WorldGen.SquareTileFrame(m, n);
				}
				else
				{
					switch (Main.tile[m, n].WallType)
					{
						case 63:
						case 65:
						case 66:
						case 68:
							Main.tile[m, n].WallType = 69;
							break;
						case 216:
							Main.tile[m, n].WallType = 217;
							break;
						case 187:
							Main.tile[m, n].WallType = 220;
							break;
					}

					switch (Main.tile[m, n].TileType)
					{
						case 225 when WorldGen.notTheBees:
						case 1:
						case 117:
							Main.tile[m, n].TileType = 25;
							break;
						case 230 when WorldGen.notTheBees:
						case 397:
							Main.tile[m, n].TileType = 398;
							break;
						case 60:
							case 662:
								
							Main.tile[m, n].TileType = 661;
							break;
						case 2:
						case 109:
							Main.tile[m, n].TileType = 23;
							break;
						case 53:
						case 123:
						case 116:
							Main.tile[m, n].TileType = 112;
							break;
						case 161:
						case 164:
							Main.tile[m, n].TileType = 163;
							break;
						case 396:
							Main.tile[m, n].TileType = 400;
							break;
						default:
							goto skipTileFrame;
					}
					WorldGen.SquareTileFrame(m, n);
				}
			}
			skipTileFrame:

			vector += vector2;
			vector2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
			if (vector2.X > speedX + 1f)
				vector2.X = speedX + 1f;

			if (vector2.X < speedX - 1f)
				vector2.X = speedX - 1f;

			if (vector.X < -num2 || vector.Y < -num2 ||
			    vector.X > Main.maxTilesX + num2 || vector.Y > Main.maxTilesY + num2)
				flag2 = false;
		}
	}
}