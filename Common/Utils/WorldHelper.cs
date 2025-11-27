using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.WorldBuilding;

namespace Pantheon.Common.Utils;

public static class WorldHelper
{
    public static void ForEachNeighbor(int i, int j, Action<int, int> action)
    {
        for (int y = -1; y < 1; y++)
        {
            for (int x = -1; x < 1; x++)
            {
                if (i != 0 && j != 0)
                {
                    action(i, j);
                }
            }
        }
    }
    
    public static void ForEachInSurroundingSquare(int i, int j, int halfWidth, int halfHeight, Action<int, int> action)
    {
        for (int y = -halfHeight; y < halfHeight; y++)
        {
            for (int x = -halfWidth; x < halfWidth; x++)
            {
                if (i != 0 && j != 0)
                {
                    action(i, j);
                }
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="xbound"></param>
    /// <param name="minTravel"></param>
    /// <param name="maxTravel"></param>
    /// <param name="tries"></param>
    /// <param name="iDoCrystalMeth">The function to run. Return true on success to end the tries.</param>
    public static void TryAtVariousPointsAlongX(int xbound, int minTravel, int maxTravel, Func<int, bool> iDoCrystalMeth)
    {
        // note to self: we literally never use the return value of the function. wtf man
        for (int x = xbound; x < Main.maxTilesX - xbound; x += WorldGen.genRand.Next(minTravel, maxTravel))
        {
            iDoCrystalMeth.Invoke(x);
        }
    }
    
    public static void TryAtVariousPointsAlongXinBounds(int minX, int maxX, int minTravel, int maxTravel, Func<int, bool> iDoCrystalMeth)
    {
        for (int x = minX; x < maxX; x += WorldGen.genRand.Next(minTravel, maxTravel))
        {

            if (iDoCrystalMeth.Invoke(x)) return;
            
        }
    }

    public static Point FindFirstInstanceOfTile(int type)
    {
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                var point = new Point(x, y);
                if (Framing.GetTileSafely(point).TileType == type)
                {
                    return point;
                } 
            }
        }

        return new Point(-1, -1);
    }

    public static Point FindSurfaceTileAtX(int i, ushort type)
    {
        Point o = new Point(-1, -1);
        for (int j = 60; j < Main.worldSurface; j++)
        {
            if (Framing.GetTileSafely(i, j).HasTile)
            {
                o = new Point(i, j);
                break;
            }
        }

        return o;
    }
    
    
}