using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common.Graphics;
using Pantheon.Core;
using Terraria;
using Terraria.ModLoader;

namespace Pantheon.Content.General.Dusts;

public class SoftGlow : ModDust
{
	public override string Texture => null;

	public override void OnSpawn(Dust dust)
	{
		base.OnSpawn(dust);
	}

	public override bool Update(Dust dust)
	{
		dust.scale *= 0.94f;
		dust.alpha += 256 / 32;
		if (dust.alpha >= 255)
		{
			dust.active = false;
		}
		return false;
	}

	public override bool PreDraw(Dust dust)
	{
		var _t = AssetReferences.Assets.Vfx.circle_05.Asset.Value;
		float a = (255 - dust.alpha) / 255f;
		var snap = new SpriteBatchSnapshot(Main.spriteBatch);
		PixelationRenderTarget.AddPixelatedRenderAction(() =>
		{
			Main.spriteBatch.BeginFromSnapshot(snap with {matrix = SpriteBatchUtils.RescaleMatrix(snap.matrix, 0.5f)});
			Main.spriteBatch.Draw(_t, dust.position - Main.screenPosition, null, dust.color * a, dust.rotation, _t.Size() / 2,
				(new Vector2(32) / _t.Size()) * dust.scale * 2, SpriteEffects.None, 0f);
			Main.spriteBatch.End();
		});
		
		return false;
	}
	
}