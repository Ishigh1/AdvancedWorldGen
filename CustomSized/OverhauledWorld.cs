using System.Diagnostics.CodeAnalysis;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AdvancedWorldGen.CustomSized
{
	public class OverhauledWorld : ModSystem
	{
		public override void LoadWorldData(TagCompound tag)
		{
			if (TryGetValue(tag, "ocean", out int value)) WorldGen.oceanDistance = value;
			if (TryGetValue(tag, "beach", out value)) WorldGen.beachDistance = value;
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

		public static bool TryGetValue<T>(TagCompound tagCompound, string name, [NotNullWhen(true)] out T? value)
		{
			if (tagCompound.ContainsKey(name))
			{
				value = tagCompound.Get<T>(name)!;
				return true;
			}

			value = default;
			return false;
		}
	}
}