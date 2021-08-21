using Terraria;

namespace AdvancedWorldGen.CustomSized
{
	public class OverhauledInit
	{
		public static void Init()
		{
			
			float worldSize = Main.maxTilesX / 4200f;
			const int beachSandDungeonExtraWidth = 40;
			const int beachSandJungleExtraWidth = 20;
			int beachBordersWidth = (int) (275 * worldSize);
			int beachSandRandomWidthRange = (int) (20 * worldSize);
			int beachSandRandomCenter = beachBordersWidth + 5 + 2 * beachSandRandomWidthRange;
			WorldGen.oceanDistance = beachBordersWidth - 25;
			WorldGen.beachDistance = beachSandRandomCenter + beachSandDungeonExtraWidth + beachSandJungleExtraWidth;
		}
	}
}