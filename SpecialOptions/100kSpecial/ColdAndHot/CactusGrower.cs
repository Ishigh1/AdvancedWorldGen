namespace AdvancedWorldGen.SpecialOptions._100kSpecial.ColdAndHot;

public class CactusGrower : GlobalTile
{
	public override void RandomUpdate(int i, int j, int type)
	{
		if (_100kWorld.Enabled && type == TileID.SnowBlock)
		{
			Cactus.GrowCactus(null, i, j);
		}
	}
}