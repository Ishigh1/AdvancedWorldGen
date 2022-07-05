using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace AdvancedWorldGen.UI.InputUI;

public abstract class InputBox<T> : OrderedUIItem
{
	public UIPanel Background = null!;
	public string Name;

	protected InputBox(string name)
	{
		Name = name;
	}

	public string? LocalizationPath { get; set; }

	public abstract T? Value { get; set; }

	public virtual void CreateUIElement()
	{
		Height = new StyleDimension
		{
			Pixels = 40
		};
		Width = new StyleDimension
		{
			Percent = 1f
		};

		Background = new UIPanel
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
		Append(Background);

		string localization = Name;
		if (LocalizationPath is not null)
		{
			localization = $"{LocalizationPath}.{Name}";
			if (!Language.Exists(localization)) localization = Name;
		}

		UIText title = new(Language.GetText(localization))
		{
			VAlign = 0.5f
		};
		Background.Append(title);
	}
}