using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.ID.TileID;

namespace AdvancedWorldGen.Base
{
	public class SpecialCase
	{
		public Func<int, int, Tile, bool> Condition;
		public int Type;

		public SpecialCase(int type, Func<int, int, Tile, bool> condition = null)
		{
			Type = type;
			Condition = condition;
		}

		public bool IsValid(int x, int y, Tile tile)
		{
			return Condition == null || Condition.Invoke(x, y, tile);
		}
	}

	public class TileReplacer
	{
		public const int None = -1;
		public const int Water = -2;
		public const int Lava = -3;
		public const int Honey = -4;

		public static List<int> NotReplaced;

		public Dictionary<int, int> Dictionary;
		public Dictionary<int, SpecialCase> SpecialCases;

		public TileReplacer(Dictionary<int, int> dictionary, Dictionary<int, SpecialCase> specialCases)
		{
			Dictionary = dictionary;
			SpecialCases = specialCases;
		}

		public static TileReplacer Snow
		{
			get
			{
				Dictionary<int, int> dictionary = new();
				UpdateDictionary(dictionary, SnowBlock, Dirt, Grass, CorruptGrass, ClayBlock, CrimsonGrass);
				UpdateDictionary(dictionary, IceBlock, Stone, GreenMoss, BrownMoss, RedMoss, BlueMoss,
					PurpleMoss, LavaMoss, KryptonMoss, XenonMoss, ArgonMoss);
				UpdateDictionary(dictionary, CorruptIce, Ebonstone);
				UpdateDictionary(dictionary, FleshIce, Crimstone);
				UpdateDictionary(dictionary, BorealWood, WoodBlock);
				UpdateDictionary(dictionary, BreakableIce, Water);
				UpdateDictionary(dictionary, Slush, Silt);
				UpdateDictionary(dictionary, None, Plants, CorruptPlants, Sunflower, Vines, Plants2, CrimsonPlants,
					CrimsonVines, VineFlowers, CorruptThorns, CrimsonThorns);
				Dictionary<int, SpecialCase> specialCases = new()
				{
					{
						ImmatureHerbs, new SpecialCase(None,
							(x, y, tile) => tile.frameX == 0 || tile.frameX == 32 || tile.frameX == 32 * 2 ||
							                tile.frameX == 32 * 3 || tile.frameX == 32 * 6)
					},
					{
						MatureHerbs, new SpecialCase(None,
							(x, y, tile) => tile.frameX == 0 || tile.frameX == 32 || tile.frameX == 32 * 2 ||
							                tile.frameX == 32 * 3 || tile.frameX == 32 * 6)
					},
					{
						BloomingHerbs, new SpecialCase(None,
							(x, y, tile) => tile.frameX == 0 || tile.frameX == 32 || tile.frameX == 32 * 2 ||
							                tile.frameX == 32 * 3 || tile.frameX == 32 * 6)
					},
					{
						Cattail, new SpecialCase(None,
							(x, y, tile) => tile.frameY == 0 || tile.frameY == 32 * 3 || tile.frameY == 32 * 4)
					},
					{
						LilyPad, new SpecialCase(None,
							(x, y, tile) => tile.frameY == 0 || tile.frameY == 32 * 3 || tile.frameY == 32 * 4)
					},
					{
						DyePlants, new SpecialCase(None,
							(x, y, tile) => tile.frameX == 32 * 3 || tile.frameX == 32 * 4 || tile.frameX == 32 * 7)
					}
				};

				return new TileReplacer(dictionary, specialCases);
			}
		}

		public static void Initialize()
		{
			NotReplaced = new List<int>
			{
				ClosedDoor, MagicalIceBlock, Traps, Boulder, Teleporter, MetalBars, PlanterBox, TrapdoorClosed,
				TallGateClosed
			};
		}

		public static void Unload()
		{
			NotReplaced = null;
		}

		public static void UpdateDictionary(Dictionary<int, int> dictionary, int to,
			params int[] from)
		{
			foreach (int tile in from) dictionary.Add(tile, to);
		}

		public void ReplaceTiles(GenerationProgress progress, string s)
		{
			float step = 1 / (float) Main.maxTilesY;
			float prog = -step;
			progress.Message = s;
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				progress.Set(prog += step);
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile == null) continue;
					if (tile.IsActive) HandleReplacement(tile.type, i, j, tile, false);

					if (tile.LiquidAmount > 0)
						HandleReplacement(-tile.LiquidType - 2, i, j, tile, true);
				}
			}
		}

		public void HandleReplacement(int tileType, int i, int j, Tile tile, bool liquid)
		{
			if (!Dictionary.TryGetValue(tileType, out int type))
			{
				if (!SpecialCases.TryGetValue(tileType, out SpecialCase specialCase) ||
				    !specialCase.IsValid(i, j, tile))
					return;
				type = specialCase.Type;
			}

			if (liquid && tile.IsActive && type > -1) return;

			if (type == -1)
			{
				if (liquid)
					tile.LiquidAmount = 0;
				else
					tile.IsActive = false;
			}
			else if (type > -1)
			{
				if (liquid)
				{
					tile.IsActive = true;
					tile.LiquidAmount = 0;
				}

				tile.type = (ushort) type;
			}
			else
			{
				if (!liquid)
				{
					tile.IsActive = false;
					tile.LiquidAmount = byte.MaxValue;
				}

				tile.LiquidType = -type + 2;
			}
		}

		public static void RandomizeWorld(GenerationProgress progress, OptionHelper optionHelper)
		{
			Dictionary<ushort, ushort> tileRandom = new();
			Dictionary<ushort, ushort> wallRandom = new();
			Dictionary<ushort, byte> paintRandom = new();
			Dictionary<ushort, byte> paintWallRandom = new();

			float step = 1 / (float) Main.maxTilesY;
			float prog = 0;
			progress.Message = "Randomizing tiles";
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				progress.Set(prog += step);
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile != null)
					{
						if (tile.IsActive) RandomizeTile(optionHelper, tile, tileRandom, paintRandom);

						if (tile.wall != 0) RandomizeWall(optionHelper, wallRandom, tile, paintWallRandom);
					}

					FallSand(i, j - 1);
				}
			}
		}

		public static void RandomizeWall(OptionHelper optionHelper, Dictionary<ushort, ushort> wallRandom, Tile tile,
			Dictionary<ushort, byte> paintWallRandom)
		{
			if (optionHelper.OptionsContains("Random"))
			{
				if (wallRandom.TryGetValue(tile.wall, out ushort type))
				{
					tile.wall = type;
				}
				else
				{
					do
					{
						type = (ushort) WorldGen._genRand.Next(1, WallLoader.WallCount);
					} while (wallRandom.ContainsValue(type));

					wallRandom[tile.wall] = type;
					tile.wall = type;
				}
			}

			if (optionHelper.OptionsContains("Painted"))
			{
				if (!paintWallRandom.TryGetValue(tile.wall, out byte paint))
				{
					paint = (byte) WorldGen._genRand.Next(PaintID.IlluminantPaint + 1);
					paintWallRandom[tile.wall] = paint;
				}

				tile.WallColor = paint;
			}
		}

		public static void RandomizeTile(OptionHelper optionHelper, Tile tile, Dictionary<ushort, ushort> tileRandom,
			Dictionary<ushort, byte> paintRandom)
		{
			if (optionHelper.OptionsContains("Random"))
			{
				if (Main.tileSolid[tile.type])
				{
					if (tileRandom.TryGetValue(tile.type, out ushort type))
					{
						tile.type = type;
					}
					else if (Main.tileSolid[tile.type] && !Sets.Platforms[tile.type] &&
					         !NotReplaced.Contains(tile.type))
					{
						do
						{
							type = (ushort) WorldGen._genRand.Next(TileLoader.TileCount);
						} while (!Main.tileSolid[type] || Sets.Platforms[type] ||
						         NotReplaced.Contains(type) ||
						         tileRandom.ContainsValue(type));

						tileRandom[tile.type] = type;
						tile.type = type;
					}
				}
				else if (tile.type == Cactus)
				{
					tile.IsActive = false;
				}
			}

			if (optionHelper.OptionsContains("Painted"))
			{
				if (!paintRandom.TryGetValue(tile.type, out byte paint))
				{
					paint = (byte) WorldGen._genRand.Next(PaintID.IlluminantPaint + 1);
					paintRandom[tile.type] = paint;
				}

				tile.Color = paint;
			}
		}

		public static void FallSand(int x, int y)
		{
			while (y >= 0)
			{
				Tile tile = Main.tile[x, y];
				if (tile != null && tile.IsActive && Sets.Falling[tile.type] &&
				    !Main.tile[x, y + 1].IsActive)
				{
					if (Main.tile[x, y + 1] == null) Main.tile[x, y + 1] = new Tile();

					Main.tile[x, y + 1].IsActive = true;
					Main.tile[x, y + 1].type = tile.type;
					Main.tile[x, y].IsActive = false;

					y--;
					continue;
				}

				break;
			}
		}
	}
}