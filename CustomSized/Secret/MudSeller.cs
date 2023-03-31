namespace AdvancedWorldGen.CustomSized.Secret;

public class MudSeller : GlobalNPC
{
	public override void ModifyShop(NPCShop shop)
	{
		if (shop.NpcType == NPCID.Dryad)
		{
			Condition condition = new("TempleWorld", () => Special.TempleWorld);
			Item mudBlock = new(ItemID.MudBlock)
			{
				shopCustomPrice = Item.buyPrice(gold: 20)
			};
			shop.Add(mudBlock, condition);
			
			Item waterBucket = new(ItemID.WaterBucket)
			{
				shopCustomPrice = Item.buyPrice(5)
			};
			shop.Add(waterBucket, condition);
		}
	}
}