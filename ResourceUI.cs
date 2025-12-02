using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    [Header("References")]
    public ResourceManager resourceManager;
    public TMP_Text goldText;
    public TMP_Text woodText;
    public TMP_Text stoneText;
    public TMP_Text foodText;

    [Header("Popup Settings")]
    public GameObject popupPrefab;
    [Tooltip("Default vertical offset of popup above the parent")]
    public float popupYOffset = 40f;

    [Header("Popup Parents (can be overridden per call)")]
    public Transform goldPopupParent;
    public Transform woodPopupParent;
    public Transform stonePopupParent;
    public Transform foodPopupParent;

    private void Start()
    {
        if (resourceManager == null) return;

        resourceManager.OnGoldChanged += UpdateGoldUI;
        resourceManager.OnWoodChanged += UpdateWoodUI;
        resourceManager.OnStoneChanged += UpdateStoneUI;
        resourceManager.OnFoodChanged += UpdateFoodUI;

        UpdateGoldUI(resourceManager.gold);
        UpdateWoodUI(resourceManager.wood);
        UpdateStoneUI(resourceManager.stone);
        UpdateFoodUI(resourceManager.food);
    }

    private void UpdateGoldUI(int amount) => goldText.text = amount.ToString();
    private void UpdateWoodUI(int amount) => woodText.text = amount.ToString();
    private void UpdateStoneUI(int amount) => stoneText.text = amount.ToString();
    private void UpdateFoodUI(int amount) => foodText.text = amount.ToString();

    /// <summary>
    /// Tworzy popup z liczbą przyrostu przy wskazanym rodzicu.
    /// Jeśli parent = null, używa domyślnego z inspektora.
    /// </summary>
    private void ShowResourcePopup(string content, Transform parent)
    {
        if (popupPrefab == null) return;

        Transform finalParent = parent;
        if (finalParent == null) return;

        GameObject popup = Instantiate(popupPrefab, finalParent);
        popup.transform.localPosition = new Vector3(0, popupYOffset, 0);

        TMP_Text textComp = popup.GetComponentInChildren<TMP_Text>();
        if (textComp != null)
            textComp.text = content;
    }

    public void ShowGoldPopup(int amount, Transform parent = null) =>
        ShowResourcePopup($"+{amount}", parent ?? goldPopupParent);

    public void ShowWoodPopup(int amount, Transform parent = null) =>
        ShowResourcePopup($"+{amount}", parent ?? woodPopupParent);

    public void ShowStonePopup(int amount, Transform parent = null) =>
        ShowResourcePopup($"+{amount}", parent ?? stonePopupParent);

    public void ShowFoodPopup(int amount, Transform parent = null) =>
        ShowResourcePopup($"+{amount}", parent ?? foodPopupParent);
}
