namespace AdvancedWorldGen.UI.InputUI.List;

public class TileExpandableList : InputBox<string>
{
	public const int Random = -1;
	public bool AllowOther;
	public Params Params;
	public string[] PossibleValues;

	public TileExpandableList(Params @params, string name, bool allowOther, params int[] possibleValues) : base(name)
	{
		AllowOther = allowOther;
		Params = @params;

		PossibleValues = new string[possibleValues.Length];
		for (int index = 0; index < possibleValues.Length; index++)
		{
			int possibleValue = possibleValues[index];
			if (possibleValue == Random)
				PossibleValues[index] = nameof(Random);
			else
				PossibleValues[index] = TileID.Search.GetName(possibleValue);
		}

		CreateUIElement();
	}

	public override string Value
	{
		get
		{
			int value = (int)Params.Data[Name];
			return value == Random ? nameof(Random) : TileID.Search.GetName(value);
		}
		set
		{
			if (value == nameof(Random))
				Params.Data[Name] = Random;
			else
				Params.Data[Name] = TileID.Search.GetId(value);
		}
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