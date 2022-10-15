namespace AdvancedWorldGen.UI.InputUI.List;

public abstract class ExpandableList : InputBox<string>
{
	public bool AllowOther;
	public string[] PossibleValues;

	protected ExpandableList(string name, string? localizationPath, bool allowOther) : base(name, localizationPath)
	{
		AllowOther = allowOther;
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