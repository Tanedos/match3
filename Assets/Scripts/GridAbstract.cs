using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAbstract : MonoBehaviour
{
    public Tile[,] grid;
    public int dimension = 8;
    public int numOfTypes;
    public List<int> types = new List<int>();
    public List<Vector2Int> emptyTiles = new List<Vector2Int>();
    public List<List<int>> moves = new List<List<int>>();
    public List<Tile> movedTiles = new List<Tile>();

    public void Init()
    {
        grid = new Tile[dimension, dimension];
        for(int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                List<int> possibleTypes = PreventMatches(col, row, true);
                int type = possibleTypes[Random.Range(0, possibleTypes.Count)];
                grid[col, row] = new Tile(type,col,row);
            }
        }
    }

    public void SwapTiles (Vector2Int a, Vector2Int b)
    {
        Tile t1 = grid[a.x, a.y];
        Tile t2 = grid[b.x, b.y];
        grid[a.x, a.y] = t2;
        grid[a.x, a.y].col = a.x;
        grid[a.x, a.y].row = a.y;
        grid[b.x, b.y] = t1;
        grid[b.x, b.y].col =  b.x;
        grid[b.x, b.y].row = b.y;
    }


    //Checking if the move is legal (results in tiles matching)
    public bool CheckMove(Tile a, Tile b)
    {
        SwapTiles(new Vector2Int(a.col,a.row), new Vector2Int(b.col, b.row));
        List<List<Tile>> lists = new List<List<Tile>>();
        lists.Add(MatchTilesX(a));
        lists.Add(MatchTilesY(a));
        lists.Add(MatchTilesX(b));
        lists.Add(MatchTilesY(b));

        bool legal = false;
        emptyTiles = new List<Vector2Int>();
        foreach (List<Tile> list in lists)
        {
            if (list.Count >= 3)
            {
                foreach (Tile t in list)
                {
                    emptyTiles.Add(new Vector2Int(t.col, t.row));
                }
                MatchTiles(list);
                legal = true;
            }
        }
        if (!legal) SwapTiles(new Vector2Int(a.col, a.row), new Vector2Int(b.col, b.row));
        else
        {
            moves = dropTiles();
            renewTiles();
        }

        return legal;
    }


    //Matched tiles become empty
    private void MatchTiles(List<Tile> tiles)
    {
        foreach (Tile t in tiles)
        {
            t.type = -1;        // -1 means tile is empty

        }
    }

    
    //Descend tiles to the bottom of the grid
    private List<List<int>> dropTiles()
    {
        List<Tile> emptyTilesY = new List<Tile>();
        List<Tile> droppingTiles = new List<Tile>();
        List<List<int>> moves = new List<List<int>>();
        for (int row = dimension - 1; row >= 0; row--)
        {
            for (int col = dimension - 1; col >= 0; col--)
            {
                if(grid[col,row].type == -1)
                {
                    emptyTilesY = emptyTilesAbove(grid[col, row]);
                    int depth = emptyTilesY.Count;
                    if(row - depth >= 0)
                    {
                        List<int> l = new List<int>();
                        l.Add(col);
                        droppingTiles = usedTilesAbove(grid[col, row-depth]);
                        foreach(Tile t in droppingTiles)
                        {
                            l.Add(t.row);
                            l.Add(t.row + depth);

                            int tmp = t.type;
                            t.type = grid[t.col, t.row + depth].type;
                            grid[t.col, t.row + depth].type = tmp;
                            movedTiles.Add(grid[t.col, t.row + depth]);
                        }
                        moves.Add(l);
                    }
                }
            }
        }
        return moves;
    }

    //Fill empty tiles with new types
    public void renewTiles()
    {
        for(int row = 0; row<dimension; row++)
        {
            for(int col = 0; col<dimension; col++)
            {
                if(grid[col,row].type == -1)
                {
                    List<int> possibleTypes = PreventMatches(col, row, false);
                    int newType = possibleTypes[Random.Range(0, possibleTypes.Count)];
                    grid[col,row].type = newType;
                }

            }
        }
    } 

    private void Clear()
    {
        emptyTiles = new List<Vector2Int>();
        moves = new List<List<int>>();
    }

    //Checking for new matches after players' move resulting in a match
    //New matched can only occur after tiles dropping, so we check only 
    //Those tiles (movedTiles list)
    public  List<List<Tile>> NewMatches()
    {
        List<Tile> usedTiles = new List<Tile>();
        List<List<Tile>> allMatches = new List<List<Tile>>();
        foreach (Tile t in movedTiles)
        {
            if (!usedTiles.Contains(t))
            {

                List<Tile> matchX = MatchTilesX(t);
                List<Tile> matchY = MatchTilesY(t);
                if (matchX.Count > 2)
                {
                    usedTiles.AddRange(matchX);
                    allMatches.Add(matchX);
                }
                if (matchY.Count > 2)
                {
                    usedTiles.AddRange(matchY);
                    allMatches.Add(matchY);
                }
            }
        }
        foreach (List<Tile> Tiles2 in allMatches)
        {
            MatchTiles(Tiles2);
        }
        if (allMatches.Count != 0)
        {
            moves = dropTiles();
            renewTiles();
        }
        return allMatches;
    }

    //check the amout of empty tiles in a row above given tile
    private List<Tile> emptyTilesAbove(Tile t)
    {
        List<Tile> tiles = new List<Tile>();
        tiles.Add(t);
        while(t.row - tiles.Count >= 0)
        {
            if (grid[t.col, t.row - tiles.Count].type == -1)
            {
                tiles.Add(grid[t.col, t.row - tiles.Count]);
            }
            else break;
        }
        return tiles;
    }

    //check the amount of filled tiles in a row above given tile
    private List<Tile> usedTilesAbove(Tile t)
    {
        int depth = 0;
        List<Tile> tiles = new List<Tile>();
        while(t.row - depth >= 0)
        {
            if (grid[t.col, t.row - depth].type != -1)
            {
                tiles.Add(grid[t.col, t.row - depth]);
                depth++;
            }
            else break;
        }
        return tiles;
    }

    //Prevents 3 same tiles on initial popularization of the board
    private List<int> PreventMatches(int col, int row, bool init)
    {
        List<int> possibleTypes = new List<int>(types);
        if (col > 1)
        {
            if (grid[col - 2, row].type == grid[col - 1, row].type)
            {
                possibleTypes.Remove(grid[col - 1, row].type);
            }
        }
        if (row > 1)
        {
            if (grid[col, row - 2].type == grid[col, row - 1].type)
            {
                possibleTypes.Remove(grid[col, row - 1].type);
            }
        }
        if (!init)
        {
            if (col < dimension - 2)
            {
                if (grid[col + 2, row].type == grid[col + 1, row].type)
                {
                    possibleTypes.Remove(grid[col + 1, row].type);
                }
            }
            if (row < dimension -2)
            {
                if (grid[col, row + 2].type == grid[col, row + 1].type)
                {
                    possibleTypes.Remove(grid[col, row + 1].type);
                }
            }
        }
        return possibleTypes;
    }

    //vertical matching of tiles
    private List<Tile> MatchTilesY(Tile tile)
    {
        List<Tile> matchedTiles = new List<Tile>();
        matchedTiles.Add(tile);
        int pos = 1;
        while (true)
        {
            if (tile.row + pos != dimension)
            {
                if (tile.type == grid[tile.col, tile.row + pos].type)
                {
                    matchedTiles.Add(grid[tile.col, tile.row + pos]);
                    pos++;
                }
                else break;
            }
            else break;
        }
        pos = -1;
        while (true)
        {
            if (tile.row + pos != -1)
            {

                if (tile.type == grid[tile.col, tile.row + pos].type)
                {
                    matchedTiles.Add(grid[tile.col, tile.row + pos]);
                    pos--;
                }
                else break;
            }
            else break;
        }
        return matchedTiles;
    }

    //horizonal matching of tiles
    private List<Tile> MatchTilesX(Tile tile)
    {
        List<Tile> matchedTiles = new List<Tile>();
        matchedTiles.Add(tile);
        int pos = 1;
        while (true)
        {
            if (tile.col + pos != dimension)
            {

                if (tile.type ==  grid[tile.col + pos, tile.row].type)
                {
                    matchedTiles.Add(grid[tile.col + pos, tile.row]);
                    pos++;
                }
                else break;
            }
            else break;
        }
        pos = -1;
        while (true)
        {
            if (tile.col + pos != -1)
            {
                
                if (tile.type == grid[tile.col + pos, tile.row].type)
                {
                    matchedTiles.Add(grid[tile.col + pos, tile.row]);
                    pos--;
                }
                else break;
            }
            else break;
        }
        return matchedTiles;
    }

    
    void Start()
    {

        Init();
    }
    
    void Update()
    {
        
    }
}
