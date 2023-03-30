namespace AdvancedWorldGen.Base;

public class Legacy
{
	public static void MoveFiles()
	{
		#region 2.9, 30/03/2023
		string advancedWorldGenPassesDataPath = Path.Combine(Main.SavePath, "AdvancedWorldGenPassesData.json");
		if (File.Exists(advancedWorldGenPassesDataPath))
		{
			File.Move(advancedWorldGenPassesDataPath, Path.Combine(AdvancedWorldGenMod.FolderPath, "PassesData.json"));
		}
		#endregion
	}
	
	public static void ReplaceOldOptions(ICollection<string> options)
	{
		#region 2.6, 29/12/2021

		if (options.Remove("Crimruption"))
			options.Add("Drunk.Crimruption");

		#endregion

		#region 2.6.4.5, 23/03/2022

		if (options.Remove("Painted"))
			options.Add("Random.Painted");

		#endregion
	}
}