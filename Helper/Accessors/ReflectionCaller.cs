namespace AdvancedWorldGen.Helper.Accessors;

public class ReflectionCaller
{
	public MethodInfo MethodInfo;
	public object? VanillaData;

	public ReflectionCaller(IReflect type, string name, object? vanillaData)
	{
		MethodInfo = type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
		VanillaData = vanillaData;
	}

	public ReflectionCaller(IReflect type, string name)
	{
		MethodInfo = type.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!;
		VanillaData = null;
	}

	public object? Call(params object[] args)
	{
		return MethodInfo.Invoke(VanillaData, args)!;
	}
}