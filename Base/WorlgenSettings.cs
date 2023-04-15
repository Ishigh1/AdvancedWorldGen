namespace AdvancedWorldGen.Base;

public class WorldgenSettings : ModConfig
{
	[Label("$Mods.AdvancedWorldGen.Config.Analytics.Label")]
	[Tooltip("$Mods.AdvancedWorldGen.Config.Analytics.Tooltip")]
	[DefaultValue(false)]
	[ReloadRequired]
	public bool Analytics;

	[Label("$Mods.AdvancedWorldGen.Config.FasterWorldgen.Label")]
	[Tooltip("$Mods.AdvancedWorldGen.Config.FasterWorldgen.Tooltip")]
	[DefaultValue(false)]
	public bool FasterWorldgen;

	[Label("$Mods.AdvancedWorldGen.Config.SaveOnFail.Label")]
	[Tooltip("$Mods.AdvancedWorldGen.Config.SaveOnFail.Tooltip")]
	[DefaultValue(false)]
	public bool SaveOnFail;

	[Label("$Mods.AdvancedWorldGen.Config.VanillaWeight.Label")]
	[Tooltip("$Mods.AdvancedWorldGen.Config.VanillaWeight.Tooltip")]
	[DefaultValue(false)]
	public bool VanillaWeight;

	[Label("$Mods.AdvancedWorldGen.Config.ZenithEnabler.Label")]
	[Tooltip("$Mods.AdvancedWorldGen.Config.ZenithEnabler.Tooltip")]
	[DefaultValue(true)]
	public bool ZenithEnabler;

	public static WorldgenSettings Instance => ModContent.GetInstance<WorldgenSettings>();

	public override ConfigScope Mode => ConfigScope.ServerSide;
}