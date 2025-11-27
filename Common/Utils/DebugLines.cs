using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace Pantheon.Common.Utils;

// completely useless class but i like using it for debugging stuff quickly
public static class DebugLines
{
    public static void Line(Vector2 start, Vector2 end, Color color, int thickness)
    {
        // this code draws a line 
        var length = start.Distance(end);
        var rotation = start.DirectionTo(end).RotatedBy(MathF.PI).ToRotation();
        start -= Main.screenPosition;
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)start.X, (int)start.Y, (int)length, thickness), null, color, rotation, new(thickness / 2, 0), SpriteEffects.None, 0f);
    }

    public static void Rectangle(Rectangle rect, Color color)
    {
        //how it feels to take 15 adderal and jerk off
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, color);

    }
}