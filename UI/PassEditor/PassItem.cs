using System.Collections.Generic;
using System.Linq;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.BetterVanillaWorldGen;
using AdvancedWorldGen.WorldRegenerator;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.UI.PassEditor
{
	public class PassItem : UIPanel
	{
		public const int SpaceBetweenIcons = 28;
		public static List<PassItem> Order = new();
		public static UIList UIList = null!;
		public readonly GenPass GenPass;
		public int Index;

		public PassItem(GenPass genPass, int index = -1)
		{
			GenPass = genPass;
			Index = index == -1 ? Order.Count : index;
			Order.Add(this);
			Width = new StyleDimension(0f, 1f);
			Height = new StyleDimension(GenPass is ControlledWorldGenPass ? 80f : 40f, 0f);
			SetupEditableZone();
			SetupButtons();

			UIList.Add(this);
		}

		public void SetupEditableZone()
		{
			ConfigElement nameInputLine = InputElement.MakeStringInputLine(GenPass, nameof(GenPass.Name), "Text : ");
			nameInputLine.VAlign = 0.5f;
			nameInputLine.MaxWidth = new StyleDimension {Percent = 1f, Pixels = -55 - 2 * SpaceBetweenIcons};
			
			if (GenPass is ControlledWorldGenPass controlledWorldGenPass) nameInputLine.Top = new StyleDimension {Pixels = -20};

			Append(nameInputLine);
		}

		public void SetupButtons()
		{
			UIImage deletePassButton = new(TextureAssets.Trash)
			{
				Top = {Percent = 0.5f, Pixels = -14},
				Left = {Percent = 1f, Pixels = -50},
				Width = {Pixels = 25},
				Height = {Pixels = 28}
			};
			deletePassButton.OnMouseDown += (_, _) =>
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				PassHandler.AvailablePasses.RemoveAt(Index);
				UIList.Remove(this);
				foreach (PassItem passItem in Order.Where(passItem => passItem.Index > Index)) passItem.Index++;
			};
			Append(deletePassButton);

			UIImage copyPassButton = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Copy"))
			{
				Top = {Percent = 0.5f, Pixels = -14},
				Left = {Percent = 1f, Pixels = -50 - SpaceBetweenIcons},
				Width = {Pixels = 25},
				Height = {Pixels = 28}
			};
			copyPassButton.OnMouseDown += (_, _) =>
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				PassHandler.AvailablePasses.Insert(Index + 1, GenPass);
				foreach (PassItem passItem in Order.Where(passItem => passItem.Index > Index))
					passItem.Index++;

				new PassItem(GenPass, Index + 1);
			};
			Append(copyPassButton);

			UIImage moveUpButton =
				new(AdvancedWorldGenMod.Instance.Assets.Request<Texture2D>("Images/ArrowUp"))
				{
					Top = {Percent = 0.5f, Pixels = -14},
					Left = {Percent = 1f, Pixels = -50 - 2 * SpaceBetweenIcons},
					Width = {Pixels = 25},
					Height = {Pixels = 13}
				};
			moveUpButton.OnMouseDown += (_, _) =>
			{
				if (Index == PassHandler.AvailablePasses.Count - 1)
					return;
				SoundEngine.PlaySound(SoundID.MenuTick);

				PassHandler.AvailablePasses[Index] = PassHandler.AvailablePasses[Index + 1];
				PassHandler.AvailablePasses[Index + 1] = GenPass;

				int elementToExchange = Order.FindIndex(passItem => passItem.Index == Index + 1);
				Index++;
				Order[elementToExchange].Index--;
				UIList.UpdateOrder();
				UIList.Recalculate();
			};
			Append(moveUpButton);

			UIImage moveDownButton =
				new(AdvancedWorldGenMod.Instance.Assets.Request<Texture2D>("Images/ArrowDown"))
				{
					Top = {Percent = 0.5f, Pixels = 1},
					Left = {Percent = 1f, Pixels = -50 - 2 * SpaceBetweenIcons},
					Width = {Pixels = 25},
					Height = {Pixels = 13}
				};
			moveDownButton.OnMouseDown += (_, _) =>
			{
				if (Index == 0)
					return;
				SoundEngine.PlaySound(SoundID.MenuTick);

				PassHandler.AvailablePasses[Index] = PassHandler.AvailablePasses[Index - 1];
				PassHandler.AvailablePasses[Index - 1] = GenPass;

				int elementToExchange = Order.FindIndex(passItem => passItem.Index == Index - 1);
				Index--;
				Order[elementToExchange].Index++;
				UIList.UpdateOrder();
				UIList.Recalculate();
			};
			Append(moveDownButton);
		}
	}
}