using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPrefabManager", menuName = "Enemy/Prefab Manager")]
public class EnemyPrefabManager : ScriptableObject
{
    public enum EnemyType
    {
        Enemy,
        Enemy_LongRange,
        Enemy_Patrol,
        Enemy_Patrol_LongRange
    }

    [System.Serializable]
    public class EnemyPrefabData
    {
        public EnemyType EnemyType;
        public UnityEngine.GameObject EnemyPrefab;
    }

    [SerializeField] List<EnemyPrefabData> _enemyPrefabDatas;

    public UnityEngine.GameObject GetPrefabByType(EnemyType type)
    {
        foreach (EnemyPrefabData data in _enemyPrefabDatas)
        {
            if (data.EnemyType == type)
            {
                return data.EnemyPrefab;
            }
        }
        return null;
    }
}
