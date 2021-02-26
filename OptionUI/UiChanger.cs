using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
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
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.Utilities;
using OnUIWorldLoad = On.Terraria.GameContent.UI.States.UIWorldLoad;
using OnUIWorldCreation = On.Terraria.GameContent.UI.States.UIWorldCreation;
using OnWorldGen = On.Terraria.WorldGen;
using UIWorldListItem = On.Terraria.GameContent.UI.Elements.UIWorldListItem;

namespace AdvancedWorldGen.OptionUI
{
	public class UiChanger
	{
		public Asset<Texture2D> CopyOptionsTexture;
		public UIText Description;
		public OptionsSelector OptionsSelector;
		public Asset<Texture2D> OptionsTexture;
		public Thread Thread;
		public UIWorldCreation UiWorldCreation;

		public void AddCancel(OnUIWorldLoad.orig_ctor orig, UIWorldLoad self)
		{
			orig(self);
			if (!Main.dedServ)
			{
				UITextPanel<string> uiTextPanel = new UITextPanel<string>("");
				self.Append(uiTextPanel);
				uiTextPanel.VAlign = 0.75f;
				uiTextPanel.HAlign = 0.5f;
				uiTextPanel.SetText("Abort");
				uiTextPanel.Recalculate();
				uiTextPanel.OnClick += UiTextPanelOnOnClick;
			}
		}

		public void ThreadifyWorldGen(OnWorldGen.orig_worldGenCallback orig, object threadContext)
		{
			if (!Main.dedServ)
			{
				Thread = new Thread(() => { orig(threadContext); });
				Thread.Start();
			}
			else
			{
				orig(threadContext);
			}
		}

		public void UiTextPanelOnOnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			Thread.Abort();
			Main.MenuUI.SetState(UiWorldCreation);
		}

		public void TweakWorldGenUi(OnUIWorldCreation.orig_AddDescriptionPanel origAddDescriptionPanel,
			UIWorldCreation self, UIElement container, float accumulatedHeight, string tagGroup)
		{
			origAddDescriptionPanel(self, container, accumulatedHeight, tagGroup);
			UiWorldCreation = self;
			FieldInfo fieldInfo = typeof(UIWorldCreation).GetField("_seedPlate", BindingFlags.NonPublic |
				BindingFlags.Instance);
			UICharacterNameButton characterNameButton = (UICharacterNameButton) fieldInfo.GetValue(self);
			characterNameButton.Width.Pixels -= 48;

			GroupOptionButton<bool> groupOptionButton = new GroupOptionButton<bool>(true, null, null, Color.White, null)
			{
				Width = new StyleDimension(40f, 0f),
				Height = new StyleDimension(40f, 0f),
				Top = characterNameButton.Top,
				Left = new StyleDimension(-128f, 1f),
				ShowHighlightWhenSelected = false,
				PaddingTop = 4f,
				PaddingLeft = 4f
			};

			fieldInfo = typeof(GroupOptionButton<bool>).GetField("_iconTexture", BindingFlags.NonPublic |
				BindingFlags.Instance);
			fieldInfo.SetValue(groupOptionButton, OptionsTexture);

			fieldInfo = typeof(UIWorldCreation).GetField("_descriptionText", BindingFlags.NonPublic |
			                                                                 BindingFlags.Instance);
			Description = (UIText) fieldInfo.GetValue(self);

			groupOptionButton.OnMouseDown += ToOptionsMenu;
			groupOptionButton.OnMouseOver += ShowOptionDescription;
			groupOptionButton.OnMouseOut += self.ClearOptionDescription;

			container.Append(groupOptionButton);

			ModifiedWorld.OptionHelper.Options.Clear();
			OptionsSelector = new OptionsSelector(self);
		}

		public void ToOptionsMenu(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuOpen);
			Main.MenuUI.SetState(OptionsSelector);
		}

		public void ShowOptionDescription(UIMouseEvent evt, UIElement listeningElement)
		{
			Description.SetText("Choose your world generation settings");
		}

		public static void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			((UIPanel) evt.Target).BackgroundColor = new Color(73, 94, 171);
			((UIPanel) evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
		}

		public static void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			((UIPanel) evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.7f;
			((UIPanel) evt.Target).BorderColor = Color.Black;
		}

		public void CopySettingsButton(UIWorldListItem.orig_ctor orig,
			Terraria.GameContent.UI.Elements.UIWorldListItem self, WorldFileData data, int orderInList,
			bool canBePlayed)
		{
			orig(self, data, orderInList, canBePlayed);

			FieldInfo fieldInfo = typeof(Terraria.GameContent.UI.Elements.UIWorldListItem).GetField("_buttonLabel",
				BindingFlags.NonPublic |
				BindingFlags.Instance);
			UIText uiText = (UIText) fieldInfo.GetValue(self);
			UIImageButton copyOptionButton = new UIImageButton(CopyOptionsTexture)
			{
				VAlign = 1f,
				Left = new StyleDimension(uiText.Left.Pixels - 4, 0f),
				PaddingTop = 4f,
				PaddingLeft = 4f
			};
			
			List<string> options = GetOptionsFromData(data);

			options = SetupCopyButton(copyOptionButton, options, uiText);

			uiText.Left.Pixels += 24f;
			self.Append(copyOptionButton);

			data.DrunkWorld = data.DrunkWorld || options.Contains("Crimruption");
		}

		private static List<string> GetOptionsFromData(WorldFileData data)
		{
			string path = Path.ChangeExtension(data.Path, ".twld");
			byte[] buf = FileUtilities.ReadAllBytes(path, data.IsCloudSave);
			TagCompound tags = TagIO.FromStream(new MemoryStream(buf));
			IList<TagCompound> modTags = tags.GetList<TagCompound>("modData");
			List<string> options =
				(from tagCompound in modTags
					where tagCompound.GetString("mod") == "AdvancedWorldGen"
					select (List<string>) tagCompound.GetCompound("data")?.GetList<string>("Options"))
				.FirstOrDefault() ??
				new List<string>();
			return options;
		}

		private static List<string> SetupCopyButton(UIImageButton copyOptionButton, List<string> options, UIText uiText)
		{
			copyOptionButton.OnMouseOver += delegate
			{
				if (options.Count == 0)
				{
					uiText.SetText("World without options");
					return;
				}

				string text = "";
				bool tooMuch = false;
				for (int index = 0; index < options.Count && !tooMuch; index++)
				{
					string option = Language.GetTextValue("Mods.AdvancedWorldGen." + options[index]);
					if (text != "") text += ", ";

					text += option;
					if (text.Length > 40)
					{
						text = text.Substring(0, 35) + "[...]";
						tooMuch = true;
					}
				}

				uiText.SetText("Copy settings \"" + text + "\"");
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
				uiText.SetText("Settings copied to clipboard");
			};
			copyOptionButton.OnMouseOut += delegate { uiText.SetText(""); };
			return options;
		}
	}
}