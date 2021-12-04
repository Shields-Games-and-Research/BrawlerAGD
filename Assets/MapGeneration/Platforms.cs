using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Platforms
{
    public List<Platform> platformList;

    public Platforms(List<Platform> l)
    {
        platformList = l;
    }
}
