using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacer : MonoBehaviour
{
    [HideInInspector] public Tile[,] tileGrid;
    public LayerMask tileLayer;
    public ResourceManager resourceManager;

    private GameObject blueprintRoot;
    private GameObject blueprintModel;
    private GameObject buildingPrefab;
    private Vector2Int buildingSize;
    private int costGold, costWood, costStone;

    private Camera cam;
    private bool isPlacing = false;
    private Renderer[] blueprintRenderers;
    private MaterialPropertyBlock mpb;

    private float pulseSpeed = 3f;
    private float pulseAmount = 0.1f;

    public bool IsPlacing => isPlacing;
    public Tile[,] TileGrid => tileGrid;

    private void Start()
    {
        cam = Camera.main;
    }

    public void StartPlacing(BuildingData building)
    {
        if (tileGrid == null || building.prefab == null) return;

        ClearBlueprint();

        buildingPrefab = building.prefab;
        buildingSize = building.size;
        costGold = building.gold;
        costWood = building.wood;
        costStone = building.stone;

        blueprintRoot = new GameObject("BlueprintRoot");
        blueprintRoot.transform.position = Vector3.zero;
        blueprintRoot.transform.rotation = Quaternion.identity;

        blueprintModel = Instantiate(building.prefab, blueprintRoot.transform);
        blueprintModel.transform.localRotation = Quaternion.identity;
        blueprintModel.transform.localScale = Vector3.one;

        Collider col = blueprintModel.GetComponent<Collider>();
        if (col != null) Destroy(col);

        foreach (var script in blueprintModel.GetComponentsInChildren<MonoBehaviour>())
            script.enabled = false;

        blueprintRenderers = blueprintModel.GetComponentsInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();

        float prefabLocalBottom = GetPrefabLocalBottom(blueprintModel);
        blueprintModel.transform.localPosition = new Vector3(0f, -prefabLocalBottom, 0f);

        isPlacing = true;
    }

    private void Update()
    {
        if (!isPlacing) return;

        MoveBlueprint();

        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryPlaceBuilding();

        if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            CancelPlacing();

        if (Keyboard.current.rKey.wasPressedThisFrame)
            RotateBlueprint();
    }

    private void MoveBlueprint()
    {
        if (blueprintRoot == null || tileGrid == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f, tileLayer)) return;

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        bool free = IsAreaFree(tile.gridPosition);
        bool hasResources = resourceManager.CanAfford(costGold, costWood, costStone);

        float centerOffsetX = (buildingSize.x - 1) * 0.5f;
        float centerOffsetZ = (buildingSize.y - 1) * 0.5f;

        float tileTopY = GetTileTop(tile);
        blueprintRoot.transform.position = new Vector3(
            tile.transform.position.x + centerOffsetX,
            tileTopY,
            tile.transform.position.z + centerOffsetZ
        );

        PulseBlueprint(free, hasResources);
    }

    private void PulseBlueprint(bool canBuild, bool hasResources)
    {
        if (blueprintRoot == null || blueprintRenderers == null) return;

        float scaleXZ = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        blueprintRoot.transform.localScale = new Vector3(scaleXZ, 1f, scaleXZ);

        Color baseColor = !canBuild ? Color.red : (!hasResources ? Color.yellow : Color.green);
        float alphaPulse = 0.3f + 0.2f * Mathf.Sin(Time.time * pulseSpeed);
        Color pulseColor = new Color(baseColor.r, baseColor.g, baseColor.b, alphaPulse);

        foreach (Renderer r in blueprintRenderers)
        {
            r.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", pulseColor);
            r.SetPropertyBlock(mpb);
        }
    }

    public bool IsAreaFree(Vector2Int startPos)
    {
        for (int x = 0; x < buildingSize.x; x++)
            for (int y = 0; y < buildingSize.y; y++)
            {
                int gx = startPos.x + x;
                int gy = startPos.y + y;

                if (gx < 0 || gx >= tileGrid.GetLength(0) || gy < 0 || gy >= tileGrid.GetLength(1))
                    return false;

                Tile t = tileGrid[gx, gy];
                if (t.type != TileType.Grass || t.isOccupied) return false;
            }
        return true;
    }

    private void TryPlaceBuilding()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f, tileLayer)) return;

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        if (!IsAreaFree(tile.gridPosition)) return;
        if (!resourceManager.SpendResources(costGold, costWood, costStone))
        {
            Debug.Log("Nie masz wystarczających surowców!");
            return;
        }

        PlaceBuilding(tile.gridPosition);
    }

    private void PlaceBuilding(Vector2Int startPos)
    {
        Tile baseTile = tileGrid[startPos.x, startPos.y];
        float tileTopY = GetTileTop(baseTile);
        float prefabLocalBottom = GetPrefabLocalBottom(buildingPrefab);

        Vector3 placedPos = new Vector3(
            baseTile.transform.position.x + (buildingSize.x - 1) * 0.5f,
            tileTopY - prefabLocalBottom,
            baseTile.transform.position.z + (buildingSize.y - 1) * 0.5f
        );

        Instantiate(buildingPrefab, placedPos, blueprintRoot.transform.rotation);

        for (int x = 0; x < buildingSize.x; x++)
            for (int y = 0; y < buildingSize.y; y++)
                tileGrid[startPos.x + x, startPos.y + y].isOccupied = true;

        ClearBlueprint();
    }

    private float GetTileTop(Tile tile)
    {
        BoxCollider col = tile.GetComponentInChildren<BoxCollider>();
        return col != null ? col.bounds.max.y : tile.transform.position.y;
    }

    private float GetPrefabLocalBottom(GameObject prefab)
    {
        Collider col = prefab.GetComponent<Collider>();
        if (col == null) return 0f;

        if (col is BoxCollider box) return box.center.y - box.size.y * 0.5f;
        if (col is CapsuleCollider capsule) return capsule.center.y - capsule.height * 0.5f;
        if (col is SphereCollider sphere) return sphere.center.y - sphere.radius;
        if (col is MeshCollider mesh)
        {
            Vector3 worldBottom = mesh.bounds.min;
            return prefab.transform.InverseTransformPoint(worldBottom).y;
        }

        return 0f;
    }

    private void RotateBlueprint()
    {
        if (blueprintRoot == null) return;
        blueprintRoot.transform.Rotate(Vector3.up, 90f);
        buildingSize = new Vector2Int(buildingSize.y, buildingSize.x);
    }

    private void CancelPlacing() => ClearBlueprint();

    private void ClearBlueprint()
    {
        if (blueprintRoot != null) Destroy(blueprintRoot);
        blueprintRoot = null;
        blueprintModel = null;
        blueprintRenderers = null;
        isPlacing = false;
    }

    public Vector2Int GetBlueprintSize() => buildingSize;

    public bool CanBuildAtTile(Vector2Int pos)
    {
        if (tileGrid == null) return false;
        if (pos.x < 0 || pos.x >= tileGrid.GetLength(0) || pos.y < 0 || pos.y >= tileGrid.GetLength(1)) return false;   
        Tile t = tileGrid[pos.x, pos.y];
        return t.type == TileType.Grass && !t.isOccupied;
    }
}
