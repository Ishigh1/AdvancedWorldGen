namespace AdvancedWorldGen.Helper;

public class JsonRange
{
	public enum ScalingMode
	{
		None,
		WorldArea,
		WorldWidth,
		WorldHeight
	}

	[JsonProperty("Max")] public readonly double Maximum;

	[JsonProperty("Min")] public readonly double Minimum;

	[JsonProperty] [JsonConverter(typeof(StringEnumConverter))]
	public readonly ScalingMode ScaleWith;

	public JsonRange(int minimum, int maximum)
	{
		Minimum = minimum;
		Maximum = maximum;
	}

	public double ScaledMinimum => ScaleValue(Minimum);

	public double ScaledMaximum => ScaleValue(Maximum);

	public double GetRandom(UnifiedRandom random)
	{
		return random.Next(ScaledMinimum, ScaledMaximum + 1);
	}

	private double ScaleValue(double value)
	{
		double num = ScaleWith switch
		{
			ScalingMode.WorldArea => Main.maxTilesX * Main.maxTilesY / (4200.0 * 1200.0),
			ScalingMode.WorldWidth => Main.maxTilesX / 4200.0,
			ScalingMode.WorldHeight => Main.maxTilesY / 1200.0,
			_ => 1.0
		};

		return num * value;
	}
}