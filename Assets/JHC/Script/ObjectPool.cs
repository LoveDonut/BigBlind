using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject _objectPrefab;
    [SerializeField] int _poolSize = 10;
    [SerializeField] List<GameObject> _poolObjects;

    void Start()
    {
        _poolObjects = new List<GameObject>();
        
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = Instantiate(_objectPrefab);
            obj.SetActive(false); 
            _poolObjects.Add(obj);
        }
    }

    public GameObject GetObject()
    {
        foreach (GameObject obj in _poolObjects)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true); 
                return obj;
            }
        }


        GameObject newObj = Instantiate(_objectPrefab);
        newObj.SetActive(true);
        _poolObjects.Add(newObj);
        return newObj;
    }

   
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false); 
    }
}
