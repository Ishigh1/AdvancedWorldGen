using Terraria;
using Terraria.GameContent.Biomes;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class Jungle
	{
		public static void GenerateHolesInMudWalls(JunglePass self)
		{
			const int smallSize = 4200;
			int holes = Main.maxTilesX * Main.maxTilesX / (2 * smallSize);
			for (int i = 0; i < holes; i++)
			{
				int x = WorldGen.genRand.Next(self.JungleX - Main.maxTilesX / 8, self.JungleX + Main.maxTilesX / 8);
				int y = WorldGen.genRand.Next((int) self.WorldSurface + 10, Main.UnderworldLayer);
				if (Main.tile[x, y].wall == 64 || Main.tile[x, y].wall == 15)
					WorldGen.MudWallRunner(x, y);
			}
		}
	}
}