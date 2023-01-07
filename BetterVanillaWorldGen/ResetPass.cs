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
		GenVars.crimsonLeft = WorldGen.genRand.NextBool(2);
		GenVars.numOceanCaveTreasure = 0;
		GenVars.skipDesertTileCheck = false;
		WorldGen.gen = true;
		Liquid.ReInit();
		WorldGen.noTileActions = true;
		Progress.Message = "";
		WorldGen.SetupStatueList();
		WorldGen.RandomizeWeather();
		Main.cloudAlpha = 0f;
		Main.maxRaining = 0f;
		Main.raining = false;
		
		//Became private
		//WorldGen.heartCount = 0;
		new FieldAccessor<int>(typeof(WorldGen), "heartCount").Value = 0;
		
		GenVars.extraBastStatueCount = 0;
		GenVars.extraBastStatueCountMax = 2 + Main.maxTilesX / 2100;
		Main.checkXMas();
		Main.checkHalloween();
		ResetGenerator.Invoke(null, null);
		GenVars.UndergroundDesertLocation = Rectangle.Empty;
		GenVars.UndergroundDesertHiveLocation = Rectangle.Empty;
		GenVars.numLarva = 0;
		GenerationChests.ShuffleChests(WorldGen.genRand);
		GenVars.skyIslandHouseCount = 0;

		const int num917 = 86400;
		Main.slimeRainTime = -WorldGen.genRand.Next(num917 * 2, num917 * 3);
		Main.cloudBGActive = -WorldGen.genRand.Next(8640, num917);
		if ((Params.Copper == TileExpandableList.Random &&
		     WorldGen.genRand.NextBool(2))
		    || Params.Copper == TileID.Copper)
		{
			WorldGen.SavedOreTiers.Copper = 7;
			GenVars.copperBar = 20;
		}
		else
		{
			GenVars.copper = 166;
			GenVars.copperBar = 703;
			WorldGen.SavedOreTiers.Copper = 166;
		}

		if ((Params.Iron == TileExpandableList.Random &&
		     ((WorldGen.dontStarveWorldGen && !WorldGen.everythingWorldGen) || WorldGen.genRand.NextBool(2)))
		    || Params.Iron == TileID.Iron)
		{
			WorldGen.SavedOreTiers.Iron = 6;
			GenVars.ironBar = 22;
		}
		else
		{
			GenVars.iron = 167;
			GenVars.ironBar = 704;
			WorldGen.SavedOreTiers.Iron = 167;
		}

		if ((Params.Silver == TileExpandableList.Random &&
		     WorldGen.genRand.NextBool(2))
		    || Params.Silver == TileID.Silver)
		{
			WorldGen.SavedOreTiers.Silver = 9;
			GenVars.silverBar = 21;
		}
		else
		{
			GenVars.silver = 168;
			GenVars.silverBar = 705;
			WorldGen.SavedOreTiers.Silver = 168;
		}

		if ((Params.Gold == TileExpandableList.Random &&
		     ((WorldGen.dontStarveWorldGen && !WorldGen.everythingWorldGen) || WorldGen.genRand.NextBool(2)))
		    || Params.Gold == TileID.Gold)
		{
			WorldGen.SavedOreTiers.Gold = 8;
			GenVars.goldBar = 19;
		}
		else
		{
			GenVars.gold = 169;
			GenVars.goldBar = 706;
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
		WorldGen.RandomizeMoonState(WorldGen.genRand);
		WorldGen.TreeTops.CopyExistingWorldInfoForWorldGeneration();

		int dungeonSide = WorldGen.genRand.NextBool(2) ? 1 : -1;
		GenVars.dungeonSide = dungeonSide;

		int minShift;
		int maxShift;
		if (WorldGen.remixWorldGen)
		{
			minShift = 20;
			maxShift = 35;
		}
		else if (WorldGen.tenthAnniversaryWorldGen)
		{
			minShift = 25;
			maxShift = 35;
		}
		else
		{
			minShift = 15;
			maxShift = 30;
		}
		int shift = (int)(Main.maxTilesX * WorldGen.genRand.Next(minShift, maxShift) * 0.01f);
		GenVars.jungleOriginX = dungeonSide == 1 ? shift : Main.maxTilesX - shift;

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

		GenVars.snowOriginLeft = snowOriginLeft;
		GenVars.snowOriginRight = snowOriginRight;

		float beachMultiplier = Params.BeachMultiplier;
		if (Params.ScaledBeaches)
			beachMultiplier *= worldSize;

		int beachSandDungeonExtraWidth = (int)(40 * beachMultiplier);
		int beachSandJungleExtraWidth = (int)(20 * beachMultiplier);
		int beachBordersWidth = (int)(275 * beachMultiplier);
		int beachSandRandomWidthRange = (int)(20 * beachMultiplier);
		int beachSandRandomCenter = beachBordersWidth + 5 + 2 * beachSandRandomWidthRange;
		GenVars.evilBiomeBeachAvoidance = beachSandRandomCenter + 60;
		/*
		No longer applicable
		if (worldSize < 1)
		{
			WorldGen.oceanDistance = beachBordersWidth - 25;
			WorldGen.beachDistance = beachSandRandomCenter + beachSandDungeonExtraWidth + beachSandJungleExtraWidth;
		}
		else
		{
			WorldGen.oceanDistance = (int)(WorldGen.oceanDistance * beachMultiplier);
			WorldGen.beachDistance = (int)(WorldGen.beachDistance * beachMultiplier);
		}
		*/

		GenVars.oceanWaterStartRandomMin = (int)(GenVars.oceanWaterStartRandomMin * beachMultiplier);
		GenVars.oceanWaterStartRandomMax = (int)(GenVars.oceanWaterStartRandomMax * beachMultiplier);
		GenVars.oceanWaterForcedJungleLength =
			(int)(GenVars.oceanWaterForcedJungleLength * beachMultiplier);

		int leftBeachEnd;
		if (WorldGen.tenthAnniversaryWorldGen && !WorldGen.everythingWorldGen)
			leftBeachEnd = beachSandRandomCenter + beachSandRandomWidthRange;
		else
		{
			leftBeachEnd = beachSandRandomCenter +
			               WorldGen.genRand.Next(-beachSandRandomWidthRange, beachSandRandomWidthRange + 1);
			leftBeachEnd += dungeonSide == 1 ? beachSandDungeonExtraWidth : beachSandJungleExtraWidth;
		}

		GenVars.leftBeachEnd = leftBeachEnd;

		int rightBeachStart;
		if (WorldGen.tenthAnniversaryWorldGen && !WorldGen.everythingWorldGen)
			rightBeachStart = Main.maxTilesX - beachSandRandomCenter + beachSandRandomWidthRange;
		else
		{
			rightBeachStart = Main.maxTilesX - beachSandRandomCenter +
			                  WorldGen.genRand.Next(-beachSandRandomWidthRange, beachSandRandomWidthRange + 1);
		}

		rightBeachStart -= dungeonSide == -1 ? beachSandDungeonExtraWidth : beachSandJungleExtraWidth;
		GenVars.rightBeachStart = rightBeachStart;

		int dungeonShift = (int)(50 * worldSize);
		if (dungeonSide == -1)
			GenVars.dungeonLocation = WorldGen.genRand.Next(leftBeachEnd + dungeonShift, (int)(Main.maxTilesX * 0.2));
		else
			GenVars.dungeonLocation =
				WorldGen.genRand.Next((int)(Main.maxTilesX * 0.8), rightBeachStart - dungeonShift);

		Main.tileSolid[659] = false;
		
		// Allow worlds to be bigger than x >= 31000, I don't like this fix though, fixed size arrays are bad.
		int numCaves = Math.Max(30, (int)(Main.maxTilesX * 0.001));
		GenVars.mCaveX = new int[numCaves];
		GenVars.mCaveY = new int[numCaves];
	}
}