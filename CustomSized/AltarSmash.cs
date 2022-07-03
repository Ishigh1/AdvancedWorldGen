using AdvancedWorldGen.BetterVanillaWorldGen;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WorldGen = On.Terraria.WorldGen;

namespace AdvancedWorldGen.CustomSized;

public static class AltarSmash
{
	public static void SmashAltar(WorldGen.orig_SmashAltar orig, int x, int y)
	{
		if (!WorldgenSettings.Revamped || ModLoader.TryGetMod("CalamityMod", out Mod _))
		{
			orig(x, y);
			return;
		}

		if (Main.netMode == NetmodeID.MultiplayerClient || !Main.hardMode || Terraria.WorldGen.noTileActions ||
		    Terraria.WorldGen.gen)
			return;

		int num = Terraria.WorldGen.altarCount % 3;
		int num2 = Terraria.WorldGen.altarCount / 3 + 1;
		float num3 = Main.maxTilesX / 4200f;
		int num4 = 1 - num;
		num3 = num3 * 310f - 85 * num;
		num3 *= 0.85f;
		num3 /= num2;
		bool flag = false;
		if (Main.drunkWorld)
			Terraria.WorldGen.SavedOreTiers.Adamantite = Terraria.WorldGen.SavedOreTiers.Adamantite switch
			{
				TileID.Adamantite => TileID.Titanium,
				TileID.Titanium => TileID.Adamantite,
				_ => Terraria.WorldGen.SavedOreTiers.Adamantite
			};

		Color color = new(50, 255, 130);
		switch (num)
		{
			case 0:
			{
				if (Terraria.WorldGen.SavedOreTiers.Cobalt == -1)
				{
					flag = true;
					Terraria.WorldGen.SavedOreTiers.Cobalt = TileID.Cobalt;
					if (Terraria.WorldGen.genRand.NextBool(2))
						Terraria.WorldGen.SavedOreTiers.Cobalt = TileID.Palladium;
				}

				int num6 = 12;
				if (Terraria.WorldGen.SavedOreTiers.Cobalt == TileID.Palladium)
				{
					num6 += 9;
					num3 *= 0.9f;
				}

				if (Main.netMode == NetmodeID.SinglePlayer)
					Main.NewText(Language.GetTextValue($"LegacyMisc.{num6}"), color);
				else if (Main.netMode == NetmodeID.Server)
					ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Language.GetText($"LegacyMisc.{num6}").Key),
						color);

				num = Terraria.WorldGen.SavedOreTiers.Cobalt;
				num3 *= 1.05f;
				break;
			}
			case 1:
			{
				if (Main.drunkWorld)
					Terraria.WorldGen.SavedOreTiers.Mythril = Terraria.WorldGen.SavedOreTiers.Mythril switch
					{
						TileID.Mythril => TileID.Orichalcum,
						TileID.Orichalcum => TileID.Mythril,
						_ => Terraria.WorldGen.SavedOreTiers.Mythril
					};

				if (Terraria.WorldGen.SavedOreTiers.Mythril == -1)
				{
					flag = true;
					Terraria.WorldGen.SavedOreTiers.Mythril = TileID.Mythril;
					if (Terraria.WorldGen.genRand.NextBool(2))
						Terraria.WorldGen.SavedOreTiers.Mythril = TileID.Orichalcum;
				}

				int num7 = 13;
				if (Terraria.WorldGen.SavedOreTiers.Mythril == TileID.Orichalcum)
				{
					num7 += 9;
					num3 *= 0.9f;
				}

				switch (Main.netMode)
				{
					case NetmodeID.SinglePlayer:
						Main.NewText(Language.GetTextValue($"LegacyMisc.{num7}"), color);
						break;
					case NetmodeID.Server:
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Language.GetText($"LegacyMisc.{num7}").Key),
							color);
						break;
				}

				num = Terraria.WorldGen.SavedOreTiers.Mythril;
				break;
			}
			default:
			{
				if (Main.drunkWorld)
					Terraria.WorldGen.SavedOreTiers.Cobalt = Terraria.WorldGen.SavedOreTiers.Cobalt switch
					{
						TileID.Cobalt => TileID.Palladium,
						TileID.Palladium => TileID.Cobalt,
						_ => Terraria.WorldGen.SavedOreTiers.Cobalt
					};

				if (Terraria.WorldGen.SavedOreTiers.Adamantite == -1)
				{
					flag = true;
					Terraria.WorldGen.SavedOreTiers.Adamantite = TileID.Adamantite;
					if (Terraria.WorldGen.genRand.NextBool(2))
						Terraria.WorldGen.SavedOreTiers.Adamantite = TileID.Titanium;
				}

				int num5 = 14;
				if (Terraria.WorldGen.SavedOreTiers.Adamantite == TileID.Titanium)
				{
					num5 += 9;
					num3 *= 0.9f;
				}

				if (Main.netMode == NetmodeID.SinglePlayer)
					Main.NewText(Language.GetTextValue($"LegacyMisc.{num5}"), color);
				else if (Main.netMode == NetmodeID.Server)
					ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Language.GetText($"LegacyMisc.{num5}").Key),
						color);

				num = Terraria.WorldGen.SavedOreTiers.Adamantite;
				break;
			}
		}

		if (flag)
			NetMessage.SendData(MessageID.WorldData);

		for (int k = 0; k < num3; k++)
		{
			double minY = Main.worldSurface;
			switch (num)
			{
				case TileID.Mythril:
				case TileID.Orichalcum:
					minY = Main.rockLayer;
					break;
				case TileID.Adamantite:
				case TileID.Titanium:
					minY = (Main.rockLayer + Main.rockLayer + Main.maxTilesY) / 3.0;
					break;
			}

			int xx = Terraria.WorldGen.genRand.Next(100, Main.maxTilesX - 100);
			int yy = Terraria.WorldGen.genRand.Next((int)minY, Main.maxTilesY - 150);
			Terraria.WorldGen.OreRunner(xx, yy, Terraria.WorldGen.genRand.Next(5, 9 + num4),
				Terraria.WorldGen.genRand.Next(5, 9 + num4), (ushort)num);
		}

		int num9 = Terraria.WorldGen.genRand.Next(3);
		int tries = 0;
		while (num9 != 2 && tries++ < 1000)
		{
			int xx = Terraria.WorldGen.genRand.Next(100, Main.maxTilesX - 100);
			int yy = Terraria.WorldGen.genRand.Next((int)Main.rockLayer + 50, Main.maxTilesY - 300);
			Tile tile = Main.tile[xx, yy];
			if (!tile.HasTile || tile.TileType != TileID.Stone)
				continue;

			if (num9 == 0)
				tile.TileType = Terraria.WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone;
			else
				tile.TileType = TileID.Pearlstone;

			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendTileSquare(-1, xx, yy);

			break;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			int count = Main.rand.Next(2) + 1;
			for (int _ = 0; _ < count; _++)
				NPC.SpawnOnPlayer(Player.FindClosest(new Vector2(x * 16, x * 16), 16, 16), NPCID.Wraith);
		}

		Terraria.WorldGen.altarCount++;
		AchievementsHelper.NotifyProgressionEvent(AchievementHelperID.Events.SmashDemonAltar);
	}
}