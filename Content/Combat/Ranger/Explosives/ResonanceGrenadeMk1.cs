using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Common.Graphics;
using Pantheon.Content.General.Dusts;
using Pantheon.Content.General.Dusts.Bursts;
using Pantheon.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Combat.Ranger.Explosives;

public class ResonanceGrenadeMk1Item : ModItem
{
	public override string Texture => AssetReferences.Content.Combat.Ranger.Explosives.FlashbangGrenade.KEY;
	public override void SetDefaults()
	{
		Item.useTime = 30;
		Item.useAnimation = 30;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.damage = 17;
		Item.maxStack = 99;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.consumable = true;
		Item.rare = ItemRarityID.Pink;
		Item.DamageType = DamageClass.Ranged;
		Item.shootSpeed = 12f;
		Item.shoot = ModContent.ProjectileType<ResonanceGrenadeMk1>();
	}
	
	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type,
		int damage, float knockback)
	{
		var proj = Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(-0.25 * player.direction), type, damage, knockback, player.whoAmI);
		if (proj.ModProjectile is ResonanceGrenadeMk1 grenade)
		{
			grenade.WorldTarget = Main.MouseWorld;
		}
		return false;
	}
}

public class ResonanceGrenadeMk1 : ModProjectile
{
	public override string Texture => AssetReferences.Content.Combat.Ranger.Explosives.FlashbangGrenade.KEY;
	public Vector2 WorldTarget;
	protected float Time
	{
		get => Projectile.ai[0];
		set => Projectile.ai[0] = value;
	}
	
	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailingMode[Type] = 3;
		ProjectileID.Sets.TrailCacheLength[Type] = 24;
		base.SetStaticDefaults();
	}

	public override void SetDefaults()
	{
		Projectile.timeLeft = 120;
		Projectile.width = AssetReferences.Content.Combat.Ranger.Explosives.FlashbangGrenade.Asset.Width();
		Projectile.height = AssetReferences.Content.Combat.Ranger.Explosives.FlashbangGrenade.Asset.Height();
	}
	
	public override void AI()
	{
		float t = 45;
		Time++;
		Projectile.rotation += Projectile.velocity.X * 0.02f;
		if (Projectile.Distance(WorldTarget) < 100f && Time < t)
		{
			Time = t;
		}
		if (Time >= t)
		{
			//takes 30 frames to do
			Projectile.velocity *= 0.9f;
			if (Time > t + 25)
			{
				Projectile.Kill();
			}
		}
		else
		{
			Projectile.velocity.Y += 9 / 60f;
		}
		base.AI();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Trails.DrawSoftTrail(Projectile.oldPos, null, progress => Color.White * 0.05f, progress => 1 - (progress), progress => 0.5f *
			(1 - progress), new Vector2(8, 7));
		float t = 45;

		var asset = AssetReferences.Content.Combat.Ranger.Explosives.FlashbangGrenade.Asset;
		if (Time > t)
		{
			var snapshot = new SpriteBatchSnapshot(Main.spriteBatch);
			Shaders.SolidColor.Asset.Value.Parameters["ucolor"].SetValue(Color.White.ToVector4());
			Main.spriteBatch.End();
			Main.spriteBatch.BeginFromSnapshot(snapshot with {effect = Shaders.SolidColor.Asset.Value});
			Main.spriteBatch.Draw(asset.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(16, 14), 1f, SpriteEffects.None, 0f);
			Main.spriteBatch.End();
			Main.spriteBatch.BeginFromSnapshot(snapshot);
		}


		var alpha = 1 - ((Time - 60) / 60f);
		Main.spriteBatch.Draw(asset.Value, Projectile.Center - Main.screenPosition, null, lightColor * (Time > 60 ? alpha : 1f), Projectile.rotation, new Vector2(16, 14), 1f, SpriteEffects.None, 0f);

		return false;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.velocity = (oldVelocity * -1f).RotatedByRandom(1.4) * 0.35f;
		SoundEngine.PlaySound(SoundID.Dig with {Volume = 0.5f}, Projectile.Center);
		Time = (Time < 60) ? 60 : Time;
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 3; i++)
		{
			Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SoftGlow>(), null, 0, Color.CadetBlue * 0.35f, 6);
		}
		Burst.SpawnBurstDust(ModContent.DustType<InvertedBurst>(), Projectile.Center, 0.25f, Color.CadetBlue, 2.5f, 0.7f, 4f);
		Burst.SpawnBurstDust(ModContent.DustType<Burst>(), Projectile.Center, 0.65f, Color.CadetBlue, 4.5f, 0.75f, 4f);
		Burst.SpawnBurstDust(ModContent.DustType<InvertedBurst>(), Projectile.Center, 0.25f, Color.CadetBlue, 8f, 0.8f, 4f);
		Burst.SpawnBurstDust(ModContent.DustType<Burst>(), Projectile.Center, 0.45f, Color.CadetBlue, 8f, 0.8f, 4f);
		foreach (var npc in Main.npc.Where(npc => npc.active && npc.DistanceSQ(Projectile.Center) < 16000))
		{
			npc.SimpleStrikeNPC(Projectile.damage, (npc.Center.X > Projectile.Center.X) ? 1 : -1, false, 1f, DamageClass.Ranged);
		}
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.WriteVector2(WorldTarget);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		WorldTarget = reader.ReadVector2();
	}
}