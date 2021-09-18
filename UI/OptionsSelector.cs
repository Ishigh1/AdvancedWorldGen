using System.Collections.Generic;
using System.Linq;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.OS;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace AdvancedWorldGen.UI
{
	public class OptionsSelector : UIState
	{
		public static Dictionary<string, Option> OptionDict = null!;
		public LocalizedText Description;

		public UIWorldCreation UiWorldCreation;

		public OptionsSelector(UIWorldCreation uiWorldCreation)
		{
			UiWorldCreation = uiWorldCreation;
			Description = Language.GetText("Mods.AdvancedWorldGen.NoneSelected.Description");

			CreateOptionPanel();
		}

		public void CreateOptionPanel()
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

			UIText uiTitle = new(Language.GetText("Mods.AdvancedWorldGen.SeedOptions"), 0.75f, true) {HAlign = 0.5f};
			uiTitle.Height = uiTitle.MinHeight;
			uiPanel.Append(uiTitle);
			uiPanel.Append(new UIHorizontalSeparator
			{
				Width = new StyleDimension(0f, 1f),
				Top = new StyleDimension(43f, 0f),
				Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
			});

			UIText uiDescription = new(Description, 0.75f) {HAlign = 0.5f, VAlign = 0.5f};

			CreateSelectableOptions(uiPanel, uiDescription);

			uiPanel.Append(new UIHorizontalSeparator
			{
				Width = new StyleDimension(0f, 1f),
				Top = new StyleDimension(-50f, 1f),
				Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
			});

			uiDescription.Height = uiDescription.MinHeight;
			UISlicedImage uIDescriptionBox =
				new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight"))
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

			UITextPanel<string> goBack = new(Language.GetTextValue("UI.Back"))
			{
				Width = new StyleDimension(0f, 0.1f),
				Top = new StyleDimension(0f, 0.75f),
				HAlign = 0.3f
			};
			goBack.OnMouseDown += UIHelper.GoTo(UiWorldCreation, false);
			goBack.OnMouseOver += UIHelper.FadedMouseOver;
			goBack.OnMouseOut += UIHelper.FadedMouseOut;
			Append(goBack);

			UITextPanel<string> customSize = new(Language.GetTextValue("Mods.AdvancedWorldGen.CustomSize"))
			{
				Width = new StyleDimension(0f, 0.1f),
				Top = new StyleDimension(0f, 0.75f),
				HAlign = 0.5f
			};
			customSize.OnMouseDown += UIHelper.GoTo(ModifiedWorld.Instance.CustomSizeUI);
			customSize.OnMouseOver += UIHelper.FadedMouseOver;
			customSize.OnMouseOut += UIHelper.FadedMouseOut;
			Append(customSize);

			UITextPanel<string> editPasses = new(Language.GetTextValue("Mods.AdvancedWorldGen.EditPasses"))
			{
				Width = new StyleDimension(0f, 0.1f),
				Top = new StyleDimension(0f, 0.75f),
				HAlign = 0.7f
			};
			editPasses.OnMouseDown += UIHelper.GoTo(new PassListEditor(this));
			editPasses.OnMouseOver += UIHelper.FadedMouseOver;
			editPasses.OnMouseOut += UIHelper.FadedMouseOut;
			Append(editPasses);
		}

		public void CreateSelectableOptions(UIElement uiPanel, UIText uiDescription)
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
			uiList.ManualSortMethod = _ => { };
			uiList.SetScrollbar(uiScrollbar);
			uiPanel.Append(uiScrollbar);
			uiPanel.Append(uiList);
			CreateOptionList(uiDescription, uiList, false);

			bool showHidden = false;
			LocalizedText showHiddenDescription = Language.GetText("Mods.AdvancedWorldGen.ShowHidden.Description");
			LocalizedText hideHiddenDescription = Language.GetText("Mods.AdvancedWorldGen.HideHidden.Description");
			UIImage uiImage = new(TextureAssets.InventoryTickOff)
			{
				HAlign = 1f
			};
			uiImage.OnMouseDown += delegate
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				showHidden = !showHidden;
				uiImage.SetImage(showHidden ? TextureAssets.InventoryTickOn : TextureAssets.InventoryTickOff);
				CreateOptionList(uiDescription, uiList, showHidden);
				uiDescription.SetText(showHidden ? hideHiddenDescription : showHiddenDescription);
			};
			uiImage.OnMouseOver += delegate
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				uiDescription.SetText(showHidden ? hideHiddenDescription : showHiddenDescription);
			};
			uiImage.OnMouseOut += delegate
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				uiDescription.SetText(Description);
			};
			uiPanel.Append(uiImage);
		}

		public void CreateOptionList(UIText uiDescription, UIList uiList, bool showHidden)
		{
			uiList.Clear();
			bool isLookingAtConflict = false;

			GroupOptionButton<bool> importButton = new(true,
				Language.GetText("Mods.AdvancedWorldGen.Import"),
				Language.GetText("Mods.AdvancedWorldGen.Import.Description"), Color.White, null)
			{
				HAlign = 0.5f,
				Width = new StyleDimension(0f, 1f),
				Height = new StyleDimension(40f, 0f)
			};
			uiList.Add(importButton);

			importButton.SetCurrentOption(false);
			importButton.OnMouseDown += delegate
			{
				string optionText = Platform.Get<IClipboard>().Value;
				HashSet<string> options = TextToOptions(optionText);
				if (options.Count != 0)
				{
					SoundEngine.PlaySound(SoundID.MenuOpen);
					ModifiedWorld.Instance.OptionHelper.Options = options;
					CreateOptionList(uiDescription, uiList, showHidden);
				}
			};
			importButton.OnMouseOver += delegate { uiDescription.SetText(importButton.Description); };

			void SetDefaultDescription(UIMouseEvent evt, UIElement listeningElement)
			{
				uiDescription.SetText(Description);
			}

			importButton.OnMouseOut += SetDefaultDescription;

			foreach (var (optionText, option) in OptionDict)
			{
				if (option.Hidden && !showHidden) continue;
				GroupOptionButton<bool> clickableText = new(true,
					Language.GetText("Mods.AdvancedWorldGen." + optionText),
					Language.GetText("Mods.AdvancedWorldGen." + optionText + ".Description"), Color.White, null)
				{
					HAlign = 0.5f,
					Width = new StyleDimension(0f, 1f),
					Height = new StyleDimension(40f, 0f)
				};
				uiList.Add(clickableText);

				clickableText.SetCurrentOption(ModifiedWorld.Instance.OptionHelper.OptionsContains(optionText));
				clickableText.OnMouseDown += delegate
				{
					bool selected = clickableText.IsSelected;
					if (selected)
						ModifiedWorld.Instance.OptionHelper.Options.Remove(optionText);
					else
						ModifiedWorld.Instance.OptionHelper.Options.Add(optionText);

					if (OptionDict[optionText].Conflicts
						.Any(conflict => ModifiedWorld.Instance.OptionHelper.OptionsContains(conflict)))
						CreateOptionList(uiDescription, uiList, showHidden);
					else
						clickableText.SetCurrentOption(!selected);
				};
				clickableText.OnMouseOver += delegate
				{
					if (!isLookingAtConflict)
						uiDescription.SetText(clickableText.Description);
				};
				clickableText.OnMouseOut += SetDefaultDescription;

				if (ModifiedWorld.Instance.OptionHelper.OptionsContains(optionText))
					foreach (string conflict in OptionDict[optionText].Conflicts)
						if (ModifiedWorld.Instance.OptionHelper.OptionsContains(conflict))
						{
							LocalizedText conflictDescription =
								Language.GetText("Mods.AdvancedWorldGen.Conflict." + optionText + "." + conflict);
							UIImage uiImage = new(UICommon.ButtonErrorTexture)
							{
								Left = new StyleDimension(-15, 0f),
								HAlign = 1f,
								VAlign = 0.5f
							};
							uiImage.OnMouseOver += delegate
							{
								SoundEngine.PlaySound(SoundID.MenuTick);
								uiDescription.SetText(conflictDescription);
								isLookingAtConflict = true;
							};
							uiImage.OnMouseOut += delegate
							{
								SoundEngine.PlaySound(SoundID.MenuTick);
								uiDescription.SetText(clickableText.Description);
								isLookingAtConflict = false;
							};
							clickableText.Append(uiImage);
							break;
						}
			}
		}

		public static HashSet<string> TextToOptions(string text)
		{
			HashSet<string> options = new();
			foreach (string s in text.Split('|').Where(s => OptionDict.Keys.Contains(s)))
				options.Add(s);

			return options;
		}
	}
}