namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class WorldgenSettings : ModConfig
{
	[Label("$Mods.AdvancedWorldGen.Config.FasterWorldgen.Label")] [Tooltip("$Mods.AdvancedWorldGen.Config.FasterWorldgen.Tooltip")] [DefaultValue(false)]
	public bool FasterWorldgen;


	[Label("$Mods.AdvancedWorldGen.Config.SaveOnFail.Label")] [Tooltip("$Mods.AdvancedWorldGen.Config.SaveOnFail.Tooltip")] [DefaultValue(false)]
	public bool SaveOnFail;

	public static bool Revamped => ModContent.GetInstance<WorldgenSettings>().FasterWorldgen;

	public static bool AbortedSaving => ModContent.GetInstance<WorldgenSettings>().SaveOnFail;

	public override ConfigScope Mode => ConfigScope.ClientSide;
}