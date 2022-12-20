namespace AdvancedWorldGen.UI;

public class CustomSizeUI : UIState
{
	public WorldSettings WorldSettings;

	public CustomSizeUI()
	{
		WorldSettings = OptionHelper.WorldSettings;
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
			new ConfigNumberTextBox<int>(nameof(Params.SizeX), 100, ushort.MaxValue, localizationPath);
		sizeXInput.Order = index++;
		uiList.Add(sizeXInput);

		NumberTextBox<int> sizeYInput =
			new ConfigNumberTextBox<int>(nameof(Params.SizeY), 100, ushort.MaxValue, localizationPath);
		sizeYInput.Order = index++;
		uiList.Add(sizeYInput);

		NumberTextBox<float> templeModifier =
			new ConfigNumberTextBox<float>(nameof(Params.TempleMultiplier), 0,
				float.PositiveInfinity, localizationPath);
		templeModifier.Order = index++;
		uiList.Add(templeModifier);

		if (WorldgenSettings.Revamped)
		{
			NumberTextBox<float> dungeonModifier =
				new ConfigNumberTextBox<float>(nameof(Params.DungeonMultiplier), 0,
					float.MaxValue, localizationPath);
			dungeonModifier.Order = index++;
			uiList.Add(dungeonModifier);

			NumberTextBox<float> beachModifier = new ConfigNumberTextBox<float>(nameof(Params.BeachMultiplier), 0,
				float.PositiveInfinity, localizationPath);
			beachModifier.Order = index++;
			uiList.Add(beachModifier);

			TileExpandableList copperList = new(nameof(Params.Copper), localizationPath, false,
				TileExpandableList.Random, TileID.Copper, TileID.Tin)
			{
				Order = index++
			};
			uiList.Add(copperList);

			TileExpandableList ironList = new(nameof(Params.Iron), localizationPath, false,
				TileExpandableList.Random, TileID.Iron, TileID.Lead)
			{
				Order = index++
			};
			uiList.Add(ironList);

			TileExpandableList silverList = new(nameof(Params.Silver), localizationPath, false,
				TileExpandableList.Random, TileID.Silver, TileID.Tungsten)
			{
				Order = index++
			};
			uiList.Add(silverList);

			TileExpandableList goldList = new(nameof(Params.Gold), localizationPath, false,
				TileExpandableList.Random, TileID.Gold, TileID.Platinum)
			{
				Order = index++
			};
			uiList.Add(goldList);

			BooleanExpandableList beachList = new(nameof(Params.ScaledBeaches), localizationPath)
			{
				Order = index++
			};
			uiList.Add(beachList);

			BooleanExpandableList terrainList = new(nameof(Params.EditTerrainPass), localizationPath)
			{
				Order = index++
			};
			uiList.Add(terrainList);

			EnumInputListBox<TerrainType> terrainTypeList = new(nameof(Params.TerrainType), localizationPath)
			{
				Order = index++
			};
			uiList.Add(terrainTypeList);
		}
		else
		{
			UITextPanel<string> overhauledDisabled =
				new(Language.GetTextValue("Mods.AdvancedWorldGen.UI.OverhauledDisabled"))
				{
					Width = new StyleDimension(0f, 1f)
				};
			uiList.Add(overhauledDisabled);
		}

		UITextPanel<string> goToVanillaConfig = new(Language.GetTextValue("Mods.AdvancedWorldGen.UI.VanillaConfig"))
		{
			Width = new StyleDimension(0f, 1f)
		};
		uiList.Add(goToVanillaConfig);

		goToVanillaConfig.OnLeftClick += ConfigVanillaWorldGen;
		goToVanillaConfig.OnMouseOver += UiChanger.FadedMouseOver;
		goToVanillaConfig.OnMouseOut += UiChanger.FadedMouseOut;

		UITextPanel<string> goToOverhauledConfig =
			new(Language.GetTextValue("Mods.AdvancedWorldGen.UI.OverhauledConfig"))
			{
				Width = new StyleDimension(0f, 1f)
			};
		uiList.Add(goToOverhauledConfig);

		goToOverhauledConfig.OnLeftClick += ConfigOverhauledWorldGen;
		goToOverhauledConfig.OnMouseOver += UiChanger.FadedMouseOver;
		goToOverhauledConfig.OnMouseOut += UiChanger.FadedMouseOut;

		UITextPanel<string> goBack = new(Language.GetTextValue("UI.Back"))
		{
			Width = new StyleDimension(0f, 0.1f),
			Top = new StyleDimension(0f, 0.75f),
			HAlign = 0.5f
		};
		goBack.OnLeftClick += GoBack;
		goBack.OnMouseOver += UiChanger.FadedMouseOver;
		goBack.OnMouseOut += UiChanger.FadedMouseOut;
		Append(goBack);
	}

	public static void ConfigVanillaWorldGen(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(SoundID.MenuOpen);
		Main.MenuUI.SetState(AdvancedWorldGenMod.Instance.UiChanger.VanillaWorldGenConfigurator);
	}

	private static void ConfigOverhauledWorldGen(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(SoundID.MenuOpen);
		Main.MenuUI.SetState(AdvancedWorldGenMod.Instance.UiChanger.OverhauledWorldGenConfigurator);
	}

	private void GoBack(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(SoundID.MenuClose);
		int size = Params.SizeX switch
		{
			4200 when Params.SizeY == 1200 => 0,
			6400 when Params.SizeY == 1800 => 1,
			8400 when Params.SizeY == 2400 => 2,
			_ => -1
		};

		FieldAccessor<int> optionSize = VanillaInterface.OptionSize(WorldSettings.UIWorldCreation);
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
		if (oldSizeX < Params.SizeX || oldSizeY < Params.SizeY)
		{
			int newSizeX = Math.Max(Params.SizeX, 8400);
			int newSizeY = Math.Max(Params.SizeY, 2100);

			if (KnownLimits.WillCrashMissingEwe(newSizeX, newSizeY))
			{
				Main.MenuUI.SetState(new WarningUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.TooBigFromRAM", newSizeX, newSizeY), Prev, Next));
				return;
			}
		}

		if (WorldgenSettings.Revamped)
		{
			if (Params.SizeX < KnownLimits.OverhauledMinX)
			{
				Main.MenuUI.SetState(new WarningUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.OverhauledMinX", KnownLimits.OverhauledMinX), Prev, Next));
				return;
			}

			if (Params.SizeY < KnownLimits.OverhauledMinY)
			{
				Main.MenuUI.SetState(new WarningUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.OverhauledMinY", KnownLimits.OverhauledMinY), Prev, Next));
				return;
			}
		}
		else
		{
			if (Params.SizeX < KnownLimits.NormalMinX)
			{
				Main.MenuUI.SetState(new WarningUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.NormalMinX", KnownLimits.NormalMinX), Prev, Next));
				return;
			}

			if (Params.SizeY > KnownLimits.NormalMaxX)
			{
				Main.MenuUI.SetState(new WarningUI(Language.GetTextValue(
					"Mods.AdvancedWorldGen.InvalidSizes.NormalMaxX"), Prev, Next));
				return;
			}

			if (Params.SizeY > KnownLimits.ComfortNormalMaxX)
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