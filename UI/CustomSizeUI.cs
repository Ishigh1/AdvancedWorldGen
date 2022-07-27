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
		int index = 0;

		const string localizationPath = "Mods.AdvancedWorldGen.UI.CustomSizes";

		NumberTextBox<int> sizeXInput =
			new ConfigNumberTextBox<int>(WorldSettings.Params, nameof(Params.SizeX), 100, ushort.MaxValue, localizationPath);
		sizeXInput.Order = index++;
		uiList.Add(sizeXInput);

		NumberTextBox<int> sizeYInput =
			new ConfigNumberTextBox<int>(WorldSettings.Params, nameof(Params.SizeY), 100, ushort.MaxValue, localizationPath);
		sizeYInput.Order = index++;
		uiList.Add(sizeYInput);

		NumberTextBox<float> templeModifier =
			new ConfigNumberTextBox<float>(WorldSettings.Params, nameof(Params.TempleMultiplier), 0,
				float.PositiveInfinity, localizationPath);
		templeModifier.Order = index++;
		uiList.Add(templeModifier);

		if (WorldgenSettings.Revamped)
		{
			NumberTextBox<float> dungeonModifier =
				new ConfigNumberTextBox<float>(WorldSettings.Params, nameof(Params.DungeonMultiplier), 0,
					float.MaxValue, localizationPath);
			dungeonModifier.Order = index++;
			uiList.Add(dungeonModifier);

			NumberTextBox<float> beachModifier = new ConfigNumberTextBox<float>(WorldSettings.Params,
				nameof(Params.BeachMultiplier), 0,
				float.PositiveInfinity, localizationPath);
			beachModifier.Order = index++;
			uiList.Add(beachModifier);

			TileExpandableList copperList = new(WorldSettings.Params, nameof(Params.Copper), false,
				TileExpandableList.Random, TileID.Copper, TileID.Tin)
			{
				Order = index++
			};
			uiList.Add(copperList);

			TileExpandableList ironList = new(WorldSettings.Params, nameof(Params.Iron), false,
				TileExpandableList.Random, TileID.Iron, TileID.Lead)
			{
				Order = index++
			};
			uiList.Add(ironList);

			TileExpandableList silverList = new(WorldSettings.Params, nameof(Params.Silver), false,
				TileExpandableList.Random, TileID.Silver, TileID.Tungsten)
			{
				Order = index++
			};
			uiList.Add(silverList);

			TileExpandableList goldList = new(WorldSettings.Params, nameof(Params.Gold), false,
				TileExpandableList.Random, TileID.Gold, TileID.Platinum)
			{
				Order = index++
			};
			uiList.Add(goldList);
		}

		UITextPanel<string> gotoConfig = new(Language.GetTextValue("Mods.AdvancedWorldGen.UI.Config"))
		{
			Width = new StyleDimension(0f, 1f)
		};
		uiList.Add(gotoConfig);

		gotoConfig.OnMouseDown += ConfigWorldGen;
		gotoConfig.OnMouseOver += UiChanger.FadedMouseOver;
		gotoConfig.OnMouseOut += UiChanger.FadedMouseOut;

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

	public static void ConfigWorldGen(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(SoundID.MenuOpen);
		Main.MenuUI.SetState(AdvancedWorldGenMod.Instance.UiChanger.WorldGenConfigurator);
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
		UIState? Prev()
		{
			return new CustomSizeUI();
		}

		UIState? Next()
		{
			return AdvancedWorldGenMod.Instance.UiChanger.OptionsSelector;
		}

		int oldSizeX = Main.tile.Width;
		int oldSizeY = Main.tile.Height;
		if (oldSizeX < WorldSettings.Params.SizeX || oldSizeY < WorldSettings.Params.SizeY)
		{
			int newSizeX = Math.Max(WorldSettings.Params.SizeX, 8400);
			int newSizeY = Math.Max(WorldSettings.Params.SizeY, 2100);

			if (KnownLimits.WillCrashMissingEwe(newSizeX, newSizeY))
			{
				Main.MenuUI.SetState(new WarningUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.TooBigFromRAM", newSizeX, newSizeY), Prev, Next));
				return;
			}
		}

		if (WorldgenSettings.Revamped)
		{
			if (WorldSettings.Params.SizeX < KnownLimits.OverhauledMinX)
			{
				Main.MenuUI.SetState(new WarningUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.OverhauledMinX", KnownLimits.OverhauledMinX), Prev, Next));
				return;
			}

			if (WorldSettings.Params.SizeY < KnownLimits.OverhauledMinY)
			{
				Main.MenuUI.SetState(new WarningUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.OverhauledMinY", KnownLimits.OverhauledMinY), Prev, Next));
				return;
			}
		}
		else
		{
			if (WorldSettings.Params.SizeX < KnownLimits.NormalMinX)
			{
				Main.MenuUI.SetState(new WarningUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.NormalMinX", KnownLimits.NormalMinX), Prev, Next));
				return;
			}

			if (WorldSettings.Params.SizeY > KnownLimits.ComfortNormalMaxX)
			{
				Main.MenuUI.SetState(new WarningUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.ComfortNormalMaxX"), Prev, Next));
				return;
			}
		}
#endif

		Main.MenuUI.SetState(AdvancedWorldGenMod.Instance.UiChanger.OptionsSelector);
	}
}