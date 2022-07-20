using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.BetterVanillaWorldGen.MicroBiomesStuff;

public class ModifiedTrackGenerator
{
	public enum TrackMode : byte
	{
		Normal,
		Tunnel
	}

	public enum TrackSlope : sbyte
	{
		Up = -1,
		Straight,
		Down
	}

	private static readonly HashSet<ushort> InvalidWalls = new()
	{
		7,
		94,
		95,
		8,
		98,
		99,
		9,
		96,
		97,
		3,
		83,
		68,
		62,
		78,
		87,
		86,
		42,
		74,
		27,
		149
	};

	private static readonly HashSet<ushort> InvalidTiles = new()
	{
		383,
		384,
		15,
		304,
		30,
		321,
		245,
		246,
		240,
		241,
		242,
		16,
		34,
		158,
		377,
		94,
		10,
		19,
		86,
		219,
		484,
		190
	};

	private readonly TrackHistory[] _rewriteHistory = new TrackHistory[25];
	private int _xDirection;
	public List<TrackHistory> CurrentTrack;
	public Rectangle CurrentTrackRectangle;
	public int PlayerHeight = 6;

	public RTree RTree;
	public byte Sample;
	public int StrictMinimum;
	public List<List<TrackHistory>> TrackList = new();

	public ModifiedTrackGenerator(int strictMinimum)
	{
		StrictMinimum = strictMinimum;
		RTree = new RTree(new Rectangle(Main.maxTilesX / 2, (int)((Main.worldSurface + Main.maxTilesY - 200) / 2), 0, 0));
		if (WorldGen.notTheBees)
			InvalidWalls.Add(108);
	}

	public bool Place(Point origin, int minLength, int maxLength)
	{
		CurrentTrack = new List<TrackHistory>(maxLength);
		foreach (List<TrackHistory> trackHistories in TrackList.Where(trackHistories => trackHistories.Count >= minLength))
		{
			CurrentTrack = trackHistories.Count > maxLength ? trackHistories.GetRange(trackHistories.Count - maxLength, maxLength) : trackHistories;
			TrackList.Remove(trackHistories);
			break;
		}

		if (CurrentTrack.Count == 0)
		{
			if (!FindSuitableOrigin(ref origin))
				return false;

			CurrentTrackRectangle = new Rectangle(origin.X, origin.Y, 1, 1);

			CurrentTrack.Add(new TrackHistory(origin.X, origin.Y, TrackSlope.Down));
			_xDirection = origin.X <= Main.maxTilesX / 2 ? 1 : -1;
			if (!FindPath(maxLength))
			{
				RegisterBadRectangle();
				return false;
			}

			int trackLength = CurrentTrack.Count;
			if (trackLength < minLength && trackLength * 2 > minLength)
			{
				CurrentTrack.Reverse();
				_xDirection *= -1;
				if (!FindPath(maxLength))
				{
					RegisterBadRectangle();
					return false;
				}
			}

			RegisterBadRectangle();
			if (CurrentTrack.Count < minLength)
			{
				if (CurrentTrack.Count >= StrictMinimum) TrackList.Add(CurrentTrack);
				return false;
			}
		}

		PlacePath();
		return true;
	}

	private bool FindSuitableOrigin(ref Point origin)
	{
		Sample = 0;
		return CalculateStateForLocation(origin.X, origin.Y) != TrackPlacementState.Available;
	}

	private bool FindPath(int maxLength)
	{
		while (CurrentTrack.Count < maxLength)
		{
			TrackSlope slope = CurrentTrack[^1].Slope != TrackSlope.Up ? TrackSlope.Down : TrackSlope.Straight;
			AppendToHistory(slope);
			TrackPlacementState trackPlacementState = TryRewriteHistoryToAvoidTiles();
			if (trackPlacementState == TrackPlacementState.Invalid)
			{
				CurrentTrack.RemoveAt(CurrentTrack.Count - 1);
				break;
			}

			TrackPlacementState trackPlacementState2 = trackPlacementState;
			while (trackPlacementState2 != TrackPlacementState.Available && CurrentTrack.Count < maxLength)
			{
				int originalCount = CurrentTrack.Count;
				trackPlacementState2 = CreateTunnel();
				if (trackPlacementState2 == TrackPlacementState.Invalid)
				{
					CurrentTrack.RemoveRange(originalCount, CurrentTrack.Count - originalCount);
					break;
				}
			}
		}

		if (CurrentTrack.Count < StrictMinimum)
			return false;

		SmoothTrack();
		return GetHistorySegmentPlacementState(0, CurrentTrack.Count) != TrackPlacementState.Invalid;
	}

	private TrackPlacementState CreateTunnel()
	{
		TrackSlope trackSlope = TrackSlope.Straight;
		int num = 10;
		TrackPlacementState trackPlacementState = TrackPlacementState.Invalid;
		TrackHistory baseTrack = CurrentTrack[^1];
		int x = baseTrack.X;
		int y = baseTrack.Y;
		for (TrackSlope trackSlope2 = TrackSlope.Up; trackSlope2 <= TrackSlope.Down; trackSlope2++)
		{
			TrackPlacementState trackPlacementState2 = TrackPlacementState.Invalid;
			for (int i = 1; i < num; i++)
			{
				int trackX = x + i * _xDirection;
				trackPlacementState2 = CalculateStateForLocation(trackX, y + i * (int)trackSlope2);
				if (trackPlacementState2 != TrackPlacementState.Obstructed)
				{
					if (trackPlacementState2 == TrackPlacementState.Available)
					{
						trackSlope = trackSlope2;
						num = i;
						trackPlacementState = trackPlacementState2;
					}

					break;
				}
			}

			if (trackPlacementState != TrackPlacementState.Available && trackPlacementState2 == TrackPlacementState.Obstructed && (trackPlacementState != TrackPlacementState.Obstructed || trackSlope != TrackSlope.Straight))
			{
				trackSlope = trackSlope2;
				num = 10;
				trackPlacementState = trackPlacementState2;
			}
		}

		if (CurrentTrack.Count == 0 || !CanSlopesTouch(baseTrack.Slope, trackSlope))
			RewriteSlopeDirection(CurrentTrack.Count - 1, TrackSlope.Straight);

		baseTrack = CurrentTrack[^1];
		baseTrack.Mode = TrackMode.Tunnel;
		CurrentTrack[^1] = baseTrack;
		for (int j = 1; j < num; j++) AppendToHistory(trackSlope, TrackMode.Tunnel);

		return trackPlacementState;
	}

	private void PlacePath()
	{
		bool[] array = new bool[CurrentTrack.Count];
		for (int i = 0; i < CurrentTrack.Count; i++)
		{
			if (WorldGen.genRand.Next(7) == 0)
				PlayerHeight = WorldGen.genRand.Next(5, 9);

			for (int j = 0; j < PlayerHeight; j++)
			{
				if (Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j - 1].WallType == 244)
					Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j - 1].WallType = 0;

				if (Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j].WallType == 244)
					Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j].WallType = 0;

				if (Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j + 1].WallType == 244)
					Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j + 1].WallType = 0;

				if (Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j].TileType == 135)
					array[i] = true;

				WorldGen.KillTile(CurrentTrack[i].X, CurrentTrack[i].Y - j, false, false, true);
			}
		}

		for (int k = 0; k < CurrentTrack.Count; k++)
		{
			if (WorldGen.genRand.Next(7) == 0)
				PlayerHeight = WorldGen.genRand.Next(5, 9);

			TrackHistory trackHistory = CurrentTrack[k];
			Tile.SmoothSlope(trackHistory.X, trackHistory.Y + 1);
			Tile.SmoothSlope(trackHistory.X, trackHistory.Y - PlayerHeight);
			Tile tile = Main.tile[trackHistory.X, trackHistory.Y];
			bool wire = tile.RedWire;
			if (array[k] && k < CurrentTrack.Count && k > 0 && CurrentTrack[k - 1].Y == trackHistory.Y && CurrentTrack[k + 1].Y == trackHistory.Y)
			{
				tile.ClearEverything();
				WorldGen.PlaceTile(trackHistory.X, trackHistory.Y, 314, false, true, -1, 1);
			}
			else
			{
				tile.ResetToType(314);
			}

			tile.RedWire = wire;
			if (k == 0)
				continue;

			for (int l = 0; l < 8; l++) WorldUtils.TileFrame(CurrentTrack[k - 1].X, CurrentTrack[k - 1].Y - l, true);

			if (k == CurrentTrack.Count - 1)
				for (int m = 0; m < PlayerHeight; m++)
					WorldUtils.TileFrame(trackHistory.X, trackHistory.Y - m, true);
		}
	}

	public void RegisterBadRectangle()
	{
		RTree.Insert(new Rectangle(CurrentTrackRectangle.X - 5, CurrentTrackRectangle.Y - 5, CurrentTrackRectangle.Width + 10, CurrentTrackRectangle.Height + 10));
	}

	private void AppendToHistory(TrackSlope slope, TrackMode mode = TrackMode.Normal)
	{
		TrackHistory trackHistory = CurrentTrack[^1];
		int x = trackHistory.X + _xDirection;
		int y = trackHistory.Y + (int)slope;
		CurrentTrack.Add(new TrackHistory(x, y, slope)
		{
			Mode = mode
		});

		if (CurrentTrackRectangle.Left > x)
			CurrentTrackRectangle.X -= 1;
		else if (CurrentTrackRectangle.Right < x)
			CurrentTrackRectangle.X += 1;
		if (CurrentTrackRectangle.Top > y)
			CurrentTrackRectangle.Y -= 1;
		else if (CurrentTrackRectangle.Bottom < y)
			CurrentTrackRectangle.Y += 1;
	}

	private TrackPlacementState TryRewriteHistoryToAvoidTiles()
	{
		int num = CurrentTrack.Count - 1;
		int num2 = Math.Min(CurrentTrack.Count, _rewriteHistory.Length);
		for (int i = 0; i < num2; i++)
			_rewriteHistory[i] = CurrentTrack[num - i];

		while (num >= CurrentTrack.Count - num2)
		{
			if (CurrentTrack[num].Slope == TrackSlope.Down)
			{
				TrackPlacementState historySegmentPlacementState = GetHistorySegmentPlacementState(num, CurrentTrack.Count - num);

				if (historySegmentPlacementState == TrackPlacementState.Available)
					return historySegmentPlacementState;

				RewriteSlopeDirection(num, TrackSlope.Straight);
			}

			num--;
		}

		if (GetHistorySegmentPlacementState(num + 1, CurrentTrack.Count - (num + 1)) == TrackPlacementState.Available)
			return TrackPlacementState.Available;

		for (num = CurrentTrack.Count - 1; num >= CurrentTrack.Count - num2 + 1; num--)
			if (CurrentTrack[num].Slope == TrackSlope.Straight)
			{
				TrackPlacementState historySegmentPlacementState2 = GetHistorySegmentPlacementState(CurrentTrack.Count - num2, num2);
				if (historySegmentPlacementState2 == TrackPlacementState.Available)
					return historySegmentPlacementState2;

				RewriteSlopeDirection(num, TrackSlope.Up);
			}

		for (int j = 0; j < num2; j++) CurrentTrack[CurrentTrack.Count - 1 - j] = _rewriteHistory[j];

		RewriteSlopeDirection(CurrentTrack.Count - 1, TrackSlope.Straight);
		return GetHistorySegmentPlacementState(num + 1, CurrentTrack.Count - (num + 1));
	}

	private void RewriteSlopeDirection(int index, TrackSlope slope)
	{
		TrackHistory trackHistory = CurrentTrack[index];
		int num = slope - trackHistory.Slope;
		trackHistory.Slope = slope;
		CurrentTrack[index] = trackHistory;
		for (int i = index; i < CurrentTrack.Count; i++)
		{
			TrackHistory history = CurrentTrack[i];
			history.Y += (short)num;
			CurrentTrack[i] = history;
		}
	}

	private TrackPlacementState GetHistorySegmentPlacementState(int startIndex, int length)
	{
		TrackPlacementState result = TrackPlacementState.Available;
		for (int i = startIndex; i < startIndex + length; i++)
		{
			TrackPlacementState trackPlacementState = CalculateStateForLocation(CurrentTrack[i].X, CurrentTrack[i].Y);
			switch (trackPlacementState)
			{
				case TrackPlacementState.Invalid:
					return TrackPlacementState.Invalid;
				case TrackPlacementState.Obstructed:
					if (CurrentTrack[i].Mode != TrackMode.Tunnel)
						result = TrackPlacementState.Obstructed;
					break;
			}
		}

		return result;
	}

	private void SmoothTrack()
	{
		int num = CurrentTrack.Count - 1;
		bool flag = false;
		for (int num2 = CurrentTrack.Count - 1; num2 >= 0; num2--)
			if (flag)
			{
				num = Math.Min(num2 + 15, num);
				if (CurrentTrack[num2].Y >= CurrentTrack[num].Y)
				{
					for (int i = num2 + 1; CurrentTrack[i].Y > CurrentTrack[num2].Y; i++)
					{
						TrackHistory trackHistory = CurrentTrack[i];
						trackHistory.Y = CurrentTrack[num2].Y;
						trackHistory.Slope = TrackSlope.Straight;
						CurrentTrack[i] = trackHistory;
					}

					if (CurrentTrack[num2].Y == CurrentTrack[num].Y)
						flag = false;
				}
			}
			else if (CurrentTrack[num2].Y > CurrentTrack[num].Y)
			{
				flag = true;
			}
			else
			{
				num = num2;
			}
	}

	private static bool CanSlopesTouch(TrackSlope leftSlope, TrackSlope rightSlope)
	{
		if (leftSlope != rightSlope && leftSlope != 0)
			return rightSlope == TrackSlope.Straight;

		return true;
	}

	private TrackPlacementState CalculateStateForLocation(int x, int y)
	{
		for (int i = 0; i < PlayerHeight; i++)
			if (IsLocationInvalid(x, y - i))
				return TrackPlacementState.Invalid;

		for (int k = 0; k < PlayerHeight; k++)
			if (WorldGen.SolidTile(x, y - k))
				return TrackPlacementState.Obstructed;

		if (Sample++ % 5 == 0 && RTree.Contains(x, y))
			return TrackPlacementState.Invalid;

		return TrackPlacementState.Available;
	}

	private static bool IsLocationInvalid(int x, int y)
	{
		if (y > Main.UnderworldLayer || x < 5 || y < (int)Main.worldSurface || x > Main.maxTilesX - 5)
			return true;

		if (WorldGen.oceanDepths(x, y))
			return true;

		ushort wall = Main.tile[x, y].WallType;
		if (InvalidWalls.Contains(wall)) return true;

		ushort type = Main.tile[x, y].TileType;
		if (InvalidTiles.Contains(type)) return true;

		for (int k = -1; k <= 1; k++)
			if (Main.tile[x + k, y].HasTile && (Main.tile[x + k, y].TileType == 314 || !TileID.Sets.GeneralPlacementTiles[Main.tile[x + k, y].TileType]) && !(WorldGen.notTheBees && Main.tile[x + k, y].TileType == 225))
				return true;

		return false;
	}

	private enum TrackPlacementState
	{
		Available,
		Obstructed,
		Invalid
	}

	[DebuggerDisplay("X = {X}, Y = {Y}, Slope = {Slope}")]
	public struct TrackHistory
	{
		public short X;
		public short Y;
		public TrackSlope Slope;
		public TrackMode Mode;

		public TrackHistory(int x, int y, TrackSlope slope)
		{
			X = (short)x;
			Y = (short)y;
			Slope = slope;
			Mode = TrackMode.Normal;
		}
	}
}