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
	
	[Label("$Mods.AdvancedWorldGen.Config.VanillaWeight.Label")]
	[Tooltip("$Mods.AdvancedWorldGen.Config.VanillaWeight.Tooltip")]
	[DefaultValue(false)]
	public bool VanillaWeight;

	public static WorldgenSettings Instance => ModContent.GetInstance<WorldgenSettings>();

	public override ConfigScope Mode => ConfigScope.ServerSide;
}