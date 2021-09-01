using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class Reset : ControlledWorldGenPass
	{
		private static readonly MethodInfo ResetGenerator =
			typeof(WorldGen).GetMethod("ResetGenerator", BindingFlags.Static | BindingFlags.NonPublic)!;

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
			ResetGenerator.Invoke(null, null);
			WorldGen.UndergroundDesertLocation = Rectangle.Empty;
			WorldGen.UndergroundDesertHiveLocation = Rectangle.Empty;
			WorldGen.numLarva = 0;
			Chest.ShuffleChests(Random);

			const int num917 = 86400;
			Main.slimeRainTime = -Random.Next(num917 * 2, num917 * 3);
			Main.cloudBGActive = -Random.Next(8640, num917);
			Replacer.VanillaInterface.SkipFramingDuringGen.Set(false);
			if (Random.NextBool(2))
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

			if (Random.NextBool(2))
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

			if (Random.NextBool(2))
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

			if (Random.NextBool(2))
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
				_ => Main.rand.NextBool(2) //Using Main.rand to not affect the worldgen
			};

			Main.worldID = Random.Next(int.MaxValue);
			WorldGen.RandomizeTreeStyle();
			WorldGen.RandomizeCaveBackgrounds();
			WorldGen.RandomizeBackgrounds(Random);
			WorldGen.RandomizeMoonState();
			WorldGen.TreeTops.CopyExistingWorldInfoForWorldGeneration();

			int dungeonSide = Random.NextBool(2) ? 1 : -1;
			Replacer.VanillaInterface.DungeonSide.Set(dungeonSide);

			int shift = (int) (Main.maxTilesX * Random.Next(15, 30) * 0.01f);
			Replacer.VanillaInterface.JungleOriginX.Set(dungeonSide == 1 ? shift : Main.maxTilesX - shift);

			int snowCenter;
			if (dungeonSide == 1 && !WorldGen.drunkWorldGen || dungeonSide == -1 && WorldGen.drunkWorldGen)
				snowCenter = (int) (Main.maxTilesX * 0.6f + Main.maxTilesX * 0.15f);
			else
				snowCenter = (int) (Main.maxTilesX * 0.25f + Main.maxTilesX * 0.15f);

			int num921 = Random.Next(50, 90);
			float worldSize = Main.maxTilesX / 4200f;
			num921 += (int) (Random.Next(20, 40) * worldSize);
			num921 += (int) (Random.Next(20, 40) * worldSize);
			int snowOriginLeft = Math.Max(0, snowCenter - num921);

			num921 = Random.Next(50, 90);
			num921 += (int) (Random.Next(20, 40) * worldSize);
			num921 += (int) (Random.Next(20, 40) * worldSize);
			int snowOriginRight = Math.Min(Main.maxTilesX, snowCenter + num921);

			Replacer.VanillaInterface.SnowOriginLeft.Set(snowOriginLeft);
			Replacer.VanillaInterface.SnowOriginRight.Set(snowOriginRight);

			int beachSandDungeonExtraWidth = (int) (40 * worldSize);
			int beachSandJungleExtraWidth = (int) (20 * worldSize);
			int beachBordersWidth = (int) (275 * worldSize);
			int beachSandRandomWidthRange = (int) (20 * worldSize);
			int beachSandRandomCenter = beachBordersWidth + 5 + 2 * beachSandRandomWidthRange;
			WorldGen.oceanDistance = beachBordersWidth - 25;
			WorldGen.beachDistance = beachSandRandomCenter + beachSandDungeonExtraWidth + beachSandJungleExtraWidth;

			int leftBeachEnd = beachSandRandomCenter +
			                   Random.Next(-beachSandRandomWidthRange, beachSandRandomWidthRange);
			leftBeachEnd += dungeonSide == 1 ? beachSandDungeonExtraWidth : beachSandJungleExtraWidth;
			Replacer.VanillaInterface.LeftBeachEnd.Set(leftBeachEnd);

			int rightBeachStart = Main.maxTilesX - beachSandRandomCenter +
			                      Random.Next(-beachSandRandomWidthRange, beachSandRandomWidthRange);
			rightBeachStart -= dungeonSide == -1 ? beachSandDungeonExtraWidth : beachSandJungleExtraWidth;
			Replacer.VanillaInterface.RightBeachStart.Set(rightBeachStart);

			int dungeonShift = (int) (50 * worldSize);
			Replacer.VanillaInterface.DungeonLocation.Set(dungeonSide == -1
				? Random.Next(leftBeachEnd + dungeonShift, (int) (Main.maxTilesX * 0.2))
				: Random.Next((int) (Main.maxTilesX * 0.8), rightBeachStart - dungeonShift));
		}
	}
}