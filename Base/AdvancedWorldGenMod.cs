namespace AdvancedWorldGen.Base;

public class AdvancedWorldGenMod : Mod
{
	public UiChanger UiChanger = null!;
	public static AdvancedWorldGenMod Instance => ModContent.GetInstance<AdvancedWorldGenMod>();

	public override void Load()
	{
		TileReplacer.Initialize();

		UiChanger = new UiChanger(this);

		OnUIWorldCreation.AddDescriptionPanel += UiChanger.TweakWorldGenUi;
		OnUIWorldCreation.FinishCreatingWorld += ModifiedWorld.Instance.LastMinuteChecks;
		OnWorldFileData.SetWorldSize += UiChanger.SetSpecialName;
		// OnUIWorldListItem.ctor += UiChanger.CopySettingsButton; // Removed until twld can be loaded in a reasonable time

		ILWorldGen.GenerateWorld += ModifiedWorld.OverrideWorldOptions;
		OnWorldFile.CreateMetadata += DedServUi.DedServOptions;

		OnUIWorldLoad.ctor += UiChanger.AddCancel;
		OnWorldGen.do_worldGenCallBack += UiChanger.ThreadifyWorldGen;

		OnUserInterface.SetState += ModifiedWorld.Instance.ResetSettings;

		OnWorldGen.NotTheBees += ClassicOptions.SmallNotTheBees;
		ILWorldGen.makeTemple += ClassicOptions.ReduceTemple;

		OnMain.UpdateTime_StartDay += ModifiedWorld.Instance.OnDawn;
		OnMain.UpdateTime_StartNight += ModifiedWorld.Instance.OnDusk;
		OnMain.checkXMas += SnowWorld.MainOnCheckXMas;
		ILProjectile.Kill += SnowWorld.RemoveSnowDropDuringChristmas;

		ILWorldGen.MakeDungeon += DrunkOptions.CrimruptionChest;

		Replacer.Replace();

		HalloweenCommon.Setup();
	}

	public override void HandlePacket(BinaryReader reader, int whoAmI)
	{
		OptionHelper.HandlePacket(reader);
	}
}