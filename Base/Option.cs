namespace AdvancedWorldGen.Base;

public class Option
{
	public List<Option> Children = new();
	public List<string> Conflicts = new();
	public bool? Enabled = false;
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

	public void CheckEnabled()
	{
		if(Children.Count == 0)
			return;
		bool foundEnabled = false;
		bool foundDisabled = false;
		foreach (Option option in Children)
		{
			switch (option.Enabled)
			{
				case true:
					foundEnabled = true;
					break;
				case false:
					foundDisabled = true;
					break;
				case null:
					Enabled = null;
					break;
			}
		}

		if (foundEnabled && foundDisabled)
			Enabled = null;
		else if (foundEnabled)
			Enabled = true;
		else
			Enabled = false;
	}

	public void Disable()
	{
		if (Enabled is false)
			return;
		Enabled = false;
		if (Children.Count > 0)
			foreach (Option child in Children)
				child.Disable();
		Parent?.CheckEnabled();
	}

	public void Enable()
	{
		if(Enabled is true)
			return;
		Enabled = true;
		OnEnable();
		foreach (Option child in Children) child.Enable();
		Parent?.CheckEnabled();
	}

	public virtual void OnEnable()
	{
	}
}