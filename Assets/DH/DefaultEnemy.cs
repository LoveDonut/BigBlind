using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemy : MonoBehaviour
{
    #region PrivateVariables

    [SerializeField] float _waitingTime = 1f;
    [SerializeField] Transform _path;
    Transform[] _paths;

    // enemy gets back starting point after arriving end point
    int currentPathIndex;

    #endregion

    #region PublicVariables

    #endregion


    #region PrivateMethods
    void Start()
    {
        _paths = _path.GetComponentsInChildren<Transform>();
    }

    void Update()
    {

    }
    #endregion

    #region PublicMethods

    public Transform GetDestination()
    {
        currentPathIndex = (currentPathIndex + 1) % _paths.Length;
        Debug.Log(currentPathIndex);
        return _paths[currentPathIndex];
    }

    public float GetWaitingTime()
    {
        return _waitingTime;
    }

    #endregion
}
