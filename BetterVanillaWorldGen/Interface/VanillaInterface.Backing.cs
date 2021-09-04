using AdvancedWorldGen.Helper;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface
{
	public partial class VanillaInterface
	{
		private static readonly VanillaAccessor<int> _optionSize =
			new(typeof(UIWorldCreation), "_optionSize", null);

		private static readonly VanillaAccessor<object[]> _sizeButtons =
			new(typeof(UIWorldCreation), "_sizeButtons", null);

		private static readonly VanillaAccessor<UICharacterNameButton> _seedPlate =
			new(typeof(UIWorldCreation), "_seedPlate", null);

		private static readonly VanillaAccessor<Asset<Texture2D>> _iconTexture =
			new(typeof(GroupOptionButton<bool>), "_iconTexture", null);

		private static readonly VanillaAccessor<UIText> _descriptionText =
			new(typeof(UIWorldCreation), "_descriptionText", null);

		private static readonly VanillaAccessor<UIText> _buttonLabel =
			new(typeof(UIWorldListItem), "_buttonLabel", null);
	}
}