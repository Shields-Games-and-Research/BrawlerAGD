﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class SerializedMove
{
    //Move Center (Relative to Player Center)
    public float moveDist;
    public float moveAngle;
    public float moveLocX;
    public float moveLocY;
    //Width Scaling
    public float widthScalar;
    //Height Scaling
    public float heightScalar;
    //Warm-Up State Duration
    public float warmUpDuration;
    //Active State Duration
    public float executionDuration;
    //Cool Down State Duration
    public float coolDownDuration;
    //Damage given from hitting
    public float damageFactor;
    public float damageGiven;
    //Knockback scalar applied from move hitting
    public float knockbackScalar;
    //Additional Knockback Vector Direction
    public float knockbackModX;
    public float knockbackModY;
    //Base Hitstun duration
    public float hitstunDuration;
    //Damage to shieldDurability
    public float shieldDamage = 10.0f;
    
    //Shield Properties
    // Is this move a shield? 0 = false, 1 = true
    public int isShield;
    // Can this shield break? 0 = false, 1 = true
    public bool canBreak;
    // How much damage a shield can take before breaking
    public float damageDurability = 20.0f;
    // The scale when the shield has full size (hasn't taken damage (example from smash bros.))
    public float fullSizeScale = 1f;
    // The scale when the shield has the smallest size (before breaking (example from smash bros.))
    public float smallSizeScale = 0.2f;
    // which shield state can the parry. 0 = disabled, 1 = startup, 2 = mid, 3 = end
    public float parryState = 0;
    // starting tick that a player can parry in, inclusive
    public float parryWindowStart = 0;
    // ending tick that a player can parry in, inclusive
    public float parryWindowEnd = 0;
    // which shield state can the parry. 0 = disabled, 1 = startup, 2 = mid, 3 = end
    public float reflectionState = 0;
    // starting tick that a player can reflect in, inclusive
    public float reflectionWindowStart = 0;
    // ending tick that a player can reflect in, inclusive
    public float reflectionWindowEnd = 0;
    
    //Move sprite index (where the sprite is in the folder)
    public int spriteIndex;
    public static float[,] ranges = {
        {0.8f, 1.5f}, // moveDist
        {0f, (float) 2f * (float) Math.PI }, // moveAngle
        {0.5f, 1.5f}, // widthScale
        {0.5f, 1.5f}, // heightScale
        {0.1f, 0.6f}, // warmUpDuration
        {0.1f, 0.4f}, // executionDuration
        {0.1f, 0.6f}, // coolDownDuration
        {0f, 10f}, // damageFactor
        {1f, 16f}, // knockbackScale
        {0, 1f}, // knockbackModX
        {-1f, 1f}, // knockbackModY
        {0f, 1f} // hitstunDuration
    };

    public SerializedMove(Random rand)
    {
        float[] genome = generateGenome(rand);
        initFromGenome(genome);
        this.SetRandomSprite(rand);
    }

    public SerializedMove(float[] genome, int _spriteIndex)
    {
        initFromGenome(genome);
        spriteIndex = _spriteIndex;
    }

    public void SetRandomSprite(Random rand) 
    {
        Sprite[] moveSprites = Resources.LoadAll<Sprite>("moves");
        this.spriteIndex = rand.Next(moveSprites.Length);
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
        for (int index = 0; index < genome.Length; index++)
        {
            genome[index] = chooseValue(index, rand);
        }

        float moveDist = genome[0];
        float moveAngle = genome[1];
        float knockbackModX = genome[9];
        float knockbackModY = genome[10];

        float moveLocX = moveDist * (float)Math.Cos(moveAngle);
        float moveLocY = moveDist * (float)Math.Sin(moveAngle);

        Vector2 knockbackVector = new Vector2(knockbackModX, knockbackModY);
        Vector2 moveLocVector = new Vector2(moveLocX, moveLocY);

        while (Vector2.Angle(moveLocVector, knockbackVector) >= 135f)
        {
            knockbackVector = Vector2.Lerp(knockbackVector, moveLocVector, 0.05f);
            genome[9] = knockbackVector.x;
            genome[10] = knockbackVector.y;
        }

        return genome;
    }

    public float[] genome()
    {
        return new float[]
        {
            moveDist,
            moveAngle,
            widthScalar,
            heightScalar,
            warmUpDuration,
            executionDuration,
            coolDownDuration,
            damageFactor,
            knockbackScalar,
            knockbackModX,
            knockbackModY,
            hitstunDuration,
        };
    }

    public void initFromGenome(float[] genome)
    {
        moveDist = genome[0];
        moveAngle = genome[1];
        widthScalar = genome[2];
        heightScalar = genome[3];
        warmUpDuration = genome[4];
        executionDuration = genome[5];
        coolDownDuration = genome[6];
        damageFactor = genome[7];
        knockbackScalar = genome[8];
        // TODO: Move direction and move knockback chosen from some arc in the direction
        knockbackModX = genome[9];
        knockbackModY = genome[10];
        hitstunDuration = genome[11];

        // Extra parameters calculated from the genome
        moveLocX = moveDist * (float)Math.Cos(moveAngle);
        moveLocY = moveDist * (float)Math.Sin(moveAngle);

        if (Vector2.Angle(new Vector2(knockbackModX, knockbackModY), new Vector2(moveLocX, moveLocY)) < 45f)
        {
            knockbackModX = -knockbackModX;
        }
        damageGiven = 5f + (warmUpDuration + executionDuration + coolDownDuration) * damageFactor;
    }

    public static SerializedMove singlePointCrossover(SerializedMove m1, SerializedMove m2, Random rand)
    {
        int whichSprite = rand.Next(2);
        int si = 0;
        if (whichSprite == 0)
        {
            si = m1.spriteIndex;
        }
        else
        {
            si = m2.spriteIndex;
        }
        float[] g1 = m1.genome();
        float[] g2 = m2.genome();
        int point = rand.Next(g1.Length);
        float[] g3 = new float[g1.Length];
        // Create a new genome with crossover
        for (int index = 0; index < g3.Length; index++)
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
        return new SerializedMove(g3, si);
    }

    public static SerializedMove randomCrossover(SerializedMove m1, SerializedMove m2, Random rand)
    {
        int whichSprite = rand.Next(2);
        int si = 0;
        if (whichSprite == 0)
        {
            si = m1.spriteIndex;
        }
        else
        {
            si = m2.spriteIndex;
        }
        float[] g1 = m1.genome();
        float[] g2 = m2.genome();
        float[] g3 = new float[g1.Length];
        for (int index = 0; index < g3.Length; index++)
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
        return new SerializedMove(g3, si);
    }

    public void mutate(Random rand)
    {
        float[] genome = this.genome();
        for (int i = 0; i < 5; i++)
        {
            int index = rand.Next(genome.Length);
            float val = chooseValue(index, rand);
            genome[index] = val;
        }
        this.initFromGenome(genome);
        this.SetRandomSprite(rand);
    }
}
