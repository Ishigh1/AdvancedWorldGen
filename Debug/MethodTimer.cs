#if false
#if SPECIALDEBUG

namespace AdvancedWorldGen.Debug;

public class MethodTimer : ModSystem
{
	public static Dictionary<string, Stopwatch> MethodTimers = new();
	public static List<MethodInfo> MethodMethods = new();
	public static MethodInfo CurrentMethod;
	
	public override void PreWorldGen()
	{
		foreach (Type type in typeof(MethodTimer).Assembly.GetTypes().Concat(typeof(Main).Assembly.GetTypes()))
		foreach (MethodInfo methodInfo in type.GetMethods((BindingFlags)0x3b))
			if (methodInfo.DeclaringType == type && !methodInfo.IsSpecialName && !methodInfo.ContainsGenericParameters && (methodInfo.GetParameters().Length == 0 || !typeof(Delegate).IsAssignableFrom(methodInfo.GetParameters()[0].ParameterType)))
			{
				CurrentMethod = methodInfo;
				AdvancedWorldGenMod.Instance.Logger.Info("Method " + methodInfo.Name);
				try
				{
					HookEndpointManager.Modify(methodInfo, IlStopWatch);
					MethodMethods.Add(methodInfo);
				}
				catch (Exception)
				{
					// ignored
				}
			}
	}

	public override void PostWorldGen()
	{
		MethodMethods.Sort((info, methodInfo) =>
		{
			string methodName1 = info.DeclaringType.FullName + "." + info.Name;
			string methodName2 = methodInfo.DeclaringType.FullName + "." + methodInfo.Name;
			return MethodTimers[methodName1].Elapsed.CompareTo(MethodTimers[methodName2].Elapsed);
		});
		foreach (MethodInfo? methodInfo in MethodMethods)
		{
			string methodName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name;
			AdvancedWorldGenMod.Instance.Logger.Info("Method " + methodInfo + " has taken " + MethodTimers[methodName].Elapsed.Humanize(2));
			HookEndpointManager.Unmodify(methodInfo, IlStopWatch);
		}
	}

	public static void IlStopWatch(ILContext il)
	{
		string methodName = CurrentMethod.DeclaringType.FullName + "." + CurrentMethod.Name;
		MethodTimers[methodName] = new Stopwatch();
		ILCursor cursor = new(il);
		cursor.EmitDelegate(() =>
		{
			MethodTimers[methodName].Start();
		});
		cursor.GotoNext(MoveType.Before, instruction => instruction.MatchRet());
		HashSet<ILLabel> labels = new();
		while (cursor.Prev.MatchNop() || cursor.Prev.OpCode.StackBehaviourPush == StackBehaviour.Push1)
		{
			labels.UnionWith(cursor.IncomingLabels);
			cursor.Next = cursor.Prev;
		}
		labels.UnionWith(cursor.IncomingLabels);

		Instruction? instruction = cursor.Prev;
		cursor.EmitDelegate(() =>
		{
			MethodTimers[methodName].Stop();
		});
		cursor.Prev = instruction;
		foreach (ILLabel ilLabel in labels) cursor.MarkLabel(ilLabel);
	}
}
#endif
#endif