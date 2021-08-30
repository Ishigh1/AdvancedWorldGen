using AdvancedWorldGen.Helper;
using Terraria;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface
{
	public partial class VanillaInterface
	{
		public VanillaAccessor<ushort> CrackedType;
		public VanillaAccessor<int> HeartCount;
		public VanillaAccessor<int> NumOceanCaveTreasure;

		public VanillaAccessor<bool> SkipFramingDuringGen;

		public void InitializeStatics()
		{
			NumOceanCaveTreasure = new VanillaAccessor<int>(typeof(WorldGen), "numOceanCaveTreasure");
			HeartCount = new VanillaAccessor<int>(typeof(WorldGen), "heartCount");
			SkipFramingDuringGen = new VanillaAccessor<bool>(typeof(WorldGen), "skipFramingDuringGen");
			CrackedType = new VanillaAccessor<ushort>(typeof(WorldGen), "crackedType");
		}
	}
}