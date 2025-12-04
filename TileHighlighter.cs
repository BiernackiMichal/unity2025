using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TileHighlighter : MonoBehaviour
{
    public LayerMask tileLayerMask;
    public Camera targetCamera;
    public BuildingPlacer placer;

    private List<TileHighlight> highlightedTiles = new List<TileHighlight>();

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        if (targetCamera == null || placer == null)
            return;
        if (placer.tileGrid == null)
            return;
        if (!placer.isPlacing)
            return;
        if (Mouse.current == null)
            return;

        // Raycast na mysz
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = targetCamera.ScreenPointToRay(mousePos);

        Tile tile = null;
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, tileLayerMask))
            tile = hit.collider.GetComponent<Tile>();

        ClearPreviousHighlights();

        if (tile != null)
        {
            Vector2Int size = placer.blueprintSize;
            HighlightArea(tile.gridPosition, size);
        }
    }

    void HighlightArea(Vector2Int startPos, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int gx = startPos.x + x;
                int gy = startPos.y + y;

                if (gx >= 0 && gx < placer.tileGrid.GetLength(0) &&
                    gy >= 0 && gy < placer.tileGrid.GetLength(1))
                {
                    Tile tile = placer.tileGrid[gx, gy];
                    TileHighlight th = tile.GetComponent<TileHighlight>();

                    if (th != null)
                    {
                        bool canBuild = placer.IsAreaFree(startPos, size);

                        th.SetHighlight(true, canBuild);
                        highlightedTiles.Add(th);
                    }
                }
            }
        }
    }

    void ClearPreviousHighlights()
    {
        foreach (var th in highlightedTiles)
            th.SetHighlight(false, true);

        highlightedTiles.Clear();
    }
}
