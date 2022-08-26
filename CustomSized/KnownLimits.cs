namespace AdvancedWorldGen.CustomSized;

public static class KnownLimits
{
	public const int OverhauledMinX = 1_500;
	public const int NormalMinX = 4_200;
	public const int ComfortNormalMaxX = 9_000;
	public const int NormalMaxX = 12_000;
	public const int OverhauledMinY = 500;

	public const int ClamityMinX = 6_400;
	public const int ClamityMaxX = 10_000;

	public const int DataLoad = 8;
	public const int MapLoad = 4;
	public const int LiquidLoad = 2;
	public const int WallLoad = 2;
	public const int TileLoad = 2;
	public const int EweLoad = DataLoad + MapLoad + LiquidLoad + WallLoad + TileLoad;

	public static bool WillCrashMissingEwe(int sizeX, int sizeY)
	{
		return sizeX * sizeY * EweLoad > GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
	}
}