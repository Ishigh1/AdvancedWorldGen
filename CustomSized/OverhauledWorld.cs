using System.Diagnostics.CodeAnalysis;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class OverhauledWorld : ModSystem
	{
		public override void LoadWorldData(TagCompound tag)
		{
			if (TryGetValue(tag, "ocean", out object? value)) WorldGen.oceanDistance = (int) value;
			if (TryGetValue(tag, "beach", out value)) WorldGen.beachDistance = (int) value;
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

		public static bool TryGetValue(TagCompound tagCompound, string name, [NotNullWhen(true)] out object? value)
		{
			if (tagCompound.ContainsKey(name))
			{
				value = tagCompound[name];
				return true;
			}

			value = default;
			return false;
		}
	}
}