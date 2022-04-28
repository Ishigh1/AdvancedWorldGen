using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace AdvancedWorldGen.UI.InputUI;

public abstract class FocusElement : UIElement
{
	public static FocusElement? CurrentFocus;
	public bool IsTheCurrentFocus => CurrentFocus == this;

	public abstract string DisplayText { get; }

	public virtual void Focus(SpriteBatch spriteBatch)
	{
		CurrentFocus?.Unfocus();
		CurrentFocus = this;
	}

	public virtual void Unfocus()
	{
		CurrentFocus = null;
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		if (Main.mouseLeft && Main.mouseLeftRelease)
			if (Parent.IsMouseHovering)
				Focus(spriteBatch);
			else if (IsTheCurrentFocus)
				Unfocus();

		if (!IsTheCurrentFocus) DrawText(spriteBatch, DisplayText);
	}

	public void DrawText(SpriteBatch spriteBatch, string text)
	{
		CalculatedStyle space = GetDimensions();
		Vector2 size =
			Utils.DrawBorderString(spriteBatch, text, new Vector2(space.X, space.Y), Color.White);
		Width.Pixels = size.X;
		Height.Pixels = size.Y;
	}
}