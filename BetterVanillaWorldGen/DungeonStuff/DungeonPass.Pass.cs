using Terraria;
using Terraria.Localization;
using static Terraria.WorldGen;

namespace AdvancedWorldGen.BetterVanillaWorldGen.DungeonStuff
{
	public partial class DungeonPass : ControlledWorldGenPass
	{
		public DungeonPass() : base("Dungeon", 477.1963f)
		{
		}

		protected override void ApplyPass()
		{
			Progress.Message = Language.GetTextValue("LegacyWorldGen.58");
			int dungeonX = Replacer.VanillaInterface.DungeonLocation.Value;
			int dungeonY;
			bool solidGround = false;
			if (drunkWorldGen)
				dungeonY = (int) Main.worldSurface + 70;
			else
			{
				dungeonY = (int) ((Main.worldSurface + Main.rockLayer) / 2.0) + Random.Next(-200, 200);
				for (int num665 = 0; num665 < 10; num665++)
					if (SolidTile(dungeonX, dungeonY + num665))
					{
						solidGround = true;
						break;
					}

				if (!solidGround)
				{
					int minX = (int) ((Main.worldSurface + Main.rockLayer) / 2.0) + 200;
					for (; dungeonY < minX && !SolidTile(dungeonX, dungeonY + 10); dungeonY++)
					{
					}
				}
			}


			MakeDungeon(dungeonX, dungeonY);
		}
	}
}