using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float maxAmountOfEnemies;
    public GameObject enemy;
    private List<GameObject> enemies;
    private Timer timer;
    private Vector3 spawnVector;
    private void SpawnEnemy()
    {
        enemies.RemoveAll(s => s == null);
        if (enemies.Count < maxAmountOfEnemies)
        {
            spawnVector.x += Random.Range(-10, 10);
            spawnVector.z += Random.Range(-10, 10);
            GameObject enemySpawned = Instantiate(enemy, spawnVector, Quaternion.identity);
            enemies.Add(enemySpawned);
        }
        spawnVector = transform.position;
    }

    void Start()
    {
        spawnVector = transform.position;
        enemies = new List<GameObject>();
        timer = GetComponent<Timer>();
        timer.Duration = 1;
        timer.Run();
    }

    void Update()
    {

        if (timer.Finished)
        {
            SpawnEnemy();
            timer.Run();
        }

        
    }
}
