namespace AdvancedWorldGen.UI.InputUI.List;

public class EnumInputListBox<T> : InputBox<string> where T : struct, Enum
{
	private JValue? JValue;
	private Params? Params;

	public EnumInputListBox(JValue jValue, string? localizationPath) : base(jValue.Path, localizationPath)
	{
		JValue = jValue;

		CreateUIElement();
	}
	
	public EnumInputListBox(Params @params, string name, string? localizationPath) : base(name, localizationPath)
	{
		Params = @params;
		
		CreateUIElement();
	}

	public override string? Value
	{
		get
		{
			if (JValue != null)
				return (string?)JValue.Value;
			else
				return Enum.GetName((T) Params![Name]);
		}
		set
		{
			if (JValue != null)
				JValue.Value = value;
			else
				Params![Name] = Enum.Parse<T>(value!);
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