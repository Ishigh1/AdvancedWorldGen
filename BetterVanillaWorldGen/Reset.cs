using System;
using System.Reflection;
using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
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

		protected override void ApplyPass()
		{
			VanillaInterface.NumOceanCaveTreasure.Value = 0;
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
			VanillaInterface.HeartCount.Value = 0;
			Main.checkXMas();
			Main.checkHalloween();
			ResetGenerator.Invoke(null, null);
			WorldGen.UndergroundDesertLocation = Rectangle.Empty;
			WorldGen.UndergroundDesertHiveLocation = Rectangle.Empty;
			WorldGen.numLarva = 0;
			Chest.ShuffleChests(WorldGen.genRand);

			const int num917 = 86400;
			Main.slimeRainTime = -WorldGen.genRand.Next(num917 * 2, num917 * 3);
			Main.cloudBGActive = -WorldGen.genRand.Next(8640, num917);
			VanillaInterface.SkipFramingDuringGen.Value = false;
			if (WorldGen.genRand.NextBool(2))
			{
				WorldGen.copperBar = 703;
				Replacer.VanillaInterface.Copper.Value = 166;
				WorldGen.SavedOreTiers.Copper = 166;
			}
			else
			{
				WorldGen.SavedOreTiers.Copper = 7;
				Replacer.VanillaInterface.Copper.Value = 20;
				WorldGen.copperBar = 20;
			}

			if (WorldGen.genRand.NextBool(2))
			{
				WorldGen.ironBar = 704;
				Replacer.VanillaInterface.Iron.Value = 167;
				WorldGen.SavedOreTiers.Iron = 167;
			}
			else
			{
				WorldGen.SavedOreTiers.Iron = 6;
				Replacer.VanillaInterface.Iron.Value = 22;
				WorldGen.ironBar = 22;
			}

			if (WorldGen.genRand.NextBool(2))
			{
				WorldGen.silverBar = 705;
				Replacer.VanillaInterface.Silver.Value = 168;
				WorldGen.SavedOreTiers.Silver = 168;
			}
			else
			{
				WorldGen.SavedOreTiers.Silver = 9;
				Replacer.VanillaInterface.Silver.Value = 21;
				WorldGen.silverBar = 21;
			}

			if (WorldGen.genRand.NextBool(2))
			{
				WorldGen.goldBar = 706;
				Replacer.VanillaInterface.Gold.Value = 169;
				WorldGen.SavedOreTiers.Gold = 169;
			}
			else
			{
				WorldGen.SavedOreTiers.Gold = 8;
				Replacer.VanillaInterface.Gold.Value = 19;
				WorldGen.goldBar = 19;
			}

			WorldGen.crimson = WorldGen.WorldGenParam_Evil switch
			{
				0 => false,
				1 => true,
				_ => Main.rand.NextBool(2) //Using Main.rand to not affect the worldgen
			};

			Main.worldID = WorldGen.genRand.Next(int.MaxValue);
			WorldGen.RandomizeTreeStyle();
			WorldGen.RandomizeCaveBackgrounds();
			WorldGen.RandomizeBackgrounds(WorldGen.genRand);
			WorldGen.RandomizeMoonState();
			WorldGen.TreeTops.CopyExistingWorldInfoForWorldGeneration();

			int dungeonSide = WorldGen.genRand.NextBool(2) ? 1 : -1;
			Replacer.VanillaInterface.DungeonSide.Value = dungeonSide;

			int shift = (int) (Main.maxTilesX * WorldGen.genRand.Next(15, 30) * 0.01f);
			Replacer.VanillaInterface.JungleOriginX.Value = dungeonSide == 1 ? shift : Main.maxTilesX - shift;

			int snowCenter;
			if (dungeonSide == 1 && !WorldGen.drunkWorldGen || dungeonSide == -1 && WorldGen.drunkWorldGen)
				snowCenter = (int) (Main.maxTilesX * 0.6f + Main.maxTilesX * 0.15f);
			else
				snowCenter = (int) (Main.maxTilesX * 0.25f + Main.maxTilesX * 0.15f);

			int num921 = WorldGen.genRand.Next(50, 90);
			float worldSize = Main.maxTilesX / 4200f;
			num921 += (int) (WorldGen.genRand.Next(20, 40) * worldSize);
			num921 += (int) (WorldGen.genRand.Next(20, 40) * worldSize);
			int snowOriginLeft = Math.Max(0, snowCenter - num921);

			num921 = WorldGen.genRand.Next(50, 90);
			num921 += (int) (WorldGen.genRand.Next(20, 40) * worldSize);
			num921 += (int) (WorldGen.genRand.Next(20, 40) * worldSize);
			int snowOriginRight = Math.Min(Main.maxTilesX, snowCenter + num921);

			Replacer.VanillaInterface.SnowOriginLeft.Value = snowOriginLeft;
			Replacer.VanillaInterface.SnowOriginRight.Value = snowOriginRight;

			worldSize = Math.Min(worldSize, 1);
			int beachSandDungeonExtraWidth = (int) (40 * worldSize);
			int beachSandJungleExtraWidth = (int) (20 * worldSize);
			int beachBordersWidth = (int) (275 * worldSize);
			int beachSandRandomWidthRange = (int) (20 * worldSize);
			int beachSandRandomCenter = beachBordersWidth + 5 + 2 * beachSandRandomWidthRange;
			if (worldSize < 1)
			{
				WorldGen.oceanDistance = beachBordersWidth - 25;
				WorldGen.beachDistance = beachSandRandomCenter + beachSandDungeonExtraWidth + beachSandJungleExtraWidth;
			}

			int leftBeachEnd = beachSandRandomCenter +
			                   WorldGen.genRand.Next(-beachSandRandomWidthRange, beachSandRandomWidthRange);
			leftBeachEnd += dungeonSide == 1 ? beachSandDungeonExtraWidth : beachSandJungleExtraWidth;
			Replacer.VanillaInterface.LeftBeachEnd.Value = leftBeachEnd;

			int rightBeachStart = Main.maxTilesX - beachSandRandomCenter +
			                      WorldGen.genRand.Next(-beachSandRandomWidthRange, beachSandRandomWidthRange);
			rightBeachStart -= dungeonSide == -1 ? beachSandDungeonExtraWidth : beachSandJungleExtraWidth;
			Replacer.VanillaInterface.RightBeachStart.Value = rightBeachStart;

			int dungeonShift = (int) (50 * worldSize);
			Replacer.VanillaInterface.DungeonLocation.Value = dungeonSide == -1
				? WorldGen.genRand.Next(leftBeachEnd + dungeonShift, (int) (Main.maxTilesX * 0.2))
				: WorldGen.genRand.Next((int) (Main.maxTilesX * 0.8), rightBeachStart - dungeonShift);
		}
	}
}