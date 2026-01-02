using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common.Graphics;
using Pantheon.Common.Utils;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Pantheon.Content.General.Dusts;

/// <summary>
/// this class is lowkey GAY as FUCK
///  dust.scale informs how big the burst will be at the end, in half-tile intervals (1 scale = 16 pixels)
///  internall uses velocity to keep track of time
/// </summary>
public class Burst : ModDust
{
    const bool _debug = false;
    public virtual bool UseShader => true;
    public override string Texture => null;

    public override void OnSpawn(Dust dust)
    {
        base.OnSpawn(dust);
    }

    public override bool Update(Dust dust)
    {
        dust.velocity.Y += 1;
        if (dust.velocity.Y > dust.velocity.X)
        {
            dust.active = false;
        }
        return false;
    }

    public override bool PreDraw(Terraria.Dust dust)
    {
        
        // burstData.time is the amt of time passed, timeLeft is the amount of time until duration expires
        var timeLeft = dust.velocity.X - dust.velocity.Y;
        var progress = (ScaleLerpMod(dust.velocity.Y / dust.velocity.X));
        var alpha = (AlphaLerpMod(1 - (dust.velocity.Y / dust.velocity.X)));
        // Main.NewText(a + " " + g );
        Vector2 size = GetScale(progress, dust) * 16;

        var FUCK = (dust.position - Main.screenPosition);
        Main.spriteBatch.End();
        // with {A = (byte)alpha}



        if (UseShader)
        {
            OldShaders.Burst.Value.Parameters["Color"].SetValue((dust.GetAlpha(dust.color) * alpha).ToVector4() * 1.1f);
            if (dust.customData is float data)
            {
                OldShaders.Burst.Value.Parameters["Intensity"].SetValue(data);
            }
            else
            {
                OldShaders.Burst.Value.Parameters["Intensity"].SetValue(2f);
            }

            OldShaders.Burst.Value.Parameters["TotalTime"].SetValue(dust.velocity.X);
            OldShaders.Burst.Value.Parameters["uTime"].SetValue(dust.velocity.Y + 1);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
                Main.Rasterizer, OldShaders.Burst.Value, Main.GameViewMatrix.TransformationMatrix);
            DebugLines.Rectangle(new Rectangle((int)(FUCK.X - size.X), (int)(FUCK.Y - size.Y), (int)
                (2 * size.X), (int)(2 * size.Y)), Color.White);
            Main.spriteBatch.End();

            //

        }

        
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
            Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        ExtraDraw(progress, dust);
        
        return false;
    }

    public static void SpawnBurstDust(int type, Vector2 position, float durationInSeconds, Color color, float scale, float alpha, float fadeamt = 2f)
    {
        var d = Dust.NewDustPerfect(position, type, new Vector2(durationInSeconds * 60, 0), (int)(alpha * 255), color, scale);
        d.customData = fadeamt;
    }

    public virtual float ScaleLerpMod(float value)
    {
        return value;
    }

    public virtual float AlphaLerpMod(float value)
    {
        return Easing.InOutCirc(value);
    }

    public virtual Vector2 GetScale(float progress, Dust dust)
    {
        return new Vector2(progress * dust.scale);
    }

    public virtual void ExtraDraw(float progress, Dust dust) {}

    public override Color? GetAlpha(Dust dust, Color lightColor)
    {
        return base.GetAlpha(dust, lightColor);
    }
}