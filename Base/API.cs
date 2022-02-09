using System.Collections.Generic;
using Terraria.ModLoader;

namespace AdvancedWorldGen.Base;

public class API
{
	/// <summary>
	///     Registers a new option
	///     To be well drawn, an entry Mods.{mod.Name}.{internalName} defining the displayed option name and an entry
	///     Mods.{mod.Name}.{internalName}.Description giving a short description must be defined on a localization file
	/// </summary>
	/// <param name="mod">The mod featuring the given option</param>
	/// <param name="internalName">The internal name of the option, it must be unique and with no space</param>
	/// <param name="hidden">Set to true if you don't want this option to be shown by default</param>
	public static void RegisterOption(Mod mod, string internalName, bool hidden = false)
	{
		ModifiedWorld.Instance.OptionHelper.OptionDict.Add(internalName, new Option
		{
			Children = new List<Option>(),
			Conflicts = new List<string>(),
			Hidden = hidden,
			ModName = mod.Name,
			Name = internalName
		});
	}

	/// <summary>
	///     Checks if at least one of the given options is enabled
	/// </summary>
	public static bool OptionsContains(params string[] options)
	{
		return ModifiedWorld.Instance.OptionHelper.OptionsContains(options);
	}
}