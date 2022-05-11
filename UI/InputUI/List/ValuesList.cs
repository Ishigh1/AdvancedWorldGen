using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AdvancedWorldGen.UI.InputUI.List;

public class ValuesList : FocusElement
{
	public static bool SkipFrame;
	public Array EnumValues;

	public InputBox<string> ParentBox;
	public List<UIElement> ShownOptions;

	public ValuesList(InputBox<string> parent, Array enumValues)
	{
		ParentBox = parent;
		EnumValues = enumValues;
		ShownOptions = new List<UIElement>();
	}

	public override string DisplayText => ParentBox.Value;

	public override void Focus()
	{
		base.Focus();

		Main.OnPostDraw += ExpandList;
	}

	public override void Unfocus()
	{
		base.Unfocus();

		Main.OnPostDraw += CollapseList;
	}

	public void ExpandList(GameTime gameTime)
	{
		UIList? parentList = null;
		UIPanel parentPanel = null!;
		UIElement? currentElement = this;
		while (currentElement != null)
		{
			currentElement = currentElement.Parent;
			switch (currentElement)
			{
				case UIList list:
					parentList = list;
					break;
				case UIPanel panel:
					parentPanel = panel;
					break;
			}
		}
		
		CalculatedStyle windowDimension = parentPanel.GetDimensions();
		float top = ParentBox.Top.Pixels + ParentBox.Height.Pixels - (parentList?.ViewPosition ?? 0f) +
		            (parentList?.Top.Pixels ?? 0f) + windowDimension.Y + 15;

		foreach (object enumValue in EnumValues)
		{
			string enumString = enumValue.ToString()!;
			UITextPanel<string> option = new(enumString, 0.75f)
			{
				Left = new StyleDimension(windowDimension.X + windowDimension.Width * 0.3f, 0f),
				Top = new StyleDimension(top, 0f),
				Width = new StyleDimension(windowDimension.Width * 0.7f, 0f)
			};
			top += 44;
			option.OnMouseDown += (_, _) =>
			{
				ParentBox.Value = enumString;
				Unfocus();
			};
			option.OnMouseOver += UiChanger.FadedMouseOver;
			option.OnMouseOut += UiChanger.FadedMouseOut;
			ShownOptions.Add(option);
			parentPanel.Parent.Append(option);
		}

		Main.OnPostDraw -= ExpandList;
	}

	public void CollapseList(GameTime gameTime)
	{
		SkipFrame = !SkipFrame;
		if (!SkipFrame)
		{
			foreach (UIElement option in ShownOptions) option.Remove();
			ShownOptions.Clear();

			Main.OnPostDraw -= CollapseList;
		}
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		base.DrawSelf(spriteBatch);

		if (IsTheCurrentFocus)
			DrawText(spriteBatch, DisplayText);
	}
}