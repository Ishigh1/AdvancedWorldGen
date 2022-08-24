namespace AdvancedWorldGen.Helper.Accessors;

public class PropertyAccessor<T> : ReflectionAccessor<T>
{
	public PropertyInfo PropertyInfo;

	public PropertyAccessor(IEnumerable<PropertyInfo> propertyInfos, string name, object? data)
	{
		PropertyInfo = propertyInfos.First(info => info.Name == name);
		Data = data;
	}

	public PropertyAccessor(IReflect type, string name, object? data)
	{
		PropertyInfo = type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;
		Data = data;
	}

	public PropertyAccessor(IReflect type, string name)
	{
		PropertyInfo = type.GetProperty(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)!;
		Data = null;
	}

	protected override void Set(T value)
	{
		PropertyInfo.SetValue(Data, value);
	}

	protected override T Get()
	{
		return (T)PropertyInfo.GetValue(Data)!;
	}
}