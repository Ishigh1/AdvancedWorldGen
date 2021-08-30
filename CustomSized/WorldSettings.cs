using System;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.Map;
using Terraria.UI;
using OnWorldGen = On.Terraria.WorldGen;
using OnUIWorldCreation = On.Terraria.GameContent.UI.States.UIWorldCreation;

namespace AdvancedWorldGen.CustomSized
{
	public class WorldSettings
	{
		public int SizeX;
		public int SizeY;

		public WorldSettings()
		{
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
			SetSizeTo(0);
		}

		public void SetSize(OnUIWorldCreation.orig_ClickSizeOption orig, UIWorldCreation self, UIMouseEvent evt,
			UIElement listeningElement)
		{
			orig(self, evt, listeningElement);
			int newSize = (int) typeof(UIWorldCreation)
				.GetField("_optionSize", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
			SetSizeTo(newSize);
		}

		public void SetSizeTo(int sizeId)
		{
			switch (sizeId)
			{
				case -1:
					SizeX = 0;
					break;
				case 0:
					SizeX = 4200;
					SizeY = 1200;
					break;
				case 1:
					SizeX = 6400;
					SizeY = 1800;
					break;
				case 2:
					SizeX = 8400;
					SizeY = 2400;
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
			if (SizeX != 0)
			{
				Main.maxTilesX = SizeX;
				Main.maxTilesY = SizeY;
			}

			int oldSizeX = Main.tile.GetLength(0);
			int oldSizeY = Main.tile.GetLength(1);
			if (oldSizeX < Main.maxTilesX || oldSizeY < Main.maxTilesY)
			{
				int newSizeX = Math.Max(Main.maxTilesX, oldSizeX);
				int newSizeY = Math.Max(Main.maxTilesY, oldSizeY);

				if ((long) newSizeX * newSizeY * 44 > GC.GetGCMemoryInfo().TotalAvailableMemoryBytes)
				{
					string message = Language.GetTextValue("Mods.AdvancedWorldGen.InvalidSizes.TooBigFromRAM", newSizeX,
						newSizeY);
					Utils.ShowFancyErrorMessage(message, 0);
					throw new Exception(message);
				}

				Main.Map = new WorldMap(newSizeX, newSizeY);

				Main.tile = new Tile[newSizeX, newSizeY];
			}

			int newWidth = (Main.maxTilesX - 1) / Main.textureMaxWidth + 1;
			int newHeight = (Main.maxTilesY - 1) / Main.textureMaxHeight + 1;
			if (newWidth > Main.mapTargetX || newHeight > Main.mapTargetY)
			{
				Main.mapTargetX = Math.Max(newWidth, Main.mapTargetX);
				Main.mapTargetY = Math.Max(newHeight, Main.mapTargetY);
				Main.instance.mapTarget = new RenderTarget2D[Main.mapTargetX, Main.mapTargetY];
				Main.initMap = new bool[Main.mapTargetX, Main.mapTargetY];
				Main.mapWasContentLost = new bool[Main.mapTargetX, Main.mapTargetY];
			}

			OverhauledInit.Init();
		}
	}
}