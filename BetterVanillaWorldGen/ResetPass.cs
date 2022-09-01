namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class ResetPass : ControlledWorldGenPass
{
	private static readonly MethodInfo ResetGenerator =
		typeof(WorldGen).GetMethod("ResetGenerator", BindingFlags.Static | BindingFlags.NonPublic)!;

	public ResetPass() : base("Reset", 0.9667f)
	{
	}

	protected override void ApplyPass()
	{
		WorldGen.numOceanCaveTreasure = 0;
		WorldGen.skipDesertTileCheck = false;
		WorldGen.gen = true;
		Liquid.ReInit();
		WorldGen.noTileActions = true;
		Progress.Message = "";
		WorldGen.SetupStatueList();
		WorldGen.RandomizeWeather();
		Main.cloudAlpha = 0f;
		Main.maxRaining = 0f;
		Main.raining = false;
		WorldGen.heartCount = 0;
		Main.checkXMas();
		Main.checkHalloween();
		ResetGenerator.Invoke(null, null);
		WorldGen.UndergroundDesertLocation = Rectangle.Empty;
		WorldGen.UndergroundDesertHiveLocation = Rectangle.Empty;
		WorldGen.numLarva = 0;
		GenerationChests.ShuffleChests(WorldGen.genRand);

		const int num917 = 86400;
		Main.slimeRainTime = -WorldGen.genRand.Next(num917 * 2, num917 * 3);
		Main.cloudBGActive = -WorldGen.genRand.Next(8640, num917);
		WorldGen.skipFramingDuringGen = false;
		if ((OptionHelper.WorldSettings.Params.Copper == TileExpandableList.Random &&
		     WorldGen.genRand.NextBool(2))
		    || OptionHelper.WorldSettings.Params.Copper == TileID.Copper)
		{
			WorldGen.SavedOreTiers.Copper = 7;
			WorldGen.copperBar = 20;
		}
		else
		{
			WorldGen.copper = 166;
			WorldGen.copperBar = 703;
			WorldGen.SavedOreTiers.Copper = 166;
		}

		if ((WorldGen.dontStarveWorldGen &&
		     OptionHelper.WorldSettings.Params.Iron == TileExpandableList.Random &&
		     WorldGen.genRand.NextBool(2))
		    || OptionHelper.WorldSettings.Params.Iron == TileID.Iron)
		{
			WorldGen.SavedOreTiers.Iron = 6;
			WorldGen.ironBar = 22;
		}
		else
		{
			WorldGen.iron = 167;
			WorldGen.ironBar = 704;
			WorldGen.SavedOreTiers.Iron = 167;
		}

		if ((OptionHelper.WorldSettings.Params.Silver == TileExpandableList.Random &&
		     WorldGen.genRand.NextBool(2))
		    || OptionHelper.WorldSettings.Params.Silver == TileID.Silver)
		{
			WorldGen.SavedOreTiers.Silver = 9;
			WorldGen.silverBar = 21;
		}
		else
		{
			WorldGen.silver = 168;
			WorldGen.silverBar = 705;
			WorldGen.SavedOreTiers.Silver = 168;
		}

		if ((WorldGen.dontStarveWorldGen &&
		     OptionHelper.WorldSettings.Params.Gold == TileExpandableList.Random &&
		     WorldGen.genRand.NextBool(2))
		    || OptionHelper.WorldSettings.Params.Gold == TileID.Gold)
		{
			WorldGen.SavedOreTiers.Gold = 8;
			WorldGen.goldBar = 19;
		}
		else
		{
			WorldGen.gold = 169;
			WorldGen.goldBar = 706;
			WorldGen.SavedOreTiers.Gold = 169;
		}

		switch (WorldGen.WorldGenParam_Evil)
		{
			case 0:
				WorldGen.crimson = false;
				break;
			case 1:
				WorldGen.crimson = true;
				break;
			default:
				WorldGen.crimson = Main.rand.NextBool(2);
				AdvancedWorldGenMod.Instance.Logger.Info($"Crimson : {WorldGen.crimson}");
				break;
		}

		Main.worldID = Main.rand.Next(int.MaxValue);
		WorldGen.RandomizeTreeStyle();
		WorldGen.RandomizeCaveBackgrounds();
		WorldGen.RandomizeBackgrounds(WorldGen.genRand);
		WorldGen.RandomizeMoonState();
		WorldGen.TreeTops.CopyExistingWorldInfoForWorldGeneration();

		int dungeonSide = WorldGen.genRand.NextBool(2) ? 1 : -1;
		WorldGen.dungeonSide = dungeonSide;

		int shift = (int)(Main.maxTilesX * WorldGen.genRand.Next(15, 30) * 0.01f);
		WorldGen.jungleOriginX = dungeonSide == 1 ? shift : Main.maxTilesX - shift;

		int snowCenter;
		if ((dungeonSide == 1 && !WorldGen.drunkWorldGen) || (dungeonSide == -1 && WorldGen.drunkWorldGen))
			snowCenter = (int)(Main.maxTilesX * 0.6f + Main.maxTilesX * 0.15f);
		else
			snowCenter = (int)(Main.maxTilesX * 0.25f + Main.maxTilesX * 0.15f);

		int num921 = WorldGen.genRand.Next(50, 90);
		float worldSize = Main.maxTilesX / 4200f;
		num921 += (int)(WorldGen.genRand.Next(20, 40) * worldSize);
		num921 += (int)(WorldGen.genRand.Next(20, 40) * worldSize);
		int snowOriginLeft = Math.Max(0, snowCenter - num921);

		num921 = WorldGen.genRand.Next(50, 90);
		num921 += (int)(WorldGen.genRand.Next(20, 40) * worldSize);
		num921 += (int)(WorldGen.genRand.Next(20, 40) * worldSize);
		int snowOriginRight = Math.Min(Main.maxTilesX, snowCenter + num921);

		WorldGen.snowOriginLeft = snowOriginLeft;
		WorldGen.snowOriginRight = snowOriginRight;

		worldSize *= OptionHelper.WorldSettings.Params.BeachMultiplier;
		int beachSandDungeonExtraWidth = (int)(40 * worldSize);
		int beachSandJungleExtraWidth = (int)(20 * worldSize);
		int beachBordersWidth = (int)(275 * worldSize);
		int beachSandRandomWidthRange = (int)(20 * worldSize);
		int beachSandRandomCenter = beachBordersWidth + 5 + 2 * beachSandRandomWidthRange;
		WorldGen.evilBiomeBeachAvoidance = beachSandRandomCenter + 60;
		if (worldSize < 1)
		{
			WorldGen.oceanDistance = beachBordersWidth - 25;
			WorldGen.beachDistance = beachSandRandomCenter + beachSandDungeonExtraWidth + beachSandJungleExtraWidth;
		}
		else
		{
			WorldGen.oceanDistance = (int)(WorldGen.oceanDistance *
			                               OptionHelper.WorldSettings.Params.BeachMultiplier);
			WorldGen.beachDistance = (int)(WorldGen.beachDistance *
			                               OptionHelper.WorldSettings.Params.BeachMultiplier);
		}

		WorldGen.oceanWaterStartRandomMin = (int)(WorldGen.oceanWaterStartRandomMin *
		                                          OptionHelper.WorldSettings.Params
			                                          .BeachMultiplier);
		WorldGen.oceanWaterStartRandomMax = (int)(WorldGen.oceanWaterStartRandomMax *
		                                          OptionHelper.WorldSettings.Params
			                                          .BeachMultiplier);
		WorldGen.oceanWaterForcedJungleLength =
			(int)(WorldGen.oceanWaterForcedJungleLength *
			      OptionHelper.WorldSettings.Params.BeachMultiplier);

		int leftBeachEnd = beachSandRandomCenter +
		                   WorldGen.genRand.Next(-beachSandRandomWidthRange, beachSandRandomWidthRange);
		leftBeachEnd += dungeonSide == 1 ? beachSandDungeonExtraWidth : beachSandJungleExtraWidth;
		WorldGen.leftBeachEnd = leftBeachEnd;

		int rightBeachStart = Main.maxTilesX - beachSandRandomCenter +
		                      WorldGen.genRand.Next(-beachSandRandomWidthRange, beachSandRandomWidthRange);
		rightBeachStart -= dungeonSide == -1 ? beachSandDungeonExtraWidth : beachSandJungleExtraWidth;
		WorldGen.rightBeachStart = rightBeachStart;

		int dungeonShift = (int)(50 * worldSize);
		WorldGen.dungeonLocation = dungeonSide == -1
			? WorldGen.genRand.Next(leftBeachEnd + dungeonShift, (int)(Main.maxTilesX * 0.2))
			: WorldGen.genRand.Next((int)(Main.maxTilesX * 0.8), rightBeachStart - dungeonShift);

		// Allow worlds to be bigger than x >= 31000, I don't like this fix though, fixed size arrays are bad.
		int numCaves = Math.Max(30, (int) (Main.maxTilesX * 0.001));
		WorldGen.mCaveX = new int[numCaves];
		WorldGen.mCaveY = new int[numCaves];
	}
}