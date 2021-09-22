using System.Collections.Generic;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.BetterVanillaWorldGen;
using AdvancedWorldGen.Helper;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AdvancedWorldGen.SpecialOptions
{
	public class Randomizer : ControlledWorldGenPass
	{

		public Randomizer() : base("Random", 2)
		{
		}

		public override void ApplyPass()
		{
			Dictionary<ushort, ushort> tileRandom = new();
			Dictionary<ushort, ushort> wallRandom = new();
			Dictionary<ushort, byte> paintRandom = new();
			Dictionary<ushort, byte> paintWallRandom = new();

			Progress.Message = Language.GetTextValue("Mods.AdvancedWorldGen.WorldGenMessage.Random");
			for (int x = 0; x < Main.maxTilesX; x++)
			{
				Progress.SetProgress(x, Main.maxTilesX, 0.5f);
				for (int y = 0; y < Main.maxTilesY; y++)
				{
					Tile tile = Main.tile[x, y];
					if (tile != null)
					{
						OptionHelper optionHelper = ModifiedWorld.Instance.OptionHelper;
						
						if (tile.IsActive) RandomizeTile(optionHelper, tile, tileRandom, paintRandom);

						if (tile.wall != 0) RandomizeWall(optionHelper, wallRandom, tile, paintWallRandom);
					}
				}
			}

			for (int x = 0; x < Main.maxTilesX; x++)
			{
				Progress.SetProgress(x, Main.maxTilesX, 0.5f, 0.5f);
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
					else if (Main.tileSolid[tile.type] && !TileID.Sets.Platforms[tile.type] &&
					         !TileReplacer.NotReplaced.Contains(tile.type))
					{
						do
						{
							type = (ushort) WorldGen._genRand.Next(TileLoader.TileCount);
						} while (!Main.tileSolid[type] || TileID.Sets.Platforms[type] || TileReplacer.NotReplaced.Contains(type) ||
						         tileRandom.ContainsValue(type));

						tileRandom[tile.type] = type;
						tile.type = type;
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
	}
}