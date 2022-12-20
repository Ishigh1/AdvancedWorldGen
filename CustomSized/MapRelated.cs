namespace AdvancedWorldGen.CustomSized;

public static class MapRelated
{
	public static bool UpdateMapTileInBounds(On_WorldGen.orig_UpdateMapTile orig, int i, int j, bool addtolist)
	{
		return WorldGen.InWorld(i, j) && orig(i, j, addtolist);
	}
}