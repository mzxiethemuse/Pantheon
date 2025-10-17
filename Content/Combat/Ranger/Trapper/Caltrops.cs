using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Combat.Ranger.Trapper;

public class Caltrops : ModProjectile
{
	public override string Texture => "Pantheon/Content/Combat/Ranger/Trapper/Caltrop";
	public override void SetDefaults()
	{
		Projectile.Size = new Vector2(18, 12);
		Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
		Projectile.usesIDStaticNPCImmunity = true;
		Projectile.idStaticNPCHitCooldown = 20;
		Projectile.penetrate = 6;
		Projectile.timeLeft = 60 * 12;
		Projectile.friendly = true;
		base.SetDefaults();
	}

	public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
	{
		fallThrough = false;
		return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		// Projectile.velocity = oldVelocity * new Vector2(0.4f, -0.4f);
		return false;
	}
}

public class CaltropsItem : ModItem
{
	public override string Texture => "Pantheon/Content/Combat/Ranger/Trapper/Caltrop";

	public override void SetDefaults()
	{
		Item.useTime = 17;
		Item.useAnimation = 17;

		Item.damage = 17;
		Item.DamageType = DamageClass.Ranged;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.maxStack = 999;
		Item.height = 24;
		Item.UseSound = SoundID.Item1;
		Item.shootSpeed = 9.5f;
		Item.shoot = ModContent.ProjectileType<Caltrops>();
	}
	
	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type,
		int damage, float knockback)
	{
		float dist = 1000 - player.Center.Distance(Main.MouseWorld);
		double angle = dist * -0.0005f;
		double peepee = angle;
		for (int i = 0; i < 3; i++)
		{
			Projectile.NewProjectile(source, position, velocity.RotatedBy(peepee), type, damage / 3, knockback, player.whoAmI);
			peepee -= angle;
		}

		return false;
	}
}