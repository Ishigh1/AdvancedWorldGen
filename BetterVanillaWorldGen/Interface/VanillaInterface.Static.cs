using AdvancedWorldGen.Helper;
using Terraria;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface
{
	public partial class VanillaInterface
	{
		public readonly VanillaAccessor<ushort> CrackedType = new(typeof(WorldGen), "crackedType");
		public readonly VanillaAccessor<int> HeartCount = new(typeof(WorldGen), "heartCount");
		public readonly VanillaAccessor<int> NumOceanCaveTreasure = new(typeof(WorldGen), "numOceanCaveTreasure");
		public readonly VanillaAccessor<bool> SkipFramingDuringGen = new(typeof(WorldGen), "skipFramingDuringGen");

		public readonly VanillaAccessor<int[]> JChestX = new(typeof(WorldGen), "JChestX");
		public readonly VanillaAccessor<int[]> JChestY = new(typeof(WorldGen), "JChestY");
		public readonly VanillaAccessor<int> NumJChests = new(typeof(WorldGen), "numJChests");

		public readonly VanillaAccessor<int> LAltarX = new(typeof(WorldGen), "lAltarX");
		public readonly VanillaAccessor<int> LAltarY = new(typeof(WorldGen), "lAltarY");
	}
}