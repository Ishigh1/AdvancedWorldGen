using AdvancedWorldGen.Base;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ID.TileID;

namespace AdvancedWorldGen.SpecialOptions
{
	public class MeltIce : GlobalTile
	{
		public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (type == BreakableIce && !fail && ModifiedWorld.OptionsContains("Santa"))
			{
				Main.tile[i, j].LiquidAmount = byte.MaxValue;
				Main.tile[i, j].LiquidType = LiquidID.Water;
			}
		}
	}
}