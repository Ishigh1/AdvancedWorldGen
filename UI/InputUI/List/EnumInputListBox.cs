namespace AdvancedWorldGen.UI.InputUI.List;

public class EnumInputListBox<T> : InputBox<string> where T : struct, Enum
{
	private readonly JValue? JValue;

	public EnumInputListBox(JValue jValue, string? localizationPath) : base(jValue.Path, localizationPath)
	{
		JValue = jValue;

		CreateUIElement();
	}

	public EnumInputListBox(string name, string? localizationPath) : base(name, localizationPath)
	{
		CreateUIElement();
	}

	public override string? Value
	{
		get
		{
			if (JValue != null)
				return (string?)JValue.Value;
			return Enum.GetName((T)Params.Get(Name));
		}
		set
		{
			if (JValue != null)
				JValue.Value = value;
			else
				Params.Set(Name, Enum.Parse<T>(value!));
		}
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