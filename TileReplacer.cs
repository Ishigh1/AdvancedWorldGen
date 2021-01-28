using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.World.Generation;
using static Terraria.ID.TileID;


namespace AdvancedSeedGen
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
		public const ushort Water = Count;
		public const ushort Lava = Count + 1;
		public const ushort Honey = Count + 2;
		public static TileReplacer Snow;


		public Dictionary<ushort, int> Dictionary;
		public Dictionary<ushort, SpecialCase> SpecialCases;

		public TileReplacer(Dictionary<ushort, int> dictionary, Dictionary<ushort, SpecialCase> specialCases)
		{
			Dictionary = dictionary;
			SpecialCases = specialCases;
		}

		public static void Initialize()
		{
			Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
			UpdateDictionary(dictionary, SnowBlock, Dirt, Grass, CorruptGrass, ClayBlock, FleshGrass);
			UpdateDictionary(dictionary, IceBlock, Stone, GreenMoss, BrownMoss, RedMoss, BlueMoss,
				PurpleMoss, LavaMoss);
			UpdateDictionary(dictionary, BreakableIce, Water);
			UpdateDictionary(dictionary, CorruptIce, Ebonstone);
			UpdateDictionary(dictionary, FleshIce, Crimstone);
			UpdateDictionary(dictionary, Slush, Silt);
			UpdateDictionary(dictionary, BorealWood, WoodBlock);
			UpdateDictionary(dictionary, SnowCloud, TileID.Cloud, RainCloud);
			UpdateDictionary(dictionary, None, Plants, CorruptPlants, Sunflower, CorruptThorns, Vines, Plants2,
				FleshWeeds,
				CrimsonVines, VineFlowers);

			Dictionary<ushort, SpecialCase> specialCases = new Dictionary<ushort, SpecialCase>
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
					DyePlants, new SpecialCase(None,
						(x, y, tile) => tile.frameX == 32 * 3 || tile.frameX == 32 * 4 || tile.frameX == 32 * 7)
				}
			};

			Snow = new TileReplacer(dictionary, specialCases);
		}

		public static void Unload()
		{
			Snow = null;
		}

		public static void UpdateDictionary(Dictionary<ushort, int> dictionary, int to,
			params ushort[] from)
		{
			foreach (ushort tile in from) dictionary.Add(tile, to);
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
					if (tile.active()) HandleReplacement(tile.type, i, j, tile, false);

					if (tile.liquid > 0) HandleReplacement((ushort) (tile.liquidType() + Count), i, j, tile, true);
				}
			}
		}

		private void HandleReplacement(ushort tileType, int i, int j, Tile tile, bool liquid)
		{
			if (!Dictionary.TryGetValue(tileType, out int type))
			{
				if (!SpecialCases.TryGetValue(tileType, out SpecialCase specialCase) ||
				    !specialCase.IsValid(i, j, tile))
					return;
				type = specialCase.Type;
			}

			if (liquid && tile.active() && (type < Count || type == -1)) return;

			if (type == -1)
			{
				if (liquid)
					tile.liquid = 0;
				else
					tile.active(false);
			}
			else if (type < Count)
			{
				if (liquid)
				{
					tile.active(true);
					tile.liquid = 0;
				}

				tile.type = (ushort) type;
			}
			else
			{
				if (!liquid)
				{
					tile.active(false);
					tile.liquid = byte.MaxValue;
				}

				tile.liquidType(type - Count);
			}
		}

		public static void RandomizeWorld(GenerationProgress progress, SeedHelper seedHelper)
		{
			Dictionary<ushort, ushort> tileRandom = new Dictionary<ushort, ushort>();
			Dictionary<ushort, ushort> wallRandom = new Dictionary<ushort, ushort>();
			Dictionary<ushort, byte> paintRandom = new Dictionary<ushort, byte>();
			Dictionary<ushort, byte> paintWallRandom = new Dictionary<ushort, byte>();

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
						if (tile.active()) RandomizeTile(seedHelper, tile, tileRandom, paintRandom);

						if (tile.wall != 0) RandomizeWall(seedHelper, wallRandom, tile, paintWallRandom);
					}

					FallSand(i, j - 1);
				}
			}
		}

		public static void RandomizeWall(SeedHelper seedHelper, Dictionary<ushort, ushort> wallRandom, Tile tile,
			Dictionary<ushort, byte> paintWallRandom)
		{
			if (seedHelper.OptionsContains("Random"))
			{
				if (wallRandom.TryGetValue(tile.wall, out ushort type))
				{
					tile.wall = type;
				}
				else
				{
					do
					{
						type = (ushort) Main.rand.Next(1, WallID.Count);
					} while (wallRandom.ContainsValue(type));

					wallRandom[tile.wall] = type;
					tile.wall = type;
				}
			}

			if (seedHelper.OptionsContains("Paint"))
			{
				if (!paintWallRandom.TryGetValue(tile.wall, out byte paint))
				{
					paint = (byte) Main.rand.Next(PaintID.Count);
					paintWallRandom[tile.wall] = paint;
				}

				tile.wallColor(paint);
			}
		}

		public static void RandomizeTile(SeedHelper seedHelper, Tile tile, Dictionary<ushort, ushort> tileRandom,
			Dictionary<ushort, byte> paintRandom)
		{
			if (seedHelper.OptionsContains("Random"))
			{
				if (Main.tileSolid[tile.type])
				{
					if (tileRandom.TryGetValue(tile.type, out ushort type))
					{
						tile.type = type;
					}
					else if (Main.tileSolid[tile.type] && !Sets.Platforms[tile.type] &&
					         !AdvancedSeedGen.NotReplaced.Contains(tile.type))
					{
						do
						{
							type = (ushort) Main.rand.Next(Count);
						} while (!Main.tileSolid[type] || Sets.Platforms[type] ||
						         AdvancedSeedGen.NotReplaced.Contains(type) ||
						         tileRandom.ContainsValue(type));

						tileRandom[tile.type] = type;
						tile.type = type;
					}
				}
				else if (tile.type == Cactus)
				{
					tile.active(false);
				}
			}

			if (seedHelper.OptionsContains("Paint"))
			{
				if (!paintRandom.TryGetValue(tile.type, out byte paint))
				{
					paint = (byte) Main.rand.Next(PaintID.Count);
					paintRandom[tile.type] = paint;
				}

				tile.color(paint);
			}
		}

		private static void FallSand(int x, int y)
		{
			while (true)
			{
				if (y >= 0)
				{
					Tile tile = Main.tile[x, y];
					if (tile != null && tile.active() && Sets.Falling[tile.type] &&
					    !Main.tile[x, y + 1].active())
					{
						if (Main.tile[x, y + 1] == null) Main.tile[x, y + 1] = new Tile();

						Main.tile[x, y + 1].active(true);
						Main.tile[x, y + 1].type = tile.type;
						Main.tile[x, y].active(false);

						y--;
						continue;
					}
				}

				break;
			}
		}
	}
}