using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class SerializedPlayer
{
    //Name
    public String playerName;
    //Controls
    public KeyCode jumpKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode attackKey;
    //Stocks
    public int stocks;
    //Accelleration of players on the ground
    public float groundAccelerationFactor;
    public float groundAcceleration;
    //Accelleration of players in the air
    public float airAccelerationFactor;
    public float airAcceleration;
    //Maximum self-applied speed from movement on the ground
    public float maxGroundSpeed;
    //Maximum self-applied speed from movement in the air
    public float maxAirSpeed;
    // Total amount of jumping force
    public float totalJumpForce;
    // Ratio between ground and air jump forces
    public float jumpRatio;
    //Ground Jump Force
    public float groundJumpForce;
    //Air Jump Force
    public float airJumpForce;
    //Mass
    public float mass;
    //Linear Drag
    public float drag;
    //Width Scaling
    public float widthScalar;
    //Height Scaling
    public float heightScalar;
    //Gravity Scaling
    public float gravityScalar;
    //Hit Stun Scalar
    public float hitstunDamageScalar;
    //Player Sprite index (where the sprite is in the folder)
    public int spriteIndex;
    public static float[,] ranges = {
        {2f, 10f}, // maxGroundSpeed
        {2f, 10f}, // maxAirSpeed
        {0f, 1f}, // groundAcceleration factor
        {0f, 1f}, // airAcceleration factor
        {1f, 15f}, // groundJump
        {1f, 15f}, // airJump
        {0.5f, 2.5f}, // mass
        {1f, 6f}, // drag
        {0.7f, 1.5f}, // widthScale                                                                  //////
        {0.5f, 1.5f}, // heightScale                                                                 ////// 
        {0.3f, 1.3f}, // gravityScale
        {0.1f, 0.3f}, // hitstunDamageScale
    };

    public SerializedPlayer(String _name, Random rand)
    {
        playerName = _name;
        stocks = 3;
        float[] genome = generateGenome(rand);
        initFromGenome(genome);
        // Choose a random sprite
        this.SetRandomSprite(rand);
    }

    public SerializedPlayer(String _name, float[] genome, int _spriteIndex)
    {
        playerName = _name;
        stocks = 3;
        initFromGenome(genome);
        spriteIndex = _spriteIndex;
    }

    public void SetRandomSprite(Random rand) 
    {
        Sprite[] playerSprites = Resources.LoadAll<Sprite>("players");
        this.spriteIndex = rand.Next(playerSprites.Length);
    }

    public static float chooseValue(int valueIndex, Random rand)
    {
        float rangeMin = ranges[valueIndex, 0];
        float rangeMax = ranges[valueIndex, 1];
        float rangeSize = rangeMax - rangeMin;
        float rangeVal = rangeSize * (float)rand.NextDouble();
        return rangeMin + rangeVal;
    }

    public static float[] generateGenome(Random rand)
    {
        float[] genome = new float[ranges.GetLength(0)];
        for (int index = 0; index < genome.Length; index ++)
        {
            genome[index] = chooseValue(index, rand);
        }
        return genome;
    }

    public float[] genome()
    {
        return new float[]
        {
            maxGroundSpeed,
            maxAirSpeed,
            groundAccelerationFactor,
            airAccelerationFactor,
            groundJumpForce,
            airJumpForce,
            mass,
            drag,
            widthScalar,
            heightScalar,
            gravityScalar,
            hitstunDamageScalar
        };
    }

    public void initFromGenome(float[] genome)
    {
        maxGroundSpeed = genome[0];
        maxAirSpeed = genome[1];
        groundAccelerationFactor = genome[2];
        airAccelerationFactor = genome[3];
        groundJumpForce = genome[4];
        airJumpForce = genome[5];
        mass = genome[6];
        drag = genome[7];
        widthScalar = genome[8];
        heightScalar = genome[9];
        gravityScalar = genome[10];
        hitstunDamageScalar = genome[11];

        // Extra parameters calculated from the genome
        airAcceleration = maxAirSpeed * airAccelerationFactor;
        groundAcceleration = maxGroundSpeed * groundAccelerationFactor;
        //groundJumpForce = totalJumpForce * jumpRatio;
        //airJumpForce = totalJumpForce * (1 - jumpRatio);
    }

    public static SerializedPlayer singlePointCrossover(SerializedPlayer p1, SerializedPlayer p2, Random rand)
    {
        int whichSprite = rand.Next(2);
        int si = 0;
        if (whichSprite == 0)
        {
            si = p1.spriteIndex;
        }
        else
        {
            si = p2.spriteIndex;
        }
        float[] g1 = p1.genome();
        float[] g2 = p2.genome();
        int point = rand.Next(g1.Length);
        float[] g3 = new float[g1.Length];
        // Create a new genome with crossover
        for (int index = 0; index < g3.Length; index ++)
        {
            if (index < point)
            {
                g3[index] = g1[index];
            }
            else
            {
                g3[index] = g2[index];
            }
        }
        return new SerializedPlayer(p1.playerName, g3, si);
    }

    public static SerializedPlayer randomCrossover(SerializedPlayer p1, SerializedPlayer p2, Random rand)
    {
        int whichSprite = rand.Next(2);
        int si = 0;
        if (whichSprite == 0)
        {
            si = p1.spriteIndex;
        }
        else
        {
            si = p2.spriteIndex;
        }
        float[] g1 = p1.genome();
        float[] g2 = p2.genome();
        float[] g3 = new float[g1.Length];
        for (int index = 0; index < g3.Length; index ++)
        {
            int which = rand.Next(2);
            if (which == 0)
            {
                g3[index] = g1[index];
            }
            else
            {
                g3[index] = g2[index];
            }    
        }
        return new SerializedPlayer(p1.playerName, g3, si);
    }

    public void mutate(Random rand)
    {
        float[] genome = this.genome();
        for (int i = 0; i < 5; i ++)
        {
            int index = rand.Next(genome.Length);
            float val = chooseValue(index, rand);
            genome[index] = val;
        }
        this.initFromGenome(genome);
        this.SetRandomSprite(rand);
    }
}