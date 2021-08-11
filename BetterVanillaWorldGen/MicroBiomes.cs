using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class MicroBiomes : GenPass
	{
		public MicroBiomes() : base("Micro Biomes", 3547.4304f)
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration passConfig)
		{
			WorldGenConfiguration configuration =
				WorldGenConfiguration.FromEmbeddedPath("Terraria.GameContent.WorldBuilding.Configuration.json");
			progress.Message = Lang.gen[76].Value + "..Dead Man's Chests";
			float num30 = 10f;
			if (WorldGen.getGoodWorldGen)
				num30 *= 10f;

			DeadMansChestBiome deadMansChestBiome = configuration.CreateBiome<DeadMansChestBiome>();
			List<int> possibleChestsToTrapify = deadMansChestBiome.GetPossibleChestsToTrapify(WorldGen.structures);
			int random = passConfig.Get<WorldGenRange>("DeadManChests").GetRandom(WorldGen.genRand);
			int num31 = 0;
			while (num31 < random && possibleChestsToTrapify.Count > 0)
			{
				int num32 = possibleChestsToTrapify[WorldGen.genRand.Next(possibleChestsToTrapify.Count)];
				Point origin = new(Main.chest[num32].x, Main.chest[num32].y);
				deadMansChestBiome.Place(origin, WorldGen.structures);
				num31++;
				possibleChestsToTrapify.Remove(num32);
			}

			progress.Message = Lang.gen[76].Value + "..Thin Ice";
			progress.Set(1f / num30);
			if (!WorldGen.notTheBees)
			{
				ThinIceBiome thinIceBiome = configuration.CreateBiome<ThinIceBiome>();
				int random2 = passConfig.Get<WorldGenRange>("ThinIcePatchCount").GetRandom(WorldGen.genRand);
				int num33 = 0;
				const int num34 = 1000;
				int num35 = 0;
				while (num35 < random2)
					if (thinIceBiome.Place(RandomSurfaceWorldPoint((int) Main.worldSurface + 20, 50, 200, 50), WorldGen.structures))
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

			progress.Message = Lang.gen[76].Value + "..Sword Shrines";
			progress.Set(0.1f);
			progress.Set(2f / num30);
			EnchantedSwordBiome enchantedSwordBiome = configuration.CreateBiome<EnchantedSwordBiome>();
			int random3 = passConfig.Get<WorldGenRange>("SwordShrineAttempts").GetRandom(WorldGen.genRand);
			float num36 = passConfig.Get<float>("SwordShrinePlacementChance");
			Point origin2 = default;
			for (int num37 = 0; num37 < random3; num37++)
				if (!(WorldGen.genRand.NextFloat() > num36))
				{
					int num38 = 0;
					while (num38++ <= Main.maxTilesX)
					{
						origin2.Y = (int) WorldGen.worldSurface + WorldGen.genRand.Next(50, 100);
						if (WorldGen.genRand.Next(2) == 0)
							origin2.X = WorldGen.genRand.Next(50, (int) (Main.maxTilesX * 0.3f));
						else
							origin2.X = WorldGen.genRand.Next((int) (Main.maxTilesX * 0.7f), Main.maxTilesX - 50);

						if (enchantedSwordBiome.Place(origin2, WorldGen.structures))
							break;
					}
				}

			progress.Message = Lang.gen[76].Value + "..Campsites";
			progress.Set(0.2f);
			progress.Set(3f / num30);
			if (!WorldGen.notTheBees)
			{
				CampsiteBiome campsiteBiome = configuration.CreateBiome<CampsiteBiome>();
				int random4 = passConfig.Get<WorldGenRange>("CampsiteCount").GetRandom(WorldGen.genRand);
				int num39 = 0;
				while (num39 < random4)
					if (campsiteBiome.Place(
						RandomSurfaceWorldPoint((int) Main.worldSurface, WorldGen.beachDistance, 200, WorldGen.beachDistance), WorldGen.structures))
						num39++;
			}

			progress.Message = Lang.gen[76].Value + "..Explosive Traps";
			progress.Set(4f / num30);
			if (!WorldGen.notTheBees)
			{
				MiningExplosivesBiome miningExplosivesBiome = configuration.CreateBiome<MiningExplosivesBiome>();
				int num40 = passConfig.Get<WorldGenRange>("ExplosiveTrapCount").GetRandom(WorldGen.genRand);
				if (WorldGen.getGoodWorldGen)
					num40 = (int) (num40 * 1.5);

				int num41 = 0;
				while (num41 < num40)
					if (miningExplosivesBiome.Place(
						WorldGen.RandomWorldPoint((int) WorldGen.rockLayer, WorldGen.beachDistance, 200, WorldGen.beachDistance), WorldGen.structures))
						num41++;
			}

			progress.Message = Lang.gen[76].Value + "..Living Trees";
			progress.Set(0.3f);
			progress.Set(5f / num30);
			MahoganyTreeBiome mahoganyTreeBiome = configuration.CreateBiome<MahoganyTreeBiome>();
			int random5 = passConfig.Get<WorldGenRange>("LivingTreeCount").GetRandom(WorldGen.genRand);
			int num42 = 0;
			int num43 = 0;
			while (num42 < random5 && num43 < 20000)
			{
				if (mahoganyTreeBiome.Place(RandomSurfaceWorldPoint((int) Main.worldSurface + 50, 50, 500, 50), WorldGen.structures))
					num42++;

				num43++;
			}

			progress.Message = Lang.gen[76].Value + "..Long Minecart Tracks";
			progress.Set(0.4f);
			progress.Set(6f / num30);
			progress.Set(7f / num30);
			// Extra patch context.
			TrackGenerator trackGenerator = new();
			int random6 = passConfig.Get<WorldGenRange>("LongTrackCount").GetRandom(WorldGen.genRand);
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

			progress.Message = Lang.gen[76].Value + "..Standard Minecart Tracks";
			progress.Set(8f / num30);
			random6 = passConfig.Get<WorldGenRange>("StandardTrackCount").GetRandom(WorldGen.genRand);
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

			progress.Message = Lang.gen[76].Value + "..Lava Traps";
			progress.Set(9f / num30);
			if (!WorldGen.notTheBees)
			{
				double num48 = Main.maxTilesX * 0.02;
				if (WorldGen.getGoodWorldGen)
					num30 *= 2f;

				for (int num49 = 0; (double) num49 < num48; num49++)
				for (int num50 = 0; num50 < 10150; num50++)
				{
					int x2 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
					int y2 = WorldGen.genRand.Next(WorldGen.lavaLine - 100, Main.maxTilesY - 210);
					if (WorldGen.placeLavaTrap(x2, y2))
						break;
				}
			}

			progress.Set(1f);
		}

		public static Point RandomSurfaceWorldPoint(int top = 0, int right = 0, int bottom = 0, int left = 0)
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
				return  RandomSurfaceWorldPoint(top, right, bottom, left);
			}

			return new Point(WorldGen.genRand.Next(left, Main.maxTilesX - right), WorldGen.genRand.Next(top, Main.maxTilesY - bottom));
		}
	}
}