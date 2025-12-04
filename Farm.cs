using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Farm : MonoBehaviour
{
[Header("Field Settings")]
public GameObject fieldPrefab;
public Vector2Int fieldSize = Vector2Int.one;


[Header("UI")]
public Canvas farmCanvas;       // Canvas w World Space
public Button fieldButton;      // Przycisk do stawiania pola

private Camera cam;
private bool uiOpen = false;

void Start()
{
    cam = Camera.main;

    // Ukryj UI na starcie
    if (farmCanvas != null)
    {
        AssignCameraToCanvas();
        farmCanvas.enabled = false;
    }

    // Podłącz kliknięcie do przycisku
    if (fieldButton != null)
        fieldButton.onClick.AddListener(OnFieldButtonClicked);
}

void LateUpdate()
{
    if (!uiOpen || farmCanvas == null) return;

    // UI patrzy na kamerę
    if (cam != null)
        farmCanvas.transform.forward = cam.transform.forward;

    HandleCloseOnClickOutside();
}

//----------------------------------------------
// UI HANDLING
//----------------------------------------------
public void OnFarmClicked()
{
ToggleUI();
}


private void ToggleUI()
{
    uiOpen = !uiOpen;

    if (farmCanvas != null)
        farmCanvas.enabled = uiOpen;

    if (uiOpen)
        UpdateFieldButtonState();
}

private void HandleCloseOnClickOutside()
{
    if (!uiOpen) return;
    if (Mouse.current == null) return;
    if (!Mouse.current.leftButton.wasPressedThisFrame) return;

    // jeśli kliknięcie w UI, nie zamykamy
    if (UnityEngine.EventSystems.EventSystem.current != null && 
        UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        return;

    // jeśli kliknięcie w farmę, nie zamykamy
    Vector2 mousePos = Mouse.current.position.ReadValue();
    Ray ray = cam.ScreenPointToRay(mousePos);
    if (Physics.Raycast(ray, out RaycastHit hit))
    {
        if (hit.collider != null && hit.collider.gameObject == gameObject)
            return;
    }

    // zamykamy UI
    uiOpen = false;
    if (farmCanvas != null)
        farmCanvas.enabled = false;
}



public void HideUI()
{
    uiOpen = false;
    if (farmCanvas != null)
        farmCanvas.enabled = false;
}

//----------------------------------------------
// FIELD BUTTON
//----------------------------------------------

private void UpdateFieldButtonState()
{
    if (fieldButton == null) return;

    ResourceManager rm = FindAnyObjectByType<ResourceManager>();
    if (rm == null) return;

    bool canAfford = rm.CanAfford(0, 0, 0); // TODO: Ustaw koszt pola

    fieldButton.interactable = canAfford;
}

private void OnFieldButtonClicked()
{
    BuildingPlacer placer = FindAnyObjectByType<BuildingPlacer>();
    if (placer != null && fieldPrefab != null)
    {
        placer.StartPlacingField(this);
    }

    HideUI();
}

//----------------------------------------------
// CANVAS CAMERA ASSIGNMENT
//----------------------------------------------

private void AssignCameraToCanvas()
{
    if (farmCanvas == null) return;

    if (farmCanvas.renderMode == RenderMode.WorldSpace ||
        farmCanvas.renderMode == RenderMode.ScreenSpaceCamera)
    {
        farmCanvas.worldCamera = Camera.main;
    }
}


}
