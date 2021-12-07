using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

[Serializable]
public class Platforms
{
    public List<Platform> platformList;

    public Platforms(List<Platform> l)
    {
        platformList = l;
    }

    // Single point crossover on the list of platforms
    // Results may not be compatible
    public static Platforms singlePointCrossover(Platforms p1, Platforms p2, Random rand)
    {
        List<Platform> pList1 = p1.platformList;
        List<Platform> pList2 = p2.platformList;
        int minLength = Math.Min(pList1.Count, pList2.Count);
        int point = rand.Next(minLength);
        List<Platform> l1 = pList1.GetRange(0, point);
        List<Platform> l2 = pList2.GetRange(point, pList2.Count - point);
        return new Platforms(l1.Concat(l2).ToList());
    }

    // Mutation is re-generate the platforms from scratch
    public void mutate(Random rand)
    {
        MapGenerator mapGen = new MapGenerator(2, 2, 3, 6, rand);
        Platforms p = mapGen.generate();
        this.platformList = p.platformList;
    }
}
