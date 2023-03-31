namespace AdvancedWorldGen.BetterVanillaWorldGen;

public class TerrainPass : ControlledWorldGenPass
{
	private enum TerrainFeatureType
	{
		Plateau,
		Hill,
		Dale,
		Mountain,
		Valley
	}

	public TerrainPass() : base("Terrain", 449.3722f)
	{
	}

	protected override void ApplyPass()
	{
		int lowDepthBeachSize = Configuration.Get<int>("FlatBeachPadding");
		Progress.Message = Language.GetTextValue("LegacyWorldGen.0");
		TerrainFeatureType terrainFeatureType = TerrainFeatureType.Plateau;
		int worldSurface = (int)(Main.maxTilesY * 0.3 * WorldGen.genRand.Next(90, 110) * 0.005);
		int rockLayer;

		if (WorldGen.remixWorldGen)
		{
			float modifier;
			if (Main.maxTilesX > 2500)
			{
				modifier = 0.6f;
			}
			else
			{
				modifier = 0.5f;
			}
			rockLayer = (int)(Main.maxTilesY * modifier * WorldGen.genRand.Next(95, 106) * 0.01f);
		}
		else
		{
			rockLayer = worldSurface + (int)(Main.maxTilesY * 0.2 * WorldGen.genRand.Next(90, 110) * 0.01);
		}
		
		if (rockLayer < worldSurface + Main.maxTilesY * 0.05)
		{
			if (worldSurface - rockLayer > Main.maxTilesY * 0.05)
				worldSurface -= 2 * (worldSurface - rockLayer);
			else
				worldSurface = (int)(rockLayer - Main.maxTilesY * 0.05);
		}

		if (worldSurface < Main.maxTilesY * 0.07)
			worldSurface = (int)(Main.maxTilesY * 0.07);
		int worldSurfaceMin = (int)(Main.maxTilesY * (Params.TerrainType is TerrainType.SkyPillars ? 0.05 : 0.17));
		int worldSurfaceMax = (int)(Main.maxTilesY * (Params.TerrainType is TerrainType.SkyPillars ? 0.40 : 0.23));
		int totalBeachSize = GenVars.leftBeachEnd + lowDepthBeachSize;
		Stopwatch?.Stop();
		AdvancedWorldGenMod.Instance.UiChanger.Stopwatch.Stop();
		Dictionary<string, object> currentState = new()
		{
			{ nameof(GenVars.leftBeachEnd), GenVars.leftBeachEnd },
			{ nameof(GenVars.rightBeachStart), GenVars.rightBeachStart },
			{ nameof(lowDepthBeachSize), lowDepthBeachSize },
			{ nameof(worldSurface), worldSurface },
			{ nameof(rockLayer), rockLayer },
			{ nameof(worldSurfaceMin), worldSurfaceMin },
			{ nameof(worldSurfaceMax), worldSurfaceMax },
			{ nameof(totalBeachSize), totalBeachSize }
		};
		DrawTerrainUI(currentState);
		GenVars.leftBeachEnd = (int)currentState[nameof(GenVars.leftBeachEnd)];
		GenVars.rightBeachStart = (int)currentState[nameof(GenVars.rightBeachStart)];
		lowDepthBeachSize = (int)currentState[nameof(lowDepthBeachSize)];
		worldSurface = (int)currentState[nameof(worldSurface)];
		rockLayer = (int)currentState[nameof(rockLayer)];
		worldSurfaceMax = (int)currentState[nameof(worldSurfaceMax)];
		totalBeachSize = (int)currentState[nameof(totalBeachSize)];
		Stopwatch?.Start();
		AdvancedWorldGenMod.Instance.UiChanger.Stopwatch.Start();

		double worldSurfaceLow = worldSurface;
		double worldSurfaceHigh = worldSurface;
		double rockLayerLow = rockLayer;
		double rockLayerHigh = rockLayer;
		SurfaceHistory surfaceHistory = new(500);
		List<(int, int)> bogoValues = new(); // Only used in bogo
		for (int i = 0; i < Main.maxTilesX; i++)
		{
			Progress.Set(i / (float)Main.maxTilesX);
			worldSurfaceLow = Math.Min(worldSurface, worldSurfaceLow);
			worldSurfaceHigh = Math.Max(worldSurface, worldSurfaceHigh);
			rockLayerLow = Math.Min(rockLayer, rockLayerLow);
			rockLayerHigh = Math.Max(rockLayer, rockLayerHigh);
			if (totalBeachSize <= 0)
			{
				terrainFeatureType = (TerrainFeatureType)WorldGen.genRand.Next(0, 5);
				totalBeachSize = WorldGen.genRand.Next(5, 40);
				if (terrainFeatureType == TerrainFeatureType.Plateau)
					totalBeachSize *= (int)(WorldGen.genRand.Next(5, 30) * 0.2);
			}

			totalBeachSize--;
			if (i > Main.maxTilesX * 0.48 && i < Main.maxTilesX * 0.52)
				terrainFeatureType = TerrainFeatureType.Plateau;
			else if (i > Main.maxTilesX * 0.45 && i < Main.maxTilesX * 0.55 &&
			         terrainFeatureType is TerrainFeatureType.Mountain or TerrainFeatureType.Valley)
				terrainFeatureType = (TerrainFeatureType)WorldGen.genRand.Next(3);

			worldSurface += GenerateWorldSurfaceOffset(terrainFeatureType);
			float num10 = 0.17f;
			float num11 = 0.26f;
			if (WorldGen.drunkWorldGen)
			{
				num10 = 0.15f;
				num11 = 0.28f;
			}

			if (Params.TerrainType == TerrainType.SkyPillars)
			{
				num10 = 0.05f;
				num11 = 0.40f;
			}

			if (i < GenVars.leftBeachEnd + lowDepthBeachSize || i > GenVars.rightBeachStart - lowDepthBeachSize)
			{
				worldSurface = Utils.Clamp(worldSurface, worldSurfaceMin, worldSurfaceMax);
			}
			else if (worldSurface < Main.maxTilesY * num10)
			{
				worldSurface = (int)(Main.maxTilesY * num10);
				totalBeachSize = 0;
			}
			else if (worldSurface > Main.maxTilesY * num11)
			{
				worldSurface = (int)(Main.maxTilesY * num11);
				totalBeachSize = 0;
			}

			while (WorldGen.genRand.NextBool(3)) rockLayer += WorldGen.genRand.Next(-2, 3);

			if (rockLayer < worldSurface + Main.maxTilesY * 0.06)
				rockLayer += 1;

			if (rockLayer > worldSurface + Main.maxTilesY * 0.35)
				rockLayer -= 1;

			if (Params.TerrainType == TerrainType.BogoTerrain)
			{
				bogoValues.Add((worldSurface, rockLayer));
			}
			else
			{
				surfaceHistory.Record(worldSurface);
				FillColumn(i, worldSurface, rockLayer);
				if (i == GenVars.rightBeachStart - lowDepthBeachSize)
				{
					if (worldSurface > worldSurfaceMax)
						RetargetSurfaceHistory(surfaceHistory, i, worldSurfaceMax);
				}
			}
			if (i == GenVars.rightBeachStart - lowDepthBeachSize)
			{
				terrainFeatureType = TerrainFeatureType.Plateau;
				totalBeachSize = Main.maxTilesX - i;
			}
		}
		if (rockLayer > Main.UnderworldLayer)
			throw new Exception(Language.GetTextValue("Mods.AdvancedWorldGen.Exceptions.RockUnderHell"));

		if (Params.TerrainType == TerrainType.BogoTerrain)
		{
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				int index = WorldGen.genRand.Next(bogoValues.Count);
				(worldSurface, rockLayer) = bogoValues[index];
				bogoValues.RemoveAt(index);
				surfaceHistory.Record(worldSurface);
				FillColumn(i, worldSurface, rockLayer);
				if (i == GenVars.rightBeachStart - lowDepthBeachSize)
				{
					if (worldSurface > worldSurfaceMax)
						RetargetSurfaceHistory(surfaceHistory, i, worldSurfaceMax);
				}
			}
		}

		Main.worldSurface = (int)(worldSurfaceHigh + 25);
		Main.rockLayer = Main.worldSurface + rockLayerHigh - Main.worldSurface;
		int waterLine = (int)(Main.rockLayer + Main.maxTilesY) / 2;
		waterLine += WorldGen.genRand.Next(-100, 20);
		int lavaLine = waterLine + WorldGen.genRand.Next(50, 80);
		if (WorldGen.remixWorldGen)
			lavaLine = (int)(Main.worldSurface * 4.0 + rockLayer) / 5;
		while (lavaLine > Main.UnderworldLayer)
		{
			waterLine -= (waterLine - rockLayer) / 8;
			lavaLine -= (lavaLine - rockLayer) / 8;
		}

		const int num14 = 20;
		if (rockLayerLow < worldSurfaceHigh + num14)
		{
			double num15 = (rockLayerLow + worldSurfaceHigh) / 2;
			double num16 = Math.Abs(rockLayerLow - worldSurfaceHigh);
			if (num16 < num14)
				num16 = num14;

			rockLayerLow = num15 + num16 / 2;
			worldSurfaceHigh = num15 - num16 / 2;
		}

		GenVars.worldSurface = worldSurface;
		GenVars.worldSurfaceHigh = worldSurfaceHigh;
		GenVars.worldSurfaceLow = worldSurfaceLow;
		GenVars.rockLayer = rockLayer;
		GenVars.rockLayerHigh = rockLayerHigh;
		GenVars.rockLayerLow = rockLayerLow;
		GenVars.waterLine = waterLine;
		GenVars.lavaLine = lavaLine;
	}

	private static void DrawTerrainUI(Dictionary<string, object> currentState)
	{
		if (Main.dedServ || !Params.EditTerrainPass)
			return;

		UIState uiState = Main.MenuUI.CurrentState;
		UIState newState = new();
		UIPanel uiPanel = new()
		{
			HAlign = 0.5f,
			VAlign = 0.5f,
			Width = new StyleDimension(0, 0.5f),
			Height = new StyleDimension(0, 0.5f),
			BackgroundColor = UICommon.MainPanelBackground
		};
		newState.Append(uiPanel);

		UIText uiTitle = new("Size options", 0.75f, true) { HAlign = 0.5f };
		uiTitle.Height = uiTitle.MinHeight;
		uiPanel.Append(uiTitle);
		uiPanel.Append(new UIHorizontalSeparator
		{
			Width = new StyleDimension(0f, 1f),
			Top = new StyleDimension(43f, 0f),
			Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
		});

		UIScrollbar uiScrollbar = new()
		{
			Height = new StyleDimension(-110f, 1f),
			Top = new StyleDimension(50, 0f),
			HAlign = 1f
		};
		UIList uiList = new()
		{
			Height = new StyleDimension(-110f, 1f),
			Width = new StyleDimension(-20f, 1f),
			Top = new StyleDimension(50, 0f)
		};
		uiList.SetScrollbar(uiScrollbar);
		uiPanel.Append(uiScrollbar);
		uiPanel.Append(uiList);
		int index = 0;

		const string localizationPath = "Mods.AdvancedWorldGen.UI.TerrainPass";

		foreach ((string? key, object _) in currentState)
		{
			NumberTextBox<int> input =
				new ConfigNumberTextBox<int>(currentState, key, 0, int.MaxValue, localizationPath);
			input.Order = index++;
			uiList.Add(input);
		}

		UITextPanel<string> goBack = new(Language.GetTextValue("UI.Back"))
		{
			Width = new StyleDimension(0f, 0.1f),
			Top = new StyleDimension(0f, 0.75f),
			HAlign = 0.5f
		};
		Thread thread = Thread.CurrentThread;
		goBack.OnLeftClick += (_, _) => thread.Interrupt();
		goBack.OnMouseOver += UiChanger.FadedMouseOver;
		goBack.OnMouseOut += UiChanger.FadedMouseOut;
		newState.Append(goBack);

		Main.MenuUI.SetState(newState);
		try
		{
			Thread.Sleep(Timeout.Infinite);
		}
		catch (ThreadInterruptedException)
		{
		}

		foreach ((string? key, object? value) in currentState)
			AdvancedWorldGenMod.Instance.Logger.Info($"{key} : {value}");

		Main.MenuUI.SetState(uiState);
	}

	private static void FillColumn(int x, double worldSurface, double rockLayer)
	{
		for (int i = 0; i < worldSurface; i++)
		{
			Tile tile = Main.tile[x, i];
			tile.HasTile = false;
			tile.TileFrameX = -1;
			tile.TileFrameY = -1;
		}

		for (int j = (int)worldSurface; j < Main.maxTilesY; j++)
		{
			Tile tile = Main.tile[x, j];
			if (j < rockLayer)
			{
				tile.HasTile = true;
				tile.TileType = 0;
				tile.TileFrameX = -1;
				tile.TileFrameY = -1;
			}
			else
			{
				tile.HasTile = true;
				tile.TileType = 1;
				tile.TileFrameX = -1;
				tile.TileFrameY = -1;
			}
		}
	}

	private static void RetargetColumn(int x, double worldSurface)
	{
		for (int i = 0; i < worldSurface; i++)
		{
			Tile tile = Main.tile[x, i];
			tile.HasTile = false;
			tile.TileFrameX = -1;
			tile.TileFrameY = -1;
		}

		for (int j = (int)worldSurface; j < Main.maxTilesY; j++)
		{
			Tile tile = Main.tile[x, j];
			if (tile.TileType != 1 || !tile.HasTile)
			{
				tile.HasTile = true;
				tile.TileType = 0;
				tile.TileFrameX = -1;
				tile.TileFrameY = -1;
			}
		}
	}

	private static int GenerateWorldSurfaceOffset(TerrainFeatureType featureType)
	{
		switch (Params.TerrainType)
		{
			case TerrainType.Superflat:
				return 0;
			case TerrainType.Flat:
				featureType = TerrainFeatureType.Plateau;
				break;
			case TerrainType.PeaksAndRifts:
			case TerrainType.SkyPillars:
				featureType = featureType switch
				{
					TerrainFeatureType.Plateau => WorldGen.genRand.NextBool(2)
						? TerrainFeatureType.Mountain
						: TerrainFeatureType.Valley,
					TerrainFeatureType.Hill => TerrainFeatureType.Mountain,
					TerrainFeatureType.Dale => TerrainFeatureType.Valley,
					_ => featureType
				};
				break;
		}

		int num = 0;
		if (Params.TerrainType is TerrainType.Mountainous or TerrainType.PeaksAndRifts or TerrainType.SkyPillars)
			switch (featureType)
			{
				case TerrainFeatureType.Plateau:
					while (WorldGen.genRand.NextBool(3)) num += WorldGen.genRand.Next(-1, 2);
					break;
				case TerrainFeatureType.Hill:
					while (WorldGen.genRand.NextBool(2, 3)) num -= 1;
					while (WorldGen.genRand.NextBool(5)) num += 1;
					break;
				case TerrainFeatureType.Dale:
					while (WorldGen.genRand.NextBool(2, 3)) num += 1;
					while (WorldGen.genRand.NextBool(5)) num -= 1;
					break;
				case TerrainFeatureType.Mountain:
					while (WorldGen.genRand.NextBool(5, 6)) num -= 1;
					while (WorldGen.genRand.NextBool(3)) num += 1;
					break;
				case TerrainFeatureType.Valley:
					while (WorldGen.genRand.NextBool(5, 6)) num += 1;
					while (WorldGen.genRand.NextBool(3)) num -= 1;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(featureType), featureType, null);
			}
		else if ((WorldGen.drunkWorldGen || WorldGen.getGoodWorldGen) && WorldGen.genRand.NextBool(2))
			switch (featureType)
			{
				case TerrainFeatureType.Plateau:
					while (WorldGen.genRand.NextBool(6)) num += WorldGen.genRand.Next(-1, 2);
					break;
				case TerrainFeatureType.Hill:
					while (WorldGen.genRand.NextBool(3)) num -= 1;
					while (WorldGen.genRand.NextBool(10)) num += 1;
					break;
				case TerrainFeatureType.Dale:
					while (WorldGen.genRand.NextBool(3)) num += 1;
					while (WorldGen.genRand.NextBool(10)) num -= 1;
					break;
				case TerrainFeatureType.Mountain:
					while (WorldGen.genRand.NextBool(2, 3)) num -= 1;
					while (WorldGen.genRand.NextBool(6)) num += 1;
					break;
				case TerrainFeatureType.Valley:
					while (WorldGen.genRand.NextBool(2, 3)) num += 1;
					while (WorldGen.genRand.NextBool(6)) num -= 1;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(featureType), featureType, null);
			}
		else
			switch (featureType)
			{
				case TerrainFeatureType.Plateau:
					while (WorldGen.genRand.NextBool(7)) num += WorldGen.genRand.Next(-1, 2);
					break;
				case TerrainFeatureType.Hill:
					while (WorldGen.genRand.NextBool(4)) num -= 1;
					while (WorldGen.genRand.NextBool(10)) num += 1;
					break;
				case TerrainFeatureType.Dale:
					while (WorldGen.genRand.NextBool(4)) num += 1;
					while (WorldGen.genRand.NextBool(10)) num -= 1;
					break;
				case TerrainFeatureType.Mountain:
					while (WorldGen.genRand.NextBool(2)) num -= 1;
					while (WorldGen.genRand.NextBool(6)) num += 1;
					break;
				case TerrainFeatureType.Valley:
					while (WorldGen.genRand.NextBool(2)) num += 1;
					while (WorldGen.genRand.NextBool(6)) num -= 1;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(featureType), featureType, null);
			}
		
		return Params.TerrainType == TerrainType.SkyPillars ? num * 10 : num;
	}

	private static void RetargetSurfaceHistory(SurfaceHistory history, int targetX, double targetHeight)
	{
		for (int i = 0; i < history.Length / 2; i++)
		{
			if (history[^1] <= targetHeight)
				break;

			for (int j = 0; j < history.Length - i * 2; j++)
			{
				double num = history[history.Length - j - 1];
				num -= 1;
				history[history.Length - j - 1] = num;
				if (num <= targetHeight)
					break;
			}
		}

		for (int k = 0; k < history.Length; k++)
		{
			double worldSurface = history[history.Length - k - 1];
			RetargetColumn(targetX - k, worldSurface);
		}
	}

	private class SurfaceHistory
	{
		private readonly double[] Heights;
		private int Index;

		public SurfaceHistory(int size)
		{
			Heights = new double[size];
		}

		public double this[int index]
		{
			get => Heights[(index + Index) % Heights.Length];
			set => Heights[(index + Index) % Heights.Length] = value;
		}

		public int Length => Heights.Length;

		public void Record(double height)
		{
			Heights[Index] = height;
			Index = (Index + 1) % Heights.Length;
		}
	}
}