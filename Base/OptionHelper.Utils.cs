namespace AdvancedWorldGen.Base;

public partial class OptionHelper
{
	public static void ClearAll()
	{
		foreach ((string? _, Option? option) in OptionDict) option.Disable();
	}

	public static void Import(ICollection<string> optionNames)
	{
		ClearAll();
		Legacy.ReplaceOldOptions(optionNames);
		foreach (string optionName in optionNames)
			if (OptionDict.TryGetValue(optionName, out Option? option))
				option.WeakEnable();
	}

	public static List<string> Export()
	{
		List<string> list = new();
		foreach ((string? _, Option? option) in OptionDict)
			if (option.Enabled && option.Children.Count == 0)
				list.Add(option.FullName);
		return list;
	}

	public static bool OptionsContains(string optionName)
	{
		if (!OptionDict.TryGetValue(optionName, out Option? option))
			return false;
		return option.Children.Count == 0 ? option.Enabled : option.Children[0].Enabled;
	}

	public static IEnumerable<Option> GetOptions(params string[] names)
	{
		foreach (string name in names)
		{
			if (OptionDict.TryGetValue(name, out Option? option))
				yield return option;
			else
				throw new ArgumentException($"{name} not found !");
		}
	}
}