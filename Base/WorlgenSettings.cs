namespace AdvancedWorldGen.Base;

public class WorldgenSettings : ModConfig
{
	[DefaultValue(false)]
	[ReloadRequired]
	[LabelKey("$Mods.AdvancedWorldGen.Config.Analytics.Label")]
	[TooltipKey("$Mods.AdvancedWorldGen.Config.Analytics.Tooltip")]
	public bool Analytics;

	[DefaultValue(false)]
	[ReloadRequired]
	[LabelKey("$Mods.AdvancedWorldGen.Config.FasterWorldgen.Label")]
	[TooltipKey("$Mods.AdvancedWorldGen.Config.FasterWorldgen.Tooltip")]
	public bool FasterWorldgen;

	[DefaultValue(false)]
	[LabelKey("$Mods.AdvancedWorldGen.Config.SaveOnFail.Label")]
	[TooltipKey("$Mods.AdvancedWorldGen.Config.SaveOnFail.Tooltip")]
	public bool SaveOnFail;

	[DefaultValue(false)]
	[LabelKey("$Mods.AdvancedWorldGen.Config.VanillaWeight.Label")]
	[TooltipKey("$Mods.AdvancedWorldGen.Config.VanillaWeight.Tooltip")]
	public bool VanillaWeight;

	[DefaultValue(true)]
	[LabelKey("$Mods.AdvancedWorldGen.Config.ZenithEnabler.Label")]
	[TooltipKey("$Mods.AdvancedWorldGen.Config.ZenithEnabler.Tooltip")]
	public bool ZenithEnabler;

	public static WorldgenSettings Instance => ModContent.GetInstance<WorldgenSettings>();

	public override ConfigScope Mode => ConfigScope.ServerSide;
}