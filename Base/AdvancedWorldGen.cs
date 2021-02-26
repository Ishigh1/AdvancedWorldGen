using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvancedWorldGen.OptionUI;
using AdvancedWorldGen.SpecialOptions;
using Newtonsoft.Json;
using On.Terraria.GameContent.UI.Elements;
using On.Terraria.GameContent.UI.States;
using On.Terraria.IO;
using Terraria;
using Terraria.ModLoader;
using Projectile = IL.Terraria.Projectile;
using WorldGen = On.Terraria.WorldGen;

namespace AdvancedWorldGen
{
	public class AdvancedWorldGen : Mod
	{
		public Crimruption Crimruption;
		public UiChanger UiChanger;

		public override void Load()
		{
			OptionsSelector.OptionDict = JsonConvert.DeserializeObject<Dictionary<string, Option>>(
				Encoding.UTF8.GetString(GetFileBytes("Options.json")));
			TileReplacer.Initialize();

			UiChanger = new UiChanger();
			if (!Main.dedServ)
			{
				UiChanger.OptionsTexture = GetTexture("Images/WorldOptions");
				UiChanger.CopyOptionsTexture = GetTexture("Images/CopyWorldButton");
			}

			Crimruption = new Crimruption();
			Crimruption.Load();

			UIWorldCreation.AddDescriptionPanel += UiChanger.TweakWorldGenUi;
			UIWorldListItem.ctor += UiChanger.CopySettingsButton;
			WorldGen.worldGenCallback += UiChanger.ThreadifyWorldGen;
			UIWorldLoad.ctor += UiChanger.AddCancel;
			// Removed as On.[...].UIWorldCreation.ProcessSpecialWorldSeeds doesn't exist
			// UIWorldCreation.ProcessSpecialWorldSeeds += ModifiedWorld.IgnoreSpecialSeed;
			IL.Terraria.WorldGen.GenerateWorld += ModifiedWorld.OverrideWorldOptions;
			WorldFile.CreateMetadata += DedServUi.DedServOptions;

			WorldGen.NotTheBees += ClassicOptions.SmallNotTheBees;
			IL.Terraria.WorldGen.makeTemple += ClassicOptions.ReduceTemple;

			On.Terraria.Main.UpdateTime_StartDay += ModifiedWorld.OnDawn;
			On.Terraria.Main.UpdateTime_StartNight += ModifiedWorld.OnDusk;
			On.Terraria.Main.checkXMas += SnowWorld.MainOncheckXMas;
			Projectile.Kill += SnowWorld.RemoveSnowDropDuringChristmas;

			IL.Terraria.WorldGen.MakeDungeon += Crimruption.CrimruptionChest;
		}

		public override void Unload()
		{
			UIWorldCreation.AddDescriptionPanel -= UiChanger.TweakWorldGenUi;
			UIWorldListItem.ctor -= UiChanger.CopySettingsButton;
			WorldGen.worldGenCallback -= UiChanger.ThreadifyWorldGen;
			UIWorldLoad.ctor -= UiChanger.AddCancel;
			IL.Terraria.WorldGen.GenerateWorld -= ModifiedWorld.OverrideWorldOptions;
			WorldFile.CreateMetadata -= DedServUi.DedServOptions;

			WorldGen.NotTheBees -= ClassicOptions.SmallNotTheBees;
			IL.Terraria.WorldGen.makeTemple -= ClassicOptions.ReduceTemple;

			On.Terraria.Main.UpdateTime_StartDay -= ModifiedWorld.OnDawn;
			On.Terraria.Main.UpdateTime_StartNight -= ModifiedWorld.OnDusk;
			On.Terraria.Main.checkXMas -= SnowWorld.MainOncheckXMas;
			Projectile.Kill -= SnowWorld.RemoveSnowDropDuringChristmas;

			IL.Terraria.WorldGen.MakeDungeon -= Crimruption.CrimruptionChest;

			UiChanger.OptionsTexture = null;
			UiChanger = null;

			Crimruption.Unload();
			Crimruption = null;

			OptionsSelector.OptionDict = null;
			TileReplacer.Unload();

			ModifiedWorld.OptionHelper = null;
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			OptionHelper.HandlePacket(reader, whoAmI);
		}
	}
}