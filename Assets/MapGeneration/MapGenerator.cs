using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using System;
using System.IO;
using System.Linq;

public class MapGenerator
{
    public int jumpHeight;
    public int jumpLength;
    public int nPlatforms;
    public int maxPlatformSize;
    Random rand;

    public static int minWidth = 1;
    public static int maxWidth = 2;
    public static int initialY = -3;

    public MapGenerator(int _jumpHeight, int _jumpLength, int _nPlatforms, int _maxPlatformSize, Random _rand)
    {
        jumpHeight = _jumpHeight;
        jumpLength = _jumpLength;
        nPlatforms = _nPlatforms;
        maxPlatformSize = _maxPlatformSize;
        rand = _rand;
    }

    public Platforms generate()
    {
        List<Platform> allPlatforms = new List<Platform>();
        Stack<Platform> stack = new Stack<Platform>();
        // Create initial platform
        Platform initialPlatform = Initial();
        allPlatforms.Add(initialPlatform);
        stack.Push(initialPlatform);
        // For each platform, create 0-2 children
        // Stop when the length reaches nPlatforms
        while (allPlatforms.Count < nPlatforms)
        {
            if (stack.Count == 0)
            {
                break;
            }
            Platform top = stack.Pop();
            if (rand.Next(0, 2) == 1)
            {
                Platform leftPlatform = Left(top);
                stack.Push(leftPlatform);
                allPlatforms.Add(leftPlatform);
            }
            if (rand.Next(0, 2) == 1)
            {
                Platform abovePlatform = Above(top);
                stack.Push(abovePlatform);
                allPlatforms.Add(abovePlatform);
            }
        }
        // Mirror everything around y = 0
        List<Platform> mirrorPlatforms = new List<Platform>();
        foreach (Platform platform in allPlatforms)
        {
            mirrorPlatforms.Add(platform.xMirror());
        }
        allPlatforms = allPlatforms.Concat(mirrorPlatforms).ToList();
        // Player 1 spawns on the initial platform
        int p1x = rand.Next(initialPlatform.x, initialPlatform.x + initialPlatform.xSize + 1);
        int p1y = initialY + initialPlatform.ySize + 1;
        // Mirror Player 2's spawn relative to Player 1's
        int p2x = -p1x;
        int p2y = p1y;
        return new Platforms(allPlatforms, p1x, p1y, p2x, p2y);
    }

    public Platform Initial()
    {
        int y = initialY;
        int ySize = rand.Next(minWidth, maxWidth + 1);
        int x = rand.Next(-maxPlatformSize - 1, -2);
        int midGap = jumpLength / 2;
        int xSize = -x + rand.Next(-midGap, 0);
        return new Platform(x, y, xSize, ySize);
    }

    public Platform Above(Platform platform)
    {
        // Generate y values
        int yMin = 2;
        int yMax = jumpHeight;
        int platformTop = platform.y + platform.ySize;
        int y = platformTop + rand.Next(yMin, yMax + 1);
        int ySize = rand.Next(minWidth, Math.Min(maxWidth, y - platformTop));
        //Math.Min(rand.Next(minWidth, maxWidth + 1), y - platform.y);
        // Generate x values
        int xMin = platform.x + 1;
        int xMax = platform.x + platform.xSize;
        int x = rand.Next(xMin, xMax);
        int xSize = rand.Next(2, platform.xSize - x + 1);
        return new Platform(x, y, xSize, ySize);
    }

    public Platform Left(Platform platform)
    {
        // Generate y values
        int yMin = platform.y - jumpHeight;
        int yMax = platform.y + jumpHeight;
        int y = rand.Next(yMin, yMax + 1);
        int ySize = rand.Next(minWidth, maxWidth + 1);
        // Generate x values
        int xSize = rand.Next(2, maxPlatformSize);
        int xRight = rand.Next(1, jumpLength);
        int x = platform.x - xRight - xSize;
        return new Platform(x, y, xSize, ySize);
    }
}