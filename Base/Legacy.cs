using System.Collections.Generic;

namespace AdvancedWorldGen.Base;

public class Legacy
{
	public static void ReplaceOldOptions(ICollection<string> options)
	{
		#region 2.6, 29/12/2021

		if (options.Remove("Crimruption"))
			options.Add("Drunk.Crimruption");

		#endregion
	}
}