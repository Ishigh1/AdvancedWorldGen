using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdvancedWorldGen.Helper
{
	public class VanillaAccessor<T>
	{
		public FieldInfo FieldInfo;
		public object? VanillaData;

		public VanillaAccessor(IEnumerable<FieldInfo> fieldInfos, string name, object vanillaData)
		{
			FieldInfo = fieldInfos.First(info => info.Name == name);
			VanillaData = vanillaData;
		}

		public VanillaAccessor(IReflect type, string name, object vanillaData)
		{
			FieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
			VanillaData = vanillaData;
		}

		public VanillaAccessor(IReflect type, string name)
		{
			FieldInfo = type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!;
			VanillaData = null;
		}

		public void Set(T value)
		{
			FieldInfo.SetValue(VanillaData, value);
		}

		public T Get()
		{
			return (T) FieldInfo.GetValue(VanillaData)!;
		}
	}
}