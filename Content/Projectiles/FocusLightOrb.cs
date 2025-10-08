using System;
using Microsoft.Build.Construction;
using Microsoft.Xna.Framework;
using Pantheon.Assets;
using Pantheon.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Projectiles;

public class FocusLightOrb : ModProjectile
{
	// Beer
	public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Ale}";

	public override void SetDefaults()
	{
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.Size = new Vector2(8, 8);
		Projectile.penetrate = -1;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 180;
		Projectile.tileCollide = false;
		base.SetDefaults();
	}

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
		Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.ai[0];
		Projectile.ai[1] = Projectile.ai[0] > 0 ? 1 : -1;
	}

	public override void AI()
	{
		for (int i = 0; i < 3; i++)
		{
			var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), DustID.GemDiamond, Vector2.Zero, 150,
				Color.White, 1.2f);
			d.noGravity = true;
			d.velocity *= 0;
		}
		var a = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), ModContent.DustType<GlowDecaySmokeThingFuck>(), Vector2.Zero, 10,
			Color.White, 1f);
		Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.2f);
		//
		// for (int i = 0; i < 6; i++)
		// {
		// 	var a = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), ModContent.DustType<GlowDecaySmokeThingFuck>(), Vector2.Zero, 100,
		// 		Color.White * 100, 1.2f);
		// }

		float r = Projectile.velocity.ToRotation();
		Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.rotation - r);
		if (MathF.Abs(Projectile.ai[2]) < MathF.Abs(Projectile.ai[0]))
		{
			float addedR = -0.06f * Projectile.ai[1];
			Projectile.ai[2] += addedR;
			Projectile.rotation += addedR;
		}
		else
		{
			Projectile.tileCollide = true;
		}
		base.AI();
	}

	public override bool PreDraw(ref Color lightColor)
	{

		return false;
	}
}