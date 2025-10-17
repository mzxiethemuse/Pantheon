using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common.Utils;
using ReLogic.Content;
using Terraria;

namespace Pantheon.Content.General.Dusts.Bursts;

/// <summary>
///  dust.scale informs how big the burst will be at the end, in half-tile intervals (1 scale = 16 pixels)
///  internall uses velocity to keep track of time
/// </summary>
public class MagicBurst : Burst
{
    const bool _debug = false;
    public override string Texture => AssetDirectory.Vfx + "Explosion";
    public virtual Asset<Texture2D> RealTexture => Textures.VFXExplosion;


    public float ScaleLerpMod(float value)
    {
        return Easing.OutCirc(value);
    }

    public float AlphaLerpMod(float value)
    {
        return Easing.InQuad(value);
    }

    public virtual Vector2 GetScale(float progress, Dust dust)
    {
        return new Vector2(progress * dust.scale * 1.25f, progress * dust.scale);
    }

    public virtual void ExtraDraw(float progress, Dust dust) {}

    public override Color? GetAlpha(Dust dust, Color lightColor)
    {
        return base.GetAlpha(dust, lightColor);
    }
}