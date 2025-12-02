using UnityEngine;
using System.Collections.Generic;


public static class ObstacleGenerator
{
    // tileGrid – mapa kafelków
    // treeChance, waterChance – szanse generowania przeszkód
    public static void GenerateObstacles(Tile[,] tileGrid, float treeChance = 0.1f, float waterChance = 0.05f)
    {
        int width = tileGrid.GetLength(0);
        int height = tileGrid.GetLength(1);

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Tile t = tileGrid[x, y];

                // Zmieniamy tylko kafelki Grass
                if(t.type != TileType.Grass) continue;

                float roll = Random.value; // UnityEngine.Random
                if (roll < treeChance)
                    t.type = TileType.Tree;
                else if (roll < treeChance + waterChance)
                    t.type = TileType.Water;
            }
        }
    }
}
