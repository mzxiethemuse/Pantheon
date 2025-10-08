using Microsoft.Xna.Framework;
using Pantheon.Assets;
using Pantheon.Common;
using Pantheon.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Items
{ 
	// This is a basic item template.
	// Please see tModLoader's ExampleMod for every other example:
	// https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
	public class DebugRod : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 50;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}
		

		public override bool? UseItem(Player player)
		{
			Burst.SpawnBurstDust(ModContent.DustType<Burst>(), Main.MouseWorld, 0.5f, Color.Aquamarine * 1f, 4f, 0.9f, 3f);

			return true;
		}
	}
}
