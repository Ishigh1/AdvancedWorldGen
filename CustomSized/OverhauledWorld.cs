using AdvancedWorldGen.Helper;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AdvancedWorldGen.CustomSized;

public class OverhauledWorld : ModSystem
{
	public override void LoadWorldData(TagCompound tag)
	{
		if (tag.TryGetValue("ocean", out int value)) WorldGen.oceanDistance = value;
		if (tag.TryGetValue("beach", out value)) WorldGen.beachDistance = value;
	}

	public override void SaveWorldData(TagCompound tagCompound)
	{
		if (Main.maxTilesX < 2400)
		{
			tagCompound["ocean"] = WorldGen.oceanDistance;
			tagCompound["beach"] = WorldGen.beachDistance;
		}
	}

	public override void OnWorldUnload()
	{
		WorldGen.oceanDistance = 250;
		WorldGen.beachDistance = 380;
	}
}