using System.Collections.Generic;
using System.Drawing;
using Pantheon.Assets;
using Pantheon.Common;
using Pantheon.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace Pantheon.Content.Items.Accessories.Mage;

//running out of mana causes you to overheat / overheating nullifies mana sickness and grants 100 mana over 5 secs / 
public class ManaBankAccessory : ModItem
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
}