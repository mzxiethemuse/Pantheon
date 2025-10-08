using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Common;
using Pantheon.Common.Templates;
using Pantheon.Common.Utils;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Items.Weapons.Summoner;

public class SunflowerTotemItem : TotemItem
{
	public override int CooldownInSeconds => 5;
	public override string TotemName => "sunflower";
	public override string Attack => "fires petals when struck by a whip";
	public override string Buff => "Happy";

	public override void SetDefaults()
	{
		Item.rare = ItemRarityID.Blue;
		Item.mana = 40;
		Item.damage = 17;
		Item.shoot = ModContent.ProjectileType<SunflowerTotem>();
		Item.shootSpeed = 13f;
		base.SetDefaults();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(1);
		recipe.AddIngredient(ItemID.Sunflower, 5);
		recipe.AddIngredient(ItemID.PlatinumBar, 12);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
		
		recipe = CreateRecipe(1);
		recipe.AddIngredient(ItemID.Sunflower, 5);
		recipe.AddIngredient(ItemID.GoldBar, 12);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}

public class SunflowerTotem : SummonerTotem
{
	
	// sheetPotRect = new Rectangle(52, 4, 32, 20);
	// flowerRect = new Rectangle(34, 8, 16, 16);

	// stem segment = new Rectangle(2, 14, 8, 8);
	// leaf = new Rectangle(2, 2, 19, 11)
	// petal = new Rectangle(22, 2, 10, 22)
	

	public override void SetDefaults()
	{
		base.SetDefaults();
		
		Projectile.Size = new Vector2(32, 64);
	}

	public override int Lifetime => 60 * 15;
	public override float BuffRange => 400f;

	
	public override Color OutlineColor => Color.Goldenrod;
	// private static Asset<Texture2D> sheet = ModContent.Request<Texture2D>("Pantheon/Content/Items/Weapons/Summoner/SunflowerSheet");
	public override int BuffToGive => BuffID.Sunflower;
	public override int WhipCooldown => 30;

	public override bool PreDraw(ref Color lightColor)
	{
		var sheet = TextureAssets.Projectile[Projectile.type];
		Vector2 stemPos = new Vector2(0, 19);
		for (int i = 0; i < 6; i++)
		{
			stemPos.X = MathF.Sin(i + (float)Main.timeForVisualEffects / 35) * 4;
			stemPos.Y -= 6;
			// dude this is some HOT ASSSSS
			if (i == 2 || i == 3)
			{
				float rot = 0f;
				if (i == 2)
				{
					rot = MathF.PI;
				}

				rot += MathF.Sin(i + (float)Main.timeForVisualEffects / 34 + 0.5f) * 0.11f;
				Main.spriteBatch.Draw(sheet.Value, Projectile.Center + stemPos - Main.screenPosition, new Rectangle(2, 2, 18, 10), Lighting.GetColor(
					(Projectile.Center + stemPos).ToTileCoordinates()), rot, new Vector2(0, 0), Vector2.One, SpriteEffects.None, 0f);
			}
			Main.spriteBatch.Draw(sheet.Value, Projectile.Center + stemPos - Main.screenPosition, new Rectangle(2, 14, 8, 8), Lighting.GetColor(
				(Projectile.Center + stemPos).ToTileCoordinates()), 0, new Vector2(4, 4), Vector2.One, SpriteEffects.None, 0f);

		}
		
		float whipCooldownPercent = Easing.OutBack(Easing.InQuad(MathF.Min(Projectile.ai[0] / WhipCooldown, 1f)));

		float r = (float)Main.timeForVisualEffects / 34;
		for (int i = 0; i < 8; i++)
		{
			
			Main.spriteBatch.Draw(sheet.Value,
				Projectile.Center + stemPos + new Vector2(0, whipCooldownPercent - 2.5f).RotatedBy(r) - Main.screenPosition,
				new Rectangle(22, 2, 10, 22), Lighting.GetColor(
					(Projectile.Center + stemPos).ToTileCoordinates()), r, new Vector2(5, 22), Vector2.One * whipCooldownPercent,
				SpriteEffects.None, 0f);
			r += MathF.PI * 2 / 8f;

		}
		
		
		Main.spriteBatch.Draw(sheet.Value, Projectile.Center + stemPos - Main.screenPosition, new Rectangle(34, 8, 16, 16), Lighting.GetColor(
			(Projectile.Center + stemPos).ToTileCoordinates()), 0f, new Vector2(8, 8), Vector2.One, SpriteEffects.None, 0f);


		
		
		Main.spriteBatch.Draw(sheet.Value, Projectile.Center + new Vector2(0, 34) - Main.screenPosition, new Rectangle(52, 4, 32, 20), Lighting.GetColor(
			(Projectile.Center + new Vector2(0, 34)).ToTileCoordinates()), 0, new Vector2(16, 20), Vector2.One, SpriteEffects.None, 0f);
		return false;
	}

	public override void OnWhipped(Projectile whip)
	{
		for (int i = 0; i < 24; i++)
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sunflower);
		}
		
		float r = (float)Main.timeForVisualEffects / 34;
		for (int i = 0; i < 8; i++)
		{
			Projectile.NewProjectile(new EntitySource_Misc("Sunflower Totem"), Projectile.Center + new Vector2(MathF.Sin(i + (float)Main.timeForVisualEffects / 35) * 4, -17) + new Vector2(0, 1).RotatedBy(r),
				new Vector2(0, -10).RotatedBy(r), ModContent.ProjectileType<SunflowerPetal>(), 17, 2f,
				Projectile.owner);
			r += MathF.PI * 2 / 8f;
			
		}
	}

	public override void OnKill(int timeLeft)
	{
		
		for (int i = 0; i < 48; i++)
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sunflower);
		}
		base.OnKill(timeLeft);
	}
}

public class SunflowerPetal : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.Size = new Vector2(10, 18);
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 60;
		
	}

	public override void AI()
	{
		Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI / 2f;
		Projectile.velocity *= 0.96f;
		base.AI();
	}
}