using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// made by KimDaehui
public class EnemySpawner : MonoBehaviour
{
    #region PrivateClasses
    [System.Serializable]
    class EnemySpawnInfo
    {
        public Transform spawnTransform;
        public EnemyPrefabManager.EnemyType enemyType;
        public float spawnTime;
    }
    #endregion

    #region PrivateVariables
    [SerializeField] EnemyPrefabManager _enemyPrefabManager;
    [SerializeField] List<EnemySpawnInfo> _enemySpawnInfos;

    //made by JHC
    [SerializeField] bool _isRepeating = false;
    #endregion

    void Start()
    {
        _enemySpawnInfos.Sort((enemy1, enemy2) => enemy1.spawnTime.CompareTo(enemy2.spawnTime));
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (_isRepeating)
        {
            float beforeSpawnTime = 0f;

            foreach (EnemySpawnInfo enemyInfo in _enemySpawnInfos)
            {
                float timeDiff = enemyInfo.spawnTime - beforeSpawnTime;

                if (timeDiff > 0f)
                {
                    // spawn enemies after spawnTime
                    yield return new WaitForSeconds(timeDiff);
                }

                // get a enemyPrefab
                GameObject enemyPrefab = _enemyPrefabManager.GetPrefabByType(enemyInfo.enemyType);

                if (enemyPrefab != null)
                {
                    // spawn enemy
                    Instantiate(enemyPrefab, enemyInfo.spawnTransform.position, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("적 프리팹이 없습니다: " + enemyInfo.enemyType);
                }

                beforeSpawnTime = enemyInfo.spawnTime;
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
