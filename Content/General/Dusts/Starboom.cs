using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common.Graphics;
using Pantheon.Common.Utils;
using Pantheon.Core;
using Terraria;
using Terraria.ModLoader;

namespace Pantheon.Content.General.Dusts;

public class Starboom : ModDust
{
	public override string Texture => null;

	public override void OnSpawn(Dust dust)
	{
		dust.rotation += Main.rand.NextFloat(-1f, 0.9f);
		base.OnSpawn(dust);
	}

	public override bool Update(Dust dust)
	{
		dust.rotation += LerpUtils.RectSin(dust.alpha / 256f) * 0.11f;
		dust.scale *= 1.04f;
		int alphaIncrease = (256 / (dust.customData is int customData ? customData : 25)) + (dust.alpha / 102);
		dust.alpha += (int)(alphaIncrease * 1.05f);
		if (dust.alpha >= 255)
		{
			dust.active = false;
		}
		return false;
	}

	public override bool PreDraw(Dust dust)
	{
		var _t = AssetReferences.Assets.Vfx.star_01.Asset.Value;
		float a = (255 - dust.alpha) / 255f;
		var snap = new SpriteBatchSnapshot(Main.spriteBatch);
		PixelationRenderTarget.AddPixelatedRenderAction(() =>
		{
			Main.spriteBatch.BeginFromSnapshot(snap with {matrix = SpriteBatchUtils.RescaleMatrix(snap.matrix, 0.5f)});
			Main.spriteBatch.Draw(_t, dust.position - Main.screenPosition, null, dust.color with {A = (byte)dust.alpha} * a, dust.rotation, _t.Size() / 2,
            				(new Vector2(16) / _t.Size()) * dust.scale, SpriteEffects.None, 0f);
			Main.spriteBatch.End();
		});
		

		
		return false;
	}
	
}