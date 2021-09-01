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

		public static bool AddBuriedChest(int x, int y, int contain = 0, bool notNearOtherChests = false,
			int style = -1, ushort chestTileType = 0)
		{
			for (;
				WorldGen.SolidTile(x - 1, y - 2) || WorldGen.SolidTile(x, y - 2) ||
				!WorldGen.SolidTile(x - 1, y) || !WorldGen.SolidTile(x, y);
				y++)
				if (y >= Main.maxTilesY - 50)
					return false;

			WorldGen.KillTile(x - 1, y - 2);
			WorldGen.KillTile(x, y - 2);
			WorldGen.KillTile(x - 1, y - 1);
			WorldGen.KillTile(x, y - 1);

			if (Main.tile[x - 1, y - 2].IsActive || Main.tile[x, y - 2].IsActive ||
			    Main.tile[x - 1, y - 1].IsActive || Main.tile[x, y - 1].IsActive)
				return false;

			Main.tile[x - 1, y].Slope = SlopeType.Solid;
			Main.tile[x, y].Slope = SlopeType.Solid;

			PreLoot(x, ref contain, style, ref chestTileType, y, out bool flag10,
				out int num8, out bool flag2, out bool flag, out bool flag3, out bool flag7, out bool flag4,
				out bool flag5, out bool flag6, out bool flag8, out bool flag9);

			int num7 = WorldGen.PlaceChest(x - 1, y - 1, chestTileType, notNearOtherChests, num8);
			if (num7 >= 0)
			{
				VanillaLoot(x, contain, chestTileType, flag10, num7, num8, y, flag9, flag4, flag5, flag8,
					flag6, flag2, flag, flag3, flag7);

				return true;
			}

			return false;
		}

		public static void PreLoot(int x, ref int contain, int style, ref ushort chestTileType, int y,
			out bool flag10, out int num8, out bool flag2, out bool flag, out bool flag3, out bool flag7,
			out bool flag4, out bool flag5, out bool flag6, out bool flag8, out bool flag9)
		{
			if (chestTileType == 0)
				chestTileType = 21;

			flag = false;
			flag2 = false;
			flag3 = false;
			flag4 = false;
			flag5 = false;
			flag6 = false;
			flag7 = false;
			flag8 = false;
			flag9 = false;
			flag10 = false;
			int angelChances = 15;
			num8 = 0;
			if (y >= Main.worldSurface + 25.0 || contain > 0)
				num8 = 1;

			if (style >= 0)
				num8 = style;

			if (contain == 0 && y >= Main.worldSurface + 25.0 && y <= Main.maxTilesY - 205 &&
			    Desert.IsUndergroundDesert(x, y))
			{
				flag2 = true;
				num8 = 10;
				chestTileType = 467;
				contain = y <= (SandstoneUp * 3 + SandstoneDown * 4) / 7
					? Utils.SelectRandom(WorldGen.genRand, ItemID.AncientChisel, ItemID.SandBoots,
						ItemID.MysticCoilSnake, ItemID.MagicConch)
					: Utils.SelectRandom(WorldGen.genRand, ItemID.ThunderSpear, ItemID.ThunderStaff,
						ItemID.DripplerFlail);

				Utils.SelectRandom(WorldGen.genRand, ItemID.AncientChisel, ItemID.SandBoots,
					ItemID.MysticCoilSnake, ItemID.MagicConch, ItemID.ThunderSpear, ItemID.ThunderStaff,
					ItemID.DripplerFlail);

				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && (num8 == 11 || contain == 0 && y >= Main.worldSurface + 25.0 &&
				y <= Main.maxTilesY - 205 &&
				Main.tile[x, y].type is 147 or 161 or 162))
			{
				flag = true;
				num8 = 11;
				contain = WorldGen.genRand.Next(6) switch
				{
					0 => 670,
					1 => 724,
					2 => 950,
					3 => 1319,
					4 => 987,
					_ => 1579
				};

				if (WorldGen.genRand.NextBool(20))
					contain = 997;

				if (WorldGen.genRand.NextBool(50))
					contain = 669;

				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = 52;
			}

			if (chestTileType == 21 &&
			    (style == 10 || contain is 211 or 212 or 213 or 753))
			{
				flag3 = true;
				num8 = 10;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && y > Main.maxTilesY - 205 && contain == 0)
			{
				flag7 = true;
				if (HellChest == HellChestItem[1])
				{
					contain = 220;
					num8 = 4;
					flag10 = true;
				}
				else if (HellChest == HellChestItem[2])
				{
					contain = 112;
					num8 = 4;
					flag10 = true;
				}
				else if (HellChest == HellChestItem[3])
				{
					contain = 218;
					num8 = 4;
					flag10 = true;
				}
				else if (HellChest == HellChestItem[4])
				{
					contain = 274;
					num8 = 4;
					flag10 = true;
				}
				else if (HellChest == HellChestItem[5])
				{
					contain = 3019;
					num8 = 4;
					flag10 = true;
				}
				else
				{
					contain = 5010;
					num8 = 4;
					flag10 = true;
				}

				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && num8 == 17)
			{
				flag4 = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && num8 == 12)
			{
				flag5 = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && num8 == 32)
			{
				flag6 = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && num8 != 0 && Dungeon.IsDungeon(x, y))
			{
				flag8 = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(angelChances) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && num8 != 0 && contain is 848 or 857 or 934)
				flag9 = true;
		}


		public static void VanillaLoot(int x, int contain, ushort chestTileType, bool flag10, int num7, int num8,
			int y, bool flag9, bool flag4, bool flag5, bool flag8, bool flag6, bool flag2, bool flag, bool flag3,
			bool flag7)
		{
			if (flag2)
			{
				SandstoneUp = SandstoneUp == 1 ? x - 1 : Math.Min(SandstoneUp, x - 1);
				SandstoneDown = Math.Max(x, SandstoneDown);
			}

			if (flag10)
			{
				HellChest++;
				if (HellChest > 4)
					HellChest = 0;
			}

			Terraria.Chest chest = Main.chest[num7];
			int index = 0;
			while (index == 0)
			{
				if (num8 == 0 && y < Main.worldSurface + 25.0 || flag9)
				{
					if (contain > 0)
					{
						chest.item[index].SetDefaults(contain);
						chest.item[index].Prefix(-1);
						index++;
						switch (contain)
						{
							case 848:
								chest.item[index].SetDefaults(866);
								index++;
								break;
							case 832:
								chest.item[index].SetDefaults(933);
								index++;
								if (WorldGen.genRand.NextBool(10))
								{
									int num14 = WorldGen.genRand.Next(2);
									switch (num14)
									{
										case 0:
											num14 = 4429;
											break;
										case 1:
											num14 = 4427;
											break;
									}

									chest.item[index].SetDefaults(num14);
									index++;
								}

								break;
						}

						if (Main.tenthAnniversaryWorld && flag9)
						{
							chest.item[index++].SetDefaults(848);
							chest.item[index++].SetDefaults(866);
						}
					}
					else
					{
						int num15 = WorldGen.genRand.Next(12);
						switch (num15)
						{
							case 0:
								chest.item[index].SetDefaults(280);
								chest.item[index].Prefix(-1);
								break;
							case 1:
								chest.item[index].SetDefaults(281);
								chest.item[index].Prefix(-1);
								break;
							case 2:
								chest.item[index].SetDefaults(284);
								chest.item[index].Prefix(-1);
								break;
							case 3:
								chest.item[index].SetDefaults(282);
								chest.item[index].stack = WorldGen.genRand.Next(40, 75);
								break;
							case 4:
								chest.item[index].SetDefaults(279);
								chest.item[index].stack = WorldGen.genRand.Next(150, 300);
								break;
							case 5:
								chest.item[index].SetDefaults(285);
								chest.item[index].Prefix(-1);
								break;
							case 6:
								chest.item[index].SetDefaults(953);
								chest.item[index].Prefix(-1);
								break;
							case 7:
								chest.item[index].SetDefaults(946);
								chest.item[index].Prefix(-1);
								break;
							case 8:
								chest.item[index].SetDefaults(3068);
								chest.item[index].Prefix(-1);
								break;
							case 9:
								chest.item[index].SetDefaults(3069);
								chest.item[index].Prefix(-1);
								break;
							case 10:
								chest.item[index].SetDefaults(3084);
								chest.item[index].Prefix(-1);
								break;
							case 11:
								chest.item[index].SetDefaults(4341);
								chest.item[index].Prefix(-1);
								break;
						}

						index++;
					}

					if (WorldGen.genRand.NextBool(6))
					{
						chest.item[index].SetDefaults(3093);
						chest.item[index].stack = 1;
						if (WorldGen.genRand.NextBool(5))
							chest.item[index].stack += WorldGen.genRand.Next(2);
						if (WorldGen.genRand.NextBool(10))
							chest.item[index].stack += WorldGen.genRand.Next(3);
						index++;
					}

					if (WorldGen.genRand.NextBool(6))
					{
						chest.item[index].SetDefaults(4345);
						chest.item[index].stack = 1;
						if (WorldGen.genRand.NextBool(5))
							chest.item[index].stack += WorldGen.genRand.Next(2);
						if (WorldGen.genRand.NextBool(10))
							chest.item[index].stack += WorldGen.genRand.Next(3);
						index++;
					}

					if (WorldGen.genRand.NextBool(3))
					{
						chest.item[index].SetDefaults(168);
						chest.item[index].stack = WorldGen.genRand.Next(3, 6);
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int num16 = WorldGen.genRand.Next(2);

						int stack = WorldGen.genRand.Next(8) + 3;
						if (num16 == 0)
							chest.item[index].SetDefaults(WorldGen.copperBar);
						if (num16 == 1)
							chest.item[index].SetDefaults(WorldGen.ironBar);
						chest.item[index].stack = stack;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int stack2 = WorldGen.genRand.Next(50, 101);
						chest.item[index].SetDefaults(965);
						chest.item[index].stack = stack2;
						index++;
					}

					if (WorldGen.genRand.Next(3) != 0)
					{
						int num17 = WorldGen.genRand.Next(2);

						int stack3 = WorldGen.genRand.Next(26) + 25;
						chest.item[index].SetDefaults(num17 == 0 ? 40 : 42);
						chest.item[index].stack = stack3;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int stack4 = WorldGen.genRand.Next(3) + 3;
						chest.item[index].SetDefaults(28);
						chest.item[index].stack = stack4;
						index++;
					}

					if (WorldGen.genRand.Next(3) != 0)
					{
						chest.item[index].SetDefaults(2350);
						chest.item[index].stack = WorldGen.genRand.Next(3, 6);
						index++;
					}

					if (WorldGen.genRand.Next(3) > 0)
					{
						int num18 = WorldGen.genRand.Next(6);

						int stack5 = WorldGen.genRand.Next(1, 3);
						switch (num18)
						{
							case 0:
								chest.item[index].SetDefaults(292);
								break;
							case 1:
								chest.item[index].SetDefaults(298);
								break;
							case 2:
								chest.item[index].SetDefaults(299);
								break;
							case 3:
								chest.item[index].SetDefaults(290);
								break;
							case 4:
								chest.item[index].SetDefaults(2322);
								break;
							case 5:
								chest.item[index].SetDefaults(2325);
								break;
						}

						chest.item[index].stack = stack5;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int num19 = WorldGen.genRand.Next(2);

						int stack6 = WorldGen.genRand.Next(11) + 10;
						chest.item[index].SetDefaults(num19 == 0 ? 8 : 31);
						chest.item[index].stack = stack6;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						chest.item[index].SetDefaults(72);
						chest.item[index].stack = WorldGen.genRand.Next(10, 30);
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						chest.item[index].SetDefaults(9);
						chest.item[index].stack = WorldGen.genRand.Next(50, 100);
						index++;
					}
				}

				else if (y < Main.rockLayer)
				{
					if (contain > 0)
					{
						if (contain == 832)
						{
							chest.item[index].SetDefaults(933);
							index++;
						}

						chest.item[index].SetDefaults(contain);
						chest.item[index].Prefix(-1);
						index++;
						if (flag4 && WorldGen.genRand.NextBool(2))
						{
							chest.item[index].SetDefaults(4460);
							index++;
						}

						if (flag5 && WorldGen.genRand.NextBool(10))
						{
							int num20 = WorldGen.genRand.Next(2);
							switch (num20)
							{
								case 0:
									num20 = 4429;
									break;
								case 1:
									num20 = 4427;
									break;
							}

							chest.item[index].SetDefaults(num20);
							index++;
						}

						if (flag8 && (!GeneratedShadowKey || WorldGen.genRand.NextBool(3)))
						{
							GeneratedShadowKey = true;
							chest.item[index].SetDefaults(329);
							index++;
						}
					}
					else
					{
						switch (WorldGen.genRand.Next(6))
						{
							case 0:
								chest.item[index].SetDefaults(49);
								chest.item[index].Prefix(-1);
								break;
							case 1:
								chest.item[index].SetDefaults(50);
								chest.item[index].Prefix(-1);
								break;
							case 2:
								chest.item[index].SetDefaults(53);
								chest.item[index].Prefix(-1);
								break;
							case 3:
								chest.item[index].SetDefaults(54);
								chest.item[index].Prefix(-1);
								break;
							case 4:
								chest.item[index].SetDefaults(5011);
								chest.item[index].Prefix(-1);
								break;
							default:
								chest.item[index].SetDefaults(975);
								chest.item[index].Prefix(-1);
								break;
						}

						index++;
						if (WorldGen.genRand.NextBool(20))
						{
							chest.item[index].SetDefaults(997);
							chest.item[index].Prefix(-1);
							index++;
						}
						else if (WorldGen.genRand.NextBool(20))
						{
							chest.item[index].SetDefaults(930);
							chest.item[index].Prefix(-1);
							index++;
							chest.item[index].SetDefaults(931);
							chest.item[index].stack = WorldGen.genRand.Next(26) + 25;
							index++;
						}

						if (flag6 && WorldGen.genRand.NextBool(2))
						{
							chest.item[index].SetDefaults(4450);
							index++;
						}

						if (flag6 && WorldGen.genRand.NextBool(3))
						{
							chest.item[index].SetDefaults(4779);
							index++;
							chest.item[index].SetDefaults(4780);
							index++;
							chest.item[index].SetDefaults(4781);
							index++;
						}
					}

					if (flag2)
					{
						if (WorldGen.genRand.NextBool(3))
						{
							chest.item[index].SetDefaults(4423);
							chest.item[index].stack = WorldGen.genRand.Next(10, 20);
							index++;
						}
					}
					else if (WorldGen.genRand.NextBool(3))
					{
						chest.item[index].SetDefaults(166);
						chest.item[index].stack = WorldGen.genRand.Next(10, 20);
						index++;
					}

					if (WorldGen.genRand.NextBool(5))
					{
						chest.item[index].SetDefaults(52);
						index++;
					}

					if (WorldGen.genRand.NextBool(3))
					{
						int stack7 = WorldGen.genRand.Next(50, 101);
						chest.item[index].SetDefaults(965);
						chest.item[index].stack = stack7;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int num21 = WorldGen.genRand.Next(2);
						int stack8 = WorldGen.genRand.Next(10) + 5;
						if (num21 == 0)
							chest.item[index].SetDefaults(WorldGen.ironBar);

						if (num21 == 1)
							chest.item[index].SetDefaults(WorldGen.silverBar);

						chest.item[index].stack = stack8;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int num22 = WorldGen.genRand.Next(2);
						int stack9 = WorldGen.genRand.Next(25) + 25;
						if (num22 == 0)
							chest.item[index].SetDefaults(40);

						if (num22 == 1)
							chest.item[index].SetDefaults(42);

						chest.item[index].stack = stack9;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int stack10 = WorldGen.genRand.Next(3) + 3;
						chest.item[index].SetDefaults(28);
						chest.item[index].stack = stack10;
						index++;
					}

					if (WorldGen.genRand.Next(3) > 0)
					{
						int num23 = WorldGen.genRand.Next(9);
						int stack11 = WorldGen.genRand.Next(1, 3);
						if (num23 == 0)
							chest.item[index].SetDefaults(289);

						if (num23 == 1)
							chest.item[index].SetDefaults(298);

						if (num23 == 2)
							chest.item[index].SetDefaults(299);

						if (num23 == 3)
							chest.item[index].SetDefaults(290);

						if (num23 == 4)
							chest.item[index].SetDefaults(303);

						if (num23 == 5)
							chest.item[index].SetDefaults(291);

						if (num23 == 6)
							chest.item[index].SetDefaults(304);

						if (num23 == 7)
							chest.item[index].SetDefaults(2322);

						if (num23 == 8)
							chest.item[index].SetDefaults(2329);

						chest.item[index].stack = stack11;
						index++;
					}

					if (WorldGen.genRand.Next(3) != 0)
					{
						int stack12 = WorldGen.genRand.Next(2, 5);
						chest.item[index].SetDefaults(2350);
						chest.item[index].stack = stack12;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int stack13 = WorldGen.genRand.Next(11) + 10;
						if (num8 == 11)
							chest.item[index].SetDefaults(974);
						else
							chest.item[index].SetDefaults(8);

						chest.item[index].stack = stack13;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						chest.item[index].SetDefaults(72);
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
						if (flag && WorldGen.genRand.NextBool(5))
						{
							chest.item[index].SetDefaults(3199);
							index++;
						}

						if (flag2)
						{
							if (WorldGen.genRand.NextBool(7))
							{
								chest.item[index].SetDefaults(4346);
								index++;
							}

							if (WorldGen.genRand.NextBool(15))
							{
								chest.item[index].SetDefaults(4066);
								index++;
							}
						}

						if (flag3 && WorldGen.genRand.NextBool(6))
						{
							chest.item[index++].SetDefaults(3360);
							chest.item[index++].SetDefaults(3361);
						}

						if (flag3 && WorldGen.genRand.NextBool(10))
							chest.item[index++].SetDefaults(4426);

						if (flag4 && WorldGen.genRand.NextBool(2))
						{
							chest.item[index].SetDefaults(4460);
							index++;
						}

						if (flag8 && (!GeneratedShadowKey || WorldGen.genRand.NextBool(3)))
						{
							GeneratedShadowKey = true;
							chest.item[index].SetDefaults(329);
							index++;
						}
					}
					else
					{
						int num24 = WorldGen.genRand.Next(8);
						if (WorldGen.genRand.NextBool(20) && y > WorldGen.lavaLine)
						{
							chest.item[index].SetDefaults(906);
							chest.item[index].Prefix(-1);
						}
						else if (WorldGen.genRand.NextBool(15))
						{
							chest.item[index].SetDefaults(997);
							chest.item[index].Prefix(-1);
						}
						else
						{
							if (num24 == 0)
							{
								chest.item[index].SetDefaults(49);
								chest.item[index].Prefix(-1);
							}

							if (num24 == 1)
							{
								chest.item[index].SetDefaults(50);
								chest.item[index].Prefix(-1);
							}

							if (num24 == 2)
							{
								chest.item[index].SetDefaults(53);
								chest.item[index].Prefix(-1);
							}

							if (num24 == 3)
							{
								chest.item[index].SetDefaults(54);
								chest.item[index].Prefix(-1);
							}

							if (num24 == 4)
							{
								chest.item[index].SetDefaults(5011);
								chest.item[index].Prefix(-1);
							}

							if (num24 == 5)
							{
								chest.item[index].SetDefaults(975);
								chest.item[index].Prefix(-1);
							}

							if (num24 == 6)
							{
								chest.item[index].SetDefaults(158);
								chest.item[index].Prefix(-1);
							}

							if (num24 == 7)
							{
								chest.item[index].SetDefaults(930);
								chest.item[index].Prefix(-1);
								index++;
								chest.item[index].SetDefaults(931);
								chest.item[index].stack = WorldGen.genRand.Next(26) + 25;
							}
						}

						index++;
						if (flag6 && WorldGen.genRand.NextBool(2))
						{
							chest.item[index].SetDefaults(4450);
							index++;
						}

						if (flag6 && WorldGen.genRand.NextBool(3))
						{
							chest.item[index].SetDefaults(4779);
							index++;
							chest.item[index].SetDefaults(4780);
							index++;
							chest.item[index].SetDefaults(4781);
							index++;
						}
					}

					if (WorldGen.genRand.NextBool(5))
					{
						chest.item[index].SetDefaults(43);
						index++;
					}

					if (WorldGen.genRand.NextBool(3))
					{
						chest.item[index].SetDefaults(167);
						index++;
					}

					if (WorldGen.genRand.NextBool(4))
					{
						chest.item[index].SetDefaults(51);
						chest.item[index].stack = WorldGen.genRand.Next(26) + 25;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int num25 = WorldGen.genRand.Next(2);
						int stack14 = WorldGen.genRand.Next(8) + 3;
						if (num25 == 0)
							chest.item[index].SetDefaults(WorldGen.goldBar);

						if (num25 == 1)
							chest.item[index].SetDefaults(WorldGen.silverBar);

						chest.item[index].stack = stack14;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int num26 = WorldGen.genRand.Next(2);
						int stack15 = WorldGen.genRand.Next(26) + 25;
						if (num26 == 0)
							chest.item[index].SetDefaults(41);

						if (num26 == 1)
							chest.item[index].SetDefaults(279);

						chest.item[index].stack = stack15;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int stack16 = WorldGen.genRand.Next(3) + 3;
						chest.item[index].SetDefaults(188);
						chest.item[index].stack = stack16;
						index++;
					}

					if (WorldGen.genRand.Next(3) > 0)
					{
						int num27 = WorldGen.genRand.Next(6);
						int stack17 = WorldGen.genRand.Next(1, 3);
						if (num27 == 0)
							chest.item[index].SetDefaults(296);

						if (num27 == 1)
							chest.item[index].SetDefaults(295);

						if (num27 == 2)
							chest.item[index].SetDefaults(299);

						if (num27 == 3)
							chest.item[index].SetDefaults(302);

						if (num27 == 4)
							chest.item[index].SetDefaults(303);

						if (num27 == 5)
							chest.item[index].SetDefaults(305);

						chest.item[index].stack = stack17;
						index++;
					}

					if (WorldGen.genRand.Next(3) > 1)
					{
						int num28 = WorldGen.genRand.Next(6);
						int stack18 = WorldGen.genRand.Next(1, 3);
						if (num28 == 0)
							chest.item[index].SetDefaults(301);

						if (num28 == 1)
							chest.item[index].SetDefaults(297);

						if (num28 == 2)
							chest.item[index].SetDefaults(304);

						if (num28 == 3)
							chest.item[index].SetDefaults(2329);

						if (num28 == 4)
							chest.item[index].SetDefaults(2351);

						if (num28 == 5)
							chest.item[index].SetDefaults(2326);

						chest.item[index].stack = stack18;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int stack19 = WorldGen.genRand.Next(2, 5);
						chest.item[index].SetDefaults(2350);
						chest.item[index].stack = stack19;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int num29 = WorldGen.genRand.Next(2);
						int stack20 = WorldGen.genRand.Next(15) + 15;
						if (num29 == 0)
						{
							if (num8 == 11)
								chest.item[index].SetDefaults(974);
							else
								chest.item[index].SetDefaults(8);
						}

						if (num29 == 1)
							chest.item[index].SetDefaults(282);

						chest.item[index].stack = stack20;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						chest.item[index].SetDefaults(73);
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
						if (flag7 && WorldGen.genRand.NextBool(10))
						{
							chest.item[index].SetDefaults(4443);
							index++;
						}

						if (flag7 && WorldGen.genRand.NextBool(10))
						{
							chest.item[index].SetDefaults(4737);
							index++;
						}
						else if (flag7 && WorldGen.genRand.NextBool(10))
						{
							chest.item[index].SetDefaults(4551);
							index++;
						}
					}
					else
					{
						int num30 = WorldGen.genRand.Next(4);
						if (num30 == 0)
						{
							chest.item[index].SetDefaults(49);
							chest.item[index].Prefix(-1);
						}

						if (num30 == 1)
						{
							chest.item[index].SetDefaults(50);
							chest.item[index].Prefix(-1);
						}

						if (num30 == 2)
						{
							chest.item[index].SetDefaults(53);
							chest.item[index].Prefix(-1);
						}

						if (num30 == 3)
						{
							chest.item[index].SetDefaults(54);
							chest.item[index].Prefix(-1);
						}

						index++;
					}

					if (WorldGen.genRand.NextBool(3))
					{
						chest.item[index].SetDefaults(167);
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int num31 = WorldGen.genRand.Next(2);
						int stack21 = WorldGen.genRand.Next(15) + 15;
						if (num31 == 0)
							chest.item[index].SetDefaults(117);

						if (num31 == 1)
							chest.item[index].SetDefaults(WorldGen.goldBar);

						chest.item[index].stack = stack21;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int num32 = WorldGen.genRand.Next(2);
						int stack22 = WorldGen.genRand.Next(25) + 50;
						if (num32 == 0)
							chest.item[index].SetDefaults(265);

						if (num32 == 1)
						{
							if (WorldGen.SavedOreTiers.Silver == 168)
								chest.item[index].SetDefaults(4915);
							else
								chest.item[index].SetDefaults(278);
						}

						chest.item[index].stack = stack22;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int stack23 = WorldGen.genRand.Next(6) + 15;
						chest.item[index].SetDefaults(227);
						chest.item[index].stack = stack23;
						index++;
					}

					if (WorldGen.genRand.Next(4) > 0)
					{
						int num33 = WorldGen.genRand.Next(8);
						int stack24 = WorldGen.genRand.Next(1, 3);
						if (num33 == 0)
							chest.item[index].SetDefaults(296);

						if (num33 == 1)
							chest.item[index].SetDefaults(295);

						if (num33 == 2)
							chest.item[index].SetDefaults(293);

						if (num33 == 3)
							chest.item[index].SetDefaults(288);

						if (num33 == 4)
							chest.item[index].SetDefaults(294);

						if (num33 == 5)
							chest.item[index].SetDefaults(297);

						if (num33 == 6)
							chest.item[index].SetDefaults(304);

						if (num33 == 7)
							chest.item[index].SetDefaults(2323);

						chest.item[index].stack = stack24;
						index++;
					}

					if (WorldGen.genRand.Next(3) > 0)
					{
						int num34 = WorldGen.genRand.Next(8);
						int stack25 = WorldGen.genRand.Next(1, 3);
						if (num34 == 0)
							chest.item[index].SetDefaults(305);

						if (num34 == 1)
							chest.item[index].SetDefaults(301);

						if (num34 == 2)
							chest.item[index].SetDefaults(302);

						if (num34 == 3)
							chest.item[index].SetDefaults(288);

						if (num34 == 4)
							chest.item[index].SetDefaults(300);

						if (num34 == 5)
							chest.item[index].SetDefaults(2351);

						if (num34 == 6)
							chest.item[index].SetDefaults(2348);

						if (num34 == 7)
							chest.item[index].SetDefaults(2345);

						chest.item[index].stack = stack25;
						index++;
					}

					if (WorldGen.genRand.NextBool(3))
					{
						int stack26 = WorldGen.genRand.Next(1, 3);
						if (WorldGen.genRand.NextBool(2))
							chest.item[index].SetDefaults(2350);
						else
							chest.item[index].SetDefaults(4870);

						chest.item[index].stack = stack26;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						int num35 = WorldGen.genRand.Next(2);
						int stack27 = WorldGen.genRand.Next(15) + 15;
						if (num35 == 0)
							chest.item[index].SetDefaults(8);

						if (num35 == 1)
							chest.item[index].SetDefaults(282);

						chest.item[index].stack = stack27;
						index++;
					}

					if (WorldGen.genRand.NextBool(2))
					{
						chest.item[index].SetDefaults(73);
						chest.item[index].stack = WorldGen.genRand.Next(2, 5);
						index++;
					}
				}

				if (index <= 0 || chestTileType != 21)
					continue;
				if (num8 == 10 && WorldGen.genRand.NextBool(4))
				{
					chest.item[index].SetDefaults(2204);
					index++;
				}

				if (num8 == 11 && WorldGen.genRand.NextBool(7))
				{
					chest.item[index].SetDefaults(2198);
					index++;
				}

				if (num8 == 13 && WorldGen.genRand.NextBool(3))
				{
					chest.item[index].SetDefaults(2197);
					index++;
				}

				if (num8 == 16)
				{
					chest.item[index].SetDefaults(2195);
					index++;
				}

				if (Main.wallDungeon[Main.tile[x, y].wall] && WorldGen.genRand.NextBool(8))
				{
					chest.item[index].SetDefaults(2192);
					index++;
				}

				if (num8 == 16)
				{
					if (WorldGen.genRand.NextBool(5))
					{
						chest.item[index].SetDefaults(2767);
						index++;
					}
					else
					{
						chest.item[index].SetDefaults(2766);
						chest.item[index].stack = WorldGen.genRand.Next(3, 8);
						index++;
					}
				}
			}
		}
	}
}