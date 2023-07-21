namespace AdvancedWorldGen.Base;

public class ModifiedWorld : ModSystem
{
	public Dictionary<string, TimeSpan>? Times;


	public List<Dictionary<string, double>> Weights = null!;
	public static ModifiedWorld Instance => ModContent.GetInstance<ModifiedWorld>();

	public static string DataPath => Path.Combine(AdvancedWorldGenMod.FolderPath, "PassesData.json");

	public override void OnModLoad()
	{
		OptionHelper.InitializeDict(Mod);
		LoadWeights();
	}

	private void LoadWeights()
	{
		if (File.Exists(DataPath))
		{
			using StreamReader r = new(DataPath);
			string json = r.ReadToEnd();
			Weights = JsonConvert.DeserializeObject<List<Dictionary<string, double>>>(json);
		}
		else
		{
			Weights = new List<Dictionary<string, double>>();
		}
	}

	private void SaveWeights()
	{
		using StreamWriter writer = new(DataPath);
		writer.Write(JsonConvert.SerializeObject(Weights));
	}

	public override void OnWorldLoad()
	{
		Replacer.IngameReplace();
		OptionHelper.ClearAll();
	}

	public override void OnWorldUnload()
	{
		Replacer.IngameUnreplace();
	}

	public override void LoadWorldData(TagCompound tag)
	{
		if (tag.TryGet("Options", out List<string> options))
		{
			OptionHelper.Import(options);
			Main.checkHalloween();
			Main.checkXMas();
		}
	}

	public override void SaveWorldData(TagCompound tagCompound)
	{
		List<string> options = OptionHelper.Export();
		if (options.Count > 0)
			tagCompound.Add("Options", options);
	}

	private static void ResetFlags()
	{
		Main.notTheBeesWorld = false;
		WorldGen.notTheBees = false;

		Main.getGoodWorld = false;
		WorldGen.getGoodWorldGen = false;

		Main.drunkWorld = false;
		WorldGen.drunkWorldGen = false;
		WorldGen.drunkWorldGenText = false;

		Main.tenthAnniversaryWorld = false;
		WorldGen.tenthAnniversaryWorldGen = false;

		Main.dontStarveWorld = false;
		WorldGen.dontStarveWorldGen = false;

		Main.noTrapsWorld = false;
		WorldGen.noTrapsWorldGen = false;

		Main.remixWorld = false;
		WorldGen.remixWorldGen = false;

		Main.zenithWorld = false;
		WorldGen.everythingWorldGen = false;
	}

	public override void PreWorldGen()
	{
		Replacer.WorldgenReplace();
		Replacer.IngameReplace();
		bool notTheBees = OptionHelper.OptionsContains("NotTheBees");
		Main.notTheBeesWorld |= notTheBees;
		WorldGen.notTheBees |= notTheBees;

		bool forTheWorthy = OptionHelper.OptionsContains("ForTheWorthy");
		Main.getGoodWorld |= forTheWorthy;
		WorldGen.getGoodWorldGen |= forTheWorthy;

		bool drunk = OptionHelper.OptionsContains("Drunk");
		Main.drunkWorld |= drunk;
		WorldGen.drunkWorldGen |= drunk;
		WorldGen.drunkWorldGenText |= drunk;

		bool celebration = OptionHelper.OptionsContains("Celebrationmk10");
		Main.tenthAnniversaryWorld |= celebration;
		WorldGen.tenthAnniversaryWorldGen |= celebration;

		bool dontStarve = OptionHelper.OptionsContains("TheConstant");
		Main.dontStarveWorld |= dontStarve;
		WorldGen.dontStarveWorldGen |= dontStarve;

		bool noTraps = OptionHelper.OptionsContains("NoTraps");
		Main.noTrapsWorld |= noTraps;
		WorldGen.noTrapsWorldGen |= noTraps;

		bool remix = OptionHelper.OptionsContains("Remix");
		Main.remixWorld |= remix;
		WorldGen.remixWorldGen |= remix;

		bool zenith = OptionHelper.OptionsContains("Zenith");
		Main.zenithWorld |= zenith;
		WorldGen.everythingWorldGen |= zenith;

		if (!Main.dayTime) Main.time = 0;
	}

	public override void PostWorldGen()
	{
		Replacer.IngameUnreplace();
		Replacer.WorldgenUnreplace();
		if (Times != null)
		{
			double totalTime = 0;
			foreach ((string? _, TimeSpan value) in Times) totalTime += value.Milliseconds;
			totalTime /= 10_000f;

			if (Weights.Count >= 20) Weights.RemoveAt(0);

			Dictionary<string, double> weights = new();
#if SPECIALDEBUG
			using TextWriter textWriter = new StreamWriter(@"d:\debug.txt", true);
#endif
			foreach ((string? key, TimeSpan value) in Times)
			{
#if SPECIALDEBUG
				textWriter.WriteLine(key + " : " + value);
#endif
				weights.Add(key, value.TotalMilliseconds / totalTime);
			}

			Weights.Add(weights);
			SaveWeights();

			if (Replacer.TimerHooks != null)
			{
				foreach (Hook timerHook in Replacer.TimerHooks) timerHook.Dispose();

				Replacer.TimerHooks = null;
			}
		}
	}

	public override void NetReceive(BinaryReader reader)
	{
		List<string> options = new();
		string optionName;
		while ((optionName = reader.ReadString()) != "") options.Add(optionName);

		OptionHelper.Import(options);

		Main.checkHalloween();
		Main.checkXMas();
	}

	public override void NetSend(BinaryWriter writer)
	{
		List<string> optionNames = OptionHelper.Export();
		foreach (string optionName in optionNames)
			writer.Write(optionName);

		writer.Write("");
	}

	//Deletes all the now-useless stuff about special seeds
	public static void OverrideWorldOptions(ILContext il)
	{
		ILCursor cursor = new(il);
		cursor.GotoNext(MoveType.After, instruction => instruction.MatchStloc(0));
		while (!cursor.Next!.MatchLdstr(
			       "Creating world - Seed: {0}, Width: {1}, Height: {2}, Evil: {3}, IsExpert: {4}"))
			cursor.Remove();

		cursor.GotoNext(MoveType.Before,
			instruction => instruction.MatchLdsfld(typeof(WorldGen).GetField(nameof(WorldGen.everythingWorldGen),
				BindingFlags.Public | BindingFlags.Static)!));
		while (!(cursor.Next!.OpCode == OpCodes.Brfalse_S)) cursor.Remove();

		cursor.OptionContains("Zenith.StarGame");
	}

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
	{
		Replacer.ReplaceGenPasses(tasks);
		DrunkOptions.AddDrunkEdits(tasks);

		tasks.TryReplacePass("Guide", new NPCPass());

		HalloweenCommon.InsertTasks(tasks);

		if (OptionHelper.OptionsContains("Santa"))
			tasks.Add(new SnowReplacer());

		if (OptionHelper.OptionsContains("Evil"))
			tasks.Add(new EvilReplacer());

		if (OptionHelper.OptionsContains("Random") || OptionHelper.OptionsContains("Random.Painted"))
			tasks.Add(new RandomReplacer());

		if (OptionHelper.OptionsContains("Santa") ||
		    OptionHelper.OptionsContains("Evil") ||
		    OptionHelper.OptionsContains("Random") ||
		    OptionHelper.OptionsContains("Random.Painted"))
		{
			int passIndex = tasks.FindIndex(pass => pass.Name == "Settle Liquids Again");
			if (passIndex != -1)
				tasks.Add(tasks[passIndex]);
		}

		if (OptionHelper.OptionsContains("SortedWorld"))
		{
			int passIndex = tasks.FindIndex(pass => pass.Name == "Final Cleanup");
			if (passIndex != -1)
				tasks.Add(new SortedWorld());
		}
	}

	public override void PostUpdateTime()
	{
		if (Main.netMode != NetmodeID.MultiplayerClient) OptionHelper.OnTick();
	}

	public static void OnDawn(On_Main.orig_UpdateTime_StartDay orig, ref bool stopEvents)
	{
		orig(ref stopEvents);
		OptionHelper.OnDawn();
	}

	public static void OnDusk(On_Main.orig_UpdateTime_StartNight orig, ref bool stopEvents)
	{
		orig(ref stopEvents);
		OptionHelper.OnDusk();
	}

	public static void ResetSettings(On_UserInterface.orig_SetState orig, UserInterface self, UIState state)
	{
		orig(self, state);
		if (state is UIWorldSelect)
		{
			ResetFlags();
			OptionHelper.ClearAll();
			Params.Wipe();
		}
	}

	public void LastMinuteChecks(On_UIWorldCreation.orig_FinishCreatingWorld orig, UIWorldCreation self)
	{
		void OrigWithLog()
		{
			Mod.Logger.Info($"Overhauled : {WorldgenSettings.Instance.FasterWorldgen}");
			Mod.Logger.Info("Options : " + OptionsParser.GetJsonText());
			orig(self);
		}

		OrigWithLog();
	}
}