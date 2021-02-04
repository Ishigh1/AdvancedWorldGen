using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Utilities;

namespace AdvancedSeedGen
{
	public class SeedHelper
	{
		public static List<string> Options;

		public static void SetSeedOptions(List<string> newOptions)
		{
			Options = newOptions;

			WorldGen.notTheBees = OptionsContains("Not The Bees", "Small Not The Bees");

			WorldGen.getGoodWorldGen = OptionsContains("For The Worthy");
		}

		public static bool OptionsContains(params string[] value)
		{
			return Options != null && value.Any(s => Options.Contains(s));
		}

		public static void ExtractOptions(string seed)
		{
			if (seed == null) return;
			List<string> seedOptions = new List<string>();
			string[] strings = seed.Split(':');

			strings = strings[0].Split(',');


			foreach (string s in strings)
				if (AdvancedSeedGen.SeedTranslator.TryGetValue(s.ToLower(), out List<string> collection))
					seedOptions.AddRange(collection);

			SetSeedOptions(seedOptions);
		}

		public static string TweakSeed(string seedText)
		{
			ExtractOptions(seedText);
			string[] strings = seedText.Split(':');
			if (Options.Count != 0)
				if (strings.Length != 2)
				{
					UnifiedRandom rand = new UnifiedRandom();
					int seed = rand.Next(999999999);
					return seedText + ":" + seed;
				}

			return seedText;
		}
	}
}