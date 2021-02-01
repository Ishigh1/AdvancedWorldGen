using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvancedSeedGen.SeedUI;
using Newtonsoft.Json;
using On.Terraria;
using On.Terraria.GameContent.UI.States;
using On.Terraria.UI;
using Terraria.ModLoader;
using static Terraria.ID.TileID;
using Projectile = IL.Terraria.Projectile;

namespace AdvancedSeedGen
{
	public class AdvancedSeedGen : Mod
	{
		public static List<string> Options;
		public static Dictionary<string, List<string>> SeedTranslator;
		public static List<int> NotReplaced;
		public SeedHelper SeedHelper;
		public CancelWorldGen CancelWorldGen;

		public override void Load()
		{
			NotReplaced = new List<int>
			{
				ClosedDoor, MagicalIceBlock, Traps, Boulder, Teleporter, PlanterBox, TallGateClosed
			};
			SeedTranslator = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
				Encoding.UTF8.GetString(GetFileBytes("SeedTranslation.json")));
			TileReplacer.Initialize();

			CancelWorldGen = new CancelWorldGen();
			
			Main.OnSeedSelected += CancelWorldGen.WorldFileDataOnSetSeed;
			UIWorldLoad.ctor += CancelWorldGen.AddCancel;
			UIElement.Append += CancelWorldGen.StealProgressBar;
			Main.checkXMas += SnowWorld.MainOncheckXMas;
			Projectile.Kill += SnowWorld.RemoveSnowDropDuringChristmas;
		}

		public override void Unload()
		{
			Main.OnSeedSelected -= CancelWorldGen.WorldFileDataOnSetSeed;
			UIWorldLoad.ctor -= CancelWorldGen.AddCancel;
			UIElement.Append -= CancelWorldGen.StealProgressBar;
			Main.checkXMas -= SnowWorld.MainOncheckXMas;
			Projectile.Kill -= SnowWorld.RemoveSnowDropDuringChristmas;
			
			CancelWorldGen = null;
			
			TileReplacer.Unload();
			NotReplaced = null;
			SeedTranslator = null;
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			SeedHelper.HandlePacket(reader, whoAmI);
		}
	}
}