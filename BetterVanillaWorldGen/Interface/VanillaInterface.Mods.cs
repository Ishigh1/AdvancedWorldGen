namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface;

public partial class VanillaInterface // Yes, I know, bad name...
{
	public CalamityInterface Calamity;

	public VanillaInterface()
	{
		Calamity = new CalamityInterface();
	}

	public class CalamityInterface
	{
		public readonly bool Enabled;
		public readonly ReflectionAccessor<int> SulphurousSeaBiomeWidth;

		public CalamityInterface()
		{
			if (ModLoader.TryGetMod("CalamityMod", out Mod mod))
			{
				Type? sulphurousSea = mod.GetType("CalamityMod.World.SulphurousSea");
				if (sulphurousSea == null)
					return;
				SulphurousSeaBiomeWidth = new PropertyAccessor<int>(sulphurousSea, "BiomeWidth");

				Enabled = true;
			}
		}
	}
}