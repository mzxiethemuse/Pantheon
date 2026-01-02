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
public class InvertedBurst : Burst
{
    const bool _debug = false;
    public override string Texture => AssetDirectory.Vfx + "Explosion";
    public virtual Asset<Texture2D> RealTexture => OldTextures.VFXExplosion;


    public override float ScaleLerpMod(float value)
    {
        return Easing.OutCirc(1 - value);
    }

    public override float AlphaLerpMod(float value)
    {
        return Easing.InQuad(1 - value);
    }

    public override Vector2 GetScale(float progress, Dust dust)
    {
        return new Vector2(progress * dust.scale, progress * dust.scale);
    }
}