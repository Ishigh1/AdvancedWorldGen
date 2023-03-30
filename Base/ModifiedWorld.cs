namespace AdvancedWorldGen.Base;

public class ModifiedWorld : ModSystem
{
	public Dictionary<string, TimeSpan>? Times;


	public List<Dictionary<string, float>> Weights = null!;
	public static ModifiedWorld Instance => ModContent.GetInstance<ModifiedWorld>();

	private static string DataPath => Path.Combine(AdvancedWorldGenMod.FolderPath, "PassesData.json");

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
			Weights = JsonConvert.DeserializeObject<List<Dictionary<string, float>>>(json);
		}
		else
		{
			Weights = new List<Dictionary<string, float>>();
		}
	}

	public void SaveWeights()
	{
		using StreamWriter writer = new(DataPath);
		writer.Write(JsonConvert.SerializeObject(Weights));
	}

	public override void OnWorldLoad()
	{
		OptionHelper.ClearAll();
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
	}

	public override void PreWorldGen()
	{
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

		if (!Main.dayTime) Main.time = 0;
	}

	public override void PostWorldGen()
	{
		if (Times != null)
		{
			float totalTime = 0;
			foreach ((string? _, TimeSpan value) in Times) totalTime += value.Milliseconds;
			totalTime /= 10_000f;

			if (Weights.Count >= 20) Weights.RemoveAt(0);

			Dictionary<string, float> weights = new();
			foreach ((string? key, TimeSpan value) in Times)
				weights.Add(key, value.Milliseconds / totalTime);

			Weights.Add(weights);
			SaveWeights();

			foreach (MethodInfo methodInfo in Replacer.MethodInfos) HookEndpointManager.Remove(methodInfo, Replacer.Timer);
			Replacer.MethodInfos.Clear();

			Times = null;
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
		cursor.GotoNext(instruction => instruction.MatchLdcI4(0));
		while (!cursor.Next.MatchLdstr(
			       "Creating world - Seed: {0} Width: {1}, Height: {2}, Evil: {3}, IsExpert: {4}"))
			cursor.Remove();
	}

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
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

	public static void OnDawn(OnMain.orig_UpdateTime_StartDay orig, ref bool stopEvents)
	{
		orig(ref stopEvents);
		OptionHelper.OnDawn();
	}

	public static void OnDusk(OnMain.orig_UpdateTime_StartNight orig, ref bool stopEvents)
	{
		orig(ref stopEvents);
		OptionHelper.OnDusk();
	}

	public void ResetSettings(OnUserInterface.orig_SetState orig, UserInterface self, UIState state)
	{
		orig(self, state);
		if (state is UIWorldSelect)
		{
			ResetFlags();
			OptionHelper.ClearAll();
			Params.Wipe();
		}
	}

	public void LastMinuteChecks(OnUIWorldCreation.orig_FinishCreatingWorld orig, UIWorldCreation self)
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