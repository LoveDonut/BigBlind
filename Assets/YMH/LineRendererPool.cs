using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererPool : MonoBehaviour
{
    private static LineRendererPool instance;
    public static LineRendererPool Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("LineRendererPool");
                instance = go.AddComponent<LineRendererPool>();
            }
            return instance;
        }
    }

    private Queue<LineRenderer> pooledObjects = new Queue<LineRenderer>();
    private int poolSize = 200;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializePool();
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewLineRenderer();
        }
    }

    private void CreateNewLineRenderer()
    {
        GameObject lineObj = new GameObject("PooledLineRenderer");
        lineObj.transform.SetParent(transform);
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.gameObject.SetActive(false);
        pooledObjects.Enqueue(lr);
    }

    public LineRenderer GetLineRenderer()
    {
        if (pooledObjects.Count == 0)
        {
            CreateNewLineRenderer();
        }
        LineRenderer lr = pooledObjects.Dequeue();
        lr.gameObject.SetActive(true);
        return lr;
    }

    public void ReturnToPool(LineRenderer lr)
    {
        lr.gameObject.SetActive(false);
        lr.transform.SetParent(transform);
        pooledObjects.Enqueue(lr);
    }
    public void ClearPool()
    {
        while (pooledObjects.Count > 0)
        {
            LineRenderer lr = pooledObjects.Dequeue();
            if (lr != null)
            {
                Destroy(lr.gameObject);
            }
        }
    }
}