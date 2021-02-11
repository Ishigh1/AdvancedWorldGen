using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;

//TODO : Add the interface to import settings
//TODO : Add a scrollbar
namespace AdvancedWorldGen.SeedUI
{
	public class OptionsSelector : UIState
	{
		public static Dictionary<string, Option> OptionDict;

		public SeedHelper SeedHelper;
		public UIWorldCreation UiWorldCreation;
		public LocalizedText Description;

		public OptionsSelector(UIWorldCreation uiWorldCreation, SeedHelper seedHelper)
		{
			SeedHelper = seedHelper;
			UiWorldCreation = uiWorldCreation;
			Description = Language.GetText("Mods.AdvancedWorldGen.NoneSelected");

			CreateOptionPanel();
		}

		private void GoBack(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.MenuUI.SetState(UiWorldCreation);
		}

		public void CreateOptionPanel()
		{
			UIPanel uiPanel = new UIPanel
			{
				HAlign = 0.5f,
				VAlign = 0.5f,
				Width = new StyleDimension(0, 0.4f),
				Height = new StyleDimension(0, 0.5f),
				BackgroundColor = UICommon.MainPanelBackground
			};
			Append(uiPanel);

			UIText uiTitle = new UIText("Seed options", 0.75f, true) {HAlign = 0.5f};
			uiTitle.Height = uiTitle.MinHeight;
			uiPanel.Append(uiTitle);
			uiPanel.Append(new UIHorizontalSeparator
			{
				Width = new StyleDimension(0f, 1f),
				Top = new StyleDimension(43f, 0f),
				Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
			});

			UIText uiDescription = new UIText(Description, 0.75f) {HAlign = 0.5f, VAlign = 0.5f};

			CreateSelectableOptions(uiPanel, uiDescription);

			uiPanel.Append(new UIHorizontalSeparator
			{
				Width = new StyleDimension(0f, 1f),
				Top = new StyleDimension(-50f, 1f),
				Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
			});

			uiDescription.Height = uiDescription.MinHeight;
			UISlicedImage uIDescriptionBox =
				new UISlicedImage(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight"))
				{
					Width = new StyleDimension(0f, 1f),
					Height = new StyleDimension(40f, 0),
					Top = new StyleDimension(-40f, 1f),
					HAlign = 0.5f,
					Color = Color.LightGray * 0.7f
				};
			uIDescriptionBox.SetSliceDepths(10);
			uIDescriptionBox.Append(uiDescription);
			uiPanel.Append(uIDescriptionBox);

			uiPanel.Recalculate();

			UITextPanel<string> goBack = new UITextPanel<string>("Go back")
			{
				Width = new StyleDimension(0f, 0.1f),
				Top = new StyleDimension(0f, 0.75f),
				HAlign = 0.5f
			};
			goBack.OnMouseDown += GoBack;
			Append(goBack);
		}

		private void CreateSelectableOptions(UIElement uiPanel, UIText uiDescription)
		{
			float currentHeight = 50;
			foreach (KeyValuePair<string, Option> keyValuePair in OptionDict)
			{
				GroupOptionButton<bool> clickableText = new GroupOptionButton<bool>(true,
					Language.GetText("Mods.AdvancedWorldGen." + keyValuePair.Key),
					Language.GetText("Mods.AdvancedWorldGen." + keyValuePair.Key + ".description"), Color.White, null)
				{
					HAlign = 0.5f,
					Width = new StyleDimension(-uiPanel.MarginLeft - uiPanel.MarginRight, 1f),
					Height = new StyleDimension(40f, 0f),
					Top = new StyleDimension(currentHeight, 0f)
				};
				currentHeight += 40;
				uiPanel.Append(clickableText);

				clickableText.SetCurrentOption(false);
				clickableText.OnMouseDown += delegate
				{
					bool selected = clickableText.IsSelected;
					if (selected)
					{
						SeedHelper.Options.Remove(keyValuePair.Key);
					}
					else
					{
						SeedHelper.Options.Add(keyValuePair.Key);
					}

					clickableText.SetCurrentOption(!selected);
				};
				clickableText.OnMouseOver += delegate
				{
					uiDescription.SetText(clickableText.Description);
				};
				clickableText.OnMouseOut += delegate
				{
					uiDescription.SetText(Description);
				};
			}
		}
	}
}