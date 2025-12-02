using UnityEngine;

public enum TileType
{
    Grass,
    Water,
    Tree,
    Road
}

public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public TileType type;
    public bool isOccupied = false;

    public float GetTileHeight()
{
    Renderer rend = GetComponent<Renderer>();
    if (rend != null)
        return rend.bounds.size.y; // wysokość tile w świecie
    return 1f; // fallback
}

}