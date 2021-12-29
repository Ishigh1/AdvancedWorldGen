using System.Collections.Generic;
using System.Linq;
using AdvancedWorldGen.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.OS;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace AdvancedWorldGen.UI
{
	public class OptionsSelector : UIState
	{
		public LocalizedText Description;

		public UIState PreviousState;

		public OptionsSelector(UIState previousState, Option? parent)
		{
			PreviousState = previousState;
			Description = Language.GetText("Mods.AdvancedWorldGen.NoneSelected.Description");

			CreateOptionPanel(parent);
		}

		public void GoBack(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			Main.MenuUI.SetState(PreviousState);
		}

		public void CreateOptionPanel(Option? parent)
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

			UIText uiTitle = new("Seed options", 0.75f, true) { HAlign = 0.5f };
			uiTitle.Height = uiTitle.MinHeight;
			uiPanel.Append(uiTitle);
			uiPanel.Append(new UIHorizontalSeparator
			{
				Width = new StyleDimension(0f, 1f),
				Top = new StyleDimension(43f, 0f),
				Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
			});

			UIText uiDescription = new(Description, 0.75f) { HAlign = 0.5f, VAlign = 0.5f };

			CreateSelectableOptions(uiPanel, uiDescription, parent);

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
				HAlign = 0.4f
			};
			goBack.OnMouseDown += GoBack;
			goBack.OnMouseOver += UiChanger.FadedMouseOver;
			goBack.OnMouseOut += UiChanger.FadedMouseOut;
			Append(goBack);

			UITextPanel<string> customSize = new(Language.GetTextValue("Mods.AdvancedWorldGen.CustomSize"))
			{
				Width = new StyleDimension(0f, 0.1f),
				Top = new StyleDimension(0f, 0.75f),
				HAlign = 0.6f
			};
			customSize.OnMouseDown += GoToCustomSize;
			customSize.OnMouseOver += UiChanger.FadedMouseOver;
			customSize.OnMouseOut += UiChanger.FadedMouseOut;
			Append(customSize);
		}

		public void GoToCustomSize(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuOpen);
			Main.MenuUI.SetState(ModifiedWorld.Instance.CustomSizeUI);
		}

		public void CreateSelectableOptions(UIElement uiPanel, UIText uiDescription, Option? parent)
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
			CreateOptionList(uiDescription, uiList, parent, false);

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
				CreateOptionList(uiDescription, uiList, parent, showHidden);
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

		public void CreateOptionList(UIText uiDescription, UIList uiList, Option? parent, bool showHidden)
		{
			float currentHeight = 50;
			uiList.Clear();
			bool isLookingAtSomething = false;
			
			void SetDefaultDescription(UIMouseEvent evt, UIElement listeningElement)
			{
				uiDescription.SetText(Description);
			}
			
			if (parent == null)
			{
				GroupOptionButton<bool> importButton = new(true,
					Language.GetText("Mods.AdvancedWorldGen.Import"),
					Language.GetText("Mods.AdvancedWorldGen.Import.Description"), Color.White, null)
				{
					HAlign = 0.5f,
					Width = new StyleDimension(0f, 1f),
					Height = new StyleDimension(40f, 0f),
					Top = new StyleDimension(currentHeight, 0f)
				};
				currentHeight += 40;
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
						CreateOptionList(uiDescription, uiList, parent, showHidden);
					}
				};
				importButton.OnMouseOver += delegate { uiDescription.SetText(importButton.Description); };

				importButton.OnMouseOut += SetDefaultDescription;
			}

			foreach ((_, Option? option) in Option.OptionDict)
			{
				if (option.Hidden && !showHidden) continue;
				if (option.Parent != parent) continue;
				GroupOptionButton<bool> clickableText = new(true,
					Language.GetText("Mods.AdvancedWorldGen." + option.SimplifiedName),
					Language.GetText("Mods.AdvancedWorldGen." + option.SimplifiedName + ".Description"), Color.White,
					null)
				{
					HAlign = 0.5f,
					Width = new StyleDimension(0f, 1f),
					Height = new StyleDimension(40f, 0f),
					Top = new StyleDimension(currentHeight, 0f)
				};
				currentHeight += 40;
				uiList.Add(clickableText);

				clickableText.SetCurrentOption(ModifiedWorld.Instance.OptionHelper.OptionsContains(option.FullName));
				clickableText.OnMouseDown += delegate
				{
					if(Main.MenuUI.CurrentState != this)
						return;
					bool selected = clickableText.IsSelected;
					if (selected)
						option.Disable();
					else
						option.Enable();

					if (option.Conflicts
					    .Any(conflict => ModifiedWorld.Instance.OptionHelper.OptionsContains(conflict)))
						CreateOptionList(uiDescription, uiList, parent, showHidden);
					else
						clickableText.SetCurrentOption(!selected);
				};
				clickableText.OnMouseOver += delegate
				{
					if (!isLookingAtSomething)
						uiDescription.SetText(clickableText.Description);
				};
				clickableText.OnMouseOut += SetDefaultDescription;


				if (option.Children.Count != 0)
				{
					LocalizedText downLevel = Language.GetText("Mods.AdvancedWorldGen.DownLevel");
					UIImage uiImage = new(UiChanger.CopyOptionsTexture)
					{
						Left = new StyleDimension(-15, 0f),
						HAlign = 1f,
						VAlign = 0.5f
					};
					uiImage.OnMouseOver += delegate
					{
						SoundEngine.PlaySound(SoundID.MenuTick);
						uiDescription.SetText(downLevel);
						isLookingAtSomething = true;
					};
					uiImage.OnMouseOut += delegate
					{
						SoundEngine.PlaySound(SoundID.MenuTick);
						uiDescription.SetText(clickableText.Description);
						isLookingAtSomething = false;
					};
					uiImage.OnMouseDown += delegate
					{
						SoundEngine.PlaySound(SoundID.MenuOpen);
						Main.MenuUI.SetState(new OptionsSelector(this, option));
					};
					clickableText.Append(uiImage);
				}
				else if (ModifiedWorld.Instance.OptionHelper.OptionsContains(option.FullName))
					foreach (string conflict in Option.OptionDict[option.FullName].Conflicts)
						if (ModifiedWorld.Instance.OptionHelper.OptionsContains(conflict))
						{
							LocalizedText conflictDescription =
								Language.GetText("Mods.AdvancedWorldGen.Conflict." + option.SimplifiedName + "." +
								                 Option.OptionDict[conflict].SimplifiedName);
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
								isLookingAtSomething = true;
							};
							uiImage.OnMouseOut += delegate
							{
								SoundEngine.PlaySound(SoundID.MenuTick);
								uiDescription.SetText(clickableText.Description);
								isLookingAtSomething = false;
							};
							clickableText.Append(uiImage);
							break;
						}
			}
		}

		public static HashSet<string> TextToOptions(string text)
		{
			HashSet<string> options = new();
			foreach (string s in text.Split('|').Where(s => Option.OptionDict.ContainsKey(s)))
				options.Add(s);

			return options;
		}
	}
}