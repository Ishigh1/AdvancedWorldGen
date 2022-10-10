using Newtonsoft.Json.Converters;

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

    [JsonProperty("Min")]
    public readonly double Minimum;
    [JsonProperty("Max")]
    public readonly double Maximum;
    [JsonProperty]
    [JsonConverter(typeof(StringEnumConverter))]
    public readonly ScalingMode ScaleWith;

    public double ScaledMinimum => ScaleValue(Minimum);

    public double ScaledMaximum => ScaleValue(Maximum);

    public JsonRange(int minimum, int maximum) {
        Minimum = minimum;
        Maximum = maximum;
    }

    public double GetRandom(UnifiedRandom random) => random.Next(ScaledMinimum, ScaledMaximum + 1);

    private double ScaleValue(double value)
    {
        double num = ScaleWith switch
        {
            ScalingMode.WorldArea => Main.maxTilesX * Main.maxTilesY / (4200.0 * 1200.0),
            ScalingMode.WorldWidth => Main.maxTilesX / 4200.0,
            ScalingMode.WorldHeight => Main.maxTilesX / 1200.0,
            _ => 1.0
        };

        return num * value;
    }
}