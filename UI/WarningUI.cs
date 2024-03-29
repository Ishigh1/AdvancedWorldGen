namespace AdvancedWorldGen.UI;

public class WarningUI : UIState
{
	public WarningUI(string message, Func<UIState?> back, Func<UIState?> next)
	{
		AdvancedWorldGenMod.Instance.Logger.Warn(message);

		UIPanel uiPanel = new()
		{
			HAlign = 0.5f,
			VAlign = 0.5f,
			Width = new StyleDimension(0, 0.5f),
			Height = new StyleDimension(0, 0.5f),
			BackgroundColor = UICommon.MainPanelBackground
		};
		Append(uiPanel);

		UIText uiTitle = new(Language.GetTextValue("Mods.AdvancedWorldGen.Warning"), 0.75f, true) { HAlign = 0.5f };
		uiTitle.Height = uiTitle.MinHeight;
		uiPanel.Append(uiTitle);
		uiPanel.Append(new UIHorizontalSeparator
		{
			Width = new StyleDimension(0f, 1f),
			Top = new StyleDimension(43f, 0f),
			Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
		});

		UIText uiText = new(message) { Top = new StyleDimension(50f, 0f) };
		uiPanel.Append(uiText);

		UITextPanel<string> goBack = new(Language.GetTextValue("UI.Back"))
		{
			Width = new StyleDimension(0f, 0.1f),
			Top = new StyleDimension(0f, 0.75f),
			HAlign = 0.35f
		};
		goBack.OnLeftClick += delegate
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			UIState? state = back.Invoke();
			if (state != null)
				Main.MenuUI.SetState(state);
		};
		goBack.OnMouseOver += UIChanger.FadedMouseOver;
		goBack.OnMouseOut += UIChanger.FadedMouseOut;
		Append(goBack);

		UITextPanel<string> goForward = new(Language.GetTextValue("tModLoader.Continue"))
		{
			Width = new StyleDimension(0f, 0.1f),
			Top = new StyleDimension(0f, 0.75f),
			HAlign = 0.65f
		};
		goForward.OnLeftClick += delegate
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			UIState? state = next.Invoke();
			if (state != null)
				Main.MenuUI.SetState(state);
		};
		goForward.OnMouseOver += UIChanger.FadedMouseOver;
		goForward.OnMouseOut += UIChanger.FadedMouseOut;
		Append(goForward);
	}
}