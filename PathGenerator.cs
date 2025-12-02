using UnityEngine;

public static class PathGenerator
{
    public static void GeneratePath(Tile[,] tileGrid)
    {
        int width = tileGrid.GetLength(0);
        int height = tileGrid.GetLength(1);

        int y = Random.Range(0, height);
        for(int x = 0; x < width; x++)
        {
            tileGrid[x,y].type = TileType.Road;

            if(Random.value < 0.3f)
            {
                y += Random.Range(-1,2);
                y = Mathf.Clamp(y,0,height-1);
            }
        }
    }
}
