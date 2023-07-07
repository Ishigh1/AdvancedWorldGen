namespace AdvancedWorldGen.Base;

public class WorldgenSettings : ModConfig
{
	[DefaultValue(false)]
	[ReloadRequired]
	public bool Analytics;

	[DefaultValue(false)]
	[ReloadRequired]
	public bool FasterWorldgen;

	[DefaultValue(false)]
	public bool SaveOnFail;

	[DefaultValue(false)]
	public bool VanillaWeight;

	[DefaultValue(true)]
	public bool ZenithEnabler;

	public static WorldgenSettings Instance => ModContent.GetInstance<WorldgenSettings>();

	public override ConfigScope Mode => ConfigScope.ServerSide;
}