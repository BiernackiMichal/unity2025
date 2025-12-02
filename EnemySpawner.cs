using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    
   [System.Serializable]
    public class EnemyWave
    {
        public GameObject enemyPrefab;
        public int count = 5;
        public float interval = 0.5f;
    }


    public Wave[] waves;
    public float timeBetweenWaves = 3f;

    private int currentWave = 0;
    public bool loopWaves = false;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return StartCoroutine(SpawnWave(waves[currentWave]));

            yield return new WaitForSeconds(timeBetweenWaves);

            currentWave++;

            if (currentWave >= waves.Length)
            {
                if (loopWaves)
                    currentWave = 0;
                else
                    yield break;
            }
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.count; i++)
        {
            Instantiate(wave.enemyPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(wave.interval);
        }
    }
}
