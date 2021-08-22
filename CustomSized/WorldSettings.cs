using System;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.States;
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

			OnWorldGen.GERunner += HardmodeConversion.ReplaceHardmodeConversion;
		}

		public void ResetSize(OnUIWorldCreation.orig_SetDefaultOptions orig, UIWorldCreation self)
		{
			orig(self);
			SetSizeTo(0);
		}

		public void SetSize(OnUIWorldCreation.orig_ClickSizeOption orig, UIWorldCreation self, UIMouseEvent evt,
			UIElement listeningelement)
		{
			orig(self, evt, listeningelement);
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

			if (Main.Map.MaxWidth < Main.maxTilesX || Main.Map.MaxHeight < Main.maxTilesY)
				Main.Map = new WorldMap(Main.maxTilesX, Main.maxTilesY);
			int oldSizeX = Main.tile.GetLength(0);
			int oldSizeY = Main.tile.GetLength(1);
			if (oldSizeX < Main.maxTilesX || oldSizeY < Main.maxTilesY)
			{
				int newSizeX = Math.Max(oldSizeX, Main.maxTilesX);
				int newSizeY = Math.Max(oldSizeY, Main.maxTilesY);
				Tile[,] oldTiles = Main.tile;
				Main.tile = new Tile[newSizeX, newSizeY];
				for (int y = 0; y < newSizeY; y++)
				{
					Array.Copy(oldTiles, Main.tile, oldSizeX);
					for (int x = oldSizeX; x < newSizeX; x++)
						Main.tile[x, y] = new Tile();
				}
			}

			int newWidth = (Main.maxTilesX - 1) / Main.textureMaxWidth + 1;
			int newHeight = (Main.maxTilesY - 1) / Main.textureMaxHeight + 1;
			if (newWidth > Main.mapTargetX ||
			    newHeight > Main.mapTargetY)
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