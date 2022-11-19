namespace AdvancedWorldGen.Helper;

public class RTree
{
	private bool Forbidden;
	private RTree LeftChild = null!;
	private RTree? MiddleChild;
	private Rectangle Rectangle;
	private RTree RightChild = null!;
	private bool Vertical;

	private RTree(Rectangle rectangle)
	{
		Rectangle = rectangle;
	}

	public static RTree Root()
	{
		return new RTree(new Rectangle(0, 0, Main.maxTilesX, Main.maxTilesY));
	}

	public bool Contains(Point point)
	{
		return Contains(point.X, point.Y);
	}

	public bool Contains(int x, int y)
	{
		if (MiddleChild == null)
			return Forbidden;
		if (Vertical)
		{
			if (x < MiddleChild.Rectangle.Left)
				return LeftChild.Contains(x, y);
			if (x > MiddleChild.Rectangle.Right)
				return RightChild.Contains(x, y);
			return MiddleChild.Contains(x, y);
		}

		if (y < MiddleChild.Rectangle.Top)
			return LeftChild.Contains(x, y);
		if (y > MiddleChild.Rectangle.Bottom)
			return RightChild.Contains(x, y);
		return MiddleChild.Contains(x, y);
	}

	public void Insert(Rectangle rectangle)
	{
		if (MiddleChild == null)
		{
			if (rectangle.Left > Rectangle.Left || rectangle.Right < Rectangle.Right)
			{
				Vertical = true;
				int left = Math.Max(Rectangle.Left, rectangle.Left);
				int right = Math.Min(Rectangle.Right, rectangle.Right);
				LeftChild = new RTree(new Rectangle(Rectangle.X, Rectangle.Y, left - Rectangle.Left, Rectangle.Height));
				MiddleChild = new RTree(new Rectangle(left, Rectangle.Y, right - left, Rectangle.Height));
				RightChild = new RTree(new Rectangle(right, Rectangle.Y, Rectangle.Right - right, Rectangle.Height));
				MiddleChild.Insert(rectangle);
			}
			else if (rectangle.Top > Rectangle.Top || rectangle.Bottom < Rectangle.Bottom)
			{
				Vertical = false;
				int top = Math.Max(Rectangle.Top, rectangle.Top);
				int bottom = Math.Min(Rectangle.Bottom, rectangle.Bottom);
				LeftChild = new RTree(new Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Width, top - Rectangle.Top));
				MiddleChild = new RTree(new Rectangle(Rectangle.X, top, Rectangle.Width, bottom - top));
				RightChild = new RTree(new Rectangle(Rectangle.X, bottom, Rectangle.Width, bottom - Rectangle.Bottom));
				MiddleChild.Insert(rectangle);
			}
			else
			{
				Forbidden = true;
			}
		}
		else if (Vertical)
		{
			if (rectangle.Left < MiddleChild.Rectangle.Left)
				LeftChild.Insert(rectangle);
			if (rectangle.Right > MiddleChild.Rectangle.Right)
				RightChild.Insert(rectangle);
			if (rectangle.Right > MiddleChild.Rectangle.Left && rectangle.Left < MiddleChild.Rectangle.Right)
				MiddleChild.Insert(rectangle);
		}
		else
		{
			if (rectangle.Top < MiddleChild.Rectangle.Top)
				LeftChild.Insert(rectangle);
			if (rectangle.Bottom > MiddleChild.Rectangle.Bottom)
				RightChild.Insert(rectangle);
			if (rectangle.Bottom > MiddleChild.Rectangle.Top && rectangle.Top < MiddleChild.Rectangle.Bottom)
				MiddleChild.Insert(rectangle);
		}
	}
}