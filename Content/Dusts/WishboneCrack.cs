using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Pantheon.Content.Dusts;

public class WishboneCrack : ModDust
{
	public override string Texture => null;

	public override bool Update(Dust dust)
	{
		if (dust.customData != null) dust.customData = 0;
		
		dust.position += dust.velocity;
		dust.velocity *= 0.95f;
		dust.velocity.Y += 0.7f;
		dust.scale *= 0.94f;
		dust.rotation += dust.velocity.X * 0.05f;
		if (dust.scale < 0.1f)
		{
			dust.active = false;
		}
		return false;
	}

	public override bool PreDraw(Dust dust) {
		if (dust.fadeIn == 0f) {
			Main.spriteBatch.Draw(Textures.Wishbone.Value, dust.position - Main.screenPosition, new Rectangle(0 + 15 * (int)dust.customData, 0, 15, 30), dust.GetAlpha(dust.color), dust.rotation, Textures.Wishbone.Size() / 2, 1f, SpriteEffects.None, 0f);
		}
		return false;
	}
	
}