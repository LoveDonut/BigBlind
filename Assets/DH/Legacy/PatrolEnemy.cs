using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : MonoBehaviour
{
    #region PrivateVariables

    [SerializeField] float _waitingTime = 1f;
    [SerializeField] Transform _path;

    [Header("°¨Áö")]
//    [SerializeField] private float fadeDuration = 1f;
//    [SerializeField] private float delay = 0.1f;
    private SpriteRenderer spriteRenderer;
    List<Transform> _paths;
    NavMeshAgent _agent;
    PlayerMovement _movement;

    [SerializeField] GameObject prefab;
    bool already_Detected;
    // enemy gets back starting point after arriving end point
    int currentPathIndex;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

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
    /*

    public void StartFadeOut()
    {
        if (already_Detected) return;
        StartCoroutine(Detected());


        already_Detected = true;
    }

    private IEnumerator Detected()
    {
        var me = Instantiate(prefab, transform.position, Quaternion.identity);

        float elapsedTime = 0f;
        SpriteRenderer render = me.GetComponent<SpriteRenderer>();  
        Color startColor = render.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += delay;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            render.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return new WaitForSeconds(delay);
        }

        already_Detected = false;
        Destroy(me);

    }
    */
}

#endregion
