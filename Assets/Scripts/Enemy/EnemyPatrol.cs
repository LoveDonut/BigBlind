using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] Transform _path;
    #endregion

    #region PublicVariables
    [HideInInspector] public bool IsFindPlayer;
    #endregion

    #region PrivateVariables
    [Header("")]
    [SerializeField] float _waitingTime = 1f;

    List<Transform> _paths;
    NavMeshAgent _agent;
    int _currentPathIndex;
    bool _isGoForward;
    #endregion

    #region PrivateMethods
    void Start()
    {
        SetPaths();
        _agent = GetComponent<NavMeshAgent>();
        IsFindPlayer = false;
    }

    void SetPaths()
    {
        _paths = new List<Transform>(_path.GetComponentsInChildren<Transform>());
        _paths.RemoveAt(0);

        _isGoForward = true;

        _currentPathIndex = -1;
    }
    Vector3 GetNextDestination()
    {
        if(_paths.Count <= 1)
        {
            return _paths.Count == 1 ? _paths[0].position : transform.position; 
        }

        if (_isGoForward)
        {
            if (_currentPathIndex + 1 >= _paths.Count)
            {
                _isGoForward = false;
                _currentPathIndex--;
            }
            else
            {
                _currentPathIndex++;
            }
        }
        else
        {
            if (_currentPathIndex - 1 < 0)
            {
                _isGoForward = true;
                _currentPathIndex++;
            }
            else
            {
                _currentPathIndex--;
            }
        }

        return _paths[_currentPathIndex].position;
    }
    #endregion

    #region PublicMethods
    public void SetDestination()
    {
        _agent.SetDestination(GetNextDestination());
    }

    public Vector3 GetCurrentDestination()
    {
        return _agent.destination;
    }
    public float GetWaitingTime()
    {
        return _waitingTime;
    }
    #endregion
}
