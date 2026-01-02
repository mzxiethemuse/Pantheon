using Pantheon.Assets;
using Pantheon.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace Pantheon.Content.Combat.Mage;

public class CeremonialPincushion : ModItem
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
		player.ManaPlayer.pincushion = true;
		
		base.UpdateEquip(player);
	}
	
	public override Color? GetAlpha(Color lightColor)
	{
		return Color.Black * 0.8f;
	}
}