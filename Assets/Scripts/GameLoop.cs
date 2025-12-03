using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public Transform minCorner;
    public Transform maxCorner;
    public GameObject enemyPrefab;
    public float spawnY = 0f;
    public float spawnInterval = 20f;

    private void OnEnable()
    {
        GameManager.OnPauseStateChanged += PauseSpawn;
    }

    private void OnDisable()
    {
        GameManager.OnPauseStateChanged -= PauseSpawn;
    }

    void PauseSpawn(bool isPaused)
    {
        this.enabled = !isPaused;
    }

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void Spawn()
    {
        if (Time.timeScale == 0) return;

        Instantiate(enemyPrefab, RandomSpawnPos(), Quaternion.identity);
    }

    Vector3 RandomSpawnPos()
    {
        float x = Random.Range(minCorner.position.x, maxCorner.position.x);
        float z = Random.Range(minCorner.position.z, maxCorner.position.z);

        return new Vector3(x, spawnY, z);
    }
}