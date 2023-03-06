namespace AdvancedWorldGen.Base;

public partial class OptionHelper
{
	public static Dictionary<string, Option> OptionDict = null!;
	public static WorldSettings WorldSettings;

	public static void InitializeDict(Mod mod)
	{
		WorldSettings = new WorldSettings();
		OptionDict = JsonConvert.DeserializeObject<Dictionary<string, Option>>(
			Encoding.UTF8.GetString(mod.GetFileBytes("Options.json")));

		for (int index = 0; index < OptionDict.Count; index++)
		{
			(_, Option? option) = OptionDict.ElementAt(index);
			if (option.Children.Count == 0)
				continue;
			Option baseOption = new()
			{
				Children = new List<Option>(),
				Conflicts = option.Conflicts,
				Name = "Base"
			};
			option.Conflicts = new List<string>();
			option.Children.Insert(0, baseOption);
			foreach (Option optionChild in option.Children)
			{
				optionChild.Parent = option;
				OptionDict.Add(optionChild.FullName, optionChild);
			}
		}

		#region handle zenith

		if (WorldgenSettings.Instance.ZenithEnabler)
		{
			if (!OptionDict.Remove("Zenith.Base", out Option? zenith))
				throw new Exception("Zenith option not found, who stole it ?");

			Option newZenith = new ZenithOption
			{
				Children = zenith.Children,
				Conflicts = zenith.Conflicts,
				Name = zenith.Name,
				Parent = zenith.Parent
			};
			OptionDict.Add("Zenith.Base", newZenith);
			zenith.Parent!.Children.Remove(zenith);
			zenith.Parent!.Children.Add(newZenith);
		}

		#endregion

		foreach ((_, Option? option) in OptionDict)
			for (int index = 0; index < option.Conflicts.Count; index++)
			{
				string optionConflict = option.Conflicts[index];
				Option conflict = OptionDict[optionConflict];
				if (conflict.Children.Count != 0)
					option.Conflicts[index] = conflict.Children[0].Name;
			}
	}
}