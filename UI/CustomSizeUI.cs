using System;
using System.Reflection;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.CustomSized;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace AdvancedWorldGen.UI
{
	public class CustomSizeUI : UIState
	{
		public WorldSettings WorldSettings;

		public CustomSizeUI(WorldSettings worldSettings)
		{
			WorldSettings = worldSettings;
			CreateCustomSizeUI();
		}

		public void CreateCustomSizeUI()
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

			UIText uiTitle = new("Size options", 0.75f, true) {HAlign = 0.5f};
			uiTitle.Height = uiTitle.MinHeight;
			uiPanel.Append(uiTitle);
			uiPanel.Append(new UIHorizontalSeparator
			{
				Width = new StyleDimension(0f, 1f),
				Top = new StyleDimension(43f, 0f),
				Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
			});

			ConfigElement sizeXInput = MakeIntInputLine(nameof(WorldSettings.SizeX), 100);
			sizeXInput.Top.Pixels = 50;
			uiPanel.Append(sizeXInput);

			ConfigElement sizeYInput = MakeIntInputLine(nameof(WorldSettings.SizeY), 100);
			sizeYInput.Top.Pixels = sizeXInput.Top.Pixels + sizeXInput.Height.Pixels + 4;
			uiPanel.Append(sizeYInput);

			UITextPanel<string> goBack = new("Back")
			{
				Width = new StyleDimension(0f, 0.1f),
				Top = new StyleDimension(0f, 0.75f),
				HAlign = 0.5f
			};
			goBack.OnMouseDown += GoBack;
			goBack.OnMouseOver += UiChanger.FadedMouseOver;
			goBack.OnMouseOut += UiChanger.FadedMouseOut;
			Append(goBack);
		}

		public ConfigElement MakeIntInputLine(string fieldName, int min)
		{
			Type intInputElementType = Assembly.GetAssembly(typeof(Main))
				.GetType("Terraria.ModLoader.Config.UI.IntInputElement");

			ConfigElement intInputElement =
				(ConfigElement) intInputElementType.GetConstructor(Array.Empty<Type>()).Invoke(null);

			intInputElementType.GetField("min", BindingFlags.Public | BindingFlags.Instance)
				.SetValue(intInputElement, min);
			intInputElementType.GetField("max", BindingFlags.Public | BindingFlags.Instance)
				.SetValue(intInputElement, 30000);
			intInputElementType.GetField("increment", BindingFlags.Public | BindingFlags.Instance)
				.SetValue(intInputElement, 100);

			intInputElement.Bind(
				new PropertyFieldWrapper(typeof(WorldSettings).GetField(fieldName,
					BindingFlags.Instance | BindingFlags.Public)), WorldSettings, null, -1);
			intInputElement.OnBind();
			intInputElement.Recalculate();
			return intInputElement;
		}

		public void GoBack(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			Main.MenuUI.SetState(Base.AdvancedWorldGen.Instance.UiChanger.OptionsSelector);
		}
	}
}