using System.Runtime.CompilerServices;

namespace AdvancedWorldGen.Helper.WorldGenHelpers;

public static class TilePlacer
{
	// All of the following methods place things starting from top-left
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void QuickPlaceFurniture(int x, int y, ushort type, int sizeX, int sizeY, int styleX = 0,
		int styleY = 0)
	{
		for (short i = 0; i < sizeX; i++)
		for (short j = 0; j < sizeY; j++)
		{
			QuickPlaceTile(x + i, y + j, type, styleX: i + styleX * sizeX, styleY: j + styleY * sizeY);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void QuickPlaceTile(int x, int y, ushort type, int styleX = 0, int styleY = 0)
	{
		Tile tile = Main.tile[x, y];
		tile.HasTile = true;
		const short styleSize = 18;
		tile.TileFrameX = (short)(styleX * styleSize);
		tile.TileFrameY = (short)(styleY * styleSize);
		tile.TileType = type;
	}
}