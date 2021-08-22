namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface
{
	public partial class VanillaInterface
	{
		public VanillaAccessor<int> NumOceanCaveTreasure;
		public VanillaAccessor<int> HeartCount;
		
		public VanillaAccessor<bool> SkipFramingDuringGen;
		
		public VanillaAccessor<int> CrackedType;

		public void InitializeStatics()
		{
			NumOceanCaveTreasure = new VanillaAccessor<int>("numOceanCaveTreasure");
			HeartCount = new VanillaAccessor<int>("heartCount");
			SkipFramingDuringGen = new VanillaAccessor<bool>("skipFramingDuringGen");
			CrackedType = new VanillaAccessor<int>("crackedType");
		}
	}
}