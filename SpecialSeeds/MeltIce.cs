using Terraria;
using Terraria.ModLoader;
using static Terraria.ID.TileID;

namespace AdvancedSeedGen.SpecialSeeds
{
	public class MeltIce : GlobalTile
	{
		public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (type == BreakableIce && !fail && CustomSeededWorld.OptionsContains("Santa"))
			{
				Main.tile[i, j].liquid = byte.MaxValue;
				Main.tile[i, j].honey(false);
				Main.tile[i, j].lava(false);
			}
		}
	}
}