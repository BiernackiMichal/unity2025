using UnityEngine;
using UnityEngine.EventSystems;

public class FarmSelector : MonoBehaviour
{
    private Farm farm;

    private void Awake()
    {
        farm = GetComponent<Farm>();
    }

    private void OnMouseDown()
    {
        // Sprawdzamy, czy nie klikamy UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        farm?.OnFarmClicked();
    }
}
