using System.Collections.Generic;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.BetterVanillaWorldGen;
using AdvancedWorldGen.Helper;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.SpecialOptions
{
	public class TileReplacer : ControlledWorldGenPass
	{
		public const int None = -1;
		public const int Water = -2;
		public const int Lava = -3;
		public const int Honey = -4;

		public static List<int> NotReplaced = null!;

		public Dictionary<int, int> DirectReplacements;
		public Dictionary<int, SpecialCase> SpecialCases;

		public TileReplacer(string name, float weight) : base(name, weight)
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

		public void UpdateDictionary(int to, params int[] from)
		{
			foreach (int tile in from) DirectReplacements.Add(tile, to);
		}
		
		public override void ApplyPass()
		{
			Progress.Message = Language.GetTextValue("Mods.AdvancedWorldGen.WorldGenMessage." + Name);
			for (int x = 0; x < Main.maxTilesX; x++)
			{
				Progress.SetProgress(x, Main.maxTilesX);
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
			{
				tile.LiquidAmount = 0;
				if (!NotReplaced.Contains(tile.type))
					tile.IsActive = false;
			}
			else
			{
				tile.IsActive = false;
			}

			switch (type)
			{
				case > -1:
					if (!tile.IsActive)
					{
						tile.IsActive = true;
						tile.type = (ushort) type;
						WorldGen.DiamondTileFrame(x, y);
					}

					break;
				case < -1:
					tile.LiquidAmount = byte.MaxValue;
					tile.LiquidType = -type + 2;
					break;
			}
		}
	}
}