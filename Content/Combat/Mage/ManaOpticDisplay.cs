using Pantheon.Assets;
using Pantheon.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace Pantheon.Content.Combat.Accessories.Mage;

public class ManaOpticDisplay : ModItem
{
	public override string Texture => AssetDirectory.Placeholder + "GenericItem";

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(15);
 
	public override void SetDefaults()
	{
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
		base.SetDefaults();
	}

	public override void UpdateEquip(Player player)
	{
		player.GetModPlayer<ManaPlayer>().displayManaRegenTicks = true;
		player.GetModPlayer<ManaPlayer>().displayManaUsage = true;

		base.UpdateEquip(player);
	}
	
	
	public override Color? GetAlpha(Color lightColor)
	{
		return Color.DodgerBlue * 0.8f;
	}
	
	
}