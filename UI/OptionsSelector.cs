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
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace AdvancedWorldGen.UI;

public class OptionsSelector : UIState
{
	public static bool ShowHidden;
	public LocalizedText Description;
	public UIText UIDescription = null!;
	public UIList UIList = null!;
	public Option? Parent;

	public UIState PreviousState;

	public OptionsSelector(UIState previousState, Option? parent)
	{
		PreviousState = previousState;
		Parent = parent;
		Description = Language.GetText("Mods.AdvancedWorldGen.NoneSelected.Description");

		CreateOptionPanel();
	}

	public void GoBack(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(SoundID.MenuClose);
		Main.MenuUI.SetState(PreviousState);
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

		UIText uiTitle = new("Seed options", 0.75f, true) { HAlign = 0.5f };
		uiTitle.Height = uiTitle.MinHeight;
		uiPanel.Append(uiTitle);
		uiPanel.Append(new UIHorizontalSeparator
		{
			Width = new StyleDimension(0f, 1f),
			Top = new StyleDimension(43f, 0f),
			Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
		});

		UIDescription = new UIText(Description, 0.75f) { HAlign = 0.5f, VAlign = 0.5f };

		CreateSelectableOptions(uiPanel);

		uiPanel.Append(new UIHorizontalSeparator
		{
			Width = new StyleDimension(0f, 1f),
			Top = new StyleDimension(-50f, 1f),
			Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
		});

		UIDescription.Height = UIDescription.MinHeight;
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
		uIDescriptionBox.Append(UIDescription);
		uiPanel.Append(uIDescriptionBox);

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

		UITextPanel<string> customSize = new(Language.GetTextValue("Mods.AdvancedWorldGen.CustomSize"))
		{
			Width = new StyleDimension(0f, 0.1f),
			Top = new StyleDimension(0f, 0.8f),
			HAlign = 0.6f
		};
		customSize.OnMouseDown += GoToCustomSize;
		customSize.OnMouseOver += UiChanger.FadedMouseOver;
		customSize.OnMouseOut += UiChanger.FadedMouseOut;
		Append(customSize);
		
		UITextPanel<string> importButton = new(Language.GetTextValue("Mods.AdvancedWorldGen.Import"))
		{
			Width = new StyleDimension(0f, 0.1f),
			Top = new StyleDimension(0f, 0.75f),
			HAlign = 0.4f
		};
		importButton.OnMouseDown += delegate
		{
			OptionsParser.Parse(Platform.Get<IClipboard>().Value);
			CreateOptionList();
		};
		importButton.OnMouseOver += UiChanger.FadedMouseOver;
		importButton.OnMouseOut += UiChanger.FadedMouseOut;
		Append(importButton);
		
		UITextPanel<string> exportButton = new(Language.GetTextValue("Mods.AdvancedWorldGen.Export"))
		{
			Width = new StyleDimension(0f, 0.1f),
			Top = new StyleDimension(0f, 0.75f),
			HAlign = 0.6f
		};
		exportButton.OnMouseDown += delegate { Platform.Get<IClipboard>().Value = OptionsParser.GetJsonText(); };
		exportButton.OnMouseOver += UiChanger.FadedMouseOver;
		exportButton.OnMouseOut += UiChanger.FadedMouseOut;
		Append(exportButton);
	}

	public static void GoToCustomSize(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(SoundID.MenuOpen);
		Main.MenuUI.SetState(new CustomSizeUI());
	}

	public void CreateSelectableOptions(UIElement uiPanel)
	{
		UIScrollbar uiScrollbar = new()
		{
			Height = new StyleDimension(-110f, 1f),
			Top = new StyleDimension(50, 0f),
			HAlign = 1f
		};
		UIList = new UIList
		{
			Height = new StyleDimension(-110f, 1f),
			Width = new StyleDimension(-20f, 1f),
			Top = new StyleDimension(50, 0f)
		};
		UIList.SetScrollbar(uiScrollbar);
		uiPanel.Append(uiScrollbar);
		uiPanel.Append(UIList);
		CreateOptionList();

		LocalizedText showHiddenDescription = Language.GetText("Mods.AdvancedWorldGen.ShowHidden.Description");
		LocalizedText hideHiddenDescription = Language.GetText("Mods.AdvancedWorldGen.HideHidden.Description");
		UIImage uiImage = new(TextureAssets.InventoryTickOff)
		{
			HAlign = 1f
		};
		uiImage.OnMouseDown += delegate
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			ShowHidden = !ShowHidden;
			uiImage.SetImage(ShowHidden ? TextureAssets.InventoryTickOn : TextureAssets.InventoryTickOff);
			CreateOptionList();
			UIDescription.SetText(ShowHidden ? hideHiddenDescription : showHiddenDescription);
		};
		uiImage.OnMouseOver += delegate
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			UIDescription.SetText(ShowHidden ? hideHiddenDescription : showHiddenDescription);
		};
		uiImage.OnMouseOut += delegate
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			UIDescription.SetText(Description);
		};
		uiPanel.Append(uiImage);
	}

	public void CreateOptionList()
	{
		float currentHeight = 50;
		UIList.Clear();
		bool isLookingAtSomething = false;

		void SetDefaultDescription(UIMouseEvent evt, UIElement listeningElement)
		{
			UIDescription.SetText(Description);
		}

		foreach ((_, Option? option) in ModifiedWorld.Instance.OptionHelper.OptionDict)
		{
			if (option.Hidden && !ShowHidden) continue;
			if (option.Parent != Parent) continue;
			GroupOptionButton<bool> clickableText = new(true,
				Language.GetText("Mods." + option.ModName + "." + option.SimplifiedName),
				Language.GetText("Mods." + option.ModName + "." + option.SimplifiedName + ".Description"), Color.White,
				null)
			{
				HAlign = 0.5f,
				Width = new StyleDimension(0f, 1f),
				Height = new StyleDimension(40f, 0f),
				Top = new StyleDimension(currentHeight, 0f)
			};
			currentHeight += 40;
			UIList.Add(clickableText);

			clickableText.SetCurrentOption(option.Enabled);
			clickableText.OnMouseDown += delegate
			{
				if (Main.MenuUI.CurrentState != this)
					return;
				bool selected = clickableText.IsSelected;
				if (selected)
					option.Disable();
				else
					option.Enable();

				if (option.Conflicts
				    .Any(conflict => API.OptionsContains(conflict)))
					CreateOptionList();
				else
					clickableText.SetCurrentOption(!selected);
			};
			clickableText.OnMouseOver += delegate
			{
				if (!isLookingAtSomething)
					UIDescription.SetText(clickableText.Description);
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
					UIDescription.SetText(downLevel);
					isLookingAtSomething = true;
				};
				uiImage.OnMouseOut += delegate
				{
					SoundEngine.PlaySound(SoundID.MenuTick);
					UIDescription.SetText(clickableText.Description);
					isLookingAtSomething = false;
				};
				uiImage.OnMouseDown += delegate
				{
					SoundEngine.PlaySound(SoundID.MenuOpen);
					Main.MenuUI.SetState(new OptionsSelector(this, option));
				};
				clickableText.Append(uiImage);
			}
			else if (option.Enabled)
			{
				foreach (string conflict in option.Conflicts)
					if (API.OptionsContains(conflict))
					{
						LocalizedText conflictDescription =
							Language.GetText("Mods.AdvancedWorldGen." + option.SimplifiedName + ".Conflicts." +
							                 conflict);
						UIImage uiImage = new(UICommon.ButtonErrorTexture)
						{
							Left = new StyleDimension(-15, 0f),
							HAlign = 1f,
							VAlign = 0.5f
						};
						uiImage.OnMouseOver += delegate
						{
							SoundEngine.PlaySound(SoundID.MenuTick);
							UIDescription.SetText(conflictDescription);
							isLookingAtSomething = true;
						};
						uiImage.OnMouseOut += delegate
						{
							SoundEngine.PlaySound(SoundID.MenuTick);
							UIDescription.SetText(clickableText.Description);
							isLookingAtSomething = false;
						};
						clickableText.Append(uiImage);
						break;
					}
			}
		}
	}
}