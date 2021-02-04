using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AdvancedSeedGen.SeedUI
{
	public class ClickableText : UIText
	{
		private bool _activated;
		public SeedSelector SeedSelector;

		public ClickableText(SeedSelector seedSelector, string text) : base(text, 0.5f, true)
		{
			SeedSelector = seedSelector;
			HAlign = 0.5f;
			OnClick += EventOnClick;
			UpdateColor();
		}

		public bool Activated
		{
			get => _activated;
			set
			{
				_activated = value;
				SeedSelector.Actualize();
			}
		}

		public void UpdateColor()
		{
			TextColor = Activated ? Color.LightGreen : Color.Red;
		}

		public void EventOnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			Activated = !Activated;
			UpdateColor();
			Recalculate();
		}
	}
}