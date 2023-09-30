namespace AdvancedWorldGen.Base;

public class ZenithOption : Option
{
	public override void OnEnable()
	{
		base.OnEnable();
		foreach (Option option in OptionHelper.GetOptions("Celebrationmk10", "Drunk", "ForTheWorthy", "NoTraps",
			         "NotTheBees", "TheConstant", "Remix")) option.Enable();
	}
}