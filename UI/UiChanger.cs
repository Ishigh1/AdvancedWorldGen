using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.BetterVanillaWorldGen;
using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
using Humanizer;
using Humanizer.Localisation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.OS;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using OnUIWorldLoad = On.Terraria.GameContent.UI.States.UIWorldLoad;
using OnUIWorldCreation = On.Terraria.GameContent.UI.States.UIWorldCreation;
using OnWorldGen = On.Terraria.WorldGen;
using OnUIWorldListItem = On.Terraria.GameContent.UI.Elements.UIWorldListItem;

namespace AdvancedWorldGen.UI;

public class UiChanger
{
	public static Asset<Texture2D> CopyOptionsTexture = null!;
	public readonly Asset<Texture2D> OptionsTexture;
	public UIText Description = null!;
	public OptionsSelector OptionsSelector = null!;
	private GenerationProgress? Progress;
	public Thread Thread = null!;
	public WorldGenConfigurator? WorldGenConfigurator;

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

	public void ThreadifyWorldGen(OnWorldGen.orig_do_worldGenCallBack orig, object? threadContext)
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

	public static void EmergencySaving(string suffix)
	{
		if (WorldgenSettings.AbortedSaving)
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

	public void AddCancel(OnUIWorldLoad.orig_ctor orig, UIWorldLoad self)
	{
		orig(self);
		if (!Main.dedServ)
		{
			Stopwatch stopwatch = new();
			stopwatch.Start();
			UITextPanel<string> timer = new("");
			self.Append(timer);
			timer.VAlign = 0.7f;
			timer.HAlign = 0.5f;
			timer.OnUpdate += _ =>
			{
				if (Progress != null && stopwatch.ElapsedMilliseconds > 500)
					timer.SetText(Language.GetTextValue("Mods.AdvancedWorldGen.Timer",
						(stopwatch.Elapsed * (1 / Progress.TotalProgress - 1)).Humanize(2, minUnit: TimeUnit.Second)));
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
			uiTextPanel.OnClick += Abort;
		}
	}

	public void Abort(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(SoundID.MenuClose);
		Tilemap tiles = Main.tile;
		Main.tile = new Tilemap();
		WorldGen._genRand = null;
		Thread.Join();
		Main.tile = tiles;

		EmergencySaving("Aborted");
	}

	public void TweakWorldGenUi(OnUIWorldCreation.orig_AddDescriptionPanel origAddDescriptionPanel,
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

		groupOptionButton.OnMouseDown += ToOptionsMenu;
		groupOptionButton.OnMouseOver += ShowOptionDescription;
		groupOptionButton.OnMouseOut += self.ClearOptionDescription;

		container.Append(groupOptionButton);

		ModifiedWorld.Instance.OptionHelper.ClearAll();
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

	public static void CopySettingsButton(OnUIWorldListItem.orig_ctor orig,
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

	public static HashSet<string> SetupCopyButton(UIImageButton copyOptionButton, HashSet<string> options, UIText uiText)
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
		copyOptionButton.OnMouseDown += delegate
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

	public static void SetSpecialName(On.Terraria.IO.WorldFileData.orig_SetWorldSize orig, WorldFileData self, int x, int y)
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
}