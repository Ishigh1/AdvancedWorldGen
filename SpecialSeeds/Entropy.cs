using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AdvancedSeedGen.SpecialSeeds
{
	public class Entropy
	{
		public int NewTile;
		public int NewWall;
		public int OldTile;
		public int OldWall;
		public byte PaintTile;
		public byte PaintWall;
		public UnifiedRandom Rand;
		public SeedHelper SeedHelper;
		public int SquareSize;
		public Dictionary<int, List<Tuple<int, int>>> Tiles;
		public Dictionary<int, List<Tuple<int, int>>> Walls;
		public int X;
		public int Y;

		public Entropy(int squareSize, SeedHelper seedHelper)
		{
			SeedHelper = seedHelper;
			Tiles = new Dictionary<int, List<Tuple<int, int>>>();
			Walls = new Dictionary<int, List<Tuple<int, int>>>();
			Rand = new UnifiedRandom();
			NewTile = -1;
			NewWall = 0;
			OldTile = -1;
			OldWall = 0;
			PaintTile = (byte) Rand.Next(PaintID.Count);
			PaintWall = (byte) Rand.Next(PaintID.Count);
			SquareSize = squareSize;
			X = Rand.Next(Main.maxTilesX);
			Y = Rand.Next(Main.maxTilesY);
		}

		public Entropy(int squareSize, BinaryReader reader)
		{
			SeedHelper = null;
			Tiles = null;
			Walls = null;
			Rand = null;
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
				for (int j = 0; j < SquareSize; j++)
				{
					int y = (j + Y) % Main.maxTilesY;
					Tile tile = Main.tile[x, y];
					if (tile == null) continue;
					List<Tuple<int, int>> coords;
					if (tile.active())
					{
						int type = tile.type;
						if (Main.tileSolid[type] && !TileID.Sets.Platforms[type] &&
						    !AdvancedSeedGen.NotReplaced.Contains(type))
						{
							if (!Tiles.TryGetValue(type, out coords))
							{
								coords = new List<Tuple<int, int>>();
								Tiles.Add(type, coords);
							}

							coords.Add(new Tuple<int, int>(x, y));
						}
					}

					ushort wall = tile.wall;
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

		public void RandomizeEntropy()
		{
			if (Tiles.Count != 0 && Tiles.Count != TileLoader.TileCount)
			{
				int c = 0;
				while (++c < 10 && (OldTile == -1 || !Main.tileSolid[OldTile] || TileID.Sets.Platforms[OldTile] ||
				                    AdvancedSeedGen.NotReplaced.Contains((ushort) OldTile)))
				{
					int tile = Rand.Next(Tiles.Count);
					OldTile = tile;
					tile -= Tiles.Keys.Count(key => key < tile);
					while (tile != 0 || !Tiles.ContainsKey(OldTile))
					{
						if (Tiles.ContainsKey(OldTile)) tile--;

						OldTile++;
					}

					c++;
				}

				c = 0;
				while (++c < 10 && (NewTile == -1 || !Main.tileSolid[NewTile] || TileID.Sets.Platforms[NewTile] ||
				                    AdvancedSeedGen.NotReplaced.Contains((ushort) NewTile)))
				{
					int tile = Rand.Next(TileLoader.TileCount - Tiles.Count);
					NewTile = tile;
					tile = Tiles.Keys.Count(key => key < tile);
					while (tile != 0 || Tiles.ContainsKey(NewTile))
					{
						if (!Tiles.ContainsKey(NewTile)) tile--;

						NewTile++;
					}

					c++;
				}
			}

			if (Walls.Count != 0 && Walls.Count != WallLoader.WallCount)
			{
				int wall = Rand.Next(1, Walls.Count + 1);
				OldWall = wall;
				wall -= Walls.Keys.Count(key => key < wall);
				while (wall != 0 || !Walls.ContainsKey(OldWall))
				{
					if (Walls.ContainsKey(OldWall)) wall--;

					OldWall++;
				}

				wall = (ushort) Rand.Next(1, WallLoader.WallCount - Walls.Count);
				NewWall = wall;
				wall = (ushort) Walls.Keys.Count(key => key < wall);
				while (wall != 0 || Walls.ContainsKey(NewWall))
				{
					if (!Walls.ContainsKey(NewWall)) wall--;

					NewWall++;
				}
			}
		}

		public void SendData()
		{
			if (Main.netMode == NetmodeID.Server &&
			    (OldTile != -1 && NewTile != -1 || OldWall != 0 && NewWall != 0))
			{
				ModPacket modPacket = SeedHelper.AdvancedSeedGen.GetPacket();
				modPacket.Write((byte) ServerChangeId.Freezing);
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
				foreach (Tuple<int, int> tuple in Tiles[OldTile])
					if (Main.tile[tuple.Item1, tuple.Item2].type == OldTile)
					{
						Main.tile[tuple.Item1, tuple.Item2].type = (ushort) NewTile;
						if (CustomSeededWorld.OptionsContains("Painted"))
							Main.tile[tuple.Item1, tuple.Item2].color(PaintTile);
					}

			if (OldWall != 0 && NewWall != 0)
				foreach (Tuple<int, int> tuple in Walls[OldWall])
					if (Main.tile[tuple.Item1, tuple.Item2].wall == OldWall)
					{
						Main.tile[tuple.Item1, tuple.Item2].wall = (ushort) NewWall;
						if (CustomSeededWorld.OptionsContains("Painted"))
							Main.tile[tuple.Item1, tuple.Item2].wallColor(PaintWall);
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
					Tile tile = Main.tile[x, y];
					s += tile + "\n";
				}

				s += "}\n";
			}

			return s;
		}

		public static void StartEntropy(SeedHelper seedHelper)
		{
			if (!CustomSeededWorld.OptionsContains("Entropy")) return;
			Thread thread = new Thread(DoEntropy) {Priority = ThreadPriority.Lowest};
			thread.Start(seedHelper);
		}

		public static void DoEntropy(object o)
		{
			Entropy entropy = new Entropy(500, o as SeedHelper);
			entropy.ExtractData();
			entropy.RandomizeEntropy();
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
					Tile tile = Main.tile[x, y];
					if (tile == null) continue;
					if (tile.type == OldTile && NewTile != -1)
					{
						tile.type = (ushort) NewTile;
						tile.color(PaintTile);
					}

					if (tile.wall == OldWall && NewTile != 0)
					{
						tile.wall = (ushort) NewWall;
						tile.wallColor(PaintWall);
					}
				}
			}
		}
	}
}