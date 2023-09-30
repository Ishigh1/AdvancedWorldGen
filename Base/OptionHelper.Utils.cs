namespace AdvancedWorldGen.Base;

public partial class OptionHelper
{
	public static void ClearAll()
	{
		foreach ((string? _, Option? option) in OptionDict) option.Disable();
	}

	public static void Import(IEnumerable<string> optionNames)
	{
		ClearAll();
		foreach (string optionName in optionNames)
			if (OptionDict.TryGetValue(optionName, out Option? option))
				option.OnEnable();
	}

	public static List<string> Export()
	{
		List<string> list = new();
		foreach ((string? _, Option? option) in OptionDict)
			if (option.Enabled is true && option.Children.Count == 0)
				list.Add(option.FullName);
		return list;
	}

	public static bool OptionsContains(string optionName)
	{
		if (!OptionDict.TryGetValue(optionName, out Option? option))
			return false;
		return option.Children.Count == 0 ? option.Enabled is true : option.Children[0].Enabled is true;
	}

	public static IEnumerable<Option> GetOptions(params string[] names)
	{
		foreach (string name in names)
			if (OptionDict.TryGetValue(name, out Option? option))
				yield return option;
			else
				throw new ArgumentException($"{name} not found !");
	}
}