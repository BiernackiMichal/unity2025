using UnityEngine;
using System.Collections.Generic;

public class EnemyPathing : MonoBehaviour
{
    public float moveSpeed = 3f;
    private List<Vector3> worldPath = new List<Vector3>();
    private int currentIndex = 0;

    private Enemy enemy;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        StartCoroutine(InitPath());
    }

    System.Collections.IEnumerator InitPath()
    {
        yield return new WaitForEndOfFrame(); // czekamy aż mapa wygeneruje się

        MapGenerator generator = Object.FindFirstObjectByType<MapGenerator>();
        if (generator == null)
        {
            Debug.LogError("Brak MapGenerator w scenie!");
            yield break;
        }

        foreach (Tile t in generator.roadTiles)
        {
            worldPath.Add(new Vector3(
                t.gridPosition.x * generator.tileSize,
                0,
                t.gridPosition.y * generator.tileSize
            ));
        }

        if (worldPath.Count > 0)
            transform.position = worldPath[0];
    }

    void Update()
    {
        if (worldPath.Count == 0 || currentIndex >= worldPath.Count) 
            return;

        Vector3 target = worldPath[currentIndex];

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        Vector3 dir = (target - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);

        if ((transform.position - target).sqrMagnitude < 0.05f)
        {
            currentIndex++;

            // Osiągnięto koniec
            if (currentIndex >= worldPath.Count)
            {
                Debug.Log("Enemy reached END OF PATH!");
                enemy.ReachEnd();
            }
        }
    }
}
