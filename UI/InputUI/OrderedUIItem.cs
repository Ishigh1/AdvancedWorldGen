namespace AdvancedWorldGen.UI.InputUI;

public class OrderedUIItem : UIElement
{
	public int Order;

	public override int CompareTo(object obj)
	{
		if (obj is OrderedUIItem other) return Order - other.Order;
		return 0;
	}
}