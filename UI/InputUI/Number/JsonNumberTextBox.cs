namespace AdvancedWorldGen.UI.InputUI.Number;

public class JsonNumberTextBox<T> : NumberTextBox<T> where T : IConvertible, IComparable
{
	public JValue JValue;

	public JsonNumberTextBox(JValue jValue, T min, T max, string? localizationPath) : base(jValue.Path, min, max,
		localizationPath)
	{
		JValue = jValue;

		CreateUIElement();
	}

	public override T? Value
	{
		get => (T)JValue.Value;
		set => JValue.Value = value;
	}
}