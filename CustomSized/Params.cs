using System.Collections.Generic;

namespace AdvancedWorldGen.CustomSized;

public class Params
{
	public Dictionary<string, object> Data = new();

	public int SizeX
	{
		get => (int)Data[nameof(SizeX)];
		set => Data[nameof(SizeX)] = value;
	}

	public int SizeY
	{
		get => (int)Data[nameof(SizeY)];
		set => Data[nameof(SizeY)] = value;
	}

	public float TempleMultiplier
	{
		get => (float)Data[nameof(TempleMultiplier)];
		set => Data[nameof(TempleMultiplier)] = value;
	}

	public Params()
	{
		Initialize();
	}

	public void Wipe()
	{
		Data.Clear();
		Initialize();
	}

	public void Initialize()
	{
		SizeX = -1;
		SizeY = -1;
		TempleMultiplier = 1;
	}
}