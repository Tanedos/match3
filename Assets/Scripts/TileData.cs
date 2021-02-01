using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public bool animationEnded = false;
    public bool dropTiles = false;
    public bool renewTiles = false;
    public bool repeatMatching = false;
    public bool movedX = false;
    public bool movedY = false;
    public bool turnReady = true;
    public bool reverseAnimation = true;
    public GameObject tile1;
    public GameObject tile2;

    void start()
    {

    }
}
