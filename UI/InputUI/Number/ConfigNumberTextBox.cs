namespace AdvancedWorldGen.UI.InputUI.Number;

public class ConfigNumberTextBox<T> : NumberTextBox<T> where T : IConvertible, IComparable
{
	public Dictionary<string, object>? Data;

	public ConfigNumberTextBox(Dictionary<string, object> data, string name, T min, T max, string? localizationPath = null) : base(name, min, max, localizationPath)
	{
		Data = data;

		CreateUIElement();
	}

	public ConfigNumberTextBox(string name, T min, T max, string? localizationPath = null) : base(name, min, max, localizationPath)
	{
		CreateUIElement();
	}

	public override T? Value
	{
		get
		{
			if (Data != null)
				return (T?)Data![Name];
			else
				return (T?)Params.Get(Name);
		}
		set
		{
			if (Data != null)
				Data![Name] = value!;
			else
				Params.Set(Name, value!);
		}
	}
}