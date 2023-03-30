namespace AdvancedWorldGen.UI.Preset;

public class PresetUI : UIState
{
	private static string DataPath => Path.Combine(AdvancedWorldGenMod.FolderPath, "Presets.json");
	private OptionsSelector PreviousState;

	public PresetUI(OptionsSelector previousState)
	{
		PreviousState = previousState;
	}

	public override void OnInitialize()
	{
		UIPanel uiPanel = new()
		{
			HAlign = 0.5f,
			VAlign = 0.5f,
			Width = new StyleDimension(0, 0.5f),
			Height = new StyleDimension(0, 0.5f),
			BackgroundColor = UICommon.MainPanelBackground
		};
		Append(uiPanel);
		
		UIText uiTitle = new("Presets", 0.75f, true) { HAlign = 0.5f };
		uiTitle.Height = uiTitle.MinHeight;
		uiPanel.Append(uiTitle);
		uiPanel.Append(new UIHorizontalSeparator
		{
			Width = new StyleDimension(0f, 1f),
			Top = new StyleDimension(43f, 0f),
			Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
		});

		UIScrollbar uiScrollbar = new()
		{
			Height = new StyleDimension(-110f, 1f),
			Top = new StyleDimension(50, 0f),
			HAlign = 1f
		};
		UIList uiList = new()
		{
			Height = new StyleDimension(-110f, 1f),
			Width = new StyleDimension(-20f, 1f),
			Top = new StyleDimension(50, 0f)
		};
		uiList.SetScrollbar(uiScrollbar);

		LoadPresets(uiList);

		uiPanel.Append(uiScrollbar);
		uiPanel.Append(uiList);
		uiPanel.Recalculate();

		UITextPanel<string> goBack = new(Language.GetTextValue("UI.Back"))
		{
			Width = new StyleDimension(0f, 0.1f),
			Top = new StyleDimension(0f, 0.8f),
			HAlign = 0.4f
		};
		goBack.OnMouseDown += GoBack;
		goBack.OnMouseOver += UiChanger.FadedMouseOver;
		goBack.OnMouseOut += UiChanger.FadedMouseOut;
		Append(goBack);
	}

	private void LoadPresets(UIList uiList)
	{
		if (File.Exists(DataPath))
		{
			TagCompound tagCompound = TagIO.FromFile(DataPath);
			foreach ((string key, object value) in tagCompound)
			{
				uiList.Add(new Preset(key, (string)value, this));
			}
		}

		UITextPanel<string> newPresetButton = new(Language.GetTextValue("Add Preset"))
		{
			HAlign = 0.5f,
			Width = new StyleDimension(0f, 1f),
			Height = new StyleDimension(40f, 0f)
		};
		newPresetButton.OnMouseDown += SaveNewPreset;
		newPresetButton.OnMouseOver += UiChanger.FadedMouseOver;
		newPresetButton.OnMouseOut += UiChanger.FadedMouseOut;
		uiList.Add(newPresetButton);
	}

	private void GoBack(UIMouseEvent evt, UIElement listeningElement)
	{
		PreviousState.CreateOptionList();
		SoundEngine.PlaySound(SoundID.MenuClose);
		Main.MenuUI.SetState(PreviousState);
	}

	private void SaveNewPreset(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(SoundID.MenuOpen);
		UIVirtualKeyboard uIVirtualKeyboard = new("Enter preset seed", "", text =>
		{
			TagCompound tagCompound;
			if (File.Exists(DataPath))
				tagCompound = TagIO.FromFile(DataPath);
			else
				tagCompound = new TagCompound();
			tagCompound.Set(text, OptionsParser.GetJsonText(false), true);
			TagIO.ToFile(tagCompound, DataPath);
			Main.MenuUI.SetState(new PresetUI(PreviousState));
		}, () =>
		{
			Main.MenuUI.SetState(new PresetUI(PreviousState));
		});
		Main.MenuUI.SetState(uIVirtualKeyboard);
	}

	public void Delete(string name)
	{
		TagCompound tagCompound = TagIO.FromFile(DataPath);
		tagCompound.Remove(name);
		TagIO.ToFile(tagCompound, DataPath);
		Main.MenuUI.SetState(new PresetUI(PreviousState));
	}
}