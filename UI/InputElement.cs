using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader.Config.UI;

namespace AdvancedWorldGen.UI
{
	public static class InputElement
	{
		private static readonly FieldInfo ConfigElementTextDisplayFunction =
			typeof(ConfigElement).GetField("TextDisplayFunction", BindingFlags.NonPublic | BindingFlags.Instance)!;
		
		#region int
		
		private static readonly Type IntInputElementType =
			typeof(Main).Assembly.GetType("Terraria.ModLoader.Config.UI.IntInputElement")!;

		private static readonly ConstructorInfo IntInputElementConstructorInfo =
			IntInputElementType.GetConstructor(Array.Empty<Type>())!;

		private static readonly FieldInfo IntInputElementMinField =
			IntInputElementType.GetField("min", BindingFlags.Public | BindingFlags.Instance)!;

		private static readonly FieldInfo IntInputElementMaxField =
			IntInputElementType.GetField("max", BindingFlags.Public | BindingFlags.Instance)!;

		private static readonly FieldInfo IntInputElementIncrementField =
			IntInputElementType.GetField("increment", BindingFlags.Public | BindingFlags.Instance)!;
		public static ConfigElement MakeIntInputLine(object data, string fieldName, int min, int max, int increment)
		{
			ConfigElement intInputElement = (ConfigElement) IntInputElementConstructorInfo.Invoke(null);

			IntInputElementMinField.SetValue(intInputElement, min);
			IntInputElementMaxField.SetValue(intInputElement, max);
			IntInputElementIncrementField.SetValue(intInputElement, increment);

			FieldInfo fieldInfo = data.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public)!;
			intInputElement.Bind(new PropertyFieldWrapper(fieldInfo), data, null, -1);
			intInputElement.OnBind();
			intInputElement.Recalculate();
			return intInputElement;
		}
		
		#endregion

		#region string
		
		private static readonly Type StringInputElementType =
			typeof(Main).Assembly.GetType("Terraria.ModLoader.Config.UI.StringInputElement")!;

		private static readonly ConstructorInfo StringInputElementConstructorInfo =
			StringInputElementType.GetConstructor(Array.Empty<Type>())!;
		public static ConfigElement MakeStringInputLine(object data, string fieldName, string label = "")
		{
			ConfigElement stringInputLine = (ConfigElement) StringInputElementConstructorInfo.Invoke(null);
			FieldInfo fieldInfo = data.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public)!;
			stringInputLine.Bind(new PropertyFieldWrapper(fieldInfo), data, null, -1);
			stringInputLine.OnBind();
			stringInputLine.Recalculate();
			if (label != "")
			{
				string DisplayReplacer() => label;
				ConfigElementTextDisplayFunction.SetValue(stringInputLine, (Func<string>) DisplayReplacer);
			}
			return stringInputLine;
		}

		#endregion
	}
}