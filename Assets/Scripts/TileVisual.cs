using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVisual : MonoBehaviour
{
    public Vector2Int position;
    private Vector3 offset;
    private Vector3 currentPos;
    private float distance = 50;
    public GridVisual grid;

    public TileData tileData;
    private float destination;
    Vector2Int newPosition;
    
    void Start()
    {
        tileData.animationEnded = false;
        tileData.dropTiles = false;
        tileData.renewTiles = false;
        tileData.repeatMatching = false;
        tileData.movedX = false;
        tileData.movedY = false;
        tileData.turnReady = true;
        tileData.reverseAnimation = true;
    }
    
    //Improper state management
    void Update()
    {
        if (tileData.animationEnded)
        {
            CheckMove();
        }
        else if (tileData.dropTiles)
        {
            dropTiles();
        }
        else if (tileData.renewTiles)
        {
            renewTiles();
        }
        else if (tileData.repeatMatching)
        {
            tileData.repeatMatching = false;
            List<List<Tile>> tiles = grid.brain.NewMatches();
            if (tiles.Count > 0)
            {
                foreach(List<Tile> l in tiles) matchTiles(l);
            }
            else
            {
                tileData.turnReady = true;
            }
        }
    }


    //Detects drag from starting position of tile until we reach distance variable
    //Upon reaching we swap tiles and check if move is valid
    void OnMouseDrag()
    {
        currentPos = Input.mousePosition + new Vector3(offset.x,offset.y,0);
        if (!tileData.movedX && !tileData.movedY && tileData.turnReady)
        {
            if (currentPos.x < tileData.tile1.transform.position.x - distance)
            {
                tileData.movedX = true;
                tileData.tile2 = gameObject.transform.parent.gameObject.GetComponent<GridVisual>().grid[position.x - 1, position.y];
                destination = tileData.tile2.transform.position.x;
                newPosition = new Vector2Int(position.x-1, position.y);
                MoveByX(tileData.tile2, tileData.tile1.transform.position.x);
                MoveByX(tileData.tile1, destination);
                tileData.turnReady = false;
            }
            if (currentPos.x > gameObject.transform.position.x + distance)
            {
                tileData.movedX = true;
                tileData.tile2 = gameObject.transform.parent.gameObject.GetComponent<GridVisual>().grid[position.x + 1, position.y];
                destination = tileData.tile2.transform.position.x;
                newPosition = new Vector2Int(position.x + 1, position.y);
                 MoveByX(tileData.tile2, tileData.tile1.transform.position.x);
                 MoveByX(tileData.tile1, destination);
                tileData.turnReady = false;
            }
            if (currentPos.y < gameObject.transform.position.y - distance)
            {
                tileData.movedY = true;
                tileData.tile2 = gameObject.transform.parent.gameObject.GetComponent<GridVisual>().grid[position.x, position.y+1];
                destination = tileData.tile2.transform.position.y;
                newPosition = new Vector2Int(position.x, position.y + 1);
                 MoveByY(tileData.tile2, tileData.tile1.transform.position.y);
                 MoveByY(tileData.tile1, destination);
                tileData.turnReady = false;
            }
            if (currentPos.y > gameObject.transform.position.y + distance)
            {
                tileData.movedY = true;
                tileData.tile2 = gameObject.transform.parent.gameObject.GetComponent<GridVisual>().grid[position.x, position.y-1];
                destination = tileData.tile2.transform.position.y;
                newPosition = new Vector2Int(position.x, position.y - 1);
                MoveByY(tileData.tile2, tileData.tile1.transform.position.y);
                MoveByY(tileData.tile1, destination);
                tileData.turnReady = false;
            }
        }
    }

    void OnMouseDown()
    {
        if (!tileData.reverseAnimation)
        {
            tileData.reverseAnimation = true;
            tileData.movedX = false;
            tileData.movedY = false;
            destination = 0;
        }
        offset = gameObject.transform.position - new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        tileData.tile1 = gameObject;
    }
    //State management for preventing turns during animations, doesn't work 100% though
    //TODO change to proper state management
    void OnMouseUp()
    {
        if (!tileData.reverseAnimation)
        {
            tileData.reverseAnimation = true;
            tileData.tile2 = null;
            tileData.movedX = false;
            tileData.movedY = false;
            destination = 0;
            newPosition = new Vector2Int(0, 0);
        }
    }


    void MoveByX(GameObject actor, float destination)
    {
        tileData.animationEnded = false;
        LeanTween.moveX(actor, destination, 0.6f).setOnComplete(() =>
        {
            tileData.animationEnded = true;
        }); ;
    }
    void MoveByY(GameObject actor, float destination)
    {
        tileData.animationEnded = false;
        LeanTween.moveY(actor, destination, 0.6f).setOnComplete(() =>
        {
            tileData.animationEnded = true;
        }); ;
    }    


    void SwapTiles(TileVisual a, TileVisual b)
    {
        Vector2Int tmp = a.position;
        a.position = b.position;
        b.position = tmp;
    }

    //Check if the move is valid and gives a 3 or bigger match
    //If so, matched tiles vanish with LeanTween.alpha method
    void CheckMove()
    {

        if (tileData.movedX || tileData.movedY)
        {
            if (tileData.movedX)
            {
                if (!grid.brain.CheckMove(grid.brain.grid[tileData.tile1.GetComponent<TileVisual>().position.x, tileData.tile1.GetComponent<TileVisual>().position.y],
                    grid.brain.grid[tileData.tile1.GetComponent<TileVisual>().newPosition.x, tileData.tile1.GetComponent<TileVisual>().newPosition.y]))
                {

                    float tmp = tileData.tile1.transform.position.x;
                    LeanTween.moveX(tileData.tile1, tileData.tile2.transform.position.x, 0.6f);

                    LeanTween.moveX(tileData.tile2, tmp, 0.6f).setOnComplete(() =>
                    {
                        tileData.reverseAnimation = false;
                        tileData.movedX = false;
                        tileData.movedY = false;
                        tileData.turnReady = true;
                    }); ;
                }
                else
                {
                    tileData.reverseAnimation = false;
                    tileData.movedX = false;
                    tileData.movedY = false;
                    destination = 0;
                    tileData.animationEnded = false;
                    grid.SwapTiles(tileData.tile1.GetComponent<TileVisual>(), tileData.tile2.GetComponent<TileVisual>());
                    SwapTiles(tileData.tile1.GetComponent<TileVisual>(), tileData.tile2.GetComponent<TileVisual>());
                    foreach (Vector2Int v in grid.brain.emptyTiles)
                    {
                        LeanTween.alpha(grid.grid[v.x, v.y], 0.0f, 0.6f).setOnComplete(() =>
                        {
                            grid.grid[v.x, v.y].GetComponent<SpriteRenderer>().sprite = null;
                            tileData.dropTiles = true;
                        }); ;
                        grid.IncreaseScore();

                    }

                }
            }
            else
            {
                if (!grid.brain.CheckMove(grid.brain.grid[tileData.tile1.GetComponent<TileVisual>().position.x, tileData.tile1.GetComponent<TileVisual>().position.y],
                    grid.brain.grid[tileData.tile1.GetComponent<TileVisual>().newPosition.x, tileData.tile1.GetComponent<TileVisual>().newPosition.y]))
                {
                    float tmp = tileData.tile1.transform.position.y;
                    LeanTween.moveY(tileData.tile1, tileData.tile2.transform.position.y, 0.6f);
                    LeanTween.moveY(tileData.tile2, tmp, 0.6f).setOnComplete(() =>
                    {
                        tileData.reverseAnimation = false;
                        tileData.movedX = false;
                        tileData.movedY = false;
                        tileData.turnReady = true;
                    }); ;
                }
                else
                {
                    tileData.reverseAnimation = false;
                    tileData.movedX = false;
                    tileData.movedY = false;
                    destination = 0;
                    tileData.animationEnded = false;
                    grid.SwapTiles(tileData.tile1.GetComponent<TileVisual>(), tileData.tile2.GetComponent<TileVisual>());
                    SwapTiles(tileData.tile1.GetComponent<TileVisual>(), tileData.tile2.GetComponent<TileVisual>());
                    foreach (Vector2Int v in grid.brain.emptyTiles)
                    {
                        LeanTween.alpha(grid.grid[v.x, v.y], 0.0f, 0.6f).setOnComplete(() =>
                        {
                            grid.grid[v.x, v.y].GetComponent<SpriteRenderer>().sprite = null;
                            tileData.dropTiles = true;
                        }); ;
                        grid.IncreaseScore();

                    }
                }
            }
            
            tileData.animationEnded = false;
        }            
    }

    //Remove tiles of streaks 3 and longer
    public void matchTiles(List<Tile> matches)
    {
        tileData.repeatMatching = false;
        foreach (Tile t in matches)
        {
            LeanTween.alpha(grid.grid[t.col, t.row], 0.0f, 0.6f).setOnComplete(() =>
            {
                grid.grid[t.col, t.row].GetComponent<SpriteRenderer>().sprite = null;
                tileData.dropTiles = true; 

            }); ;
            grid.IncreaseScore();

        }
    }

    //Move tiles to the bottom of the grid via series of swaps
    public void dropTiles()
    {
        if (grid.brain.moves.Count > 0)
        {
            foreach (List<int> l in grid.brain.moves)
            {
                int counter = 1;
                while (counter < l.Count)
                {
                    GameObject a = grid.grid[l[0], l[counter]];
                    GameObject b = grid.grid[l[0], l[counter + 1]];

                    if (a.GetComponent<SpriteRenderer>().sprite == null)
                    {
                        a.transform.position = new Vector3(grid.positions[new Vector2Int(l[0], l[counter + 1])].x, grid.positions[new Vector2Int(l[0], l[counter + 1])].y, 0);

                    }
                    else
                    {
                        LeanTween.moveY(a, grid.positions[new Vector2Int(l[0], l[counter + 1])].y, 0.6f).setOnComplete(() =>
                          {
                              tileData.renewTiles = true;

                          }); ;
                    }
                    if (b.GetComponent<SpriteRenderer>().sprite == null)
                    {

                        b.transform.position = new Vector3(grid.positions[new Vector2Int(l[0], l[counter])].x, grid.positions[new Vector2Int(l[0], l[counter])].y, 0);
                    }
                    else
                    {

                        LeanTween.moveY(b, grid.positions[new Vector2Int(l[0], l[counter])].y, 0.6f).setOnComplete(() =>
                         {
                             tileData.renewTiles = true;
                         }); ;
                    }
                    grid.SwapTiles(new Vector2Int(l[0], l[counter]), new Vector2Int(l[0], l[counter + 1]));
                    SwapTiles(a.GetComponent<TileVisual>(), b.GetComponent<TileVisual>());
                    counter += 2;
                }
            }
        }
        else tileData.renewTiles = true;
        tileData.dropTiles = false;
        
    }

    //Give used tiles new sprites
    //Types are chosen by GridAbstract methods
    public void renewTiles()
    {
        for(int row = 0; row<grid.dimension; row++)
        {
            for(int col = 0; col < grid.dimension; col++)
            {
                if (grid.grid[col,row].GetComponent<SpriteRenderer>().sprite == null)
                {
                    int type = grid.brain.grid[col, row].type;
                    grid.grid[col,row].GetComponent<SpriteRenderer>().sprite = grid.sprites[type];
                    LeanTween.alpha(grid.grid[col,row], 1.0f, 0.6f).setOnComplete(() =>
                    {
                        tileData.repeatMatching = true;
                    }); ;
                }
            }
        }
        tileData.renewTiles = false;

    }

}


