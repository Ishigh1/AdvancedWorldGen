namespace AdvancedWorldGen.UI.InputUI.List;

public class EnumInputListBox<T> : InputBox<string>
{
	public JValue JValue;

	public EnumInputListBox(JValue jValue) : base(jValue.Path)
	{
		JValue = jValue;

		CreateUIElement();
	}

	public override string Value
	{
		get => (string)JValue.Value;
		set => JValue.Value = value;
	}

	public override void CreateUIElement()
	{
		base.CreateUIElement();

		Array enumValues = Enum.GetValues(typeof(T));
		ValuesList valuesList = new(this, enumValues)
		{
			VAlign = 0.5f,
			HAlign = 1f
		};
		Background.Append(valuesList);
	}
}