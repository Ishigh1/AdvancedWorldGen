namespace AdvancedWorldGen.Base;

public class AdvancedWorldGenMod : Mod
{
	public UIChanger UIChanger = null!;
	public static AdvancedWorldGenMod Instance => ModContent.GetInstance<AdvancedWorldGenMod>();
	public static string FolderPath => Path.Combine(Main.SavePath, "AdvancedWorldGen");

#if SPECIALDEBUG
	private delegate void PublishMod(object o1, object o2);

	private static void Crash(PublishMod orig, object o1, object o2)
	{
		throw new Exception();
	}
#endif

	public override void Load()
	{
#if SPECIALDEBUG
		MethodInfo method =
			typeof(WorkshopHelper).GetMethod("PublishMod", BindingFlags.NonPublic | BindingFlags.Static)!;
		new Hook(method, Crash); // Prevents me from sending the dev version again
#endif

		//Remove ThreadInterruptedException from logging (interrupting thread.sleep)
		Logging.IgnoreExceptionContents("System.Threading.ThreadInterruptedException");

		if (!Directory.Exists(FolderPath))
			Directory.CreateDirectory(FolderPath);
		Legacy.MoveFiles();

		TileReplacer.Initialize();

		UIChanger = new UIChanger(this);

		On_UIWorldCreation.AddDescriptionPanel += UIChanger.TweakWorldGenUi;
		On_UIWorldCreation.FinishCreatingWorld += ModifiedWorld.Instance.LastMinuteChecks;
		On_WorldFileData.SetWorldSize += UIChanger.SetSpecialName;
		// On_UIWorldListItem.ctor += UiChanger.CopySettingsButton; // Removed until twld can be loaded in a reasonable time
#if SPECIALDEBUG
		On_UIWorldSelect.OnInitialize += UIChanger.DeleteAllButLast;
#endif

		IL_WorldGen.GenerateWorld += ModifiedWorld.OverrideWorldOptions;
		On_WorldFile.CreateMetadata += DedServUi.DedServOptions;

		On_UIWorldLoad.ctor += UIChanger.AddCancel;
		On_WorldGen.do_worldGenCallBack += UIChanger.ThreadifyWorldGen;

		On_UserInterface.SetState += ModifiedWorld.ResetSettings;

		On_WorldGen.NotTheBees += ClassicOptions.SmallNotTheBees;

		On_Main.UpdateTime_StartDay += ModifiedWorld.OnDawn;
		On_Main.UpdateTime_StartNight += ModifiedWorld.OnDusk;
		On_Main.checkXMas += SnowWorld.MainOnCheckXMas;
		IL_Projectile.Kill += SnowWorld.RemoveSnowDropDuringChristmas;

		IL_WorldGen.MakeDungeon += DrunkOptions.CrimruptionChest;

		IL_WaterfallManager.FindWaterfalls += WaterfallFix.FixWaterfalls;

		HalloweenCommon.Setup();
	}

	public override void HandlePacket(BinaryReader reader, int whoAmI)
	{
		OptionHelper.HandlePacket(reader);
	}

	public override object? Call(params object[] args)
	{
		int argCount = args.Length;
		if (argCount >= 1 && args[0] is string command)
			switch (command)
			{
				// Syntax : Call("Options Contains", option);
				// Tests if the option is currently enabled
				case "Options Contains" when argCount is 2:
				{
					if (args[1] is string option)
						return OptionHelper.OptionsContains(option);
				}
					break;

				// Syntax : Call("Register Option", modName, optionName, (hidden), (parentNames));
				// Registers a new option
				// To be well drawn, an entry Mods.{mod.Name}(.{parent}).{internalName} defining the displayed option name and an entry
				// Mods.{mod.Name}.{internalName}.Description giving a short description must be defined on a localization file
				case "Register Option" when argCount is 3 or 4 or 5:
				{
					if (args[1] is string modName && args[2] is string optionName)
					{
						bool hidden = argCount is 4 or 5 && (args[3] is true || args[4] is true);
						Option? parent = null;
						if (argCount is 4 or 5)
						{
							string? parentNames = null;
							if (args[3] is string)
								parentNames = args[3] as string;
							else if (args[4] is string)
								parentNames = args[4] as string;
							if (parentNames != null)
								parent = parentNames.Split('.').Aggregate(parent,
									(current, parentName) =>
										current is null
											? OptionHelper.OptionDict[parentName]
											: current.Children.Find(option => option.Name == parentName));
						}

						if (parent is null)
							OptionHelper.OptionDict.Add(optionName, new Option
							{
								ModName = modName,
								Name = optionName,
								Hidden = hidden
							});
						else
							parent.Children.Add(new Option
							{
								ModName = modName,
								Name = optionName,
								Hidden = hidden,
								Parent = parent
							});
					}
				}
					break;

				// Syntax : Call("Remove Option", optionName);
				case "Remove Option" when argCount is 2:
				{
					if (args[1] is string optionName)
						OptionHelper.OptionDict.Remove(optionName);
				}
					break;

				// Syntax : Call("Set Option Visibility", optionName, visibility);
				case "Set Option Visibility" when argCount is 3:
				{
					if (args[1] is string optionName && args[2] is bool visibility)
						if (OptionHelper.OptionDict.TryGetValue(optionName, out Option? option))
							option.Hidden = visibility;
				}
					break;

				// Syntax : Call("Enable Option", optionName, (enableChildren));
				case "Enable Option" when argCount is 2 or 3:
				{
					if (args[1] is string optionName)
						if (OptionHelper.OptionDict.TryGetValue(optionName, out Option? option))
						{
							if (argCount == 3 && args[2] is true)
								option.Enable();
							else
								option.OnEnable();
						}
				}
					break;

				// Syntax : Call("Disable Option", optionName);
				case "Disable Option" when argCount is 2:
				{
					if (args[1] is string optionName)
						if (OptionHelper.OptionDict.TryGetValue(optionName, out Option? option))
							option.Disable();
				}
					break;
				
				// Syntax : Call("Set Default Size", x, y);
				case "Set Default Size" when argCount is 3:
				{
					if (args[1] is int baseX && args[2] is int baseY)
					{
						WorldSettings.BaseX = baseX;
						WorldSettings.BaseY = baseY;
					}
				}
					break;
			}

		return null;
	}
}