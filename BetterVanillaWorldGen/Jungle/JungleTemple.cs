using System.Collections.Generic;
using AdvancedWorldGen.BetterVanillaWorldGen.Interface;
using AdvancedWorldGen.Helper;
using AdvancedWorldGen.SpecialOptions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Jungle;

public class JungleTemple : ControlledWorldGenPass
{
	public JungleTemple() : base("Jungle Temple", 156.7158f)
	{
	}

	protected override void ApplyPass()
	{
		Progress.Message = Language.GetTextValue("LegacyWorldGen.70");
		int x = WorldGen.genRand.Next(VanillaInterface.JungleLeft, VanillaInterface.JungleRight);
		MakeTemple(Progress, x);
	}

	public static bool IsValid(int x, int y)
	{
		Tile tile = Main.tile[x, y];
		if (tile.HasTile && tile.TileType == TileID.JungleGrass)
			return true;

		return tile.WallType is WallID.Jungle or WallID.JungleUnsafe or WallID.JungleUnsafe1 or WallID.JungleUnsafe2
			or WallID.JungleUnsafe3 or WallID.JungleUnsafe4;
	}

	public static void MakeTemple(GenerationProgress generationProgress, int templeX)
	{
		List<Rectangle> rooms = new();
		float worldSize = Main.maxTilesX / 4200f;
		int ignored = 0;
		int templeRoomCount = ClassicOptions.GetTempleRooms(ref ignored, worldSize);

		int direction = WorldGen.genRand.NextBool(2) ? 1 : -1;

		AllocateRooms(generationProgress, templeX, templeRoomCount, rooms, out int num4, ref direction, out int height);

		int templeY;
		if (height > Main.UnderworldLayer - WorldGen.rockLayer)
			templeY = Main.UnderworldLayer - height - WorldGen.genRand.Next(Main.UnderworldLayer - WorldGen.lavaLine);
		else
			templeY = WorldGen.genRand.Next((int)WorldGen.rockLayer, Main.UnderworldLayer - height);
		MoveRooms(generationProgress, rooms, templeY);

		CreateRoomBorders(generationProgress, templeRoomCount, rooms);

		FillRooms(generationProgress, templeRoomCount, rooms);

		MakeTemplePath(generationProgress, templeX, templeY, templeRoomCount, rooms, direction);

		MakeOutside(generationProgress, templeRoomCount, rooms, out int templeLeft, out int templeRight,
			out int templeTop, out int templeBottom);

		CleanOutside(generationProgress, templeX, num4, templeY, templeLeft, templeRight, templeTop, templeBottom);

		MakeGolemRoom(templeRoomCount, rooms, templeX);

		MakeTraps(templeRoomCount, rooms);

		WorldGen.tLeft = templeLeft;
		WorldGen.tRight = templeRight;
		WorldGen.tTop = templeTop;
		WorldGen.tBottom = templeBottom;
		WorldGen.tRooms = templeRoomCount;
	}

	public static void MakeTraps(int templeRoomCount, List<Rectangle> rooms)
	{
		float num98 = templeRoomCount * 1.1f;
		num98 *= 1f + WorldGen.genRand.Next(-25, 26) * 0.01f;
		if (WorldGen.drunkWorldGen)
			num98 *= 1.5f;

		int num99 = 0;
		while (num98 > 0f)
		{
			num99++;
			int roomIndex = WorldGen.genRand.Next(templeRoomCount);
			int x = WorldGen.genRand.Next(rooms[roomIndex].X, rooms[roomIndex].X + rooms[roomIndex].Width);
			int y = WorldGen.genRand.Next(rooms[roomIndex].Y, rooms[roomIndex].Y + rooms[roomIndex].Height);
			if (Main.tile[x, y].WallType == WallID.LihzahrdBrickUnsafe && !Main.tile[x, y].HasTile)
			{
				bool flag5 = false;
				if (WorldGen.genRand.NextBool(2))
				{
					int directionY = 1;
					if (WorldGen.genRand.NextBool(2))
						directionY = -1;

					for (; !Main.tile[x, y].HasTile; y += directionY)
					{
					}

					y -= directionY;
					bool num104 = WorldGen.genRand.NextBool(2);
					int range = WorldGen.genRand.Next(3, 10);
					bool found = true;
					for (int xx = x - range; xx < x + range; xx++)
					{
						for (int yy = y - range; yy < y + range; yy++)
							if (Main.tile[xx, yy].HasTile &&
							    Main.tile[xx, yy].TileType is TileID.ClosedDoor or TileID.LihzahrdAltar)
							{
								found = false;
								break;
							}

						if (found)
							break;
					}

					if (found)
						for (int xx = x - range; xx < x + range; xx++)
						for (int yy = y - range; yy < y + range; yy++)
						{
							if (!WorldGen.SolidTile(xx, yy) ||
							    Main.tile[xx, yy].TileType == TileID.WoodenSpikes ||
							    WorldGen.SolidTile(xx, yy - directionY))
								continue;

							Main.tile[xx, yy].TileType = TileID.WoodenSpikes;
							flag5 = true;
							if (num104)
							{
								Tile tile = Main.tile[xx, yy - 1];
								tile.TileType = TileID.WoodenSpikes;
								tile.HasTile = true;
								if (WorldGen.drunkWorldGen)
								{
									Tile tile1 = Main.tile[xx, yy - 2];
									tile1.TileType = TileID.WoodenSpikes;
									tile1.HasTile = true;
								}
							}
							else
							{
								Tile tile = Main.tile[xx, yy + 1];
								tile.TileType = TileID.WoodenSpikes;
								tile.HasTile = true;
								if (WorldGen.drunkWorldGen)
								{
									Tile tile1 = Main.tile[xx, yy + 2];
									tile1.TileType = TileID.WoodenSpikes;
									tile1.HasTile = true;
								}
							}

							num104 = !num104;
						}

					if (flag5)
					{
						num99 = 0;
						num98 -= 1f;
					}
				}
				else
				{
					int directionX = 1;
					if (WorldGen.genRand.NextBool(2))
						directionX = -1;

					for (; !Main.tile[x, y].HasTile; x += directionX)
					{
					}

					x -= directionX;
					bool num111 = WorldGen.genRand.NextBool(2);
					int range = WorldGen.genRand.Next(3, 10);
					bool flag7 = true;
					for (int xx = x - range; xx < x + range; xx++)
					for (int yy = y - range; yy < y + range; yy++)
						if (Main.tile[xx, yy].HasTile && Main.tile[xx, yy].TileType == 10)
						{
							flag7 = false;
							break;
						}

					if (flag7)
						for (int xx = x - range; xx < x + range; xx++)
						for (int yy = y - range; yy < y + range; yy++)
						{
							if (!WorldGen.SolidTile(xx, yy) ||
							    Main.tile[xx, yy].TileType == TileID.WoodenSpikes ||
							    WorldGen.SolidTile(xx - directionX, yy))
								continue;

							Main.tile[xx, yy].TileType = TileID.WoodenSpikes;
							flag5 = true;
							if (num111)
							{
								Tile tile = Main.tile[xx - 1, yy];
								tile.TileType = TileID.WoodenSpikes;
								tile.HasTile = true;
							}
							else
							{
								Tile tile = Main.tile[xx + 1, yy];
								tile.TileType = TileID.WoodenSpikes;
								tile.HasTile = true;
							}

							if (WorldGen.drunkWorldGen)
							{
								Tile tile1 = Main.tile[xx - 2, yy];
								tile1.TileType = TileID.WoodenSpikes;
								tile1.HasTile = true;
							}

							num111 = !num111;
						}

					if (flag5)
					{
						num99 = 0;
						num98 -= 1f;
					}
				}
			}

			if (num99 > 1000)
			{
				num99 = 0;
				num98 -= 1f;
			}
		}
	}

	public static void MakeGolemRoom(int templeRoomCount, List<Rectangle> rooms, int templeX)
	{
		if (templeRoomCount > 0)
		{
			int tries = 0;
			Rectangle rectangle3 = rooms[templeRoomCount - 1];
			int num88 = rectangle3.Width / 2;
			int num89 = rectangle3.Height / 2;
			while (true)
			{
				tries++;
				int x = rectangle3.X + num88 + 15 - WorldGen.genRand.Next(30);
				int y = rectangle3.Y + num89 + 15 - WorldGen.genRand.Next(30);
				WorldGen.PlaceTile(x, y, TileID.LihzahrdAltar);
				if (Main.tile[x, y].TileType == TileID.LihzahrdAltar)
				{
					int lAltarX = x - Main.tile[x, y].TileFrameX / 18;
					int lAltarY = y - Main.tile[x, y].TileFrameY / 18;
					VanillaInterface.LAltarX.Value = lAltarX;
					VanillaInterface.LAltarY.Value = lAltarY;
					break;
				}

				if (tries < 1000)
					continue;

				x = rectangle3.X + num88;
				y = rectangle3.Y + num89;
				x += WorldGen.genRand.Next(-10, 11);
				for (y += WorldGen.genRand.Next(-10, 11); !Main.tile[x, y].HasTile; y++)
				{
				}

				Tile tile = Main.tile[x - 1, y];
				tile.HasTile = true;
				tile.Slope = SlopeType.Solid;
				tile.IsHalfBlock = false;
				tile.TileType = TileID.LihzahrdBrick;
				Tile tile1 = Main.tile[x, y];
				tile1.HasTile = true;
				tile1.Slope = SlopeType.Solid;
				tile1.IsHalfBlock = false;
				tile1.TileType = TileID.LihzahrdBrick;
				Tile tile2 = Main.tile[x + 1, y];
				tile2.HasTile = true;
				tile2.Slope = SlopeType.Solid;
				tile2.IsHalfBlock = false;
				tile2.TileType = TileID.LihzahrdBrick;
				y -= 2;
				x--;
				for (int num92 = -1; num92 <= 3; num92++)
				for (int num93 = -1; num93 <= 1; num93++)
				{
					templeX = x + num92;
					y += num93;
					Tile tile3 = Main.tile[templeX, y];
					tile3.HasTile = false;
				}

				int lAltarX2 = x;
				int lAltarY2 = y;
				VanillaInterface.LAltarX.Value = lAltarX2;
				VanillaInterface.LAltarY.Value = lAltarY2;
				for (int num94 = 0; num94 <= 2; num94++)
				for (int num95 = 0; num95 <= 1; num95++)
				{
					templeX = x + num94;
					y += num95;
					Tile tile3 = Main.tile[templeX, y];
					tile3.HasTile = true;
					tile3.TileType = TileID.LihzahrdAltar;
					tile3.TileFrameX = (short)(num94 * 18);
					tile3.TileFrameY = (short)(num95 * 18);
				}

				for (int num96 = 0; num96 <= 2; num96++)
				for (int num97 = 0; num97 <= 1; num97++)
				{
					templeX = x + num96;
					y += num97;
					WorldGen.SquareTileFrame(templeX, y);
				}

				break;
			}
		}
	}

	public static void CleanOutside(GenerationProgress generationProgress, int templeX, int num4, int templeY,
		int templeLeft, int templeRight, int templeTop, int templeBottom)
	{
		int direction;
		direction = -num4;
		Vector2 vector = new(templeX, templeY);
		int num63 = WorldGen.genRand.Next(2, 5);
		bool flag3 = true;
		int num64 = 0;
		int num65 = WorldGen.genRand.Next(9, 14);
		while (flag3)
		{
			num64++;
			if (num64 >= num65)
			{
				num64 = 0;
				vector.Y -= 1f;
			}

			vector.X += direction;
			int num66 = (int)vector.X;
			flag3 = false;
			for (int num67 = (int)vector.Y - num63; num67 < vector.Y + num63; num67++)
			{
				Tile tile = Main.tile[num66, num67];
				if (tile.WallType == 87 ||
				    (tile.HasTile && tile.TileType == 226))
					flag3 = true;

				if (tile.HasTile && tile.TileType == 226)
				{
					tile.HasTile = false;
					tile.WallType = 87;
				}
			}
		}

		int num68 = templeX;
		int num69;
		for (num69 = templeY; !Main.tile[num68, num69].HasTile; num69++)
		{
		}

		num69 -= 4;
		int num70 = num69;
		while ((Main.tile[num68, num70].HasTile && Main.tile[num68, num70].TileType == 226) ||
		       Main.tile[num68, num70].WallType == 87) num70--;

		num70 += 2;
		for (int num71 = num68 - 1; num71 <= num68 + 1; num71++)
		for (int num72 = num70; num72 <= num69; num72++)
		{
			Tile tile = Main.tile[num71, num72];
			tile.HasTile = true;
			tile.TileType = TileID.LihzahrdBrick;
			tile.LiquidAmount = 0;
			tile.Slope = SlopeType.Solid;
			tile.IsHalfBlock = false;
		}

		for (int num73 = num68 - 4; num73 <= num68 + 4; num73++)
		for (int num74 = num69 - 1; num74 < num69 + 3; num74++)
		{
			Tile tile = Main.tile[num73, num74];
			tile.HasTile = false;
			tile.WallType = 87;
		}

		for (int num75 = num68 - 1; num75 <= num68 + 1; num75++)
		for (int num76 = num69 - 5; num76 <= num69 + 8; num76++)
		{
			Tile tile = Main.tile[num75, num76];
			tile.HasTile = true;
			tile.TileType = TileID.LihzahrdBrick;
			tile.LiquidAmount = 0;
			tile.Slope = SlopeType.Solid;
			tile.IsHalfBlock = false;
		}

		for (int num77 = num68 - 3; num77 <= num68 + 3; num77++)
		for (int num78 = num69 - 2; num78 < num69 + 3; num78++)
			if (num78 >= num69 || num77 < templeX - 1 || num77 > templeX + 1)
			{
				Tile tile = Main.tile[num77, num78];
				tile.HasTile = false;
				tile.WallType = 87;
			}

		WorldGen.PlaceTile(num68, num69, 10, true, false, -1, 11);
		for (int num79 = templeLeft; num79 < templeRight; num79++)
		{
			generationProgress.Set(num79, templeRight, 1 / 12f, 9 / 12f);
			for (int num80 = templeTop; num80 < templeBottom; num80++) WorldGen.templeCleaner(num79, num80);
		}

		for (int num81 = templeBottom; num81 >= templeTop; num81--)
		{
			generationProgress.Set(num81, templeTop, 1 / 12f, 10 / 12f);
			for (int num82 = templeRight; num82 >= templeLeft; num82--) WorldGen.templeCleaner(num82, num81);
		}

		for (int num83 = templeLeft; num83 < templeRight; num83++)
		{
			generationProgress.Set(num83, templeRight, 1 / 12f, 11 / 12f);
			for (int num84 = templeTop; num84 < templeBottom; num84++)
			{
				bool flag4 = true;
				for (int num85 = num83 - 1; flag4 && num85 <= num83 + 1; num85++)
				for (int num86 = num84 - 1; num86 <= num84 + 1; num86++)
					if ((!Main.tile[num85, num86].HasTile || Main.tile[num85, num86].TileType != 226) &&
					    Main.tile[num85, num86].WallType != 87)
					{
						flag4 = false;
						break;
					}

				if (flag4)
					Main.tile[num83, num84].WallType = 87;
			}
		}
	}

	public static void MakeOutside(GenerationProgress generationProgress, int templeRoomCount, List<Rectangle> rooms,
		out int templeLeft, out int templeRight, out int templeTop, out int templeBottom)
	{
		templeLeft = Main.maxTilesX - 20;
		templeRight = 20;
		templeTop = Main.maxTilesY - 20;
		templeBottom = 20;
		for (int num54 = 0; num54 < templeRoomCount; num54++)
		{
			if (rooms[num54].X < templeLeft)
				templeLeft = rooms[num54].X;

			if (rooms[num54].X + rooms[num54].Width > templeRight)
				templeRight = rooms[num54].X + rooms[num54].Width;

			if (rooms[num54].Y < templeTop)
				templeTop = rooms[num54].Y;

			if (rooms[num54].Y + rooms[num54].Height > templeBottom)
				templeBottom = rooms[num54].Y + rooms[num54].Height;
		}

		templeLeft -= 10;
		templeRight += 10;
		templeTop -= 10;
		templeBottom += 10;
		for (int num55 = templeLeft; num55 < templeRight; num55++)
		{
			generationProgress.Set(num55, templeRight, 1 / 12f, 5 / 12f);
			for (int num56 = templeTop; num56 < templeBottom; num56++) WorldGen.outerTempled(num55, num56);
		}

		for (int num57 = templeRight; num57 >= templeLeft; num57--)
		{
			generationProgress.Set(num57, templeLeft, 1 / 12f, 6 / 12f);
			for (int num58 = templeTop; num58 < templeBottom / 2; num58++) WorldGen.outerTempled(num57, num58);
		}

		for (int num59 = templeTop; num59 < templeBottom; num59++)
		{
			generationProgress.Set(num59, templeBottom, 1 / 12f, 7 / 12f);
			for (int num60 = templeLeft; num60 < templeRight; num60++) WorldGen.outerTempled(num60, num59);
		}

		for (int num61 = templeBottom; num61 >= templeTop; num61--)
		{
			generationProgress.Set(num61, templeTop, 1 / 12f, 8 / 12f);
			for (int num62 = templeLeft; num62 < templeRight; num62++) WorldGen.outerTempled(num62, num61);
		}
	}

	public static void MakeTemplePath(GenerationProgress generationProgress, int templeX, int templeY,
		int templeRoomCount,
		List<Rectangle> rooms, int direction)
	{
		Vector2 templePath = new(templeX, templeY);
		for (int num39 = 0; num39 < templeRoomCount; num39++)
		{
			generationProgress.Set(num39, templeRoomCount, 1 / 12f, 4 / 12f);
			Rectangle rectangle2 = rooms[num39];
			rectangle2.X += 8;
			rectangle2.Y += 8;
			rectangle2.Width -= 16;
			rectangle2.Height -= 16;
			bool flag2 = true;
			while (flag2)
			{
				int num40 = WorldGen.genRand.Next(rectangle2.X, rectangle2.X + rectangle2.Width);
				int num41 = WorldGen.genRand.Next(rectangle2.Y, rectangle2.Y + rectangle2.Height);
				if (num39 == templeRoomCount - 1)
				{
					num40 = rectangle2.X + rectangle2.Width / 2 + WorldGen.genRand.Next(-10, 10);
					num41 = rectangle2.Y + rectangle2.Height / 2 + WorldGen.genRand.Next(-10, 10);
				}

				templePath = WorldGen.templePather(templePath, num40, num41);
				if (templePath.X == num40 && templePath.Y == num41)
					flag2 = false;
			}

			if (num39 >= templeRoomCount - 1)
				continue;

			if (WorldGen.genRand.Next(3) != 0)
			{
				int num42 = num39 + 1;
				if (rooms[num42].Y >= rooms[num39].Y + rooms[num39].Height)
				{
					rectangle2.X = rooms[num42].X;
					if (num39 == 0)
					{
						if (direction > 0)
							rectangle2.X += (int)(rooms[num42].Width * 0.8);
						else
							rectangle2.X += (int)(rooms[num42].Width * 0.2);
					}
					else if (rooms[num42].X < rooms[num39].X)
					{
						rectangle2.X += (int)(rooms[num42].Width * 0.2);
					}
					else
					{
						rectangle2.X += (int)(rooms[num42].Width * 0.8);
					}

					rectangle2.Y = rooms[num42].Y;
				}
				else
				{
					rectangle2.X = (rooms[num39].X + rooms[num39].Width / 2 + rooms[num42].X +
					                rooms[num42].Width / 2) / 2;
					rectangle2.Y = (int)(rooms[num42].Y + rooms[num42].Height * 0.8);
				}

				int x3 = rectangle2.X;
				int y4 = rectangle2.Y;
				flag2 = true;
				while (flag2)
				{
					int num43 = WorldGen.genRand.Next(x3 - 6, x3 + 7);
					int num44 = WorldGen.genRand.Next(y4 - 6, y4 + 7);
					templePath = WorldGen.templePather(templePath, num43, num44);
					if (templePath.X == num43 && templePath.Y == num44)
						flag2 = false;
				}

				continue;
			}

			int num45 = num39 + 1;
			int num46 = (rooms[num39].X + rooms[num39].Width / 2 + rooms[num45].X + rooms[num45].Width / 2) / 2;
			int num47 = (rooms[num39].Y + rooms[num39].Height / 2 + rooms[num45].Y + rooms[num45].Height / 2) / 2;
			flag2 = true;
			while (flag2)
			{
				int num48 = WorldGen.genRand.Next(num46 - 6, num46 + 7);
				int num49 = WorldGen.genRand.Next(num47 - 6, num47 + 7);
				templePath = WorldGen.templePather(templePath, num48, num49);
				if (templePath.X == num48 && templePath.Y == num49)
					flag2 = false;
			}
		}
	}

	public static void FillRooms(GenerationProgress generationProgress, int templeRoomCount, List<Rectangle> rooms)
	{
		for (int num24 = 0; num24 < templeRoomCount; num24++)
		{
			generationProgress.Set(num24, templeRoomCount, 1 / 12f, 3 / 12f);
			for (int num25 = rooms[num24].X; num25 < rooms[num24].X + rooms[num24].Width; num25++)
			for (int num26 = rooms[num24].Y; num26 < rooms[num24].Y + rooms[num24].Height; num26++)
			{
				Tile tile = Main.tile[num25, num26];
				tile.HasTile = true;
				tile.TileType = TileID.LihzahrdBrick;
				tile.LiquidAmount = 0;
				tile.Slope = SlopeType.Solid;
				tile.IsHalfBlock = false;
			}

			int x2 = rooms[num24].X;
			int num27 = x2 + rooms[num24].Width;
			int y3 = rooms[num24].Y;
			int num28 = y3 + rooms[num24].Height;
			x2 += WorldGen.genRand.Next(3, 8);
			num27 -= WorldGen.genRand.Next(3, 8);
			y3 += WorldGen.genRand.Next(3, 8);
			num28 -= WorldGen.genRand.Next(3, 8);
			int num29 = x2;
			int num30 = num27;
			int num31 = y3;
			int num32 = num28;
			int num33 = (x2 + num27) / 2;
			int num34 = (y3 + num28) / 2;
			for (int num35 = x2; num35 < num27; num35++)
			for (int num36 = y3; num36 < num28; num36++)
			{
				if (WorldGen.genRand.NextBool(20))
					num31 += WorldGen.genRand.Next(-1, 2);

				if (WorldGen.genRand.NextBool(20))
					num32 += WorldGen.genRand.Next(-1, 2);

				if (WorldGen.genRand.NextBool(20))
					num29 += WorldGen.genRand.Next(-1, 2);

				if (WorldGen.genRand.NextBool(20))
					num30 += WorldGen.genRand.Next(-1, 2);

				if (num29 < x2)
					num29 = x2;

				if (num30 > num27)
					num30 = num27;

				if (num31 < y3)
					num31 = y3;

				if (num32 > num28)
					num32 = num28;

				if (num29 > num33)
					num29 = num33;

				if (num30 < num33)
					num30 = num33;

				if (num31 > num34)
					num31 = num34;

				if (num32 < num34)
					num32 = num34;

				if (num35 >= num29 && num35 < num30 && num36 >= num31 && num36 <= num32)
				{
					Tile tile = Main.tile[num35, num36];
					tile.HasTile = false;
					tile.WallType = 87;
				}
			}

			for (int num37 = num28; num37 > y3; num37--)
			for (int num38 = num27; num38 > x2; num38--)
			{
				if (WorldGen.genRand.NextBool(20))
					num31 += WorldGen.genRand.Next(-1, 2);

				if (WorldGen.genRand.NextBool(20))
					num32 += WorldGen.genRand.Next(-1, 2);

				if (WorldGen.genRand.NextBool(20))
					num29 += WorldGen.genRand.Next(-1, 2);

				if (WorldGen.genRand.NextBool(20))
					num30 += WorldGen.genRand.Next(-1, 2);

				if (num29 < x2)
					num29 = x2;

				if (num30 > num27)
					num30 = num27;

				if (num31 < y3)
					num31 = y3;

				if (num32 > num28)
					num32 = num28;

				if (num29 > num33)
					num29 = num33;

				if (num30 < num33)
					num30 = num33;

				if (num31 > num34)
					num31 = num34;

				if (num32 < num34)
					num32 = num34;

				if (num38 >= num29 && num38 < num30 && num37 >= num31 && num37 <= num32)
				{
					Tile tile = Main.tile[num38, num37];
					tile.HasTile = false;
					tile.WallType = 87;
				}
			}
		}
	}

	public static void CreateRoomBorders(GenerationProgress generationProgress, int templeRoomCount,
		List<Rectangle> rooms)
	{
		int counter = 0;
		for (int k = 0; k < templeRoomCount; k++)
		for (int l = 0; l < 2; l++)
		for (int m = 0; m < templeRoomCount; m++)
		for (int n = 0; n < 2; n++)
		{
			generationProgress.Set(counter++, templeRoomCount * templeRoomCount * 4, 1 / 12f, 2 / 12f);
			int num17 = rooms[k].X;
			if (l == 1)
				num17 += rooms[k].Width - 1;

			int num18 = rooms[k].Y;
			int num19 = num18 + rooms[k].Height;
			int num20 = rooms[m].X;
			if (n == 1)
				num20 += rooms[m].Width - 1;

			int y2 = rooms[m].Y;
			int num21 = y2 + rooms[m].Height;
			while (num17 != num20 || num18 != y2 || num19 != num21)
			{
				if (num17 < num20)
					num17++;

				if (num17 > num20)
					num17--;

				if (num18 < y2)
					num18++;

				if (num18 > y2)
					num18--;

				if (num19 < num21)
					num19++;

				if (num19 > num21)
					num19--;

				int num22 = num17;
				for (int num23 = num18; num23 < num19; num23++)
				{
					Tile tile = Main.tile[num22, num23];
					tile.HasTile = true;
					tile.TileType = TileID.LihzahrdBrick;
					tile.LiquidAmount = 0;
					tile.Slope = SlopeType.Solid;
					tile.IsHalfBlock = false;
				}
			}
		}
	}


	public static void MoveRooms(GenerationProgress generationProgress, List<Rectangle> rooms, int templeY)
	{
		for (int index = 0; index < rooms.Count; index++)
		{
			generationProgress.Set(index, rooms.Count, 1 / 12f, 1 / 12f);
			Rectangle rectangle = rooms[index];
			rectangle.Y += templeY;
			rooms[index] = rectangle;
		}
	}

	public static void AllocateRooms(GenerationProgress generationProgress, int templeX, int templeRoomCount,
		List<Rectangle> rooms, out int num4, ref int direction, out int templeHeight)
	{
		num4 = direction;
		int num7 = templeX;
		templeHeight = 0;
		int lineLimit = WorldGen.genRand.Next(1, 3);
		int roomsInTheSameLine = 0;
		for (int i = 0; i < templeRoomCount; i++)
		{
			generationProgress.Set(i, templeRoomCount, 1 / 12f);
			roomsInTheSameLine++;
			int num11 = direction;
			int x = num7;
			int y = templeHeight;
			bool flag = true;
			int num16 = -10;
			while (flag)
			{
				x = num7;
				y = templeHeight;
				int width = WorldGen.genRand.Next(25, 50);
				int height = WorldGen.genRand.Next(20, 35);
				if (height > width)
					height = width;

				if (i == templeRoomCount - 1) //Golem room
				{
					width = WorldGen.genRand.Next(55, 65);
					height = WorldGen.genRand.Next(45, 50);
					if (height > width)
						height = width;

					width = (int)(width * 1.6);
					height = (int)(height * 1.35);
					y += WorldGen.genRand.Next(5, 10);
				}

				if (roomsInTheSameLine > lineLimit)
				{
					y += WorldGen.genRand.Next(height + 1, height + 3) + num16;
					x += WorldGen.genRand.Next(-5, 6);
					num11 = direction * -1;
				}
				else
				{
					x += (WorldGen.genRand.Next(width + 1, width + 3) + num16) * num11;
					y += WorldGen.genRand.Next(-5, 6);
				}

				flag = false;
				Rectangle rectangle = new(x - width / 2, y - height / 2, width, height);
				for (int j = 0; j < i; j++)
					if (rectangle.Intersects(rooms[j]))
					{
						flag = true;
						break;
					}

				if (!flag)
					rooms.Add(rectangle);

				if (WorldGen.genRand.NextBool(100))
					num16++;
			}

			if (roomsInTheSameLine > lineLimit)
			{
				lineLimit++;
				roomsInTheSameLine = 1;
			}

			direction = num11;
			num7 = x;
			templeHeight = y;
		}
	}
}