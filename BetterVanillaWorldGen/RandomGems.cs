using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class RandomGems : ControlledWorldGenPass
	{
		public RandomGems() : base("Random Gems", 18.4925f)
		{
		}

		protected override void ApplyPass()
		{
			Progress.Set(1f);
			int yMax = Main.maxTilesY - 300 > Main.rockLayer ? Main.maxTilesY - 300 : Main.UnderworldLayer;
			for (int _ = 0; _ < Main.maxTilesX; _++)
			{
				int x = Random.Next(20, Main.maxTilesX - 20);
				int y = Random.Next((int) Main.rockLayer, yMax);
				Tile tile = Main.tile[x, y];
				if (!tile.IsActive && tile.LiquidType != LiquidID.Lava &&
				    !Main.wallDungeon[tile.wall] && tile.wall != WallID.Planked)
				{
					int style = Random.Next(12) switch
					{
						< 3 => 0,
						< 6 => 1,
						< 8 => 2,
						< 10 => 3,
						>= 11 => 5,
						_ => 4
					};
					WorldGen.PlaceTile(x, y, TileID.ExposedGems, true, false, -1, style);
				}
			}

			for (int _ = 0; _ < Main.maxTilesX; _++)
			{
				int x = Random.Next(20, Main.maxTilesX - 20);
				int y = Random.Next((int) Main.worldSurface, yMax);
				Tile tile = Main.tile[x, y];
				if (!tile.IsActive && tile.LiquidType != LiquidID.Lava && tile.wall is WallID.HardenedSand or WallID.Sandstone)
				{
					int num169 = Random.Next(1, 4);
					int num170 = Random.Next(1, 4);
					int num171 = Random.Next(1, 4);
					int num172 = Random.Next(1, 4);
					for (int xx = x - num169; xx < x + num170; xx++)
					for (int yy = y - num171; yy < y + num172; yy++)
						if (!Main.tile[xx, yy].IsActive)
							WorldGen.PlaceTile(xx, yy, TileID.ExposedGems, true, false, -1, 6);
				}
			}
		}
	}
}