using System;

namespace AdvancedWorldGen.CustomSized;

public static class KnownLimits
{
	public const int OverhauledMinX = 1_500;
	public const int NormalMinX = 4_200;
	public const int ComfortNormalMaxX = 10_000;
	public const int OverhauledMinY = 500;

	public const int ClamityMinX = 6_400;
	public const int ClamityMaxX = 10_000;

	public static bool WillCrashMissingEwe(int sizeX, int sizeY)
	{
		const int dataLoad = 8;
		const int mapLoad = 4;
		const int liquidLoad = 2;
		const int wallLoad = 2;
		const int tileLoad = 2;
		const int eweLoad = dataLoad + mapLoad + liquidLoad + wallLoad + tileLoad;
		return sizeX * sizeY * eweLoad > GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
	}
}