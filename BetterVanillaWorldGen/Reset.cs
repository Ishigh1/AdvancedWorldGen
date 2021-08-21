using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;
using static Terraria.WorldGen;

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
			skipDesertTileCheck = false;
			gen = true;
			Liquid.ReInit();
			noTileActions = true;
			progress.Message = "";
			SetupStatueList();
			RandomizeWeather();
			Main.cloudAlpha = 0f;
			Main.maxRaining = 0f;
			Main.raining = false;
			Replacer.VanillaInterface.HeartCount.Set(0);
			Main.checkXMas();
			Main.checkHalloween();
			typeof(WorldGen).GetMethod("ResetGenerator", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
			UndergroundDesertLocation = Rectangle.Empty;
			UndergroundDesertHiveLocation = Rectangle.Empty;
			numLarva = 0;
			Chest.ShuffleChests();

			const int num917 = 86400;
			Main.slimeRainTime = -genRand.Next(num917 * 2, num917 * 3);
			Main.cloudBGActive = -genRand.Next(8640, num917);
			Replacer.VanillaInterface.SkipFramingDuringGen.Set(false);
			if (genRand.Next(2) == 0)
			{
				Replacer.VanillaInterface.Copper.Set(166);
				copperBar = 703;
				SavedOreTiers.Copper = 166;
			}
			else
			{
				SavedOreTiers.Copper = 7;
				copperBar = 20;
			}

			if (genRand.Next(2) == 0)
			{
				Replacer.VanillaInterface.Iron.Set(167);
				ironBar = 704;
				SavedOreTiers.Iron = 167;
			}
			else
			{
				SavedOreTiers.Iron = 6;
				ironBar = 22;
			}

			if (genRand.Next(2) == 0)
			{
				Replacer.VanillaInterface.Silver.Set(168);
				silverBar = 705;
				SavedOreTiers.Silver = 168;
			}
			else
			{
				SavedOreTiers.Silver = 9;
				silverBar = 21;
			}

			if (genRand.Next(2) == 0)
			{
				Replacer.VanillaInterface.Gold.Set(169);
				goldBar = 706;
				SavedOreTiers.Gold = 169;
			}
			else
			{
				SavedOreTiers.Gold = 8;
				goldBar = 19;
			}

			crimson = WorldGenParam_Evil switch
			{
				0 => false,
				1 => true,
				_ => genRand.Next(2) == 0
			};

			Main.worldID = genRand.Next(int.MaxValue);
			RandomizeTreeStyle();
			RandomizeCaveBackgrounds();
			RandomizeBackgrounds(genRand);
			RandomizeMoonState();
			TreeTops.CopyExistingWorldInfoForWorldGeneration();

			int dungeonSide = genRand.Next(2) == 0 ? 1 : -1;
			Replacer.VanillaInterface.DungeonSide.Set(dungeonSide);

			int shift = (int) (Main.maxTilesX * genRand.Next(15, 30) * 0.01f);
			Replacer.VanillaInterface.JungleOriginX.Set(dungeonSide == 1 ? shift : Main.maxTilesX - shift);

			int snowCenter = genRand.Next((int) (Main.maxTilesX * 0.85f));
			if (dungeonSide == 1 && !drunkWorldGen)
			{
				if (snowCenter > Main.maxTilesX * 0.6f)
					snowCenter += (int) (Main.maxTilesX * 0.15f);
			}
			else
			{
				if (snowCenter > Main.maxTilesX * 0.25f)
					snowCenter += (int) (Main.maxTilesX * 0.15f);
			}

			int num921 = genRand.Next(50, 90);
			float num922 = Main.maxTilesX / 4200f;
			num921 += (int) (genRand.Next(20, 40) * num922);
			num921 += (int) (genRand.Next(20, 40) * num922);
			int snowOriginLeft = Math.Max(0, snowCenter - num921);

			num921 = genRand.Next(50, 90);
			num921 += (int) (genRand.Next(20, 40) * num922);
			num921 += (int) (genRand.Next(20, 40) * num922);
			int snowOriginRight = Math.Min(Main.maxTilesX, snowCenter + num921);

			Replacer.VanillaInterface.SnowOriginLeft.Set(snowOriginLeft);
			Replacer.VanillaInterface.SnowOriginRight.Set(snowOriginRight);

			const int beachSandRandomWidthRange = 20;
			const int beachSandDungeonExtraWidth = 40;
			const int beachSandJungleExtraWidth = 20;
			const int beachBordersWidth = 275;
			const int beachSandRandomCenter = beachBordersWidth + 5 + 40;
			int leftBeachEnd = genRand.Next(beachSandRandomCenter - beachSandRandomWidthRange,
				beachSandRandomCenter + beachSandRandomWidthRange);

			if (dungeonSide == 1)
				leftBeachEnd += beachSandDungeonExtraWidth;
			else
				leftBeachEnd += beachSandJungleExtraWidth;
			Replacer.VanillaInterface.LeftBeachEnd.Set(leftBeachEnd);

			int rightBeachStart = Main.maxTilesX - genRand.Next(beachSandRandomCenter - beachSandRandomWidthRange,
				beachSandRandomCenter + beachSandRandomWidthRange);
			if (dungeonSide == -1)
				rightBeachStart -= beachSandDungeonExtraWidth;
			else
				rightBeachStart -= beachSandJungleExtraWidth;
			Replacer.VanillaInterface.RightBeachStart.Set(rightBeachStart);

			const int num925 = 50;
			Replacer.VanillaInterface.DungeonLocation.Set(dungeonSide == -1
				? genRand.Next(leftBeachEnd + num925, (int) (Main.maxTilesX * 0.2))
				: genRand.Next((int) (Main.maxTilesX * 0.8), rightBeachStart - num925));
		}
	}
}