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
    public float groundAcceleration;
    //Accelleration of players in the air
    public float airAcceleration;
    //Maximum self-applied speed from movement on the ground
    public float maxGroundSpeed;
    //Maximum self-applied speed from movement in the air
    public float maxAirSpeed;
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
    //Respawn Location
    public float respawnX;
    public float respawnY;
    //Hit Stun Scalar
    public float hitstunDamageScalar;
    //Player Sprite index (where the sprite is in the folder)
    public int spriteIndex;

    public SerializedPlayer(String _name, KeyCode _jump, KeyCode _left, KeyCode _right, KeyCode _attack, Random rand)
    {
        playerName = _name;
        jumpKey = _jump;
        leftKey = _left;
        rightKey = _right;
        attackKey = _attack;
        stocks = 3;
        maxGroundSpeed = 2f + 8 * (float)rand.NextDouble();
        maxAirSpeed = 2f + 8 * (float)rand.NextDouble();
        groundAcceleration = maxGroundSpeed * (float)rand.NextDouble();
        airAcceleration = maxAirSpeed * (float)rand.NextDouble();
        float totalJumpForce = 5f + 12 * (float)rand.NextDouble();
        float jumpRatio = 0.2f + (float)rand.NextDouble();
        groundJumpForce = totalJumpForce * jumpRatio;
        airJumpForce = totalJumpForce * (1 - jumpRatio);
        mass = 0.5f + 2 * (float)rand.NextDouble();
        drag = 1f + 5 * (float)rand.NextDouble();
        widthScalar = 0.7f + 0.8f * (float)rand.NextDouble();
        heightScalar = 0.5f + (float)rand.NextDouble();
        gravityScalar = 0.3f + (float)rand.NextDouble();
        hitstunDamageScalar = 0.1f + 0.2f * (float)rand.NextDouble();
        respawnX = 0f;
        respawnY = 0f;
        Sprite[] playerSprites = Resources.LoadAll<Sprite>("players");
        spriteIndex = -1;

    }
}