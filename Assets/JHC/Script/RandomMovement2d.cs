using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomMovement2d : MonoBehaviour
{
    public float moveRadius = 10f;
    public float moveInterval = 3f;

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        SetRandomDestination();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= moveInterval)
        {
            SetRandomDestination();
            timer = 0;
        }
    }

    void SetRandomDestination()
    {
        Vector2 randomDirection = Random.insideUnitCircle * moveRadius;
        Vector3 destination = transform.position + new Vector3(randomDirection.x, randomDirection.y, 0f);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(destination, out hit, moveRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
