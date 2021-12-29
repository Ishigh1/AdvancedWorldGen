using System.Collections.Generic;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.Helper;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.SpecialOptions;

public class TileReplacer
{
	public const int None = -1;
	public const int Water = -2;
	public const int Lava = -3;
	public const int Honey = -4;

	public static List<int> NotReplaced = null!;

	public Dictionary<int, int> DirectReplacements;
	public Dictionary<int, SpecialCase> SpecialCases;

	public TileReplacer()
	{
		DirectReplacements = new Dictionary<int, int>();
		SpecialCases = new Dictionary<int, SpecialCase>();
	}

	public static void Initialize()
	{
		NotReplaced = new List<int>
		{
			TileID.ClosedDoor,
			TileID.MagicalIceBlock,
			TileID.Traps,
			TileID.Boulder,
			TileID.Teleporter,
			TileID.MetalBars,
			TileID.PlanterBox,
			TileID.TrapdoorClosed,
			TileID.TallGateClosed
		};
		IEnumerable<ModTile> modTiles = ModLoader.GetMod("ModLoader").GetContent<ModTile>();

		foreach (ModTile modTile in modTiles) NotReplaced.Add(modTile.Type);
	}

	public static bool DontReplace(int type)
	{
		return !Main.tileSolid[type] || TileID.Sets.Platforms[type] || NotReplaced.Contains(type) ||
		       Main.tileFrameImportant[type];
	}

	public void UpdateDictionary(int to, params int[] from)
	{
		foreach (int tile in from) DirectReplacements.Add(tile, to);
	}

	public void ReplaceTiles(GenerationProgress progress, string s)
	{
		progress.Message = Language.GetTextValue("Mods.AdvancedWorldGen.WorldGenMessage." + s);
		for (int x = 0; x < Main.maxTilesX; x++)
		{
			progress.Set(x, Main.maxTilesX);
			for (int y = 0; y < Main.maxTilesY; y++)
			{
				Tile tile = Main.tile[x, y];
				if (tile == null) continue;
				if (tile.IsActive) HandleReplacement(tile.type, x, y, tile);

				if (tile.LiquidAmount > 0)
					HandleReplacement(-tile.LiquidType - 2, x, y, tile);
			}
		}
	}

	public void HandleReplacement(int tileType, int x, int y, Tile tile)
	{
		if (!DirectReplacements.TryGetValue(tileType, out int type))
		{
			if (!SpecialCases.TryGetValue(tileType, out SpecialCase? specialCase) || !specialCase.IsValid(x, y, tile))
				return;
			type = specialCase.Type;
		}

		if (tileType < -1)
			tile.LiquidAmount = 0;
		else
			tile.IsActive = false;

		switch (type)
		{
			case > -1:
				if (!tile.IsActive)
				{
					tile.IsActive = true;
					tile.type = (ushort)type;
					WorldGen.DiamondTileFrame(x, y);
				}

				break;
			case < -1:
				tile.LiquidAmount = byte.MaxValue;
				tile.LiquidType = -type + 2;
				break;
		}
	}

	public static void RandomizeWorld(GenerationProgress progress, OptionHelper optionHelper)
	{
		Dictionary<ushort, ushort> tileRandom = new();
		Dictionary<ushort, ushort> wallRandom = new();
		Dictionary<ushort, byte> paintRandom = new();
		Dictionary<ushort, byte> paintWallRandom = new();

		progress.Message = Language.GetTextValue("Mods.AdvancedWorldGen.WorldGenMessage.Random");
		for (int x = 0; x < Main.maxTilesX; x++)
		{
			progress.Set(x, Main.maxTilesX, 0.5f);
			for (int y = 0; y < Main.maxTilesY; y++)
			{
				Tile tile = Main.tile[x, y];
				if (tile != null)
				{
					if (tile.IsActive) RandomizeTile(optionHelper, tile, tileRandom, paintRandom);

					if (tile.wall != 0) RandomizeWall(optionHelper, wallRandom, tile, paintWallRandom);
				}
			}
		}

		for (int x = 0; x < Main.maxTilesX; x++)
		{
			progress.Set(x, Main.maxTilesX, 0.5f, 0.5f);
			int previousBlock = 0;
			for (int y = Main.maxTilesY - 1; y >= 1; y--)
			{
				Tile tile = Main.tile[x, y];
				if (!tile.IsActive)
					continue;
				if (tile.type == TileID.Cactus)
				{
					WorldGen.CheckCactus(x, y);
					continue;
				}

				if (previousBlock != 0 && y != previousBlock - 1 &&
				    TileID.Sets.Falling[tile.type])
				{
					Tile tileAbove = Main.tile[x, y - 1];
					if (tileAbove.IsActive && !Main.tileSolid[tileAbove.type])
					{
						WorldGen.KillTile(x, y - 1);
						if (!tileAbove.IsActive)
						{
							previousBlock = y;
							continue;
						}
					}

					Tile newPos = Main.tile[x, previousBlock - 1];
					newPos.IsActive = true;
					newPos.type = tile.type;
					newPos.IsHalfBlock = tile.IsHalfBlock;
					newPos.Slope = tile.Slope;
					tile.IsActive = false;
					previousBlock--;
				}
				else
				{
					previousBlock = y;
				}
			}
		}
	}

	public static void RandomizeTile(OptionHelper optionHelper, Tile tile, Dictionary<ushort, ushort> tileRandom,
		Dictionary<ushort, byte> paintRandom)
	{
		if (optionHelper.OptionsContains("Random"))
			if (Main.tileSolid[tile.type])
			{
				if (tileRandom.TryGetValue(tile.type, out ushort type))
				{
					tile.type = type;
				}
				else if (!DontReplace(type))
				{
					do
					{
						type = (ushort)WorldGen._genRand.Next(TileLoader.TileCount);
					} while (DontReplace(type) || tileRandom.ContainsValue(type));

					tileRandom[tile.type] = type;
					tile.type = type;
				}
			}

		if (optionHelper.OptionsContains("Painted"))
		{
			if (!paintRandom.TryGetValue(tile.type, out byte paint))
			{
				paint = (byte)WorldGen._genRand.Next(PaintID.IlluminantPaint + 1);
				paintRandom[tile.type] = paint;
			}

			tile.Color = paint;
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
					type = (ushort)WorldGen._genRand.Next(1, WallLoader.WallCount);
				} while (wallRandom.ContainsValue(type));

				wallRandom[tile.wall] = type;
				tile.wall = type;
			}
		}

		if (optionHelper.OptionsContains("Painted"))
		{
			if (!paintWallRandom.TryGetValue(tile.wall, out byte paint))
			{
				paint = (byte)WorldGen._genRand.Next(PaintID.IlluminantPaint + 1);
				paintWallRandom[tile.wall] = paint;
			}

			tile.WallColor = paint;
		}
	}
}