using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Terraria.ModLoader;

namespace AdvancedWorldGen.Base;

public class Option
{
	public static Dictionary<string, Option> OptionDict = null!;
	public List<Option> Children = null!;
	public List<string> Conflicts = null!;
	public bool Enabled;
	public bool Hidden;

	public string ModName = nameof(AdvancedWorldGen);
	public string Name = null!;
	public Option? Parent;

	public string FullName
	{
		get
		{
			if (Parent == null)
				return Name;
			else
				return Parent.FullName + "." + Name;
		}
	}

	public string SimplifiedName => Name == "Base" ? Parent!.FullName : FullName;

	public void Disable()
	{
		if (!Enabled)
			return;
		Enabled = false;
		if (Children.Count == 0)
			ModifiedWorld.Instance.OptionHelper.Options.Remove(FullName);
		else
			foreach (Option child in Children)
				child.Disable();
		if (Parent?.Children.TrueForAll(child => !child.Enabled) == true)
			Parent.Disable();
	}

	public void Enable()
	{
		WeakEnable();
		foreach (Option child in Children) child.Enable();
	}

	public void WeakEnable()
	{
		if (Enabled)
			return;
		Enabled = true;
		if (Children.Count == 0)
			ModifiedWorld.Instance.OptionHelper.Options.Add(FullName);
		Parent?.WeakEnable();
	}

	public static void InitializeDict(Mod mod)
	{
		OptionDict = JsonConvert.DeserializeObject<Dictionary<string, Option>>(
			Encoding.UTF8.GetString(mod.GetFileBytes("Options.json")));

		for (int index = 0; index < OptionDict.Count; index++)
		{
			(_, Option? option) = OptionDict.ElementAt(index);
			if (option.Children.Count == 0)
				continue;
			Option baseOption = new()
			{
				Children = new List<Option>(),
				Conflicts = option.Conflicts,
				Name = "Base"
			};
			option.Conflicts = new List<string>();
			option.Children.Insert(0, baseOption);
			foreach (Option optionChild in option.Children)
			{
				optionChild.Parent = option;
				OptionDict.Add(optionChild.FullName, optionChild);
			}
		}

		foreach ((_, Option? option) in OptionDict)
			for (int index = 0; index < option.Conflicts.Count; index++)
			{
				string optionConflict = option.Conflicts[index];
				Option conflict = OptionDict[optionConflict];
				if (conflict.Children.Count != 0)
					option.Conflicts[index] = conflict.Children[0].Name;
			}
	}
}