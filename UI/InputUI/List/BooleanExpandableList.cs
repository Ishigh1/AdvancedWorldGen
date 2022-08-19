namespace AdvancedWorldGen.UI.InputUI.List;

public class BooleanExpandableList : ExpandableList
{
	public BooleanExpandableList(Params data, string name) : base(data, name, false)
	{
		PossibleValues = new[]
		{
			bool.TrueString, bool.FalseString
		};

		CreateUIElement();
	}

	public override string Value
	{
		get => Data[Name].ToString()!;
		set => Data[Name] = bool.Parse(value);
	}
}