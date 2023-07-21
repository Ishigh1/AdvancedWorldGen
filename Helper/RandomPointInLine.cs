namespace AdvancedWorldGen.Helper;

public class RandomPointInLine
{
	private int BiomeSize;
	private int Min, Max;

	private int _weakMalus;
	public int WeakMalus
	{
		get => _weakMalus;
		set
		{
			_weakMalus = value;
			AllowedSize = -1;
		}
	}

	private List<(bool weak, int left, int right)> Blocks;
	//if weak, left should be computed as weak + WeakMalus, reversed for right. Should be deleted if the interval is 0 or less
	private List<(int left, int right)> Cache; // Stores the forbidden points in the interval
	private int AllowedSize;

	public RandomPointInLine(int biomeSize, int min, int max)
	{
		BiomeSize = biomeSize;
		Min = min;
		Max = max;
		Blocks = new List<(bool weak, int left, int right)>();
		Cache = new List<(int left, int size)>();
	}

	private void UpdateCache()
	{
		if (AllowedSize >= 0)
			return;

		Cache.Clear();
		AllowedSize = 0;

		Order();
		
		int current = Min;
		int currentId = 0;
		(int left, int right) lastValue = (-1, -1);
		int max = Max - BiomeSize;
		while (current < max && currentId < Blocks.Count)
		{
			(bool weak, int left, int right) block = Blocks[currentId];
			int left = block.left + (block.weak ? WeakMalus : 0) - BiomeSize;
			int right = block.right - (block.weak ? WeakMalus : 0);

			if (current < left)
			{
				// Add the current interval to the cache
				AllowedSize += left - current;

				// Move to the interval
				current = left;
			}
			// Current interval overlaps with the block
			else if (current >= right)
			{
				// Block is on the left
				currentId++;
			}
			else
			{
				int end = Math.Min(right, max);
				if (lastValue.right == current)
					lastValue.right = end;
				else
                {
                    Cache.Add(lastValue);
                    lastValue = (current, end);
				}

				current = end;
				currentId++;
            }
        }
		if (lastValue.left != -1)
	        Cache.Add(lastValue);

        if (current < max)
		{
			AllowedSize += max - current;
		}
	}
	
	public int GetRandomPoint()
	{
		UpdateCache();

		if (AllowedSize == 0)
			return -1; // no available points

		int index = WorldGen.genRand.Next(AllowedSize + 1) + Min;

		foreach ((int left, int right) interval in Cache)
		{
			if (index < interval.left)
				return index;
			index += interval.right - interval.left;
		}

		return index;
	}
	
	public void AddBlock(bool weak, int left, int right)
	{
		if (weak && right - left > WeakMalus)
			return;
		Blocks.Add((weak, left, right));
		AllowedSize = -1; // Reset the cache so it will be updated on the next call to GetRandomPoint()
	}

	private void Order()
	{
		Blocks.RemoveAll(b => b.left >= b.right || (b.weak && b.right - b.left <= WeakMalus * 2));

		// Sort blocks by effective left
		Blocks.Sort((a, b) =>
		{
			int aEffectiveLeft = a.left + (a.weak ? WeakMalus : 0);
			int bEffectiveLeft = b.left + (b.weak ? WeakMalus : 0);
			return aEffectiveLeft.CompareTo(bEffectiveLeft);
		});
	}
}