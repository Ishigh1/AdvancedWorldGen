namespace AdvancedWorldGen.Base;

public class Legacy
{
	public static void MoveFiles()
	{
		#region 2.9, 30/03/2023

		string advancedWorldGenPassesDataPath = Path.Combine(Main.SavePath, "AdvancedWorldGenPassesData.json");
		if (File.Exists(advancedWorldGenPassesDataPath))
			File.Move(advancedWorldGenPassesDataPath, ModifiedWorld.DataPath);

		#endregion

		#region 3.1, 04/04/2023

		string presetDataPath = Path.Combine(AdvancedWorldGenMod.FolderPath, "Presets.json");
		if (File.Exists(presetDataPath)) File.Move(presetDataPath, PresetUI.DataPath);

		#endregion
	}
}