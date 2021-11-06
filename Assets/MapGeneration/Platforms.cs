using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Platforms
{
    public List<Platform> platformList;
    public int player1x;
    public int player1y;
    public int player2x;
    public int player2y;

    public Platforms(List<Platform> l, int p1x, int p1y, int p2x, int p2y)
    {
        platformList = l;
        player1x = p1x;
        player1y = p1y;
        player2x = p2x;
        player2y = p2y;
    }
}
