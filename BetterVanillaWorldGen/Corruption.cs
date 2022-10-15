namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class Corruption : ControlledWorldGenPass
{
    private List<(int start, int end)> OtherBiomes;

    public Corruption() : base("Corruption", 1094.237f)
    {
        OtherBiomes = new List<(int start, int end)>();
    }

    protected override void ApplyPass()
    {
        int biomeNumber = (int)OverhauledWorldGenConfigurator.Configuration.Next("Evil")
            .Get<JsonRange>("BiomeAmount").GetRandom(WorldGen.genRand);
        bool oldCrimson = WorldGen.crimson;
        int middlePadding = OptionHelper.OptionsContains("Drunk.Crimruption") ? 100 : 200;

        OtherBiomes.Add((Main.maxTilesX / 2 - middlePadding, Main.maxTilesX / 2 + middlePadding)); //Center
        OtherBiomes.Add((WorldGen.UndergroundDesertLocation.Left, WorldGen.UndergroundDesertLocation.Right)); //Desert
        OtherBiomes.Add((WorldGen.snowOriginLeft, WorldGen.snowOriginRight)); //Snow
        OtherBiomes.Add((VanillaInterface.JungleLeft - 10, VanillaInterface.JungleRight + 10)); //Jungle
        OtherBiomes.Add(WorldGen.dungeonSide < 0 ? (0, 400) : (Main.maxTilesX - 400, Main.maxTilesX)); // Dungeon beach
        OtherBiomes.Add((WorldGen.dungeonLocation - 100, WorldGen.dungeonLocation + 100)); // Dungeon
        if (OptionHelper.OptionsContains("Drunk.Crimruption"))
        {
            bool isOdd = biomeNumber % 2 == 0;
            biomeNumber /= 2;
            int crimsonNumber = biomeNumber;
            int corruptionNumber = biomeNumber;
            if (isOdd)
            {
                if (WorldGen.genRand.NextBool(2))
                    crimsonNumber += 1;
                else
                    corruptionNumber += 1;
            }

            bool left = WorldGen.genRand.NextBool(2);
            GenerateCrimson(crimsonNumber, left);
            GenerateCorruption(corruptionNumber, !left);
        }
        else if (oldCrimson)
        {
            GenerateCrimson(biomeNumber, true);
        }
        else
        {
            GenerateCorruption(biomeNumber, true);
        }

        WorldGen.crimson = oldCrimson;
    }

    private void GenerateCorruption(double biomeNumber, bool left)
    {
        Progress.Message = Lang.gen[20].Value;

        WorldGen.crimson = false;
        for (int biome = 0; biome < biomeNumber; biome++)
        {
            Progress.Set(biome, (float)biomeNumber);
            (int corruptionLeft, int corruptionCenter, int corruptionRight) = FindSuitableCenter(left);

            int minY = (from floatingIslandInfo in VanillaInterface.FloatingIslandInfos
                let islandX = floatingIslandInfo.X
                where corruptionLeft - 100 < islandX && corruptionRight + 100 > islandX
                select floatingIslandInfo.Y + 50).Prepend((int)WorldGen.worldSurfaceLow - 50).Max();

            MakeSingleCorruptionBiome(corruptionLeft, corruptionRight, corruptionCenter, minY);
        }
    }

    public static void MakeSingleCorruptionBiome(int corruptionLeft, int corruptionRight, int corruptionCenter,
        int minY)
    {
        for (int y = minY; y < Main.worldSurface - 1; y++)
            if (Main.tile[corruptionCenter, y].HasTile || Main.tile[corruptionCenter, y].WallType > 0)
            {
                WorldGen.ChasmRunner(corruptionCenter, y, WorldGen.genRand.Next(150, 300), true);
            }

        int pitSpacing = 20;
        for (int x = corruptionCenter; x > corruptionLeft; x--)
        {
            CorruptColumn(corruptionLeft, corruptionRight, corruptionCenter, minY, x, ref pitSpacing);
            pitSpacing--;
        }

        pitSpacing = 19;
        for (int x = corruptionCenter + 1; x < corruptionRight; x++)
        {
            CorruptColumn(corruptionLeft, corruptionRight, corruptionCenter, minY, x, ref pitSpacing);
            pitSpacing--;
        }

        double deepEnough = WorldGen.worldSurfaceHigh + 60.0;

        for (int x = corruptionLeft; x < corruptionRight; x++)
        {
            bool flag52 = false;
            for (int y = minY; y < deepEnough; y++)
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
                    if (Main.tile[x, y].TileType == 1 && x >= corruptionLeft + WorldGen.genRand.Next(5) &&
                        x <= corruptionRight - WorldGen.genRand.Next(5))
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

        #region protecc the orbs

        for (int x1 = corruptionLeft; x1 < corruptionRight; x1 += 2)
        for (int y1 = 0; y1 < Main.maxTilesY - 50; y1 += 2) // Main.maxTilesY - 50 is too deep
            if (Main.tile[x1, y1].HasTile && Main.tile[x1, y1].TileType == TileID.ShadowOrbs)
            {
                int xMin = Math.Max(x1 - 13, 10);
                int xMax = Math.Min(x1 + 13, Main.maxTilesX - 10);
                int yMin = Math.Max(y1 - 13, 10);
                int yMax = Math.Min(y1 + 13, Main.maxTilesY - 10);
                for (int x2 = xMin; x2 <= xMax; x2++)
                for (int y2 = yMin; y2 <= yMax; y2++)
                {
                    Tile tile = Main.tile[x2, y2];
                    int xDiff = Math.Abs(x2 - x1);
                    int yDiff = Math.Abs(y2 - y1);
                    if (tile.HasTile && tile.TileType == TileID.ShadowOrbs)
                    {
                    }
                    else if (xDiff <= 2 + WorldGen.genRand.Next(3) &&
                             yDiff <= 2 + WorldGen.genRand.Next(3))
                        WorldGen.KillTile(x2, y2);
                    else if (xDiff + yDiff < 9 + WorldGen.genRand.Next(11) &&
                             WorldGen.genRand.NextBool(2, 3))
                        WorldGen.PlaceTile(x2, y2, TileID.Ebonstone, true);
                }
            }

        #endregion
    }

    private static void CorruptColumn(int corruptionLeft, int corruptionRight, int corruptionCenter, int minY, int x,
        ref int pitSpacing)
    {
        if (pitSpacing <= 0 && WorldGen.genRand.NextBool(35))
            for (int y = minY; y < Main.worldSurface - 1; y++)
                if (Main.tile[x, y].HasTile || Main.tile[x, y].WallType > 0)
                {
                    if (x == corruptionCenter)
                    {
                        pitSpacing = 20;
                    }
                    else
                    {
                        pitSpacing = 30;
                        WorldGen.ChasmRunner(x, y, WorldGen.genRand.Next(50, 100), true);
                    }

                    break;
                }

        for (int y = (int)WorldGen.worldSurfaceLow; y < Main.worldSurface - 1.0; y++)
            if (Main.tile[x, y].HasTile)
            {
                int num741 = y + WorldGen.genRand.Next(10, 14);
                for (int num742 = y; num742 < num741; num742++)
                    if (Main.tile[x, num742].TileType is 59 or 60 &&
                        x >= corruptionLeft + WorldGen.genRand.Next(5) &&
                        x < corruptionRight - WorldGen.genRand.Next(5))
                        Main.tile[x, num742].TileType = 0;

                break;
            }
    }

    private (int left, int center, int right) FindSuitableCenter(bool left)
    {
        int pity = 0;
        JsonRange biomeSideSize = OverhauledWorldGenConfigurator.Configuration.Next("Evil")
            .Get<JsonRange>("EvilBiomeSizeAroundCenter");
        while (true)
        {
            int half = Main.maxTilesX / 2;
            double doubleCenter = biomeSideSize.GetRandom(WorldGen.genRand);
            int center = (int)Math.Round(doubleCenter);
            int biomeSize = (int)(doubleCenter + biomeSideSize.GetRandom(WorldGen.genRand));

            int min = left ? WorldGen.evilBiomeBeachAvoidance : half;
            int currentX = min;
            int max = left && OptionHelper.OptionsContains("Drunk.Crimruption")
                ? half
                : Main.maxTilesX - WorldGen.evilBiomeBeachAvoidance;
            int allowedX = 0;

            List<(int start, int size)> skips = new();

            while (true)
            {
                int nextX = max;
                int nextIndex = -1;
                for (int i = 0; i < OtherBiomes.Count; i++)
                {
                    int start = OtherBiomes[i].start + pity;
                    if (start < nextX)
                    {
                        nextX = start;
                        nextIndex = i;
                    }
                }

                if (nextX > currentX + biomeSize) allowedX = nextX - currentX - biomeSize;

                if (nextIndex == -1) break;

                if (nextX > currentX)
                {
                    int start = Math.Max(nextX - biomeSize, currentX);
                    int end = Math.Max(OtherBiomes[nextIndex].end - pity, start);
                    if (start != end)
                    {
                        skips.Add((start, end - start));
                        currentX = end;
                    }
                }

                OtherBiomes.RemoveAt(nextIndex);
            }

            if (allowedX == 0)
            {
                pity += 50;
                continue;
            }

            int x = min + WorldGen.genRand.Next(allowedX);
            foreach ((int start, int size) in skips)
                if (start <= x)
                    x += size;
                else
                    break;

            OtherBiomes.Add((x, x + biomeSize));
            return (x, x + center, x + biomeSize);
        }
    }

    private void GenerateCrimson(double biomeNumber, bool left)
    {
        Progress.Message = Lang.gen[72].Value;

        WorldGen.crimson = true;

        for (int biome = 0; biome < biomeNumber; biome++)
        {
            Progress.Set(biome, (float)biomeNumber);
            (int crimsonLeft, int crimsonCenter, int crimsonRight) = FindSuitableCenter(left);


            int minY = (from floatingIslandInfo in VanillaInterface.FloatingIslandInfos
                let islandX = floatingIslandInfo.X
                where crimsonRight - 100 < islandX && crimsonRight + 100 > islandX
                select floatingIslandInfo.Y + 50).Prepend((int)WorldGen.worldSurfaceLow - 50).Max();

            WorldGen.CrimStart(crimsonCenter, minY - 10);
            for (int x = crimsonLeft; x < crimsonRight; x++)
            for (int y = minY; y < Main.worldSurface - 1.0; y++)
                if (Main.tile[x, y].HasTile)
                {
                    int num716 = y + WorldGen.genRand.Next(10, 14);
                    for (int num717 = y; num717 < num716; num717++)
                        if (Main.tile[x, num717].TileType is 59 or 60 && x >= crimsonLeft + WorldGen.genRand.Next(5) &&
                            x < crimsonRight - WorldGen.genRand.Next(5))
                            Main.tile[x, num717].TileType = 0;

                    break;
                }

            double worldTop = WorldGen.worldSurfaceHigh + 60.0;

            for (int x = crimsonLeft; x < crimsonRight; x++)
            {
                bool flag49 = false;
                for (int y = minY; y < worldTop; y++)
                    if (Main.tile[x, y].HasTile)
                    {
                        if (Main.tile[x, y].TileType == 53 && x >= crimsonLeft + WorldGen.genRand.Next(5) &&
                            x <= crimsonRight - WorldGen.genRand.Next(5))
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
                                if (x >= crimsonLeft + WorldGen.genRand.Next(5) &&
                                    x <= crimsonRight - WorldGen.genRand.Next(5))
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
                    int num726 = WorldGen.genRand.Next((int)(Main.worldSurface - num724 / 2f),
                        (int)(Main.worldSurface + 100 + num724));
                    while (WorldGen.oceanDepths(num725, num726))
                    {
                        num725 = WorldGen.genRand.Next(crimsonLeft - num724, crimsonRight + num724);
                        num726 = WorldGen.genRand.Next((int)(Main.worldSurface - num724 / 2f),
                            (int)(Main.worldSurface + 100 + num724));
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

                    if ((num724 > 10 || (Main.tile[num725, num726 + 1].HasTile &&
                                         Main.tile[num725, num726 + 1].TileType == 203)) &&
                        !WorldGen.IsTileNearby(num725, num726, 26, 3))
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