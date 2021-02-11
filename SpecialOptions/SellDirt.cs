using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AdvancedWorldGen.SpecialSeeds
{
	public class SellDirt : GlobalNPC
	{
		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			if (type == NPCID.Dryad && CustomSeededWorld.OptionsContains("Random"))
			{
				Item expensiveDirt = shop.item[nextSlot++];
				expensiveDirt.SetDefaults(ItemID.DirtBlock);
				expensiveDirt.shopCustomPrice = Item.buyPrice(gold: 20);
			}
		}
	}
}