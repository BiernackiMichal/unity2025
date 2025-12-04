using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Grid")]
    public Tile[,] tileGrid;

    [Header("Input / Raycast")]
    public LayerMask tileLayer;

    [Header("Resources")]
    public ResourceManager resourceManager;

    [Header("State (debug)")]
    public bool isPlacing = false;

    private Camera cam;

    // blueprint
    public GameObject currentBlueprintPrefab;  // ← TileHighlighter to potrzebuje
    private GameObject blueprintRoot;
    private GameObject blueprintModel;
    private Renderer[] blueprintRenderers;
    private MaterialPropertyBlock mpb;

    // building data
    private GameObject buildingPrefab;
    public Vector2Int blueprintSize;           // ← TileHighlighter to potrzebuje
    private int costGold, costWood, costStone;

    // placing fields
    private Farm currentFarm;

    // pulse
    private float pulseSpeed = 3f;
    private float pulseAmount = 0.1f;


    public bool IsPlacing => isPlacing;

    private void Start()
    {
        cam = Camera.main;
    }

    // ----------------------------------------------------------
    // START PLACING BUILDINGS
    // ----------------------------------------------------------

    public void StartPlacing(BuildingData data)
    {
        if (tileGrid == null || data.prefab == null) return;

        ClearBlueprint();

        buildingPrefab = data.prefab;
        currentBlueprintPrefab = data.prefab;
        blueprintSize = data.size;

        costGold = data.gold;
        costWood = data.wood;
        costStone = data.stone;

        currentFarm = null;

        CreateBlueprint(currentBlueprintPrefab);
        isPlacing = true;
    }

    public void StartPlacingField(Farm farm)
    {
        if (tileGrid == null || farm == null || farm.fieldPrefab == null) return;

        ClearBlueprint();

        buildingPrefab = farm.fieldPrefab;
        currentBlueprintPrefab = farm.fieldPrefab;
        blueprintSize = farm.fieldSize;

        costGold = 0;
        costWood = 0;
        costStone = 0;

        currentFarm = farm;

        CreateBlueprint(currentBlueprintPrefab);
        isPlacing = true;
    }

    private void CreateBlueprint(GameObject prefab)
    {
        blueprintRoot = new GameObject("BlueprintRoot");

        blueprintModel = Instantiate(prefab, blueprintRoot.transform);
        blueprintModel.transform.localPosition = Vector3.zero;
        blueprintModel.transform.localRotation = Quaternion.identity;
        blueprintModel.transform.localScale = Vector3.one;

        RemoveCollidersAndScripts(blueprintModel);

        blueprintRenderers = blueprintModel.GetComponentsInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();

        float bottom = GetPrefabLocalBottom(blueprintModel);
        blueprintModel.transform.localPosition = new Vector3(0, -bottom, 0);
    }

    private void RemoveCollidersAndScripts(GameObject obj)
    {
        foreach (var c in obj.GetComponentsInChildren<Collider>())
            Destroy(c);

        foreach (var s in obj.GetComponentsInChildren<MonoBehaviour>())
            s.enabled = false;
    }

    // ----------------------------------------------------------
    // UPDATE LOOP
    // ----------------------------------------------------------

    private void Update()
    {
        if (!isPlacing) return;
        if (Mouse.current == null) return;

        MoveBlueprint();

        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryPlaceBuilding();

        if (Mouse.current.rightButton.wasPressedThisFrame ||
            Keyboard.current.escapeKey.wasPressedThisFrame)
            CancelPlacing();

        if (Keyboard.current.rKey.wasPressedThisFrame)
            RotateBlueprint();
    }

    private void MoveBlueprint()
    {
        Vector2 mp = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mp);

        if (!Physics.Raycast(ray, out RaycastHit hit, 200f, tileLayer))
            return;

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        bool free = IsBlueprintAreaFree(tile.gridPosition);
        bool resources = resourceManager.CanAfford(costGold, costWood, costStone);

        float offsetX = (blueprintSize.x - 1) * 0.5f;
        float offsetZ = (blueprintSize.y - 1) * 0.5f;

        float tileTop = GetTileTop(tile);

        blueprintRoot.transform.position = new Vector3(
            tile.transform.position.x + offsetX,
            tileTop,
            tile.transform.position.z + offsetZ
        );

        PulseBlueprint(free, resources);
    }

    private void PulseBlueprint(bool canBuild, bool hasResources)
    {
        float puls = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        blueprintRoot.transform.localScale = new Vector3(puls, 1f, puls);

        Color baseC = !canBuild ? Color.red :
                      (!hasResources ? Color.yellow : Color.green);

        float alpha = 0.3f + 0.2f * Mathf.Sin(Time.time * pulseSpeed);

        Color c = new Color(baseC.r, baseC.g, baseC.b, alpha);

        foreach (var r in blueprintRenderers)
        {
            r.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", c);
            r.SetPropertyBlock(mpb);
        }
    }

    // ----------------------------------------------------------
    // AREA CHECKS (FIELDS INCLUDE DIAGONALS)
    // ----------------------------------------------------------

    public bool IsAreaFree(Vector2Int start, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int gx = start.x + x;
                int gy = start.y + y;

                if (gx < 0 || gy < 0 || gx >= tileGrid.GetLength(0) || gy >= tileGrid.GetLength(1))
                    return false;

                Tile t = tileGrid[gx, gy];

                if (t.isOccupied || t.type != TileType.Grass)
                    return false;
            }
        }

        return true;
    }

    public bool IsBlueprintAreaFree(Vector2Int startPos)
    {
        bool isField = buildingPrefab.GetComponent<Field>() != null;

        if (!IsAreaFree(startPos, blueprintSize))
            return false;

        // field must touch farm (diagonals included)
        if (isField && currentFarm != null)
        {
            if (!CheckFarmAdjacency(startPos))
                return false;
        }

        return true;
    }

    private bool CheckFarmAdjacency(Vector2Int startPos)
    {
        Vector2Int[] offsets =
        {
            new Vector2Int( 1, 0), new Vector2Int(-1, 0),
            new Vector2Int( 0, 1), new Vector2Int( 0,-1),
            new Vector2Int( 1, 1), new Vector2Int(-1, 1),
            new Vector2Int( 1,-1), new Vector2Int(-1,-1)
        };

        for (int x = 0; x < blueprintSize.x; x++)
        {
            for (int y = 0; y < blueprintSize.y; y++)
            {
                int gx = startPos.x + x;
                int gy = startPos.y + y;

                foreach (var off in offsets)
                {
                    int nx = gx + off.x;
                    int ny = gy + off.y;

                    if (nx < 0 || ny < 0 || nx >= tileGrid.GetLength(0) || ny >= tileGrid.GetLength(1))
                        continue;

                    Tile n = tileGrid[nx, ny];
                    FarmTileMarker mark = n.GetComponent<FarmTileMarker>();

                    if (mark != null && mark.farm == currentFarm)
                        return true;
                }
            }
        }

        return false;
    }

    // ----------------------------------------------------------
    // PLACEMENT
    // ----------------------------------------------------------

    private void TryPlaceBuilding()
    {
        Vector2 mp = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mp);

        if (!Physics.Raycast(ray, out RaycastHit hit, 200f, tileLayer))
            return;

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        if (!IsBlueprintAreaFree(tile.gridPosition))
            return;

        bool isFarm = buildingPrefab.GetComponent<Farm>() != null;
        bool isField = buildingPrefab.GetComponent<Field>() != null;

        if (!isFarm && !isField)
        {
            if (!resourceManager.SpendResources(costGold, costWood, costStone))
                return;
        }

        PlaceBuilding(tile.gridPosition);
    }

    private void PlaceBuilding(Vector2Int startPos)
    {
        Tile baseTile = tileGrid[startPos.x, startPos.y];
        float tileTop = GetTileTop(baseTile);
        float bottom = GetPrefabLocalBottom(buildingPrefab);

        Vector3 pos = new Vector3(
            baseTile.transform.position.x + (blueprintSize.x - 1) * 0.5f,
            tileTop - bottom,
            baseTile.transform.position.z + (blueprintSize.y - 1) * 0.5f
        );

        GameObject inst = Instantiate(buildingPrefab, pos, blueprintRoot.transform.rotation);

        for (int x = 0; x < blueprintSize.x; x++)
        {
            for (int y = 0; y < blueprintSize.y; y++)
            {
                Tile t = tileGrid[startPos.x + x, startPos.y + y];
                t.isOccupied = true;

                var farm = inst.GetComponent<Farm>();
                if (farm != null)
                {
                    FarmTileMarker mark = t.gameObject.AddComponent<FarmTileMarker>();
                    mark.farm = farm;
                }
            }
        }

        ClearBlueprint();
    }

    // ----------------------------------------------------------
    // UTILS
    // ----------------------------------------------------------

    private float GetTileTop(Tile tile)
    {
        var col = tile.GetComponentInChildren<Collider>();
        if (col != null) return col.bounds.max.y;
        return tile.transform.position.y;
    }

    private float GetPrefabLocalBottom(GameObject prefab)
    {
        Collider col = prefab.GetComponent<Collider>();
        if (col == null) return 0;

        if (col is BoxCollider b) return b.center.y - b.size.y * 0.5f;
        if (col is CapsuleCollider c) return c.center.y - c.height * 0.5f;
        if (col is SphereCollider s) return s.center.y - s.radius;
        if (col is MeshCollider m)
            return prefab.transform.InverseTransformPoint(m.bounds.min).y;

        return 0;
    }

    private void RotateBlueprint()
    {
        if (blueprintRoot == null) return;

        blueprintRoot.transform.Rotate(Vector3.up, 90f);

        blueprintSize = new Vector2Int(blueprintSize.y, blueprintSize.x);
    }

    private void CancelPlacing()
    {
        ClearBlueprint();
    }

    private void ClearBlueprint()
    {
        if (blueprintRoot != null)
            Destroy(blueprintRoot);

        blueprintRoot = null;
        blueprintModel = null;
        currentBlueprintPrefab = null;

        blueprintRenderers = null;

        isPlacing = false;
        currentFarm = null;
    }
}
