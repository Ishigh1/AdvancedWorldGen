namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface
{
	public partial class VanillaInterface
	{
		public VanillaAccessor<int> numOceanCaveTreasure;
		public VanillaAccessor<int> heartCount;
		public VanillaAccessor<bool> skipFramingDuringGen;

		public void InitializeStatics()
		{
			numOceanCaveTreasure = new VanillaAccessor<int>("numOceanCaveTreasure");
			heartCount = new VanillaAccessor<int>("heartCount");
			skipFramingDuringGen = new VanillaAccessor<bool>("skipFramingDuringGen");
		}
	}
}