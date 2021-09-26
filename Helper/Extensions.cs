using System.Diagnostics.CodeAnalysis;
using AdvancedWorldGen.Base;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.Helper
{
	public static class Extensions
	{
		public static bool TryGetValue<T>(this TagCompound tagCompound, string name, [NotNullWhen(true)] out T? value)
		{
			if (tagCompound.ContainsKey(name))
			{
				value = tagCompound.Get<T>(name)!;
				return true;
			}

			value = default;
			return false;
		}

		#region ILHelper

		public static void OptionContains(this ILCursor cursor, string option)
		{
			cursor.Emit(OpCodes.Ldc_I4_1);
			cursor.Emit(OpCodes.Newarr, typeof(string));
			cursor.Emit(OpCodes.Dup);
			cursor.Emit(OpCodes.Ldc_I4_0);
			cursor.Emit(OpCodes.Ldstr, option);
			cursor.Emit(OpCodes.Stelem_Ref);
			cursor.Emit(OpCodes.Call, typeof(ModifiedWorld).GetMethod(nameof(ModifiedWorld.OptionsContains)));
		}

		public static void OptionContains(this ILCursor cursor, string option1, string option2)
		{
			cursor.Emit(OpCodes.Ldc_I4_2);
			cursor.Emit(OpCodes.Newarr, typeof(string));
			cursor.Emit(OpCodes.Dup);
			cursor.Emit(OpCodes.Ldc_I4_0);
			cursor.Emit(OpCodes.Ldstr, option1);
			cursor.Emit(OpCodes.Stelem_Ref);
			cursor.Emit(OpCodes.Dup);
			cursor.Emit(OpCodes.Ldc_I4_1);
			cursor.Emit(OpCodes.Ldstr, option2);
			cursor.Emit(OpCodes.Stelem_Ref);
			cursor.Emit(OpCodes.Call, typeof(ModifiedWorld).GetMethod(nameof(ModifiedWorld.OptionsContains)));
		}

		#endregion

		#region GenerationProgress

		public static void Set(this GenerationProgress generationProgress, int currentValue, float maxValue,
			float weight = 1f, float pastWeight = 0f)
		{
			generationProgress.Set(currentValue / maxValue * weight + pastWeight);
		}

		#endregion
	}
}