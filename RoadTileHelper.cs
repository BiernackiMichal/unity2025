using UnityEngine;

public static class RoadTileHelper
{
    // Zwraca prefab i rotację w zależności od sąsiednich kafelków drogi
    public static void GetRoadTileInfo(Tile[,] grid, Tile tile, 
        out GameObject prefab, out Quaternion rotation,
        GameObject straightPrefab, GameObject cornerPrefab,
        GameObject tPrefab, GameObject crossPrefab, GameObject endPrefab)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        Vector2Int pos = tile.gridPosition;

        bool up = (pos.y + 1 < height) && grid[pos.x, pos.y + 1].type == TileType.Road;
        bool down = (pos.y - 1 >= 0) && grid[pos.x, pos.y - 1].type == TileType.Road;
        bool left = (pos.x - 1 >= 0) && grid[pos.x - 1, pos.y].type == TileType.Road;
        bool right = (pos.x + 1 < width) && grid[pos.x + 1, pos.y].type == TileType.Road;

        int connections = (up ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);

        prefab = straightPrefab;
        rotation = Quaternion.identity;

        switch (connections)
        {
            case 1: // End
                prefab = endPrefab;
                if (up) rotation = Quaternion.Euler(0, 180, 0);
                else if (down) rotation = Quaternion.identity;
                else if (left) rotation = Quaternion.Euler(0, 90, 0);
                else if (right) rotation = Quaternion.Euler(0, -90, 0);
                break;

            case 2: // Straight or Corner
                if ((up && down) || (left && right))
                {
                    prefab = straightPrefab;
                    if (left && right) rotation = Quaternion.Euler(0, 90, 0);
                }
                else
                {
                    prefab = cornerPrefab;
                    if (up && right) rotation = Quaternion.Euler(0, 0, 0);
                    else if (right && down) rotation = Quaternion.Euler(0, 90, 0);
                    else if (down && left) rotation = Quaternion.Euler(0, 180, 0);
                    else if (left && up) rotation = Quaternion.Euler(0, -90, 0);
                }
                break;

            case 3: // T-Junction
                prefab = tPrefab;
                if (!up) rotation = Quaternion.Euler(0, 0, 0);
                else if (!right) rotation = Quaternion.Euler(0, 90, 0);
                else if (!down) rotation = Quaternion.Euler(0, 180, 0);
                else if (!left) rotation = Quaternion.Euler(0, -90, 0);
                break;

            case 4: // Cross
                prefab = crossPrefab;
                rotation = Quaternion.identity;
                break;
        }
    }
}
