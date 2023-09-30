using Mono.Cecil;
using MonoMod.Utils;

namespace AdvancedWorldGen.BetterVanillaWorldGen;

public static partial class Replacer
{
	private static List<ILHook>? Hooks;

	private static void ILReplace(ILContext il)
	{
		ILCursor cursor = new(il);

		Hooks = new List<ILHook>();

		if (OptionHelper.OptionsContains("Celebrationmk10.Painted"))
		{
			cursor.GotoNext(MoveType.After, instruction => instruction.MatchLdstr("Final Cleanup"));
			WorldGenLegacyMethod finalCleanup =
				(WorldGenLegacyMethod)((FieldReference)cursor.Next.Operand).ResolveReflection().GetValue(null);
			Hooks.Add(new ILHook(finalCleanup.Method, CelebrationFinisher));
		}

		Hooks.Add(new ILHook(
			typeof(WorldGen).GetMethod("FinishTenthAnniversaryWorld", BindingFlags.NonPublic | BindingFlags.Static),
			CelebrationPaint));
	}

	private static void ILUnreplace()
	{
		foreach (ILHook ilHook in Hooks)
		{
			ilHook.Dispose();
		}

		IL_WorldGen.AddGenPasses -= ILReplace;
		Hooks = null;
	}

	private static void CelebrationFinisher(ILContext il)
	{
		if (WorldGen.tenthAnniversaryWorldGen)
			return;
		ILCursor cursor = new(il);
		cursor.GotoNext(instruction => instruction.MatchCall(typeof(WorldGen), "FinishTenthAnniversaryWorld"));
		List<ILLabel> ilLabels = new();
		while (!cursor.Prev.MatchCall(typeof(WorldGen), "FinishNoTraps"))
		{
			cursor.GotoPrev();
			ilLabels.AddRange(cursor.IncomingLabels);
			cursor.Remove();
		}

		foreach (ILLabel ilLabel in ilLabels)
		{
			cursor.MarkLabel(ilLabel);
		}
	}

	private static void CelebrationPaint(ILContext il)
	{
		ILCursor cursor = new(il);
		bool finishedPainting = false;
		bool celebration = OptionHelper.OptionsContains("Celebrationmk10");
		bool painting = OptionHelper.OptionsContains("Celebrationmk10.Painted");
		if (!(painting ^ celebration))
			return;

		if (celebration)
		{
			cursor.GotoNext(instruction => instruction.MatchLdcI4(24));
			cursor.DeleteUntil(instruction => instruction.MatchLdsfld(typeof(WorldGen), nameof(WorldGen.getGoodWorldGen)));
		}
		else
		{
			cursor.GotoNext(instruction => instruction.MatchLdsfld(typeof(WorldGen), nameof(WorldGen.getGoodWorldGen)));
			cursor.DeleteUntil(instruction => instruction.MatchLdcI4(24));
			cursor.GotoNext(instruction => instruction.MatchLdsfld(typeof(WorldGen), nameof(WorldGen.getGoodWorldGen)));
			cursor.DeleteUntil(instruction => instruction.MatchRet());
		}
	}
}