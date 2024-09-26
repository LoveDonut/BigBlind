using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] _enemys;
    [SerializeField] float _spawnTime = 2f;

    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnTime);

            int enemyIndex = Random.Range(0, _enemys.Length);

            Instantiate(_enemys[enemyIndex], new Vector3(0f,6f,0f), Quaternion.identity);
        }
    }
}
