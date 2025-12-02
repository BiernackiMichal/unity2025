using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 50;
    public int height = 50;
    public float tileSize = 1f;

    [Header("Prefabs")]
    public GameObject tilePrefab;

    [Header("Tile Visuals")]
    public GameObject grassPrefab;
    public GameObject waterPrefab;
    public GameObject treePrefab;
    public GameObject roadStraightPrefab;
    public GameObject roadCornerPrefab;
    public GameObject roadTJunctionPrefab;
    public GameObject roadCrossPrefab;
    public GameObject roadEndPrefab;

    [Header("Cluster Probabilities")]
    [Range(0f,1f)] public float waterClusterChance = 0.1f;
    [Range(0f,1f)] public float treeClusterChance = 0.15f;

    [Header("Obstacle Settings (Optional)")]
    [Range(0f,1f)] public float obstacleTreeChance = 0.1f;
    [Range(0f,1f)] public float obstacleWaterChance = 0.05f;

    [HideInInspector] public Tile[,] tileGrid;
    [HideInInspector] public List<Tile> roadTiles = new List<Tile>();

    private System.Random rnd;

    [Header("References")]
    public TileHighlighter tileHighlighter;
    public BuildingPlacer buildingPlacer;

    void Start()
    {
        rnd = new System.Random();
        GenerateMap();

        // Podłączenie tileGrid do Highlighter i Placer
        if (tileHighlighter != null)
            tileHighlighter.placer = buildingPlacer;

        if (buildingPlacer != null)
            buildingPlacer.tileGrid = tileGrid;
    }

    public void GenerateMap()
    {
        tileGrid = new Tile[width, height];
        roadTiles.Clear();

        for(int x=0; x<width; x++)
        {
            for(int y=0; y<height; y++)
            {
                Vector3 pos = new Vector3(x*tileSize,0,y*tileSize);
                GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                tileObj.name = $"Tile {x},{y}";

                Tile tile = tileObj.GetComponent<Tile>();
                tile.gridPosition = new Vector2Int(x,y);
                tile.type = TileType.Grass;
                tile.isOccupied = false;

                tileGrid[x,y] = tile;

                TileVisual visual = tileObj.GetComponent<TileVisual>();
                if(visual != null)
                {
                    visual.tile = tile;
                    visual.grassPrefab = grassPrefab;
                    visual.waterPrefab = waterPrefab;
                    visual.treePrefab = treePrefab;
                    visual.roadStraightPrefab = roadStraightPrefab;
                    visual.roadCornerPrefab = roadCornerPrefab;
                    visual.roadTJunctionPrefab = roadTJunctionPrefab;
                    visual.roadCrossPrefab = roadCrossPrefab;
                    visual.roadEndPrefab = roadEndPrefab;
                }
            }
        }

        GenerateRoad();
        GenerateTerrainClusters();
        ObstacleGenerator.GenerateObstacles(tileGrid, obstacleTreeChance, obstacleWaterChance);

        for(int x=0; x<width; x++)
            for(int y=0; y<height; y++)
                tileGrid[x,y].GetComponent<TileVisual>()?.UpdateTileVisual(tileGrid);

        Debug.Log("MAP GENERATED ✔");
    }

    #region Road Generation
    private void GenerateRoad()
    {
        HashSet<Vector2Int> used = new HashSet<Vector2Int>();
        int x = 0;
        int y = rnd.Next(1, height-1);
        Vector2Int current = new Vector2Int(x,y);

        while(x < width-1)
        {
            if(!used.Contains(current))
            {
                Tile t = tileGrid[current.x,current.y];
                t.type = TileType.Road;
                roadTiles.Add(t);
                used.Add(current);
            }

            int verticalDir = rnd.Next(-1,2);
            int maxHorizontal = Mathf.Min(5,width-1-current.x);
            int horizontalLength = rnd.Next(1,maxHorizontal+1);

            for(int i=0;i<horizontalLength;i++)
            {
                current.x++;
                if(current.x >= width) break;
                if(!used.Contains(current))
                {
                    Tile t = tileGrid[current.x,current.y];
                    t.type = TileType.Road;
                    roadTiles.Add(t);
                    used.Add(current);
                }
            }

            if(verticalDir != 0)
            {
                int maxVertical = (verticalDir>0) ? height-2-current.y : current.y-1;
                if(maxVertical>0)
                {
                    int verticalLength = rnd.Next(1,maxVertical+1);
                    for(int i=0;i<verticalLength;i++)
                    {
                        current.y += verticalDir;
                        if(current.y<1 || current.y>=height-1) break;
                        if(used.Contains(current)) break;

                        Tile t = tileGrid[current.x,current.y];
                        t.type = TileType.Road;
                        roadTiles.Add(t);
                        used.Add(current);
                    }
                }
            }

            x = current.x;
        }
    }
    #endregion

    #region Terrain Clusters
    private void GenerateTerrainClusters()
    {
        bool[,] visited = new bool[width,height];
        int totalAttempts = Mathf.CeilToInt(width*height*0.05f);

        for(int i=0;i<totalAttempts;i++)
        {
            int startX = rnd.Next(1,width-1);
            int startY = rnd.Next(1,height-1);
            if(visited[startX,startY]) continue;

            float roll = (float)rnd.NextDouble();
            TileType type = TileType.Grass;

            if(roll < waterClusterChance)
                type = TileType.Water;
            else if(roll < waterClusterChance+treeClusterChance)
                type = TileType.Tree;
            else
                continue;

            int clusterSize = rnd.Next(3,8);
            FloodFillCluster(startX,startY,clusterSize,type,visited);
        }
    }

    private int FloodFillCluster(int x,int y,int remaining,TileType type,bool[,] visited)
    {
        if(remaining<=0 || x<0 || x>=width || y<0 || y>=height) return 0;
        if(visited[x,y]) return 0;

        Tile tile = tileGrid[x,y];
        if(tile.type == TileType.Road) return 0;

        tile.type = type;
        visited[x,y] = true;
        remaining--;

        int placed = 1;

        Vector2Int[] dirs = new Vector2Int[]
        {
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
            new Vector2Int(1,0),
            new Vector2Int(-1,0)
        };

        for(int i=dirs.Length-1;i>0;i--)
        {
            int j = rnd.Next(0,i+1);
            Vector2Int tmp = dirs[i];
            dirs[i]=dirs[j];
            dirs[j]=tmp;
        }

        foreach(var dir in dirs)
            placed += FloodFillCluster(x+dir.x,y+dir.y,remaining,type,visited);

        return placed;
    }
    #endregion
}
