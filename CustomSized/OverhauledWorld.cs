using AdvancedWorldGen.Helper;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AdvancedWorldGen.CustomSized;

public class OverhauledWorld : ModSystem
{
	public const int BaseOceanDistance = 250;
	public const int BaseBeachDistance = 380;

	public override void LoadWorldData(TagCompound tag)
	{
		if (tag.TryGetValue("ocean", out int value)) WorldGen.oceanDistance = value;
		if (tag.TryGetValue("beach", out value)) WorldGen.beachDistance = value;
	}

	public override void SaveWorldData(TagCompound tagCompound)
	{
		if (WorldGen.oceanDistance != BaseOceanDistance)
			tagCompound["ocean"] = WorldGen.oceanDistance;
		if (WorldGen.beachDistance != BaseBeachDistance)
			tagCompound["beach"] = WorldGen.beachDistance;
	}

	public override void OnWorldUnload()
	{
		WorldGen.oceanDistance = BaseOceanDistance;
		WorldGen.beachDistance = BaseBeachDistance;
	}
}