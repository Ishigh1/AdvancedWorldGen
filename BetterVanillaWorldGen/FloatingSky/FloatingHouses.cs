namespace AdvancedWorldGen.BetterVanillaWorldGen.FloatingSky;

public class FloatingHouses : ControlledWorldGenPass
{
	public FloatingHouses() : base("Floating Island Houses", 1.5022f)
	{
	}

	protected override void ApplyPass()
	{
		foreach (FloatingIslandInfo floatingIslandInfo in VanillaInterface.FloatingIslandInfos.Where(floatingIslandInfo => !floatingIslandInfo.IsLake))
			WorldGen.IslandHouse(floatingIslandInfo.X, floatingIslandInfo.Y, floatingIslandInfo.Style);
	}
}