using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBoss : MonoBehaviour
{
    ObjectDestroyChecker objectDestroyChecker;
    void Awake()
    {
        objectDestroyChecker = GetComponent<ObjectDestroyChecker>();
    }

    // Update is called once per frame
    void Update()
    {
        if (objectDestroyChecker == null) return;
        if (objectDestroyChecker.Count <= 0)
        {
            Destroy(gameObject);    
        }
    }
}
