namespace AdvancedWorldGen.SpecialOptions;

public class TileReplacer : GenPass
{
	public const int None = -1;
	public const int Water = -2;
	public const int Lava = -3;
	public const int Honey = -4;

	public static HashSet<int> NotReplaced = null!;

	private new string Name;
	private Dictionary<int, int> DirectReplacements;
	public Dictionary<int, SpecialCase> SpecialCases;

	public TileReplacer(string name) : base("replace_" + name, 200f)
	{
		Name = name;
		DirectReplacements = new Dictionary<int, int>();
		SpecialCases = new Dictionary<int, SpecialCase>();
	}

	public static void Initialize()
	{
		NotReplaced = new HashSet<int>
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

	protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
	{
		progress.Message = Language.GetTextValue("Mods.AdvancedWorldGen.WorldGenMessage." + Name);
		for (int x = 0; x < Main.maxTilesX; x++)
		{
			progress.Set(x, Main.maxTilesX);
			for (int y = 0; y < Main.maxTilesY; y++)
			{
				Tile tile = Main.tile[x, y];
				if (tile.HasTile) HandleReplacement(tile.TileType, x, y, tile);

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
			tile.HasTile = false;

		switch (type)
		{
			case > -1:
				if (!tile.HasTile)
				{
					tile.HasTile = true;
					tile.TileType = (ushort)type;
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