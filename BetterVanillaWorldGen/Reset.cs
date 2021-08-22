using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class Reset : GenPass
	{
		public Reset() : base("Reset", 0.9667f)
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration passConfig)
		{
			Replacer.VanillaInterface.NumOceanCaveTreasure.Set(0);
			WorldGen.skipDesertTileCheck = false;
			WorldGen.gen = true;
			Liquid.ReInit();
			WorldGen.noTileActions = true;
			progress.Message = "";
			WorldGen.SetupStatueList();
			WorldGen.RandomizeWeather();
			Main.cloudAlpha = 0f;
			Main.maxRaining = 0f;
			Main.raining = false;
			Replacer.VanillaInterface.HeartCount.Set(0);
			Main.checkXMas();
			Main.checkHalloween();
			typeof(WorldGen).GetMethod("ResetGenerator", BindingFlags.Static | BindingFlags.NonPublic)
				.Invoke(null, null);
			WorldGen.UndergroundDesertLocation = Rectangle.Empty;
			WorldGen.UndergroundDesertHiveLocation = Rectangle.Empty;
			WorldGen.numLarva = 0;
			Chest.ShuffleChests();

			const int num917 = 86400;
			Main.slimeRainTime = -WorldGen.genRand.Next(num917 * 2, num917 * 3);
			Main.cloudBGActive = -WorldGen.genRand.Next(8640, num917);
			Replacer.VanillaInterface.SkipFramingDuringGen.Set(false);
			if (WorldGen.genRand.Next(2) == 0)
			{
				Replacer.VanillaInterface.Copper.Set(166);
				WorldGen.copperBar = 703;
				WorldGen.SavedOreTiers.Copper = 166;
			}
			else
			{
				WorldGen.SavedOreTiers.Copper = 7;
				WorldGen.copperBar = 20;
			}

			if (WorldGen.genRand.Next(2) == 0)
			{
				Replacer.VanillaInterface.Iron.Set(167);
				WorldGen.ironBar = 704;
				WorldGen.SavedOreTiers.Iron = 167;
			}
			else
			{
				WorldGen.SavedOreTiers.Iron = 6;
				WorldGen.ironBar = 22;
			}

			if (WorldGen.genRand.Next(2) == 0)
			{
				Replacer.VanillaInterface.Silver.Set(168);
				WorldGen.silverBar = 705;
				WorldGen.SavedOreTiers.Silver = 168;
			}
			else
			{
				WorldGen.SavedOreTiers.Silver = 9;
				WorldGen.silverBar = 21;
			}

			if (WorldGen.genRand.Next(2) == 0)
			{
				Replacer.VanillaInterface.Gold.Set(169);
				WorldGen.goldBar = 706;
				WorldGen.SavedOreTiers.Gold = 169;
			}
			else
			{
				WorldGen.SavedOreTiers.Gold = 8;
				WorldGen.goldBar = 19;
			}

			WorldGen.crimson = WorldGen.WorldGenParam_Evil switch
			{
				0 => false,
				1 => true,
				_ => WorldGen.genRand.Next(2) == 0
			};

			Main.worldID = WorldGen.genRand.Next(int.MaxValue);
			WorldGen.RandomizeTreeStyle();
			WorldGen.RandomizeCaveBackgrounds();
			WorldGen.RandomizeBackgrounds(WorldGen.genRand);
			WorldGen.RandomizeMoonState();
			WorldGen.TreeTops.CopyExistingWorldInfoForWorldGeneration();

			int dungeonSide = WorldGen.genRand.Next(2) == 0 ? 1 : -1;
			Replacer.VanillaInterface.DungeonSide.Set(dungeonSide);

			int shift = (int) (Main.maxTilesX * WorldGen.genRand.Next(15, 30) * 0.01f);
			Replacer.VanillaInterface.JungleOriginX.Set(dungeonSide == 1 ? shift : Main.maxTilesX - shift);

			int snowCenter = WorldGen.genRand.Next((int) (Main.maxTilesX * 0.85f));
			if (dungeonSide == 1 && !WorldGen.drunkWorldGen || dungeonSide == -1 && WorldGen.drunkWorldGen)
			{
				if (snowCenter > Main.maxTilesX * 0.6f)
					snowCenter += (int) (Main.maxTilesX * 0.15f);
			}
			else
			{
				if (snowCenter > Main.maxTilesX * 0.25f)
					snowCenter += (int) (Main.maxTilesX * 0.15f);
			}

			int num921 = WorldGen.genRand.Next(50, 90);
			float num922 = Main.maxTilesX / 4200f;
			num921 += (int) (WorldGen.genRand.Next(20, 40) * num922);
			num921 += (int) (WorldGen.genRand.Next(20, 40) * num922);
			int snowOriginLeft = Math.Max(0, snowCenter - num921);

			num921 = WorldGen.genRand.Next(50, 90);
			num921 += (int) (WorldGen.genRand.Next(20, 40) * num922);
			num921 += (int) (WorldGen.genRand.Next(20, 40) * num922);
			int snowOriginRight = Math.Min(Main.maxTilesX, snowCenter + num921);

			Replacer.VanillaInterface.SnowOriginLeft.Set(snowOriginLeft);
			Replacer.VanillaInterface.SnowOriginRight.Set(snowOriginRight);

			float worldSize = Main.maxTilesX / 4200f;
			const int beachSandDungeonExtraWidth = 40;
			const int beachSandJungleExtraWidth = 20;
			int beachBordersWidth = (int) (275 * worldSize);
			int beachSandRandomWidthRange = (int) (20 * worldSize);
			int beachSandRandomCenter = beachBordersWidth + 5 + 2 * beachSandRandomWidthRange;
			WorldGen.oceanDistance = beachBordersWidth - 25;
			WorldGen.beachDistance = beachSandRandomCenter + beachSandDungeonExtraWidth + beachSandJungleExtraWidth;
			
			int leftBeachEnd = beachSandRandomCenter + WorldGen.genRand.Next(-beachSandRandomWidthRange, beachSandRandomWidthRange);
			leftBeachEnd += dungeonSide == 1 ? beachSandDungeonExtraWidth : beachSandJungleExtraWidth;
			Replacer.VanillaInterface.LeftBeachEnd.Set(leftBeachEnd);

			int rightBeachStart = Main.maxTilesX - beachSandRandomCenter + WorldGen.genRand.Next(-beachSandRandomWidthRange, beachSandRandomWidthRange);
			rightBeachStart -= dungeonSide == -1 ? beachSandDungeonExtraWidth : beachSandJungleExtraWidth;
			Replacer.VanillaInterface.RightBeachStart.Set(rightBeachStart);

			const int num925 = 50;
			Replacer.VanillaInterface.DungeonLocation.Set(dungeonSide == -1
				? WorldGen.genRand.Next(leftBeachEnd + num925, (int) (Main.maxTilesX * 0.2))
				: WorldGen.genRand.Next((int) (Main.maxTilesX * 0.8), rightBeachStart - num925));
		}
	}
}