using System;
using Pantheon.Assets;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.General.Items;

public class PlatfortleItem : ModItem
{
	public override string Texture => AssetDirectory.Items_Misc + "Platfortle";

	
	public override void SetDefaults()
	{
		Item.Size = new(34, 30);
		Item.useTime = 40;
		Item.useAnimation = 40;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.shoot = ModContent.ProjectileType<Platfortle>();
		Item.shootSpeed = 2f;
		Item.consumable = true;
		Item.maxStack = 99;
		base.SetDefaults();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.WoodPlatform, 128);
		recipe.AddIngredient(ItemID.Wood, 10);
		recipe.AddRecipeGroup(RecipeGroupID.IronBar, 1);
		recipe.Register();
		recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.Turtle);
		recipe.AddIngredient(ItemID.Wood, 10);
		recipe.Register();
		base.AddRecipes();
	}
}


public class Platfortle : ModProjectile
{
	public override string Texture => AssetDirectory.Items_Misc + "Platfortle";
	public override void SetDefaults()
	{
		Projectile.Size = new(34, 30);
		Projectile.aiStyle = -1;
		// 121 platforms, 8 seconds, vel 4
		Projectile.timeLeft = 60 * 14;
		base.SetDefaults();
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.velocity.X = Projectile.velocity.X > 0 ? 4f : -4f;
		Projectile.velocity.Y = 0;
		base.OnSpawn(source);
	}

	public override void PostAI()
	{
		if (Main.rand.NextBool(16)) Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);

		
		if (TileID.Sets.Platforms[Framing.GetTileSafely(Projectile.Center).TileType] != true)
		{
			for (int i = 0; i < 4; i++)
			{
				Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke).velocity *= 0f;
			}
			Projectile.ai[0]++;
			Point16 p = Projectile.Center.ToTileCoordinates16();
			WorldGen.PlaceTile((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), TileID.Platforms,
				(Projectile.timeLeft % 30 == 0), true);
		}
		if (Projectile.ai[0] >= 128) Projectile.Kill();
		Projectile.rotation = Projectile.velocity.X * 0.1f + MathF.Sin(Projectile.spriteDirection * Projectile.timeLeft / 25f) * 0.01f;
		Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;
		Projectile.gfxOffY = -4 + MathF.Sin(Projectile.timeLeft / 25f) * 2;
		base.PostAI();
	}
	
	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
		for (int i = 0; i < 28; i++)
		{
			Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Flare).velocity *= 0.5f;
			Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke).velocity *= 0.5f;
		}
		base.OnKill(timeLeft);
	}
}