using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace AdvancedSeedGen.SeedUI
{
	public class SeedSelector : UIState
	{
		public List<ClickableText> TextArray;
		public UIVirtualKeyboard UiVirtualKeyboard;

		public SeedSelector(UIVirtualKeyboard uiVirtualKeyboard)
		{
			UiVirtualKeyboard = uiVirtualKeyboard;
		}

		public static void AddSeedInterface(On.Terraria.GameContent.UI.States.UIVirtualKeyboard.orig_ctor orig,
			UIVirtualKeyboard self, string labelText, string startingText,
			UIVirtualKeyboard.KeyboardSubmitEvent submitAction, Action cancelAction,
			int inputMode, bool allowEmpty)
		{
			orig(self, labelText, startingText, submitAction, cancelAction, inputMode, allowEmpty);
			if (labelText != Language.GetTextValue("UI.EnterSeed")) return;

			FieldInfo fieldInfo = typeof(UIVirtualKeyboard).GetField("_textBox", BindingFlags.NonPublic |
				BindingFlags.Instance);
			UITextBox buttonBox = (UITextBox) fieldInfo.GetValue(self);
			buttonBox.SetTextMaxLength(50);
			new SeedSelector(self).CreateSeedPanel();
		}

		public void CreateSeedPanel()
		{
			FieldInfo fieldInfo = typeof(UIVirtualKeyboard).GetField("outerLayer1", BindingFlags.NonPublic |
				BindingFlags.Instance);
			UIElement buttonBox = (UIElement) fieldInfo.GetValue(UiVirtualKeyboard);
			UIPanel uiPanel = new UIPanel
			{
				HAlign = 0.5f,
				Top =
				{
					Pixels = buttonBox.Top.Pixels + buttonBox.Height.Pixels
				},
				BackgroundColor = UICommon.MainPanelBackground
			};
			UiVirtualKeyboard.Append(uiPanel);

			uiPanel.Height.Pixels += uiPanel.PaddingTop * 2;

			UIText uiText = new UIText("Seed options", 0.75f, true);
			uiText.Width = uiText.MinWidth;
			uiText.Height = uiText.MinHeight;
			uiText.HAlign = 0.5f;
			uiPanel.Append(uiText);
			uiPanel.Height.Pixels += uiText.Height.Pixels;
			uiPanel.Width.Pixels = uiText.Width.Pixels;

			CreateSelectableOptions(uiPanel, uiText);

			uiPanel.Width.Pixels += uiPanel.PaddingLeft + uiPanel.PaddingRight;
			uiPanel.Recalculate();
		}

		private void CreateSelectableOptions(UIElement uiPanel, UIElement uiText)
		{
			List<string> options = SeedHelper.GetWorldOptions();

			TextArray = new List<ClickableText>();
			for (int i = 0; i < options.Count; i++)
			{
				string s = options[i];
				ClickableText clickableText = new ClickableText(this, s);
				clickableText.Width = clickableText.MinWidth;
				clickableText.Height = clickableText.MinHeight;
				if (i != 0)
				{
					ClickableText previousClickableText = TextArray[i - 1];
					clickableText.Top.Pixels =
						previousClickableText.Top.Pixels + previousClickableText.Height.Pixels * 1.5f;
					uiPanel.Height.Pixels +=
						previousClickableText.Height.Pixels * 0.25f + clickableText.Height.Pixels * 0.25f;
				}
				else
				{
					clickableText.Top.Pixels =
						uiText.Top.Pixels + uiText.Height.Pixels * 1.5f;
					uiPanel.Height.Pixels +=
						uiText.Height.Pixels * 0.25f + clickableText.Height.Pixels * 0.25f;
				}

				uiPanel.Height.Pixels += clickableText.Height.Pixels;
				uiPanel.Append(clickableText);
				uiPanel.Width.Pixels = Math.Max(uiPanel.Width.Pixels, clickableText.Width.Pixels);

				TextArray.Add(clickableText);
			}
		}

		public void Actualize()
		{
			List<string> selectedOptions = new List<string>();
			foreach (ClickableText clickableText in TextArray)
				if (clickableText.Activated)
					selectedOptions.Add(clickableText.Text);

			int unsatisfied = selectedOptions.Count;
			UiVirtualKeyboard.Text = "";

			while (unsatisfied != 0)
			{
				string selectedSeed = "";
				int bestSeed = 0;
				foreach (KeyValuePair<string, List<string>> keyValuePair in SeedHelper.SeedTranslator)
					if (keyValuePair.Value.All(s => selectedOptions.Contains(s)))
					{
						int currentSeed = keyValuePair.Value.Count;
						if (currentSeed == unsatisfied)
						{
							if (UiVirtualKeyboard.Text == "")
								UiVirtualKeyboard.Text = keyValuePair.Key;
							else
								UiVirtualKeyboard.Text += "," + keyValuePair.Key;

							return;
						}

						if (currentSeed > bestSeed)
						{
							bestSeed = currentSeed;
							selectedSeed = keyValuePair.Key;
						}
					}

				unsatisfied -= bestSeed;
				if (UiVirtualKeyboard.Text == "")
					UiVirtualKeyboard.Text = selectedSeed;
				else
					UiVirtualKeyboard.Text += "," + selectedSeed;

				foreach (string s in SeedHelper.SeedTranslator[selectedSeed]) selectedOptions.Remove(s);
			}
		}
	}
}