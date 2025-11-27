using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Common.Utils;
using Pantheon.Content.General.Dusts;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Pantheon.Content.Combat.Melee.Spears;

public abstract class BaseSpearProjectile : ModProjectile
{
	protected Player Owner => Main.player[Projectile.owner];
	private int Lifetime = 18;

	protected virtual float Range => 64;

	protected bool shouldBeHeldProj = true;

	public virtual int DamageIncreasePerPierce => 0;

	protected bool pointingBackwards => Owner.direction == -1; //((Projectile.rotation + MathF.PI) % MathHelper.TwoPi < MathF.PI / 2);
	private bool hasReachedPeak = false;

	protected float lifetime
	{
		get => Projectile.ai[0];
		set => Projectile.ai[0] = value;
	}

	private float effectiveProgress
	{
		get => Projectile.ai[2];
		set => Projectile.ai[2] = value;
	}

	public float progress => (lifetime - Projectile.timeLeft)/lifetime;

	public override void SetStaticDefaults()
	{
		
		base.SetStaticDefaults();
	}

	public override void SetDefaults()
	{
		Projectile.width = 24;
		Projectile.height = 24;
		Projectile.hide = true;
		Projectile.scale = 1.5f;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = Lifetime;

		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = Lifetime;
		base.SetDefaults();
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.knockBack *= 0.5f;
		Projectile.rotation = Projectile.velocity.ToRotation();
		Projectile.rotation += Main.rand.NextFloat(-0.1f, 0.1f);
		lifetime = Lifetime;
		base.OnSpawn(source);
	}


	public override void AI()
	{
		if (shouldBeHeldProj)
		{
			Owner.heldProj = Projectile.whoAmI;
		}
		else
		{
			Projectile.hide = false;
		}
		float offsetMultipler = MathF.Sin(MathF.Pow(progress, 0.9f) * MathF.PI);
		float finalprogress = PositionLerp(offsetMultipler);
		if (Math.Abs(finalprogress - 1) < 0.05f && !hasReachedPeak)
		{
			// Main.NewText(finalprogress);
			OnReachPeakOfStrike();
			hasReachedPeak = true;
		}

		if (hasReachedPeak)
		{
			// Main.NewText(finalprogress);
		}
		Vector2 position = Vector2.UnitX.RotatedBy(Projectile.rotation) * (Range * MathHelper.Lerp(0.5f, 1.5f, finalprogress));
		effectiveProgress = finalprogress;

		Projectile.Center = Owner.MountedCenter + position;
	}

	
	public override bool PreDraw(ref Color lightColor)
	{
		SpriteEffects effect = pointingBackwards ? SpriteEffects.None : SpriteEffects.FlipVertically & SpriteEffects.FlipHorizontally;
		Asset<Texture2D> texture = TextureAssets.Projectile[Projectile.type];
		Main.spriteBatch.Draw(
			texture.Value,
			Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + Owner.gfxOffY),
			null,
			lightColor,
			Projectile.rotation + (MathF.PI * 0.75f),
			Projectile.Size / 4,
			Projectile.scale,
			SpriteEffects.None,
			0f
			);
		return false;
	}

	public virtual void OnReachPeakOfStrike()
	{
		Burst.SpawnBurstDust(ModContent.DustType<Burst>(), Projectile.Center, 0.2f, Color.Silver, 2f, 0.95f);
		Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Vector2.Zero, 0, Color.Red, 2f);
	}
	
	public virtual void OnHitNPCDuringPeakOfStrike(NPC target, ref NPC.HitModifiers modifiers)
	{
		Burst.SpawnBurstDust(ModContent.DustType<Burst>(), target.Center, 0.5f, Color.Silver, 2f, 0.9f);
		// Main.NewText("Rubs my big boner");
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers,
		List<int> overWiresUI)
	{
		// overPlayers.Add(index);
		base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
	}

	public virtual float PositionLerp(float value)
	{
		return Easing.OutCirc(value);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (Math.Abs(effectiveProgress - 1) < 0.25f)
		{
			// the crit is a lie
			modifiers.FinalDamage *= 0.65f; // six seven
			modifiers.SetCrit();
			OnHitNPCDuringPeakOfStrike(target, ref modifiers);
		}
		else
		{
			modifiers.FinalDamage *= MathHelper.Lerp(0.25f, 0.95f, MathF.Pow(effectiveProgress, 1.5f));
			Projectile.damage += 5;
		}
		base.ModifyHitNPC(target, ref modifiers);
	}
}