using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common;
using Pantheon.Common.Players;
using Pantheon.Content.Combat.Melee;
using Pantheon.Content.Combat.Melee.Spears;
using Pantheon.Content.General.Dusts;
using Pantheon.Content.World.ChallengeAltars;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
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
			Item.useTime = 48;
			Item.useAnimation = 48;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}
		

		public override bool? UseItem(Player player)
		{
			Burst.SpawnBurstDust(ModContent.DustType<Burst>(), Main.MouseWorld, 0.5f, Color.Aquamarine * 1f, 4f, 0.9f, 3f);
			// AltarGeneration.TryGenerateAltarAtPos(Main.MouseWorld.ToTileCoordinates());
			return true;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale,
			int whoAmI)
		{
			spriteBatch.End();
			Shaders.Shine.Value.Parameters["uTime"].SetValue((float)(Main.timeForVisualEffects * 0.05f));
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
				Main.Rasterizer, Shaders.Shine.Value, Main.GameViewMatrix.TransformationMatrix);
			spriteBatch.Draw(TextureAssets.Item[Type].Value, Item.Center - Main.screenPosition, null, lightColor, rotation, Item.Size / 2, scale, SpriteEffects.None, 0f);
			spriteBatch.End();

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
				Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}
	}
}
