using Pantheon.Assets;
using Pantheon.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace Pantheon.Content.Combat.Mage;

//running out of mana causes you to overheat / overheating nullifies mana sickness and grants 100 mana over 5 secs / 
public class FocusLens : ModItem
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
		player.ManaPlayer().focuslens = true;
		
		base.UpdateEquip(player);
	}
	
	public override Color? GetAlpha(Color lightColor)
	{
		return Color.DarkGray * 0.8f;
	}
}