using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{

    public BoxCollider2D bc;
    public Camera mainOrthoCam;

    /**GENERATOR PARAMETERS
     */
    //determines the overflow of the arena from the camera. Parameterize to 0 - 2
    public float arenaCameraBufferX = 1.1f;
    public float arenaCameraBufferY = 1.1f;

    // Start is called before the first frame update
    void Start()
    {
        float arenaHeight = 2f * mainOrthoCam.orthographicSize * arenaCameraBufferY;
        float arenaWidth = arenaHeight * mainOrthoCam.aspect * arenaCameraBufferX;
        bc.size = new Vector2(arenaWidth, arenaHeight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
