using System;
using AdvancedWorldGen.BetterVanillaWorldGen;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.Achievements;
using Terraria.Localization;
using Terraria.ID;

namespace AdvancedWorldGen.CustomSized
{
	public static class AltarSmash
	{
		public static void SmashAltar(On.Terraria.WorldGen.orig_SmashAltar orig, int x, int y)
		{
			if (!WorldgenSettings.Revamped)
			{
				orig(x, y);
				return;
			}
			
			if (Main.netMode == NetmodeID.MultiplayerClient || !Main.hardMode || WorldGen.noTileActions || WorldGen.gen)
				return;

			int num = WorldGen.altarCount % 3;
			int num2 = WorldGen.altarCount / 3 + 1;
			float num3 = Main.maxTilesX / 4200f;
			int num4 = 1 - num;
			num3 = num3 * 310f - 85 * num;
			num3 *= 0.85f;
			num3 /= num2;
			bool flag = false;
			if (Main.drunkWorld)
			{
				WorldGen.SavedOreTiers.Adamantite = WorldGen.SavedOreTiers.Adamantite switch
				{
					111 => 223,
					223 => 111,
					_ => WorldGen.SavedOreTiers.Adamantite
				};
			}

			switch (num)
			{
				case 0:
				{
					if (WorldGen.SavedOreTiers.Cobalt == -1)
					{
						flag = true;
						WorldGen.SavedOreTiers.Cobalt = 107;
						if (WorldGen.genRand.Next(2) == 0) WorldGen.SavedOreTiers.Cobalt = 221;
					}

					int num6 = 12;
					if (WorldGen.SavedOreTiers.Cobalt == 221)
					{
						num6 += 9;
						num3 *= 0.9f;
					}

					if (Main.netMode == 0)
						Main.NewText(Lang.misc[num6].Value, 50, byte.MaxValue, 130);
					else if (Main.netMode == 2)
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[num6].Key),
							new Color(50, 255, 130));

					num = WorldGen.SavedOreTiers.Cobalt;
					num3 *= 1.05f;
					break;
				}
				case 1:
				{
					if (Main.drunkWorld)
					{
						WorldGen.SavedOreTiers.Mythril = WorldGen.SavedOreTiers.Mythril switch
						{
							108 => 222,
							222 => 108,
							_ => WorldGen.SavedOreTiers.Mythril
						};
					}

					if (WorldGen.SavedOreTiers.Mythril == -1)
					{
						flag = true;
						WorldGen.SavedOreTiers.Mythril = 108;
						if (WorldGen.genRand.Next(2) == 0) WorldGen.SavedOreTiers.Mythril = 222;
					}

					int num7 = 13;
					if (WorldGen.SavedOreTiers.Mythril == 222)
					{
						num7 += 9;
						num3 *= 0.9f;
					}

					switch (Main.netMode)
					{
						case NetmodeID.SinglePlayer:
							Main.NewText(Lang.misc[num7].Value, 50, byte.MaxValue, 130);
							break;
						case NetmodeID.Server:
							ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[num7].Key),
								new Color(50, 255, 130));
							break;
					}

					num = WorldGen.SavedOreTiers.Mythril;
					break;
				}
				default:
				{
					if (Main.drunkWorld)
					{
						WorldGen.SavedOreTiers.Cobalt = WorldGen.SavedOreTiers.Cobalt switch
						{
							107 => 221,
							221 => 107,
							_ => WorldGen.SavedOreTiers.Cobalt
						};
					}

					if (WorldGen.SavedOreTiers.Adamantite == -1)
					{
						flag = true;
						WorldGen.SavedOreTiers.Adamantite = 111;
						if (WorldGen.genRand.Next(2) == 0) WorldGen.SavedOreTiers.Adamantite = 223;
					}

					int num5 = 14;
					if (WorldGen.SavedOreTiers.Adamantite == 223)
					{
						num5 += 9;
						num3 *= 0.9f;
					}

					if (Main.netMode == 0)
						Main.NewText(Lang.misc[num5].Value, 50, byte.MaxValue, 130);
					else if (Main.netMode == 2)
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[num5].Key),
							new Color(50, 255, 130));

					num = WorldGen.SavedOreTiers.Adamantite;
					break;
				}
			}

			if (flag)
				NetMessage.SendData(7);

			for (int k = 0; (float) k < num3; k++)
			{
				int i2 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
				double num8 = Main.worldSurface;
				switch (num)
				{
					case 108:
					case 222:
						num8 = Main.rockLayer;
						break;
					case 111:
					case 223:
						num8 = (Main.rockLayer + Main.rockLayer + Main.maxTilesY) / 3.0;
						break;
				}

				int j2 = WorldGen.genRand.Next((int) num8, Main.maxTilesY - 150);
				WorldGen.OreRunner(i2, j2, WorldGen.genRand.Next(5, 9 + num4), WorldGen.genRand.Next(5, 9 + num4),
					(ushort) num);
			}

			int num9 = WorldGen.genRand.Next(3);
			int num10 = 0;
			while (num9 != 2 && num10++ < 1000)
			{
				int num11 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
				int num12 = WorldGen.genRand.Next((int) Main.rockLayer + 50, Main.maxTilesY - 300);
				if (!Main.tile[num11, num12].IsActive || Main.tile[num11, num12].type != 1)
					continue;

				if (num9 == 0)
				{
					if (WorldGen.crimson)
						Main.tile[num11, num12].type = 203;
					else
						Main.tile[num11, num12].type = 25;
				}
				else
				{
					Main.tile[num11, num12].type = 117;
				}

				if (Main.netMode == 2)
					NetMessage.SendTileSquare(-1, num11, num12);

				break;
			}

			if (Main.netMode != 1)
			{
				int num13 = Main.rand.Next(2) + 1;
				for (int l = 0; l < num13; l++)
					NPC.SpawnOnPlayer(Player.FindClosest(new Vector2(x * 16, x * 16), 16, 16), 82);
			}

			WorldGen.altarCount++;
			AchievementsHelper.NotifyProgressionEvent(6);
		}
	}
}