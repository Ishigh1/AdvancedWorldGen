using System.Reflection;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface
{
	public partial class VanillaInterface
	{
		public VanillaAccessor<int> Copper;
		public VanillaAccessor<int> Iron;
		public VanillaAccessor<int> Silver;
		public VanillaAccessor<int> Gold;

		public VanillaAccessor<int> DungeonSide;
		public VanillaAccessor<int> DungeonLocation;

		public VanillaAccessor<int> JungleOriginX;

		public VanillaAccessor<int> SnowOriginLeft;
		public VanillaAccessor<int> SnowOriginRight;

		public VanillaAccessor<int> LeftBeachEnd;
		public VanillaAccessor<int> RightBeachStart;

		public VanillaInterface(GenPass vanillaReset)
		{
			WorldGenLegacyMethod method = (WorldGenLegacyMethod) typeof(PassLegacy)
				.GetField("_method", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(vanillaReset);
			object vanillaData = method.GetType().GetField("_target", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(method);
			FieldInfo[] fieldInfos = vanillaData.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

			Copper = new VanillaAccessor<int>(fieldInfos, "copper", vanillaData);
			Iron = new VanillaAccessor<int>(fieldInfos, "iron", vanillaData);
			Silver = new VanillaAccessor<int>(fieldInfos, "silver", vanillaData);
			Gold = new VanillaAccessor<int>(fieldInfos, "gold", vanillaData);

			DungeonSide = new VanillaAccessor<int>(fieldInfos, "dungeonSide", vanillaData);
			DungeonLocation = new VanillaAccessor<int>(fieldInfos, "dungeonLocation", vanillaData);

			JungleOriginX = new VanillaAccessor<int>(fieldInfos, "jungleOriginX", vanillaData);

			SnowOriginLeft = new VanillaAccessor<int>(fieldInfos, "snowOriginLeft", vanillaData);
			SnowOriginRight = new VanillaAccessor<int>(fieldInfos, "snowOriginRight", vanillaData);

			LeftBeachEnd = new VanillaAccessor<int>(fieldInfos, "leftBeachEnd", vanillaData);
			RightBeachStart = new VanillaAccessor<int>(fieldInfos, "rightBeachStart", vanillaData);

			InitializeStatics();
		}
	}
}