using AdvancedWorldGen.Helper.Accessors;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface;

public partial class VanillaInterface
{
	private static readonly FieldAccessor<int> _optionSize =
		new(typeof(UIWorldCreation), "_optionSize", null);

	private static readonly FieldAccessor<object[]> _sizeButtons =
		new(typeof(UIWorldCreation), "_sizeButtons", null);

	private static readonly FieldAccessor<UICharacterNameButton> _seedPlate =
		new(typeof(UIWorldCreation), "_seedPlate", null);

	private static readonly FieldAccessor<Asset<Texture2D>> _iconTexture =
		new(typeof(GroupOptionButton<bool>), "_iconTexture", null);

	private static readonly FieldAccessor<UIText> _descriptionText =
		new(typeof(UIWorldCreation), "_descriptionText", null);

	private static readonly FieldAccessor<UIText> _buttonLabel =
		new(typeof(UIWorldListItem), "_buttonLabel", null);
}