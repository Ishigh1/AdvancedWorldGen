namespace AdvancedWorldGen.UI.InputUI;

public abstract class InputBox<T> : OrderedUIItem
{
	public UIPanel Background = null!;
	public string Name;
	private string? LocalizationPath;

	protected InputBox(string name, string? localizationPath)
	{
		Name = name;
		LocalizationPath = localizationPath;
	}

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
			localization = localization.Replace(' ', '_');
			if (!Language.Exists(localization)) localization = Name;
		}

		UIText title = new(Language.GetText(localization))
		{
			VAlign = 0.5f
		};
		Background.Append(title);
	}
}