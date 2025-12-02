using UnityEngine;

[System.Serializable]
public class BuildingData
{
    public string name;
    public GameObject prefab;
    public Vector2Int size;

    public int gold = 50;
    public int wood = 20;
    public int stone = 10;
    public int food = 0;

    public Sprite icon; // Ikona budynku do przycisku
}
