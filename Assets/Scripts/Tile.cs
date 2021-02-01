using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int type;
    public int col;
    public int row;

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public bool Equal (Tile t)
    {
        return this.type == t.type;
    }

    public Tile (int t, int x, int y)
    {
        this.type = t;
        this.col = x;
        this.row = y;
    }
}
