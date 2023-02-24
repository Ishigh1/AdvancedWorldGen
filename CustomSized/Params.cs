namespace AdvancedWorldGen.CustomSized;

public static class Params
{
	public static Dictionary<string, object> Data = new();

	public static int SizeX
	{
		get => (int)Data[nameof(SizeX)];
		set => Data[nameof(SizeX)] = value;
	}

	public static int SizeY
	{
		get => (int)Data[nameof(SizeY)];
		set => Data[nameof(SizeY)] = value;
	}

	public static float TempleMultiplier
	{
		get => (float)Data[nameof(TempleMultiplier)];
		set => Data[nameof(TempleMultiplier)] = value;
	}

	public static float DungeonMultiplier
	{
		get => (float)Data[nameof(DungeonMultiplier)];
		set => Data[nameof(DungeonMultiplier)] = value;
	}

	public static float BeachMultiplier
	{
		get => (float)Data[nameof(BeachMultiplier)];
		set => Data[nameof(BeachMultiplier)] = value;
	}

	public static int Copper
	{
		get => (int)Data[nameof(Copper)];
		set => Data[nameof(Copper)] = value;
	}

	public static int Iron
	{
		get => (int)Data[nameof(Iron)];
		set => Data[nameof(Iron)] = value;
	}

	public static int Silver
	{
		get => (int)Data[nameof(Silver)];
		set => Data[nameof(Silver)] = value;
	}

	public static int Gold
	{
		get => (int)Data[nameof(Gold)];
		set => Data[nameof(Gold)] = value;
	}

	public static int Cobalt
	{
		get => (int)Data[nameof(Cobalt)];
		set => Data[nameof(Cobalt)] = value;
	}

	public static int Mythril
	{
		get => (int)Data[nameof(Mythril)];
		set => Data[nameof(Mythril)] = value;
	}

	public static int Adamantite
	{
		get => (int)Data[nameof(Adamantite)];
		set => Data[nameof(Adamantite)] = value;
	}

	public static bool ScaledBeaches
	{
		get => (bool)Data[nameof(ScaledBeaches)];
		set => Data[nameof(ScaledBeaches)] = value;
	}

	public static bool EditTerrainPass
	{
		get => (bool)Data[nameof(EditTerrainPass)];
		set => Data[nameof(EditTerrainPass)] = value;
	}

	public static TerrainType TerrainType
	{
		get => (TerrainType)Data[nameof(TerrainType)];
		set => Data[nameof(TerrainType)] = value;
	}

	public static object Get(string name)
	{
		return Data[name];
	}

	public static void Set(string name, object value)
	{
		Data[name] = value;
	}

	public static void Wipe()
	{
		Data.Clear();
		Initialize();
	}

	private static void Initialize()
	{
		SizeX = -1;
		SizeY = -1;
		TempleMultiplier = 1;
		DungeonMultiplier = 1;
		BeachMultiplier = 1;

		Copper = TileExpandableList.Random;
		Iron = TileExpandableList.Random;
		Silver = TileExpandableList.Random;
		Gold = TileExpandableList.Random;
		
		Cobalt = TileExpandableList.Random;
		Mythril = TileExpandableList.Random;
		Adamantite = TileExpandableList.Random;

		ScaledBeaches = false;
		EditTerrainPass = false;
		TerrainType = TerrainType.Normal;
	}

	public static bool TryGetValue(string key, out object? value)
	{
		return Data.TryGetValue(key, out value);
	}
}