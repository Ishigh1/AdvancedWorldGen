using System;
using System.Reflection;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
using AdvancedWorldGen.Helper;
using AdvancedWorldGen.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.Map;
using Terraria.UI;
using OnWorldGen = On.Terraria.WorldGen;
using OnUIWorldCreation = On.Terraria.GameContent.UI.States.UIWorldCreation;

namespace AdvancedWorldGen.CustomSized;

public class WorldSettings
{
	public Params Params;

	public UIWorldCreation UIWorldCreation = null!;

	public WorldSettings()
	{
		Params = new Params();
		OnUIWorldCreation.SetDefaultOptions += ResetSize;
		OnUIWorldCreation.ClickSizeOption += SetSize;
		OnWorldGen.setWorldSize += SetWorldSize;
		OnWorldGen.clearWorld += SetWorldSize;

		OnWorldGen.SmashAltar += AltarSmash.SmashAltar;
		OnWorldGen.GERunner += HardmodeConversion.ReplaceHardmodeConversion;
	}

	public void ResetSize(OnUIWorldCreation.orig_SetDefaultOptions orig, UIWorldCreation self)
	{
		orig(self);
		UIWorldCreation = self;
		SetSizeTo(0);
		AdvancedWorldGenMod.Instance.UiChanger.WorldGenConfigurator?.Dispose();
		AdvancedWorldGenMod.Instance.UiChanger.WorldGenConfigurator = new WorldGenConfigurator();
	}

	public void SetSize(OnUIWorldCreation.orig_ClickSizeOption orig, UIWorldCreation self, UIMouseEvent evt,
		UIElement listeningElement)
	{
		orig(self, evt, listeningElement);

		VanillaAccessor<int> optionSize = VanillaInterface.OptionSize(self);
		int newSize = optionSize.Value;
		SetSizeTo(newSize);
	}

	public void SetSizeTo(int sizeId)
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

	public void SetWorldSize(OnWorldGen.orig_setWorldSize orig)
	{
		SetWorldSize();

		orig();
	}

	public void SetWorldSize(OnWorldGen.orig_clearWorld orig)
	{
		SetWorldSize();

		orig();
	}

	public void SetWorldSize()
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

		int newWidth = Main.maxTilesX / Main.textureMaxWidth + 1;
		int newHeight = Main.maxTilesY / Main.textureMaxHeight + 1;
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