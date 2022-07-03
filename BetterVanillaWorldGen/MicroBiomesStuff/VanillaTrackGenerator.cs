using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;
namespace AdvancedWorldGen.BetterVanillaWorldGen.MicroBiomesStuff;

public class VanillaTrackGenerator
	{
		private enum TrackPlacementState
		{
			Available,
			Obstructed,
			Invalid
		}

		private enum TrackSlope : sbyte
		{
			Up = -1,
			Straight,
			Down
		}

		private enum TrackMode : byte
		{
			Normal,
			Tunnel
		}

		[DebuggerDisplay("X = {X}, Y = {Y}, Slope = {Slope}")]
		private struct TrackHistory
		{
			public short X;
			public short Y;
			public TrackSlope Slope;
			public TrackMode Mode;

			public TrackHistory(int x, int y, TrackSlope slope) {
				X = (short)x;
				Y = (short)y;
				Slope = slope;
				Mode = TrackMode.Normal;
			}
		}

		private static readonly ushort[] InvalidWalls = new ushort[20] {
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
		private static readonly ushort[] InvalidTiles = new ushort[22] {
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

		private readonly List<TrackHistory> CurrentTrack = new();
		private readonly TrackHistory[] _rewriteHistory = new TrackHistory[25];
		private int _xDirection;
		private int _length;
		private int playerHeight = 6;

		public bool Place(Point origin, int minLength, int maxLength) {
			if (!FindSuitableOrigin(ref origin))
				return false;

			CreateTrackStart(origin);
			if (!FindPath(minLength, maxLength))
				return false;

			PlacePath();
			return true;
		}

		private void PlacePath() {
			bool[] array = new bool[_length];
			for (int i = 0; i < _length; i++) {
				if (WorldGen.genRand.Next(7) == 0)
					playerHeight = WorldGen.genRand.Next(5, 9);

				for (int j = 0; j < playerHeight; j++) {
					if (Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j - 1].WallType == 244)
						Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j - 1].WallType = 0;

					if (Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j].WallType == 244)
						Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j].WallType = 0;

					if (Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j + 1].WallType == 244)
						Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j + 1].WallType = 0;

					if (Main.tile[CurrentTrack[i].X, CurrentTrack[i].Y - j].TileType == 135)
						array[i] = true;

					WorldGen.KillTile(CurrentTrack[i].X, CurrentTrack[i].Y - j, fail: false, effectOnly: false, noItem: true);
				}
			}

			for (int k = 0; k < _length; k++) {
				if (WorldGen.genRand.Next(7) == 0)
					playerHeight = WorldGen.genRand.Next(5, 9);

				TrackHistory trackHistory = CurrentTrack[k];
				Tile.SmoothSlope(trackHistory.X, trackHistory.Y + 1);
				Tile.SmoothSlope(trackHistory.X, trackHistory.Y - playerHeight);
				Tile tile = Main.tile[trackHistory.X, trackHistory.Y];
				bool wire = tile.RedWire;
				if (array[k] && k < _length && k > 0 && CurrentTrack[k - 1].Y == trackHistory.Y && CurrentTrack[k + 1].Y == trackHistory.Y) {
					tile.ClearEverything();
					WorldGen.PlaceTile(trackHistory.X, trackHistory.Y, 314, mute: false, forced: true, -1, 1);
				}
				else {
					tile.ResetToType(314);
				}

				tile.RedWire = wire;
				if (k == 0)
					continue;

				for (int l = 0; l < 8; l++) {
					WorldUtils.TileFrame(CurrentTrack[k - 1].X, CurrentTrack[k - 1].Y - l, frameNeighbors: true);
				}

				if (k == _length - 1) {
					for (int m = 0; m < playerHeight; m++) {
						WorldUtils.TileFrame(trackHistory.X, trackHistory.Y - m, frameNeighbors: true);
					}
				}
			}
		}

		private void CreateTrackStart(Point origin) {
			_xDirection = ((origin.X <= Main.maxTilesX / 2) ? 1 : (-1));
			_length = 1;
			CurrentTrack.Clear();
			for (int i = 0; i < 1; i++) {
				CurrentTrack.Add(new TrackHistory(origin.X + i * _xDirection, origin.Y + i, TrackSlope.Down));
			}
		}

		private bool FindPath(int minLength, int maxLength) {
			int length = _length;
			while (_length < 4096 - 100) {
				TrackSlope slope = (CurrentTrack[_length - 1].Slope != TrackSlope.Up) ? TrackSlope.Down : TrackSlope.Straight;
				AppendToHistory(slope);
				TrackPlacementState trackPlacementState = TryRewriteHistoryToAvoidTiles();
				if (trackPlacementState == TrackPlacementState.Invalid)
					break;

				length = _length;
				TrackPlacementState trackPlacementState2 = trackPlacementState;
				while (trackPlacementState2 != 0) {
					trackPlacementState2 = CreateTunnel();
					if (trackPlacementState2 == TrackPlacementState.Invalid)
						break;

					length = _length;
				}

				if (_length >= maxLength)
					break;
			}

			_length = Math.Min(maxLength, length);
			if (_length < minLength)
				return false;

			SmoothTrack();
			return GetHistorySegmentPlacementState(0, _length) != TrackPlacementState.Invalid;
		}

		private TrackPlacementState CreateTunnel() {
			TrackSlope trackSlope = TrackSlope.Straight;
			int num = 10;
			TrackPlacementState trackPlacementState = TrackPlacementState.Invalid;
			TrackHistory trackHistory = CurrentTrack[_length - 1];
			int x = trackHistory.X;
			int y = trackHistory.Y;
			for (TrackSlope trackSlope2 = TrackSlope.Up; trackSlope2 <= TrackSlope.Down; trackSlope2++) {
				TrackPlacementState trackPlacementState2 = TrackPlacementState.Invalid;
				for (int i = 1; i < num; i++) {
					trackPlacementState2 = CalculateStateForLocation(x + i * _xDirection, y + i * (int)trackSlope2);
					switch (trackPlacementState2) {
						case TrackPlacementState.Available:
							trackSlope = trackSlope2;
							num = i;
							trackPlacementState = trackPlacementState2;
							break;
						case TrackPlacementState.Obstructed:
							continue;
						case TrackPlacementState.Invalid:
							break;
					}

					break;
				}

				if (trackPlacementState != 0 && trackPlacementState2 == TrackPlacementState.Obstructed && (trackPlacementState != TrackPlacementState.Obstructed || trackSlope != 0)) {
					trackSlope = trackSlope2;
					num = 10;
					trackPlacementState = trackPlacementState2;
				}
			}

			if (_length == 0 || !CanSlopesTouch(trackHistory.Slope, trackSlope))
				RewriteSlopeDirection(_length - 1, TrackSlope.Straight);

			trackHistory = CurrentTrack[_length - 1];
			trackHistory.Mode = TrackMode.Tunnel;
			CurrentTrack[_length - 1] = trackHistory;
			for (int j = 1; j < num; j++) {
				AppendToHistory(trackSlope, TrackMode.Tunnel);
			}

			return trackPlacementState;
		}

		private void AppendToHistory(TrackSlope slope, TrackMode mode = TrackMode.Normal) {
			CurrentTrack.Add(new TrackHistory(CurrentTrack[_length - 1].X + _xDirection, (int)CurrentTrack[_length - 1].Y + (int)slope, slope));
			TrackHistory trackHistory = CurrentTrack[_length];
			trackHistory.Mode = mode;
			CurrentTrack[_length] = trackHistory;
			_length++;
		}

		private TrackPlacementState TryRewriteHistoryToAvoidTiles() {
			int num = _length - 1;
			int num2 = Math.Min(_length, _rewriteHistory.Length);
			for (int i = 0; i < num2; i++) {
				_rewriteHistory[i] = CurrentTrack[num - i];
			}

			while (num >= _length - num2) {
				if (CurrentTrack[num].Slope == TrackSlope.Down) {
					TrackPlacementState historySegmentPlacementState = GetHistorySegmentPlacementState(num, _length - num);
					if (historySegmentPlacementState == TrackPlacementState.Available)
						return historySegmentPlacementState;

					RewriteSlopeDirection(num, TrackSlope.Straight);
				}

				num--;
			}

			if (GetHistorySegmentPlacementState(num + 1, _length - (num + 1)) == TrackPlacementState.Available)
				return TrackPlacementState.Available;

			for (num = _length - 1; num >= _length - num2 + 1; num--) {
				if (CurrentTrack[num].Slope == TrackSlope.Straight) {
					TrackPlacementState historySegmentPlacementState2 = GetHistorySegmentPlacementState(_length - num2, num2);
					if (historySegmentPlacementState2 == TrackPlacementState.Available)
						return historySegmentPlacementState2;

					RewriteSlopeDirection(num, TrackSlope.Up);
				}
			}

			for (int j = 0; j < num2; j++) {
				CurrentTrack[_length - 1 - j] = _rewriteHistory[j];
			}

			RewriteSlopeDirection(_length - 1, TrackSlope.Straight);
			return GetHistorySegmentPlacementState(num + 1, _length - (num + 1));
		}

		private void RewriteSlopeDirection(int index, TrackSlope slope) {
			TrackHistory history = CurrentTrack[index];
			int num = slope - history.Slope;
			history.Slope = slope;
			CurrentTrack[index] = history;
			for (int i = index; i < _length; i++)
			{
				TrackHistory trackHistory = CurrentTrack[i];
				trackHistory.Y += (short)num;
				CurrentTrack[i] = trackHistory;
			}
		}

		private TrackPlacementState GetHistorySegmentPlacementState(int startIndex, int length) {
			TrackPlacementState result = TrackPlacementState.Available;
			for (int i = startIndex; i < startIndex + length; i++) {
				TrackPlacementState trackPlacementState = CalculateStateForLocation(CurrentTrack[i].X, CurrentTrack[i].Y);
				switch (trackPlacementState) {
					case TrackPlacementState.Invalid:
						return trackPlacementState;
					case TrackPlacementState.Obstructed:
						if (CurrentTrack[i].Mode != TrackMode.Tunnel)
							result = trackPlacementState;
						break;
				}
			}

			return result;
		}

		private void SmoothTrack() {
			int num = _length - 1;
			bool flag = false;
			for (int num2 = _length - 1; num2 >= 0; num2--) {
				if (flag) {
					num = Math.Min(num2 + 15, num);
					if (CurrentTrack[num2].Y >= CurrentTrack[num].Y) {
						for (int i = num2 + 1; CurrentTrack[i].Y > CurrentTrack[num2].Y; i++) {
							TrackHistory trackHistory = CurrentTrack[i];
							trackHistory.Y = CurrentTrack[num2].Y;
							trackHistory.Slope = TrackSlope.Straight;
							CurrentTrack[i] = trackHistory;
						}

						if (CurrentTrack[num2].Y == CurrentTrack[num].Y)
							flag = false;
					}
				}
				else if (CurrentTrack[num2].Y > CurrentTrack[num].Y) {
					flag = true;
				}
				else {
					num = num2;
				}
			}
		}

		private static bool CanSlopesTouch(TrackSlope leftSlope, TrackSlope rightSlope) {
			if (leftSlope != rightSlope && leftSlope != 0)
				return rightSlope == TrackSlope.Straight;

			return true;
		}

		private static bool FindSuitableOrigin(ref Point origin) {
			TrackPlacementState trackPlacementState;
			while ((trackPlacementState = CalculateStateForLocation(origin.X, origin.Y)) != TrackPlacementState.Obstructed) {
				origin.Y++;
				if (trackPlacementState == TrackPlacementState.Invalid)
					return false;
			}

			origin.Y--;
			return CalculateStateForLocation(origin.X, origin.Y) == TrackPlacementState.Available;
		}

		private static TrackPlacementState CalculateStateForLocation(int x, int y) {
			for (int i = 0; i < 6; i++) {
				if (IsLocationInvalid(x, y - i))
					return TrackPlacementState.Invalid;
			}

			for (int j = 0; j < 6; j++) {
				if (IsMinecartTrack(x, y + j))
					return TrackPlacementState.Invalid;
			}

			for (int k = 0; k < 6; k++) {
				if (WorldGen.SolidTile(x, y - k))
					return TrackPlacementState.Obstructed;
			}

			if (WorldGen.IsTileNearby(x, y, 314, 30))
				return TrackPlacementState.Invalid;

			return TrackPlacementState.Available;
		}

		private static bool IsMinecartTrack(int x, int y) {
			if (Main.tile[x, y].HasTile)
				return Main.tile[x, y].TileType == 314;

			return false;
		}

		private static bool IsLocationInvalid(int x, int y) {
			if (y > Main.UnderworldLayer || x < 5 || y < (int)Main.worldSurface || x > Main.maxTilesX - 5)
				return true;

			if (WorldGen.oceanDepths(x, y))
				return true;

			ushort WallType = Main.tile[x, y].WallType;
			for (int i = 0; i < InvalidWalls.Length; i++) {
				if (WallType == InvalidWalls[i] && (!WorldGen.notTheBees || WallType != 108))
					return true;
			}

			ushort TileType = Main.tile[x, y].TileType;
			for (int j = 0; j < InvalidTiles.Length; j++) {
				if (TileType == InvalidTiles[j])
					return true;
			}

			for (int k = -1; k <= 1; k++) {
				if (Main.tile[x + k, y].HasTile && (Main.tile[x + k, y].TileType == 314 || !TileID.Sets.GeneralPlacementTiles[Main.tile[x + k, y].TileType]) && (!WorldGen.notTheBees || Main.tile[x + k, y].TileType != 225))
					return true;
			}

			return false;
		}

		[Conditional("DEBUG")]
		private void DrawPause() {
		}
	}