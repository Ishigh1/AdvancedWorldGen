using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using On.Terraria;
using Terraria.ModLoader;
using static Terraria.ID.TileID;
using Projectile = IL.Terraria.Projectile;

namespace AdvancedSeedGen
{
	public class AdvancedSeedGen : Mod
	{
		public static List<string> Options;
		public static Dictionary<string, List<string>> SeedTranslator;
		public static List<ushort> NotReplaced;
		public SeedHelper SeedHelper;

		public override void Load()
		{
			NotReplaced = new List<ushort>
			{
				ClosedDoor, MagicalIceBlock, Traps, Boulder, Teleporter, PlanterBox, TallGateClosed
			};
			SeedTranslator = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
				Encoding.UTF8.GetString(GetFileBytes("SeedTranslation.json")));
			TileReplacer.Initialize();

			Main.OnSeedSelected += WorldFileDataOnSetSeed;
			Main.checkXMas += SnowWorld.MainOncheckXMas;
			Projectile.Kill += SnowWorld.RemoveSnowDropDuringChristmas;
		}

		public override void Unload()
		{
			NotReplaced = null;
			SeedTranslator = null;
			TileReplacer.Unload();

			Main.OnSeedSelected -= WorldFileDataOnSetSeed;
			Main.checkXMas -= SnowWorld.MainOncheckXMas;
			Projectile.Kill -= SnowWorld.RemoveSnowDropDuringChristmas;
		}

		private static void WorldFileDataOnSetSeed(Main.orig_OnSeedSelected origOnSeedSelected, Terraria.Main main,
			string seedtext)
		{
			string seedText = SeedHelper.TweakSeedText(seedtext);
			origOnSeedSelected(main, seedText);
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			SeedHelper.HandlePacket(reader, whoAmI);
		}
	}
}