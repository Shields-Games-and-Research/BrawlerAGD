using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Platform
{
    public int x;
    public int y;
    public int xSize;
    public int ySize;

    // 2d Platform
    public static int z = 0;
    public static int zSize = 1;

    public Platform(int av1, int av2, int av3, int av4)
    {
        x = av1;
        y = av2;
        xSize = av3;
        ySize = av4;
    }

    public BoundsInt ToBounds()
    {
        return new BoundsInt(x, y, z, xSize, ySize, zSize);
    }

    public Platform xMirror()
    {
        int newX = -x - xSize;
        return new Platform(newX, y, xSize, ySize);
    }
}
