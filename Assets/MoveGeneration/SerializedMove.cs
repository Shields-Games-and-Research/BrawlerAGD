using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class SerializedMove
{
    //Move Center (Relative to Player Center)
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
    public float damageGiven;
    //Knockback scalar applied from move hitting
    public float knockbackScalar;
    //Additional Knockback Vector Direction
    public float knockbackModX;
    public float knockbackModY;
    //Base Hitstun duration
    public float hitstunDuration;
    //Move sprite index (where the sprite is in the folder)
    public int spriteIndex;

    public SerializedMove(Random rand)
    {
        float moveDist = 0.8f + 0.7f * (float)rand.NextDouble();
        float moveAngle = (float)(2 * Math.PI * rand.NextDouble());
        moveLocX = moveDist * (float)Math.Cos(moveAngle);
        moveLocY = moveDist * (float)Math.Sin(moveAngle);
        widthScalar = 0.5f + (float)rand.NextDouble();
        heightScalar = 0.5f + (float)rand.NextDouble();
        warmUpDuration = 0.1f + 0.5f * (float)rand.NextDouble();
        executionDuration = 0.1f + 0.3f * (float)rand.NextDouble();
        coolDownDuration = 0.1f + 0.5f * (float)rand.NextDouble();
        damageGiven = 5 + (warmUpDuration + executionDuration + coolDownDuration) * (float)rand.NextDouble() * 10f;
        knockbackScalar = 1f + 15f * (float)rand.NextDouble();
        // TODO: Move direction and move knockback chosen from some arc in the direction
        knockbackModX = -1f + (float)rand.NextDouble();
        knockbackModY = -1f + (float)rand.NextDouble();
        hitstunDuration = (float)rand.NextDouble();
        Sprite[] moveSprites = Resources.LoadAll<Sprite>("moves");
        this.spriteIndex = -1;
    }
}
