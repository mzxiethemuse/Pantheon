using Pantheon.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Items.Accessories.Ranger.Flamethrower;

public class Hairspray : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 32;
		Item.accessory = true;
		base.SetDefaults();
	}

	public override void UpdateEquip(Player player)
	{
		if (player.HeldItem.useAmmo == AmmoID.Gel)
		{
			player.GetAttackSpeed(DamageClass.Ranged) += 0.20f;
		}
		base.UpdateEquip(player);
	}
}