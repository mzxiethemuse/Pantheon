using Pantheon.Assets;
using Pantheon.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace Pantheon.Content.Combat.Mage;

public class Nadir : ModItem
{

	public override string Texture => AssetDirectory.Placeholder + "GenericItem";

	public override void SetDefaults()
	{
		Item.rare = ItemRarityID.Red;
		Item.accessory = true;
		base.SetDefaults();
	}

	public override void UpdateEquip(Player player)
	{
		player.ManaPlayer.nadir = true;
		
		base.UpdateEquip(player);
	}
	
	public override Color? GetAlpha(Color lightColor)
	{
		return Color.MediumVioletRed * 0.6f;
	}
}