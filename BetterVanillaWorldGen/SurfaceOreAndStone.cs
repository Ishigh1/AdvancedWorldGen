namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class SurfaceOreAndStone : ControlledWorldGenPass
{
	public SurfaceOreAndStone() : base("Surface Ore and Stone", 64.9990f)
	{
	}

	protected override void ApplyPass()
	{
		int num360 = WorldGen.genRand.Next(Main.maxTilesX / (4200 / 5), Main.maxTilesX / (4200 / 10));
		List<int> orePatchesX = new();

		for (int num361 = 0; num361 < num360; num361++)
		{
			Progress.Set(num361, num360, 0.5f);
			int num362 = Main.maxTilesX / 420;
			while (num362 > 0)
			{
				num362--;
				int x = WorldGen.genRand.Next(WorldGen.beachDistance, (int)(Main.maxTilesX * 0.48f));
				if (WorldGen.genRand.NextBool(2))
					x = Main.maxTilesX - x;

				int y3 = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, (int)WorldGen.worldSurface);
				bool flag19 = orePatchesX.Any(patchX => Math.Abs(patchX - x) < 200);

				if (!flag19 && WorldGen.OrePatch(x, y3))
				{
					orePatchesX.Add(x);

					break;
				}
			}
		}

		int maxStonePatches = Main.maxTilesX / (4200 / 7);
		num360 = maxStonePatches > 1 ? WorldGen.genRand.Next(1, maxStonePatches) : 1;
		for (int num365 = 0; num365 < num360; num365++)
		{
			Progress.Set(num365, num360, 0.5f, 0.5f);
			int num366 = Main.maxTilesX / 420;
			while (num366 > 0)
			{
				num366--;
				int x = WorldGen.genRand.Next(WorldGen.beachDistance, (int)(Main.maxTilesX * 0.47f));
				if (WorldGen.genRand.NextBool(2))
					x = Main.maxTilesX - x;

				int y4 = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, (int)WorldGen.worldSurface);
				bool flag20 = orePatchesX.Any(patchX => Math.Abs(patchX - x) < 100);

				if (!flag20 && WorldGen.StonePatch(x, y4))
					break;
			}
		}
	}
}