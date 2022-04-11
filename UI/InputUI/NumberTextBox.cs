using System;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace AdvancedWorldGen.UI.InputUI;

public abstract class NumberTextBox<T> : OrderedUIItem where T : IConvertible, IComparable
{
	public string Name;
	
	public T Min;
	public T Max;

	protected NumberTextBox(string name, T min, T max)
	{
		Name = name;
		Min = min;
		Max = max;
	}

	public abstract T Value { get; set; }

	public void CreateUIElement()
	{
		Height = new StyleDimension
		{
			Pixels = 40
		};
		Width = new StyleDimension
		{
			Percent = 1f
		};

		UIPanel background = new()
		{
			Height =
			{
				Percent = 1f
			},
			Width =
			{
				Percent = 1f
			}
		};
		Append(background);

		UIText title = new(Language.GetText(Name))
		{
			VAlign = 0.5f
		};
		background.Append(title);

		EditableText<T> editableText = new(this)
		{
			VAlign = 0.5f,
			HAlign = 1f
		};
		background.Append(editableText);
	}
}