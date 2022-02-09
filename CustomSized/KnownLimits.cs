using System;

namespace AdvancedWorldGen.CustomSized;

public static class KnownLimits
{
	public const int OverhauledMinX = 1900;
	public const int NormalMinX = 4200;
	public const int ComfortNormalMaxX = 10_000;
	public const int OverhauledMinY = 500;

	public static bool WillCrashMissingEwe(int sizeX, int sizeY)
	{
		const int wireLoad = 8;
		const int tileMapLoad = 4;
		const int liquidLoad = 2;
		const int wallLoad = 2;
		const int tileLoad = 2;
		const int eweLoad = wireLoad + tileMapLoad + liquidLoad + wallLoad + tileLoad;
		return sizeX * sizeY * eweLoad > GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
	}
}