using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class Platforms
{
    public List<Platform> platformList;

    public Platforms(List<Platform> l)
    {
        platformList = l;
    }

    public static Platforms singlePointCrossover(Platforms p1, Platforms p2, Random rand)
    {
        // TODO
        return p1;
    }

    public void mutate(Random rand)
    {
        //TODO
    }
}
