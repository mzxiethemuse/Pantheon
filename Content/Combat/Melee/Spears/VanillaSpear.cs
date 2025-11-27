using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Common.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Combat.Melee.Spears;

/// Abstraction upon abstraction! This is divine fucking intellect
public class VanillaSpear : BaseSpearProjectile
{
	public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Spear}";
	public int ProjectileIDToUseForTexture = -1;

	private new int Lifetime = 28;

	public float RangeMultipler = 1f;
	protected override float Range => TextureAssets.Projectile[ProjectileIDToUseForTexture].Size().Length() * RangeMultipler * 1.25f;

	public override bool PreAI()
	{
		return base.PreAI();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		
		if (ProjectileIDToUseForTexture == ProjectileID.Gungnir && RangeMultipler > 0.5f)
		{
			lightColor = Color.LightGoldenrodYellow * 0.24f;
		}
		if (ProjectileIDToUseForTexture != -1)
		{
			SpriteEffects effect = pointingBackwards
				? SpriteEffects.None
				: SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally;
			Asset<Texture2D> texture = TextureAssets.Projectile[ProjectileIDToUseForTexture];
			Main.spriteBatch.Draw(
				texture.Value,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + Owner.gfxOffY),
				null,
				lightColor,
				(Projectile.rotation + (MathF.PI * 0.75f)),
				Projectile.Size * 0.1f,
				Projectile.scale,
				SpriteEffects.None,
				0f
			);
		}
		return false;
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(ProjectileIDToUseForTexture);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		var recievedTextureID = reader.ReadInt32();
		if (ProjectileIDToUseForTexture != -1)
		{
			ProjectileIDToUseForTexture = recievedTextureID;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (ProjectileIDToUseForTexture == ProjectileID.DarkLance)
		{
			Projectile.NewProjectileDirect(Owner.GetProjectileSource_Item(Owner.HeldItem),
				target.Center - new Vector2(0, 8), new Vector2(0, 2).RotatedByRandom(0.2),
				ProjectileID.LightsBane, Projectile.damage / 4, Projectile.knockBack,
				Owner.whoAmI, 1f);
		}
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void OnHitNPCDuringPeakOfStrike(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (ProjectileIDToUseForTexture == ProjectileID.Gungnir)
		{
			if (Main.projectile.Count(proj => proj.type == Projectile.type && proj.active) < 3 && Projectile.ai[1] < 1) {
				var player = Main.player[Projectile.owner];

				var spear = Projectile.NewProjectileDirect(player.GetProjectileSource_Item(player.HeldItem),
					player.Center, Projectile.velocity,
					ModContent.ProjectileType<VanillaSpear>(), Projectile.damage / 4, Projectile.knockBack,
					player.whoAmI);
				if (spear.ModProjectile is VanillaSpear vanillaSpear)
				{
					vanillaSpear.ProjectileIDToUseForTexture = (player.HeldItem.shoot);
					vanillaSpear.shouldBeHeldProj = false;
					spear.ai[1] = Projectile.ai[1];
					spear.timeLeft += 7;
					vanillaSpear.lifetime += 7;
					vanillaSpear.RangeMultipler =
						MathHelper.Clamp(
							((VanillaSpear)(Projectile.ModProjectile)).RangeMultipler * 1.25f +
							Main.rand.NextFloat(0, 0.25f), 1f, 1.2f);

				}
				
			}
			else
			{
				modifiers.DisableCrit();
			}
			Projectile.ai[1] += 1f;
		}
		
		
	}
}

public class VanillaSpearReplacements : GlobalItem
{
	// most of these are just magic numbers rather than actual IDs because it was quicker just to go from the wiki
	private static readonly int[] VanillaSpears = [
		ItemID.Spear,
		ItemID.Trident,
		ItemID.DarkLance,
		ItemID.ThunderSpear,
		2332,
		802,
		277,
		406,
		1226,
		537,
		550,
		756,
		390,
		2331,
		1193,
		1186,
		1200,
		ItemID.ChlorophytePartisan
	];
	
	public override bool AppliesToEntity(Item entity, bool lateInstantiation) => VanillaSpears.Contains(entity.type);

	public override void SetDefaults(Item entity)
	{
		base.SetDefaults(entity);
		entity.useStyle = ItemUseStyleID.Rapier;
		entity.holdStyle = ItemHoldStyleID.HoldUp;
		entity.noUseGraphic = false;
		if (entity.type == ItemID.Gungnir)
		{
			entity.useTime += 10;
			entity.useAnimation += 10;
		}
	}

	public override void HoldStyle(Item item, Player player, Rectangle heldItemFrame)
	{
		SpearItemHelpers.HoldStyle(item, player, heldItemFrame);
		base.HoldStyle(item, player, heldItemFrame);
	}

	


	// beyond this point you reach "holdusegraphicfuckshit gorge"
	public override void HoldItem(Item item, Player player)
	{
		SpearItemHelpers.HoldItem(item, player);
	}

	public override Vector2? HoldoutOrigin(int type)
	{
		return TextureAssets.Item[type].Size() / 2;
	}


	public override bool? UseItem(Item item, Player player)
	{
		item.noUseGraphic = true;
		return base.UseItem(item, player);
	}

	public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type,
		int damage, float knockback)
	{
		
		Projectile spear = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<VanillaSpear>(), damage, knockback, player.whoAmI);
		if (spear.ModProjectile is VanillaSpear vanillaSpear)
		{
			vanillaSpear.ProjectileIDToUseForTexture = item.shoot;
			vanillaSpear.RangeMultipler = 0.5f;

		}


		((VanillaSpear)(spear.ModProjectile)).ProjectileIDToUseForTexture = item.shoot;
		return false;
	}
}