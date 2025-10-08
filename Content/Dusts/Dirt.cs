using System;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Pantheon.Content.Dusts;

public class Dirt : ModDust
{
	public override string Texture => null;

	public override void OnSpawn(Dust dust)
	{
		dust.customData = Main.rand.Next(0, 6);
		base.OnSpawn(dust);
	}

	public override bool Update(Dust dust)
	{
		if (dust.customData != null) dust.customData = 0;
		
		dust.position += dust.velocity;
		dust.velocity *= 0.95f;
		dust.scale *= 0.94f;
		dust.alpha += 256 / 32;
		dust.rotation += dust.velocity.X * 0.05f;
		if (dust.alpha >= 255)
		{
			dust.active = false;
		}
		return false;
	}

	public override bool PreDraw(Dust dust)
	{
		var _t = Textures.VFXSmoke[(int)dust.customData].Value;
		float a = (255 - dust.alpha) / 255f;
		Main.spriteBatch.Draw(_t, dust.position - Main.screenPosition, null, dust.color with {A = (byte)dust.alpha} * 0.6f * a, dust.rotation, _t.Size() / 2,
			(new Vector2(32) / _t.Size()) * dust.scale, SpriteEffects.None, 0f);
		
		return false;
	}
	
}