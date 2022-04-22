using System.Collections.Generic;
using AdvancedWorldGen.Base;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.SpecialOptions.Halloween.Worldgen;

public static class Graveyards
{
	public static void GenerateStructures(GenerationProgress progress, GameConfiguration configuration)
	{
		for (int i = 0; i < Utilities.WorldSize; i++) PlaceSimpleGraveyard();
	}

	public static void PlaceSimpleGraveyard()
	{
		int size = (int)(Main.rand.Next(10, 30) * Utilities.WorldSize);
		int x = Main.rand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);
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

			if (i != size && Main.rand.NextBool(2))
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
				tileType = Main.tile[i, y].TileType;
				yMin = y;
				xAtMin = i;
				if (tileType == TileID.LeafBlock)
					return false;
			}
		}

		if (tileType is TileID.Grass or TileID.CrimsonGrass)
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
			if (Main.rand.NextBool(5))
				goalY++;

			Tile tile = Main.tile[i, y];
			tile.Slope = SlopeType.Solid;
			tile.IsHalfBlock = false;
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
			if (Main.rand.NextBool(5))
				goalY++;

			Tile tile = Main.tile[i, y];
			tile.Slope = SlopeType.Solid;
			tile.IsHalfBlock = false;
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
				Tile tile = Main.tile[i, yCurrent];
				if (yCurrent < yPrev && tile.Slope == SlopeType.Solid)
				{
					tile.IsHalfBlock = true;
				}
				else if (yCurrent > yPrev && Main.tile[i, yPrev].Slope == SlopeType.Solid)
				{
					Tile tile1 = Main.tile[--i, yPrev];
					tile1.IsHalfBlock = true;
					yPrev = yCurrent;
				}
			}

			Tile currentTile = Main.tile[i, yPrev];
			switch (currentTile.TileType)
			{
				case TileID.Mud:
					currentTile.TileType = TileID.JungleGrass;
					goto case TileID.JungleGrass;
				case TileID.Dirt:
					currentTile.TileType = TileID.CrimsonGrass;
					goto case TileID.CrimsonGrass;
				case TileID.JungleGrass:
					currentTile.TileColor = PaintID.OrangePaint;
					if (yCurrent == yPrev)
						PaintTree(i, yPrev - 1, PaintID.OrangePaint);
					break;
				case TileID.Grass:
				case TileID.CrimsonGrass:
				case TileID.CorruptGrass:
					currentTile.TileColor = PaintID.YellowPaint;
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
			switch (currentTile.TileType)
			{
				case TileID.Trees:
				case TileID.Cactus:
				case TileID.PalmTree:
				case TileID.VanityTreeSakura:
				case TileID.VanityTreeYellowWillow:
					currentTile.TileColor = paint;
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
					: Language.GetTextValue(key, deadName, Main.worldName);
			case 2: //Projectile death
				projectileName = Language.RandomFromCategory("ProjectileName").Value;
				return Language.GetTextValue("DeathSource.Projectile", baseText, projectileName);
			case 3: //NPC death
				string npcName = Language.RandomFromCategory("NPCName").Value;
				return Language.GetTextValue("DeathSource.NPC", baseText, npcName);
			case 4: //Player death by projectile
				playerKillerName = GetRandomName();
				projectileName = Language.RandomFromCategory("ProjectileName").Value;
				return Language.GetTextValue("DeathSource.Player", baseText, playerKillerName, projectileName);
			default: //Player death by direct weapon
				playerKillerName = GetRandomName();
				string itemName = Language.RandomFromCategory("ItemName").Value;
				return Language.GetTextValue("DeathSource.Player", baseText, playerKillerName, itemName);
		}
	}

	public static string GetRandomName()
	{
		switch (Main.rand.Next(3))
		{
			case 0: //From credits

				static bool Filter(string key, LocalizedText text)
				{
					return key.StartsWith("CreditsRollCategory") && !key.EndsWith(".1");
				}

				string name = Language.SelectRandom(Filter).Value;
				return name.EndsWith(".com") ? GetRandomName() : name;
			case 1: //From paintings
				return Language.RandomFromCategory("PaintingArtist").Value;
			default: //From NPC names
				int npcs = ModifiedWorld.NPCs.Count;
				List<int> petList = new() { NPCID.TownCat, NPCID.TownDog, NPCID.TownBunny };
				int pets = petList.Count;

				int npcId = Main.rand.Next(npcs + pets);
				npcId = npcId < npcs ? ModifiedWorld.NPCs[npcId] : petList[npcId - npcs];

				NPC tmpNPC = new()
				{
					type = npcId
				};
				if (TownNPCProfiles.Instance.GetProfile(tmpNPC, out ITownNPCProfile profile))
				{
					tmpNPC.townNpcVariationIndex = profile.RollVariation();
					return profile.GetNameForVariant(tmpNPC);
				}

				return GetRandomName(); //Something went wrong
		}
	}
}