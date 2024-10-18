using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PhalanxBoss : MonoBehaviour
{
    public float rotationSpeed = 100f; // 회전 속도
    public float minRotationTime = 3f; // 최소 회전 시간
    public float maxRotationTime = 5f; // 최대 회전 시간

    private bool rotatingLeft = true;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // NavMeshAgent가 회전을 자동으로 하지 않도록 설정
            agent.updateRotation = false;
        }

        // 코루틴 시작
        StartCoroutine(RotateObject());
    }

    void Update()
    {
        if (agent == null || !agent.updateRotation)
        {
            // 왼쪽 또는 오른쪽으로 회전
            float rotationDirection = rotatingLeft ? 1 : -1;
            transform.Rotate(0, 0, rotationDirection * rotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator RotateObject()
    {
        while (true)
        {
            // 랜덤한 시간 동안 회전
            float rotationDuration = Random.Range(minRotationTime, maxRotationTime);
            yield return new WaitForSeconds(rotationDuration);

            // 회전 방향을 반대로 전환
            rotatingLeft = !rotatingLeft;
        }
    }
}
