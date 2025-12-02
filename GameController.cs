using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public BuildingPlacer buildingPlacer;

    void Start()
    {
        StartCoroutine(SetupBuildingPlacer());
    }

    private IEnumerator SetupBuildingPlacer()
    {
        // Czekamy, aż mapa się wygeneruje
        

        // Opcjonalnie czekamy jedną klatkę, jeśli GenerateMap() używa coroutines wewnętrznie
        yield return null;

        // Przypisanie tileGrid
        buildingPlacer.tileGrid = mapGenerator.tileGrid;

        Debug.Log("BuildingPlacer tileGrid assigned ✔");
    }
}
