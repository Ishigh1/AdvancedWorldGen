namespace AdvancedWorldGen.BetterVanillaWorldGen;

public static class GenerationChests
{
	private static int HellChest;
	private static List<int> HellChestItem = null!;
	private static bool GeneratedShadowKey;

	public static void ShuffleChests(UnifiedRandom unifiedRandom)
	{
		GeneratedShadowKey = false;
		HellChest = 0;

		HellChestItem = new List<int> { ItemID.Sunfury, ItemID.Flamelash, ItemID.DarkLance, ItemID.HellwingBow };
		if (WorldGen.remixWorldGen)
			HellChestItem.Add(ItemID.UnholyTrident);
		else
			HellChestItem.Add(ItemID.FlowerofFire);
		for (int index = 0; index < HellChestItem.Count - 1; index++)
		{
			int indexToExchange = unifiedRandom.Next(HellChestItem.Count - index);
			if (indexToExchange != index)
				(HellChestItem[index], HellChestItem[indexToExchange]) =
					(HellChestItem[indexToExchange], HellChestItem[index]);
		}
	}

	public static bool AddBuriedChest(int chestRightX, int y, int contain = 0,
		bool notNearOtherChests = false, int style = -1, ushort chestTileType = TileID.Containers)
	{
		int chestLeftX = chestRightX - 1;

		if (WorldGen.remixWorldGen && chestLeftX > Main.maxTilesX * 0.37 && chestRightX < Main.maxTilesX * 0.63 &&
		    y > Main.maxTilesY - 250)
			return false;

		int groundY;
		for (groundY = y;; groundY++)
		{
			if (Main.tile[chestRightX, groundY].LiquidType == LiquidID.Shimmer)
				return false;

			const int spread = 2;
			for (int n = chestRightX - spread - 1; n <= chestRightX + spread; n++)
			for (int num6 = groundY - spread; num6 <= groundY + spread; num6++)
				if (Main.tile[n, num6].HasTile && (TileID.Sets.Boulders[Main.tile[n, num6].TileType] ||
				                                   Main.tile[n, num6].TileType is 26 or 237))
					return false;

			if (WorldGen.SolidTile(chestRightX, groundY) && WorldGen.SolidTile(chestLeftX, groundY))
				break;
			if (groundY == y + 10)
				return false;
		}

		int chestBottomY = groundY - 1;

		PreLoot(chestRightX, groundY,
			ref contain, style,
			ref chestTileType, out bool shadowChest, out style, out bool desertBiome,
			out bool iceBiome, out bool jungleBiome, out bool underworld,
			out bool water, out bool livingWood, out bool glowingMushroomBiome, out bool dungeon, out bool pyramid,
			out bool sky);

		int chestIndex = WorldGen.PlaceChest(chestLeftX, chestBottomY, chestTileType, notNearOtherChests, style);
		if (chestIndex < 0)
			return false;

		VanillaLoot(chestRightX, groundY,
			contain, chestTileType, shadowChest, chestIndex, style, pyramid, water, livingWood,
			dungeon, glowingMushroomBiome, desertBiome, iceBiome, jungleBiome, underworld, sky);

		return true;
	}

	private static void PreLoot(int x, int y,
		ref int contain, int style, ref ushort chestTileType,
		out bool shadowChest, out int outStyle,
		out bool desertBiome, out bool iceBiome, out bool jungleBiome, out bool underworld,
		out bool water, out bool livingWood, out bool glowingMushroomBiome,
		out bool dungeon, out bool pyramid, out bool sky)
	{
		if (chestTileType == 0)
			chestTileType = TileID.Containers;

		iceBiome = false;
		desertBiome = false;
		jungleBiome = false;
		water = false;
		livingWood = false;
		glowingMushroomBiome = false;
		underworld = false;
		dungeon = false;
		pyramid = false;
		shadowChest = false;
		sky = false; //flag10 in 1.4.4.9
		int angelChances = 15;
		if (WorldGen.tenthAnniversaryWorldGen)
			angelChances *= 3;

		outStyle = 0;
		bool underground = y >= Main.worldSurface + 25.0; //flag12 in 1.4.4.9

		bool remixUnderground;
		if (WorldGen.remixWorldGen)
			remixUnderground = y < Main.maxTilesY - 400;
		else
			remixUnderground = underground;

		if (remixUnderground || contain > ItemID.None)
			outStyle = 1;

		if (style >= 0)
			outStyle = style;

		if (contain == ItemID.None && y <= Main.maxTilesY - 205 && Desert.IsUndergroundDesert(x, y))
		{
			outStyle = 10;
			chestTileType = TileID.Containers2;
		}

		switch (chestTileType)
		{
			case TileID.Containers2 when outStyle == 10:
			{
				desertBiome = true;

				if (WorldGen.getGoodWorldGen && WorldGen.genRand.NextBool(angelChances))
					contain = ItemID.AngelStatue;
				else
					contain = y <= (GenVars.desertHiveHigh * 3 + GenVars.desertHiveLow * 4) / 7
						? Utils.SelectRandom(WorldGen.genRand, ItemID.AncientChisel, ItemID.SandBoots,
							ItemID.MysticCoilSnake, ItemID.MagicConch)
						: Utils.SelectRandom(WorldGen.genRand, ItemID.ThunderSpear, ItemID.ThunderStaff,
							ItemID.DripplerFlail);
				return;
			}
			case TileID.Containers when outStyle == 11 || (contain == ItemID.None &&
			                                               underground &&
			                                               y <= Main.maxTilesY - 205 &&
			                                               Main.tile[x, y].TileType is TileID.SnowBlock
				                                               or TileID.IceBlock
				                                               or TileID.BreakableIce):
			{
				iceBiome = true;
				outStyle = 11;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.NextBool(angelChances))
					contain = ItemID.AngelStatue;
				else if (WorldGen.genRand.NextBool(50))
					contain = ItemID.Fish;
				else if (WorldGen.genRand.NextBool(20))
					contain = ItemID.Extractinator;
				else
					contain = WorldGen.genRand.Next(6) switch
					{
						0 => ItemID.IceBoomerang,
						1 => ItemID.IceBlade,
						2 => ItemID.IceSkates,
						3 => WorldGen.remixWorldGen ? ItemID.IceBow : ItemID.SnowballCannon,
						4 => ItemID.BlizzardinaBottle,
						_ => ItemID.FlurryBoots
					};
				return;
			}
			case TileID.Containers when
				style == 10 || contain is ItemID.FeralClaws or ItemID.AnkletoftheWind or ItemID.StaffofRegrowth
					or ItemID.Seaweed:
			{
				jungleBiome = true;
				outStyle = 10;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.NextBool(angelChances))
					contain = ItemID.AngelStatue;
				return;
			}
			case TileID.Containers when y > Main.maxTilesY - 205 && contain == ItemID.None:
			{
				underworld = true;
				contain = HellChestItem[HellChest];
				outStyle = 4;
				shadowChest = true;

				if (WorldGen.getGoodWorldGen && WorldGen.genRand.NextBool(angelChances))
					contain = ItemID.AngelStatue;
				return;
			}
			case TileID.Containers when outStyle == 17:
			{
				water = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.NextBool(angelChances))
					contain = ItemID.AngelStatue;
				return;
			}
			case TileID.Containers when outStyle == 12:
			{
				livingWood = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.NextBool(angelChances))
					contain = ItemID.AngelStatue;
				return;
			}
			case TileID.Containers when outStyle == 32:
			{
				glowingMushroomBiome = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.NextBool(angelChances))
					contain = ItemID.AngelStatue;
				return;
			}
			case TileID.Containers when outStyle != 0 && DungeonPass.IsDungeon(x, y):
			{
				dungeon = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.NextBool(angelChances))
					contain = ItemID.AngelStatue;
				return;
			}
			case TileID.Containers when outStyle != 0 &&
			                            contain is ItemID.PharaohsMask or ItemID.SandstorminaBottle
				                            or ItemID.FlyingCarpet:
				pyramid = true;
				return;
			case TileID.Containers when outStyle == 13 ||
			                            contain is ItemID.ShinyRedBalloon or ItemID.Starfury
				                            or ItemID.LuckyHorseshoe or ItemID.CelestialMagnet:
				sky = true;
				if (WorldGen.remixWorldGen && !WorldGen.everythingWorldGen)
				{
					if (WorldGen.crimson)
					{
						outStyle = 43;
					}
					else
					{
						chestTileType = TileID.Containers2;
						outStyle = 3;
					}
				}

				return;
		}

		if (WorldGen.everythingWorldGen && outStyle == 1 && chestTileType == TileID.Containers &&
		    !WorldGen.genRand.NextBool(3))
		{
			chestTileType = TileID.Containers2;
			outStyle = 4;
		}
	}


	private static void VanillaLoot(int x, int y, int contain, ushort chestTileType, bool shadowChest,
		int chestIndex, int style, bool pyramid, bool water, bool livingWood, bool dungeon,
		bool glowingMushroomBiome, bool desertBiome, bool iceBiome, bool jungleBiome, bool underworld, bool sky)
	{
		if (shadowChest)
			if (++HellChest == HellChestItem.Count)
				HellChest = 0;

		Chest chest = Main.chest[chestIndex];
		int index = 0;
		while (index == 0)
		{
			bool underground;
			if (WorldGen.remixWorldGen)
				underground = y >= (Main.rockLayer + (Main.maxTilesY - 350) * 2) / 3.0;
			else
				underground = y < Main.worldSurface + 25.0;
			if ((style == 0 && underground) || pyramid)
			{
				if (contain > 0)
				{
					chest.item[index].SetDefaults(contain);
					chest.item[index].Prefix(-1);
					index++;
					switch (contain)
					{
						case ItemID.PharaohsMask:
							chest.item[index].SetDefaults(ItemID.PharaohsRobe);
							index++;
							break;
						case ItemID.LivingWoodWand:
							chest.item[index].SetDefaults(ItemID.LeafWand);
							index++;
							if (WorldGen.genRand.NextBool(10))
								switch (WorldGen.genRand.Next(2))
								{
									case 0:
										chest.item[index].SetDefaults(ItemID.SunflowerMinecart);
										index++;
										break;
									case 1:
										chest.item[index].SetDefaults(ItemID.LadybugMinecart);
										index++;
										break;
								}

							break;
					}

					if (WorldGen.tenthAnniversaryWorldGen && pyramid)
					{
						chest.item[index++].SetDefaults(ItemID.PharaohsMask);
						chest.item[index++].SetDefaults(ItemID.PharaohsRobe);
					}
				}
				else
				{
					switch (WorldGen.genRand.Next(10))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.Spear);
							chest.item[index].Prefix(-1);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.Blowpipe);
							chest.item[index].Prefix(-1);
							break;
						case 2:
							chest.item[index].SetDefaults(ItemID.WoodenBoomerang);
							chest.item[index].Prefix(-1);
							break;
						case 3:
							chest.item[index].SetDefaults(ItemID.Aglet);
							chest.item[index].Prefix(-1);
							break;
						case 4:
							chest.item[index].SetDefaults(ItemID.ClimbingClaws);
							chest.item[index].Prefix(-1);
							break;
						case 5:
							chest.item[index].SetDefaults(ItemID.Umbrella);
							chest.item[index].Prefix(-1);
							break;
						case 6:
							chest.item[index].SetDefaults(ItemID.CordageGuide);
							chest.item[index].Prefix(-1);
							break;
						case 7:
							if (WorldGen.remixWorldGen)
								chest.item[index].SetDefaults(ItemID.WandofSparking);
							else
								chest.item[index].SetDefaults(ItemID.MagicDagger);
							chest.item[index].Prefix(-1);
							break;
						case 10:
							chest.item[index].SetDefaults(ItemID.Radar);
							chest.item[index].Prefix(-1);
							break;
						case 11:
							chest.item[index].SetDefaults(ItemID.PortableStool);
							chest.item[index].Prefix(-1);
							break;
					}

					index++;
				}

				if (WorldGen.genRand.NextBool(6))
				{
					chest.item[index].SetDefaults(ItemID.Glowstick);
					chest.item[index++].stack = WorldGen.genRand.Next(40, 75);
				}

				if (WorldGen.genRand.NextBool(6))
				{
					chest.item[index].SetDefaults(ItemID.ThrowingKnife);
					chest.item[index++].stack = WorldGen.genRand.Next(150, 300);
				}

				if (WorldGen.genRand.NextBool(6))
				{
					chest.item[index].SetDefaults(ItemID.HerbBag);
					chest.item[index].stack = 1;
					if (WorldGen.genRand.NextBool(5))
						chest.item[index].stack += WorldGen.genRand.Next(2);
					if (WorldGen.genRand.NextBool(10))
						chest.item[index].stack += WorldGen.genRand.Next(3);
					index++;
				}

				if (WorldGen.genRand.NextBool(6))
				{
					chest.item[index].SetDefaults(ItemID.CanOfWorms);
					chest.item[index].stack = 1;
					if (WorldGen.genRand.NextBool(5))
						chest.item[index].stack += WorldGen.genRand.Next(2);
					if (WorldGen.genRand.NextBool(10))
						chest.item[index].stack += WorldGen.genRand.Next(3);
					index++;
				}

				if (WorldGen.genRand.NextBool(3))
				{
					chest.item[index].SetDefaults(ItemID.Grenade);
					chest.item[index].stack = WorldGen.genRand.Next(3, 6);
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					switch (WorldGen.genRand.Next(2))
					{
						case 0:
							chest.item[index].SetDefaults(GenVars.copperBar);
							break;
						case 1:
							chest.item[index].SetDefaults(GenVars.ironBar);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(8) + 3;
					index++;
				}

				bool dimaryp = _100kWorld.Enabled && pyramid;
				if (WorldGen.genRand.NextBool(2) || dimaryp)
				{
					int ropeStacks = dimaryp ? 10 : 1;
					for (int i = 0; i < ropeStacks; i++)
					{
						chest.item[index].SetDefaults(ItemID.Rope);
						chest.item[index].stack = WorldGen.genRand.Next(50, 101);
						index++;
					}
				}

				if (WorldGen.genRand.Next(3) != 0)
				{
					switch (WorldGen.genRand.Next(2))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.WoodenArrow);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.Shuriken);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(26) + 25;
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					chest.item[index].SetDefaults(ItemID.LesserHealingPotion);
					chest.item[index].stack = WorldGen.genRand.Next(3) + 3;
					index++;
				}

				if (!WorldGen.genRand.NextBool(3))
				{
					chest.item[index].SetDefaults(ItemID.RecallPotion);
					chest.item[index].stack = WorldGen.genRand.Next(3, 6);
					index++;
				}

				if (!WorldGen.genRand.NextBool(3))
				{
					switch (WorldGen.genRand.Next(6))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.IronskinPotion);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.ShinePotion);
							break;
						case 2:
							chest.item[index].SetDefaults(ItemID.NightOwlPotion);
							break;
						case 3:
							chest.item[index].SetDefaults(ItemID.SwiftnessPotion);
							break;
						case 4:
							chest.item[index].SetDefaults(ItemID.MiningPotion);
							break;
						case 5:
							chest.item[index].SetDefaults(ItemID.BuilderPotion);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(1, 3);
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					switch (WorldGen.genRand.Next(2))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.Torch);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.Bottle);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(10, 21);
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					chest.item[index].SetDefaults(ItemID.SilverCoin);
					chest.item[index].stack = WorldGen.genRand.Next(10, 30);
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					chest.item[index].SetDefaults(ItemID.Wood);
					chest.item[index].stack = WorldGen.genRand.Next(50, 100);
					index++;
				}
			}

			else if ((!WorldGen.remixWorldGen && y < Main.rockLayer)
			         || (WorldGen.remixWorldGen && y > Main.rockLayer && y < Main.maxTilesY - 250))
			{
				if (contain > ItemID.None)
				{
					if (contain == ItemID.LivingWoodWand)
					{
						chest.item[index].SetDefaults(ItemID.LeafWand);
						index++;
					}

					chest.item[index].SetDefaults(contain);
					chest.item[index].Prefix(-1);
					index++;
					if (water)
					{
						if (WorldGen.genRand.NextBool(2))
							chest.item[index++].SetDefaults(ItemID.SharkBait);
						if (WorldGen.genRand.NextBool(2))
							chest.item[index++].SetDefaults(ItemID.SandcastleBucket);
					}

					if (sky && WorldGen.genRand.NextBool(40))
						chest.item[index++].SetDefaults(ItemID.CreativeWings);

					if (livingWood && WorldGen.genRand.NextBool(10))
					{
						switch (WorldGen.genRand.Next(2))
						{
							case 0:
								chest.item[index].SetDefaults(ItemID.SunflowerMinecart);
								break;
							case 1:
								chest.item[index].SetDefaults(ItemID.LadybugMinecart);
								break;
						}

						index++;
					}

					if (dungeon && (!GeneratedShadowKey || WorldGen.genRand.NextBool(3)))
					{
						GeneratedShadowKey = true;
						chest.item[index].SetDefaults(ItemID.ShadowKey);
						index++;
					}
				}
				else
				{
					switch (WorldGen.genRand.Next(6))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.BandofRegeneration);
							chest.item[index].Prefix(-1);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.MagicMirror);
							chest.item[index].Prefix(-1);
							break;
						case 2:
							chest.item[index].SetDefaults(ItemID.CloudinaBottle);
							chest.item[index].Prefix(-1);
							break;
						case 3:
							chest.item[index].SetDefaults(ItemID.HermesBoots);
							chest.item[index].Prefix(-1);
							break;
						case 4:
							chest.item[index].SetDefaults(ItemID.Mace);
							chest.item[index].Prefix(-1);
							break;
						default:
							chest.item[index].SetDefaults(ItemID.ShoeSpikes);
							chest.item[index].Prefix(-1);
							break;
					}

					index++;
					if (WorldGen.genRand.NextBool(20))
					{
						chest.item[index].SetDefaults(ItemID.Extractinator);
						chest.item[index].Prefix(-1);
						index++;
					}
					else if (WorldGen.genRand.NextBool(20))
					{
						chest.item[index].SetDefaults(ItemID.FlareGun);
						chest.item[index].Prefix(-1);
						index++;
						chest.item[index].SetDefaults(ItemID.Flare);
						chest.item[index].stack = WorldGen.genRand.Next(26) + 25;
						index++;
					}

					if (glowingMushroomBiome && WorldGen.genRand.NextBool(2))
					{
						chest.item[index].SetDefaults(ItemID.ShroomMinecart);
						index++;
					}

					if (glowingMushroomBiome && WorldGen.genRand.NextBool(3))
					{
						chest.item[index].SetDefaults(ItemID.MushroomHat);
						index++;
						chest.item[index].SetDefaults(ItemID.MushroomVest);
						index++;
						chest.item[index].SetDefaults(ItemID.MushroomPants);
						index++;
					}
				}

				if (desertBiome)
				{
					if (WorldGen.genRand.NextBool(3))
					{
						chest.item[index].SetDefaults(ItemID.ScarabBomb);
						chest.item[index].stack = WorldGen.genRand.Next(10, 20);
						index++;
					}
				}
				else if (WorldGen.genRand.NextBool(3))
				{
					chest.item[index].SetDefaults(ItemID.Bomb);
					chest.item[index].stack = WorldGen.genRand.Next(10, 20);
					index++;
				}

				if (WorldGen.genRand.NextBool(5))
				{
					chest.item[index].SetDefaults(ItemID.AngelStatue);
					index++;
				}

				if (WorldGen.genRand.NextBool(3))
				{
					chest.item[index].SetDefaults(ItemID.Rope);
					chest.item[index].stack = WorldGen.genRand.Next(50, 101);
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					switch (WorldGen.genRand.Next(2))
					{
						case 0:
							chest.item[index].SetDefaults(GenVars.ironBar);
							break;
						case 1:
							chest.item[index].SetDefaults(GenVars.silverBar);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(10) + 5;
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					switch (WorldGen.genRand.Next(2))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.WoodenArrow);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.Shuriken);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(25) + 25;
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					chest.item[index].SetDefaults(ItemID.LesserHealingPotion);
					chest.item[index].stack = WorldGen.genRand.Next(3) + 3;
					index++;
				}

				if (WorldGen.genRand.Next(3) > 0)
				{
					switch (WorldGen.genRand.Next(9))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.RegenerationPotion);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.ShinePotion);
							break;
						case 2:
							chest.item[index].SetDefaults(ItemID.NightOwlPotion);
							break;
						case 3:
							chest.item[index].SetDefaults(ItemID.SwiftnessPotion);
							break;
						case 4:
							chest.item[index].SetDefaults(ItemID.ArcheryPotion);
							break;
						case 5:
							chest.item[index].SetDefaults(ItemID.GillsPotion);
							break;
						case 6:
							chest.item[index].SetDefaults(ItemID.HunterPotion);
							break;
						case 7:
							chest.item[index].SetDefaults(ItemID.MiningPotion);
							break;
						case 8:
							chest.item[index].SetDefaults(ItemID.TrapsightPotion);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(1, 3);
					index++;
				}

				if (WorldGen.genRand.Next(3) != 0)
				{
					chest.item[index].SetDefaults(ItemID.RecallPotion);
					chest.item[index].stack = WorldGen.genRand.Next(2, 5);
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					if (style == 11)
						chest.item[index].SetDefaults(ItemID.IceTorch);
					else
						chest.item[index].SetDefaults(ItemID.Torch);

					chest.item[index].stack = WorldGen.genRand.Next(11) + 10;
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					chest.item[index].SetDefaults(ItemID.SilverCoin);
					chest.item[index].stack = WorldGen.genRand.Next(50, 90);
					index++;
				}
			}
			else if (y < Main.maxTilesY - 250
			         || (WorldGen.remixWorldGen && style is 7 or 14))
			{
				if (contain > 0)
				{
					chest.item[index].SetDefaults(contain);
					chest.item[index].Prefix(-1);
					index++;
					if (iceBiome && WorldGen.genRand.NextBool(5))
					{
						chest.item[index].SetDefaults(ItemID.IceMirror);
						index++;
					}

					if (desertBiome)
					{
						if (WorldGen.genRand.NextBool(7))
						{
							chest.item[index].SetDefaults(ItemID.EncumberingStone);
							index++;
						}

						if (WorldGen.genRand.NextBool(15))
						{
							chest.item[index].SetDefaults(ItemID.DesertMinecart);
							index++;
						}
					}

					if (jungleBiome && WorldGen.genRand.NextBool(6))
					{
						chest.item[index++].SetDefaults(ItemID.LivingMahoganyWand);
						chest.item[index++].SetDefaults(ItemID.LivingMahoganyLeafWand);
					}

					if (jungleBiome && WorldGen.genRand.NextBool(10))
						chest.item[index++].SetDefaults(ItemID.BeeMinecart);

					if (water)
					{
						if (WorldGen.genRand.NextBool(2))
							chest.item[index++].SetDefaults(ItemID.SharkBait);
						if (WorldGen.genRand.NextBool(2))
							chest.item[index++].SetDefaults(ItemID.SandcastleBucket);
					}

					if (dungeon && (!GeneratedShadowKey || WorldGen.genRand.NextBool(3)))
					{
						GeneratedShadowKey = true;
						chest.item[index].SetDefaults(ItemID.ShadowKey);
						index++;
					}
				}
				else
				{
					bool underLava;
					if (WorldGen.remixWorldGen)
						underLava = y > Main.worldSurface && y < Main.rockLayer;
					else
						underLava = y > GenVars.lavaLine;

					int charmChance = WorldGen.tenthAnniversaryWorldGen ? 15 : 20;

					if (WorldGen.genRand.NextBool(charmChance) && underLava)
					{
						chest.item[index].SetDefaults(ItemID.LavaCharm);
						chest.item[index].Prefix(-1);
					}
					else if (WorldGen.genRand.NextBool(15))
					{
						chest.item[index].SetDefaults(ItemID.Extractinator);
						chest.item[index].Prefix(-1);
					}
					else
					{
						switch (WorldGen.genRand.Next(7))
						{
							case 0:
								chest.item[index].SetDefaults(ItemID.BandofRegeneration);
								chest.item[index].Prefix(-1);
								break;
							case 1:
								chest.item[index].SetDefaults(ItemID.MagicMirror);
								chest.item[index].Prefix(-1);
								break;
							case 2:
								chest.item[index].SetDefaults(ItemID.CloudinaBottle);
								chest.item[index].Prefix(-1);
								break;
							case 3:
								chest.item[index].SetDefaults(ItemID.HermesBoots);
								chest.item[index].Prefix(-1);
								break;
							case 4:
								chest.item[index].SetDefaults(ItemID.Mace);
								chest.item[index].Prefix(-1);
								break;
							case 5:
								chest.item[index].SetDefaults(ItemID.ShoeSpikes);
								chest.item[index].Prefix(-1);
								break;
							case 6:
								chest.item[index].SetDefaults(ItemID.FlareGun);
								chest.item[index].Prefix(-1);
								index++;
								chest.item[index].SetDefaults(ItemID.Flare);
								chest.item[index].stack = WorldGen.genRand.Next(26) + 25;
								break;
						}
					}

					index++;
					if (glowingMushroomBiome)
					{
						if (WorldGen.genRand.NextBool(2))
						{
							chest.item[index++].SetDefaults(ItemID.ShroomMinecart);
						}
						else
						{
							chest.item[index++].SetDefaults(ItemID.MushroomHat);
							chest.item[index++].SetDefaults(ItemID.MushroomVest);
							chest.item[index++].SetDefaults(ItemID.MushroomPants);
						}
					}
				}

				if (WorldGen.genRand.NextBool(5))
				{
					chest.item[index].SetDefaults(ItemID.SuspiciousLookingEye);
					index++;
				}

				if (WorldGen.genRand.NextBool(3))
				{
					chest.item[index].SetDefaults(ItemID.Dynamite);
					index++;
				}

				if (WorldGen.genRand.NextBool(4))
				{
					chest.item[index].SetDefaults(ItemID.JestersArrow);
					chest.item[index].stack = WorldGen.genRand.Next(26) + 25;
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					switch (WorldGen.genRand.Next(2))
					{
						case 0:
							chest.item[index].SetDefaults(GenVars.goldBar);
							break;
						case 1:
							chest.item[index].SetDefaults(GenVars.silverBar);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(8) + 3;
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					switch (WorldGen.genRand.Next(2))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.FlamingArrow);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.ThrowingKnife);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(26) + 25;
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					chest.item[index].SetDefaults(ItemID.HealingPotion);
					chest.item[index].stack = WorldGen.genRand.Next(3) + 3;
					index++;
				}

				if (WorldGen.genRand.Next(3) > 0)
				{
					switch (WorldGen.genRand.Next(6))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.SpelunkerPotion);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.FeatherfallPotion);
							break;
						case 2:
							chest.item[index].SetDefaults(ItemID.NightOwlPotion);
							break;
						case 3:
							chest.item[index].SetDefaults(ItemID.WaterWalkingPotion);
							break;
						case 4:
							chest.item[index].SetDefaults(ItemID.ArcheryPotion);
							break;
						case 5:
							chest.item[index].SetDefaults(ItemID.GravitationPotion);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(1, 3);
					index++;
				}

				if (WorldGen.genRand.Next(3) > 1)
				{
					switch (WorldGen.genRand.Next(6))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.ThornsPotion);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.InvisibilityPotion);
							break;
						case 2:
							chest.item[index].SetDefaults(ItemID.HunterPotion);
							break;
						case 3:
							chest.item[index].SetDefaults(ItemID.TrapsightPotion);
							break;
						case 4:
							chest.item[index].SetDefaults(ItemID.TeleportationPotion);
							break;
						case 5:
							chest.item[index].SetDefaults(ItemID.TitanPotion);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(1, 3);
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					chest.item[index].SetDefaults(ItemID.RecallPotion);
					chest.item[index].stack = WorldGen.genRand.Next(2, 5);
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					switch (WorldGen.genRand.Next(2))
					{
						case 0 when style == 11:
							chest.item[index].SetDefaults(ItemID.IceTorch);
							break;
						case 0:
							chest.item[index].SetDefaults(ItemID.Torch);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.Glowstick);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(15) + 15;
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					chest.item[index].SetDefaults(ItemID.GoldCoin);
					chest.item[index].stack = WorldGen.genRand.Next(1, 3);
					index++;
				}
			}
			else
			{
				if (contain > 0)
				{
					chest.item[index].SetDefaults(contain);
					chest.item[index].Prefix(-1);
					index++;
					if (underworld)
					{
						if (WorldGen.genRand.NextBool(5)) chest.item[index++].SetDefaults(ItemID.TreasureMagnet);

						if (WorldGen.genRand.NextBool(10)) chest.item[index++].SetDefaults(ItemID.HellMinecart);

						if (WorldGen.genRand.NextBool(10)) chest.item[index++].SetDefaults(ItemID.HellMinecart);

						if (WorldGen.genRand.NextBool(10)) chest.item[index++].SetDefaults(ItemID.OrnateShadowKey);

						if (WorldGen.genRand.NextBool(10)) chest.item[index++].SetDefaults(ItemID.HellCake);
					}
				}
				else
				{
					switch (WorldGen.genRand.Next(4))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.BandofRegeneration);
							chest.item[index].Prefix(-1);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.MagicMirror);
							chest.item[index].Prefix(-1);
							break;
						case 2:
							chest.item[index].SetDefaults(ItemID.CloudinaBottle);
							chest.item[index].Prefix(-1);
							break;
						case 3:
							chest.item[index].SetDefaults(ItemID.HermesBoots);
							chest.item[index].Prefix(-1);
							break;
					}

					index++;
				}

				if (WorldGen.genRand.NextBool(3))
				{
					chest.item[index].SetDefaults(ItemID.Dynamite);
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					switch (WorldGen.genRand.Next(2))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.MeteoriteBar);
							break;
						case 1:
							chest.item[index].SetDefaults(GenVars.goldBar);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(15) + 15;
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					switch (WorldGen.genRand.Next(2))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.HellfireArrow);
							break;
						case 1 when WorldGen.SavedOreTiers.Silver == 168:
							chest.item[index].SetDefaults(ItemID.TungstenBullet);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.SilverBullet);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(25) + 50;
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					chest.item[index].SetDefaults(ItemID.RestorationPotion);
					chest.item[index].stack = WorldGen.genRand.Next(6) + 15;
					index++;
				}

				if (WorldGen.genRand.Next(4) > 0)
				{
					switch (WorldGen.genRand.Next(8))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.SpelunkerPotion);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.FeatherfallPotion);
							break;
						case 2:
							chest.item[index].SetDefaults(ItemID.ManaRegenerationPotion);
							break;
						case 3:
							chest.item[index].SetDefaults(ItemID.ObsidianSkinPotion);
							break;
						case 4:
							chest.item[index].SetDefaults(ItemID.MagicPowerPotion);
							break;
						case 5:
							chest.item[index].SetDefaults(ItemID.InvisibilityPotion);
							break;
						case 6:
							chest.item[index].SetDefaults(ItemID.HunterPotion);
							break;
						case 7:
							chest.item[index].SetDefaults(ItemID.HeartreachPotion);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(1, 3);
					index++;
				}

				if (WorldGen.genRand.Next(3) > 0)
				{
					switch (WorldGen.genRand.Next(8))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.GravitationPotion);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.ThornsPotion);
							break;
						case 2:
							chest.item[index].SetDefaults(ItemID.WaterWalkingPotion);
							break;
						case 3:
							chest.item[index].SetDefaults(ItemID.ObsidianSkinPotion);
							break;
						case 4:
							chest.item[index].SetDefaults(ItemID.BattlePotion);
							break;
						case 5:
							chest.item[index].SetDefaults(ItemID.TeleportationPotion);
							break;
						case 6:
							chest.item[index].SetDefaults(ItemID.InfernoPotion);
							break;
						case 7:
							chest.item[index].SetDefaults(ItemID.LifeforcePotion);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(1, 3);
					index++;
				}

				if (WorldGen.genRand.NextBool(3))
				{
					if (WorldGen.genRand.NextBool(2))
						chest.item[index].SetDefaults(ItemID.RecallPotion);
					else
						chest.item[index].SetDefaults(ItemID.PotionOfReturn);

					chest.item[index].stack = WorldGen.genRand.Next(1, 3);
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					switch (WorldGen.genRand.Next(2))
					{
						case 0:
							chest.item[index].SetDefaults(ItemID.Torch);
							break;
						case 1:
							chest.item[index].SetDefaults(ItemID.Glowstick);
							break;
					}

					chest.item[index].stack = WorldGen.genRand.Next(15) + 15;
					index++;
				}

				if (WorldGen.genRand.NextBool(2))
				{
					chest.item[index].SetDefaults(ItemID.GoldCoin);
					chest.item[index].stack = WorldGen.genRand.Next(2, 5);
					index++;
				}
			}

			if (sky)
			{
				int drop = WorldGen.genRand.Next(6) switch
				{
					0 => ItemID.SeeTheWorldForWhatItIs,
					1 => ItemID.HighPitch,
					2 => ItemID.BlessingfromTheHeavens,
					3 => ItemID.Constellation,
					4 => ItemID.LoveisintheTrashSlot,
					_ => ItemID.SunOrnament // not available in vanilla lesion chests, but fixed there because why not
				};
				chest.item[index++].SetDefaults(drop);

				chest.item[index].SetDefaults(ItemID.Cloud);
				chest.item[index++].stack = WorldGen.genRand.Next(50, 101);

				if (WorldGen.genRand.NextBool(3))
					chest.item[index++].SetDefaults(ItemID.SkyMill);
			}

			if (chestTileType == TileID.Containers2 && style == 13 && WorldGen.genRand.NextBool(2))
				chest.item[index++].SetDefaults(ItemID.RemnantsofDevotion);

			if (index > 0 && chestTileType == TileID.Containers)
			{
				switch (style)
				{
					case 10 when WorldGen.genRand.NextBool(4):
						chest.item[index++].SetDefaults(ItemID.HoneyDispenser);
						break;
					case 11 when WorldGen.genRand.NextBool(7):
						chest.item[index++].SetDefaults(ItemID.IceMachine);
						break;
					case 13 when WorldGen.genRand.NextBool(3):
						chest.item[index++].SetDefaults(ItemID.SkyMill);
						break;
					case 16:
						chest.item[index++].SetDefaults(ItemID.LihzahrdFurnace);
						break;
				}

				if (Main.wallDungeon[Main.tile[x, y].WallType] && WorldGen.genRand.NextBool(8))
					chest.item[index++].SetDefaults(ItemID.BoneWelder);

				switch (style)
				{
					case >= 23 and <= 27 when WorldGen.genRand.NextBool(2):
						chest.item[index++].SetDefaults(ItemID.RemnantsofDevotion);
						break;
					case 16 when WorldGen.genRand.NextBool(5):
						chest.item[index++].SetDefaults(ItemID.SolarTablet);
						break;
					case 16:
						chest.item[index].SetDefaults(ItemID.LunarTabletFragment);
						chest.item[index++].stack = WorldGen.genRand.Next(3, 8);
						break;
				}
			}
		}
	}
}