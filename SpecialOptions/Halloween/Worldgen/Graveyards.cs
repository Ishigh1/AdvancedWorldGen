using System.Collections.Generic;
using AdvancedWorldGen.Base;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.SpecialOptions.Halloween.Worldgen
{
	public class Graveyards
	{
		public static void GenerateStructures(GenerationProgress progress, GameConfiguration configuration)
		{
			for (int i = 0; i < 20 * Utilities.WorldSize; i++) PlaceSimpleGraveyard();
		}

		public static void PlaceSimpleGraveyard()
		{
			int x = Main.rand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);
			int size = Main.rand.Next(20, 50) * Utilities.WorldSize;
			int y;
			int prevTerrainY = -1;
			if (!FlattenGround(x - size, x + size))
			{
				PlaceSimpleGraveyard();
				return;
			}

			FinishFlattenGround(x - size, x + size);

			int prevFenceHeight = -1;
			for (int i = -size; i <= size; i++)
			{
				y = Utilities.FindGround(x + i);
				int currentFenceHeight = prevFenceHeight;

				switch (Main.rand.Next(10))
				{
					case 0:
						currentFenceHeight++;
						break;
					case 1:
						currentFenceHeight--;
						break;
				}

				currentFenceHeight -= y - prevTerrainY;
				if (currentFenceHeight < 2)
					currentFenceHeight = 2;
				if (currentFenceHeight > 2 + (size + i) / 2)
					currentFenceHeight = 2 + (size + i) / 2;
				if (currentFenceHeight > 2 + (size - i) / 2)
					currentFenceHeight = 2 + (size - i) / 2;
				prevFenceHeight = currentFenceHeight;
				prevTerrainY = y;

				for (int newY = y - 1; newY >= y - 3; newY--)
					WorldGen.PlaceWall(x + i, newY, WallID.WroughtIronFence, true);

				if (i != size && Main.rand.Next(2) == 0)
				{
					if (!WorldGen.PlaceTile(x + i, y - 1, TileID.Tombstones, true, true,
						style: Main.rand.Next(11))) continue;
					int signPos = Sign.ReadSign(x + i, y - 1);
					if (signPos == -1) continue;
					string signText = GenerateSignText();
					Sign.TextSign(signPos, signText);
				}
			}
		}

		public static bool FlattenGround(int xMin, int xMax)
		{
			int yMin = -1;
			int tileType = -1;
			int xAtMin = -1;
			int prevTerrainY = -1;
			int y;
			for (int i = xMin; i <= xMax; i++)
			{
				y = Utilities.FindGround(i);
				if (yMin == -1 || y < yMin)
				{
					tileType = Main.tile[i, y].type;
					yMin = y;
					xAtMin = i;
					if (tileType == TileID.LeafBlock)
						return false;
				}
			}

			if (tileType == TileID.Grass || tileType == TileID.CrimsonGrass)
				tileType = TileID.Dirt;
			if (tileType == TileID.JungleGrass)
				tileType = TileID.Mud;

			for (int i = xAtMin; i >= xMin; i--)
			{
				y = Utilities.FindGround(i);
				if (i == xAtMin || y <= prevTerrainY + 1)
				{
					prevTerrainY = y;
					continue;
				}

				int goalY = prevTerrainY;
				if (Main.rand.Next(5) == 0)
					goalY++;

				Main.tile[i, y].Slope = SlopeType.Solid;
				Main.tile[i, y].IsHalfBlock = false;
				for (int j = y - 1; j >= goalY; j--)
				{
					WorldGen.KillTile(i, j);
					WorldGen.PlaceTile(i, j, tileType, forced: true);
				}

				prevTerrainY = goalY;
			}

			if (xMin < xAtMin - 1)
				FlattenGround(xMin, xAtMin - 1);

			for (int i = xAtMin; i <= xMax; i++)
			{
				y = Utilities.FindGround(i);
				if (i == xAtMin || y <= prevTerrainY + 1)
				{
					prevTerrainY = y;
					continue;
				}

				int goalY = prevTerrainY;
				if (Main.rand.Next(5) == 0)
					goalY++;

				Main.tile[i, y].Slope = SlopeType.Solid;
				Main.tile[i, y].IsHalfBlock = false;
				for (int j = y - 1; j >= goalY; j--)
				{
					WorldGen.KillTile(i, j);
					WorldGen.PlaceTile(i, j, tileType, forced: true);
				}

				prevTerrainY = goalY;
			}

			if (xMax > xAtMin + 1)
				FlattenGround(xAtMin + 1, xMax);
			return true;
		}

		public static void FinishFlattenGround(int xMin, int xMax)
		{
			int yPrev = -1;
			for (int i = xMin; i <= xMax; i++)
			{
				int yCurrent;
				if (i == xMin)
				{
					yCurrent = Utilities.FindGround(i);
					yPrev = yCurrent;
				}
				else
				{
					yCurrent = yPrev;
					Utilities.GoAtTop(i, ref yCurrent);
					if (yCurrent < yPrev && Main.tile[i, yCurrent].Slope == SlopeType.Solid)
					{
						Main.tile[i, yCurrent].IsHalfBlock = true;
					}
					else if (yCurrent > yPrev && Main.tile[i, yPrev].Slope == SlopeType.Solid)
					{
						Main.tile[--i, yPrev].IsHalfBlock = true;
						yPrev = yCurrent;
					}
				}

				Tile currentTile = Main.tile[i, yPrev];
				switch (currentTile.type)
				{
					case TileID.Mud:
						currentTile.type = TileID.JungleGrass;
						goto case TileID.JungleGrass;
					case TileID.Dirt:
						currentTile.type = TileID.CrimsonGrass;
						goto case TileID.CrimsonGrass;
					case TileID.JungleGrass:
						currentTile.Color = PaintID.OrangePaint;
						if (yCurrent == yPrev)
							PaintTree(i, yPrev - 1, PaintID.OrangePaint);
						break;
					case TileID.Grass:
					case TileID.CrimsonGrass:
					case TileID.CorruptGrass:
						currentTile.Color = PaintID.YellowPaint;
						if (yCurrent == yPrev)
							PaintTree(i, yPrev - 1, PaintID.YellowPaint);
						break;
					case TileID.Sand:
					case TileID.Crimsand:
					case TileID.Ebonsand:
						if (yCurrent == yPrev)
							PaintTree(i, yPrev - 1, PaintID.OrangePaint);
						break;
				}

				if (yCurrent < yPrev)
				{
					yPrev = yCurrent;
					i--;
				}
			}
		}

		public static void PaintTree(int x, int y, byte paint)
		{
			for (int i = -1; i <= 1; i++)
			{
				Tile currentTile = Main.tile[x + i, y];
				switch (currentTile.type)
				{
					case TileID.Trees:
					case TileID.Cactus:
					case TileID.PalmTree:
					case TileID.VanityTreeSakura:
					case TileID.VanityTreeYellowWillow:
						currentTile.Color = paint;
						if (i == 0)
							PaintTree(x, y - 1, paint);
						break;
					case TileID.Plants:
					case TileID.CorruptPlants:
					case TileID.CorruptThorns:
					case TileID.JunglePlants:
					case TileID.Plants2:
					case TileID.CrimsonPlants:
					case TileID.CrimsonThorns:
						WorldGen.KillTile(x + i, y);
						break;
				}
			}
		}

		public static string GenerateSignText()
		{
			string deadName = GetRandomName();
			string baseText = NetworkText.FromKey(Language.RandomFromCategory("DeathTextGeneric").Key,
				deadName, Main.worldName).ToString();
			string projectileName;
			string playerKillerName;
			switch (Main.rand.Next(5))
			{
				case 0: //Generic
					return baseText;
				case 1: //Special death
					string key = Language.RandomFromCategory("DeathText").Key;
					return key == "DeathText.Default"
						? GenerateSignText()
						: NetworkText.FromKey(key, deadName, Main.worldName).ToString();
				case 2: //Projectile death
					projectileName = Language.RandomFromCategory("ProjectileName").Value;
					return NetworkText.FromKey("DeathSource.Projectile", baseText, projectileName).ToString();
				case 3: //NPC death
					string npcName = Language.RandomFromCategory("NPCName").Value;
					return NetworkText.FromKey("DeathSource.NPC", baseText, npcName).ToString();
				case 4: //Player death by projectile
					playerKillerName = GetRandomName();
					projectileName = Language.RandomFromCategory("ProjectileName").Value;
					return NetworkText.FromKey("DeathSource.Player", baseText, playerKillerName, projectileName)
						.ToString();
				default: //Player death by direct weapon
					playerKillerName = GetRandomName();
					string itemName = Language.RandomFromCategory("ItemName").Value;
					return NetworkText.FromKey("DeathSource.Player", baseText, playerKillerName, itemName)
						.ToString();
			}
		}

		public static string GetRandomName()
		{
			switch (Main.rand.Next(3))
			{
				case 0: //From credits

					bool Filter(string key, LocalizedText text)
					{
						return key.StartsWith("CreditsRollCategory") && !key.EndsWith(".1");
					}

					string name = Language.SelectRandom(Filter).Value;
					return name.EndsWith(".com") ? GetRandomName() : name;
				case 1: //From paintings
					return Language.RandomFromCategory("PaintingArtist").Value;
				default: //From NPC names
					int npcs = ModifiedWorld.NPCs.Count;
					List<int> petList = new() {NPCID.TownCat, NPCID.TownDog, NPCID.TownBunny};
					int pets = petList.Count;

					int npcId = Main.rand.Next(npcs + pets);
					npcId = npcId < npcs ? ModifiedWorld.NPCs[npcId] : petList[npcId - npcs];

					NPC tmpNPC = new();
					tmpNPC.type = npcId;
					if (TownNPCProfiles.Instance.GetProfile(npcId, out ITownNPCProfile profile))
					{
						tmpNPC.townNpcVariationIndex = profile.RollVariation();
						return profile.GetNameForVariant(tmpNPC);
					}

					return GetRandomName(); //Something went wrong
			}
		}
	}
}