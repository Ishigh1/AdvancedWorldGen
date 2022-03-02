using System;
using System.Collections.Generic;
using AdvancedWorldGen.BetterVanillaWorldGen;
using AdvancedWorldGen.BetterVanillaWorldGen.Jungle;
using AdvancedWorldGen.Helper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;

namespace AdvancedWorldGen.CustomSized.Secret;

public class TempleWorld : ControlledWorldGenPass
{
	public TempleWorld() : base("Jungle Temple", float.PositiveInfinity)
	{
	}

	protected override void ApplyPass()
	{
		Progress.Message = Language.GetTextValue("LegacyWorldGen.70");
		List<Rectangle> rooms = new();
		int direction = WorldGen.genRand.NextBool(2) ? 1 : -1;
		Main.worldSurface = 100;

		for (int x = 5; x < Main.maxTilesX - 5; x++)
		for (int y = 5; y < Main.maxTilesY - 5; y++)
		{
			Tile tile = Main.tile[x, y];
			if (x >= 39 && x < Main.maxTilesX - 39 && y >= 39 && y < Main.maxTilesY - 39)
			{
				tile.HasTile = true;
				tile.TileType = TileID.LihzahrdBrick;
				tile.WallType = WallID.LihzahrdBrickUnsafe;
			}
			else
				tile.WallType = WallID.LihzahrdBrick;
		}

		AllocateRooms(out int templeRoomCount, rooms, direction, out int templeX, out int templeY);

		JungleTemple.FillRooms(Progress, templeRoomCount, rooms);

		JungleTemple.MakeTemplePath(Progress, templeX, templeY, templeRoomCount, rooms, direction);

		JungleTemple.MakeGolemRoom(templeRoomCount, rooms, templeX);

		JungleTemple.MakeTraps(templeRoomCount, rooms);

		WorldGen.tLeft = 50;
		WorldGen.tRight = Main.maxTilesX - 50;
		WorldGen.tTop = 50;
		WorldGen.tBottom = Main.maxTilesY - 50;
		WorldGen.tRooms = templeRoomCount;
		
		WorldGen.templePart2();
		
		for (int x = 40; x < Main.maxTilesX - 40; x++)
		for (int y = 40; y < Main.maxTilesY - 40; y++)
			if (WorldGen.genRand.NextBool(1000))
				WorldGen.AddLifeCrystal(x, y);
			else if (WorldGen.genRand.NextBool(10))
				WorldGen.PlacePot(x, y, style:WorldGen.genRand.Next(28, 31));

		while (WorldGen.SolidTile(Main.spawnTileX, Main.spawnTileY--))
		{
		}

		Main.spawnTileY++;
	
		while (true)
		{
			Rectangle randomRoom = rooms[WorldGen.genRand.Next(rooms.Count)];
			int x = WorldGen.genRand.Next(randomRoom.Left, randomRoom.Right + 1);
			int y = randomRoom.Bottom;
			
			while (WorldGen.SolidTile(x, y--))
			{
			}
			y++;
			
			if (y < 10 || WorldGen.SolidTile(Main.tile[x, y - 10]))
				continue;

			Vector2 center = new(x * 16 + 8, y * 16 - 64 - 8 - 27);
			if (!CultistRitual.CheckFloor(center, out Point[]? _))
				continue;

			Main.dungeonX = x;
			Main.dungeonY = y;
			int id = NPC.NewNPC(new EntitySource_WorldGen(), x * 16, y * 16, NPCID.OldMan);
			Main.npc[id].homeless = false;
			Main.npc[id].homeTileX = Main.dungeonX;
			Main.npc[id].homeTileY = Main.dungeonY;
			break;
		}
	}

	public void AllocateRooms(out int templeRoomCount, List<Rectangle> rooms, int direction, out int templeX,
		out int templeY)
	{
		if (direction == 1)
			templeX = 50;
		else
			templeX = Main.maxTilesX - 50;

		templeY = 50;
		int x = templeX, y = templeY;

		List<Rectangle> lastLine = new();
		List<Rectangle> currentLine = new();
		Rectangle room = default;
		while (y < Main.maxTilesY - 75)
		{
			Progress.Set(y * Main.maxTilesX + Main.maxTilesX / 2 - direction * x, Main.maxTilesX * Main.maxTilesY, 3/12f);
			int width = WorldGen.genRand.Next(35, 60);
			int height = WorldGen.genRand.Next(30, 45);
			if (height > width)
				height = width;

			x = Math.Min(x, Main.maxTilesX - 50 - width / 2);
			x = Math.Max(x, 50 + width / 2);
			y = Math.Min(y, Main.maxTilesY - 50 - height / 2);
			y = Math.Max(y, 50 + height / 2);
			room = new Rectangle(x - width / 2, y - height / 2, width, height);
			bool roomValid = true;
			foreach (Rectangle rectangle in lastLine)
				if (room.Intersects(rectangle))
				{
					y++;
					roomValid = false;
				}

			foreach (Rectangle rectangle in currentLine)
				if (room.Intersects(rectangle))
				{
					x += direction;
					roomValid = false;
				}

			if (roomValid)
			{
				if (rooms.Count == 0)
				{
					Main.spawnTileX = x;
					Main.spawnTileY = y;
				}

				currentLine.Add(room);
				rooms.Add(room);
			}

			switch (direction)
			{
				case 1 when x > Main.maxTilesX - 100:
					direction = -1;
					lastLine = currentLine;
					currentLine = new List<Rectangle>();
					y += height + WorldGen.genRand.Next(5, 10);
					break;
				case -1 when x < 100:
					direction = 1;
					lastLine = currentLine;
					currentLine = new List<Rectangle>();
					y += height + WorldGen.genRand.Next(5, 10);
					break;
				default:
				{
					if (roomValid) x += direction * 5;

					break;
				}
			}

			x += WorldGen.genRand.Next(-5, 6);
			y += WorldGen.genRand.Next(-2, 2);
		}

		for (int index = 0; index < rooms.Count; index++)
		{
			room = rooms[index];
			room.X -= 5;
			room.Y -= 5;
			room.Width -= 10;
			room.Height -= 10;
			rooms[index] = room;
		}

		int bossRoomWidth = WorldGen.genRand.Next(55, 65);
		int bossRoomHeight = WorldGen.genRand.Next(45, 50);
		if (bossRoomHeight > bossRoomWidth)
			bossRoomHeight = bossRoomWidth;

		bossRoomWidth = (int)(bossRoomWidth * 1.6);
		bossRoomHeight = (int)(bossRoomHeight * 1.35);

		room.X -= (bossRoomWidth - room.Width) / 2;
		room.Width = bossRoomWidth;
		room.Height = bossRoomHeight;
		rooms[^1] = room;

		templeRoomCount = rooms.Count;
	}
}