using System;
using Microsoft.Xna.Framework;
using Terraria;

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
    
    public static int GetWeightedRandom(int[] weights)
    {
        int total = 0;
        foreach (var w in weights)
        {
            total += w;
        }

        int random = Main.rand.Next(0, total);

        int cursor = 0;
        int i = 0;
        foreach (var w in weights)
        {
            cursor += w;
            if (cursor >= random)
            {
                return i;
            }
            i++;
        }

        return -1;
    }
}

public struct ValueWithSource
{
    public object value;
    public string source;
}