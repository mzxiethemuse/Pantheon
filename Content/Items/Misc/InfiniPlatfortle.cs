using System;
using Microsoft.Xna.Framework;
using Pantheon.Assets;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Items.Misc;

public class InfiniPlatfortleItem : ModItem
{
	public override string Texture => AssetDirectory.Items_Misc + "InfinitePlatfortle";

	public override void SetDefaults()
	{
		Item.Size = new(34, 30);
		Item.useTime = 40;
		Item.useAnimation = 40;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.shoot = ModContent.ProjectileType<InfiniPlatfortle>();
		Item.shootSpeed = 2f;
		Item.maxStack = 1;
		base.SetDefaults();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.Turtle);
		recipe.AddIngredient(ItemID.SoulofLight, 8);
		recipe.AddIngredient(ItemID.ShimmerBlock, 8);

		recipe.Register();
		base.AddRecipes();
	}
	

}


public class InfiniPlatfortle : ModProjectile
{
	public override string Texture => AssetDirectory.Items_Misc + "InfinitePlatfortle";
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
		Projectile.velocity.X = Projectile.velocity.X > 0 ? 3.5f : -3.5f;
		Projectile.velocity.Y = 0;
		base.OnSpawn(source);
	}

	public override void PostAI()
	{
		if (TileID.Sets.Platforms[Framing.GetTileSafely(Projectile.Center).TileType] != true)
		{
			for (int i = 0; i < 2; i++)
			{
				Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ShimmerSpark).velocity *= 0f;
			}
			Projectile.ai[0]++;
			Point16 p = Projectile.Center.ToTileCoordinates16();
			WorldGen.PlaceTile((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), TileID.Platforms,
				(Projectile.timeLeft % 30 == 0), true, -1, 0);
		}
		if (Projectile.ai[0] >= 96) Projectile.Kill();
		Projectile.rotation = Projectile.velocity.X * 0.1f + MathF.Sin(Projectile.spriteDirection * Projectile.timeLeft / 25f) * 0.01f;
		Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;
		Projectile.gfxOffY = -10 + MathF.Sin(Projectile.timeLeft / 25f) * 2;
		base.PostAI();
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
		for (int i = 0; i < 28; i++)
		{
			Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric).velocity *= 0f;
			Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ShimmerSpark).velocity *= 0f;
		}
		base.OnKill(timeLeft);
	}
}