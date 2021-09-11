using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;
using VanillaJunglePass = Terraria.GameContent.Biomes.JunglePass;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Jungle
{
	public class JunglePass : ControlledWorldGenPass
	{
		public int DungeonSide;
		public int JungleOriginX;
		public int LeftBeachEnd;
		public int RightBeachStart;
		public float WorldScale;
		public double WorldSurface;

		public JunglePass() : base("Jungle", 10154.652f)
		{
		}

		protected override void ApplyPass()
		{
			Progress.Message = Language.GetTextValue("LegacyWorldGen.11");

			JungleOriginX = Replacer.VanillaInterface.JungleOriginX.Value;
			DungeonSide = Replacer.VanillaInterface.DungeonSide.Value;
			WorldSurface = WorldGen.worldSurface;
			LeftBeachEnd = Replacer.VanillaInterface.LeftBeachEnd.Value;
			RightBeachStart = Replacer.VanillaInterface.RightBeachStart.Value;

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
			int num = Random.Next((int) (400f * WorldScale), (int) (600f * WorldScale));
			int num2 = (int) (25f * WorldScale);
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
				int i = x + Random.Next(-(int) (125f * WorldScale), (int) (125f * WorldScale));
				int j = y + Random.Next(-(int) (125f * WorldScale), (int) (125f * WorldScale));
				int strength = Random.Next(3, 7);
				int steps = Random.Next(3, 8);
				int type = Random.Next(baseGem, baseGem + gemVariants);
				WorldGen.TileRunner(i, j, strength, steps, type);
			}
		}

		public void PlaceFirstPassMud(int x, int y, int xSpeedScale)
		{
			WorldGen.mudWall = true;
			WorldGen.TileRunner(x, y, Random.Next((int) (250f * WorldScale), (int) (500f * WorldScale)),
				Random.Next(50, 150), 59, false, DungeonSide * xSpeedScale);
			WorldGen.mudWall = false;
		}

		public Point CreateStartPoint() => new(JungleOriginX, (int) (Main.maxTilesY + Main.rockLayer) / 2);

		public void ApplyRandomMovement(ref int x, ref int y, int xRange, int yRange)
		{
			x += Random.Next((int) (-xRange * WorldScale), 1 + (int) (xRange * WorldScale));
			y += Random.Next((int) (-yRange * WorldScale), 1 + (int) (yRange * WorldScale));
			y = Utils.Clamp(y, (int) Main.rockLayer, Main.UnderworldLayer);
		}

		public void GenerateTunnelToSurface(int x, int y)
		{
			double num = Random.Next(5, 11);
			Vector2 vector = new()
			{
				X = x,
				Y = y
			};
			Vector2 vector2 = new()
			{
				X = Random.Next(-10, 11) * 0.1f,
				Y = Random.Next(10, 20) * 0.1f
			};
			int num2 = 0;
			bool flag = true;
			while (flag)
			{
				if (vector.Y < Main.worldSurface)
				{
					if (WorldGen.drunkWorldGen)
						flag = false;

					int value = (int) vector.X;
					int value2 = (int) vector.Y;
					value = Utils.Clamp(value, 10, Main.maxTilesX - 10);
					value2 = Utils.Clamp(value2, 10, Main.maxTilesY - 10);
					if (value2 < 5)
						value2 = 5;

					if (Main.tile[value, value2].wall == 0 && !Main.tile[value, value2].IsActive &&
					    Main.tile[value, value2 - 3].wall == 0 && !Main.tile[value, value2 - 3].IsActive &&
					    Main.tile[value, value2 - 1].wall == 0 && !Main.tile[value, value2 - 1].IsActive &&
					    Main.tile[value, value2 - 4].wall == 0 && !Main.tile[value, value2 - 4].IsActive &&
					    Main.tile[value, value2 - 2].wall == 0 && !Main.tile[value, value2 - 2].IsActive &&
					    Main.tile[value, value2 - 5].wall == 0 && !Main.tile[value, value2 - 5].IsActive)
						flag = false;
				}

				num += Random.Next(-20, 21) * 0.1f;
				if (num < 5.0)
					num = 5.0;

				if (num > 10.0)
					num = 10.0;

				int value3 = (int) (vector.X - num * 0.5);
				int value4 = (int) (vector.X + num * 0.5);
				int value5 = (int) (vector.Y - num * 0.5);
				int value6 = (int) (vector.Y + num * 0.5);
				int num3 = Utils.Clamp(value3, 10, Main.maxTilesX - 10);
				value4 = Utils.Clamp(value4, 10, Main.maxTilesX - 10);
				value5 = Utils.Clamp(value5, 10, Main.maxTilesY - 10);
				value6 = Utils.Clamp(value6, 10, Main.maxTilesY - 10);
				for (int k = num3; k < value4; k++)
				for (int l = value5; l < value6; l++)
					if (Math.Abs(k - vector.X) + Math.Abs(l - vector.Y) <
					    num * 0.5 * (1.0 + Random.Next(-10, 11) * 0.015))
						WorldGen.KillTile(k, l);

				num2++;
				if (num2 > 10 && Random.Next(50) < num2)
				{
					num2 = 0;
					int num4 = -2;
					if (Random.NextBool(2))
						num4 = 2;

					WorldGen.TileRunner((int) vector.X, (int) vector.Y, Random.Next(3, 20), Random.Next(10, 100), -1,
						false, num4);
				}

				vector += vector2;
				vector2.Y += Random.Next(-10, 11) * 0.01f;
				if (vector2.Y > 0f)
					vector2.Y = 0f;

				if (vector2.Y < -2f)
					vector2.Y = -2f;

				vector2.X += Random.Next(-10, 11) * 0.1f;
				if (vector.X < x - 200)
					vector2.X += Random.Next(5, 21) * 0.1f;

				if (vector.X > x + 200)
					vector2.X -= Random.Next(5, 21) * 0.1f;

				if (vector2.X > 1.5)
					vector2.X = 1.5f;

				if (vector2.X < -1.5)
					vector2.X = -1.5f;
			}
		}

		public void GenerateHolesInMudWalls()
		{
			int minX = Replacer.VanillaInterface.JungleMinX;
			int maxX = Replacer.VanillaInterface.JungleMaxX;
			for (int i = 0; i < Main.maxTilesX / 4; i++)
			{
				int x = Random.Next(minX, maxX);
				int y = Random.Next((int) WorldSurface + 10, Main.UnderworldLayer);
				if (Main.tile[x, y].wall is WallID.JungleUnsafe or WallID.MudUnsafe)
					WorldGen.MudWallRunner(x, y);
			}
		}

		public void DelimitJungle()
		{
			int y = (int) ((WorldGen.rockLayer + Main.UnderworldLayer) / 2);
			int x = JungleOriginX;
			Tile tile = Main.tile[x, y];
			while (!tile.IsActive || tile.type is TileID.Mud or TileID.JungleGrass)
			{
				x--;
				tile = Main.tile[x, y];
			}
			Replacer.VanillaInterface.JungleMinX = x + 1;
			
			x = JungleOriginX;
			tile = Main.tile[x, y];
			while (!tile.IsActive || tile.type is TileID.Mud or TileID.JungleGrass)
			{
				x++;
				tile = Main.tile[x, y];
			}
			Replacer.VanillaInterface.JungleMaxX = x - 1;
		}

		public void GenerateFinishingTouches(GenerationProgress progress, int oldX, int oldY)
		{
			int x = oldX;
			int y = oldY;
			float worldScale = WorldScale;
			for (int i = 0; i <= 20f * worldScale; i++)
			{
				progress.Set((60f + i / worldScale) * 0.01f);
				x += Random.Next((int) (-5f * worldScale), (int) (6f * worldScale));
				y += Random.Next((int) (-5f * worldScale), (int) (6f * worldScale));
				WorldGen.TileRunner(x, y, Random.Next(40, 100), Random.Next(300, 500), 59);
			}

			for (int j = 0; j <= 10f * worldScale; j++)
			{
				progress.Set((80f + j / worldScale * 2f) * 0.01f);
				x = oldX + Random.Next((int) (-600f * worldScale), (int) (600f * worldScale));
				y = oldY + Random.Next((int) (-200f * worldScale), (int) (200f * worldScale));
				while (x < 1 || x >= Main.maxTilesX - 1 || y < 1 || y >= Main.maxTilesY - 1 ||
				       Main.tile[x, y].type != 59)
				{
					x = oldX + Random.Next((int) (-600f * worldScale), (int) (600f * worldScale));
					y = oldY + Random.Next((int) (-200f * worldScale), (int) (200f * worldScale));
				}

				for (int k = 0; k < 8f * worldScale; k++)
				{
					x += Random.Next(-30, 31);
					y += Random.Next(-30, 31);
					int type = -1;
					if (Random.NextBool(7))
						type = -2;

					WorldGen.TileRunner(x, y, Random.Next(10, 20), Random.Next(30, 70), type);
				}
			}

			for (int _ = 0; _ <= 300f * worldScale; _++)
			{
				x = oldX + Random.Next((int) (-600f * worldScale), (int) (600f * worldScale));
				y = oldY + Random.Next((int) (-200f * worldScale), (int) (200f * worldScale));
				while (x < 1 || x >= Main.maxTilesX - 1 || y < 1 || y >= Main.maxTilesY - 1 ||
				       Main.tile[x, y].type != 59)
				{
					x = oldX + Random.Next((int) (-600f * worldScale), (int) (600f * worldScale));
					y = oldY + Random.Next((int) (-200f * worldScale), (int) (200f * worldScale));
				}

				WorldGen.TileRunner(x, y, Random.Next(4, 10), Random.Next(5, 30), 1);
				if (Random.NextBool(4))
				{
					int type = Random.Next(TileID.Sapphire, TileID.JungleThorns);
					WorldGen.TileRunner(x + Random.Next(-1, 2), y + Random.Next(-1, 2), Random.Next(3, 7),
						Random.Next(4, 8), type);
				}
			}
		}
	}
}