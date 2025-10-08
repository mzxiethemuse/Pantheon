using System;
using Microsoft.Xna.Framework;

namespace Pantheon.Common.Utils;

//name is misleading.
public static class Utilities
{

    public static Vector2 Abs(this Vector2 v)
    {
        return new Vector2(Math.Abs(v.X), Math.Abs(v.Y));
    }

    public static float AttenuateKindaMaybeIdkWhatToCallThisFunction(float x, float bound)
    {
        return bound * MathF.Tanh(x * (MathF.PI / bound));
    }

    public static string SafeSubString(this string str, int length)
    {
        if (str.Length < length)
        {
            return str.Substring(0, str.Length);

        }
        else
        {
            return str.Substring(0, length);

        }
    }
    
}

public struct ValueWithSource
{
    public object value;
    public string source;
}