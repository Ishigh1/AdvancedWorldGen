namespace AdvancedWorldGen.UI.InputUI.List;

public class BooleanExpandableList : ExpandableList
{
    public BooleanExpandableList(string name, string? localizationPath) : base(name, localizationPath, false)
    {
        PossibleValues = new[]
        {
            bool.TrueString, bool.FalseString
        };

        CreateUIElement();
    }

    public override string Value
    {
        get => Params.Get(Name).ToString()!;
        set => Params.Set(Name, bool.Parse(value));
    }
}