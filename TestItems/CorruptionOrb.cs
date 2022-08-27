namespace AdvancedWorldGen.TestItems;

public class CorruptionOrb : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;
		Item.consumable = true;
		Item.useStyle = ItemUseStyleID.HoldUp;
		Item.useAnimation = 45;
		Item.useTime = 45;
		Item.maxStack = 20;
	}

	public override bool? UseItem(Player player)
	{
		int centerX = (int)(player.position.X / 16);
		int corruptionLeft = centerX - Main.rand.Next(100, 300);
		int corruptionRight = centerX + Main.rand.Next(100, 300);
		int positionY = (int)(player.position.Y / 16);
		WorldGen.worldSurfaceHigh = positionY + 50;
		Corruption.MakeSingleCorruptionBiome(corruptionLeft, corruptionRight, centerX, positionY);
		for (int x = corruptionLeft; x < corruptionRight; x++)
		for (int y = positionY - 20; y < Main.rockLayer; y++)
			WorldGen.TileFrame(x, y);
		return true;
	}
}