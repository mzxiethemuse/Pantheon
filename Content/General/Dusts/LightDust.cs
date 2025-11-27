using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Pantheon.Content.General.Dusts;

public class LightDust : ModDust
{
	public override string Texture => null;
	
	public override bool PreDraw(Dust dust) {
		if (dust.fadeIn == 0f) {
		
			Main.spriteBatch.Draw(Textures.VFXLightStar.Value, dust.position - Main.screenPosition, dust.frame, dust.GetAlpha(dust.color), dust.rotation, Textures.VFXLightStar.Size() / 2, dust.scale, SpriteEffects.None, 0f);
		}
		return false;
	}
}