namespace AdvancedWorldGen.Base;

public class ZenithOption : Option
{
	public override void WeakEnable()
	{
		base.WeakEnable();
		foreach (Option option in OptionHelper.GetOptions("Celebrationmk10", "Drunk", "ForTheWorthy", "NoTraps", "NotTheBees", "TheConstant", "Remix"))
		{
			option.Enable();
		}
	}
}