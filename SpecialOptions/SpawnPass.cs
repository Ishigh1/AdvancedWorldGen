namespace AdvancedWorldGen.SpecialOptions;

public class SpawnPass : ControlledWorldGenPass
{
	public SpawnPass() : base("Spawn Point", 1f)
	{
	}

	protected override void ApplyPass()
	{
		int num310 = 5;
		bool flag10 = true;
		int num311;
		if (OptionHelper.OptionsContains("Celebrationmk10.BeachSpawn") && !WorldGen.remixWorldGen) {
			int num312 = GenVars.beachBordersWidth + 15;
			num311 = WorldGen.genRand.NextBool(2) ? Main.maxTilesX - num312 : num312;
		}
		else
		{
			num311 = Main.maxTilesX / 2;
		}

		while (flag10) {
			int num313 = num311 + WorldGen.genRand.Next(-num310, num310 + 1);
			for (int num314 = 0; num314 < Main.maxTilesY; num314++) {
				if (Main.tile[num313, num314].HasTile) {
					Main.spawnTileX = num313;
					Main.spawnTileY = num314;
					break;
				}
			}

			flag10 = false;
			num310++;
			if (Main.spawnTileY > Main.worldSurface)
				flag10 = true;

			if (Main.tile[Main.spawnTileX, Main.spawnTileY - 1].LiquidAmount > 0)
				flag10 = true;
		}

		int num315 = 10;
		while (Main.spawnTileY > Main.worldSurface) {
			int num316 = WorldGen.genRand.Next(num311 - num315, num311 + num315);
			for (int num317 = 0; num317 < Main.maxTilesY; num317++) {
				if (Main.tile[num316, num317].HasTile) {
					Main.spawnTileX = num316;
					Main.spawnTileY = num317;
					break;
				}
			}

			num315++;
		}

		if (WorldGen.remixWorldGen) {
			int num318 = Main.maxTilesY - 10;
			while (WorldGen.SolidTile(Main.spawnTileX, num318)) {
				num318--;
			}

			Main.spawnTileY = num318 + 1;
		}
	}
}