namespace AdvancedWorldGen.SpecialOptions._100k_special.InvertedPyramid;

public class GuideHouse
{
	private int x;
	private int holeLeft;
	private int holeRight;
	private int y;

	public GuideHouse(int holeLeft, int holeRight, int floorY)
	{
		this.holeLeft = holeLeft;
		this.holeRight = holeRight;
		y = floorY;
	}

	public enum HouseTheme
	{
		Classic,
		Rustic,
		Modern,
		Medieval,
		Futuristic,
		Gothic
	}

	private HouseTheme Theme;
	private int Width;
	private int Height;

	public void GenerateHouse()
	{
		// Step 2: Set the house theme based on the location and biome
		SetHouseTheme();

		SetSize();

		// Step 3: Build the frame of the house
		BuildHouseFrame();

		// Step 5: Build the roof
		BuildRoof();

		AddDoors();

		// Step 7: Add random furniture, decoration, lights and a sign
		AddFurnitureAndDecoration();

		Main.spawnTileX = x + Width / 2;
		Main.spawnTileY = y - 3;
	}

	private void SetHouseTheme()
	{
		int themeIndex = WorldGen.genRand.Next(0, Enum.GetNames(typeof(HouseTheme)).Length);
		Theme = (HouseTheme)themeIndex;
	}

	private void SetSize()
	{
		// Calculate the minimum and maximum width and height based on the theme
		int minWidth, maxWidth, minHeight, maxHeight;

		switch (Theme)
		{
			case HouseTheme.Classic:
				minWidth = 15;
				maxWidth = 25;
				minHeight = 10;
				maxHeight = 15;
				break;
			case HouseTheme.Rustic:
				minWidth = 20;
				maxWidth = 30;
				minHeight = 12;
				maxHeight = 18;
				break;
			case HouseTheme.Modern:
				minWidth = 12;
				maxWidth = 20;
				minHeight = 8;
				maxHeight = 12;
				break;
			case HouseTheme.Medieval:
				minWidth = 25;
				maxWidth = 35;
				minHeight = 15;
				maxHeight = 20;
				break;
			case HouseTheme.Futuristic:
				minWidth = 15;
				maxWidth = 25;
				minHeight = 10;
				maxHeight = 15;
				break;
			case HouseTheme.Gothic:
				minWidth = 20;
				maxWidth = 30;
				minHeight = 12;
				maxHeight = 18;
				break;
			default:
				minWidth = 15;
				maxWidth = 25;
				minHeight = 10;
				maxHeight = 15;
				break;
		}

		// Set the width and height of the house to a random value within the range
		Width = Main.rand.Next(minWidth, maxWidth + 1);
		Height = Main.rand.Next(minHeight, maxHeight + 1);
		x = (holeLeft + holeRight) / 2 - Width / 2;
	}

	private void BuildHouseFrame()
	{
		// Step 4: Build the main structure of the house
		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				// Place the walls on the edges of the house
				if (i == 0 || i == Width - 1)
				{
					PlaceTile(x + i, y - j);
				}
				else if (j == 0 || j == Height - 1)
				{
					PlacePlatform(x + i, y - j);
				}
				else
				{
					PlaceWall(x + i, y - j);
				}
			}
		}
	}

	private void BuildRoof()
	{
		const int roofHeight = 6;
		int roofX = x - 1;
		int roofY = y - Height;

		for (int i = 0; i < Width + 2; i++)
		{
			int yOffset = (int)(Math.Sin((double)i / (double)Width * Math.PI * 2) * roofHeight);

			for (int j = 0; j < roofHeight + yOffset; j++)
			{
				PlaceTile(roofX + i, roofY - j);
			}
		}
	}


	public void AddDoors()
	{
		int doorWidth = 3; // The width of the door, including the frame

		// Calculate the left and right positions for the doors
		int leftDoorX = x;
		int rightDoorX = x + Width - 1;

		// Determine the style of the doors based on the house theme
		int doorStyle = Theme switch
		{
			HouseTheme.Classic => 0 // Wooden Door
			,
			HouseTheme.Rustic => 2 // Rich Mahogany Door
			,
			HouseTheme.Modern => 20 // Glass Door
			,
			HouseTheme.Medieval => 19 // Obsidian Door
			,
			HouseTheme.Futuristic => 32 // Martian Door
			,
			HouseTheme.Gothic => 5 // Flesh Door
			,
			_ => throw new ArgumentException("Invalid HouseTheme value.")
		};

		// Clear the tiles where the doors will be placed
		for (int i = 0; i < doorWidth; i++)
		{
			WorldGen.KillTile(leftDoorX, y - i - 1);
			WorldGen.KillTile(rightDoorX, y - i - 1);
		}

		// Place the doors
		WorldGen.PlaceTile(leftDoorX, y - 1, TileID.ClosedDoor, style: doorStyle);
		WorldGen.PlaceTile(rightDoorX, y - 1, TileID.ClosedDoor, style: doorStyle);
	}


	public void AddFurnitureAndDecoration()
	{
		// Add a sign
		int signX = x + Width / 2;
		int signY = y - Height + 3;
		WorldGen.PlaceTile(signX, signY, TileID.Signs, true, style: 0);
		int index = Sign.ReadSign(signX, signY);
		if (index >= 0)
			Sign.TextSign(index, "Made by ChatGPT");
		
		// Add furniture
		for (int i = x + 1; i < x + Width - 1; i++)
		{
			for (int j = y - 1; j > y - Height + 1; j--)
			{
				if (Main.rand.Next(10) == 0)
				{
					// 1 in 10 chance of adding furniture to this tile
					int furnitureType = Main.rand.Next(4); // There are 4 furniture types
					switch (furnitureType)
					{
						case 0:
							// Chair
							WorldGen.PlaceTile(i, j, TileID.Chairs, true, style: (byte)Main.rand.Next(13));
							break;
						case 1:
							// Table
							WorldGen.PlaceTile(i, j, TileID.Tables, true, style: (byte)Main.rand.Next(22));
							break;
						case 2:
							// Bookcase
							WorldGen.PlaceTile(i, j, TileID.Bookcases, true, style: (byte)Main.rand.Next(6));
							break;
						case 3:
							// Dresser
							WorldGen.PlaceTile(i, j, TileID.Dressers, true, style: (byte)Main.rand.Next(12));
							break;
					}
				}
			}
		}

		// Add decoration
		for (int i = x + 1; i < x + Width - 1; i++)
		{
			for (int j = y - 1; j > y - Height + 1; j--)
			{
				if (Main.rand.Next(20) == 0)
				{
					// 1 in 20 chance of adding decoration to this tile
					int decorationType = Main.rand.Next(3); // There are 3 decoration types
					switch (decorationType)
					{
						case 0:
							// Painting
							WorldGen.PlaceTile(i, j,
								Main.rand.Next(new int[]
								{
									TileID.Painting2X3, TileID.Painting3X2, TileID.Painting3X3, TileID.Painting4X3,
									TileID.Painting6X4
								}), true, style: (byte)Main.rand.Next(32));
							break;
						case 1:
							// Statue
							WorldGen.PlaceTile(i, j, TileID.Statues, true, style: (byte)Main.rand.Next(36));
							break;
						case 2:
							// Torch
							WorldGen.PlaceTile(i, j, TileID.Torches, true);
							break;
					}
				}
			}
		}

		// Add lights
		for (int i = x + 1; i < x + Width - 1; i++)
		{
			for (int j = y - 1; j > y - Height + 1; j--)
			{
				if (Main.rand.Next(50) == 0)
				{
					// 1 in 50 chance of adding light to this tile
					WorldGen.PlaceTile(i, j, TileID.Lamps);
				}
			}
		}
	}

	public void PlaceTile(int x, int y)
	{
		// Place a tile based on the current theme
		switch (Theme)
		{
			case HouseTheme.Classic:
				WorldGen.PlaceTile(x, y, TileID.MarbleBlock);
				break;
			case HouseTheme.Rustic:
				WorldGen.PlaceTile(x, y, TileID.WoodBlock);
				break;
			case HouseTheme.Modern:
				WorldGen.PlaceTile(x, y, TileID.GrayBrick);
				break;
			case HouseTheme.Medieval:
				WorldGen.PlaceTile(x, y, TileID.Stone);
				break;
			case HouseTheme.Futuristic:
				WorldGen.PlaceTile(x, y, TileID.MartianConduitPlating);
				break;
			case HouseTheme.Gothic:
				WorldGen.PlaceTile(x, y, TileID.Ebonstone);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void PlacePlatform(int x, int y)
	{
		int style = Theme switch
		{
			HouseTheme.Classic => 0 // Wood Platform
			,
			HouseTheme.Rustic => 2 // Rich Mahogany Platform
			,
			HouseTheme.Modern => 9 // Metal Shelf
			,
			HouseTheme.Medieval => 43 // Stone Platform
			,
			HouseTheme.Futuristic => 14 // Glass Platform
			,
			HouseTheme.Gothic => 13 // Obsidian Platform
			,
			_ => throw new ArgumentException("Invalid HouseTheme value.")
		};
		WorldGen.PlaceTile(x, y, TileID.Platforms, style: style);
	}


	public void PlaceWall(int x, int y)
	{
		// Place a wall based on the current theme
		switch (Theme)
		{
			case HouseTheme.Classic:
				WorldGen.PlaceWall(x, y, WallID.Marble);
				break;
			case HouseTheme.Rustic:
				WorldGen.PlaceWall(x, y, WallID.Wood);
				break;
			case HouseTheme.Modern:
				WorldGen.PlaceWall(x, y, WallID.GrayBrick);
				break;
			case HouseTheme.Medieval:
				WorldGen.PlaceWall(x, y, WallID.Stone);
				break;
			case HouseTheme.Futuristic:
				WorldGen.PlaceWall(x, y, WallID.MartianConduit);
				break;
			case HouseTheme.Gothic:
				WorldGen.PlaceWall(x, y, WallID.Ebonwood);
				break;
		}
	}
}