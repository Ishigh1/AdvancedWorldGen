using AdvancedWorldGen.Helper;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AdvancedWorldGen.CustomSized
{
	public class OverhauledWorld : ModSystem
	{
		public override void LoadWorldData(TagCompound tag)
		{
			if (tag.TryGetValue("ocean", out int value)) WorldGen.oceanDistance = value;
			if (tag.TryGetValue("beach", out value)) WorldGen.beachDistance = value;
		}

		public override TagCompound SaveWorldData()
		{
			TagCompound data = new();
			if (Main.maxTilesX < 2400)
			{
				data["ocean"] = WorldGen.oceanDistance;
				data["beach"] = WorldGen.beachDistance;
			}

			return data;
		}

		public override void OnWorldUnload()
		{
			WorldGen.oceanDistance = 250;
			WorldGen.beachDistance = 380;
		}
	}
}