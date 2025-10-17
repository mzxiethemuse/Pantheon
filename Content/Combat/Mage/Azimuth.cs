using System.Collections.Generic;
using Pantheon.Assets;
using Pantheon.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace Pantheon.Content.Combat.Accessories.Mage;

public class Azimuth : ModItem
{

	public override string Texture => AssetDirectory.Placeholder + "GenericItem";

	public override void SetDefaults()
	{
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
		base.SetDefaults();
	}

	public override void UpdateEquip(Player player)
	{
		player.ManaPlayer().manabank = true;
		
		base.UpdateEquip(player);
	}
	
	public override Color? GetAlpha(Color lightColor)
	{
		return Color.DarkBlue * 0.6f;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		if (Main.LocalPlayer.ManaPlayer().wishbone)
		{
			tooltips.Add(new TooltipLine(Mod, "WishboneSynergy", $"[i:{ModContent.ItemType<Wishbone>()}] : Activates wishbone effect when mana bank is activated."));
		}
		base.ModifyTooltips(tooltips);
	}
	
	public override void AddRecipes()
	{
		var recipe = CreateRecipe();
		recipe.AddRecipeGroup("Pantheon:PlatinumBar", 8);
		recipe.AddIngredient(ItemID.FallenStar, 15);
		recipe.AddTile(TileID.Anvils);
		recipe.AddCustomShimmerResult(ModContent.ItemType<Nadir>());
		recipe.Register();
	}
}