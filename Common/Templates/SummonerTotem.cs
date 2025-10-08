using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common.Utils;
using ReLogic.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Pantheon.Common.Templates;

public abstract class SummonerTotem : ModProjectile
{
	
	public virtual int Lifetime => 600;
	public virtual int WhipCooldown => 60;
	public virtual int BuffToGive => -1;
	public virtual int DebuffToGive => -1;

	public virtual Color OutlineColor => Color.White;
	public virtual float BuffRange => 800f;
	public float gravity = 0.3f;
	
	public override void SetDefaults()
	{
		Projectile.shouldFallThrough = false;
		Projectile.minion = true;
		Projectile.minionSlots = 0;
		Projectile.penetrate = -1;
		Projectile.damage = 0;
		Projectile.knockBack = 0;
		Projectile.friendly = false;
		Projectile.tileCollide = true;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = Lifetime;
		
		base.SetDefaults();
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.timeLeft = (int)(Lifetime * (Projectile.owner != 255
			? (1 + Main.player[Projectile.owner].TotemPlayer().TotemDurationIncrease)
			: 1));
		base.OnSpawn(source);
	}

	public override void AI()
	{
		Projectile.ai[0]++;
		// if (Projectile.ai[0] % 20 == 0)
		// {
		// 	float r = 0;
		// 	int g = 180;
		// 	for (int i = 0; i < g; i++)
		// 	{
		// 		if (Main.rand.NextBool(4))
		// 		{
		// 				Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(BuffRange, 0).RotatedBy(r * 2),
  //                   		DustID.TintableDust, Vector2.Zero);
  //                   	d.noGravity = true;
  //                   	d.color = OutlineColor;
  //                   	r += MathF.PI * 2 / (float)g;
		// 		}
	 //
		// 	}
		// }

		if (BuffToGive != -1)
		{
			foreach (Player player in Main.player.Where(player => player.active))
			{
				if (player.Center.Distance(Projectile.Center) < BuffRange)
				{
					player.AddBuff(BuffToGive, 1);
				}
			}
		}
		
		if (DebuffToGive != -1)
		{
			foreach (NPC npc in Main.npc.Where(npc => npc.active && !npc.friendly))
			{
				if (npc.Center.Distance(Projectile.Center) < BuffRange)
				{
					npc.AddBuff(DebuffToGive, 1);
				}
			}
		}
		Projectile.velocity.Y = MathF.Min(Projectile.velocity.Y + gravity, 9);
		base.AI();
	}

	public void PreWhipped(Projectile whip)
	{
		if (Projectile.ai[0] > WhipCooldown) OnWhipped(whip);

		Projectile.ai[0] = 0;

	}

	public virtual void OnWhipped(Projectile whip)
	{
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.velocity.X *= 0.5f;
		return false;
	}

	public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
	{
		fallThrough = false;
		return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
	}

	public override void PostDraw(Color lightColor)
	{
		string txt = (Projectile.timeLeft / 60f).ToString("0.0");
		Vector2 size = FontAssets.MouseText.Value.MeasureString(txt);
		Color color = Color.Lerp(Color.White, Color.Red, Easing.InExpo((Lifetime - Projectile.timeLeft) / (float)Lifetime));
		Main.spriteBatch.DrawString(FontAssets.ItemStack.Value, txt, Projectile.Center + new Vector2(-size.X / 2, Projectile.Size.Y * 1.25f) - Main.screenPosition, color);
		DrawEffectAreaIndicatorRing(Projectile.Center, true, OutlineColor, BuffRange, (float)Projectile.timeLeft / Lifetime);

	}

	public static void DrawEffectAreaIndicatorRing(Vector2 center, bool endSpritebatch, Color color, float range, float progress)
	{
		if (endSpritebatch) Main.spriteBatch.End();
		center -= Main.screenPosition;
		range *= 2;
		
		Shaders.Burst.Value.Parameters["Color"].SetValue(color.ToVector4() * 0.05f * (MathF.Tanh(10 * MathF.Sin(MathF.PI * progress))));
		Shaders.Burst.Value.Parameters["Intensity"].SetValue(50f);
		Shaders.Burst.Value.Parameters["TotalTime"].SetValue(1);
		Shaders.Burst.Value.Parameters["uTime"].SetValue(0);

		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
			Main.Rasterizer, Shaders.Burst.Value, Main.GameViewMatrix.TransformationMatrix);
		Lines.Rectangle(new Rectangle(
			(int)(center.X - range / 2), (int)(center.Y - range / 2),
			(int)range, (int)range), Color.White * 0f);
		Main.spriteBatch.End();

		if (endSpritebatch)	Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
			Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

	}
}

public abstract class TotemItem : ModItem
{

	public virtual string TotemName => "Totem";
	public virtual string Buff => "Totem";
	public virtual string Attack => "Totem";


	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TotemName, Attack, Buff);
	public virtual int CooldownInSeconds => 5;
	public override void SetDefaults()
	{
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.useStyle = 1;
		Item.useTime = 40;
		Item.useAnimation = 40;
		Item.DamageType = DamageClass.Summon;
		base.SetDefaults();
	}

	public override bool? UseItem(Player player)
	{
		foreach (var proj in Main.projectile.Where(proj => proj.active && proj.type == Item.shoot && proj.owner == player.whoAmI))
		{
			proj.Kill();
		}
		
		if (player.whoAmI == Main.myPlayer)
		{
			player.ItemCooldownPlayer().SetCooldown(Item.type, 60 * CooldownInSeconds);
		}
		return base.UseItem(player);
	}
}