using System.Drawing;
using Pantheon.Assets;
using Pantheon.Common;
using Pantheon.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace Pantheon.Content.Items.Accessories.Summoner;

public class Manastorm : ModItem
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
		player.GetModPlayer<ManaPlayer>().manastorm = true;
		base.UpdateEquip(player);
	}
	
	
	public override Color? GetAlpha(Color lightColor)
	{
		return Color.DodgerBlue * 0.8f;
	}
	
	
}