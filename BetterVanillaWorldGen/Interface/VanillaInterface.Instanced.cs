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

		public static VanillaAccessor<int> OptionSize(UIWorldCreation instance)
		{
			_optionSize.VanillaData = instance;
			return _optionSize;
		}

		public static VanillaAccessor<object[]> SizeButtons(UIWorldCreation instance)
		{
			_sizeButtons.VanillaData = instance;
			return _sizeButtons;
		}

		public static VanillaAccessor<UICharacterNameButton> SeedPlate(UIWorldCreation instance)
		{
			_seedPlate.VanillaData = instance;
			return _seedPlate;
		}

		public static VanillaAccessor<Asset<Texture2D>> IconTexture(GroupOptionButton<bool> instance)
		{
			_iconTexture.VanillaData = instance;
			return _iconTexture;
		}

		public static VanillaAccessor<UIText> DescriptionText(UIWorldCreation instance)
		{
			_descriptionText.VanillaData = instance;
			return _descriptionText;
		}

		public static VanillaAccessor<UIText> ButtonLabel(UIWorldListItem instance)
		{
			_buttonLabel.VanillaData = instance;
			return _buttonLabel;
		}
	}
}