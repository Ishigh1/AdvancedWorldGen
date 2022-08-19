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

        OptionHelper.ClearAll();

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
		foreach ((_, Option? option) in OptionHelper.OptionDict)
			if (option.Children.Count == 0 && (!option.Hidden || showHidden))
				Console.WriteLine(id++ + " : " +
				                  Language.GetTextValue("Mods." + option.ModName + "." + option.SimplifiedName) +
				                  (option.Enabled
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

		foreach ((string? _, Option? option) in OptionHelper.OptionDict)
			if (option.Enabled && option.Children.Count == 0)
				foreach (string conflictName in option.Conflicts)
					if (string.Compare(option.SimplifiedName, conflictName, StringComparison.Ordinal) < 0 &&
					    OptionHelper.OptionsContains(conflictName))
					{
						Console.WriteLine(
							Language.GetTextValue("Mods.AdvancedWorldGen." + option.SimplifiedName + ".Conflicts." +
							                      option.SimplifiedName));
						conflict = true;
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
				HashSet<string> options = new(strings[1].Split("|"));
				if (options.Count == 0)
					errorMessage = Language.GetTextValue("Mods.AdvancedWorldGen.Conflict.InvalidImport");
				else
                    OptionHelper.Import(options);
			}
			else
			{
				errorMessage = Language.GetTextValue("Mods.AdvancedWorldGen.Conflict.InvalidInput");
			}
		}
	}

	public static bool ConvertIdToOption(bool showHidden, int id)
	{
		for (int i = 0; i < OptionHelper.OptionDict.Count; i++)
		{
			(string _, Option option) = OptionHelper.OptionDict.ElementAt(i);
			if ((!option.Hidden || showHidden) && option.Children.Count == 0 && --id == 0)
			{
				if (option.Enabled)
					option.Disable();
				else
					option.WeakEnable();

				return true;
			}
		}

		return false;
	}
}