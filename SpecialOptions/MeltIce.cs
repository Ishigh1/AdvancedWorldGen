using static Terraria.ID.TileID;

namespace AdvancedWorldGen.SpecialOptions;

public class MeltIce : GlobalTile
{
	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (type == BreakableIce && !fail && API.OptionsContains("Santa"))
		{
			Tile tile = Main.tile[i, j];
			tile.LiquidAmount = byte.MaxValue;
			tile.LiquidType = LiquidID.Water;
		}
	}
}