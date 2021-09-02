using AdvancedWorldGen.Base;
using AdvancedWorldGen.Helper;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace AdvancedWorldGen.SpecialOptions
{
	public static class EvilReplacer
	{
		public static void CorruptWorld(GenerationProgress progress)
		{
			progress.Message = Language.GetTextValue("Mods.AdvancedWorldGen.WorldGenMessage.Evil");
			bool isDrunk = WorldGen.drunkWorldGen || ModifiedWorld.OptionsContains("Crimruption");
			bool corruptOnLeft = isDrunk;
			if (isDrunk)
				for (int x = WorldGen.beachDistance; x < Main.maxTilesX - WorldGen.beachDistance; x++)
				for (int y = (int) Main.worldSurface; y < Main.rockLayer; y++)
				{
					Tile tile = Main.tile[x, y];
					if(!tile.IsActive)
						continue;
					ushort tileType = tile.type;
					if (TileID.Sets.Corrupt[tileType])
					{
						corruptOnLeft = x < Main.maxTilesX / 2;
						break;
					}
					else if(TileID.Sets.Crimson[tileType])
					{
						corruptOnLeft = x >= Main.maxTilesX / 2;
						break;
					}
				}

			int conversionType;
			if (isDrunk)
				conversionType = corruptOnLeft ? 1 : 4;
			else
				conversionType = WorldGen.crimson ? 4 : 1;
			for (int x = 0; x < Main.maxTilesX; x++)
			{
				GenPassHelper.SetProgress(progress, x, Main.maxTilesX);
				if (x == Main.maxTilesX / 2 && isDrunk)
					conversionType = corruptOnLeft ? 4 : 1;
				for (int y = 0; y < Main.maxTilesY; y++) WorldGen.Convert(x, y, conversionType, 0);
			}
		}
	}
}