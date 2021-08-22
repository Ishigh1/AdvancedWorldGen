using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace AdvancedWorldGen.SpecialOptions.Halloween.Worldgen
{
	public class Utilities
	{
		public static float WorldSize => Main.maxTilesX / 4200f;

		public static (int x, int y) FindSuitableSpotForSurfacePressurePlate(int direction, int x)
		{
			int y = FindGround(x);
			while (Main.tile[x, y].Slope != SlopeType.Solid || Main.tile[x, y].IsHalfBlock)
			{
				if (x <= WorldGen.beachDistance)
					direction = 1;
				else if (x >= Main.maxTilesX - WorldGen.beachDistance) direction = -1;

				if (Main.tile[x, y - 1].IsActive && Main.tile[x, y - 1].type == TileID.PressurePlates)
					return FindSuitableSpotForSurfacePressurePlate(direction,
						x + Main.rand.Next(2 * direction, 10 * direction));
				x += direction;
				GoAtTop(x, ref y);
			}

			Tile tile = Main.tile[x, y];
			if (Main.tile[x - 1, y].IsActive && Main.tileSolid[Main.tile[x - 1, y].type] &&
			    Main.tile[x + 1, y].IsActive && Main.tileSolid[Main.tile[x + 1, y].type] ||
			    tile.wall != 0 || tile.type == TileID.LeafBlock || Main.tile[x, y - 1].LiquidAmount > 0)
				return FindSuitableSpotForSurfacePressurePlate(direction, x + direction);
			y--;
			return (x, y);
		}

		public static bool IsSuitableForTraps(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			return tile.IsActive && Main.tileSolid[tile.type] && !tile.IsHalfBlock && tile.Slope == SlopeType.Solid;
		}

		public static bool IsValidTop(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			return tile.IsActive && Main.tileSolid[tile.type];
		}

		public static void GoAtTop(int x, ref int y)
		{
			while (IsValidTop(x, --y))
			{
			}

			while (!IsValidTop(x, ++y))
			{
			}
		}

		public static int FindGround(int x)
		{
			int y = 100;
			while (true)
			{
				if (IsValidTop(x, y))
				{
					if (!IsIsland(x, y))
						break;
					y += 10;
				}

				y++;
			}

			return y;
		}

		public static bool IsIsland(int x, int y)
		{
			for (int i = 0; i < 50; i++)
			{
				int type = Main.tile[x, y + i].type;
				if (type == TileID.Cloud || type == TileID.RainCloud || type == TileID.SnowCloud) return true;
			}

			return false;
		}

		public static bool IsValidStructure(int x, int y, int width, int height, int padding)
		{
			if (!WorldGen.structures.CanPlace(new Rectangle(x, y, width, height),
				padding)) return false;
			WorldGen.structures.AddStructure(new Rectangle(x, y, width, height),
				padding);
			return true;
		}
	}
}