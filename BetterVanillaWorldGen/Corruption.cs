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
		const int num700 = 10;
		VanillaInterface.JungleLeft -= num700;
		VanillaInterface.JungleRight += num700;
		int beachPadding = WorldGen.oceanDistance * 2;
		const int num702 = 100;
		double biomeNumber = Main.maxTilesX * 0.00045;
		bool oldCrimson = WorldGen.crimson;
		if (API.OptionsContains("Drunk.Crimruption"))
		{
			biomeNumber /= 2.0;
			bool flag47 = WorldGen.genRand.NextBool(2);
			GenerateCrimson(biomeNumber, flag47, beachPadding, num702);
			GenerateCorruption(biomeNumber, flag47, beachPadding, num702);
		}
		else if (oldCrimson)
		{
			GenerateCrimson(biomeNumber, true, beachPadding, num702);
		}
		else
		{
			GenerateCorruption(biomeNumber, true, beachPadding, num702);
		}

		WorldGen.crimson = oldCrimson;
	}

	private void GenerateCorruption(double biomeNumber, bool flag47, int beachPadding, int num702)
	{
		Progress.Message = Lang.gen[20].Value;

		WorldGen.crimson = false;

		int corruptionMin = Math.Max(WorldGen.evilBiomeBeachAvoidance, 0);
		int corruptionMax = Main.maxTilesX - Math.Min(WorldGen.evilBiomeBeachAvoidance, 0);

		for (int biome = 0; biome < biomeNumber; biome++)
		{
			int num728 = WorldGen.snowOriginLeft;
			int num729 = WorldGen.snowOriginRight;
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

				if (corruptionLeft < WorldGen.dungeonLocation + num702 && corruptionRight > WorldGen.dungeonLocation - num702)
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

			int minY = (int)WorldGen.worldSurfaceLow;
			for (int index = 0; index < WorldGen.floatingIslandHouseX.Length; index++)
			{
				int islandX = WorldGen.floatingIslandHouseX[index];
				if (corruptionLeft - 100 < islandX && corruptionRight + 100 > islandX)
					minY = Math.Max(minY, WorldGen.floatingIslandHouseY[index] + 30);
			}

			int num737 = 0;
			for (int x = corruptionLeft; x < corruptionRight; x++)
			{
				if (num737 > 0)
					num737--;

				if (x == num732 || num737 == 0)
					for (int y = minY; y < Main.worldSurface - 1.0; y++)
						if (Main.tile[x, y].HasTile || Main.tile[x, y].WallType > 0)
						{
							if (x == num732)
							{
								num737 = 20;
								WorldGen.ChasmRunner(x, y, WorldGen.genRand.Next(150) + 150, true);
							}
							else if (WorldGen.genRand.Next(35) == 0 && num737 == 0)
							{
								num737 = 30;
								WorldGen.ChasmRunner(x, y, WorldGen.genRand.Next(50) + 50, true);
							}
							break;
						}

				for (int num740 = (int)WorldGen.worldSurfaceLow; num740 < Main.worldSurface - 1.0; num740++)
					if (Main.tile[x, num740].HasTile)
					{
						int num741 = num740 + WorldGen.genRand.Next(10, 14);
						for (int num742 = num740; num742 < num741; num742++)
							if (Main.tile[x, num742].TileType is 59 or 60 && x >= corruptionLeft + WorldGen.genRand.Next(5) && x < corruptionRight - WorldGen.genRand.Next(5))
								Main.tile[x, num742].TileType = 0;

						break;
					}
			}

			double worldTop = WorldGen.worldSurfaceHigh + 60.0;

			for (int index = 0; index < WorldGen.floatingIslandHouseX.Length; index++)
			{
				int islandX = WorldGen.floatingIslandHouseX[index];
				if (corruptionLeft - 100 < islandX && corruptionRight + 100 > islandX) 
					worldTop = Math.Max(worldTop, WorldGen.floatingIslandHouseY[index] + 30);
			}
			
			for (int x = corruptionLeft; x < corruptionRight; x++)
			{
				bool flag52 = false;
				for (int y = minY; y < worldTop; y++)
					if (Main.tile[x, y].HasTile)
					{
						if (Main.tile[x, y].TileType == 53 && x >= corruptionLeft + WorldGen.genRand.Next(5) && 
						    x <= corruptionRight - WorldGen.genRand.Next(5))
							Main.tile[x, y].TileType = 112;

						if (Main.tile[x, y].TileType == 0 && y < Main.worldSurface - 1.0 && !flag52)
						{
							WorldGen.grassSpread = 0;
							WorldGen.SpreadGrass(x, y, 0, 23);
						}

						flag52 = true;
						if (Main.tile[x, y].TileType == 1 && x >= corruptionLeft + WorldGen.genRand.Next(5) && x <= corruptionRight - WorldGen.genRand.Next(5))
							Main.tile[x, y].TileType = 25;

						Main.tile[x, y].WallType = Main.tile[x, y].WallType switch
						{
							216 => 217,
							187 => 220,
							_ => Main.tile[x, y].WallType
						};

						Main.tile[x, y].TileType = Main.tile[x, y].TileType switch
						{
							2 => 23,
							161 => 163,
							396 => 400,
							397 => 398,
							_ => Main.tile[x, y].TileType
						};
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

	private void GenerateCrimson(double biomeNumber, bool flag47, int beachPadding, int num702)
	{
		Progress.Message = Lang.gen[72].Value;

		WorldGen.crimson = true;

		int crimsonMin = Math.Max(WorldGen.evilBiomeBeachAvoidance, WorldGen.dungeonSide == -1 ? beachPadding * 4 / 5 : 0);
		int crimsonMax = Main.maxTilesX - Math.Min(WorldGen.evilBiomeBeachAvoidance, WorldGen.dungeonSide == 1 ? beachPadding * 4 / 5 : 0);

		for (int biome = 0; biome < biomeNumber; biome++)
		{
			int num705 = WorldGen.snowOriginLeft;
			int num706 = WorldGen.snowOriginRight;
			int num707 = VanillaInterface.JungleLeft;
			int num708 = VanillaInterface.JungleRight;
			Progress.Set(biome, (float)biomeNumber);
			bool flag48 = false;
			int crimX = 0;
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
					crimX = flag47 ? WorldGen.genRand.Next(beachPadding, (int)(Main.maxTilesX * 0.5)) : WorldGen.genRand.Next((int)(Main.maxTilesX * 0.5), Main.maxTilesX - beachPadding);
				}
				else
				{
					crimX = WorldGen.genRand.Next(beachPadding, Main.maxTilesX - beachPadding);
				}

				crimsonLeft = crimX - WorldGen.genRand.Next(200) - 100;
				crimsonRight = crimX + WorldGen.genRand.Next(200) + 100;
				crimsonLeft = Utils.Clamp(crimsonLeft, crimsonMin, crimsonMax);
				crimsonRight = Utils.Clamp(crimsonRight, crimsonMin, crimsonMax);

				if (crimX < crimsonLeft + WorldGen.evilBiomeAvoidanceMidFixer)
					crimX = crimsonLeft + WorldGen.evilBiomeAvoidanceMidFixer;

				if (crimX > crimsonRight - WorldGen.evilBiomeAvoidanceMidFixer)
					crimX = crimsonRight - WorldGen.evilBiomeAvoidanceMidFixer;

				if (crimX > num712 - num713 && crimX < num712 + num713)
					flag48 = false;

				if (crimsonLeft > num712 - num713 && crimsonLeft < num712 + num713)
					flag48 = false;

				if (crimsonRight > num712 - num713 && crimsonRight < num712 + num713)
					flag48 = false;

				if (crimX > WorldGen.UndergroundDesertLocation.X && crimX < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
					flag48 = false;

				if (crimsonLeft > WorldGen.UndergroundDesertLocation.X && crimsonLeft < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
					flag48 = false;

				if (crimsonRight > WorldGen.UndergroundDesertLocation.X && crimsonRight < WorldGen.UndergroundDesertLocation.X + WorldGen.UndergroundDesertLocation.Width)
					flag48 = false;

				if (crimsonLeft < WorldGen.dungeonLocation + num702 && crimsonRight > WorldGen.dungeonLocation - num702)
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

			int crimY = (int)WorldGen.worldSurfaceLow;
			for (int index = 0; index < WorldGen.floatingIslandHouseX.Length; index++)
			{
				int islandX = WorldGen.floatingIslandHouseX[index];
				if (crimsonLeft - 100 < islandX && crimsonRight + 100 > islandX)
					crimY = Math.Max(crimY, WorldGen.floatingIslandHouseY[index] + 30);
			}

			WorldGen.CrimStart(crimX, crimY - 10);
			for (int x = crimsonLeft; x < crimsonRight; x++)
			for (int y = crimY; y < Main.worldSurface - 1.0; y++)
				if (Main.tile[x, y].HasTile)
				{
					int num716 = y + WorldGen.genRand.Next(10, 14);
					for (int num717 = y; num717 < num716; num717++)
						if (Main.tile[x, num717].TileType is 59 or 60 && x >= crimsonLeft + WorldGen.genRand.Next(5) && x < crimsonRight - WorldGen.genRand.Next(5))
							Main.tile[x, num717].TileType = 0;

					break;
				}

			double worldTop = WorldGen.worldSurfaceHigh + 60.0;

			for (int index = 0; index < WorldGen.floatingIslandHouseX.Length; index++)
			{
				int islandX = WorldGen.floatingIslandHouseX[index];
				if (crimsonLeft - 100 < islandX && crimsonRight + 100 > islandX) 
					worldTop = Math.Max(worldTop, WorldGen.floatingIslandHouseY[index] + 30);
			}
			
			for (int x = crimsonLeft; x < crimsonRight; x++)
			{
				bool flag49 = false;
				for (int y = crimY; y < worldTop; y++)
					if (Main.tile[x, y].HasTile)
					{
						if (Main.tile[x, y].TileType == 53 && x >= crimsonLeft + WorldGen.genRand.Next(5) && x <= crimsonRight - WorldGen.genRand.Next(5))
							Main.tile[x, y].TileType = 234;

						if (Main.tile[x, y].TileType == 0 && y < Main.worldSurface - 1.0 && !flag49)
						{
							WorldGen.grassSpread = 0;
							WorldGen.SpreadGrass(x, y, 0, 199);
						}

						flag49 = true;
						Main.tile[x, y].WallType = Main.tile[x, y].WallType switch
						{
							216 => 218,
							187 => 221,
							_ => Main.tile[x, y].WallType
						};

						switch (Main.tile[x, y].TileType)
						{
							case 1:
							{
								if (x >= crimsonLeft + WorldGen.genRand.Next(5) && x <= crimsonRight - WorldGen.genRand.Next(5))
									Main.tile[x, y].TileType = 203;
								break;
							}
							case 2:
								Main.tile[x, y].TileType = 199;
								break;
							case 161:
								Main.tile[x, y].TileType = 200;
								break;
							case 396:
								Main.tile[x, y].TileType = 401;
								break;
							case 397:
								Main.tile[x, y].TileType = 399;
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