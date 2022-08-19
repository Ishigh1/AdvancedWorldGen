namespace AdvancedWorldGen.UI.InputUI.Number;

public class ConfigNumberTextBox<T> : NumberTextBox<T> where T : IConvertible, IComparable
{
	public Dictionary<string, object>? Data;
	public Params? Params;

	public ConfigNumberTextBox(Dictionary<string, object> data, string name, T min, T max, string? localizationPath = null) : base(name, min, max)
	{
		Data = data;
		LocalizationPath = localizationPath;

		CreateUIElement();
	}

	public ConfigNumberTextBox(Params @params, string name, T min, T max, string? localizationPath = null) : base(name, min, max)
	{
		Params = @params;
		LocalizationPath = localizationPath;

		CreateUIElement();
	}

	public override T? Value
	{
		get
		{
			if (Params != null)
				return (T?)Params[Name];
			else
				return (T?)Data![Name];
		}
		set
		{
			if (Params != null)
				Params[Name] = value!;
			else
				Data![Name] = value!;
		}
	}
}