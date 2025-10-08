using System;
using Microsoft.Xna.Framework;
using Pantheon.Content.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common.Templates;

/// <summary>
/// Traps use indexes 1 & 2 of the ai array.
/// Use Projectile.ai[1] == 1 to check if the trap is primed.
/// </summary>
public abstract class TrapProjectile : ModProjectile
{
	public bool collideY = false;
	public bool collideX = false;
	public virtual float TimeToPrime => 1f;
	public virtual float PrimedDamageIncrease => 0f;
	public override void AI()
	{
		Projectile.ai[2]++;
		if (Projectile.ai[2] % (60 * TimeToPrime) == 0 && Projectile.ai[1] == 0)
		{
			Projectile.ai[1] = 1;
			Projectile.damage = (int)(Projectile.damage * (1 + PrimedDamageIncrease));
			OnPrimed();
		}
	}

	public override bool? CanHitNPC(NPC target)
	{
		return Projectile.ai[1] == 1;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Detonate(target, hit);
	}

	public virtual void OnPrimed()
	{
		SoundEngine.PlaySound(SoundID.MaxMana, Projectile.Center);
	}

	public virtual void Detonate(NPC target, NPC.HitInfo hit)
	{
		
	}

	// public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
	// {
	// 	fallThrough = false;
	// 	return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
	// }

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		collideX = MathF.Abs(oldVelocity.X) > 0 && Projectile.velocity.X == 0;
		collideY = MathF.Abs(oldVelocity.Y) > 0 && Projectile.velocity.Y == 0;

		// Projectile.velocity = oldVelocity * new Vector2(0.4f, -0.4f);
		return false;
	}
}