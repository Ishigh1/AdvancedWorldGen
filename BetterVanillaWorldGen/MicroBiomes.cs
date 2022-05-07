using System.Collections.Generic;
using AdvancedWorldGen.Helper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Generation;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class MicroBiomes : ControlledWorldGenPass
{
	public MicroBiomes() : base("Micro Biomes", 7784.3704f)
	{
	}

	protected override void ApplyPass()
	{
		WorldGenConfiguration configuration = VanillaInterface.Configuration.Value;
		const int totalSteps = 9;
		int currentStep = 0;
		
		Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Dead Man's Chests";
		Progress.Set(currentStep++, totalSteps);
		MakeDeadManChests(configuration);

		Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Thin Ice";
		Progress.Set(currentStep++, totalSteps);
		MakeThinIcePatches(configuration);

		Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Sword Shrines";
		Progress.Set(currentStep++, totalSteps);
		MakeEnchantedSwordShrines(configuration);

		Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Campsites";
		Progress.Set(currentStep++, totalSteps);
		MakeCampsites(configuration);

		Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Explosive Traps";
		Progress.Set(currentStep++, totalSteps);
		MakeExplosiveTraps(configuration);

		Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Living Trees";
		Progress.Set(currentStep++, totalSteps);
		MakeMahoganyTrees(configuration);

		Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Long Minecart Tracks";
		Progress.Set(currentStep++, totalSteps);
		TrackGenerator trackGenerator = new();
		MakeLongMinecartTracks(trackGenerator);

		Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Standard Minecart Tracks";
		Progress.Set(currentStep++, totalSteps);
		MakeStandardMinecartTracks(trackGenerator);

		Progress.Message = Language.GetTextValue("LegacyWorldGen.76") + "..Lava Traps";
		Progress.Set(currentStep++, totalSteps);
		MakeLavaTraps();
		
		Progress.Set(currentStep, totalSteps);
	}

	private void MakeDeadManChests(WorldGenConfiguration configuration)
	{
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
	}

	private void MakeThinIcePatches(WorldGenConfiguration configuration)
	{
		if (!WorldGen.notTheBees)
		{
			ThinIceBiome thinIceBiome = configuration.CreateBiome<ThinIceBiome>();
			int random2 = Configuration.Get<WorldGenRange>("ThinIcePatchCount").GetRandom(WorldGen.genRand);
			int num33 = 0;
			const int iceMaxAttempts = 1000;
			int num35 = 0;
			while (num35 < random2)
				if (thinIceBiome.Place(
					    RandomUnderSurfaceWorldPoint((int)Main.worldSurface + 20, 200, 50, 50),
					    WorldGen.structures))
				{
					num35++;
					num33 = 0;
				}
				else
				{
					num33++;
					if (num33 > iceMaxAttempts)
					{
						num35++;
						num33 = 0;
					}
				}
		}
	}

	private void MakeEnchantedSwordShrines(WorldGenConfiguration configuration)
	{
		EnchantedSwordBiome enchantedSwordBiome = configuration.CreateBiome<EnchantedSwordBiome>();
		int swordShrines = Configuration.Get<WorldGenRange>("SwordShrineAttempts").GetRandom(WorldGen.genRand);
		float num36 = Configuration.Get<float>("SwordShrinePlacementChance");
		Point origin2 = default;
		for (int num37 = 0; num37 < swordShrines; num37++)
			if (!(WorldGen.genRand.NextFloat() > num36))
			{
				int num38 = 0;
				while (num38++ <= Main.maxTilesX)
				{
					origin2.Y = (int)WorldGen.worldSurface + WorldGen.genRand.Next(50, 100);
					origin2.X = WorldGen.genRand.NextBool(2)
						? WorldGen.genRand.Next(50, (int)(Main.maxTilesX * 0.3f))
						: WorldGen.genRand.Next((int)(Main.maxTilesX * 0.7f), Main.maxTilesX - 50);

					if (enchantedSwordBiome.Place(origin2, WorldGen.structures))
						break;
				}
			}
	}

	private void MakeCampsites(WorldGenConfiguration configuration)
	{
		if (!WorldGen.notTheBees)
		{
			CampsiteBiome campsiteBiome = configuration.CreateBiome<CampsiteBiome>();
			int random4 = Configuration.Get<WorldGenRange>("CampsiteCount").GetRandom(WorldGen.genRand);
			int num39 = 0;
			while (num39 < random4)
				if (campsiteBiome.Place(
					    RandomUnderSurfaceWorldPoint((int)Main.worldSurface, 200,
						    WorldGen.beachDistance, WorldGen.beachDistance), WorldGen.structures))
					num39++;
		}
	}

	private void MakeExplosiveTraps(WorldGenConfiguration configuration)
	{
		if (!WorldGen.notTheBees)
		{
			MiningExplosivesBiome miningExplosivesBiome = configuration.CreateBiome<MiningExplosivesBiome>();
			int num40 = Configuration.Get<WorldGenRange>("ExplosiveTrapCount").GetRandom(WorldGen.genRand);
			if (WorldGen.getGoodWorldGen)
				num40 = (int)(num40 * 1.5);

			int num41 = 0;
			while (num41 < num40)
				if (miningExplosivesBiome.Place(
					    WorldGen.RandomWorldPoint((int)WorldGen.rockLayer, WorldGen.beachDistance, 200,
						    WorldGen.beachDistance), WorldGen.structures))
					num41++;
		}
	}

	private void MakeMahoganyTrees(WorldGenConfiguration configuration)
	{
		MahoganyTreeBiome mahoganyTreeBiome = configuration.CreateBiome<MahoganyTreeBiome>();
		int treeNumber = Configuration.Get<WorldGenRange>("LivingTreeCount").GetRandom(WorldGen.genRand);
		int placed = 0;

		int tries = 0;
		int top = (int)Main.worldSurface + 50;
		int bottom = 500;
		int left = VanillaInterface.JungleMinX;
		int right = Main.maxTilesY - VanillaInterface.JungleMaxX;
		const int mahoganyMaxAttempts = 20000;

		while (placed < treeNumber && tries < mahoganyMaxAttempts)
		{
			if (mahoganyTreeBiome.Place(RandomUnderSurfaceWorldPoint(top, bottom, left, right), WorldGen.structures))
				placed++;
			else
				tries++;
			if (tries % 5000 == 0)
			{
				top -= 50;
				bottom -= 50;
				left -= 50;
				right -= 50;
			}
		}
	}

	private void MakeLongMinecartTracks(TrackGenerator trackGenerator)
	{
		int longTracks = Configuration.Get<WorldGenRange>("LongTrackCount").GetRandom(WorldGen.genRand);
		WorldGenRange worldGenRange = Configuration.Get<WorldGenRange>("LongTrackLength");
		int attempts = 0;
		int longTrackGenerated = 0;
		const int longTrackMaxAttempts = 20000;

		while (longTrackGenerated < longTracks && attempts < longTrackMaxAttempts)
			if (trackGenerator.Place(
				    RandomUnderSurfaceWorldPoint((int)Main.worldSurface, 200, 10, 10),
				    worldGenRange.ScaledMinimum, worldGenRange.ScaledMaximum))
				longTrackGenerated++;
			else
				attempts++;
	}

	private void MakeStandardMinecartTracks(TrackGenerator trackGenerator)
	{
		int longTracks = Configuration.Get<WorldGenRange>("StandardTrackCount").GetRandom(WorldGen.genRand);
		WorldGenRange worldGenRange = Configuration.Get<WorldGenRange>("StandardTrackLength");
		int maxAttempts = Main.maxTilesX * 10;
		int attempts = 0;
		int num47 = 0;
		while (num47 < longTracks)
			if (trackGenerator.Place(
				    RandomUnderSurfaceWorldPoint((int)Main.worldSurface, 200, 10, 10),
				    worldGenRange.ScaledMinimum, worldGenRange.ScaledMaximum))
			{
				num47++;
				attempts = 0;
			}
			else
			{
				attempts++;
				if (attempts > maxAttempts)
				{
					num47++;
					attempts = 0;
				}
			}
	}

	private static void MakeLavaTraps()
	{
		if (WorldGen.notTheBees) return;
		
		const int maxAttempts = 20000;
		int attempts = 0;
		double lavaTraps = Main.maxTilesX * 0.02;

		int generatedLavaTraps = 0;
		while (generatedLavaTraps < lavaTraps && attempts < maxAttempts)
		{
			int x = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
			int y = WorldGen.genRand.Next(WorldGen.lavaLine - 100, Main.maxTilesY - 210);
			if (WorldGen.placeLavaTrap(x, y))
				generatedLavaTraps++;
			else
				attempts++;
		}
	}

	public static Point RandomUnderSurfaceWorldPoint(int top = 0, int bottom = 0, int left = 0, int right = 0)
	{
		while (left + right > Main.maxTilesX)
		{
			left /= 2;
			right /= 2;
		}

		while (top + bottom > Main.maxTilesY)
		{
			top -= (int)(top - Main.worldSurface) / 2;
			bottom -= (Main.UnderworldLayer - Main.maxTilesY + bottom) / 2;
		}

		left = Utils.Clamp(left, 0, Main.maxTilesX - 1);
		right = Utils.Clamp(right, 0, Main.maxTilesX - 1);
		top = Utils.Clamp(top, 0, Main.maxTilesY - 1);
		bottom = Utils.Clamp(bottom, 0, Main.maxTilesY - 1);

		return new Point(WorldGen.genRand.Next(left, Main.maxTilesX - 1 - right),
			WorldGen.genRand.Next(top, Main.maxTilesY - 1 - bottom));
	}
}