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
	}
}