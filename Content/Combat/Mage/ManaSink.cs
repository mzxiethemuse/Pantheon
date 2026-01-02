using System.Collections.Generic;
using Pantheon.Assets;
using Pantheon.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace Pantheon.Content.Combat.Mage;

public class ManaSink : ModItem
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
		player.GetDamage(DamageClass.Magic) += 0.20f *
		                                       ((player.statManaMax2 - player.statMana) / (float)(player.statManaMax2));
		player.ManaPlayer.hollowRock = true;
		base.UpdateEquip(player);
	}
	
	
	public override Color? GetAlpha(Color lightColor)
	{
		return Color.DodgerBlue * 0.8f;
	}
	
	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		if (Main.LocalPlayer.ManaPlayer.pincushion)
		{
			tooltips.Add(new TooltipLine(Mod, "WishboneSynergy", $"[i:{ModContent.ItemType<CeremonialPincushion>()}] : Magic damage increases with damage taken."));
		}
		base.ModifyTooltips(tooltips);
	}
}