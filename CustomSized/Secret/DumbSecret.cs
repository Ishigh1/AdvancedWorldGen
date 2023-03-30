namespace AdvancedWorldGen.CustomSized.Secret;

public class DumbSecret : ModSystem
{
	public static string? SecretString;

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
	{
		if (SecretString == "Spookypizza")
		{
			totalWeight = 0;
			tasks.Clear();
			Main.spawnTileX = 10;
			Main.spawnTileY = 10;
		}
	}

	public override void OnWorldUnload()
	{
		SecretString = null;
	}
}