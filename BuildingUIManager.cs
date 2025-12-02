using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BuildingUIManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform civilianPanel;
    public Transform militaryPanel;
    public Button buildingButtonPrefab;

    [Header("Buildings Data")]
    public BuildingData[] civilianBuildings;
    public BuildingData[] militaryBuildings;

    [Header("Placer & Resources")]
    public BuildingPlacer placer;
    public ResourceManager resourceManager;

    private List<(Button button, BuildingData data)> allButtons = new List<(Button, BuildingData)>();

    private void Start()
    {
        PopulatePanel(civilianPanel, civilianBuildings);
        PopulatePanel(militaryPanel, militaryBuildings);

        // Subskrybuj eventy zasobów raz, aktualizacja wszystkich przycisków
        resourceManager.OnGoldChanged += (_) => UpdateAllButtons();
        resourceManager.OnWoodChanged += (_) => UpdateAllButtons();
        resourceManager.OnStoneChanged += (_) => UpdateAllButtons();
        resourceManager.OnFoodChanged += (_) => UpdateAllButtons();
    }

    private void PopulatePanel(Transform panel, BuildingData[] buildings)
    {
        foreach (var b in buildings)
        {
            Button btn = Instantiate(buildingButtonPrefab, panel);

            // Ustaw nazwę
            TMP_Text textComp = btn.GetComponentInChildren<TMP_Text>();
            if (textComp != null) textComp.text = b.name;

            // Ustaw ikonę, jeśli jest
            Image iconComp = btn.GetComponentInChildren<Image>();
            if (iconComp != null && b.icon != null)
                iconComp.sprite = b.icon;

            // Kliknięcie – start budowy
            btn.onClick.AddListener(() => placer.StartPlacing(b));

            // Dodaj do listy do aktualizacji
            allButtons.Add((btn, b));
        }

        // Aktualizacja początkowa
        UpdateAllButtons();
    }

    private void UpdateAllButtons()
    {
        foreach (var (button, data) in allButtons)
        {
            bool canAfford = resourceManager.CanAfford(data.gold, data.wood, data.stone, data.food);
            button.interactable = canAfford;

            ColorBlock cb = button.colors;
            cb.normalColor = canAfford ? Color.white : Color.gray;
            button.colors = cb;
        }
    }

    // Przełączanie paneli kategorii
    public void ShowCivilianPanel(bool show)
    {
        civilianPanel.gameObject.SetActive(show);
        militaryPanel.gameObject.SetActive(!show);
    }

    public void ShowMilitaryPanel(bool show)
    {
        ShowCivilianPanel(!show);
    }
}
