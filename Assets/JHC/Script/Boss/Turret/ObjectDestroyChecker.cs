using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyChecker : MonoBehaviour
{
    [SerializeField]Transform _objectParent;
    List<GameObject> objectList = new List<GameObject>();
    public int Count => objectList.Count;

    public void Start()
    {
        for (int i = 0; i < _objectParent.childCount; i++)
        {
            RegisterObject(_objectParent.GetChild(i).gameObject);
        }

    }
    public void AddObject(GameObject obj)
    {
        objectList.Add(obj);
    }

    private void OnDestroyObject(GameObject obj)
    {
        objectList.Remove(obj);
    }

    public void RegisterObject(GameObject obj)
    {
        AddObject(obj);
        obj.AddComponent<DestroyNotifier>().OnDestroyed += OnDestroyObject;
    }
}

public class DestroyNotifier : MonoBehaviour
{
    public delegate void Destroyed(GameObject obj);
    public event Destroyed OnDestroyed;

    private void OnDestroy()
    {
        if (OnDestroyed != null)
        {
            OnDestroyed(gameObject);
        }
    }
}
