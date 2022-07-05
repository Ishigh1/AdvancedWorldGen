using System;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
using AdvancedWorldGen.Helper;
using Terraria;
using Terraria.ID;

namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class Corruption : ControlledWorldGenPass
{
	public Corruption() : base("Corruption", 1094.237f)
	{
	}

	protected override void ApplyPass()
	{
		int snowLeft = Main.maxTilesX;
		int snowRight = 0;
		for (int x = 0; x < Main.maxTilesX; x++)
		for (int y = 0; y < Main.worldSurface; y++)
			if (Main.tile[x, y].HasTile)
				switch (Main.tile[x, y].TileType)
				{
					case TileID.SnowBlock or TileID.IceBlock:
					{
						if (x < snowLeft)
							snowLeft = x;

						if (x > snowRight)
							snowRight = x;
						break;
					}
				}

		const int num700 = 10;
		VanillaInterface.JungleLeft -= num700;
		VanillaInterface.JungleRight += num700;
		snowLeft -= num700;
		snowRight += num700;
		int beachPadding = WorldGen.oceanDistance * 2;
		const int num702 = 100;
		double biomeNumber = Main.maxTilesX * 0.00045;
		if (API.OptionsContains("Drunk.Crimruption"))
		{
			biomeNumber /= 2.0;
			bool flag47 = WorldGen.genRand.NextBool(2);
			GenerateCrimson(biomeNumber, snowLeft, snowRight, flag47, beachPadding, num702);
			GenerateCorruption(biomeNumber, snowLeft, snowRight, flag47, beachPadding, num702);
		}
		else if (WorldGen.crimson)
		{
			GenerateCrimson(biomeNumber, snowLeft, snowRight, true, beachPadding, num702);
		}
		else
		{
			GenerateCorruption(biomeNumber, snowLeft, snowRight, true, beachPadding, num702);
		}
	}

	private void GenerateCorruption(double biomeNumber, int snowLeft, int snowRight, bool flag47, int beachPadding, int num702)
	{
		int corruptionMin = Math.Max(VanillaInterface.EvilBiomeBeachAvoidance, 0);
		int corruptionMax = Main.maxTilesX - Math.Min(VanillaInterface.EvilBiomeBeachAvoidance, 0);

		Progress.Message = Lang.gen[20].Value;
		for (int biome = 0; biome < biomeNumber; biome++)
		{
			int num728 = snowLeft;
			int num729 = snowRight;
			int num730 = VanillaInterface.JungleLeft;
			int num731 = VanillaInterface.JungleRight;
			Progress.Set(biome, (float)biomeNumber);
			bool isValid = false;
			int num732 = 0;
			int corruptionLeft = 0;
			int corruptionRight = 0;
			while (!isValid)
			{
				isValid = true;
				int half = Main.maxTilesX / 2;
				const int num736 = 200;
				if (API.OptionsContains("Drunk.Crimruption"))
					num732 = flag47 ? WorldGen.genRand.Next(half, Main.maxTilesX - beachPadding) : WorldGen.genRand.Next(beachPadding, half);
				else
					num732 = WorldGen.genRand.Next(beachPadding, Main.maxTilesX - beachPadding);
				corruptionLeft = num732 - WorldGen.genRand.Next(200) - 100;
				corruptionRight = num732 + WorldGen.genRand.Next(200) + 100;
				corruptionLeft = Utils.Clamp(corruptionLeft, corruptionMin, corruptionMax);
				corruptionRight = Utils.Clamp(corruptionRight, corruptionMin, corruptionMax);

				if (num732 < corruptionLeft + VanillaInterface.EvilBiomeAvoidanceMidFixer)
					num732 = corruptionLeft + VanillaInterface.EvilBiomeAvoidanceMidFixer;

				if (num732 > corruptionRight - VanillaInterface.EvilBiomeAvoidanceMidFixer)
					num732 = corruptionRight - VanillaInterface.EvilBiomeAvoidanceMidFixer;

				if (num732 > half - num736 && num732 < half + num736)
					isValid = false;

				if (corruptionLeft > half - num736 && corruptionLeft < half + num736)
					isValid = false;

				if (corruptionRight > half - num736 && corruptionRight < half + num736)
					isValid = false;

				if (num732 > WorldGen.UndergroundDesertLocation.X && num732 < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
					isValid = false;

				if (corruptionLeft > WorldGen.UndergroundDesertLocation.X && corruptionLeft < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
					isValid = false;

				if (corruptionRight > WorldGen.UndergroundDesertLocation.X && corruptionRight < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
					isValid = false;

				if (corruptionLeft < VanillaInterface.DungeonLocation + num702 && corruptionRight > VanillaInterface.DungeonLocation - num702)
					isValid = false;

				if (corruptionLeft < num729 && corruptionRight > num728)
				{
					num728++;
					num729--;
					isValid = false;
				}

				if (corruptionLeft < num731 && corruptionRight > num730)
				{
					num730++;
					num731--;
					isValid = false;
				}
			}

			int num737 = 0;
			for (int num738 = corruptionLeft; num738 < corruptionRight; num738++)
			{
				if (num737 > 0)
					num737--;

				if (num738 == num732 || num737 == 0)
					for (int num739 = (int)WorldGen.worldSurfaceLow; num739 < Main.worldSurface - 1.0; num739++)
						if (Main.tile[num738, num739].HasTile || Main.tile[num738, num739].WallType > 0)
						{
							if (num738 == num732)
							{
								num737 = 20;
								WorldGen.ChasmRunner(num738, num739, WorldGen.genRand.Next(150) + 150, true);
							}
							else if (WorldGen.genRand.Next(35) == 0 && num737 == 0)
							{
								num737 = 30;
								WorldGen.ChasmRunner(num738, num739, WorldGen.genRand.Next(50) + 50, true);
							}

							break;
						}

				for (int num740 = (int)WorldGen.worldSurfaceLow; num740 < Main.worldSurface - 1.0; num740++)
					if (Main.tile[num738, num740].HasTile)
					{
						int num741 = num740 + WorldGen.genRand.Next(10, 14);
						for (int num742 = num740; num742 < num741; num742++)
							if (Main.tile[num738, num742].TileType is 59 or 60 && num738 >= corruptionLeft + WorldGen.genRand.Next(5) && num738 < corruptionRight - WorldGen.genRand.Next(5))
								Main.tile[num738, num742].TileType = 0;

						break;
					}
			}

			double num743 = Main.worldSurface + 40.0;
			for (int num744 = corruptionLeft; num744 < corruptionRight; num744++)
			{
				num743 += WorldGen.genRand.Next(-2, 3);
				if (num743 < Main.worldSurface + 30.0)
					num743 = Main.worldSurface + 30.0;

				if (num743 > Main.worldSurface + 50.0)
					num743 = Main.worldSurface + 50.0;

				bool flag52 = false;
				for (int num745 = (int)WorldGen.worldSurfaceLow; num745 < num743; num745++)
					if (Main.tile[num744, num745].HasTile)
					{
						if (Main.tile[num744, num745].TileType == 53 && num744 >= corruptionLeft + WorldGen.genRand.Next(5) && num744 <= corruptionRight - WorldGen.genRand.Next(5))
							Main.tile[num744, num745].TileType = 112;

						if (Main.tile[num744, num745].TileType == 0 && num745 < Main.worldSurface - 1.0 && !flag52)
						{
							VanillaInterface.GrassSpread.Value = 0;
							WorldGen.SpreadGrass(num744, num745, 0, 23);
						}

						flag52 = true;
						if (Main.tile[num744, num745].TileType == 1 && num744 >= corruptionLeft + WorldGen.genRand.Next(5) && num744 <= corruptionRight - WorldGen.genRand.Next(5))
							Main.tile[num744, num745].TileType = 25;

						Main.tile[num744, num745].WallType = Main.tile[num744, num745].WallType switch
						{
							216 => 217,
							187 => 220,
							_ => Main.tile[num744, num745].WallType
						};

						if (Main.tile[num744, num745].TileType == 2)
							Main.tile[num744, num745].TileType = 23;

						if (Main.tile[num744, num745].TileType == 161)
							Main.tile[num744, num745].TileType = 163;
						else if (Main.tile[num744, num745].TileType == 396)
							Main.tile[num744, num745].TileType = 400;
						else if (Main.tile[num744, num745].TileType == 397)
							Main.tile[num744, num745].TileType = 398;
					}
			}

			for (int num746 = corruptionLeft; num746 < corruptionRight; num746++)
			for (int num747 = 0; num747 < Main.maxTilesY - 50; num747++)
				if (Main.tile[num746, num747].HasTile && Main.tile[num746, num747].TileType == 31)
				{
					int num748 = num746 - 13;
					int num749 = num746 + 13;
					int num750 = num747 - 13;
					int num751 = num747 + 13;
					for (int num752 = num748; num752 < num749; num752++)
						if (num752 > 10 && num752 < Main.maxTilesX - 10)
							for (int num753 = num750; num753 < num751; num753++)
							{
								Tile tile = Main.tile[num752, num753];
								if (Math.Abs(num752 - num746) + Math.Abs(num753 - num747) < 9 + WorldGen.genRand.Next(11) && WorldGen.genRand.Next(3) != 0 && tile.HasTile && tile.TileType != 31)
									if (Math.Abs(num752 - num746) > 1 || Math.Abs(num753 - num747) > 1)
										WorldGen.PlaceTile(num752, num753, 25, true);

								if (Main.tile[num752, num753].TileType != 31 && Math.Abs(num752 - num746) <= 2 + WorldGen.genRand.Next(3) && Math.Abs(num753 - num747) <= 2 + WorldGen.genRand.Next(3)) WorldGen.KillTile(num752, num753);
							}
				}
		}
	}

	private void GenerateCrimson(double biomeNumber, int snowLeft, int snowRight, bool flag47, int beachPadding, int num702)
	{
		Progress.Message = Lang.gen[72].Value;

		int crimsonMin = Math.Max(VanillaInterface.EvilBiomeBeachAvoidance, VanillaInterface.DungeonSide == -1 ? beachPadding * 4 / 5 : 0);
		int crimsonMax = Main.maxTilesX - Math.Min(VanillaInterface.EvilBiomeBeachAvoidance, VanillaInterface.DungeonSide == 1 ? beachPadding * 4 / 5 : 0);

		for (int biome = 0; biome < biomeNumber; biome++)
		{
			int num705 = snowLeft;
			int num706 = snowRight;
			int num707 = VanillaInterface.JungleLeft;
			int num708 = VanillaInterface.JungleRight;
			Progress.Set(biome, (float)biomeNumber);
			bool flag48 = false;
			int num709 = 0;
			int crimsonLeft = 0;
			int crimsonRight = 0;
			while (!flag48)
			{
				flag48 = true;
				int num712 = Main.maxTilesX / 2;
				int num713 = 200;
				if (API.OptionsContains("Drunk.Crimruption"))
				{
					num713 = 100;
					num709 = flag47 ? WorldGen.genRand.Next(beachPadding, (int)(Main.maxTilesX * 0.5)) : WorldGen.genRand.Next((int)(Main.maxTilesX * 0.5), Main.maxTilesX - beachPadding);
				}
				else
				{
					num709 = WorldGen.genRand.Next(beachPadding, Main.maxTilesX - beachPadding);
				}

				crimsonLeft = num709 - WorldGen.genRand.Next(200) - 100;
				crimsonRight = num709 + WorldGen.genRand.Next(200) + 100;
				crimsonLeft = Utils.Clamp(crimsonLeft, crimsonMin, crimsonMax);
				crimsonRight = Utils.Clamp(crimsonLeft, crimsonMin, crimsonMax);

				if (num709 < crimsonLeft + VanillaInterface.EvilBiomeAvoidanceMidFixer)
					num709 = crimsonLeft + VanillaInterface.EvilBiomeAvoidanceMidFixer;

				if (num709 > crimsonRight - VanillaInterface.EvilBiomeAvoidanceMidFixer)
					num709 = crimsonRight - VanillaInterface.EvilBiomeAvoidanceMidFixer;

				if (num709 > num712 - num713 && num709 < num712 + num713)
					flag48 = false;

				if (crimsonLeft > num712 - num713 && crimsonLeft < num712 + num713)
					flag48 = false;

				if (crimsonRight > num712 - num713 && crimsonRight < num712 + num713)
					flag48 = false;

				if (num709 > WorldGen.UndergroundDesertLocation.X && num709 < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
					flag48 = false;

				if (crimsonLeft > WorldGen.UndergroundDesertLocation.X && crimsonLeft < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
					flag48 = false;

				if (crimsonRight > WorldGen.UndergroundDesertLocation.X && crimsonRight < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
					flag48 = false;

				if (crimsonLeft < VanillaInterface.DungeonLocation + num702 && crimsonRight > VanillaInterface.DungeonLocation - num702)
					flag48 = false;

				if (crimsonLeft < num706 && crimsonRight > num705)
				{
					num705++;
					num706--;
					flag48 = false;
				}

				if (crimsonLeft < num708 && crimsonRight > num707)
				{
					num707++;
					num708--;
					flag48 = false;
				}
			}

			WorldGen.CrimStart(num709, (int)WorldGen.worldSurfaceLow - 10);
			for (int num714 = crimsonLeft; num714 < crimsonRight; num714++)
			for (int num715 = (int)WorldGen.worldSurfaceLow; num715 < Main.worldSurface - 1.0; num715++)
				if (Main.tile[num714, num715].HasTile)
				{
					int num716 = num715 + WorldGen.genRand.Next(10, 14);
					for (int num717 = num715; num717 < num716; num717++)
						if (Main.tile[num714, num717].TileType is 59 or 60 && num714 >= crimsonLeft + WorldGen.genRand.Next(5) && num714 < crimsonRight - WorldGen.genRand.Next(5))
							Main.tile[num714, num717].TileType = 0;

					break;
				}

			double num718 = Main.worldSurface + 40.0;
			for (int num719 = crimsonLeft; num719 < crimsonRight; num719++)
			{
				num718 += WorldGen.genRand.Next(-2, 3);
				num718 = Utils.Clamp(num718, Main.worldSurface + 30, Main.worldSurface + 50.0);

				bool flag49 = false;
				for (int num720 = (int)WorldGen.worldSurfaceLow; num720 < num718; num720++)
					if (Main.tile[num719, num720].HasTile)
					{
						if (Main.tile[num719, num720].TileType == 53 && num719 >= crimsonLeft + WorldGen.genRand.Next(5) && num719 <= crimsonRight - WorldGen.genRand.Next(5))
							Main.tile[num719, num720].TileType = 234;

						if (Main.tile[num719, num720].TileType == 0 && num720 < Main.worldSurface - 1.0 && !flag49)
						{
							VanillaInterface.GrassSpread.Value = 0;
							WorldGen.SpreadGrass(num719, num720, 0, 199);
						}

						flag49 = true;
						if (Main.tile[num719, num720].WallType == 216)
							Main.tile[num719, num720].WallType = 218;
						else if (Main.tile[num719, num720].WallType == 187)
							Main.tile[num719, num720].WallType = 221;

						switch (Main.tile[num719, num720].TileType)
						{
							case 1:
							{
								if (num719 >= crimsonLeft + WorldGen.genRand.Next(5) && num719 <= crimsonRight - WorldGen.genRand.Next(5))
									Main.tile[num719, num720].TileType = 203;
								break;
							}
							case 2:
								Main.tile[num719, num720].TileType = 199;
								break;
							case 161:
								Main.tile[num719, num720].TileType = 200;
								break;
							case 396:
								Main.tile[num719, num720].TileType = 401;
								break;
							case 397:
								Main.tile[num719, num720].TileType = 399;
								break;
						}
					}
			}

			int num721 = WorldGen.genRand.Next(10, 15);
			for (int num722 = 0; num722 < num721; num722++)
			{
				int num723 = 0;
				bool flag50 = false;
				int num724 = 0;
				while (!flag50)
				{
					num723++;
					int num725 = WorldGen.genRand.Next(crimsonLeft - num724, crimsonRight + num724);
					int num726 = WorldGen.genRand.Next((int)(Main.worldSurface - num724 / 2f), (int)(Main.worldSurface + 100 + num724));
					while (WorldGen.oceanDepths(num725, num726))
					{
						num725 = WorldGen.genRand.Next(crimsonLeft - num724, crimsonRight + num724);
						num726 = WorldGen.genRand.Next((int)(Main.worldSurface - num724 / 2f), (int)(Main.worldSurface + 100 + num724));
					}

					if (num723 > 100)
					{
						num724++;
						num723 = 0;
					}

					if (!Main.tile[num725, num726].HasTile)
					{
						for (; !Main.tile[num725, num726].HasTile; num726++)
						{
						}

						num726--;
					}
					else
					{
						while (Main.tile[num725, num726].HasTile && num726 > Main.worldSurface) num726--;
					}

					if ((num724 > 10 || (Main.tile[num725, num726 + 1].HasTile && Main.tile[num725, num726 + 1].TileType == 203)) && !WorldGen.IsTileNearby(num725, num726, 26, 3))
					{
						WorldGen.Place3x2(num725, num726, 26, 1);
						if (Main.tile[num725, num726].TileType == 26)
							flag50 = true;
					}

					if (num724 > 100)
						flag50 = true;
				}
			}
		}

		WorldGen.CrimPlaceHearts();
	}
}