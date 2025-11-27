using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Common.Graphics;
using Pantheon.Content.General.Dusts;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Reworks.Visual;

public class LifeCrystalVisuals : GlobalItem
{
	public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.LifeCrystal;

	public override void SetStaticDefaults()
	{
		ItemID.Sets.ItemNoGravity[ItemID.LifeCrystal] = true;
	}

	public override void PostUpdate(Item item) {
		if (Main.timeForVisualEffects % 4 == 0)
		{
			Vector2 dustPos = Main.rand.NextVector2CircularEdge(50, 50);

			Dust.NewDustPerfect(item.Center + dustPos, DustID.TintableDustLighted, dustPos.DirectionTo(Vector2.Zero), 0, new Color(146, 20, 49), 2f);

		}
		
		
		Lighting.AddLight(item.Center, Color.MediumVioletRed.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
	}

	public override bool? UseItem(Item item, Player player)
	{
		if (!player.ItemAnimationJustStarted) return base.UseItem(item, player);
		Burst.SpawnBurstDust(ModContent.DustType<Burst>(), player.Center, 0.5f, new Color(146, 20, 49), 2f, 0.5f, 3f);
		for (int i = 0; i < 48; i++)
		{
			Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
			Dust.NewDustPerfect(player.Center, DustID.TintableDustLighted, velocity, 0, new Color(146, 20, 49), 2f);
			Dust.NewDustPerfect(player.Center + velocity * 16, DustID.TintableDustLighted, (velocity * 4).DirectionTo(Vector2.Zero), 0, new Color(146, 20, 49), 2f);

		}
		return base.UseItem(item, player);
	}


	public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation,
		ref float scale, int whoAmI)
	{
		
		Texture2D texture = TextureAssets.Item[item.type].Value;
		// OutlineRenderTarget.AddAction(new OutlineDrawAction(
		// 	ref TextureAssets.Item[item.type],
		// 	item.Center - new Vector2(0, 2.5f), 
		// 	rotation,
		// 	texture.Bounds,
		// 	SpriteEffects.None,
		// 	new Color(146, 20, 49)
		// 	));
		OutlineRenderTarget.AddAction(new OutlineDrawAction(item, whoAmI, new Color(146, 20, 49)));

		if (!Main.graphics.GraphicsDevice.GetRenderTargets().Contains(ModContent.GetInstance<OutlineRenderTarget>().renderTarget))
		{
			int ghostAmount = 4;
			for (int i = 1; i < (ghostAmount + 1); i++)
			{
				float ghostRotation = MathF.PI * 2 * (i / (float)ghostAmount);
				float opacity = MathF.Sin((float)(Main.timeForVisualEffects * 0.1f + i * 4)) * 0.25f;
				Vector2 offset = new Vector2(0, 8).RotatedBy(ghostRotation + Main.timeForVisualEffects * 0.025f);
				spriteBatch.Draw(
					texture,
					item.Center - Main.screenPosition + offset - new Vector2(0, 2.5f),
					null,
					(lightColor * (0.75f + opacity)) with { A = (byte)(lightColor.A / 4) },
					rotation,
					texture.Size() / 2,
					1.05f,
					SpriteEffects.None,
					0f
				);
			}
		}

		return true;
	}


	public override Color? GetAlpha(Item item, Color lightColor)
	{
		var baseColor = base.GetAlpha(item, lightColor);
		return Color.Lerp(Color.MediumVioletRed * 0.55f * Main.essScale, Color.White, 0.75f);
	}
}