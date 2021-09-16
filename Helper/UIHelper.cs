using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.UI;

namespace AdvancedWorldGen.Helper
{
	public static class UIHelper
	{
		public static void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			UIPanel panel = (UIPanel) evt.Target;
			panel.BackgroundColor = new Color(73, 94, 171);
			panel.BorderColor = Colors.FancyUIFatButtonMouseOver;
		}

		public static void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			UIPanel panel = (UIPanel) evt.Target;
			panel.BackgroundColor = new Color(63, 82, 151) * 0.7f;
			panel.BorderColor = Color.Black;
		}

		public static UIElement.MouseEvent GoTo(UIState uiState, bool open = true)
		{
			int soundId = open ? SoundID.MenuOpen : SoundID.MenuClose;
			return (_, _) =>
			{
				SoundEngine.PlaySound(soundId);
				Main.MenuUI.SetState(uiState);
			};
		}
	}
}