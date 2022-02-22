using System;
using System.Reflection;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.BetterVanillaWorldGen;
using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
using AdvancedWorldGen.CustomSized;
using AdvancedWorldGen.Helper;
using AdvancedWorldGen.UI.InputUI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace AdvancedWorldGen.UI;

public class CustomSizeUI : UIState
{
	public WorldSettings WorldSettings;

	public CustomSizeUI()
	{
		WorldSettings = ModifiedWorld.Instance.OptionHelper.WorldSettings;
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

		UIText uiTitle = new("Size options", 0.75f, true) { HAlign = 0.5f };
		uiTitle.Height = uiTitle.MinHeight;
		uiPanel.Append(uiTitle);
		uiPanel.Append(new UIHorizontalSeparator
		{
			Width = new StyleDimension(0f, 1f),
			Top = new StyleDimension(43f, 0f),
			Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
		});

		NumberTextBox<int> sizeXInput = new(WorldSettings.Params, nameof(Params.SizeX), 100, ushort.MaxValue);
		sizeXInput.Top.Pixels = 50;
		uiPanel.Append(sizeXInput);

		NumberTextBox<int> sizeYInput = new(WorldSettings.Params, nameof(Params.SizeY), 100, ushort.MaxValue);
		sizeYInput.Top.Pixels = sizeXInput.Top.Pixels + sizeXInput.Height.Pixels + 4;
		uiPanel.Append(sizeYInput);

		NumberTextBox<float> templeModifier = new(WorldSettings.Params, nameof(Params.TempleMultiplier), 0, float.PositiveInfinity);
		templeModifier.Top.Pixels = sizeYInput.Top.Pixels + sizeYInput.Height.Pixels + 4;
		uiPanel.Append(templeModifier);

		UITextPanel<string> goBack = new(Language.GetTextValue("UI.Back"))
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

	public void GoBack(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(SoundID.MenuClose);
		int size = WorldSettings.Params.SizeX switch
		{
			4200 when WorldSettings.Params.SizeY == 1200 => 0,
			6400 when WorldSettings.Params.SizeY == 1800 => 1,
			8400 when WorldSettings.Params.SizeY == 2400 => 2,
			_ => -1
		};

		VanillaAccessor<int> optionSize = VanillaInterface.OptionSize(WorldSettings.UIWorldCreation);
		optionSize.Value = size;

		object[] sizeButtons = VanillaInterface.SizeButtons(WorldSettings.UIWorldCreation).Value;

		Type groupOptionButtonType = sizeButtons.GetType().GetElementType()!;
		MethodInfo setCurrentOptionMethod =
			groupOptionButtonType.GetMethod("SetCurrentOption", BindingFlags.Instance | BindingFlags.Public)!;

		foreach (object groupOptionButton in sizeButtons)
			setCurrentOptionMethod.Invoke(groupOptionButton, new object[] { size });

#if !SPECIALDEBUG
		int oldSizeX = Main.tile.Width;
		int oldSizeY = Main.tile.Height;
		if (oldSizeX < WorldSettings.Params.SizeX || oldSizeY < WorldSettings.Params.SizeY)
		{
			int newSizeX = Math.Max(WorldSettings.Params.SizeX, oldSizeX);
			int newSizeY = Math.Max(WorldSettings.Params.SizeY, oldSizeY);

			if ((long)newSizeX * newSizeY * 44 > GC.GetGCMemoryInfo().TotalAvailableMemoryBytes)
			{
				Main.MenuUI.SetState(new ErrorUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.TooBigFromRAM", newSizeX, newSizeY)));
				return;
			}
		}

		if (WorldgenSettings.Revamped)
		{
			if (WorldSettings.Params.SizeX < KnownLimits.OverhauledMinX)
			{
				Main.MenuUI.SetState(new ErrorUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.OverhauledMinX", KnownLimits.OverhauledMinX)));
				return;
			}

			if (WorldSettings.Params.SizeY < KnownLimits.OverhauledMinY)
			{
				Main.MenuUI.SetState(new ErrorUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.OverhauledMinY", KnownLimits.OverhauledMinY)));
				return;
			}
		}
		else
		{
			if (WorldSettings.Params.SizeX < KnownLimits.NormalMinX)
			{
				Main.MenuUI.SetState(new ErrorUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.NormalMinX", KnownLimits.NormalMinX)));
				return;
			}

			if (WorldSettings.Params.SizeY > KnownLimits.ComfortNormalMaxX)
			{
				Main.MenuUI.SetState(new ErrorUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.ComfortNormalMaxX")));
				return;
			}
		}
#endif

		Main.MenuUI.SetState(AdvancedWorldGenMod.Instance.UiChanger.OptionsSelector);
	}
}