using AdvancedWorldGen.Base;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace AdvancedWorldGen.Helper
{
	public class ILHelper
	{
		public static void OptionContains(ILCursor cursor, string option)
		{
			cursor.Emit(OpCodes.Ldc_I4_1);
			cursor.Emit(OpCodes.Newarr, typeof(string));
			cursor.Emit(OpCodes.Dup);
			cursor.Emit(OpCodes.Ldc_I4_0);
			cursor.Emit(OpCodes.Ldstr, option);
			cursor.Emit(OpCodes.Stelem_Ref);
			cursor.Emit(OpCodes.Call, typeof(ModifiedWorld).GetMethod("OptionsContains"));
		}
		
		public static void OptionContains(ILCursor cursor, string option1, string option2)
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
			cursor.Emit(OpCodes.Call, typeof(ModifiedWorld).GetMethod("OptionsContains"));
		}
	}
}