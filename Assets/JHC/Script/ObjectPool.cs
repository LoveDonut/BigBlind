using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPool : MonoBehaviour
{
    [SerializeField] UnityEngine.GameObject _objectPrefab;
    [SerializeField] int _poolSize = 10;
    [SerializeField] List<UnityEngine.GameObject> _poolObjects;

    void Start()
    {
        _poolObjects = new List<UnityEngine.GameObject>();
        
        for (int i = 0; i < _poolSize; i++)
        {
            UnityEngine.GameObject obj = Instantiate(_objectPrefab);
            obj.SetActive(false); 
            _poolObjects.Add(obj);
        }
    }

    public UnityEngine.GameObject GetObject()
    {
        foreach (UnityEngine.GameObject obj in _poolObjects)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true); 
                return obj;
            }
        }


        UnityEngine.GameObject newObj = Instantiate(_objectPrefab);
        newObj.SetActive(true);
        _poolObjects.Add(newObj);
        return newObj;
    }

   
    public void ReturnObject(UnityEngine.GameObject obj)
    {
        obj.SetActive(false); 
    }
}
