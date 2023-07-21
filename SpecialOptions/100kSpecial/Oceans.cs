namespace AdvancedWorldGen.SpecialOptions._100kSpecial;

public class Oceans : ControlledWorldGenPass
{
	public Oceans() : base("Oceans", 0f)
	{
	}

	protected override void ApplyPass()
	{
		int floodedLeft;
		int floodedRight;
		int dryLeft;
		int dryRight;
		int bubbleColumn;
		if (_random.NextBool(2))
		{
			floodedLeft = 0;
			floodedRight = GenVars.leftBeachEnd;
			bubbleColumn = GenVars.leftBeachEnd + 1;
			dryLeft = GenVars.rightBeachStart;
			dryRight = Main.maxTilesX - 1;
		}
		else
		{
			dryLeft = 0;
			dryRight = GenVars.leftBeachEnd;
			floodedLeft = GenVars.rightBeachStart;
			floodedRight = Main.maxTilesX - 1;
			bubbleColumn = GenVars.rightBeachStart - 1;
		}

		#region flood

		int y = 10;
		while (true)
		{
			bool airFound = false;
			for (int x = floodedLeft; x <= floodedRight; x++)
			{
				Tile tile = Main.tile[x, y];
				if (!tile.HasTile)
				{
					tile.LiquidAmount = byte.MaxValue;
					tile.LiquidType = LiquidID.Water;
					airFound = true;
				}
			}

			if (!airFound || y == Main.rockLayer)
			{
				break;
			}

			y++;
		}

		#endregion

		#region bubbles

		y = 10;
		while (true)
		{
			Tile tile = Main.tile[bubbleColumn, y];
			if (tile.HasTile)
				break;

			tile.ResetToType(TileID.Bubble);
			y++;
		}

		#endregion

		#region drought

		y = 10;
		while (true)
		{
			bool airFound = false;
			for (int x = dryLeft; x <= dryRight; x++)
			{
				Tile tile = Main.tile[x, y];
				if (!tile.HasTile)
				{
					tile.LiquidAmount = 0;
					airFound = true;
				}
			}

			if (!airFound || y == Main.rockLayer)
			{
				break;
			}

			y++;
		}

		#endregion
	}
}