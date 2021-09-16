using AdvancedWorldGen.Base;
using AdvancedWorldGen.Helper;
using AdvancedWorldGen.WorldRegenerator;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.UI
{
	public class PassListEditor : UIState
	{
		public OptionsSelector OptionsSelector;
		public PassListEditor(OptionsSelector optionsSelector)
		{
			OptionsSelector = optionsSelector;
			if (!PassHandler.ReplacePasses) PassHandler.LoadPasses();
			CreatePassListEditorPanel();
		}

		public void CreatePassListEditorPanel()
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

			UIText uiTitle = new(Language.GetText("Mods.AdvancedWorldGen.PassEditor"), 0.75f, true) {HAlign = 0.5f};
			uiTitle.Height = uiTitle.MinHeight;
			uiPanel.Append(uiTitle);
			uiPanel.Append(new UIHorizontalSeparator
			{
				Width = new StyleDimension(0f, 1f),
				Top = new StyleDimension(43f, 0f),
				Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
			});

			CreatePassEditor(uiPanel);

			uiPanel.Append(new UIHorizontalSeparator
			{
				Width = new StyleDimension(0f, 1f),
				Top = new StyleDimension(-50f, 1f),
				Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
			});

			uiPanel.Recalculate();

			UITextPanel<string> goBack = new(Language.GetTextValue("UI.Back"))
			{
				Width = new StyleDimension(0f, 0.1f),
				Top = new StyleDimension(0f, 0.75f),
				HAlign = 0.5f
			};
			goBack.OnMouseDown += UIHelper.GoTo(OptionsSelector, false);
			goBack.OnMouseOver += UIHelper.FadedMouseOver;
			goBack.OnMouseOut += UIHelper.FadedMouseOut;
			Append(goBack);
		}

		public static void CreatePassEditor(UIPanel uiPanel)
		{
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
			uiPanel.Append(uiScrollbar);
			uiPanel.Append(uiList);

			float currentHeight = 0f;
			foreach (GenPass availablePass in PassHandler.AvailablePasses)
			{
				LocalizedText fakeLocalization = LanguageManager.Instance.GetText(availablePass.Name);
				UIIconTextButton passEntry = new(fakeLocalization, Color.White, null)
				{
					HAlign = 0.5f,
					Width = new StyleDimension(0f, 1f),
					Height = new StyleDimension(40f, 0f),
					Top = new StyleDimension(currentHeight, 0f)
				};
				uiList.Add(passEntry);
				currentHeight += 40f;
			}
		}
	}
}