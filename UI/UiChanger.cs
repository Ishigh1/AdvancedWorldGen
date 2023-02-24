namespace AdvancedWorldGen.UI;

public class UiChanger
{
	public static Asset<Texture2D> CopyOptionsTexture = null!;
	private readonly Asset<Texture2D> OptionsTexture;
	private UIText Description = null!;
	public OptionsSelector OptionsSelector = null!;
	public OverhauledWorldGenConfigurator? OverhauledWorldGenConfigurator;
	private GenerationProgress? Progress;
	public Stopwatch Stopwatch;
	private Thread Thread = null!;
	public VanillaWorldGenConfigurator? VanillaWorldGenConfigurator;

	public UiChanger(Mod mod)
	{
		if (!Main.dedServ)
		{
			OptionsTexture = mod.Assets.Request<Texture2D>("Images/WorldOptions");
			CopyOptionsTexture = mod.Assets.Request<Texture2D>("Images/CopyWorldButton");
		}
		else
		{
			OptionsTexture = null!;
		}
	}

	public void ThreadifyWorldGen(On_WorldGen.orig_do_worldGenCallBack orig, object? threadContext)
	{
		if (!Main.dedServ)
		{
			threadContext ??= new GenerationProgress();
			Progress = (GenerationProgress)threadContext;
			Thread = new Thread(() =>
			{
				try
				{
					orig(threadContext);
				}
				catch (Exception)
				{
					if (Main.tile.Width > 0) EmergencySaving("Failed");
				}
			}) { Name = "WorldGen" };
			Thread.Start();
		}
		else
		{
			orig(threadContext);
		}
	}

	private static void EmergencySaving(string suffix)
	{
		if (WorldgenSettings.Instance.SaveOnFail)
		{
			Main.WorldFileMetadata = FileMetadata.FromCurrentSettings(FileType.World);
			Main.worldName += "_" + suffix;
			if (Main.spawnTileX == 0)
			{
				Main.spawnTileX = Main.maxTilesX / 2;
				Main.spawnTileY = Main.maxTilesY / 2;
			}

			WorldFile.SaveWorld();
		}
	}

	public void AddCancel(On_UIWorldLoad.orig_ctor orig, UIWorldLoad self)
	{
		orig(self);
		if (!Main.dedServ)
		{
			Stopwatch = new Stopwatch();
			UITextPanel<string> timer = new("");
			self.Append(timer);
			timer.VAlign = 0.7f;
			timer.HAlign = 0.5f;
			timer.OnUpdate += _ =>
			{
				if (Progress is { TotalProgress: > 0 } && Stopwatch.ElapsedMilliseconds > 500)
					timer.SetText(Language.GetTextValue("Mods.AdvancedWorldGen.Timer",
						(Stopwatch.Elapsed * (1 / Progress.TotalProgress - 1)).Humanize(2, minUnit: TimeUnit.Second)));
			};
			timer.Recalculate();

			UITextPanel<string> uiTextPanel = new("");
			self.Append(uiTextPanel);
			uiTextPanel.VAlign = 0.75f;
			uiTextPanel.HAlign = 0.5f;
			uiTextPanel.SetText(Language.GetTextValue("Mods.AdvancedWorldGen.Abort"));
			uiTextPanel.Recalculate();
			uiTextPanel.OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
			uiTextPanel.OnMouseOut += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
			uiTextPanel.OnLeftClick += Abort;
		}

		Stopwatch.Start();
	}

	public void Abort(UIMouseEvent evt, UIElement listeningElement)
	{
		AdvancedWorldGenMod.Instance.Logger.Info("Worldgen aborted, ignore any exception below");
		SoundEngine.PlaySound(SoundID.MenuClose);
		Tilemap tiles = Main.tile;
		Main.tile = new Tilemap();
		WorldGen._genRand = null;
		Thread.Join();
		Main.tile = tiles;
		AdvancedWorldGenMod.Instance.Logger.Info("End of abortion, exceptions are now relevant again");

		EmergencySaving("Aborted");
	}

	public void TweakWorldGenUi(On_UIWorldCreation.orig_AddDescriptionPanel origAddDescriptionPanel,
		UIWorldCreation self, UIElement container, float accumulatedHeight, string tagGroup)
	{
		origAddDescriptionPanel(self, container, accumulatedHeight, tagGroup);

		UICharacterNameButton characterNameButton = VanillaInterface.SeedPlate(self).Value;
		characterNameButton.Width.Pixels -= 48;

		GroupOptionButton<bool> groupOptionButton = new(true, null, null, Color.White, null)
		{
			Width = new StyleDimension(40f, 0f),
			Height = new StyleDimension(40f, 0f),
			Top = characterNameButton.Top,
			Left = new StyleDimension(-128f, 1f),
			ShowHighlightWhenSelected = false,
			PaddingTop = 4f,
			PaddingLeft = 4f
		};

		VanillaInterface.IconTexture(groupOptionButton).Value = OptionsTexture;

		Description = VanillaInterface.DescriptionText(self).Value;

		groupOptionButton.OnLeftClick += ToOptionsMenu;
		groupOptionButton.OnMouseOver += ShowOptionDescription;
		groupOptionButton.OnMouseOut += self.ClearOptionDescription;

		container.Append(groupOptionButton);

		OptionHelper.ClearAll();
		OptionsSelector = new OptionsSelector(self, null);
	}

	public void ToOptionsMenu(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(SoundID.MenuOpen);
		Main.MenuUI.SetState(OptionsSelector);
	}

	public void ShowOptionDescription(UIMouseEvent evt, UIElement listeningElement)
	{
		Description.SetText(Language.GetTextValue("Mods.AdvancedWorldGen.OptionButton"));
	}

	public static void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(SoundID.MenuTick);
		UIPanel panel = (UIPanel)evt.Target;
		panel.BackgroundColor = new Color(73, 94, 171);
		panel.BorderColor = Colors.FancyUIFatButtonMouseOver;
	}

	public static void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
	{
		UIPanel panel = (UIPanel)evt.Target;
		panel.BackgroundColor = new Color(63, 82, 151) * 0.7f;
		panel.BorderColor = Color.Black;
	}

	public static void CopySettingsButton(On_UIWorldListItem.orig_ctor orig,
		UIWorldListItem self, WorldFileData data, int orderInList,
		bool canBePlayed)
	{
		orig(self, data, orderInList, canBePlayed);

		UIText uiText = VanillaInterface.ButtonLabel(self).Value;
		UIImageButton copyOptionButton = new(CopyOptionsTexture)
		{
			VAlign = 1f,
			Left = new StyleDimension(uiText.Left.Pixels - 4, 0f),
			PaddingTop = 4f,
			PaddingLeft = 4f
		};

		HashSet<string> options = GetOptionsFromData(data);

		options = SetupCopyButton(copyOptionButton, options, uiText);

		uiText.Left.Pixels += 24f;
		self.Append(copyOptionButton);

		data.DrunkWorld = data.DrunkWorld || options.Contains("Drunk.Crimruption");
	}

	public static HashSet<string> GetOptionsFromData(WorldFileData data)
	{
		string path = Path.ChangeExtension(data.Path, ".twld");
		if (!FileUtilities.Exists(path, data.IsCloudSave))
			return new HashSet<string>();
		byte[] buf = FileUtilities.ReadAllBytes(path, data.IsCloudSave);
		TagCompound tags = TagIO.FromStream(new MemoryStream(buf));
		IList<TagCompound> modTags = tags.GetList<TagCompound>("modData");
		HashSet<string> options =
			(from tagCompound in modTags
				where tagCompound.GetString("mod") == "AdvancedWorldGen"
				select new HashSet<string>(tagCompound.GetCompound("data")?.GetList<string>("Options")))
			.FirstOrDefault() ??
			new HashSet<string>();
		return options;
	}

	public static HashSet<string> SetupCopyButton(UIImageButton copyOptionButton, HashSet<string> options,
		UIText uiText)
	{
		copyOptionButton.OnMouseOver += delegate
		{
			if (options.Count == 0)
			{
				uiText.SetText(Language.GetTextValue("Mods.AdvancedWorldGen.NoOptions"));
				return;
			}

			string text = Language.GetTextValue("Mods.AdvancedWorldGen.CopySettings") + " \"";
			bool first = true;
			foreach (string optionText in options.Select(option =>
				         Language.GetTextValue("Mods.AdvancedWorldGen." + option)))
			{
				if (first)
					first = false;
				else
					text += ", ";

				text += optionText;
				if (text.Length > 55)
				{
					text = string.Concat(text.AsSpan(0, 50), "[...]");
					break;
				}
			}

			uiText.SetText(text + "\"");
		};
		copyOptionButton.OnLeftClick += delegate
		{
			if (options.Count == 0) return;
			string text = "";
			foreach (string option in options)
			{
				if (text != "") text += "|";

				text += option;
			}

			Platform.Get<IClipboard>().Value = text;
			uiText.SetText(Language.GetTextValue("Mods.AdvancedWorldGen.CopiedSettings"));
		};
		copyOptionButton.OnMouseOut += delegate { uiText.SetText(""); };
		return options;
	}

	public static void SetSpecialName(On_WorldFileData.orig_SetWorldSize orig, WorldFileData self, int x, int y)
	{
		self.WorldSizeX = x;
		self.WorldSizeY = y;
		self._worldSizeName = (x, y) switch
		{
			(4200, 1200) => Language.GetText("UI.WorldSizeSmall"),
			(6400, 1800) => Language.GetText("UI.WorldSizeMedium"),
			(8400, 2400) => Language.GetText("UI.WorldSizeLarge"),
			_ => Language.GetText(Language.GetTextValue("Mods.AdvancedWorldGen.CustomSizedWorld", x, y))
		};
	}

#if SPECIALDEBUG
	public static void DeleteAllButLast()
	{
		UIWorldSelect uiWorldSelect = (UIWorldSelect)typeof(Main).GetField("_worldSelectMenu", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		uiWorldSelect.Initialize();
		UITextPanel<string> uiTextPanel = new(Language.GetTextValue("Mods.AdvancedWorldGen.DeleteWorlds"))
		{
			Left =
			{
				Pixels = 100
			},
			Top =
			{
				Pixels = 100
			}
		};
		uiTextPanel.OnLeftClick += (evt, element) =>
		{
			MethodInfo eraser = typeof(Main).GetMethod("EraseWorld", BindingFlags.Static | BindingFlags.NonPublic)!;
			int saved = -1;
			for (int index = 0; index < Main.WorldList.Count;)
			{
				WorldFileData worldFileData = Main.WorldList[index];
				if (!worldFileData.IsCloudSave && !worldFileData.IsFavorite)
				{
					if (saved == -1)
						saved = index++;
					else if (worldFileData.CreationTime > Main.WorldList[saved].CreationTime)
					{
						eraser.Invoke(null, new object?[] { saved });
						saved = index - 1;
					}
					else
						eraser.Invoke(null, new object?[] { index });
				}
				else
					index++;
			}

			MethodInfo refresher =
				typeof(UIWorldSelect).GetMethod("UpdateWorldsList", BindingFlags.Instance | BindingFlags.NonPublic)!;
			refresher.Invoke(uiWorldSelect, null);
		};
        uiTextPanel.OnMouseOver += FadedMouseOver;
        uiTextPanel.OnMouseOut += FadedMouseOut;

        uiWorldSelect.Append(uiTextPanel);
	}
#endif
}