using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common.Templates;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Combat.Weapons.Summoner;

public class OrichalcumTotemItem : TotemItem
{
	public override int CooldownInSeconds => 5;
	public override string TotemName => "orichalcum bloom";
	public override string Attack => "fires petals when struck by a whip";
	public override string Buff => "Happy";
	public override string Texture => AssetDirectory.Placeholder + "GenericItem";


	public override void SetDefaults()
	{
		Item.rare = ItemRarityID.LightRed;
		Item.mana = 40;
		Item.damage = 48;
		Item.shoot = ModContent.ProjectileType<OrichalcumTotem>();
		Item.shootSpeed = 13f;
		base.SetDefaults();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(1);
		recipe.AddIngredient<SunflowerTotemItem>(1);
		recipe.AddIngredient(ItemID.OrichalcumBar, 16);
		recipe.AddTile(TileID.MythrilAnvil);
		recipe.Register();
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return Color.HotPink;
	}
}

public class OrichalcumTotem : SummonerTotem
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

	public override Color OutlineColor => Color.DeepPink;
	public override int Lifetime => 60 * 20;
	public override float BuffRange => 600f;

	// private static Asset<Texture2D> sheet = ModContent.Request<Texture2D>("Pantheon/Content/Items/Weapons/Summoner/SunflowerSheet");
	public override int BuffToGive => BuffID.Sunflower;
	public override int WhipCooldown => 15;

	public override bool PreDraw(ref Color lightColor)
	{
		var sheet = TextureAssets.Projectile[Projectile.type];
		Vector2 stemPos = new Vector2(0, 19);
		for (int i = 0; i < 6; i++)
		{
			stemPos.X = MathF.Sin(2 * i + (float)Main.timeForVisualEffects / 35) * 6;
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
		
		float whipCooldownPercent = (MathF.Min(Projectile.ai[0] / WhipCooldown, 1f));

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
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Orichalcum);
		}
		
		float r = (float)Main.timeForVisualEffects / 34;
		for (int i = 0; i < 8; i++)
		{
			Projectile.NewProjectile(new EntitySource_Misc("Orichalcum Totem"), Projectile.Center + new Vector2(MathF.Sin(i + (float)Main.timeForVisualEffects / 35) * 4, -17) + new Vector2(0, 1).RotatedBy(r),
				new Vector2(0, -5).RotatedBy(r), ProjectileID.FlowerPetal, 48, 2f,
				Projectile.owner);
			r += MathF.PI * 2 / 8f;
			
		}
	}

	public override void OnKill(int timeLeft)
	{
		
		for (int i = 0; i < 48; i++)
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Orichalcum);
		}
		base.OnKill(timeLeft);
	}
}