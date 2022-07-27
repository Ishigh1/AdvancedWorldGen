namespace AdvancedWorldGen.SpecialOptions;

public class SpecialCase
{
	public delegate bool ConditionDelegate(int x, int y, Tile tile);

	public ConditionDelegate? Condition;
	public int Type;

	public SpecialCase(int type, ConditionDelegate? condition = null)
	{
		Type = type;
		Condition = condition;
	}

	public bool IsValid(int x, int y, Tile tile)
	{
		return Condition == null || Condition(x, y, tile);
	}
}