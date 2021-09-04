using System;
using Terraria.Localization;

namespace AdvancedWorldGen.BetterVanillaWorldGen
{
	public class RockUnderHellException : Exception
	{
		public RockUnderHellException() :
			base(Language.GetTextValue("Mods.AdvancedWorldGen.Exceptions.RockUnderHell"))
		{
		}
	}
}