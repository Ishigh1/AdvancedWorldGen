using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using WorldFile = On.Terraria.IO.WorldFile;

namespace AdvancedWorldGen.OptionUI
{
	public class DedServUi
	{
		public static WorldFileData DedServOptions(WorldFile.orig_CreateMetadata orig, string name, bool cloudSave,
			int gamemode)
		{
			if (Main.dedServ)
			{
				bool finished = false;
				bool showHidden = false;
				string errorMessage = "";

				ModifiedWorld.OptionHelper.Options.Clear();

				while (!finished)
				{
					DedServPrint(showHidden, ref errorMessage);

					string s = Console.ReadLine();
					switch (s)
					{
						case "h":
							showHidden = !showHidden;
							break;
						case "y":
						case "":
							finished = true;
							break;
						default:
							HandleDedServId(s, ref errorMessage, showHidden);
							break;
					}

					Console.Clear();
				}
			}

			return orig(name, cloudSave, gamemode);
		}

		public static void DedServPrint(bool showHidden, ref string errorMessage)
		{
			Console.WriteLine(Language.GetTextValue("Mods.AdvancedWorldGen.NoneSelected.description"));
			int id = 1;
			bool hidden = showHidden;
			foreach (KeyValuePair<string, Option> keyValuePair in from keyValuePair in OptionsSelector.OptionDict
				where !keyValuePair.Value.Hidden || hidden
				select keyValuePair)
				Console.WriteLine(id++ + " : " + Language.GetTextValue("Mods.AdvancedWorldGen." + keyValuePair.Key) +
				                  (ModifiedWorld.OptionHelper.OptionsContains(keyValuePair.Key) ? "(chosen)" : ""));

			if (!showHidden) Console.WriteLine("h : Show hidden options");

			Console.WriteLine("i <options> : Import options\n");
			Console.WriteLine("y : Validate\n");

			if (errorMessage != "")
			{
				Console.WriteLine(errorMessage);
				errorMessage = "";
			}
			else
			{
				PrintConflicts();
			}
		}

		public static void PrintConflicts()
		{
			bool conflict = false;
			List<string> options = ModifiedWorld.OptionHelper.Options.ToList();
			for (int i = 0; i < options.Count; i++)
			{
				string option = options[i];
				for (int j = i; j < options.Count; j++)
				{
					string option2 = options[j];
					if (OptionsSelector.OptionDict[option].Conflicts?.Contains(option2) == true)
					{
						Console.WriteLine(Language.GetTextValue("Mods.AdvancedWorldGen.conflict." +
						                                        options[i] +
						                                        "." + options[j]));
						conflict = true;
					}
				}
			}

			if (conflict) Console.WriteLine();
		}

		public static void HandleDedServId(string s, ref string errorMessage, bool showHidden)
		{
			if (!int.TryParse(s, out int id) || id <= 0 ||
			    !ConvertIdToOption(showHidden, ref id))
			{
				errorMessage = "Input not recognized";
			}
			else
			{
				string[] strings = s.Split(' ');
				if (strings.Length == 2 && strings[0] == "i")
				{
					HashSet<string> options = OptionsSelector.TextToOptions(strings[1]);
					if (options == null)
						errorMessage = "Input not recognized";
					else
						ModifiedWorld.OptionHelper.Options = options;
				}
			}
		}

		public static bool ConvertIdToOption(bool showHidden, ref int id)
		{
			for (int i = 0; i < OptionsSelector.OptionDict.Count; i++)
			{
				KeyValuePair<string, Option> pair = OptionsSelector.OptionDict.ElementAt(i);
				if ((!pair.Value.Hidden || showHidden) && --id == 0)
				{
					if (ModifiedWorld.OptionHelper.OptionsContains(pair.Key))
						ModifiedWorld.OptionHelper.Options.Remove(pair.Key);
					else
						ModifiedWorld.OptionHelper.Options.Add(pair.Key);

					return true;
				}
			}

			return false;
		}
	}
}