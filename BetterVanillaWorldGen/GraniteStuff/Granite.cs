namespace AdvancedWorldGen.BetterVanillaWorldGen.GraniteStuff;

public class Granite : ControlledWorldGenPass
{
	public Granite() : base("Granite", 6769.5537f)
	{
	}

	protected override void ApplyPass()
	{
		Progress.Message = Lang.gen[81].Value;
		int count = Configuration.Get<WorldGenRange>("Count").GetRandom(WorldGen.genRand);
		int num797 = (Main.maxTilesX - 200) / count;
		List<Point> list2 = new(count);
		int num798 = 0;
		int tries = 0;
		while (tries < count)
		{
			float progress = tries / (float)count;
			Progress.Set(progress);
			Point point3 = WorldGen.RandomRectanglePoint((int)(progress * (Main.maxTilesX - 200)) + 100,
				(int)GenVars.rockLayer + 20, (int)num797, Main.maxTilesY - ((int)GenVars.rockLayer + 40) - 200);
			
			if (WorldGen.remixWorldGen)
			{
				point3 = WorldGen.RandomRectanglePoint((int)(progress * (double)(Main.maxTilesX - 200)) + 100, (int)GenVars.worldSurface + 100, (int)num797, (int)GenVars.rockLayer - (int)GenVars.worldSurface - 100);
			}
			
			while (point3.X > Main.maxTilesX * 0.45 && point3.X < Main.maxTilesX * 0.55)
			{
				point3.X = WorldGen.genRand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);
			}

			num798++;
			if (GraniteBiome.CanPlace(point3, GenVars.structures))
			{
				list2.Add(point3);
				tries++;
			}
			else if (num798 > Main.maxTilesX * 10)
			{
				count = tries;
				tries++;
				num798 = 0;
			}
		}

		GraniteBiome graniteBiome = GenVars.configuration.CreateBiome<GraniteBiome>();
		
		foreach (Point point in list2)
		{
			graniteBiome.Place(point, GenVars.structures);
		}
	}
}