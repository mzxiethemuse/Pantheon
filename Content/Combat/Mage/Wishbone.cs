using Pantheon.Assets;
using Pantheon.Common;
using Pantheon.Common.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Combat.Mage;

//running out of mana causes you to overheat / overheating nullifies mana sickness and grants 100 mana over 5 secs / 
public class Wishbone : ModItem
{

	// public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(15);

	public override void Load()
	{
		ChestLootGen.AddLoot(new ChestLoot(ChestLootGen.ChestType.Sandstone, Type, 1, 2));
	}

	public override void SetDefaults()
	{
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
		base.SetDefaults();
	}

	public override void UpdateEquip(Player player)
	{
		player.ManaPlayer.wishbone = true;
		
		base.UpdateEquip(player);
	}

	public override void AddRecipes()
	{
		var recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.DesertFossil, 8);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
		base.AddRecipes();
	}
}

public class WishboneBuff : ModBuff
{
	public override string Texture => AssetDirectory.Placeholder + "GenericBuff";
}