#if SPECIALDEBUG
namespace AdvancedWorldGen.Debug;

public static class ILPrinter
{
	public static void Print(this ILCursor cursor)
	{
		foreach (Instruction cursorInstr in cursor.Instrs)
		{
			if(cursorInstr.Operand is ILLabel label)
				Console.WriteLine($"At {cursorInstr.Offset} {cursorInstr.OpCode} : {label.Target.Offset}");
			else
				Console.WriteLine($"At {cursorInstr.Offset} {cursorInstr.OpCode} : {cursorInstr.Operand}");
		}
	}
}
#endif