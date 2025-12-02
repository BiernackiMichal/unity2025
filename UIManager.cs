using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Building Placer")]
    public BuildingPlacer buildingPlacer;

    [Header("UI Panel")]
    public Transform buttonParent;
    public Button buttonPrefab;

    [Header("Buildings")]
    public BuildingData[] buildings;

    private void Start()
    {
        foreach (var b in buildings)
            CreateButton(b);
    }

    private void CreateButton(BuildingData building)
{
    Button btn = Instantiate(buttonPrefab, buttonParent);
    btn.GetComponentInChildren<TextMeshProUGUI>().text =
        $"{building.name} ({building.gold}G, {building.wood}W, {building.stone}S)";

    btn.onClick.AddListener(() => buildingPlacer.StartPlacing(building));
}

}
