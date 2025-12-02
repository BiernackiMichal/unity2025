using UnityEngine;

public class BuildingTabSwitcher : MonoBehaviour
{
    public GameObject civilianPanel;
    public GameObject militaryPanel;

    public void ShowCivilian()
    {
        civilianPanel.SetActive(true);
        militaryPanel.SetActive(false);
    }

    public void ShowMilitary()
    {
        civilianPanel.SetActive(false);
        militaryPanel.SetActive(true);
    }
}
