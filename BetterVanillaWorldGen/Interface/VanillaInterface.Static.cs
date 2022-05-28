using AdvancedWorldGen.Helper;
using Terraria;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface;

public partial class VanillaInterface
{
	public static readonly VanillaAccessor<ushort> CrackedType = new(typeof(WorldGen), "crackedType");
	public static readonly VanillaAccessor<int> HeartCount = new(typeof(WorldGen), "heartCount");
	public static readonly VanillaAccessor<int> NumOceanCaveTreasure = new(typeof(WorldGen), "numOceanCaveTreasure");
	public static readonly VanillaAccessor<bool> SkipFramingDuringGen = new(typeof(WorldGen), "skipFramingDuringGen");

	public static readonly VanillaAccessor<int[]> JChestX = new(typeof(WorldGen), "JChestX");
	public static readonly VanillaAccessor<int[]> JChestY = new(typeof(WorldGen), "JChestY");
	public static readonly VanillaAccessor<int> NumJChests = new(typeof(WorldGen), "numJChests");

	public static readonly VanillaAccessor<int> LAltarX = new(typeof(WorldGen), "lAltarX");
	public static readonly VanillaAccessor<int> LAltarY = new(typeof(WorldGen), "lAltarY");

	//Island stuff
	public static readonly VanillaAccessor<int> NumIslandHouses = new(typeof(WorldGen), "numIslandHouses");
	public static readonly VanillaAccessor<int[]> FloatingIslandHouseX = new(typeof(WorldGen), "floatingIslandHouseX");
	public static readonly VanillaAccessor<int[]> FloatingIslandHouseY = new(typeof(WorldGen), "floatingIslandHouseY");
	public static readonly VanillaAccessor<int[]> FloatingIslandStyle = new(typeof(WorldGen), "floatingIslandStyle");
	public static readonly VanillaAccessor<bool[]> SkyLake = new(typeof(WorldGen), "skyLake");
	
	public static readonly VanillaAccessor<int> GrassSpread = new(typeof(WorldGen), "grassSpread");
}