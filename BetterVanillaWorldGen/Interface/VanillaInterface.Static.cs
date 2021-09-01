using System.Diagnostics.CodeAnalysis;
using AdvancedWorldGen.Helper;
using Terraria;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface
{
	public partial class VanillaInterface
	{
		public VanillaAccessor<ushort> CrackedType;
		public VanillaAccessor<int> HeartCount;
		public VanillaAccessor<int> NumOceanCaveTreasure;

		public VanillaAccessor<int[]> JChestX;
		public VanillaAccessor<int[]> JChestY;
		public VanillaAccessor<int> NumJChests;

		public VanillaAccessor<bool> SkipFramingDuringGen;

		[MemberNotNull(nameof(NumOceanCaveTreasure), nameof(HeartCount), nameof(SkipFramingDuringGen), nameof(CrackedType),
			nameof(JChestX), nameof(JChestY), nameof(NumJChests))]
		public void InitializeStatics()
		{
			NumOceanCaveTreasure = new VanillaAccessor<int>(typeof(WorldGen), "numOceanCaveTreasure");
			HeartCount = new VanillaAccessor<int>(typeof(WorldGen), "heartCount");
			SkipFramingDuringGen = new VanillaAccessor<bool>(typeof(WorldGen), "skipFramingDuringGen");
			CrackedType = new VanillaAccessor<ushort>(typeof(WorldGen), "crackedType");

			JChestX = new VanillaAccessor<int[]>(typeof(WorldGen), "JChestX");
			JChestY = new VanillaAccessor<int[]>(typeof(WorldGen), "JChestY");
			NumJChests = new VanillaAccessor<int>(typeof(WorldGen), "numJChests");
		}
	}
}