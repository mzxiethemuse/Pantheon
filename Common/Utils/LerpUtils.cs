using System;
using Microsoft.Xna.Framework;

namespace Pantheon.Common.Utils;

/// <summary>
/// A class with various functions to process lerp values (i.e. values that ramp from 0 -> 1). most functions here are based off of sound synthesis. if you wanna know what they do, honestly just look up "Bitwig Grid phase modules"
/// </summary>
public static class LerpUtils
{
    // Q: Why are these extension methods? A: FUck you
    public static LerpValue Clip(this LerpValue value, float amp)
    {
        return value.a + amp;
    }
    
    /// <summary>
    /// only works properly on ramp values. ramp first, then modify
    /// </summary>
    /// <param name="value"></param>
    /// <param name="amp"></param>
    /// <returns></returns>
    public static LerpValue Formant(this LerpValue value, float amp) => ((amp * value.a) + ((1 - value.a) / 2));

    public static LerpValue VShift(this LerpValue value, float shift)
    {
        var b = value.a;
        while (b > 1)
        {
            b--;
        }
        while (b < 0)
        {
            b++;
        }
        return b;
    }

    public static LerpValue Flip(this LerpValue value) => 1 - value.a;

    public static LerpValue Bend(this LerpValue value, float bend) => (MathF.Pow(value.a,MathF.Pow(MathHelper.Clamp(bend,-0.99f,100),2)));
    
    public static LerpValue Window(this LerpValue value) => value.a * MathF.Sin(value.a * MathF.PI);
    
    public static LerpValue RectSin(this LerpValue value, float sync = 1) => MathF.Sin(sync * value.a * MathF.PI);
    
    
}

public class LerpValue
{
    public LerpValue(float a)
    {
        this.a = MathHelper.Clamp(a, 0, 1);
    }
    public float a;
    public static implicit operator float(LerpValue value) => value.a;
    public static implicit operator LerpValue(float value) => new LerpValue(value);
    
}