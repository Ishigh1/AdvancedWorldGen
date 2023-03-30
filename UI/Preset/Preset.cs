namespace AdvancedWorldGen.UI.Preset;

public class Preset : UITextPanel<string>
{
	private string Name;
	private string Content;
	private PresetUI PresetUI;

	public Preset(string key, string value, PresetUI parent) : base(key)
	{
		Name = key;
		Content = value;
		PresetUI = parent;
	}

	public override void OnInitialize()
	{
		HAlign = 0.5f;
		Width = new StyleDimension(0f, 1f);
		Height = new StyleDimension(40f, 0f);

		TextHAlign = 0f;
		OnClick += (_, _) =>
		{
			SoundEngine.PlaySound(SoundID.MenuOpen);
			OptionsParser.Parse(Content);
		};
		OnMouseOver += UiChanger.FadedMouseOver;
		OnMouseOut += UiChanger.FadedMouseOut;

		UITextPanel<string> deletePresetBox = new("X", 0.5f)
		{
			Width = new StyleDimension(24f, 0f),
			Height = new StyleDimension(16f, 0f),
			PaddingTop = 5f,
			PaddingBottom = 5f,
			PaddingLeft = 5f,
			PaddingRight = 5f,
			HAlign = 1f
		};
		deletePresetBox.OnClick += (_, _) =>
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			PresetUI.Delete(Name);
		};
		deletePresetBox.OnMouseOver += UiChanger.FadedMouseOver;
		deletePresetBox.OnMouseOut += UiChanger.FadedMouseOut;
		Append(deletePresetBox);
	}
}