namespace AdvancedWorldGen.BetterVanillaWorldGen;

public static partial class Replacer
{
	private static List<GenPass>? GenPasses;
	public static HashSet<MethodInfo> MethodInfos = new();

	public static void Replace()
	{
		On_DesertHive.Place += ReplaceDesertHive;
		On_WorldGen.AddBuriedChest_int_int_int_bool_int_bool_ushort += ReplaceChest;
		On_DesertDescription.CreateFromPlacement += ReplaceDesertDescriptionCreation;

#if !SPECIALDEBUG
		On_WorldGenerator.GenerateWorld += ChangeWeights;
#endif
	}

	private static void ReplaceDesertHive(On_DesertHive.orig_Place orig,
		DesertDescription description)
	{
		if (WorldgenSettings.Revamped)
			ModifiedDesertHive.Place(description);
		else
			orig(description);
	}

	private static bool ReplaceChest(On_WorldGen.orig_AddBuriedChest_int_int_int_bool_int_bool_ushort orig, int i,
		int j, int contain, bool notNearOtherChests, int style, bool trySlope, ushort chestTileType)
	{
		return WorldgenSettings.Revamped
			? GenerationChests.AddBuriedChest(i, j, contain, notNearOtherChests, style, chestTileType)
			: orig(i, j, contain, notNearOtherChests, style, trySlope, chestTileType);
	}

	private static DesertDescription ReplaceDesertDescriptionCreation(
		On_DesertDescription.orig_CreateFromPlacement orig, Point origin)
	{
		return WorldgenSettings.Revamped ? Desert.CreateFromPlacement(origin) : orig(origin);
	}

#if !SPECIALDEBUG
	private static void ChangeWeights(On_WorldGenerator.orig_GenerateWorld orig, WorldGenerator self,
		GenerationProgress progress)
	{
		if (GenPasses != null)
		{
			Dictionary<string, (double weight, int found)> weights = new();
			double originalWeights = 0;
			foreach (GenPass pass in GenPasses)
			{
				weights.TryAdd(pass.Name, (0, 0));
				originalWeights += pass.Weight;
			}

			foreach (Dictionary<string, float> instanceWeight in ModifiedWorld.Instance.Weights)
			{
				double totalWeight = 0;
				double passWeights = originalWeights;
				foreach (GenPass genPass in GenPasses)
					if (instanceWeight.TryGetValue(genPass.Name, out float weight))
						totalWeight += weight;
					else
						passWeights -= genPass.Weight;

				totalWeight /= passWeights;

				foreach (GenPass genPass in GenPasses)
					if (instanceWeight.TryGetValue(genPass.Name, out float weight))
					{
						(double currentWeight, int found) = weights[genPass.Name];
						weights[genPass.Name] = (currentWeight + weight / totalWeight, found + 1);
					}
			}

			foreach (GenPass genPass in GenPasses)
			{
				(double weight, int found) = weights[genPass.Name];
				if (weight != 0) genPass.Weight = weight / found;
				Type type = genPass.GetType();
				MethodInfo? methodInfo =
					type.GetMethod("ApplyPass", BindingFlags.Instance | BindingFlags.NonPublic,
						new[] { typeof(GenerationProgress), typeof(GameConfiguration) });
				if (methodInfo != null)
				{
					if (!MethodInfos.Contains(methodInfo) &&
					    MethodInfos.All(info => info.DeclaringType != methodInfo.DeclaringType))
					{
						HookEndpointManager.Add(methodInfo, Timer);
						MethodInfos.Add(methodInfo);
					}
				}
				else
				{
					AdvancedWorldGenMod.Instance.Logger.Debug("Methodinfo not found for genpass " + genPass.Name +
					                                          "(Mod : " + type.Assembly.FullName + ")");
				}
			}

			ModifiedWorld.Instance.Times = new Dictionary<string, TimeSpan>();
		}

		orig(self, progress);
	}

	public static void Timer(On_GenPass.orig_Apply orig, GenPass self, GenerationProgress progress,
		GameConfiguration configuration)
	{
		if (ModifiedWorld.Instance.Times != null)
		{
			Stopwatch stopwatch = new();
			if (self is ControlledWorldGenPass controlledWorldGenPass)
				controlledWorldGenPass.Stopwatch = stopwatch;
			stopwatch.Start();
			orig(self, progress, configuration);
			stopwatch.Stop();
			ModifiedWorld.Instance.Times.TryAdd(self.Name, stopwatch.Elapsed);
		}
		else
		{
			orig(self, progress, configuration);
		}
	}
#endif
}