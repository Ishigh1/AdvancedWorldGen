namespace AdvancedWorldGen.SpecialOptions;

public class SellDirt : GlobalNPC
{
	public override void ModifyShop(NPCShop shop)
	{
		if (shop.NpcType == NPCID.Dryad)
		{
			Item expensiveDirt = new(ItemID.DirtBlock)
			{
				shopCustomPrice = Item.buyPrice(gold: 20)
			};
			shop.Add(expensiveDirt, new Condition("randomWorld", () => OptionHelper.OptionsContains("Random")));
		}
	}
}