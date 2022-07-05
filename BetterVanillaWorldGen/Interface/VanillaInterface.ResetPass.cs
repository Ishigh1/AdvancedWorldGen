using System.Reflection;
using AdvancedWorldGen.Helper;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface;

public partial class VanillaInterface
{
	//For Micro Biomes
	public readonly VanillaAccessor<WorldGenConfiguration> Configuration;
	public readonly VanillaAccessor<int> Copper;
	public readonly VanillaAccessor<int> DungeonLocation;

	public readonly VanillaAccessor<int> DungeonSide;
	public readonly VanillaAccessor<int> EvilBiomeAvoidanceMidFixer;

	//For Corruption
	public readonly VanillaAccessor<int> EvilBiomeBeachAvoidance;
	public readonly VanillaAccessor<int> Gold;
	public readonly VanillaAccessor<int> Iron;

	public readonly VanillaAccessor<int> JungleOriginX;

	public readonly VanillaAccessor<int> LeftBeachEnd;
	public readonly VanillaAccessor<int> OceanWaterForcedJungleLength;
	public readonly VanillaAccessor<int> OceanWaterStartRandomMax;

	public readonly VanillaAccessor<int> OceanWaterStartRandomMin;
	public readonly VanillaAccessor<int> RightBeachStart;

	public readonly VanillaAccessor<int> ShellStartXLeft;
	public readonly VanillaAccessor<int> ShellStartXRight;
	public readonly VanillaAccessor<int> ShellStartYLeft;
	public readonly VanillaAccessor<int> ShellStartYRight;
	public readonly VanillaAccessor<int> Silver;
	public readonly VanillaAccessor<int> SnowBottom;
	public readonly VanillaAccessor<int[]> SnowMaxX;
	public readonly VanillaAccessor<int[]> SnowMinX;

	public readonly VanillaAccessor<int> SnowOriginLeft;
	public readonly VanillaAccessor<int> SnowOriginRight;
	public readonly VanillaAccessor<int> SnowTop;

	public VanillaInterface(GenPass vanillaResetPass)
	{
		VanillaAccessor<WorldGenLegacyMethod> methodAccessor = new(typeof(PassLegacy), "_method", vanillaResetPass);
		WorldGenLegacyMethod method = methodAccessor.Value;

		VanillaAccessor<object> dataAccessor = new(method.GetType(), "_target", method);
		object vanillaData = dataAccessor.Value;

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
		SnowMinX = new VanillaAccessor<int[]>(fieldInfos, "snowMinX", vanillaData);
		SnowMaxX = new VanillaAccessor<int[]>(fieldInfos, "snowMaxX", vanillaData);
		SnowTop = new VanillaAccessor<int>(fieldInfos, "snowTop", vanillaData);
		SnowBottom = new VanillaAccessor<int>(fieldInfos, "snowBottom", vanillaData);

		LeftBeachEnd = new VanillaAccessor<int>(fieldInfos, "leftBeachEnd", vanillaData);
		RightBeachStart = new VanillaAccessor<int>(fieldInfos, "rightBeachStart", vanillaData);

		ShellStartXLeft = new VanillaAccessor<int>(fieldInfos, "shellStartXLeft", vanillaData);
		ShellStartYLeft = new VanillaAccessor<int>(fieldInfos, "shellStartYLeft", vanillaData);
		ShellStartXRight = new VanillaAccessor<int>(fieldInfos, "shellStartXRight", vanillaData);
		ShellStartYRight = new VanillaAccessor<int>(fieldInfos, "shellStartYRight", vanillaData);

		OceanWaterStartRandomMin = new VanillaAccessor<int>(fieldInfos, "oceanWaterStartRandomMin", vanillaData);
		OceanWaterStartRandomMax = new VanillaAccessor<int>(fieldInfos, "oceanWaterStartRandomMax", vanillaData);
		OceanWaterForcedJungleLength =
			new VanillaAccessor<int>(fieldInfos, "oceanWaterForcedJungleLength", vanillaData);

		EvilBiomeBeachAvoidance = new VanillaAccessor<int>(fieldInfos, "evilBiomeBeachAvoidance", vanillaData);
		EvilBiomeAvoidanceMidFixer = new VanillaAccessor<int>(fieldInfos, "evilBiomeAvoidanceMidFixer", vanillaData);

		Configuration = new VanillaAccessor<WorldGenConfiguration>(fieldInfos, "configuration", vanillaData);
	}
}