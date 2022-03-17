using System;
using AdvancedWorldGen.CustomSized;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace AdvancedWorldGen.UI.InputUI;

public class NumberTextBox<T> : UIElement where T : IConvertible, IComparable
{
	public T Max;
	public T Min;
	public string Name;
	public Params Params;

	public NumberTextBox(Params @params, string name, T min, T max)
	{
		Params = @params;
		Min = min;
		Max = max;
		Name = name;

		CreateUIElement();
	}

	public T Value
	{
		get => (T)Params.Data[Name];
		set => Params.Data[Name] = value;
	}

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

		UIText title = new(Language.GetText("Mods.AdvancedWorldGen.UI.CustomSizes." + Name))
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