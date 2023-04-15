namespace AdvancedWorldGen.SpecialOptions;

public class SortedWorld : ControlledWorldGenPass
{
	public SortedWorld() : base("Sorted World", 10f)
	{
	}

	protected override void ApplyPass()
	{
		Dictionary<int, int> heights = new();
		for (int i = 5; i < Main.maxTilesX - 5; i++)
		{
			int j;
			for (j = 5; j < Main.maxTilesY - 5; j++)
				if (WorldGen.SolidTile(i, j))
					break;

			heights.Add(i, j);
		}

		Dictionary<int, int> ordered = new();
		int targetX = 5;
		foreach ((int x, int _) in heights.OrderByDescending(key => key.Value)) ordered.Add(targetX++, x);

		while (ordered.Count > 0)
		{
			(targetX, int nextX) = ordered.ElementAt(0);
			ordered.Remove(targetX);

			if (nextX != targetX)
			{
				for (int i = 5; i < Main.maxTilesY - 5; i++) Main.tile[0, i].CopyFrom(Main.tile[nextX, i]);

				int targetX2 = nextX;
				while (targetX2 != targetX)
				{
					int source = ordered[targetX2];
					ordered.Remove(targetX2);

					for (int i = 5; i < Main.maxTilesY - 5; i++) Main.tile[targetX2, i].CopyFrom(Main.tile[source, i]);

					targetX2 = source;
				}

				for (int i = 5; i < Main.maxTilesY - 5; i++) Main.tile[targetX, i].CopyFrom(Main.tile[0, i]);
			}
		}
	}
}