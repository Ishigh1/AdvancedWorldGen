namespace AdvancedWorldGen.UI.InputUI.List;

public abstract class ExpandableList : InputBox<string>
{
	public bool AllowOther;
	public Params Data;
	public string[] PossibleValues;

	protected ExpandableList(Params data, string name, bool allowOther) : base(name)
	{
		AllowOther = allowOther;
		Data = data;
	}

	public override void CreateUIElement()
	{
		base.CreateUIElement();

		ValuesList valuesList = new(this, PossibleValues)
		{
			VAlign = 0.5f,
			HAlign = 1f
		};
		Background.Append(valuesList);
	}
}