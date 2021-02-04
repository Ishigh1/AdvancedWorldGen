using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using On.Terraria.IO;
using Terraria.ModLoader;

namespace AdvancedSeedGen
{
	public class AdvancedSeedGen : Mod
	{
		public static List<string> Options;
		public static Dictionary<string, List<string>> SeedTranslator;

		public override void Load()
		{
			SeedTranslator = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
				Encoding.UTF8.GetString(GetFileBytes("SeedTranslation.json")));
			WorldFileData.SetSeed += WorldFileDataOnSetSeed;
		}

		public override void Unload()
		{
			WorldFileData.SetSeed -= WorldFileDataOnSetSeed;
		}

		private static void WorldFileDataOnSetSeed(WorldFileData.orig_SetSeed orig, Terraria.IO.WorldFileData self,
			string seedtext)
		{
			string seedText = SeedHelper.TweakSeed(seedtext);
			orig(self, seedText);
		}
	}
}