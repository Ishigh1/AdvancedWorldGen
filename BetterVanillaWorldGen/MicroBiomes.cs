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

		protected override void ApplyPass()
		{
			WorldGenConfiguration configuration =
				WorldGenConfiguration.FromEmbeddedPath("Terraria.GameContent.WorldBuilding.Configuration.json");
			Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Dead Man's Chests";
			const float totalSteps = 10f;

			DeadMansChestBiome deadMansChestBiome = configuration.CreateBiome<DeadMansChestBiome>();
			List<int> possibleChestsToTrapify = deadMansChestBiome.GetPossibleChestsToTrapify(WorldGen.structures);
			int random = Configuration.Get<WorldGenRange>("DeadManChests").GetRandom(WorldGen.genRand);
			int num31 = 0;
			while (num31 < random && possibleChestsToTrapify.Count > 0)
			{
				int num32 = possibleChestsToTrapify[WorldGen.genRand.Next(possibleChestsToTrapify.Count)];
				Point origin = new(Main.chest[num32].x, Main.chest[num32].y);
				deadMansChestBiome.Place(origin, WorldGen.structures);
				num31++;
				possibleChestsToTrapify.Remove(num32);
			}

			Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Thin Ice";
			Progress.Set(1f / totalSteps);
			if (!WorldGen.notTheBees)
			{
				ThinIceBiome thinIceBiome = configuration.CreateBiome<ThinIceBiome>();
				int random2 = Configuration.Get<WorldGenRange>("ThinIcePatchCount").GetRandom(WorldGen.genRand);
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

			Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Sword Shrines";
			Progress.Set(0.1f);
			Progress.Set(2f / totalSteps);
			EnchantedSwordBiome enchantedSwordBiome = configuration.CreateBiome<EnchantedSwordBiome>();
			int random3 = Configuration.Get<WorldGenRange>("SwordShrineAttempts").GetRandom(WorldGen.genRand);
			float num36 = Configuration.Get<float>("SwordShrinePlacementChance");
			Point origin2 = default;
			for (int num37 = 0; num37 < random3; num37++)
				if (!(WorldGen.genRand.NextFloat() > num36))
				{
					int num38 = 0;
					while (num38++ <= Main.maxTilesX)
					{
						origin2.Y = (int) WorldGen.worldSurface + WorldGen.genRand.Next(50, 100);
						if (WorldGen.genRand.NextBool(2))
							origin2.X = WorldGen.genRand.Next(50, (int) (Main.maxTilesX * 0.3f));
						else
							origin2.X = WorldGen.genRand.Next((int) (Main.maxTilesX * 0.7f), Main.maxTilesX - 50);

						if (enchantedSwordBiome.Place(origin2, WorldGen.structures))
							break;
					}
				}

			Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Campsites";
			Progress.Set(0.2f);
			Progress.Set(3f / totalSteps);
			if (!WorldGen.notTheBees)
			{
				CampsiteBiome campsiteBiome = configuration.CreateBiome<CampsiteBiome>();
				int random4 = Configuration.Get<WorldGenRange>("CampsiteCount").GetRandom(WorldGen.genRand);
				int num39 = 0;
				while (num39 < random4)
					if (campsiteBiome.Place(
						RandomSurfaceWorldPoint((int) Main.worldSurface, WorldGen.beachDistance, 200,
							WorldGen.beachDistance), WorldGen.structures))
						num39++;
			}

			Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Explosive Traps";
			Progress.Set(4f / totalSteps);
			if (!WorldGen.notTheBees)
			{
				MiningExplosivesBiome miningExplosivesBiome = configuration.CreateBiome<MiningExplosivesBiome>();
				int num40 = Configuration.Get<WorldGenRange>("ExplosiveTrapCount").GetRandom(WorldGen.genRand);
				if (WorldGen.getGoodWorldGen)
					num40 = (int) (num40 * 1.5);

				int num41 = 0;
				while (num41 < num40)
					if (miningExplosivesBiome.Place(
						WorldGen.RandomWorldPoint((int) WorldGen.rockLayer, WorldGen.beachDistance, 200,
							WorldGen.beachDistance), WorldGen.structures))
						num41++;
			}

			Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Living Trees";
			Progress.Set(0.3f);
			Progress.Set(5f / totalSteps);
			MahoganyTreeBiome mahoganyTreeBiome = configuration.CreateBiome<MahoganyTreeBiome>();
			int random5 = Configuration.Get<WorldGenRange>("LivingTreeCount").GetRandom(WorldGen.genRand);
			int num42 = 0;
			int num43 = 0;
			while (num42 < random5 && num43 < 20000)
			{
				if (mahoganyTreeBiome.Place(RandomSurfaceWorldPoint((int) Main.worldSurface + 50, 50, 500, 50),
					WorldGen.structures))
					num42++;

				num43++;
			}

			Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Long Minecart Tracks";
			Progress.Set(0.4f);
			Progress.Set(6f / totalSteps);
			Progress.Set(7f / totalSteps);
			// Extra patch context.
			TrackGenerator trackGenerator = new();
			int random6 = Configuration.Get<WorldGenRange>("LongTrackCount").GetRandom(WorldGen.genRand);
			WorldGenRange worldGenRange = Configuration.Get<WorldGenRange>("LongTrackLength");
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

			Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Standard Minecart Tracks";
			Progress.Set(8f / totalSteps);
			random6 = Configuration.Get<WorldGenRange>("StandardTrackCount").GetRandom(WorldGen.genRand);
			worldGenRange = Configuration.Get<WorldGenRange>("StandardTrackLength");
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

			Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Lava Traps";
			Progress.Set(9f / totalSteps);
			if (!WorldGen.notTheBees)
			{
				double num48 = Main.maxTilesX * 0.02;

				for (int _ = 0; _ < num48; _++)
				for (int __ = 0; __ < 10150; __++)
				{
					int x = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
					int y = WorldGen.genRand.Next(WorldGen.lavaLine - 100, Main.maxTilesY - 210);
					if (WorldGen.placeLavaTrap(x, y))
						break;
				}
			}

			Progress.Set(1f);
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

			return new Point(WorldGen.genRand.Next(left, Main.maxTilesX - right),
				WorldGen.genRand.Next(top, Main.maxTilesY - bottom));
		}
	}
}