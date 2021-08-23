using System;
using System.Reflection;
using AdvancedWorldGen.BetterVanillaWorldGen;
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
				.SetValue(intInputElement, 500000);
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

#if !WARNINGLESS
			int oldSizeX = Main.tile.GetLength(0);
			int oldSizeY = Main.tile.GetLength(1);
			if (oldSizeX < WorldSettings.SizeX || oldSizeY < WorldSettings.SizeY)
			{
				int newSizeX = Math.Max(WorldSettings.SizeX, oldSizeX);
				int newSizeY = Math.Max(WorldSettings.SizeY, oldSizeY);

				if ((long) newSizeX * newSizeY * 44 > GC.GetGCMemoryInfo().TotalAvailableMemoryBytes)
				{
					Main.MenuUI.SetState(new ErrorUI("A world with a size of " + newSizeX + " x " + newSizeY +
					                                 " is too big for this computer."));
					return;
				}
			}

			if (WorldgenSettings.Revamped)
			{
				if (WorldSettings.SizeX < KnownLimits.OverhauledMinX)
				{
					Main.MenuUI.SetState(new ErrorUI("The world is known to not be able to generate under x=" +
					                                 WorldSettings.SizeX));
					return;
				}

				if (WorldSettings.SizeY < KnownLimits.OverhauledMinY)
				{
					Main.MenuUI.SetState(new ErrorUI("The world is known to not be able to generate under y=" +
					                                 WorldSettings.SizeY));
					return;
				}
			}
			else
			{
				if (WorldSettings.SizeX < KnownLimits.NormalMinX)
				{
					Main.MenuUI.SetState(new ErrorUI(
						"You need to enable the overhauled worldgen in the mod settings" +
						"\nto be able to generate a world under x=" +
						WorldSettings.SizeX));
					return;
				}

				if (WorldSettings.SizeY > KnownLimits.ComfortNormalMaxX)
				{
					Main.MenuUI.SetState(new ErrorUI("The worldgen will be slow to generate a big world," +
					                                 "\nyou should maybe enable the overhauled worldgen in the mod settings"));
					return;
				}
			}
#endif

			Main.MenuUI.SetState(Base.AdvancedWorldGen.Instance.UiChanger.OptionsSelector);
		}
	}
}