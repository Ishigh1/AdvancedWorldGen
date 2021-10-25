using System;
using System.Reflection;
using AdvancedWorldGen.BetterVanillaWorldGen;
using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
using AdvancedWorldGen.CustomSized;
using AdvancedWorldGen.Helper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace AdvancedWorldGen.UI
{
	public class CustomSizeUI : UIState
	{
		private static readonly Type IntInputElementType =
			typeof(Main).Assembly.GetType("Terraria.ModLoader.Config.UI.IntInputElement")!;

		private static readonly ConstructorInfo IntInputElementConstructorInfo =
			IntInputElementType.GetConstructor(Array.Empty<Type>())!;

		private static readonly FieldInfo IntInputElementMinField =
			IntInputElementType.GetField("min", BindingFlags.Public | BindingFlags.Instance)!;

		private static readonly FieldInfo IntInputElementMaxField =
			IntInputElementType.GetField("max", BindingFlags.Public | BindingFlags.Instance)!;

		private static readonly FieldInfo IntInputElementIncrementField =
			IntInputElementType.GetField("increment", BindingFlags.Public | BindingFlags.Instance)!;

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

		public ConfigElement MakeIntInputLine(string fieldName, int min)
		{
			ConfigElement intInputElement = (ConfigElement) IntInputElementConstructorInfo.Invoke(null);

			IntInputElementMinField.SetValue(intInputElement, min);
			IntInputElementMaxField.SetValue(intInputElement, 500000);
			IntInputElementIncrementField.SetValue(intInputElement, 100);

			FieldInfo fieldInfo = typeof(WorldSettings).GetField(fieldName, BindingFlags.Instance | BindingFlags.Public)!;
			intInputElement.Bind(new PropertyFieldWrapper(fieldInfo), WorldSettings, null, -1);
			intInputElement.OnBind();
			intInputElement.Recalculate();
			return intInputElement;
		}

		public void GoBack(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			int size = WorldSettings.SizeX switch
			{
				4200 when WorldSettings.SizeY == 1200 => 0,
				6400 when WorldSettings.SizeY == 1800 => 1,
				8400 when WorldSettings.SizeY == 2400 => 2,
				_ => -1
			};
			
			VanillaAccessor<int> optionSize = VanillaInterface.OptionSize(WorldSettings.UIWorldCreation);
			optionSize.Value = size;

			object[] sizeButtons = VanillaInterface.SizeButtons(WorldSettings.UIWorldCreation).Value;

			Type groupOptionButtonType = sizeButtons.GetType().GetElementType()!;
			MethodInfo setCurrentOptionMethod =
				groupOptionButtonType.GetMethod("SetCurrentOption", BindingFlags.Instance | BindingFlags.Public)!;

			foreach (object groupOptionButton in sizeButtons)
				setCurrentOptionMethod.Invoke(groupOptionButton, new object[] {size});

#if !SPECIALDEBUG
			int oldSizeX = Main.tile.GetLength(0);
			int oldSizeY = Main.tile.GetLength(1);
			if (oldSizeX < WorldSettings.SizeX || oldSizeY < WorldSettings.SizeY)
			{
				int newSizeX = Math.Max(WorldSettings.SizeX, oldSizeX);
				int newSizeY = Math.Max(WorldSettings.SizeY, oldSizeY);

				if ((long) newSizeX * newSizeY * 44 > GC.GetGCMemoryInfo().TotalAvailableMemoryBytes)
				{
					Main.MenuUI.SetState(new ErrorUI(Language.GetTextValue(
						"Mods.AdvancedWorldGen.InvalidSizes.TooBigFromRAM", newSizeX, newSizeY)));
					return;
				}
			}

			if (WorldgenSettings.Revamped)
			{
				if (WorldSettings.SizeX < KnownLimits.OverhauledMinX)
				{
					Main.MenuUI.SetState(new ErrorUI(Language.GetTextValue(
						"Mods.AdvancedWorldGen.InvalidSizes.OverhauledMinX", KnownLimits.OverhauledMinX)));
					return;
				}

				if (WorldSettings.SizeY < KnownLimits.OverhauledMinY)
				{
					Main.MenuUI.SetState(new ErrorUI(Language.GetTextValue(
						"Mods.AdvancedWorldGen.InvalidSizes.OverhauledMinY", KnownLimits.OverhauledMinY)));
					return;
				}
			}
			else
			{
				if (WorldSettings.SizeX < KnownLimits.NormalMinX)
				{
					Main.MenuUI.SetState(
						new ErrorUI(Language.GetTextValue("Mods.AdvancedWorldGen.InvalidSizes.NormalMinX")));
					return;
				}

				if (WorldSettings.SizeY > KnownLimits.ComfortNormalMaxX)
				{
					Main.MenuUI.SetState(new ErrorUI(Language.GetTextValue(
						"Mods.AdvancedWorldGen.InvalidSizes.ComfortNormalMaxX")));
					return;
				}
			}
#endif

			Main.MenuUI.SetState(Base.AdvancedWorldGenMod.Instance.UiChanger.OptionsSelector);
		}
	}
}