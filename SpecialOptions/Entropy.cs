using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AdvancedWorldGen.Base;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AdvancedWorldGen.SpecialOptions;

public class Entropy
{
	public int NewTile;
	public int NewWall;
	public int OldTile;
	public int OldWall;
	public byte PaintTile;
	public byte PaintWall;
	public UnifiedRandom Rand;
	public int SquareSize;
	public Dictionary<int, List<Tuple<int, int>>> Tiles;
	public Dictionary<int, List<Tuple<int, int>>> Walls;
	public int X;
	public int Y;

	public Entropy(int squareSize)
	{
		Tiles = new Dictionary<int, List<Tuple<int, int>>>();
		Walls = new Dictionary<int, List<Tuple<int, int>>>();
		Rand = new UnifiedRandom();
		NewTile = -1;
		NewWall = 0;
		OldTile = -1;
		OldWall = 0;
		PaintTile = (byte)Rand.Next(PaintID.IlluminantPaint + 1);
		PaintWall = (byte)Rand.Next(PaintID.IlluminantPaint + 1);
		SquareSize = squareSize;
		X = Rand.Next(Main.maxTilesX);
		Y = Rand.Next(Main.maxTilesY);
	}

	public Entropy(int squareSize, BinaryReader reader)
	{
		Tiles = null!;
		Walls = null!;
		Rand = null!;
		NewTile = reader.ReadInt32();
		NewWall = reader.ReadUInt16();
		OldTile = reader.ReadInt32();
		OldWall = reader.ReadUInt16();
		PaintTile = reader.ReadByte();
		PaintWall = reader.ReadByte();
		SquareSize = squareSize;
		X = reader.ReadInt32();
		Y = reader.ReadInt32();
	}

	public void ExtractData()
	{
		for (int i = 0; i < SquareSize; i++)
		{
			int x = (i + X) % Main.maxTilesX;
			for (int j = SquareSize - 1; j >= 0; j--)
			{
				int y = (j + Y) % Main.maxTilesY;
				Tile tile = Framing.GetTileSafely(x, y);
				if (tile == null) continue;
				List<Tuple<int, int>>? coords;
				if (tile.HasTile)
				{
					int type = tile.TileType;
					if (!TileReplacer.DontReplace(type))
					{
						if (!Tiles.TryGetValue(type, out coords))
						{
							coords = new List<Tuple<int, int>>();
							Tiles.Add(type, coords);
						}

						coords.Add(new Tuple<int, int>(x, y));
					}
				}

				ushort wall = tile.WallType;
				if (wall != 0)
				{
					if (!Walls.TryGetValue(wall, out coords))
					{
						coords = new List<Tuple<int, int>>();
						Walls.Add(wall, coords);
					}

					coords.Add(new Tuple<int, int>(x, y));
				}
			}
		}
	}

	public void RandomizeWalls()
	{
		if (Walls.Count != 0 && Walls.Count != WallLoader.WallCount)
		{
			int wall = Rand.Next(1, Walls.Count + 1);
			OldWall = wall;
			wall -= Walls.Keys.Count(key => key < wall);
			while (wall != 0 || !Walls.ContainsKey(OldWall))
				if (Walls.ContainsKey(++OldWall))
					wall--;

			wall = Rand.Next(1, WallLoader.WallCount - Walls.Count);
			NewWall = wall;
			wall = Walls.Keys.Count(key => key < wall);
			while (wall != 0 || Walls.ContainsKey(NewWall))
				if (!Walls.ContainsKey(++NewWall))
					wall--;
		}
	}

	public void RandomizeTiles()
	{
		if (Tiles.Count != 0 && Tiles.Count != TileLoader.TileCount)
		{
			int c = 0;
			while (++c < 10 && (OldTile == -1 || TileReplacer.DontReplace(OldTile)))
			{
				int tile = Rand.Next(Tiles.Count);
				OldTile = tile;
				tile -= Tiles.Keys.Count(key => key < tile);
				while (tile != 0 || !Tiles.ContainsKey(OldTile))
					if (Tiles.ContainsKey(++OldTile))
						tile--;

				c++;
			}

			c = 0;
			while (++c < 10 && (NewTile == -1 || TileReplacer.DontReplace(NewTile)))
			{
				int tile = Rand.Next(TileLoader.TileCount - Tiles.Count);
				NewTile = tile;
				tile = Tiles.Keys.Count(key => key < tile);
				while (tile != 0 || Tiles.ContainsKey(NewTile))
					if (!Tiles.ContainsKey(++NewTile))
						tile--;

				c++;
			}
		}
	}

	public void SendData()
	{
		if (Main.netMode == NetmodeID.Server &&
		    ((OldTile != -1 && NewTile != -1) || (OldWall != 0 && NewWall != 0)))
		{
			ModPacket modPacket = AdvancedWorldGenMod.Instance.GetPacket();
			modPacket.Write((byte)PacketId.SantaWaterFreezing);
			modPacket.Write(NewTile);
			modPacket.Write(NewWall);
			modPacket.Write(OldTile);
			modPacket.Write(OldWall);
			modPacket.Write(PaintTile);
			modPacket.Write(PaintWall);
			modPacket.Write(SquareSize);
			modPacket.Write(X);
			modPacket.Write(Y);
			modPacket.Send();
		}
	}

	public void ApplyChanges()
	{
		if (OldTile != -1 && NewTile != -1)
			foreach ((int x, int y) in Tiles[OldTile]
				         .Where(tuple => Framing.GetTileSafely(tuple.Item1, tuple.Item2).TileType == OldTile))
			{
				Tile tileSafely = Framing.GetTileSafely(x, y);
				tileSafely.TileType = (ushort)NewTile;
				WorldGen.DiamondTileFrame(x, y);
				if (API.OptionsContains("Random.Painted"))
					tileSafely.TileColor = PaintTile;
			}

		if (OldWall != 0 && NewWall != 0)
			foreach ((int x, int y) in Walls[OldWall]
				         .Where(tuple => Framing.GetTileSafely(tuple.Item1, tuple.Item2).WallType == OldWall))
			{
				Tile tileSafely = Framing.GetTileSafely(x, y);
				tileSafely.WallType = (ushort)NewWall;
				WorldGen.SquareWallFrame(x, y);
				if (API.OptionsContains("Random.Painted"))
					tileSafely.WallColor = PaintWall;
			}
	}

	public override string ToString()
	{
		return base.ToString() + "\n" +
		       "x = " + X + ", y = " + Y + "\n" +
		       "oldTile = " + OldTile + ", newTile = " + NewTile +
		       "oldWall = " + OldWall + ", newWall = " + NewWall;
	}

	public string Values()
	{
		string s = "";
		foreach (KeyValuePair<int, List<Tuple<int, int>>> keyValuePair in Tiles)
		{
			s += keyValuePair.Key + "\n{\n";
			foreach ((int x, int y) in keyValuePair.Value)
			{
				s += "(" + x + ", " + y + ") : ";
				Tile tile = Framing.GetTileSafely(x, y);
				s += tile + "\n";
			}

			s += "}\n";
		}

		return s;
	}

	public static void StartEntropy()
	{
		if (!API.OptionsContains("Entropy")) return;
		Thread thread = new(DoEntropy) { Priority = ThreadPriority.Lowest };
		thread.Start();
	}

	public static void DoEntropy()
	{
		Entropy entropy = new(500);
		entropy.ExtractData();
		entropy.RandomizeTiles();
		entropy.RandomizeWalls();
		entropy.SendData();
		entropy.ApplyChanges();
	}

	public void TreatTiles()
	{
		for (int i = 0; i < SquareSize; i++)
		{
			int x = (i + X) % Main.maxTilesX;
			for (int j = 0; j < SquareSize; j++)
			{
				int y = (j + Y) % Main.maxTilesY;
				Tile tile = Framing.GetTileSafely(x, y);
				if (tile == null) continue;
				if (tile.TileType == OldTile && NewTile != -1)
				{
					tile.TileType = (ushort)NewTile;
					tile.TileColor = PaintTile;
				}

				if (tile.WallType == OldWall && NewTile != 0)
				{
					tile.WallType = (ushort)NewWall;
					tile.WallColor = PaintWall;
				}
			}
		}
	}
}