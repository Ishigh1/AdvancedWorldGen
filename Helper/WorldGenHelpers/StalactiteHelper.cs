using System.Runtime.CompilerServices;

namespace AdvancedWorldGen.Helper.WorldGenHelpers;

public static class StalactiteHelper
{
	public const int Ice = 0; // No stalagmites
	public const int Stone = 3;
	public const int Web = 6; // Only big stalactites
	public const int Honey = 9; // Only small
	public const int Hallow = 12;
	public const int Corruption = 15;
	public const int Crimson = 18;
	public const int Sand = 21;
	public const int Granite = 24;
	public const int Marble = 27;
	public const int HallowIce = 30; // No stalagmites
	public const int CorruptIce = 33; // No stalagmites
	public const int CrimsonIce = 36; // No stalagmites


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void PlaceAnyStalactite(int x, int y, int style)
	{
		if (WorldGen.genRand.NextBool(2))
			PlaceBigStalactite(x, y, style);
		else
			PlaceSmallStalactite(x, y, style);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void PlaceBigStalactite(int x, int y, int style)
	{
		TilePlacer.QuickPlaceFurniture(x, y, TileID.Stalactite, 1, 2, styleX: style + WorldGen.genRand.Next(3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void PlaceSmallStalactite(int x, int y, int style)
	{
		TilePlacer.QuickPlaceTile(x, y, TileID.Stalactite, styleX: style + WorldGen.genRand.Next(3), styleY: 4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void PlaceAnyStalagmite(int x, int y, int style)
	{
		if (WorldGen.genRand.NextBool(2))
			PlaceBigStalagmite(x, y, style);
		else
			PlaceSmallStalagmite(x, y + 1, style);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void PlaceBigStalagmite(int x, int y, int style)
	{
		TilePlacer.QuickPlaceFurniture(x, y, TileID.Stalactite, 1, 2, styleX: style + WorldGen.genRand.Next(3),
			styleY: 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void PlaceSmallStalagmite(int x, int y, int style)
	{
		TilePlacer.QuickPlaceTile(x, y, TileID.Stalactite, styleX: style + WorldGen.genRand.Next(3), styleY: 5);
	}
}