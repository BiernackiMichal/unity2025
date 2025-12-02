using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TileHighlighter : MonoBehaviour
{
    public LayerMask tileLayerMask;
    public Camera targetCamera;
    public bool debugRay = false;

    public BuildingPlacer placer;

    private List<TileHighlight> highlightedTiles = new List<TileHighlight>();

    void Start()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (placer == null) Debug.LogWarning("TileHighlighter: placer not assigned!");
    }

    void Update()
    {
        if (targetCamera == null || placer == null || placer.TileGrid == null) return;

        Vector2 mousePos = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;
        Ray ray = targetCamera.ScreenPointToRay(mousePos);

        if (debugRay) Debug.DrawRay(ray.origin, ray.direction * 100f, Color.cyan, 0.1f);

        Tile tile = null;
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, tileLayerMask))
            tile = hit.collider.GetComponent<Tile>();

        ClearPreviousHighlights();

        if (tile != null)
        {
            Vector2Int size = Vector2Int.one;
            if (placer != null && placer.IsPlacing)
                size = placer.GetBlueprintSize();

            HighlightArea(tile.gridPosition, size);
        }
    }

    void HighlightArea(Vector2Int startPos, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                int gx = startPos.x + x;
                int gy = startPos.y + y;
                if (gx >= 0 && gx < placer.TileGrid.GetLength(0) && gy >= 0 && gy < placer.TileGrid.GetLength(1))
                {
                    TileHighlight th = placer.TileGrid[gx, gy].GetComponent<TileHighlight>();
                    if (th != null)
                    {
                        // Sprawdzamy czy **ten konkretny tile** jest wolny
                        bool canBuildHere = placer.CanBuildAtTile(new Vector2Int(gx, gy));
                        th.SetHighlight(true, canBuildHere);
                        highlightedTiles.Add(th);
                    }
                }
            }
    }

    void ClearPreviousHighlights()
    {
        foreach (var th in highlightedTiles)
            th.SetHighlight(false, true); // false + domyślnie zielone
        highlightedTiles.Clear();
    }
}
