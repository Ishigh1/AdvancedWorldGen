using AdvancedWorldGen.Base;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace AdvancedWorldGen.UI
{
	public class ErrorUI : UIState
	{
		public ErrorUI(string message)
		{
			BasicSetup(message);
		}

		public void BasicSetup(string message)
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

			UIText uiTitle = new(Language.GetTextValue("Mods.AdvancedWorldGen.Warning"), 0.75f, true) {HAlign = 0.5f};
			uiTitle.Height = uiTitle.MinHeight;
			uiPanel.Append(uiTitle);
			uiPanel.Append(new UIHorizontalSeparator
			{
				Width = new StyleDimension(0f, 1f),
				Top = new StyleDimension(43f, 0f),
				Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
			});

			UIText uiText = new(message) {Top = new StyleDimension(50f, 0f)};
			uiPanel.Append(uiText);

			UITextPanel<string> goBack = new(Language.GetTextValue("UI.Back"))
			{
				Width = new StyleDimension(0f, 0.1f),
				Top = new StyleDimension(0f, 0.75f),
				HAlign = 0.35f
			};
			goBack.OnMouseDown += GoBack;
			goBack.OnMouseOver += UiChanger.FadedMouseOver;
			goBack.OnMouseOut += UiChanger.FadedMouseOut;
			Append(goBack);

			UITextPanel<string> goForward = new(Language.GetTextValue("tModLoader.Continue"))
			{
				Width = new StyleDimension(0f, 0.1f),
				Top = new StyleDimension(0f, 0.75f),
				HAlign = 0.65f
			};
			goForward.OnMouseDown += Continue;
			goForward.OnMouseOver += UiChanger.FadedMouseOver;
			goForward.OnMouseOut += UiChanger.FadedMouseOut;
			Append(goForward);
		}

		public static void GoBack(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			Main.MenuUI.SetState(ModifiedWorld.Instance.CustomSizeUI);
		}

		public static void Continue(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			Main.MenuUI.SetState(Base.AdvancedWorldGenMod.Instance.UiChanger.OptionsSelector);
		}
	}
}