using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedWorldGen.Base;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using OnWorldFile = On.Terraria.IO.WorldFile;

namespace AdvancedWorldGen.UI;

public static class DedServUi
{
	public static WorldFileData DedServOptions(OnWorldFile.orig_CreateMetadata orig, string name, bool cloudSave,
		int gamemode)
	{
		if (!Main.dedServ)
			return orig(name, cloudSave, gamemode);

		bool finished = false;
		bool showHidden = false;
		string errorMessage = "";

		ModifiedWorld.Instance.OptionHelper.Options.Clear();

		while (!finished)
		{
			DedServPrint(showHidden, ref errorMessage);

			string s = Console.ReadLine()!;
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

		return orig(name, cloudSave, gamemode);
	}

	public static void DedServPrint(bool showHidden, ref string errorMessage)
	{
		Console.WriteLine(Language.GetTextValue("Mods.AdvancedWorldGen.NoneSelected.Description"));
		int id = 1;
		bool hidden = showHidden;
		foreach ((string key, _) in from keyValuePair in Option.OptionDict
		         where !keyValuePair.Value.Hidden || hidden
		         select keyValuePair)
			Console.WriteLine(id++ + " : " + Language.GetTextValue("Mods.AdvancedWorldGen." + key) +
			                  (ModifiedWorld.Instance.OptionHelper.OptionsContains(key)
				                  ? Language.GetTextValue("Mods.AdvancedWorldGen.DedServ.Selected")
				                  : ""));

		if (!showHidden) Console.WriteLine(Language.GetTextValue("Mods.AdvancedWorldGen.DedServ.ShowHidden"));

		Console.WriteLine(Language.GetTextValue("Mods.AdvancedWorldGen.DedServ.Import"));
		Console.WriteLine(Language.GetTextValue("Mods.AdvancedWorldGen.DedServ.Validate"));

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
		List<string> options = ModifiedWorld.Instance.OptionHelper.Options.ToList();
		for (int i = 0; i < options.Count; i++)
		{
			string option = options[i];
			for (int j = i; j < options.Count; j++)
			{
				string option2 = options[j];
				if (Option.OptionDict[option].Conflicts.Contains(option2))
				{
					Console.WriteLine(
						Language.GetTextValue("Mods.AdvancedWorldGen.Conflict." + options[i] + "." + options[j]));
					conflict = true;
				}
			}
		}

		if (conflict) Console.WriteLine();
	}

	public static void HandleDedServId(string s, ref string errorMessage, bool showHidden)
	{
		if (int.TryParse(s, out int id) && (id <= 0 || !ConvertIdToOption(showHidden, id)))
		{
			errorMessage = Language.GetTextValue("Mods.AdvancedWorldGen.Conflict.InvalidId");
		}
		else
		{
			string[] strings = s.Split(' ');
			if (strings.Length == 2 && strings[0] == "i")
			{
				HashSet<string> options = OptionsSelector.TextToOptions(strings[1]);
				if (options.Count == 0)
					errorMessage = Language.GetTextValue("Mods.AdvancedWorldGen.Conflict.InvalidImport");
				else
					ModifiedWorld.Instance.OptionHelper.Options = options;
			}
			else
			{
				errorMessage = Language.GetTextValue("Mods.AdvancedWorldGen.Conflict.InvalidInput");
			}
		}
	}

	public static bool ConvertIdToOption(bool showHidden, int id)
	{
		for (int i = 0; i < Option.OptionDict.Count; i++)
		{
			(string key, Option option) = Option.OptionDict.ElementAt(i);
			if ((!option.Hidden || showHidden) && --id == 0)
			{
				if (ModifiedWorld.Instance.OptionHelper.OptionsContains(key))
					ModifiedWorld.Instance.OptionHelper.Options.Remove(key);
				else
					ModifiedWorld.Instance.OptionHelper.Options.Add(key);

				return true;
			}
		}

		return false;
	}
}