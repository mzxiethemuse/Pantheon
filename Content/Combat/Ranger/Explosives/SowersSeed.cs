using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Common.Graphics;
using Pantheon.Content.General.Dusts;
using Pantheon.Content.Projectiles;
using Pantheon.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Combat.Ranger.Explosives;

public class SowersSeedItem : ModItem
{
	public override string Texture => AssetReferences.Content.Combat.Ranger.Explosives.SowersSeed.KEY;
	public override void SetDefaults()
    {
    	Item.useTime = 17;
    	Item.useAnimation = 17  ;
    	Item.useStyle = ItemUseStyleID.Swing;
    	Item.damage = 17;
    	Item.maxStack = 999;
    	Item.consumable = true;
	    Item.noMelee = true;
	    Item.noUseGraphic = true;
    	Item.rare = ItemRarityID.Pink;
    	Item.DamageType = DamageClass.Ranged;
    	Item.shootSpeed = 9f;
    	Item.shoot = ModContent.ProjectileType<SowersSeed>();
    }
	
	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type,
    	int damage, float knockback)
    {
    	var proj = Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(-0.25 * player.direction).RotatedByRandom(0.45), type, damage, knockback, player.whoAmI);
    	return false;
    }
}

public class SowersSeed : ModProjectile
{
	public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Seed}";

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailingMode[Type] = 3;
		ProjectileID.Sets.TrailCacheLength[Type] = 24;
		base.SetStaticDefaults();
	}

	public override void SetDefaults()
	{
		Projectile.width = 12;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.scale = 1.35f;
		base.SetDefaults();
	}

	public override void AI()
	{
		Projectile.velocity.Y += 10 / 60f;
		base.AI();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var texture = AssetReferences.Assets.Vfx.SoftGlowUnmultiplied.Asset.Value;
		var snapshot = new SpriteBatchSnapshot(Main.spriteBatch);
		PixelationRenderTarget.AddPixelatedRenderAction(() =>
		{
			Main.spriteBatch.With(snapshot with {matrix = SpriteBatchUtils.RescaleMatrix(snapshot.matrix, 0.5f)}, () => {
				for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
				{
					float alpha = 1 - (i / (float)Projectile.oldPos.Length);
					Main.spriteBatch.Draw(texture, Projectile.oldPos[i] - Main.screenPosition + (Projectile.Size / 4),
						null, Color.Orange with { A = 0 } * alpha * 0.2f, 0f, texture.Size() / 2, 0.025f * alpha,
						SpriteEffects.None, 0f);
					Main.spriteBatch.Draw(texture, Projectile.oldPos[i] - Main.screenPosition + (Projectile.Size / 4),
						null, Color.Red with { A = 0 } * alpha * 0.2f, 0f, texture.Size() / 2, 0.035f * alpha,
						SpriteEffects.None, 0f);
				}
			});
		});

		return true;
	}

	public override void OnKill(int timeLeft)
	{
		Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Starboom>(), null, 0, Color.DarkGray, 5);
		Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Starboom>(), null, 0, Color.Black, 4);

		SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
		for (int i = 0; i < 20; i++)
		{
			Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dirt>(), null, 0, new Color(0.02f, 0.02f, 0.02f), 8);

			Dust.NewDustDirect(Projectile.Center - new Vector2(40, 40), 80, 80, DustID.Flare, 0, 0, 0, default, 2.65f).noGravity = true;
			Dust.NewDust(Projectile.Center - new Vector2(40, 40), 80, 80, DustID.Smoke, 0, 0, 0, default, 1.5f);

			Dust.NewDust(Projectile.Center - new Vector2(40, 40), 80, 80, DustID.Smoke);
		}
		Hitbox.SpawnHitbox(Projectile.GetSource_Death(), Projectile.Center, 80, 80, Projectile.damage, 10, -1, true);
		base.OnKill(timeLeft);
	}
}