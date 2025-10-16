using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [Header("�ʼ� ����")]
    [Tooltip("�÷��̾� ����")]
    [SerializeField] private Transform player;
    private EnemyUI uI;
    private enum EnemyState { Idle, Patrol, Chase, Attack }

    [Header("����")]
    [Tooltip("���� ���� ����")]
    [SerializeField] private EnemyState currentState;

    [Header("���� ����(����)")]
    [Tooltip("���� ������")]
    [SerializeField] private List<Vector3> patrolPoints = new List<Vector3>();
    private int patrolIndex = 0; // ���� ���� ���� �ε���

    [Header("�þ�/û�� ����")]
    [Tooltip("�þ� ����(�翷 �հ� ����)")]
    [SerializeField] private float fovAngle = 100f;
    [Tooltip("�þ� �ִ� �Ÿ�")]
    [SerializeField] private float viewDistance = 18f;
    [Tooltip("�þ� ���� ����")]
    [SerializeField] private float eyeHeight = 1.7f;
    [Tooltip("�÷��̾� ���� ����")]
    [SerializeField] private float playerHeight = 1.1f;
    [Tooltip("��û �Ÿ�(�÷��̾ ���� ���ص� �Ҹ��� ����)")]
    [SerializeField] private float hearingRadius = 8f;
    [Tooltip("�þ߸� �����ϴ� ���̾�(�� ��)")]
    [SerializeField] private LayerMask obstacleMask;

    [Tooltip("�÷��̾� �߰� �Ӱ�ġ")]
    [SerializeField] private float alertThreshold = 1f;
    private float currentAlert = 0f; // ���� ���� ����ġ

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        uI = GetComponentInChildren<EnemyUI>();
    }
    // ���� ��ȯ
    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }
    private void Update()
    {
        // �÷��̾� �߰� �� ���� ���·� ��ȯ
        if (IsPlayerInSight())
            ChangeState(EnemyState.Chase);

        // ���� ����
        switch (currentState)
        {
            case EnemyState.Idle:
                IDle();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }
    }
    // �ð� ����
    private bool CanSeePlayer()
    {
        Vector3 toTarget = player.position - transform.position;
        float sqrDist = toTarget.sqrMagnitude;
        if (sqrDist > viewDistance * viewDistance) return false;

        Vector3 dir = toTarget.normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > fovAngle * 0.5f) return false;

        // ������ ����
        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Vector3 targetCenter = player.position + Vector3.up * playerHeight;

        // �þ� ���� üũ(�� ��)
        if (Physics.Linecast(eye, targetCenter, obstacleMask)) return false;

        return true;
    }
    // û�� ����: �ܼ� �ݰ� üũ(�ʿ��ϸ� �÷��̾� �ӵ�/�޸��⸸ ��û ó���� Ȯ�� ����)
    private bool CanHearPlayer()
    {
        Vector3 toTarget = player.position - transform.position;
        float sqrDist = toTarget.sqrMagnitude;
        return sqrDist <= hearingRadius * hearingRadius;
    }
    // �÷��̾� �߰� ����
    private bool IsPlayerInSight()
    {
        // ����ġ ��� ����
        if (currentAlert < alertThreshold)
        {
            if (CanSeePlayer())
                currentAlert += Time.deltaTime;
            else
                currentAlert -= Time.deltaTime;

            if (CanHearPlayer())
                currentAlert += Time.deltaTime * 0.5f;
            else
                currentAlert -= Time.deltaTime * 0.5f;

            currentAlert = Mathf.Clamp(currentAlert, 0f, alertThreshold);

            uI.SetSeeFillAmount(currentAlert, alertThreshold);

            return false;
        }
        else // ����ġ�� �Ӱ�ġ ���� �� �߰�
        {
            uI.SetSeeFillAmount(1, 1);
            return true;
        }
    }

    // ��� ����
    private void IDle()
    {
        ChangeState(EnemyState.Patrol);
    }

    [ContextMenu("���� ���� �߰�")]
    private void AddPartolPoint()
    {
        patrolPoints.Add(transform.position);
    }
    // ���� ����
    private void Patrol()
    {
        if (patrolPoints.Count == 0) return;

        if (Vector3.Distance(transform.position, patrolPoints[patrolIndex]) > agent.stoppingDistance)
        {
            agent.SetDestination(patrolPoints[patrolIndex]);
        }
        else
        {
            Debug.Log("���� ���� �������� �̵�");
            if (patrolIndex < patrolPoints.Count - 1)
                ++patrolIndex;
            else
                patrolIndex = 0;
        }
    }
    // ���� ����
    private void Chase()
    {
        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= agent.stoppingDistance)
        {
            ChangeState(EnemyState.Attack);
        }
        else if (!IsPlayerInSight())
        {
            ChangeState(EnemyState.Idle);
        }
    }
    // ���� ����
    private void Attack()
    {
        Debug.Log("Attack!");
    }
    // ����ȭ
    private void OnDrawGizmosSelected()
    {
        // ���� �÷��̾� ���� ����
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(transform.position + Vector3.up * eyeHeight, 0.1f);
        Gizmos.DrawSphere(player.position + Vector3.up * playerHeight, 0.1f);
        Gizmos.DrawLine(transform.position + Vector3.up * eyeHeight, player.position + Vector3.up * playerHeight);

        // �þ� ���� ǥ��
        Gizmos.color = Color.red;
        Vector3 left = Quaternion.Euler(0f, -fovAngle * 0.5f, 0f) * transform.forward;
        Vector3 right = Quaternion.Euler(0f, fovAngle * 0.5f, 0f) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + left * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + right * viewDistance);

        // ��û �Ÿ� ǥ��
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);

        // ���� ��� ǥ��
        Gizmos.color = Color.blue;
        if (patrolPoints != null)
        {
            if (patrolPoints.Count > 1)
            {
                for (int i = 0; i < patrolPoints.Count; i++)
                {
                    Vector3 current = patrolPoints[i];
                    Vector3 next = patrolPoints[(i + 1) % patrolPoints.Count];
                    Gizmos.DrawSphere(current, 0.3f);
                    Gizmos.DrawLine(current, next);
                }
            }
            else if (patrolPoints.Count == 1)
            {
                Gizmos.DrawSphere(patrolPoints[0], 0.3f);
            }
        }
    }
}
