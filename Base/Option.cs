namespace AdvancedWorldGen.Base;

public class Option
{
	public List<Option> Children = new();
	public List<string> Conflicts = new();
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
			return Parent.FullName + "." + Name;
		}
	}

	public string SimplifiedName => Name == "Base" ? Parent!.FullName : FullName;

	public void Disable()
	{
		if (!Enabled)
			return;
		Enabled = false;
		if (Children.Count > 0)
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
		Parent?.WeakEnable();
	}
}