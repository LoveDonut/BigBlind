using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PhalanxBoss : MonoBehaviour
{
    public float rotationSpeed = 100f; // ȸ�� �ӵ�
    public float minRotationTime = 3f; // �ּ� ȸ�� �ð�
    public float maxRotationTime = 5f; // �ִ� ȸ�� �ð�

    private bool rotatingLeft = true;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // NavMeshAgent�� ȸ���� �ڵ����� ���� �ʵ��� ����
            agent.updateRotation = false;
        }

        // �ڷ�ƾ ����
        StartCoroutine(RotateObject());
    }

    void Update()
    {
        if (agent == null || !agent.updateRotation)
        {
            // ���� �Ǵ� ���������� ȸ��
            float rotationDirection = rotatingLeft ? 1 : -1;
            transform.Rotate(0, 0, rotationDirection * rotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator RotateObject()
    {
        while (true)
        {
            // ������ �ð� ���� ȸ��
            float rotationDuration = Random.Range(minRotationTime, maxRotationTime);
            yield return new WaitForSeconds(rotationDuration);

            // ȸ�� ������ �ݴ�� ��ȯ
            rotatingLeft = !rotatingLeft;
        }
    }
}
