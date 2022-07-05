using Microsoft.Xna.Framework;

namespace AdvancedWorldGen.BetterVanillaWorldGen.MicroBiomesStuff;

public class RTree
{
	private readonly int Sum;
	private RTree? LeftChild;
	private Rectangle Rectangle;
	private RTree? RightChild;

	public RTree(Rectangle rectangle)
	{
		Rectangle = rectangle;
		Point center = rectangle.Center;
		Sum = center.X + center.Y;
	}

	public bool Contains(Point point)
	{
		return Contains(point.X, point.Y);
	}

	public bool Contains(int x, int y)
	{
		return Contains(x, y, x + y);
	}

	private bool Contains(int x, int y, int sum)
	{
		if (Rectangle.Contains(x, y))
			return true;

		if (sum <= Sum)
			return LeftChild?.Contains(x, y, sum) == true;
		else
			return RightChild?.Contains(x, y, sum) == true;
	}

	public void Insert(Rectangle rectangle)
	{
		Insert(new RTree(rectangle));
	}

	private void Insert(RTree rTree)
	{
		if (rTree.Sum <= Sum)
		{
			if (LeftChild == null)
				LeftChild = rTree;
			else
				LeftChild.Insert(rTree);
		}
		else
		{
			if (RightChild == null)
				RightChild = rTree;
			else
				RightChild.Insert(rTree);
		}
	}
}