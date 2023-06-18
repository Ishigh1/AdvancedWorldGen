namespace AdvancedWorldGen.CustomSized;

public class WorldSettings
{
	public UIWorldCreation UIWorldCreation = null!;
	public static int BaseX;
	public static int BaseY;

	public WorldSettings()
	{
		Params.Wipe();

		On_UIWorldCreation.SetDefaultOptions += ResetSize;
		On_UIWorldCreation.ClickSizeOption += SetSize;
		On_WorldGen.setWorldSize += SetWorldSize;
		On_WorldGen.clearWorld += SetWorldSize;

		On_WorldGen.SmashAltar += AltarSmash.SmashAltar;
		On_WorldGen.GERunner += HardmodeConversion.ReplaceHardmodeConversion;
		On_WorldGen.UpdateMapTile += MapRelated.UpdateMapTileInBounds;
	}

	private void ResetSize(On_UIWorldCreation.orig_SetDefaultOptions orig, UIWorldCreation self)
	{
		orig(self);
		UIWorldCreation = self;
		if (BaseX <= 0 && BaseY <= 0)
		{
			SetSizeTo(ModLoader.TryGetMod("CalamityMod", out Mod _)
				? 2
				: 1); //Calamity have large worlds by defaults and do it in a way that fucks with this logic
		}

		if (BaseX > 0)
			Params.SizeX = BaseX;
		if (BaseY > 0)
			Params.SizeY = BaseY;
		if (BaseX > 0 || BaseY > 0)
			ApplySize();

		AdvancedWorldGenMod.Instance.UiChanger.VanillaWorldGenConfigurator?.Dispose();
		AdvancedWorldGenMod.Instance.UiChanger.VanillaWorldGenConfigurator = new VanillaWorldGenConfigurator();
		AdvancedWorldGenMod.Instance.UiChanger.OverhauledWorldGenConfigurator = new OverhauledWorldGenConfigurator();
	}

	private static void SetSize(On_UIWorldCreation.orig_ClickSizeOption orig, UIWorldCreation self, UIMouseEvent evt,
		UIElement listeningElement)
	{
		orig(self, evt, listeningElement);

		FieldAccessor<int> optionSize = VanillaInterface.OptionSize(self);
		int newSize = optionSize.Value;
		SetSizeTo(newSize);
	}

	public void ApplySize()
	{
		int size = Params.SizeX switch
		{
			4200 when Params.SizeY == 1200 => 0,
			6400 when Params.SizeY == 1800 => 1,
			8400 when Params.SizeY == 2400 => 2,
			_ => -1
		};

		FieldAccessor<int> optionSize = VanillaInterface.OptionSize(UIWorldCreation);
		optionSize.Value = size;

		object[] sizeButtons = VanillaInterface.SizeButtons(UIWorldCreation).Value;

		Type groupOptionButtonType = sizeButtons.GetType().GetElementType()!;
		MethodInfo setCurrentOptionMethod =
			groupOptionButtonType.GetMethod("SetCurrentOption", BindingFlags.Instance | BindingFlags.Public)!;

		foreach (object groupOptionButton in sizeButtons)
			setCurrentOptionMethod.Invoke(groupOptionButton, new object[] { size });
	}

	private static void SetSizeTo(int sizeId)
	{
		switch (sizeId)
		{
			case 0:
				Params.SizeX = 4200;
				Params.SizeY = 1200;
				break;
			case 1:
				Params.SizeX = 6400;
				Params.SizeY = 1800;
				break;
			case 2:
				Params.SizeX = 8400;
				Params.SizeY = 2400;
				break;
		}
	}

	private static void SetWorldSize(On_WorldGen.orig_setWorldSize orig)
	{
		SetWorldSize();

		orig();
	}

	private static void SetWorldSize(On_WorldGen.orig_clearWorld orig)
	{
		SetWorldSize();

		orig();
	}

	private static void SetWorldSize()
	{
		if (Params.SizeX != -1)
		{
			Main.maxTilesX = Params.SizeX;
			Main.maxTilesY = Params.SizeY;
		}

		if (8400 < Main.maxTilesX || 2400 < Main.maxTilesY)
		{
			int chunkX = (Main.maxTilesX - 1) / Main.sectionWidth + 1;
			int chunkY = (Main.maxTilesY - 1) / Main.sectionHeight + 1;
			int newSizeX = Math.Max(chunkX * Main.sectionWidth, 8400);
			int newSizeY = Math.Max(chunkY * Main.sectionHeight, 2400);

			if (KnownLimits.WillCrashMissingEwe(newSizeX, newSizeY))
			{
				string message =
					Language.GetTextValue("Mods.AdvancedWorldGen.InvalidSizes.TooBigFromRAM", newSizeX, newSizeY);
				Utils.ShowFancyErrorMessage(message, 0);
				throw new Exception(message);
			}

			Main.Map = new WorldMap(newSizeX, newSizeY);

			ConstructorInfo constructorInfo = typeof(Tilemap).GetConstructor(
				BindingFlags.NonPublic | BindingFlags.Instance, new[] { typeof(ushort), typeof(ushort) })!;
			Main.tile = (Tilemap)constructorInfo.Invoke(new object?[] { (ushort)newSizeX, (ushort)newSizeY });
		}

		int newWidth = Main.maxTilesX / Main.textureMaxWidth + 2;
		int newHeight = Main.maxTilesY / Main.textureMaxHeight + 2;
		if (newWidth > Main.mapTargetX || newHeight > Main.mapTargetY)
		{
			Main.mapTargetX = Math.Max(5, newWidth);
			Main.mapTargetY = Math.Max(3, newHeight);
			Main.instance.mapTarget = new RenderTarget2D[Main.mapTargetX, Main.mapTargetY];
			Main.initMap = new bool[Main.mapTargetX, Main.mapTargetY];
			Main.mapWasContentLost = new bool[Main.mapTargetX, Main.mapTargetY];
		}
	}
}