using AdvancedWorldGen.Helper.Accessors;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface;

public partial class VanillaInterface
{
	private static FieldAccessor<TFieldType> InstancedAccessor<TFieldType, TInstance>
		(FieldAccessor<TFieldType> accessor, TInstance instance)
		where TInstance : class
	{
		accessor.VanillaData = instance;
		return accessor;
	}

	public static FieldAccessor<int> OptionSize(UIWorldCreation instance)
	{
		return InstancedAccessor(_optionSize, instance);
	}

	public static FieldAccessor<object[]> SizeButtons(UIWorldCreation instance)
	{
		return InstancedAccessor(_sizeButtons, instance);
	}

	public static FieldAccessor<UICharacterNameButton> SeedPlate(UIWorldCreation instance)
	{
		return InstancedAccessor(_seedPlate, instance);
	}

	public static FieldAccessor<Asset<Texture2D>> IconTexture(GroupOptionButton<bool> instance)
	{
		return InstancedAccessor(_iconTexture, instance);
	}

	public static FieldAccessor<UIText> DescriptionText(UIWorldCreation instance)
	{
		return InstancedAccessor(_descriptionText, instance);
	}

	public static FieldAccessor<UIText> ButtonLabel(UIWorldListItem instance)
	{
		return InstancedAccessor(_buttonLabel, instance);
	}
}