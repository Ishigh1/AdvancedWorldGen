using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.Map;
using Terraria.UI;
using OnWorldGen = On.Terraria.WorldGen;
using OnUIWorldCreation = On.Terraria.GameContent.UI.States.UIWorldCreation;

namespace AdvancedWorldGen.Base
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
		}

		private void ResetSize(OnUIWorldCreation.orig_SetDefaultOptions orig, UIWorldCreation self)
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

		private void SetWorldSize(OnWorldGen.orig_setWorldSize orig)
		{
			if (SizeX == 0)
			{
				orig();
				return;
			}

			Main.maxTilesX = SizeX;
			Main.maxTilesY = SizeY;
			if (Main.Map.MaxWidth < SizeX || Main.Map.MaxHeight < SizeY) Main.Map = new WorldMap(SizeX, SizeY);
			int oldSizeX = Main.tile.GetLength(0);
			int oldSizeY = Main.tile.GetLength(1);
			if (oldSizeX < SizeX || oldSizeY < SizeY)
			{
				int newSizeX = Math.Max(oldSizeX, SizeX);
				int newSizeY = Math.Max(oldSizeY, SizeY);
				Tile[,] oldTiles = Main.tile;
				Main.tile = new Tile[newSizeX, newSizeY];
				for (int x = 0; x < newSizeX; x++)
				for (int y = 0; y < newSizeY; y++)
					if (x < oldSizeX && y < oldSizeY)
						Main.tile[x, y] = oldTiles[x, y];
					else
						Main.tile[x, y] = new Tile();
			}

			orig();
		}
	}
}