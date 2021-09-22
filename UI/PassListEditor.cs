using System.Collections.Generic;
using System.Linq;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.Helper;
using AdvancedWorldGen.WorldRegenerator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
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

		public void CreatePassEditor(UIPanel uiPanel)
		{
			UIScrollbar uiScrollbar = new()
			{
				Height = new StyleDimension(-50f, 1f),
				Top = new StyleDimension(50, 0f),
				HAlign = 1f
			};
			UIList uiList = new()
			{
				Height = new StyleDimension(-50f, 1f),
				Width = new StyleDimension(-20f, 1f),
				Top = new StyleDimension(50, 0f)
			};
			Dictionary<UIElement, int> order = new();
			uiList.ManualSortMethod = uiElements =>
				uiElements.Sort((element1, element2) => order[element1] >= order[element2] ? 1 : -1);
			uiList.SetScrollbar(uiScrollbar);
			uiPanel.Append(uiScrollbar);
			uiPanel.Append(uiList);

			for (int index = 0; index < PassHandler.AvailablePasses.Count; index++)
				CreatePassEntry(index, order, uiList);
		}

		public static void CreatePassEntry(int index, Dictionary<UIElement, int> order, UIList uiList)
		{
			const int spaceBetweenIcons = 28;
			GenPass availablePass = PassHandler.AvailablePasses[index];
			UIIconTextButton passEntry = new(LocalizedText.Empty, Color.White, null)
			{
				HAlign = 0.5f,
				Width = new StyleDimension(0f, 1f),
				Height = new StyleDimension(40f, 0f)
			};
			order.Add(passEntry, index);

			UIText uiText = new VanillaAccessor<UIText>(typeof(UIIconTextButton), "_title", passEntry).Value;
			new VanillaAccessor<string>(typeof(UIText), "_text", uiText).Value = availablePass.Name;

			UIImage deletePassButton = new(TextureAssets.Trash)
			{
				Top = {Percent = 0.5f, Pixels = -14},
				Left = {Percent = 1f, Pixels = -50},
				Width = {Pixels = 25},
				Height = {Pixels = 28}
			};
			deletePassButton.OnMouseDown += (_, _) =>
			{
				index = order[passEntry];
				
				SoundEngine.PlaySound(SoundID.MenuTick);
				PassHandler.AvailablePasses.RemoveAt(index);
				uiList.Remove(passEntry);
				for (var i = 0; i < order.Count; i++)
				{
					(UIElement element, int value) = order.ElementAt(i);
					if (value > index)
						order[element] = value - 1;
				}
			};
			passEntry.Append(deletePassButton);

			UIImage copyPassButton = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Copy"))
			{
				Top = {Percent = 0.5f, Pixels = -14},
				Left = {Percent = 1f, Pixels = -50 - spaceBetweenIcons},
				Width = {Pixels = 25},
				Height = {Pixels = 28}
			};
			copyPassButton.OnMouseDown += (_, _) =>
			{
				index = order[passEntry];
				
				SoundEngine.PlaySound(SoundID.MenuTick);
				PassHandler.AvailablePasses.Insert(index + 1, availablePass);
				for (var i = 0; i < order.Count; i++)
				{
					(UIElement element, int value) = order.ElementAt(i);
					if (value > index)
						order[element] = value + 1;
				}

				CreatePassEntry(index + 1, order, uiList);
			};
			passEntry.Append(copyPassButton);

			UIImage moveUpButton =
				new(AdvancedWorldGenMod.Instance.Assets.Request<Texture2D>("Images/ArrowUp"))
				{
					Top = {Percent = 0.5f, Pixels = -14},
					Left = {Percent = 1f, Pixels = -50 - 2*spaceBetweenIcons},
					Width = {Pixels = 25},
					Height = {Pixels = 13}
				};
			moveUpButton.OnMouseDown += (_, _) =>
			{
				index = order[passEntry];
				if(index == PassHandler.AvailablePasses.Count - 1)
					return;
				SoundEngine.PlaySound(SoundID.MenuTick);
				
				PassHandler.AvailablePasses[index] = PassHandler.AvailablePasses[index + 1];
				PassHandler.AvailablePasses[++index] = availablePass;
				
				UIElement elementToExchange = order.First(pair => pair.Value == index).Key;
				order[elementToExchange] = index - 1;
				order[passEntry] = index;
				uiList.UpdateOrder();
				uiList.Recalculate();
			};
			passEntry.Append(moveUpButton);

			UIImage moveDownButton =
				new(AdvancedWorldGenMod.Instance.Assets.Request<Texture2D>("Images/ArrowDown"))
				{
					Top = {Percent = 0.5f, Pixels = 1},
					Left = {Percent = 1f, Pixels = -50 - 2*spaceBetweenIcons},
					Width = {Pixels = 25},
					Height = {Pixels = 13}
				};
			moveDownButton.OnMouseDown += (_, _) =>
			{
				index = order[passEntry];
				if(index == 0)
					return;
				SoundEngine.PlaySound(SoundID.MenuTick);
				
				PassHandler.AvailablePasses[index] = PassHandler.AvailablePasses[index - 1];
				PassHandler.AvailablePasses[--index] = availablePass;
				
				UIElement elementToExchange = order.First(pair => pair.Value == index).Key;
				order[elementToExchange] = index + 1;
				order[passEntry] = index;
				uiList.UpdateOrder();
				uiList.Recalculate();
			};
			passEntry.Append(moveDownButton);

			uiList.Add(passEntry);
		}
	}
}