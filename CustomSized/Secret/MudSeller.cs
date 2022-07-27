namespace AdvancedWorldGen.CustomSized.Secret;

public class MudSeller : GlobalNPC
{
	public override void SetupShop(int type, Chest shop, ref int nextSlot)
	{
		if (Special.TempleWorld && type == NPCID.Dryad)
		{
			shop.item[nextSlot].SetDefaults(ItemID.MudBlock);
			shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 20);

			shop.item[nextSlot].SetDefaults(ItemID.WaterBucket);
			shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(5);
		}
	}
}