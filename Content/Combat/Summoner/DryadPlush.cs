using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common.Templates;
using Pantheon.Common.Utils;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Combat.Summoner;

public class DryadPlushItem : TotemItem
{
	public override string Texture => AssetDirectory.Placeholder + "GenericItem";

	public override string TotemName => "dryad plush";
	public override string Attack => "curses nearby enemies";
	public override string Buff => "Dryad's Ward";

	public override int CooldownInSeconds => 5;

	public override void SetDefaults()
	{
		
		Item.rare = ItemRarityID.Green;
		Item.mana = 40;
		Item.shoot = ModContent.ProjectileType<DryadPlush>();
		Item.shootSpeed = 13f;
		base.SetDefaults();
	}


}

public class DryadPlush : SummonerTotem
{
	//head : 0, 0, 24, 20
	// body : 26, 0, 18, 14
	//lim b : 28, 16, 14, 10
	public override void SetDefaults()
	{
		base.SetDefaults();
		Projectile.damage = 0;
		Projectile.Size = new Vector2(26, 24);
	}

	public override Color OutlineColor => Color.Green;
	public override int Lifetime => 60 * 10;
	public override float BuffRange => 400f;

	private static Asset<Texture2D> sheet = ModContent.Request<Texture2D>("Pantheon/Content/Items/Weapons/Summoner/DryadPlush");
	public override int BuffToGive => BuffID.DryadsWard;
	public override int DebuffToGive => BuffID.DryadsWardDebuff;

	public override int WhipCooldown => 30;

	public override bool PreDraw(ref Color lightColor)
	{
		float whipCooldownPercent = 1 - Easing.OutBack(Easing.InQuad(MathF.Min(Projectile.ai[0] / WhipCooldown, 1f)));

		
		float rotationx = Projectile.velocity.X * -0.1f + whipCooldownPercent;
		float limbRot = MathF.PI * 0.5f + whipCooldownPercent;
		float limbRotVelocity = Projectile.velocity.X * 0.1f + whipCooldownPercent;
		float yLimbVelocity = MathF.Max(Projectile.velocity.Y * 0.15f, -0.25f) + whipCooldownPercent;
		//LEGS
		Main.spriteBatch.Draw(sheet.Value, Projectile.Center - Main.screenPosition + new Vector2(1, 5.5f), new Rectangle(28, 16, 14, 10), lightColor, 0 - limbRotVelocity - yLimbVelocity, new Vector2(0, 0), 1f, SpriteEffects.FlipHorizontally, 0f );
		Main.spriteBatch.Draw(sheet.Value, Projectile.Center - Main.screenPosition + new Vector2(-1, 5.5f), new Rectangle(28, 16, 14, 10), lightColor, 0 - limbRotVelocity + yLimbVelocity, new Vector2(14, 0), 1f, SpriteEffects.None, 0f );

		

		//ARMS

		//left
		Main.spriteBatch.Draw(sheet.Value, Projectile.Center - Main.screenPosition + new Vector2(-3, -5.5f), new Rectangle(28, 16, 14, 10), lightColor, limbRot + limbRotVelocity + yLimbVelocity, new Vector2(0, 0), 1f, SpriteEffects.FlipHorizontally, 0f );
		//right
		Main.spriteBatch.Draw(sheet.Value, Projectile.Center - Main.screenPosition + new Vector2(3, -5.5f), new Rectangle(28, 16, 14, 10), lightColor, -limbRot + limbRotVelocity - yLimbVelocity, new Vector2(14, 0), 1f, SpriteEffects.None, 0f );

		
		//HEAD
		Main.spriteBatch.Draw(sheet.Value, Projectile.Center - Main.screenPosition + new Vector2((rotationx * yLimbVelocity) * 10, -16), new Rectangle(0, 0, 24, 20), lightColor, rotationx * yLimbVelocity * 0.5f, new Vector2(12, 10), 1f, SpriteEffects.None, 0f );
 
		//BODY
		Main.spriteBatch.Draw(sheet.Value, Projectile.Center - Main.screenPosition , new Rectangle(26, 0, 18, 14), lightColor, 0, new Vector2(9, 7), 1f, SpriteEffects.None, 0f );

		
		return false;
	}
	
	public override void OnKill(int timeLeft)
	{
		
		for (int i = 0; i < 48; i++)
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DryadsWard);
		}
		base.OnKill(timeLeft);
	}
}