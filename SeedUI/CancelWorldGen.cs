using System.Threading;
using On.Terraria;
using On.Terraria.GameContent.UI.States;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.World.Generation;

namespace AdvancedSeedGen.SeedUI
{
	public class CancelWorldGen
	{
		public Thread Thread;
		public UIGenProgressBar UiGenProgressBar;

		public void AddCancel(UIWorldLoad.orig_ctor orig, Terraria.GameContent.UI.States.UIWorldLoad self,
			GenerationProgress progress)
		{
			orig(self, progress);
			UITextPanel<string> uiTextPanel = new UITextPanel<string>("");
			self.Append(uiTextPanel);
			uiTextPanel.Top.Pixels = UiGenProgressBar.Top.Pixels + 50 + UiGenProgressBar.Height.Pixels;
			uiTextPanel.Left.Pixels = UiGenProgressBar.Left.Pixels - uiTextPanel.MarginLeft;
			uiTextPanel.HAlign = UiGenProgressBar.HAlign;
			uiTextPanel.VAlign = UiGenProgressBar.VAlign;
			uiTextPanel.SetText("Abort");
			uiTextPanel.Recalculate();
			uiTextPanel.OnClick += UiTextPanelOnOnClick;
		}

		public void WorldFileDataOnSetSeed(Main.orig_OnSeedSelected origOnSeedSelected, Terraria.Main main,
			string seedText)
		{
			seedText = SeedHelper.TweakSeedText(seedText);
			Thread = new Thread(() => { origOnSeedSelected(main, seedText); });
			Thread.Start();
		}

		public void UiTextPanelOnOnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			Thread.Abort();
			Terraria.Main.menuMode = 5000;
		}

		public void StealProgressBar(On.Terraria.UI.UIElement.orig_Append orig, UIElement self, UIElement element)
		{
			if (self.GetType() == typeof(Terraria.GameContent.UI.States.UIWorldLoad) &&
			    element.GetType() == typeof(UIGenProgressBar))
				UiGenProgressBar = (UIGenProgressBar) element;
			orig(self, element);
		}
	}
}