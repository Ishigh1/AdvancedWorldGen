using System;
using Terraria;
using Terraria.ID;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class ShellPiles : ControlledWorldGenPass
	{
		public ShellPiles() : base("Shell Piles", 1f)
		{
		}

		protected override void ApplyPass()
		{
			float worldSize = Main.maxTilesX < 4200 ? Main.maxTilesX / 4200f : 1;
			if (WorldGen.genRand.Next(2) == 0)
			{
				int baseShellStartXLeft = Replacer.VanillaInterface.ShellStartXLeft.Value;
				int baseShellStartYLeft = Replacer.VanillaInterface.ShellStartYLeft.Value;
				int shellStartXLeft = baseShellStartXLeft;
				int shellStartYLeft = baseShellStartYLeft;

				int xMin = Math.Max(5, baseShellStartXLeft - 20);
				int xMax = Math.Min(Main.maxTilesX - 6, baseShellStartXLeft + 20);
				int yMin = Math.Max(5, baseShellStartYLeft - 10);
				int yMax = Math.Min(Main.UnderworldLayer, baseShellStartYLeft + 10);
				
				for (int x = xMin; x <= xMax; x++)
				for (int y = yMin; y <= yMax; y++)
				{
					Tile tile = Main.tile[x, y];
					Tile tileAbove = Main.tile[x, y - 1];
					Tile tileLeft = Main.tile[x - 1, y];
					if (tile.IsActive && tile.type == TileID.Sand &&
					    !tileAbove.IsActive && tileAbove.LiquidAmount == 0 &&
					    !tileLeft.IsActive && tileLeft.LiquidAmount > 0)
					{
						shellStartXLeft = x;
						shellStartYLeft = y;
					}
				}

				shellStartYLeft -= 50;
				shellStartXLeft -= WorldGen.genRand.Next(5);
				if (WorldGen.genRand.Next(2) == 0)
					shellStartXLeft -= WorldGen.genRand.Next(10);

				if (WorldGen.genRand.Next(3) == 0)
					shellStartXLeft -= WorldGen.genRand.Next(15);

				if (WorldGen.genRand.Next(4) != 0) WorldGen.ShellPile(shellStartXLeft, shellStartYLeft);

				int shellRarity = WorldGen.genRand.Next(2, 4);
				if (WorldGen.genRand.Next(shellRarity) == 0)
					WorldGen.ShellPile((int) (shellStartXLeft - WorldGen.genRand.Next(10, 35) * worldSize), shellStartYLeft);

				if (WorldGen.genRand.Next(shellRarity) == 0)
					WorldGen.ShellPile((int) (shellStartXLeft - WorldGen.genRand.Next(40, 65) * worldSize), shellStartYLeft);

				if (WorldGen.genRand.Next(shellRarity) == 0)
					WorldGen.ShellPile((int) (shellStartXLeft - WorldGen.genRand.Next(70, 95) * worldSize), shellStartYLeft);

				if (WorldGen.genRand.Next(shellRarity) == 0)
					WorldGen.ShellPile((int) (shellStartXLeft - WorldGen.genRand.Next(100, 125) * worldSize), shellStartYLeft);

				if (WorldGen.genRand.Next(shellRarity) == 0)
					WorldGen.ShellPile((int) (shellStartXLeft + WorldGen.genRand.Next(10, 25) * worldSize), shellStartYLeft);
			}

			if (WorldGen.genRand.Next(2) == 0)
			{
				int baseShellStartXRight = Replacer.VanillaInterface.ShellStartXRight.Value;
				int baseShellStartYRight = Replacer.VanillaInterface.ShellStartYRight.Value;
				int shellStartXRight = baseShellStartXRight;
				int shellStartYRight = baseShellStartYRight;

				int xMin = Math.Max(5, baseShellStartXRight - 20);
				int xMax = Math.Min(Main.maxTilesX - 6, baseShellStartXRight + 20);
				int yMin = Math.Max(5, baseShellStartYRight - 10);
				int yMax = Math.Min(Main.UnderworldLayer, baseShellStartYRight + 10);

				for (int x = xMin; x <= xMax; x++)
				for (int y = yMin; y <= yMax; y++)
				{
					Tile tile = Main.tile[x, y];
					Tile tileAbove = Main.tile[x, y - 1];
					Tile tileRight = Main.tile[x + 1, y];
					if (tile.IsActive && tile.type == TileID.Sand &&
					    !tileAbove.IsActive && tileAbove.LiquidAmount == 0 &&
					    !tileRight.IsActive && tileRight.LiquidAmount > 0)
					{
						shellStartXRight = x;
						shellStartYRight = y;
					}
				}

				shellStartYRight -= 50;
				shellStartXRight += WorldGen.genRand.Next(5);
				if (WorldGen.genRand.Next(2) == 0)
					shellStartXRight += WorldGen.genRand.Next(10);

				if (WorldGen.genRand.Next(3) == 0)
					shellStartXRight += WorldGen.genRand.Next(15);

				if (WorldGen.genRand.Next(4) != 0) WorldGen.ShellPile(shellStartXRight, shellStartYRight);

				int shellRarity = WorldGen.genRand.Next(2, 4);
				if (WorldGen.genRand.Next(shellRarity) == 0)
					WorldGen.ShellPile((int) (shellStartXRight + WorldGen.genRand.Next(10, 35) * worldSize), shellStartYRight);

				if (WorldGen.genRand.Next(shellRarity) == 0)
					WorldGen.ShellPile((int) (shellStartXRight + WorldGen.genRand.Next(40, 65) * worldSize), shellStartYRight);

				if (WorldGen.genRand.Next(shellRarity) == 0)
					WorldGen.ShellPile((int) (shellStartXRight + WorldGen.genRand.Next(70, 95) * worldSize), shellStartYRight);

				if (WorldGen.genRand.Next(shellRarity) == 0)
					WorldGen.ShellPile((int) (shellStartXRight + WorldGen.genRand.Next(100, 125) * worldSize), shellStartYRight);

				if (WorldGen.genRand.Next(shellRarity) == 0)
					WorldGen.ShellPile((int) (shellStartXRight - WorldGen.genRand.Next(10, 25) * worldSize), shellStartYRight);
			}
		}
	}
}