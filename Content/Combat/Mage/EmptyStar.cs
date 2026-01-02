using System.Collections.Generic;
using Pantheon.Assets;
using Pantheon.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace Pantheon.Content.Combat.Mage;

public class EmptyStar : ModItem
{
	public override string Texture => AssetDirectory.Placeholder + "GenericItem";

	// public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(15);
 
	public override void SetDefaults()
	{
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
		base.SetDefaults();
	}

	public override void UpdateEquip(Player player)
	{
		player.GetDamage(DamageClass.Magic) *= 1 + 0.20f * (player.statManaMax - player.statMana) / (float)(player.statManaMax);
		player.manaCost -= (player.statManaMax - player.statMana) / (float)(player.statManaMax) * 0.2f;
		player.ManaPlayer.hollowRock = true;
		base.UpdateEquip(player);
	}
	
	
	public override Color? GetAlpha(Color lightColor)
	{
		return Color.DodgerBlue * 0.8f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<ManaSink>();
		recipe.AddIngredient(ItemID.SoulofNight, 6);
		recipe.AddTile(TileID.MythrilAnvil);
		recipe.Register();
		base.AddRecipes();
	}
	
	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		if (Main.LocalPlayer.ManaPlayer.pincushion)
		{
			tooltips.Add(new TooltipLine(Mod, "PinCushionSynergy", $"[i:{ModContent.ItemType<CeremonialPincushion>()}] : Magic damage increases with damage taken."));
		}
		base.ModifyTooltips(tooltips);
	}
}