using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Common;
using Pantheon.Common.Graphics;
using Pantheon.Common.Utils;
using Pantheon.Content.General.Dusts;
using Pantheon.Content.General.Dusts.Bursts;
using Pantheon.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Combat.Ranger.Explosives;

// 
public class FlashbangGrenadeItem : ModItem
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
		Item.shootSpeed = 9f;
		Item.shoot = ModContent.ProjectileType<FlashbangGrenade>();
	}

	public override bool? UseItem(Player player)
	{
		player.ItemCooldownPlayer.SetCooldown(Item.type, 12 * 60);
		return null;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type,
		int damage, float knockback)
	{
		var proj = Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(-0.25 * player.direction), type, damage, knockback, player.whoAmI);
		if (proj.ModProjectile is FlashbangGrenade grenade)
		{
			grenade.WorldTarget = Main.MouseWorld;
		}
		return false;
	}
}

public class FlashbangGrenade : ModProjectile
{

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
		Projectile.timeLeft = 60;
		Projectile.width = AssetReferences.Content.Combat.Ranger.Explosives.FlashbangGrenade.Asset.Width();
		Projectile.height = AssetReferences.Content.Combat.Ranger.Explosives.FlashbangGrenade.Asset.Height();

	}

	public override void AI()
	{

		Time++;
		Projectile.rotation += Projectile.velocity.X * 0.02f;
		if (Projectile.Distance(WorldTarget) < 100f && Time < 60)
		{
			Time = 60;
		}
		if (Time >= 60)
		{
			//takes 30 frames to do
			Projectile.velocity *= 0.9f;
			
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
			(1 - progress), new Vector2(6, 16));
		
		var asset = AssetReferences.Content.Combat.Ranger.Explosives.FlashbangGrenade.Asset;
		if (Time > 60)
		{
			var snapshot = new SpriteBatchSnapshot(Main.spriteBatch);
			Shaders.SolidColor.Asset.Value.Parameters["ucolor"].SetValue(Color.White.ToVector4());
			Main.spriteBatch.End();
			Main.spriteBatch.BeginFromSnapshot(snapshot with {effect = Shaders.SolidColor.Asset.Value});
			Main.spriteBatch.Draw(asset.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(6, 16), 1f, SpriteEffects.None, 0f);
			Main.spriteBatch.End();
			Main.spriteBatch.BeginFromSnapshot(snapshot);
		}


		var alpha = 1 - ((Time - 60) / 60f);
		var color = lightColor;

		Main.spriteBatch.Draw(asset.Value, Projectile.Center - Main.screenPosition, null, color * (Time > 60 ? alpha : 1f), Projectile.rotation, new Vector2(6, 16), 1f, SpriteEffects.None, 0f);


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
		SoundEngine.PlaySound(Sounds.FlashbangDetonate.Asset, Projectile.Center);
		for (int i = 0; i < 40; i++)
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-4, 4));
		}
		Burst.SpawnBurstDust(ModContent.DustType<Burst>(), Projectile.Center, 0.45f, Color.White, 8f, 0.8f, 0.5f);
		Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Starboom>(), null, 0, Color.White, 8);
		foreach (var npc in Main.npc.Where(npc => npc.active && npc.DistanceSQ(Projectile.Center) < 16000))
		{
			Burst.SpawnBurstDust(ModContent.DustType<Burst>(), npc.Center, 0.25f, Color.White, 2f, 0.2f, 0.5f);
			npc.StrikeNPC(Projectile.damage, 0, -npc.direction, false, false, false);
			npc.AddBuff(BuffID.Confused, 60 * 10, false);
			npc.AddBuff(ModContent.BuffType<FlashedDebuff>(), 60 * 10, false);

		}
		
		base.OnKill(timeLeft);
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

public class FlashedDebuff : ModBuff
{
	public override string Texture => AssetReferences.Assets.Placeholders.GenericDebuff.KEY;
	
}

public class FlashedDebuffNPC : GlobalNPC
{
	public static bool AmIDrawingToAvoidStackOverflowIDidntEvenKnowICouldCauseThat = false;

	public override void Load()
	{
		if (!Main.dedServ)
		{
			// On_Main.DrawNPCs += On_MainOnDrawNPCs;
			ModContent.GetInstance<OutlineRenderTarget>().PreRenderOutlines += OnPreRenderOutlines;
		}
	}

	public override void Unload()
	{
		if (!Main.dedServ)
		{
			ModContent.GetInstance<OutlineRenderTarget>().PreRenderOutlines -= OnPreRenderOutlines;
		}
	}

	private void OnPreRenderOutlines()
	{
		foreach (var npc in Main.npc.Where(npc => npc.active && npc.HasBuff<FlashedDebuff>()))
		{
			OutlineRenderTarget.AddAction(new OutlineDrawAction(npc.whoAmI, Color.White * 0.5f));
		}
	}

	public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		
		if (npc.HasBuff<FlashedDebuff>() && Main.spriteBatch.spriteEffect != Shaders.SolidColor.Asset.Value && !AmIDrawingToAvoidStackOverflowIDidntEvenKnowICouldCauseThat)
		{
			{
				AmIDrawingToAvoidStackOverflowIDidntEvenKnowICouldCauseThat = true;
				int buffIndex = npc.FindBuffIndex(ModContent.BuffType<FlashedDebuff>());
				float amount = Easing.InQuad(npc.buffTime[buffIndex] / 600f);
				
				var snapshot = new SpriteBatchSnapshot(Main.spriteBatch);
				Shaders.SolidColor.Asset.Value.Parameters["ucolor"].SetValue((Color.White * amount).ToVector4());
				Main.spriteBatch.End();
				Main.spriteBatch.BeginFromSnapshot(snapshot with { effect = Shaders.SolidColor.Asset.Value });
				Main.instance.DrawNPC(npc.whoAmI, npc.behindTiles);
				Main.spriteBatch.End();
				Main.spriteBatch.BeginFromSnapshot(snapshot);
				AmIDrawingToAvoidStackOverflowIDidntEvenKnowICouldCauseThat = false;
			}
		}
	}

	public override void DrawEffects(NPC npc, ref Color drawColor)
	{
		if (npc.HasBuff<FlashedDebuff>() && npc.buffTime[npc.FindBuffIndex(ModContent.BuffType<FlashedDebuff>())] % 10 == 0)
		{
			Dust.NewDust(npc.position, npc.width, npc.height, DustID.AncientLight, Main.rand.NextFloat(-1, 1) * 0.25f, Main.rand.NextFloat(-1, 1) * 0.25f);
		}
	}

	public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
	{
		
		if (npc.HasBuff<FlashedDebuff>())
		{
			if (modifiers.DamageType == DamageClass.Ranged)
			{
				modifiers.FinalDamage.Additive += 0.1f;
			}
		}
	}
}