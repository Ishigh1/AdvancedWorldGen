using System;
using AdvancedWorldGen.BetterVanillaWorldGen;
using Microsoft.Xna.Framework;
using Terraria;
using OnWorldGen = On.Terraria.WorldGen;

namespace AdvancedWorldGen.CustomSized;

public static class HardmodeConversion
{
	public static void ReplaceHardmodeConversion(OnWorldGen.orig_GERunner orig, int x, int y, float speedX,
		float speedY, bool good)
	{
		if (!WorldgenSettings.Revamped)
		{
			orig(x, y, speedX, speedY, good);
			return;
		}

		float worldSize = Main.maxTilesX / 4200f;
		float num2 = WorldGen.genRand.Next(200, 250) * worldSize;
		Vector2 vector = default;
		vector.X = x;
		vector.Y = y;
		Vector2 vector2 = default;
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
			int num5 = (int)(vector.X - num2 * 0.5);
			int num6 = (int)(vector.X + num2 * 0.5);
			int num7 = (int)(vector.Y - num2 * 0.5);
			int num8 = (int)(vector.Y + num2 * 0.5);
			if (num5 < 0)
				num5 = 0;

			if (num6 > Main.maxTilesX)
				num6 = Main.maxTilesX;

			if (num7 < 0)
				num7 = 0;

			if (num8 > Main.maxTilesY - 5)
				num8 = Main.maxTilesY - 5;

			for (int m = num5; m < num6; m++)
			for (int n = num7; n < num8; n++)
			{
				if (!(Math.Abs(m - vector.X) + Math.Abs(n - vector.Y) <
				      num2 * 0.5 * (1.0 + WorldGen.genRand.Next(-10, 11) * 0.015)))
					continue;

				if (good)
				{
					switch (Main.tile[m, n].wall)
					{
						case 63:
						case 65:
						case 66:
						case 68:
						case 69:
						case 81:
							Main.tile[m, n].wall = 70;
							break;
						case 216:
							Main.tile[m, n].wall = 219;
							break;
						case 187:
							Main.tile[m, n].wall = 222;
							break;
					}

					if (Main.tile[m, n].wall is 3 or 83)
						Main.tile[m, n].wall = 28;

					switch (Main.tile[m, n].type)
					{
						case 225 when WorldGen.notTheBees:
							Main.tile[m, n].type = 117;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 230 when WorldGen.notTheBees:
							Main.tile[m, n].type = 402;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 2:
							Main.tile[m, n].type = 109;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 1:
							Main.tile[m, n].type = 117;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 53:
						case 123:
							Main.tile[m, n].type = 116;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 23:
						case 199:
							Main.tile[m, n].type = 109;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 25:
						case 203:
							Main.tile[m, n].type = 117;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 112:
						case 234:
							Main.tile[m, n].type = 116;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 161:
						case 163:
						case 200:
							Main.tile[m, n].type = 164;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 396:
							Main.tile[m, n].type = 403;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 397:
							Main.tile[m, n].type = 402;
							WorldGen.SquareTileFrame(m, n);
							break;
					}
				}
				else if (WorldGen.crimson)
				{
					switch (Main.tile[m, n].wall)
					{
						case 63:
						case 65:
						case 66:
						case 68:
							Main.tile[m, n].wall = 81;
							break;
						case 216:
							Main.tile[m, n].wall = 218;
							break;
						case 187:
							Main.tile[m, n].wall = 221;
							break;
					}

					switch (Main.tile[m, n].type)
					{
						case 225 when WorldGen.notTheBees:
							Main.tile[m, n].type = 203;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 230 when WorldGen.notTheBees:
							Main.tile[m, n].type = 399;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 2:
							Main.tile[m, n].type = 199;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 1:
							Main.tile[m, n].type = 203;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 53:
						case 123:
							Main.tile[m, n].type = 234;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 109:
							Main.tile[m, n].type = 199;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 117:
							Main.tile[m, n].type = 203;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 116:
							Main.tile[m, n].type = 234;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 161:
						case 164:
							Main.tile[m, n].type = 200;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 396:
							Main.tile[m, n].type = 401;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 397:
							Main.tile[m, n].type = 399;
							WorldGen.SquareTileFrame(m, n);
							break;
					}
				}
				else
				{
					switch (Main.tile[m, n].wall)
					{
						case 63:
						case 65:
						case 66:
						case 68:
							Main.tile[m, n].wall = 69;
							break;
						case 216:
							Main.tile[m, n].wall = 217;
							break;
						case 187:
							Main.tile[m, n].wall = 220;
							break;
					}

					switch (Main.tile[m, n].type)
					{
						case 225 when WorldGen.notTheBees:
							Main.tile[m, n].type = 25;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 230 when WorldGen.notTheBees:
							Main.tile[m, n].type = 398;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 2:
							Main.tile[m, n].type = 23;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 1:
							Main.tile[m, n].type = 25;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 53:
						case 123:
							Main.tile[m, n].type = 112;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 109:
							Main.tile[m, n].type = 23;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 117:
							Main.tile[m, n].type = 25;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 116:
							Main.tile[m, n].type = 112;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 161:
						case 164:
							Main.tile[m, n].type = 163;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 396:
							Main.tile[m, n].type = 400;
							WorldGen.SquareTileFrame(m, n);
							break;
						case 397:
							Main.tile[m, n].type = 398;
							WorldGen.SquareTileFrame(m, n);
							break;
					}
				}
			}

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