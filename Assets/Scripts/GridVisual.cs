using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GridVisual : MonoBehaviour
{
    public GameObject[,] grid;
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public GridAbstract brain;
    public int dimension;
    public List<Sprite> sprites;
    public float spacing;
    public GameObject TilePrefab;
    public Dictionary<Vector2Int, Vector2Int> positions = new Dictionary<Vector2Int, Vector2Int>();

    


    void SpawnGrid()
    {
        grid = new GameObject[dimension, dimension];
        Vector3 offset = transform.position - new Vector3(dimension * spacing / 2.0f, dimension * spacing / 2.0f, 0);
        for(int row = 0; row<dimension; row++)
        {
            for(int col = 0; col<dimension; col++)
            {
                GameObject tile = Instantiate(TilePrefab);
                if (brain != null)
                {
                    tile.GetComponent<SpriteRenderer>().sprite = 
                        sprites[brain.grid[col, row].type];
                }
                else
                {
                    tile.GetComponent<SpriteRenderer>().sprite =
                        sprites[Random.Range(0, sprites.Count)];
                }
                tile.transform.parent = transform;
                tile.transform.position = new Vector3(col * spacing, (dimension-row) * spacing, 0)+offset;
                grid[col, row] = tile;
                tile.GetComponent<TileVisual>().position = new Vector2Int(col, row);
                tile.GetComponent<TileVisual>().grid = this ;
                positions.Add(new Vector2Int(col, row), new Vector2Int((int)tile.transform.position.x,(int) tile.transform.position.y));
            }
        }
    }

    public void SwapTiles(TileVisual a, TileVisual b)
    {
        GameObject tmp = grid[a.position.x, a.position.y];
        grid[a.position.x, a.position.y] = grid[b.position.x, b.position.y];
        grid[b.position.x, b.position.y] = tmp;
    }

    public void SwapTiles(Vector2Int a, Vector2Int b)
    {
        GameObject tmp = grid[a.x, a.y];
        grid[a.x, a.y] = grid[b.x, b.y];
        grid[b.x, b.y] = tmp;
    }

    public void IncreaseScore ()
    {
        score++;
        scoreText.text = "Score: " + score.ToString();
    }

    void Start()
    {
        grid = new GameObject[dimension, dimension];
       if(brain = null)
        {
           
        }
        
        SpawnGrid();
    }

    void Update()
    {
        
    }
}
