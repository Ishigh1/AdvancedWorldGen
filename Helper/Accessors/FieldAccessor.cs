namespace AdvancedWorldGen.Helper.Accessors;

public class FieldAccessor<T> : ReflectionAccessor<T>
{
	public FieldInfo FieldInfo;
	public object? VanillaData;

	public FieldAccessor(IEnumerable<FieldInfo> fieldInfos, string name, object? vanillaData)
	{
		FieldInfo = fieldInfos.First(info => info.Name == name);
		VanillaData = vanillaData;
	}

	public FieldAccessor(IReflect type, string name, object? vanillaData)
	{
		FieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
		VanillaData = vanillaData;
	}

	public FieldAccessor(IReflect type, string name)
	{
		FieldInfo = type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!;
		VanillaData = null;
	}

	protected override void Set(T value)
	{
		FieldInfo.SetValue(VanillaData, value);
	}

	protected override T Get()
	{
		return (T)FieldInfo.GetValue(VanillaData)!;
	}
}