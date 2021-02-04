using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvancedSeedGen.SeedUI;
using AdvancedSeedGen.SpecialSeeds;
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
		public static List<int> NotReplaced;
		public CancelWorldGen CancelWorldGen;
		public SeedHelper SeedHelper;

		public override void Load()
		{
			NotReplaced = new List<int>
			{
				ClosedDoor, MagicalIceBlock, Traps, Boulder, Teleporter, MetalBars, PlanterBox, TrapdoorClosed, TallGateClosed
			};
			SeedHelper.SeedTranslator = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
				Encoding.UTF8.GetString(GetFileBytes("SeedTranslation.json")));
			TileReplacer.Initialize();

			CancelWorldGen = new CancelWorldGen();

			Main.OnSeedSelected += CancelWorldGen.WorldFileDataOnSetSeed;
			UIWorldLoad.ctor += CancelWorldGen.AddCancel;
			UIElement.Append += CancelWorldGen.StealProgressBar;
			UIVirtualKeyboard.ctor += SeedSelector.AddSeedInterface;

			Main.checkXMas += SnowWorld.MainOncheckXMas;
			Projectile.Kill += SnowWorld.RemoveSnowDropDuringChristmas;
		}

		public override void Unload()
		{
			Main.OnSeedSelected -= CancelWorldGen.WorldFileDataOnSetSeed;
			UIWorldLoad.ctor -= CancelWorldGen.AddCancel;
			UIElement.Append -= CancelWorldGen.StealProgressBar;
			UIVirtualKeyboard.ctor += SeedSelector.AddSeedInterface;

			Main.checkXMas -= SnowWorld.MainOncheckXMas;
			Projectile.Kill -= SnowWorld.RemoveSnowDropDuringChristmas;

			TileReplacer.Unload();
			NotReplaced = null;
			Options = null;
			SeedHelper.SeedTranslator = null;
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			SeedHelper.HandlePacket(reader, whoAmI);
		}
	}
}