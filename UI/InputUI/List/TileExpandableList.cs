namespace AdvancedWorldGen.UI.InputUI.List;

public class TileExpandableList : ExpandableList
{
	public const int Random = -1;

	public TileExpandableList(Params data, string name, string? localizationPath, bool allowOther, params int[] possibleValues) : base(data, name, localizationPath, allowOther)
	{
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
			int value = (int)Data[Name];
			return value == Random ? nameof(Random) : TileID.Search.GetName(value);
		}
		set
		{
			if (value == nameof(Random))
				Data[Name] = Random;
			else
				Data[Name] = TileID.Search.GetId(value);
		}
	}
}