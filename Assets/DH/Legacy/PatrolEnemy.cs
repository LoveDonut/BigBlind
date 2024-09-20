using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : MonoBehaviour
{
    #region PrivateVariables

    [SerializeField] float _waitingTime = 1f;
    [SerializeField] Transform _path;
    List<Transform> _paths;
    NavMeshAgent _agent;
    PlayerMovement _movement;

    // enemy gets back starting point after arriving end point
    int currentPathIndex;

    #endregion

    #region PublicVariables

    #endregion


    #region PrivateMethods
    void Start()
    {
        _paths = new List<Transform>(_path.GetComponentsInChildren<Transform>());
        _paths.RemoveAt(0);
        _movement = FindObjectOfType<PlayerMovement>();
        _agent = GetComponent<NavMeshAgent>();
//        _agent.SetDestination(_movement.transform.position);
//        Debug.Log(_movement.transform.position);
    }

    void Update()
    {
        transform.rotation = Quaternion.identity;
    }
    #endregion

    #region PublicMethods

    public Transform GetDestination()
    {
        currentPathIndex = (currentPathIndex + 1) % _paths.Count;
        //Debug.Log($"paths' length : {_paths.Count}");
        //foreach(Transform transform in _paths)
        //{
        //    Debug.Log(transform.position);
        //}
//        Debug.Log($"currentPathIndex : {currentPathIndex}, Destination's position : { _paths[currentPathIndex].position}");
        return _paths[currentPathIndex];
    }

    public float GetWaitingTime()
    {
        return _waitingTime;
    }

    #endregion
}
