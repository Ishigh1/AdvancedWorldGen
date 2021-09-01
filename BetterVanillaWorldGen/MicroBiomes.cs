using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class MicroBiomes : ControlledWorldGenPass
	{
		public MicroBiomes() : base("Micro Biomes", 3547.4304f)
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration passConfig)
		{
			WorldGenConfiguration configuration =
				WorldGenConfiguration.FromEmbeddedPath("Terraria.GameContent.WorldBuilding.Configuration.json");
			progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Dead Man's Chests";
			const float totalSteps = 10f;

			DeadMansChestBiome deadMansChestBiome = configuration.CreateBiome<DeadMansChestBiome>();
			List<int> possibleChestsToTrapify = deadMansChestBiome.GetPossibleChestsToTrapify(WorldGen.structures);
			int random = passConfig.Get<WorldGenRange>("DeadManChests").GetRandom(Random);
			int num31 = 0;
			while (num31 < random && possibleChestsToTrapify.Count > 0)
			{
				int num32 = possibleChestsToTrapify[Random.Next(possibleChestsToTrapify.Count)];
				Point origin = new(Main.chest[num32].x, Main.chest[num32].y);
				deadMansChestBiome.Place(origin, WorldGen.structures);
				num31++;
				possibleChestsToTrapify.Remove(num32);
			}

			progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Thin Ice";
			progress.Set(1f / totalSteps);
			if (!WorldGen.notTheBees)
			{
				ThinIceBiome thinIceBiome = configuration.CreateBiome<ThinIceBiome>();
				int random2 = passConfig.Get<WorldGenRange>("ThinIcePatchCount").GetRandom(Random);
				int num33 = 0;
				const int num34 = 1000;
				int num35 = 0;
				while (num35 < random2)
					if (thinIceBiome.Place(RandomSurfaceWorldPoint((int) Main.worldSurface + 20, 50, 200, 50),
						WorldGen.structures))
					{
						num35++;
						num33 = 0;
					}
					else
					{
						num33++;
						if (num33 > num34)
						{
							num35++;
							num33 = 0;
						}
					}
			}

			progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Sword Shrines";
			progress.Set(0.1f);
			progress.Set(2f / totalSteps);
			EnchantedSwordBiome enchantedSwordBiome = configuration.CreateBiome<EnchantedSwordBiome>();
			int random3 = passConfig.Get<WorldGenRange>("SwordShrineAttempts").GetRandom(Random);
			float num36 = passConfig.Get<float>("SwordShrinePlacementChance");
			Point origin2 = default;
			for (int num37 = 0; num37 < random3; num37++)
				if (!(Random.NextFloat() > num36))
				{
					int num38 = 0;
					while (num38++ <= Main.maxTilesX)
					{
						origin2.Y = (int) WorldGen.worldSurface + Random.Next(50, 100);
						if (Random.Next(2) == 0)
							origin2.X = Random.Next(50, (int) (Main.maxTilesX * 0.3f));
						else
							origin2.X = Random.Next((int) (Main.maxTilesX * 0.7f), Main.maxTilesX - 50);

						if (enchantedSwordBiome.Place(origin2, WorldGen.structures))
							break;
					}
				}

			progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Campsites";
			progress.Set(0.2f);
			progress.Set(3f / totalSteps);
			if (!WorldGen.notTheBees)
			{
				CampsiteBiome campsiteBiome = configuration.CreateBiome<CampsiteBiome>();
				int random4 = passConfig.Get<WorldGenRange>("CampsiteCount").GetRandom(Random);
				int num39 = 0;
				while (num39 < random4)
					if (campsiteBiome.Place(
						RandomSurfaceWorldPoint((int) Main.worldSurface, WorldGen.beachDistance, 200,
							WorldGen.beachDistance), WorldGen.structures))
						num39++;
			}

			progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Explosive Traps";
			progress.Set(4f / totalSteps);
			if (!WorldGen.notTheBees)
			{
				MiningExplosivesBiome miningExplosivesBiome = configuration.CreateBiome<MiningExplosivesBiome>();
				int num40 = passConfig.Get<WorldGenRange>("ExplosiveTrapCount").GetRandom(Random);
				if (WorldGen.getGoodWorldGen)
					num40 = (int) (num40 * 1.5);

				int num41 = 0;
				while (num41 < num40)
					if (miningExplosivesBiome.Place(
						WorldGen.RandomWorldPoint((int) WorldGen.rockLayer, WorldGen.beachDistance, 200,
							WorldGen.beachDistance), WorldGen.structures))
						num41++;
			}

			progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Living Trees";
			progress.Set(0.3f);
			progress.Set(5f / totalSteps);
			MahoganyTreeBiome mahoganyTreeBiome = configuration.CreateBiome<MahoganyTreeBiome>();
			int random5 = passConfig.Get<WorldGenRange>("LivingTreeCount").GetRandom(Random);
			int num42 = 0;
			int num43 = 0;
			while (num42 < random5 && num43 < 20000)
			{
				if (mahoganyTreeBiome.Place(RandomSurfaceWorldPoint((int) Main.worldSurface + 50, 50, 500, 50),
					WorldGen.structures))
					num42++;

				num43++;
			}

			progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Long Minecart Tracks";
			progress.Set(0.4f);
			progress.Set(6f / totalSteps);
			progress.Set(7f / totalSteps);
			// Extra patch context.
			TrackGenerator trackGenerator = new();
			int random6 = passConfig.Get<WorldGenRange>("LongTrackCount").GetRandom(Random);
			WorldGenRange worldGenRange = passConfig.Get<WorldGenRange>("LongTrackLength");
			int num44 = Main.maxTilesX * 10;
			int num45 = 0;
			int num46 = 0;
			while (num46 < random6)
				if (trackGenerator.Place(RandomSurfaceWorldPoint((int) Main.worldSurface, 10, 200, 10),
					worldGenRange.ScaledMinimum, worldGenRange.ScaledMaximum))
				{
					num46++;
					num45 = 0;
				}
				else
				{
					num45++;
					if (num45 > num44)
					{
						num46++;
						num45 = 0;
					}
				}

			progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Standard Minecart Tracks";
			progress.Set(8f / totalSteps);
			random6 = passConfig.Get<WorldGenRange>("StandardTrackCount").GetRandom(Random);
			worldGenRange = passConfig.Get<WorldGenRange>("StandardTrackLength");
			num45 = 0;
			int num47 = 0;
			while (num47 < random6)
				if (trackGenerator.Place(RandomSurfaceWorldPoint((int) Main.worldSurface, 10, 200, 10),
					worldGenRange.ScaledMinimum, worldGenRange.ScaledMaximum))
				{
					num47++;
					num45 = 0;
				}
				else
				{
					num45++;
					if (num45 > num44)
					{
						num47++;
						num45 = 0;
					}
				}

			progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Lava Traps";
			progress.Set(9f / totalSteps);
			if (!WorldGen.notTheBees)
			{
				double num48 = Main.maxTilesX * 0.02;

				for (int _ = 0; _ < num48; _++)
				for (int __ = 0; __ < 10150; __++)
				{
					int x = Random.Next(200, Main.maxTilesX - 200);
					int y = Random.Next(WorldGen.lavaLine - 100, Main.maxTilesY - 210);
					if (WorldGen.placeLavaTrap(x, y))
						break;
				}
			}

			progress.Set(1f);
		}

		public Point RandomSurfaceWorldPoint(int top = 0, int right = 0, int bottom = 0, int left = 0)
		{
			if (left + right > Main.maxTilesX)
			{
				left /= 2;
				right /= 2;
				return RandomSurfaceWorldPoint(top, right, bottom, left);
			}

			if (top + bottom > Main.maxTilesY)
			{
				top -= (int) (top - Main.worldSurface) / 2;
				bottom -= (Main.UnderworldLayer - bottom) / 2;
				return RandomSurfaceWorldPoint(top, right, bottom, left);
			}

			return new Point(Random.Next(left, Main.maxTilesX - right),
				Random.Next(top, Main.maxTilesY - bottom));
		}
	}
}