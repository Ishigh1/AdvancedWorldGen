using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvancedWorldGen.SeedUI;
using AdvancedWorldGen.SpecialSeeds;
using Newtonsoft.Json;
using On.Terraria.GameContent.UI.States;
using Terraria;
using Terraria.ModLoader;
using Projectile = IL.Terraria.Projectile;
using WorldGen = On.Terraria.WorldGen;

namespace AdvancedWorldGen
{
	public class AdvancedWorldGen : Mod
	{
		public UiChanger UiChanger;

		public override void Load()
		{
			OptionsSelector.OptionDict = JsonConvert.DeserializeObject<Dictionary<string, Option>>(
				Encoding.UTF8.GetString(GetFileBytes("Options.json")));
			TileReplacer.Initialize();

			UiChanger = new UiChanger();
			if (!Main.dedServ) UiChanger.OptionsTexture = GetTexture("Images/WorldOptions");

			UIWorldCreation.AddDescriptionPanel += UiChanger.TweakWorldGenUi;
			WorldGen.worldGenCallback += UiChanger.ThreadifyWorldGen;
			UIWorldLoad.ctor += UiChanger.AddCancel;

			On.Terraria.Main.UpdateTime_StartDay += CustomSeededWorld.OnDawn;
			On.Terraria.Main.UpdateTime_StartNight += CustomSeededWorld.OnDusk;
			On.Terraria.Main.checkXMas += SnowWorld.MainOncheckXMas;
			Projectile.Kill += SnowWorld.RemoveSnowDropDuringChristmas;
		}

		public override void Unload()
		{
			UIWorldCreation.AddDescriptionPanel -= UiChanger.TweakWorldGenUi;
			WorldGen.worldGenCallback -= UiChanger.ThreadifyWorldGen;
			UIWorldLoad.ctor -= UiChanger.AddCancel;

			On.Terraria.Main.UpdateTime_StartDay -= CustomSeededWorld.OnDawn;
			On.Terraria.Main.UpdateTime_StartNight -= CustomSeededWorld.OnDusk;
			On.Terraria.Main.checkXMas -= SnowWorld.MainOncheckXMas;
			Projectile.Kill -= SnowWorld.RemoveSnowDropDuringChristmas;

			UiChanger = null;

			OptionsSelector.OptionDict = null;
			TileReplacer.Unload();
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			SeedHelper.HandlePacket(reader, whoAmI);
		}
	}
}