using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmpBoss : MonoBehaviour
{
    [SerializeField] List<GameObject> _enemyList;
    [SerializeField] List<Transform> _spawnPoints;

    ObjectDestroyChecker objectDestroyChecker;
    int beforeObjCnt;
    void Awake()
    {
        objectDestroyChecker = GetComponent<ObjectDestroyChecker>();
    }

    private void Start()
    {
        beforeObjCnt = objectDestroyChecker.Count;
    }
    private void SpawnEnemy()
    {
        foreach (GameObject obj in _enemyList)
        {
            Transform spawnPoint = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)];
            var enemy = Instantiate(obj, spawnPoint.position, Quaternion.identity);
            enemy.transform.parent = this.transform;
        }
    }
    void Update()
    {
        if (objectDestroyChecker == null) return;
        if (objectDestroyChecker.Count != beforeObjCnt)
        {
            beforeObjCnt = objectDestroyChecker.Count;
            SpawnEnemy();
        }
    }
}
