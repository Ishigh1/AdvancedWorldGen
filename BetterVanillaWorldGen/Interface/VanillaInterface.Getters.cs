using AdvancedWorldGen.Helper;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface
{
	public partial class VanillaInterface
	{
		private static VanillaAccessor<TFieldType> InstancedAccessor<TFieldType, TInstance>
			(VanillaAccessor<TFieldType> accessor, TInstance instance)
			where TInstance : class
		{
			accessor.VanillaData = instance;
			return accessor;
		}

		public static VanillaAccessor<int> OptionSize(UIWorldCreation instance) =>
			InstancedAccessor(_optionSize, instance);

		public static VanillaAccessor<object[]> SizeButtons(UIWorldCreation instance) =>
			InstancedAccessor(_sizeButtons, instance);

		public static VanillaAccessor<UICharacterNameButton> SeedPlate(UIWorldCreation instance) =>
			InstancedAccessor(_seedPlate, instance);

		public static VanillaAccessor<Asset<Texture2D>> IconTexture(GroupOptionButton<bool> instance) =>
			InstancedAccessor(_iconTexture, instance);

		public static VanillaAccessor<UIText> DescriptionText(UIWorldCreation instance) =>
			InstancedAccessor(_descriptionText, instance);

		public static VanillaAccessor<UIText> ButtonLabel(UIWorldListItem instance) =>
			InstancedAccessor(_buttonLabel, instance);
	}
}