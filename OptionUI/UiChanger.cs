using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.UI;
using OnUIWorldLoad = On.Terraria.GameContent.UI.States.UIWorldLoad;
using OnUIWorldCreation = On.Terraria.GameContent.UI.States.UIWorldCreation;
using OnWorldGen = On.Terraria.WorldGen;

namespace AdvancedWorldGen.SeedUI
{
	public class UiChanger
	{
		public UIText Description;
		public OptionsSelector OptionsSelector;
		public Asset<Texture2D> OptionsTexture;
		public Thread Thread;
		public UIWorldCreation UiWorldCreation;

		public void AddCancel(OnUIWorldLoad.orig_ctor orig, UIWorldLoad self)
		{
			orig(self);
			UITextPanel<string> uiTextPanel = new UITextPanel<string>("");
			self.Append(uiTextPanel);
			uiTextPanel.VAlign = 0.75f;
			uiTextPanel.HAlign = 0.5f;
			uiTextPanel.SetText("Abort");
			uiTextPanel.Recalculate();
			uiTextPanel.OnClick += UiTextPanelOnOnClick;
		}

		public void ThreadifyWorldGen(OnWorldGen.orig_worldGenCallback orig, object threadContext)
		{
			Thread = new Thread(() => { orig(threadContext); });
			Thread.Start();
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
				HAlign = 0f,
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

			SeedHelper seedHelper = new SeedHelper(new List<string>());
			CustomSeededWorld.GetCurrentWorld().SeedHelper = seedHelper;
			OptionsSelector = new OptionsSelector(self, seedHelper);
		}

		private void ToOptionsMenu(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.MenuUI.SetState(OptionsSelector);
		}

		private void ShowOptionDescription(UIMouseEvent evt, UIElement listeningElement)
		{
			Description.SetText("Choose your world generation settings");
		}
	}
}