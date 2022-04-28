using System;
using System.Linq;
using System.Reflection;
using AdvancedWorldGen.UI.InputUI;
using AdvancedWorldGen.UI.InputUI.List;
using AdvancedWorldGen.UI.InputUI.Number;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.UI;

public class WorldGenConfigurator : UIState
{
	public WorldGenConfiguration WorldGenConfiguration;

	public WorldGenConfigurator()
	{
		WorldGenConfiguration =
			WorldGenConfiguration.FromEmbeddedPath("Terraria.GameContent.WorldBuilding.Configuration.json");
		WorldGen.Hooks.ProcessWorldGenConfig(ref WorldGenConfiguration);
		WorldGen.Hooks.OnWorldGenConfigProcess += SetConfig;

		SetupUI();
	}

	public void Dispose()
	{
		WorldGen.Hooks.OnWorldGenConfigProcess -= SetConfig;
	}

	public void SetupUI()
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

		UIText uiTitle = new("Worldgen Config", 0.75f, true) { HAlign = 0.5f };
		uiTitle.Height = uiTitle.MinHeight;
		uiPanel.Append(uiTitle);
		uiPanel.Append(new UIHorizontalSeparator
		{
			Width = new StyleDimension(0f, 1f),
			Top = new StyleDimension(43f, 0f),
			Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
		});


		UIScrollbar uiScrollbar = new()
		{
			Height = new StyleDimension(-60f, 1f),
			Top = new StyleDimension(50, 0f),
			HAlign = 1f
		};
		UIList uiList = new()
		{
			Height = new StyleDimension(-60f, 1f),
			Width = new StyleDimension(-20f, 1f),
			Top = new StyleDimension(50, 0f)
		};
		uiList.SetScrollbar(uiScrollbar);
		uiPanel.Append(uiScrollbar);
		uiPanel.Append(uiList);

		int index = 0;

		foreach (JObject jObject in typeof(WorldGenConfiguration)
			         .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
			         .Select(fieldInfo => (JObject)fieldInfo.GetValue(WorldGenConfiguration)!))
			TransformJsonToUI(uiList, ref index, jObject);


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
		Recalculate();
	}

	public static void TransformJsonToUI(UIList uiPanel, ref int index, JToken jToken)
	{
		switch (jToken.Type)
		{
			case JTokenType.Object:
				JObject jObject = (JObject)jToken;
				foreach ((string? key, JToken? child) in jObject)
					if (child is not null)
						TransformJsonToUI(uiPanel, ref index, child);
				return;
			case JTokenType.Integer:
				//Create an long number text box.
				NumberTextBox<long> longInput = new JsonNumberTextBox<long>((JValue)jToken, 0, ushort.MaxValue)
				{
					Order = index++
				};
				uiPanel.Add(longInput);
				break;
			case JTokenType.Float:
				//Create a double number text box.
				NumberTextBox<double> doubleInput =
					new JsonNumberTextBox<double>((JValue)jToken, 0, double.PositiveInfinity)
					{
						Order = index++
					};
				uiPanel.Add(doubleInput);
				break;
			case JTokenType.String:
				//Create a world scaling text box.
				EnumInputListBox<WorldGenRange.ScalingMode> enumInput = new((JValue)jToken)
				{
					Order = index++
				};
				uiPanel.Add(enumInput);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void SetConfig(ref WorldGenConfiguration config)
	{
		config = WorldGenConfiguration;
	}

	public static void GoBack(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(SoundID.MenuClose);
		Main.MenuUI.SetState(new CustomSizeUI());
	}
}