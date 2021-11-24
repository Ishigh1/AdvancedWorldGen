using System;
using System.Collections.Generic;
using AdvancedWorldGen.BetterVanillaWorldGen.DesertStuff;
using AdvancedWorldGen.BetterVanillaWorldGen.DungeonStuff;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public static class Chest
	{
		public static int HellChest;
		public static List<int> HellChestItem = null!;
		public static bool GeneratedShadowKey;
		public static int SandstoneUp;
		public static int SandstoneDown;

		public static void ShuffleChests(UnifiedRandom unifiedRandom)
		{
			GeneratedShadowKey = false;
			HellChest = 0;

			HellChestItem = new List<int> {0, 1, 2, 3, 4, 5, 6, 7};
			for (int index = 0; index < HellChestItem.Count - 1; index++)
			{
				int indexToExchange = unifiedRandom.Next(HellChestItem.Count - index);
				if (indexToExchange != index)
					(HellChestItem[index], HellChestItem[indexToExchange]) =
						(HellChestItem[indexToExchange], HellChestItem[index]);
			}

			SandstoneUp = -1;
			SandstoneDown = -1;
		}

		public static bool AddBuriedChest(int chestRightX, int groundY, int contain = 0,
			bool notNearOtherChests = false, int style = -1, ushort chestTileType = TileID.Containers)
		{
			int chestLeftX = chestRightX - 1;

			int chestTopY = groundY - 2;
			int chestBottomY = groundY - 1;

			for (;
				WorldGen.SolidTile(chestLeftX, chestTopY) || WorldGen.SolidTile(chestRightX, chestTopY) ||
				!WorldGen.SolidTile(chestLeftX, groundY) || !WorldGen.SolidTile(chestRightX, groundY);
				)
			{
				if (groundY >= Main.maxTilesY - 50)
					return false;

				chestTopY = chestBottomY;
				chestBottomY = groundY;
				groundY++;
			}

			WorldGen.KillTile(chestLeftX, chestTopY);
			WorldGen.KillTile(chestRightX, chestTopY);
			WorldGen.KillTile(chestLeftX, chestBottomY);
			WorldGen.KillTile(chestRightX, chestBottomY);

			if (Main.tile[chestLeftX, chestTopY].IsActive || Main.tile[chestRightX, chestTopY].IsActive ||
			    Main.tile[chestLeftX, chestBottomY].IsActive || Main.tile[chestRightX, chestBottomY].IsActive)
				return false;

			Main.tile[chestLeftX, groundY].Slope = SlopeType.Solid;
			Main.tile[chestRightX, groundY].Slope = SlopeType.Solid;

			PreLoot(chestRightX, groundY,
				ref contain, style,
				ref chestTileType, out bool shadowChest, out style, out bool desertBiome,
				out bool iceBiome, out bool jungleBiome, out bool underworld,
				out bool water, out bool livingWood, out bool glowingMushroomBiome, out bool dungeon, out bool pyramid);
			
			int chestIndex = WorldGen.PlaceChest(chestLeftX, chestBottomY, chestTileType, notNearOtherChests, style);
			if (chestIndex < 0)
				return false;

			VanillaLoot(chestRightX, groundY,
				contain, chestTileType, shadowChest, chestIndex, style, pyramid, water, livingWood,
				dungeon, glowingMushroomBiome, desertBiome, iceBiome, jungleBiome, underworld);

			return true;
		}

		public static void PreLoot(int x, int y,
			ref int contain, int style, ref ushort chestTileType,
			out bool shadowChest, out int outStyle,
			out bool desertBiome, out bool iceBiome, out bool jungleBiome, out bool underworld,
			out bool water, out bool livingWood, out bool glowingMushroomBiome,
			out bool dungeon, out bool pyramid)
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
			const int angelChances = 15;
			outStyle = 0;
			if (y >= Main.worldSurface + 25.0 || contain > ItemID.None)
				outStyle = 1;

			if (style >= 0)
				outStyle = style;

			if (contain == ItemID.None && y >= Main.worldSurface + 25.0 && y <= Main.maxTilesY - 205 &&
			    Desert.IsUndergroundDesert(x, y))
			{
				desertBiome = true;
				outStyle = 10;
				chestTileType = TileID.Containers2;
				contain = y <= (SandstoneUp * 3 + SandstoneDown * 4) / 7
					? Utils.SelectRandom(WorldGen.genRand, ItemID.AncientChisel, ItemID.SandBoots,
						ItemID.MysticCoilSnake, ItemID.MagicConch)
					: Utils.SelectRandom(WorldGen.genRand, ItemID.ThunderSpear, ItemID.ThunderStaff,
						ItemID.DripplerFlail);

				Utils.SelectRandom(WorldGen.genRand, ItemID.AncientChisel, ItemID.SandBoots,
					ItemID.MysticCoilSnake, ItemID.MagicConch, ItemID.ThunderSpear, ItemID.ThunderStaff,
					ItemID.DripplerFlail);

				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = ItemID.AngelStatue;
			}

			if (chestTileType == TileID.Containers && (outStyle == 11 || contain == ItemID.None &&
				y >= Main.worldSurface + 25.0 &&
				y <= Main.maxTilesY - 205 &&
				Main.tile[x, y].type is TileID.SnowBlock or TileID.IceBlock or TileID.BreakableIce))
			{
				iceBiome = true;
				outStyle = 11;
				contain = WorldGen.genRand.Next(6) switch
				{
					0 => ItemID.IceBoomerang,
					1 => ItemID.IceBlade,
					2 => ItemID.IceSkates,
					3 => ItemID.SnowballCannon,
					4 => ItemID.BlizzardinaBottle,
					_ => ItemID.FlurryBoots
				};

				if (WorldGen.genRand.NextBool(20))
					contain = ItemID.Extractinator;

				if (WorldGen.genRand.NextBool(50))
					contain = ItemID.Fish;

				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = ItemID.AngelStatue;
			}

			if (chestTileType == TileID.Containers &&
			    (style == 10 ||
			     contain is ItemID.FeralClaws or ItemID.AnkletoftheWind or ItemID.StaffofRegrowth or ItemID.Seaweed))
			{
				jungleBiome = true;
				outStyle = 10;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = ItemID.AngelStatue;
			}

			if (chestTileType == TileID.Containers && y > Main.maxTilesY - 205 && contain == ItemID.None)
			{
				underworld = true;
				if (HellChest == HellChestItem[1])
				{
					contain = ItemID.Sunfury;
					outStyle = 4;
					shadowChest = true;
				}
				else if (HellChest == HellChestItem[2])
				{
					contain = ItemID.FlowerofFire;
					outStyle = 4;
					shadowChest = true;
				}
				else if (HellChest == HellChestItem[3])
				{
					contain = ItemID.Flamelash;
					outStyle = 4;
					shadowChest = true;
				}
				else if (HellChest == HellChestItem[4])
				{
					contain = ItemID.DarkLance;
					outStyle = 4;
					shadowChest = true;
				}
				else if (HellChest == HellChestItem[5])
				{
					contain = ItemID.HellwingBow;
					outStyle = 4;
					shadowChest = true;
				}
				else
				{
					contain = ItemID.TreasureMagnet;
					outStyle = 4;
					shadowChest = true;
				}

				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = ItemID.AngelStatue;
			}

			if (chestTileType == TileID.Containers && outStyle == 17)
			{
				water = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = ItemID.AngelStatue;
			}

			if (chestTileType == TileID.Containers && outStyle == 12)
			{
				livingWood = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = ItemID.AngelStatue;
			}

			if (chestTileType == TileID.Containers && outStyle == 32)
			{
				glowingMushroomBiome = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = ItemID.AngelStatue;
			}

			if (chestTileType == TileID.Containers && outStyle != 0 && DungeonPass.IsDungeon(x, y))
			{
				dungeon = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = ItemID.AngelStatue;
			}

			if (chestTileType == TileID.Containers && outStyle != 0 &&
			    contain is ItemID.PharaohsMask or ItemID.SandstorminaBottle or ItemID.FlyingCarpet)
				pyramid = true;
		}


		public static void VanillaLoot(int x, int y, int contain, ushort chestTileType, bool shadowChest,
			int chestIndex, int style, bool pyramid, bool water, bool livingWood, bool dungeon, 
			bool glowingMushroomBiome, bool desertBiome, bool iceBiome, bool jungleBiome, bool underworld)
		{
			if (desertBiome)
			{
				SandstoneUp = SandstoneUp == 1 ? x - 1 : Math.Min(SandstoneUp, x - 1);
				SandstoneDown = Math.Max(x, SandstoneDown);
			}

			if (shadowChest)
			{
				HellChest++;
				if (HellChest > 4)
					HellChest = 0;
			}

			Terraria.Chest chest = Main.chest[chestIndex];
			int index = 0;
			while (index == 0)
			{
				if (style == 0 && y < Main.worldSurface + 25.0 || pyramid)
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
						switch (WorldGen.genRand.Next(12))
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
								chest.item[index].SetDefaults(ItemID.Glowstick);
								chest.item[index].stack = WorldGen.genRand.Next(40, 75);
								break;
							case 4:
								chest.item[index].SetDefaults(ItemID.ThrowingKnife);
								chest.item[index].stack = WorldGen.genRand.Next(150, 300);
								break;
							case 5:
								chest.item[index].SetDefaults(ItemID.Aglet);
								chest.item[index].Prefix(-1);
								break;
							case 6:
								chest.item[index].SetDefaults(ItemID.ClimbingClaws);
								chest.item[index].Prefix(-1);
								break;
							case 7:
								chest.item[index].SetDefaults(ItemID.Umbrella);
								chest.item[index].Prefix(-1);
								break;
							case 8:
								chest.item[index].SetDefaults(ItemID.CordageGuide);
								chest.item[index].Prefix(-1);
								break;
							case 9:
								chest.item[index].SetDefaults(ItemID.WandofSparking);
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
								chest.item[index].SetDefaults(WorldGen.copperBar);
								break;
							case 1:
								chest.item[index].SetDefaults(WorldGen.ironBar);
								break;
						}

						chest.item[index].stack = WorldGen.genRand.Next(8) + 3;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						chest.item[index].SetDefaults(ItemID.Rope);
						chest.item[index].stack = WorldGen.genRand.Next(50, 101);
						index++;
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

					if (WorldGen.genRand.Next(3) != 0)
					{
						chest.item[index].SetDefaults(ItemID.RecallPotion);
						chest.item[index].stack = WorldGen.genRand.Next(3, 6);
						index++;
					}

					if (WorldGen.genRand.Next(3) > 0)
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

						chest.item[index].stack = WorldGen.genRand.Next(11) + 10;
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

				else if (y < Main.rockLayer)
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
						if (water && WorldGen.genRand.NextBool(2))
						{
							chest.item[index].SetDefaults(ItemID.SandcastleBucket);
							index++;
						}

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
								chest.item[index].SetDefaults(WorldGen.ironBar);
								break;
							case 1:
								chest.item[index].SetDefaults(WorldGen.silverBar);
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
				else if (y < Main.maxTilesY - 250)
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

						if (water && WorldGen.genRand.NextBool(2))
						{
							chest.item[index].SetDefaults(ItemID.SandcastleBucket);
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
						if (WorldGen.genRand.NextBool(20) && y > WorldGen.lavaLine)
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
							switch (WorldGen.genRand.Next(8))
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
									chest.item[index].SetDefaults(ItemID.LuckyHorseshoe);
									chest.item[index].Prefix(-1);
									break;
								case 7:
									chest.item[index].SetDefaults(ItemID.FlareGun);
									chest.item[index].Prefix(-1);
									index++;
									chest.item[index].SetDefaults(ItemID.Flare);
									chest.item[index].stack = WorldGen.genRand.Next(26) + 25;
									break;
							}
						}

						index++;
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
								chest.item[index].SetDefaults(WorldGen.goldBar);
								break;
							case 1:
								chest.item[index].SetDefaults(WorldGen.silverBar);
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
							if (WorldGen.genRand.NextBool(10))
							{
								chest.item[index].SetDefaults(ItemID.HellMinecart);
								index++;
							}

							if (WorldGen.genRand.NextBool(10))
							{
								chest.item[index].SetDefaults(ItemID.OrnateShadowKey);
								index++;
							}
							else if (WorldGen.genRand.NextBool(10))
							{
								chest.item[index].SetDefaults(ItemID.HellCake);
								index++;
							}
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
								chest.item[index].SetDefaults(WorldGen.goldBar);
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

				if (index <= 0 || chestTileType != TileID.Containers)
					continue;

				switch (style)
				{
					case 10 when WorldGen.genRand.NextBool(4):
						chest.item[index].SetDefaults(ItemID.HoneyDispenser);
						index++;
						break;
					case 11 when WorldGen.genRand.NextBool(7):
						chest.item[index].SetDefaults(ItemID.IceMachine);
						index++;
						break;
					case 13 when WorldGen.genRand.NextBool(3):
						chest.item[index].SetDefaults(ItemID.SkyMill);
						index++;
						break;
					case 16:
						chest.item[index].SetDefaults(ItemID.LihzahrdFurnace);
						index++;
						break;
				}

				if (Main.wallDungeon[Main.tile[x, y].wall] && WorldGen.genRand.NextBool(8))
				{
					chest.item[index].SetDefaults(ItemID.BoneWelder);
					index++;
				}

				if (style == 16)
				{
					if (WorldGen.genRand.NextBool(5))
					{
						chest.item[index].SetDefaults(ItemID.SolarTablet);
						index++;
					}
					else
					{
						chest.item[index].SetDefaults(ItemID.LunarTabletFragment);
						chest.item[index].stack = WorldGen.genRand.Next(3, 8);
						index++;
					}
				}
			}
		}
	}
}