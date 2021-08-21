using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;

namespace AdvancedWorldGen.BetterVanillaWorldGen.Interface
{
	public class VanillaAccessor<T>
	{
		public FieldInfo FieldInfo;
		public object VanillaData;
		public VanillaAccessor(IEnumerable<FieldInfo> fieldInfos, string name, object vanillaData)
		{
			FieldInfo = fieldInfos.First(info => info.Name == name);
			VanillaData = vanillaData;
		}
		
		public VanillaAccessor(string name)
		{
			FieldInfo = typeof(WorldGen).GetField(name, BindingFlags.Static | BindingFlags.NonPublic);
			VanillaData = null;
		}

		public void Set(T value)
		{
			FieldInfo.SetValue(VanillaData, value);
		}

		public T Get()
		{
			return (T) FieldInfo.GetValue(VanillaData);
		}
	}
}