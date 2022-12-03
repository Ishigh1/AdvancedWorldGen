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
		float num797 = (Main.maxTilesX - 200) / (float)count;
		List<Point> list2 = new(count);
		int num798 = 0;
		int tries = 0;
		while (tries < count)
		{
			float progress = tries / (float)count;
			Progress.Set(progress);
			Point point3 = WorldGen.RandomRectanglePoint((int)(progress * (Main.maxTilesX - 200)) + 100,
				(int)WorldGen.rockLayer + 20, (int)num797, Main.maxTilesY - ((int)WorldGen.rockLayer + 40) - 200);
			while (point3.X > Main.maxTilesX * 0.45 && point3.X < Main.maxTilesX * 0.55)
			{
				point3.X = WorldGen.genRand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);
			}

			num798++;
			if (GraniteBiome.CanPlace(point3, WorldGen.structures))
			{
				list2.Add(point3);
				tries++;
			}
			else if (num798 > Main.maxTilesX)
			{
				tries++;
				num798 = 0;
			}
		}

		GraniteBiome graniteBiome = WorldGen.configuration.CreateBiome<GraniteBiome>();
		for (int num801 = 0; num801 < count; num801++)
		{
			graniteBiome.Place(list2[num801], WorldGen.structures);
		}
	}
}