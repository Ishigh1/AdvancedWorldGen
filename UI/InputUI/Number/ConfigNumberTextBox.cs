namespace AdvancedWorldGen.UI.InputUI.Number;

public class ConfigNumberTextBox<T> : NumberTextBox<T> where T : IConvertible, IComparable
{
	public Dictionary<string, object> Data;

	public ConfigNumberTextBox(Dictionary<string, object> data, string name, T min, T max, string? localizationPath = null) : base(name, min, max)
	{
		Data = data;
		LocalizationPath = localizationPath;

		CreateUIElement();
	}

	public override T? Value
	{
		get => (T)Data[Name];
		set => Data[Name] = value!;
	}
}