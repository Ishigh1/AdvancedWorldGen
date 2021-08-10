using Terraria;
using Terraria.ID;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class Chest
	{
		public static int HellChest;
		public static int[] HellChestItem = new int[7];
		public static bool GeneratedShadowKey;

		public static void ShuffleChests()
		{
			GeneratedShadowKey = false;
			HellChest = 0;
			for (int num915 = 0; num915 < HellChestItem.Length; num915++)
			{
				bool flag63 = true;
				while (flag63)
				{
					flag63 = false;
					HellChestItem[num915] = WorldGen.genRand.Next(HellChestItem.Length);
					for (int num916 = 0; num916 < num915; num916++)
						if (HellChestItem[num916] == HellChestItem[num915])
							flag63 = true;
				}
			}
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

			PreLoot(x, ref contain, style, ref chestTileType, y, 15, out bool flag10, out int num6,
				out int num8, out bool flag2, out bool flag, out bool flag3, out bool flag7, out bool flag4,
				out bool flag5, out bool flag6, out bool flag8, out bool flag9);

			int num7 = WorldGen.PlaceChest(x - 1, num6 - 1, chestTileType, notNearOtherChests, num8);
			if (num7 >= 0)
			{
				VanillaLoot(x, contain, chestTileType, flag10, num7, num8, num6, flag9, flag4, flag5, flag8,
					flag6, flag2, flag, flag3, flag7, y);

				return true;
			}

			return false;
		}

		public static void PreLoot(int i, ref int contain, int style, ref ushort chestTileType, int k, int maxValue,
			out bool flag10, out int num6, out int num8, out bool flag2, out bool flag, out bool flag3, out bool flag7,
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
			num6 = k;
			num8 = 0;
			if (num6 >= Main.worldSurface + 25.0 || contain > 0)
				num8 = 1;

			if (style >= 0)
				num8 = style;

			if (contain == 0 && num6 >= Main.worldSurface + 25.0 && num6 <= Main.maxTilesY - 205 &&
			    Desert.IsUndergroundDesert(i, k))
			{
				flag2 = true;
				num8 = 10;
				chestTileType = 467;

				Utils.SelectRandom(WorldGen.genRand, ItemID.AncientChisel, ItemID.SandBoots,
					ItemID.MysticCoilSnake, ItemID.MagicConch, ItemID.ThunderSpear, ItemID.ThunderStaff,
					ItemID.DripplerFlail);

				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(maxValue) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && (num8 == 11 || contain == 0 && num6 >= Main.worldSurface + 25.0 &&
				num6 <= Main.maxTilesY - 205 &&
				(Main.tile[i, k].type == 147 ||
				 Main.tile[i, k].type == 161 ||
				 Main.tile[i, k].type == 162)))
			{
				flag = true;
				num8 = 11;
				switch (WorldGen.genRand.Next(6))
				{
					case 0:
						contain = 670;
						break;
					case 1:
						contain = 724;
						break;
					case 2:
						contain = 950;
						break;
					case 3:
						contain = 1319;
						break;
					case 4:
						contain = 987;
						break;
					default:
						contain = 1579;
						break;
				}

				if (WorldGen.genRand.Next(20) == 0)
					contain = 997;

				if (WorldGen.genRand.Next(50) == 0)
					contain = 669;

				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(maxValue) == 0)
					contain = 52;
			}

			if (chestTileType == 21 &&
			    (style == 10 || contain == 211 || contain == 212 || contain == 213 || contain == 753))
			{
				flag3 = true;
				num8 = 10;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(maxValue) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && num6 > Main.maxTilesY - 205 && contain == 0)
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

				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(maxValue) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && num8 == 17)
			{
				flag4 = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(maxValue) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && num8 == 12)
			{
				flag5 = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(maxValue) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && num8 == 32)
			{
				flag6 = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(maxValue) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && num8 != 0 && Dungeon.IsDungeon(i, k))
			{
				flag8 = true;
				if (WorldGen.getGoodWorldGen && WorldGen.genRand.Next(maxValue) == 0)
					contain = 52;
			}

			if (chestTileType == 21 && num8 != 0 && (contain == 848 || contain == 857 || contain == 934))
				flag9 = true;
		}


		public static void VanillaLoot(int i, int contain, ushort chestTileType, bool flag10, int num7, int num8,
			int num6,
			bool flag9, bool flag4, bool flag5, bool flag8, bool flag6, bool flag2, bool flag, bool flag3,
			bool flag7, int k)
		{
			if (flag10)
			{
				HellChest++;
				if (HellChest > 4)
					HellChest = 0;
			}

			Terraria.Chest chest = Main.chest[num7];
			int num13 = 0;
			while (num13 == 0)
			{
				if (num8 == 0 && num6 < Main.worldSurface + 25.0 || flag9)
				{
					if (contain > 0)
					{
						chest.item[num13].SetDefaults(contain);
						chest.item[num13].Prefix(-1);
						num13++;
						switch (contain)
						{
							case 848:
								chest.item[num13].SetDefaults(866);
								num13++;
								break;
							case 832:
								chest.item[num13].SetDefaults(933);
								num13++;
								if (WorldGen.genRand.Next(10) == 0)
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

									chest.item[num13].SetDefaults(num14);
									num13++;
								}

								break;
						}

						if (Main.tenthAnniversaryWorld && flag9)
						{
							chest.item[num13++].SetDefaults(848);
							chest.item[num13++].SetDefaults(866);
						}
					}
					else
					{
						int num15 = WorldGen.genRand.Next(12);
						switch (num15)
						{
							case 0:
								chest.item[num13].SetDefaults(280);
								chest.item[num13].Prefix(-1);
								break;
							case 1:
								chest.item[num13].SetDefaults(281);
								chest.item[num13].Prefix(-1);
								break;
							case 2:
								chest.item[num13].SetDefaults(284);
								chest.item[num13].Prefix(-1);
								break;
							case 3:
								chest.item[num13].SetDefaults(282);
								chest.item[num13].stack = WorldGen.genRand.Next(40, 75);
								break;
							case 4:
								chest.item[num13].SetDefaults(279);
								chest.item[num13].stack = WorldGen.genRand.Next(150, 300);
								break;
							case 5:
								chest.item[num13].SetDefaults(285);
								chest.item[num13].Prefix(-1);
								break;
							case 6:
								chest.item[num13].SetDefaults(953);
								chest.item[num13].Prefix(-1);
								break;
							case 7:
								chest.item[num13].SetDefaults(946);
								chest.item[num13].Prefix(-1);
								break;
							case 8:
								chest.item[num13].SetDefaults(3068);
								chest.item[num13].Prefix(-1);
								break;
							case 9:
								chest.item[num13].SetDefaults(3069);
								chest.item[num13].Prefix(-1);
								break;
							case 10:
								chest.item[num13].SetDefaults(3084);
								chest.item[num13].Prefix(-1);
								break;
							case 11:
								chest.item[num13].SetDefaults(4341);
								chest.item[num13].Prefix(-1);
								break;
						}

						num13++;
					}

					if (WorldGen.genRand.Next(6) == 0)
					{
						chest.item[num13].SetDefaults(3093);
						chest.item[num13].stack = 1;
						if (WorldGen.genRand.Next(5) == 0)
							chest.item[num13].stack += WorldGen.genRand.Next(2);
						if (WorldGen.genRand.Next(10) == 0)
							chest.item[num13].stack += WorldGen.genRand.Next(3);
						num13++;
					}

					if (WorldGen.genRand.Next(6) == 0)
					{
						chest.item[num13].SetDefaults(4345);
						chest.item[num13].stack = 1;
						if (WorldGen.genRand.Next(5) == 0)
							chest.item[num13].stack += WorldGen.genRand.Next(2);
						if (WorldGen.genRand.Next(10) == 0)
							chest.item[num13].stack += WorldGen.genRand.Next(3);
						num13++;
					}

					if (WorldGen.genRand.Next(3) == 0)
					{
						chest.item[num13].SetDefaults(168);
						chest.item[num13].stack = WorldGen.genRand.Next(3, 6);
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int num16 = WorldGen.genRand.Next(2);

						int stack = WorldGen.genRand.Next(8) + 3;
						if (num16 == 0)
							chest.item[num13].SetDefaults(WorldGen.copperBar);
						if (num16 == 1)
							chest.item[num13].SetDefaults(WorldGen.ironBar);
						chest.item[num13].stack = stack;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int stack2 = WorldGen.genRand.Next(50, 101);
						chest.item[num13].SetDefaults(965);
						chest.item[num13].stack = stack2;
						num13++;
					}

					if (WorldGen.genRand.Next(3) != 0)
					{
						int num17 = WorldGen.genRand.Next(2);

						int stack3 = WorldGen.genRand.Next(26) + 25;
						chest.item[num13].SetDefaults(num17 == 0 ? 40 : 42);
						chest.item[num13].stack = stack3;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int stack4 = WorldGen.genRand.Next(3) + 3;
						chest.item[num13].SetDefaults(28);
						chest.item[num13].stack = stack4;
						num13++;
					}

					if (WorldGen.genRand.Next(3) != 0)
					{
						chest.item[num13].SetDefaults(2350);
						chest.item[num13].stack = WorldGen.genRand.Next(3, 6);
						num13++;
					}

					if (WorldGen.genRand.Next(3) > 0)
					{
						int num18 = WorldGen.genRand.Next(6);

						int stack5 = WorldGen.genRand.Next(1, 3);
						switch (num18)
						{
							case 0:
								chest.item[num13].SetDefaults(292);
								break;
							case 1:
								chest.item[num13].SetDefaults(298);
								break;
							case 2:
								chest.item[num13].SetDefaults(299);
								break;
							case 3:
								chest.item[num13].SetDefaults(290);
								break;
							case 4:
								chest.item[num13].SetDefaults(2322);
								break;
							case 5:
								chest.item[num13].SetDefaults(2325);
								break;
						}

						chest.item[num13].stack = stack5;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int num19 = WorldGen.genRand.Next(2);

						int stack6 = WorldGen.genRand.Next(11) + 10;
						chest.item[num13].SetDefaults(num19 == 0 ? 8 : 31);
						chest.item[num13].stack = stack6;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						chest.item[num13].SetDefaults(72);
						chest.item[num13].stack = WorldGen.genRand.Next(10, 30);
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						chest.item[num13].SetDefaults(9);
						chest.item[num13].stack = WorldGen.genRand.Next(50, 100);
						num13++;
					}
				}

				else if (num6 < Main.rockLayer)
				{
					if (contain > 0)
					{
						if (contain == 832)
						{
							chest.item[num13].SetDefaults(933);
							num13++;
						}

						chest.item[num13].SetDefaults(contain);
						chest.item[num13].Prefix(-1);
						num13++;
						if (flag4 && WorldGen.genRand.Next(2) == 0)
						{
							chest.item[num13].SetDefaults(4460);
							num13++;
						}

						if (flag5 && WorldGen.genRand.Next(10) == 0)
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

							chest.item[num13].SetDefaults(num20);
							num13++;
						}

						if (flag8 && (!GeneratedShadowKey || WorldGen.genRand.Next(3) == 0))
						{
							GeneratedShadowKey = true;
							chest.item[num13].SetDefaults(329);
							num13++;
						}
					}
					else
					{
						switch (WorldGen.genRand.Next(6))
						{
							case 0:
								chest.item[num13].SetDefaults(49);
								chest.item[num13].Prefix(-1);
								break;
							case 1:
								chest.item[num13].SetDefaults(50);
								chest.item[num13].Prefix(-1);
								break;
							case 2:
								chest.item[num13].SetDefaults(53);
								chest.item[num13].Prefix(-1);
								break;
							case 3:
								chest.item[num13].SetDefaults(54);
								chest.item[num13].Prefix(-1);
								break;
							case 4:
								chest.item[num13].SetDefaults(5011);
								chest.item[num13].Prefix(-1);
								break;
							default:
								chest.item[num13].SetDefaults(975);
								chest.item[num13].Prefix(-1);
								break;
						}

						num13++;
						if (WorldGen.genRand.Next(20) == 0)
						{
							chest.item[num13].SetDefaults(997);
							chest.item[num13].Prefix(-1);
							num13++;
						}
						else if (WorldGen.genRand.Next(20) == 0)
						{
							chest.item[num13].SetDefaults(930);
							chest.item[num13].Prefix(-1);
							num13++;
							chest.item[num13].SetDefaults(931);
							chest.item[num13].stack = WorldGen.genRand.Next(26) + 25;
							num13++;
						}

						if (flag6 && WorldGen.genRand.Next(2) == 0)
						{
							chest.item[num13].SetDefaults(4450);
							num13++;
						}

						if (flag6 && WorldGen.genRand.Next(3) == 0)
						{
							chest.item[num13].SetDefaults(4779);
							num13++;
							chest.item[num13].SetDefaults(4780);
							num13++;
							chest.item[num13].SetDefaults(4781);
							num13++;
						}
					}

					if (flag2)
					{
						if (WorldGen.genRand.Next(3) == 0)
						{
							chest.item[num13].SetDefaults(4423);
							chest.item[num13].stack = WorldGen.genRand.Next(10, 20);
							num13++;
						}
					}
					else if (WorldGen.genRand.Next(3) == 0)
					{
						chest.item[num13].SetDefaults(166);
						chest.item[num13].stack = WorldGen.genRand.Next(10, 20);
						num13++;
					}

					if (WorldGen.genRand.Next(5) == 0)
					{
						chest.item[num13].SetDefaults(52);
						num13++;
					}

					if (WorldGen.genRand.Next(3) == 0)
					{
						int stack7 = WorldGen.genRand.Next(50, 101);
						chest.item[num13].SetDefaults(965);
						chest.item[num13].stack = stack7;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int num21 = WorldGen.genRand.Next(2);
						int stack8 = WorldGen.genRand.Next(10) + 5;
						if (num21 == 0)
							chest.item[num13].SetDefaults(WorldGen.ironBar);

						if (num21 == 1)
							chest.item[num13].SetDefaults(WorldGen.silverBar);

						chest.item[num13].stack = stack8;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int num22 = WorldGen.genRand.Next(2);
						int stack9 = WorldGen.genRand.Next(25) + 25;
						if (num22 == 0)
							chest.item[num13].SetDefaults(40);

						if (num22 == 1)
							chest.item[num13].SetDefaults(42);

						chest.item[num13].stack = stack9;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int stack10 = WorldGen.genRand.Next(3) + 3;
						chest.item[num13].SetDefaults(28);
						chest.item[num13].stack = stack10;
						num13++;
					}

					if (WorldGen.genRand.Next(3) > 0)
					{
						int num23 = WorldGen.genRand.Next(9);
						int stack11 = WorldGen.genRand.Next(1, 3);
						if (num23 == 0)
							chest.item[num13].SetDefaults(289);

						if (num23 == 1)
							chest.item[num13].SetDefaults(298);

						if (num23 == 2)
							chest.item[num13].SetDefaults(299);

						if (num23 == 3)
							chest.item[num13].SetDefaults(290);

						if (num23 == 4)
							chest.item[num13].SetDefaults(303);

						if (num23 == 5)
							chest.item[num13].SetDefaults(291);

						if (num23 == 6)
							chest.item[num13].SetDefaults(304);

						if (num23 == 7)
							chest.item[num13].SetDefaults(2322);

						if (num23 == 8)
							chest.item[num13].SetDefaults(2329);

						chest.item[num13].stack = stack11;
						num13++;
					}

					if (WorldGen.genRand.Next(3) != 0)
					{
						int stack12 = WorldGen.genRand.Next(2, 5);
						chest.item[num13].SetDefaults(2350);
						chest.item[num13].stack = stack12;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int stack13 = WorldGen.genRand.Next(11) + 10;
						if (num8 == 11)
							chest.item[num13].SetDefaults(974);
						else
							chest.item[num13].SetDefaults(8);

						chest.item[num13].stack = stack13;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						chest.item[num13].SetDefaults(72);
						chest.item[num13].stack = WorldGen.genRand.Next(50, 90);
						num13++;
					}
				}
				else if (num6 < Main.maxTilesY - 250)
				{
					if (contain > 0)
					{
						chest.item[num13].SetDefaults(contain);
						chest.item[num13].Prefix(-1);
						num13++;
						if (flag && WorldGen.genRand.Next(5) == 0)
						{
							chest.item[num13].SetDefaults(3199);
							num13++;
						}

						if (flag2)
						{
							if (WorldGen.genRand.Next(7) == 0)
							{
								chest.item[num13].SetDefaults(4346);
								num13++;
							}

							if (WorldGen.genRand.Next(15) == 0)
							{
								chest.item[num13].SetDefaults(4066);
								num13++;
							}
						}

						if (flag3 && WorldGen.genRand.Next(6) == 0)
						{
							chest.item[num13++].SetDefaults(3360);
							chest.item[num13++].SetDefaults(3361);
						}

						if (flag3 && WorldGen.genRand.Next(10) == 0)
							chest.item[num13++].SetDefaults(4426);

						if (flag4 && WorldGen.genRand.Next(2) == 0)
						{
							chest.item[num13].SetDefaults(4460);
							num13++;
						}

						if (flag8 && (!GeneratedShadowKey || WorldGen.genRand.Next(3) == 0))
						{
							GeneratedShadowKey = true;
							chest.item[num13].SetDefaults(329);
							num13++;
						}
					}
					else
					{
						int num24 = WorldGen.genRand.Next(8);
						if (WorldGen.genRand.Next(20) == 0 && num6 > WorldGen.lavaLine)
						{
							chest.item[num13].SetDefaults(906);
							chest.item[num13].Prefix(-1);
						}
						else if (WorldGen.genRand.Next(15) == 0)
						{
							chest.item[num13].SetDefaults(997);
							chest.item[num13].Prefix(-1);
						}
						else
						{
							if (num24 == 0)
							{
								chest.item[num13].SetDefaults(49);
								chest.item[num13].Prefix(-1);
							}

							if (num24 == 1)
							{
								chest.item[num13].SetDefaults(50);
								chest.item[num13].Prefix(-1);
							}

							if (num24 == 2)
							{
								chest.item[num13].SetDefaults(53);
								chest.item[num13].Prefix(-1);
							}

							if (num24 == 3)
							{
								chest.item[num13].SetDefaults(54);
								chest.item[num13].Prefix(-1);
							}

							if (num24 == 4)
							{
								chest.item[num13].SetDefaults(5011);
								chest.item[num13].Prefix(-1);
							}

							if (num24 == 5)
							{
								chest.item[num13].SetDefaults(975);
								chest.item[num13].Prefix(-1);
							}

							if (num24 == 6)
							{
								chest.item[num13].SetDefaults(158);
								chest.item[num13].Prefix(-1);
							}

							if (num24 == 7)
							{
								chest.item[num13].SetDefaults(930);
								chest.item[num13].Prefix(-1);
								num13++;
								chest.item[num13].SetDefaults(931);
								chest.item[num13].stack = WorldGen.genRand.Next(26) + 25;
							}
						}

						num13++;
						if (flag6 && WorldGen.genRand.Next(2) == 0)
						{
							chest.item[num13].SetDefaults(4450);
							num13++;
						}

						if (flag6 && WorldGen.genRand.Next(3) == 0)
						{
							chest.item[num13].SetDefaults(4779);
							num13++;
							chest.item[num13].SetDefaults(4780);
							num13++;
							chest.item[num13].SetDefaults(4781);
							num13++;
						}
					}

					if (WorldGen.genRand.Next(5) == 0)
					{
						chest.item[num13].SetDefaults(43);
						num13++;
					}

					if (WorldGen.genRand.Next(3) == 0)
					{
						chest.item[num13].SetDefaults(167);
						num13++;
					}

					if (WorldGen.genRand.Next(4) == 0)
					{
						chest.item[num13].SetDefaults(51);
						chest.item[num13].stack = WorldGen.genRand.Next(26) + 25;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int num25 = WorldGen.genRand.Next(2);
						int stack14 = WorldGen.genRand.Next(8) + 3;
						if (num25 == 0)
							chest.item[num13].SetDefaults(WorldGen.goldBar);

						if (num25 == 1)
							chest.item[num13].SetDefaults(WorldGen.silverBar);

						chest.item[num13].stack = stack14;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int num26 = WorldGen.genRand.Next(2);
						int stack15 = WorldGen.genRand.Next(26) + 25;
						if (num26 == 0)
							chest.item[num13].SetDefaults(41);

						if (num26 == 1)
							chest.item[num13].SetDefaults(279);

						chest.item[num13].stack = stack15;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int stack16 = WorldGen.genRand.Next(3) + 3;
						chest.item[num13].SetDefaults(188);
						chest.item[num13].stack = stack16;
						num13++;
					}

					if (WorldGen.genRand.Next(3) > 0)
					{
						int num27 = WorldGen.genRand.Next(6);
						int stack17 = WorldGen.genRand.Next(1, 3);
						if (num27 == 0)
							chest.item[num13].SetDefaults(296);

						if (num27 == 1)
							chest.item[num13].SetDefaults(295);

						if (num27 == 2)
							chest.item[num13].SetDefaults(299);

						if (num27 == 3)
							chest.item[num13].SetDefaults(302);

						if (num27 == 4)
							chest.item[num13].SetDefaults(303);

						if (num27 == 5)
							chest.item[num13].SetDefaults(305);

						chest.item[num13].stack = stack17;
						num13++;
					}

					if (WorldGen.genRand.Next(3) > 1)
					{
						int num28 = WorldGen.genRand.Next(6);
						int stack18 = WorldGen.genRand.Next(1, 3);
						if (num28 == 0)
							chest.item[num13].SetDefaults(301);

						if (num28 == 1)
							chest.item[num13].SetDefaults(297);

						if (num28 == 2)
							chest.item[num13].SetDefaults(304);

						if (num28 == 3)
							chest.item[num13].SetDefaults(2329);

						if (num28 == 4)
							chest.item[num13].SetDefaults(2351);

						if (num28 == 5)
							chest.item[num13].SetDefaults(2326);

						chest.item[num13].stack = stack18;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int stack19 = WorldGen.genRand.Next(2, 5);
						chest.item[num13].SetDefaults(2350);
						chest.item[num13].stack = stack19;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int num29 = WorldGen.genRand.Next(2);
						int stack20 = WorldGen.genRand.Next(15) + 15;
						if (num29 == 0)
						{
							if (num8 == 11)
								chest.item[num13].SetDefaults(974);
							else
								chest.item[num13].SetDefaults(8);
						}

						if (num29 == 1)
							chest.item[num13].SetDefaults(282);

						chest.item[num13].stack = stack20;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						chest.item[num13].SetDefaults(73);
						chest.item[num13].stack = WorldGen.genRand.Next(1, 3);
						num13++;
					}
				}
				else
				{
					if (contain > 0)
					{
						chest.item[num13].SetDefaults(contain);
						chest.item[num13].Prefix(-1);
						num13++;
						if (flag7 && WorldGen.genRand.Next(10) == 0)
						{
							chest.item[num13].SetDefaults(4443);
							num13++;
						}

						if (flag7 && WorldGen.genRand.Next(10) == 0)
						{
							chest.item[num13].SetDefaults(4737);
							num13++;
						}
						else if (flag7 && WorldGen.genRand.Next(10) == 0)
						{
							chest.item[num13].SetDefaults(4551);
							num13++;
						}
					}
					else
					{
						int num30 = WorldGen.genRand.Next(4);
						if (num30 == 0)
						{
							chest.item[num13].SetDefaults(49);
							chest.item[num13].Prefix(-1);
						}

						if (num30 == 1)
						{
							chest.item[num13].SetDefaults(50);
							chest.item[num13].Prefix(-1);
						}

						if (num30 == 2)
						{
							chest.item[num13].SetDefaults(53);
							chest.item[num13].Prefix(-1);
						}

						if (num30 == 3)
						{
							chest.item[num13].SetDefaults(54);
							chest.item[num13].Prefix(-1);
						}

						num13++;
					}

					if (WorldGen.genRand.Next(3) == 0)
					{
						chest.item[num13].SetDefaults(167);
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int num31 = WorldGen.genRand.Next(2);
						int stack21 = WorldGen.genRand.Next(15) + 15;
						if (num31 == 0)
							chest.item[num13].SetDefaults(117);

						if (num31 == 1)
							chest.item[num13].SetDefaults(WorldGen.goldBar);

						chest.item[num13].stack = stack21;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int num32 = WorldGen.genRand.Next(2);
						int stack22 = WorldGen.genRand.Next(25) + 50;
						if (num32 == 0)
							chest.item[num13].SetDefaults(265);

						if (num32 == 1)
						{
							if (WorldGen.SavedOreTiers.Silver == 168)
								chest.item[num13].SetDefaults(4915);
							else
								chest.item[num13].SetDefaults(278);
						}

						chest.item[num13].stack = stack22;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int stack23 = WorldGen.genRand.Next(6) + 15;
						chest.item[num13].SetDefaults(227);
						chest.item[num13].stack = stack23;
						num13++;
					}

					if (WorldGen.genRand.Next(4) > 0)
					{
						int num33 = WorldGen.genRand.Next(8);
						int stack24 = WorldGen.genRand.Next(1, 3);
						if (num33 == 0)
							chest.item[num13].SetDefaults(296);

						if (num33 == 1)
							chest.item[num13].SetDefaults(295);

						if (num33 == 2)
							chest.item[num13].SetDefaults(293);

						if (num33 == 3)
							chest.item[num13].SetDefaults(288);

						if (num33 == 4)
							chest.item[num13].SetDefaults(294);

						if (num33 == 5)
							chest.item[num13].SetDefaults(297);

						if (num33 == 6)
							chest.item[num13].SetDefaults(304);

						if (num33 == 7)
							chest.item[num13].SetDefaults(2323);

						chest.item[num13].stack = stack24;
						num13++;
					}

					if (WorldGen.genRand.Next(3) > 0)
					{
						int num34 = WorldGen.genRand.Next(8);
						int stack25 = WorldGen.genRand.Next(1, 3);
						if (num34 == 0)
							chest.item[num13].SetDefaults(305);

						if (num34 == 1)
							chest.item[num13].SetDefaults(301);

						if (num34 == 2)
							chest.item[num13].SetDefaults(302);

						if (num34 == 3)
							chest.item[num13].SetDefaults(288);

						if (num34 == 4)
							chest.item[num13].SetDefaults(300);

						if (num34 == 5)
							chest.item[num13].SetDefaults(2351);

						if (num34 == 6)
							chest.item[num13].SetDefaults(2348);

						if (num34 == 7)
							chest.item[num13].SetDefaults(2345);

						chest.item[num13].stack = stack25;
						num13++;
					}

					if (WorldGen.genRand.Next(3) == 0)
					{
						int stack26 = WorldGen.genRand.Next(1, 3);
						if (WorldGen.genRand.Next(2) == 0)
							chest.item[num13].SetDefaults(2350);
						else
							chest.item[num13].SetDefaults(4870);

						chest.item[num13].stack = stack26;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						int num35 = WorldGen.genRand.Next(2);
						int stack27 = WorldGen.genRand.Next(15) + 15;
						if (num35 == 0)
							chest.item[num13].SetDefaults(8);

						if (num35 == 1)
							chest.item[num13].SetDefaults(282);

						chest.item[num13].stack = stack27;
						num13++;
					}

					if (WorldGen.genRand.Next(2) == 0)
					{
						chest.item[num13].SetDefaults(73);
						chest.item[num13].stack = WorldGen.genRand.Next(2, 5);
						num13++;
					}
				}

				if (num13 <= 0 || chestTileType != 21)
					continue;
				if (num8 == 10 && WorldGen.genRand.Next(4) == 0)
				{
					chest.item[num13].SetDefaults(2204);
					num13++;
				}

				if (num8 == 11 && WorldGen.genRand.Next(7) == 0)
				{
					chest.item[num13].SetDefaults(2198);
					num13++;
				}

				if (num8 == 13 && WorldGen.genRand.Next(3) == 0)
				{
					chest.item[num13].SetDefaults(2197);
					num13++;
				}

				if (num8 == 16)
				{
					chest.item[num13].SetDefaults(2195);
					num13++;
				}

				if (Main.wallDungeon[Main.tile[i, k].wall] && WorldGen.genRand.Next(8) == 0)
				{
					chest.item[num13].SetDefaults(2192);
					num13++;
				}

				if (num8 == 16)
				{
					if (WorldGen.genRand.Next(5) == 0)
					{
						chest.item[num13].SetDefaults(2767);
						num13++;
					}
					else
					{
						chest.item[num13].SetDefaults(2766);
						chest.item[num13].stack = WorldGen.genRand.Next(3, 8);
						num13++;
					}
				}
			}
		}
	}
}