using UnityEngine;

public class TileVisual : MonoBehaviour
{
    public Tile tile;

    [Header("Road Prefabs")]
    public GameObject roadStraightPrefab;
    public GameObject roadCornerPrefab;
    public GameObject roadTJunctionPrefab;
    public GameObject roadCrossPrefab;
    public GameObject roadEndPrefab;

    [Header("Terrain Prefabs")]
    public GameObject grassPrefab;
    public GameObject waterPrefab;
    public GameObject treePrefab;

    public void UpdateTileVisual(Tile[,] tileGrid = null)
    {
        if (tile == null) return;

        GameObject prefabToSpawn = null;
        Quaternion rotation = Quaternion.identity;

        switch (tile.type)
        {
            case TileType.Grass: prefabToSpawn = grassPrefab; break;
            case TileType.Water: prefabToSpawn = waterPrefab; break;
            case TileType.Tree: prefabToSpawn = treePrefab; break;

            case TileType.Road:
                if (tileGrid != null)
                {
                    // Wyliczamy typ drogi i rotację
                    RoadTileHelper.GetRoadTileInfo(tileGrid, tile, out prefabToSpawn, out rotation,
                        roadStraightPrefab, roadCornerPrefab, roadTJunctionPrefab, roadCrossPrefab, roadEndPrefab);
                }
                else
                {
                    prefabToSpawn = roadStraightPrefab; // fallback
                }
                break;
        }

        if (prefabToSpawn != null)
        {
            // Usuń poprzednie dzieci
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            // Utwórz nowy child
            GameObject instance = Instantiate(prefabToSpawn, transform.position, rotation, transform);
            instance.name = tile.type.ToString();
        }
    }
}
