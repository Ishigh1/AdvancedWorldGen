namespace AdvancedWorldGen.UI.InputUI.Number;

public abstract class NumberTextBox<T> : InputBox<T> where T : IConvertible, IComparable
{
	public T Max;
	public T Min;

	protected NumberTextBox(string name, T min, T max, string? localizationPath) : base(name, localizationPath)
	{
		Min = min;
		Max = max;
	}

	public override void CreateUIElement()
	{
		base.CreateUIElement();

		EditableText<T> editableText = new(this)
		{
			VAlign = 0.5f,
			HAlign = 1f
		};
		Background.Append(editableText);
	}
}