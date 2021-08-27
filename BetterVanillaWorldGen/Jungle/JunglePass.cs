using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;
using VanillaJunglePass = Terraria.GameContent.Biomes.JunglePass;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Jungle
{
	public class JunglePass : GenPass
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

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Lang.gen[11].Value;

			JungleOriginX = Replacer.VanillaInterface.JungleOriginX.Get();
			DungeonSide = Replacer.VanillaInterface.DungeonSide.Get();
			WorldSurface = WorldGen.worldSurface;
			LeftBeachEnd = Replacer.VanillaInterface.LeftBeachEnd.Get();
			RightBeachStart = Replacer.VanillaInterface.RightBeachStart.Get();

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
			progress.Set(0.15f);
			ApplyRandomMovement(ref x, ref y, 250, 150);
			zero.X += x;
			zero.Y += y;
			PlaceFirstPassMud(x, y, 0);
			PlaceGemsAt(x, y, 65, 2);
			progress.Set(0.3f);
			int oldX = x;
			int oldY = y;
			ApplyRandomMovement(ref x, ref y, 400, 150);
			zero.X += x;
			zero.Y += y;
			PlaceFirstPassMud(x, y, -3);
			PlaceGemsAt(x, y, 67, 2);
			progress.Set(0.45f);
			x = zero.X / 3;
			y = zero.Y / 3;
			int num = _random.Next((int) (400f * WorldScale), (int) (600f * WorldScale));
			int num2 = (int) (25f * WorldScale);
			x = Utils.Clamp(x, LeftBeachEnd + num / 2 + num2, RightBeachStart - num / 2 - num2);
			WorldGen.mudWall = true;
			WorldGen.TileRunner(x, y, num, 10000, 59, false, 0f, -20f, true);
			GenerateTunnelToSurface(x, y);
			WorldGen.mudWall = false;
			progress.Set(0.6f);
			GenerateHolesInMudWalls();
			GenerateFinishingTouches(progress, oldX, oldY);
		}

		public void PlaceGemsAt(int x, int y, ushort baseGem, int gemVariants)
		{
			for (int i = 0; (float) i < 6f * WorldScale; i++)
				WorldGen.TileRunner(x + _random.Next(-(int) (125f * WorldScale), (int) (125f * WorldScale)),
					y + _random.Next(-(int) (125f * WorldScale), (int) (125f * WorldScale)), _random.Next(3, 7),
					_random.Next(3, 8), _random.Next(baseGem, baseGem + gemVariants));
		}

		public void PlaceFirstPassMud(int x, int y, int xSpeedScale)
		{
			WorldGen.mudWall = true;
			WorldGen.TileRunner(x, y, _random.Next((int) (250f * WorldScale), (int) (500f * WorldScale)),
				_random.Next(50, 150), 59, false, DungeonSide * xSpeedScale);
			WorldGen.mudWall = false;
		}

		public Point CreateStartPoint()
		{
			return new(JungleOriginX, (int) (Main.maxTilesY + Main.rockLayer) / 2);
		}

		public void ApplyRandomMovement(ref int x, ref int y, int xRange, int yRange)
		{
			x += _random.Next((int) (-xRange * WorldScale), 1 + (int) (xRange * WorldScale));
			y += _random.Next((int) (-yRange * WorldScale), 1 + (int) (yRange * WorldScale));
			y = Utils.Clamp(y, (int) Main.rockLayer, Main.UnderworldLayer);
		}

		public void GenerateTunnelToSurface(int x, int y)
		{
			double num = _random.Next(5, 11);
			Vector2 vector = default;
			vector.X = x;
			vector.Y = y;
			Vector2 vector2 = default;
			vector2.X = _random.Next(-10, 11) * 0.1f;
			vector2.Y = _random.Next(10, 20) * 0.1f;
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

				num += _random.Next(-20, 21) * 0.1f;
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
					    num * 0.5 * (1.0 + _random.Next(-10, 11) * 0.015))
						WorldGen.KillTile(k, l);

				num2++;
				if (num2 > 10 && _random.Next(50) < num2)
				{
					num2 = 0;
					int num4 = -2;
					if (_random.Next(2) == 0)
						num4 = 2;

					WorldGen.TileRunner((int) vector.X, (int) vector.Y, _random.Next(3, 20), _random.Next(10, 100), -1,
						false, num4);
				}

				vector += vector2;
				vector2.Y += _random.Next(-10, 11) * 0.01f;
				if (vector2.Y > 0f)
					vector2.Y = 0f;

				if (vector2.Y < -2f)
					vector2.Y = -2f;

				vector2.X += _random.Next(-10, 11) * 0.1f;
				if (vector.X < x - 200)
					vector2.X += _random.Next(5, 21) * 0.1f;

				if (vector.X > x + 200)
					vector2.X -= _random.Next(5, 21) * 0.1f;

				if (vector2.X > 1.5)
					vector2.X = 1.5f;

				if (vector2.X < -1.5)
					vector2.X = -1.5f;
			}

			Replacer.VanillaInterface.JungleX = (int) vector.X;
		}

		public void GenerateHolesInMudWalls()
		{
			for (int i = 0; i < Main.maxTilesX / 4; i++)
			{
				int num = _random.Next(20, Main.maxTilesX - 20);
				int num2 = _random.Next((int) WorldSurface + 10, Main.UnderworldLayer);
				while (Main.tile[num, num2].wall != 64 && Main.tile[num, num2].wall != 15)
				{
					num = _random.Next(20, Main.maxTilesX - 20);
					num2 = _random.Next((int) WorldSurface + 10, Main.UnderworldLayer);
				}

				WorldGen.MudWallRunner(num, num2);
			}
		}

		public void GenerateFinishingTouches(GenerationProgress progress, int oldX, int oldY)
		{
			int num = oldX;
			int num2 = oldY;
			float worldScale = WorldScale;
			for (int i = 0; (float) i <= 20f * worldScale; i++)
			{
				progress.Set((60f + i / worldScale) * 0.01f);
				num += _random.Next((int) (-5f * worldScale), (int) (6f * worldScale));
				num2 += _random.Next((int) (-5f * worldScale), (int) (6f * worldScale));
				WorldGen.TileRunner(num, num2, _random.Next(40, 100), _random.Next(300, 500), 59);
			}

			for (int j = 0; (float) j <= 10f * worldScale; j++)
			{
				progress.Set((80f + j / worldScale * 2f) * 0.01f);
				num = oldX + _random.Next((int) (-600f * worldScale), (int) (600f * worldScale));
				num2 = oldY + _random.Next((int) (-200f * worldScale), (int) (200f * worldScale));
				while (num < 1 || num >= Main.maxTilesX - 1 || num2 < 1 || num2 >= Main.maxTilesY - 1 ||
				       Main.tile[num, num2].type != 59)
				{
					num = oldX + _random.Next((int) (-600f * worldScale), (int) (600f * worldScale));
					num2 = oldY + _random.Next((int) (-200f * worldScale), (int) (200f * worldScale));
				}

				for (int k = 0; (float) k < 8f * worldScale; k++)
				{
					num += _random.Next(-30, 31);
					num2 += _random.Next(-30, 31);
					int type = -1;
					if (_random.Next(7) == 0)
						type = -2;

					WorldGen.TileRunner(num, num2, _random.Next(10, 20), _random.Next(30, 70), type);
				}
			}

			for (int l = 0; (float) l <= 300f * worldScale; l++)
			{
				num = oldX + _random.Next((int) (-600f * worldScale), (int) (600f * worldScale));
				num2 = oldY + _random.Next((int) (-200f * worldScale), (int) (200f * worldScale));
				while (num < 1 || num >= Main.maxTilesX - 1 || num2 < 1 || num2 >= Main.maxTilesY - 1 ||
				       Main.tile[num, num2].type != 59)
				{
					num = oldX + _random.Next((int) (-600f * worldScale), (int) (600f * worldScale));
					num2 = oldY + _random.Next((int) (-200f * worldScale), (int) (200f * worldScale));
				}

				WorldGen.TileRunner(num, num2, _random.Next(4, 10), _random.Next(5, 30), 1);
				if (_random.Next(4) == 0)
				{
					int type2 = _random.Next(63, 69);
					WorldGen.TileRunner(num + _random.Next(-1, 2), num2 + _random.Next(-1, 2), _random.Next(3, 7),
						_random.Next(4, 8), type2);
				}
			}
		}
	}
}