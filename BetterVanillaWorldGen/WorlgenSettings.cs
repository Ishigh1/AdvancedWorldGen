namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class WorldgenSettings : ModConfig
{
	[Label("$Mods.AdvancedWorldGen.Config.FasterWorldgen.Label")]
	[Tooltip("$Mods.AdvancedWorldGen.Config.FasterWorldgen.Tooltip")]
	[DefaultValue(false)]
	public bool FasterWorldgen;
	
	[Label("$Mods.AdvancedWorldGen.Config.SaveOnFail.Label")]
	[Tooltip("$Mods.AdvancedWorldGen.Config.SaveOnFail.Tooltip")]
	[DefaultValue(false)]
	public bool SaveOnFail;

	[Label("$Mods.AdvancedWorldGen.Config.ZenithEnabler.Label")]
	[Tooltip("$Mods.AdvancedWorldGen.Config.ZenithEnabler.Tooltip")]
	[DefaultValue(true)]
	public bool ZenithEnabler;

	public static bool Revamped => ModContent.GetInstance<WorldgenSettings>().FasterWorldgen;

	public static bool AbortedSaving => ModContent.GetInstance<WorldgenSettings>().SaveOnFail;
	
	public static bool ZenithEnables => ModContent.GetInstance<WorldgenSettings>().ZenithEnabler;

	public override ConfigScope Mode => ConfigScope.ServerSide;
}