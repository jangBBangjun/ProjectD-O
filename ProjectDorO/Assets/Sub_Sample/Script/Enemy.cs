using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [Header("필수 참조")]
    [Tooltip("플레이어 참조")]
    [SerializeField] private Transform player;
    private EnemyUI uI;
    private enum EnemyState { Idle, Patrol, Chase, Attack }

    [Header("상태")]
    [Tooltip("적의 현재 상태")]
    [SerializeField] private EnemyState currentState;

    [Header("순찰 설정(선택)")]
    [Tooltip("순찰 지점들")]
    [SerializeField] private List<Vector3> patrolPoints = new List<Vector3>();
    private int patrolIndex = 0; // 현재 순찰 지점 인덱스

    [Header("시야/청각 설정")]
    [Tooltip("시야 각도(양옆 합계 각도)")]
    [SerializeField] private float fovAngle = 100f;
    [Tooltip("시야 최대 거리")]
    [SerializeField] private float viewDistance = 18f;
    [Tooltip("시야 높이 보정")]
    [SerializeField] private float eyeHeight = 1.7f;
    [Tooltip("플레이어 높이 보정")]
    [SerializeField] private float playerHeight = 1.1f;
    [Tooltip("가청 거리(플레이어를 보지 못해도 소리로 감지)")]
    [SerializeField] private float hearingRadius = 8f;
    [Tooltip("시야를 차단하는 레이어(벽 등)")]
    [SerializeField] private LayerMask obstacleMask;

    [Tooltip("플레이어 발견 임계치")]
    [SerializeField] private float alertThreshold = 1f;
    private float currentAlert = 0f; // 현재 감지 누적치

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        uI = GetComponentInChildren<EnemyUI>();
    }
    // 상태 전환
    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }
    private void Update()
    {
        // 플레이어 발견 시 추적 상태로 전환
        if (IsPlayerInSight())
            ChangeState(EnemyState.Chase);

        // 상태 진행
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
    // 시각 판정
    private bool CanSeePlayer()
    {
        Vector3 toTarget = player.position - transform.position;
        float sqrDist = toTarget.sqrMagnitude;
        if (sqrDist > viewDistance * viewDistance) return false;

        Vector3 dir = toTarget.normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > fovAngle * 0.5f) return false;

        // 눈높이 보정
        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Vector3 targetCenter = player.position + Vector3.up * playerHeight;

        // 시야 차단 체크(벽 등)
        if (Physics.Linecast(eye, targetCenter, obstacleMask)) return false;

        return true;
    }
    // 청각 판정: 단순 반경 체크(필요하면 플레이어 속도/달리기만 가청 처리로 확장 가능)
    private bool CanHearPlayer()
    {
        Vector3 toTarget = player.position - transform.position;
        float sqrDist = toTarget.sqrMagnitude;
        return sqrDist <= hearingRadius * hearingRadius;
    }
    // 플레이어 발견 여부
    private bool IsPlayerInSight()
    {
        // 누적치 기반 감지
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
        else // 누적치가 임계치 도달 시 발견
        {
            uI.SetSeeFillAmount(1, 1);
            return true;
        }
    }

    // 대기 상태
    private void IDle()
    {
        ChangeState(EnemyState.Patrol);
    }

    [ContextMenu("순찰 지점 추가")]
    private void AddPartolPoint()
    {
        patrolPoints.Add(transform.position);
    }
    // 순찰 상태
    private void Patrol()
    {
        if (patrolPoints.Count == 0) return;

        if (Vector3.Distance(transform.position, patrolPoints[patrolIndex]) > agent.stoppingDistance)
        {
            agent.SetDestination(patrolPoints[patrolIndex]);
        }
        else
        {
            Debug.Log("다음 순찰 지점으로 이동");
            if (patrolIndex < patrolPoints.Count - 1)
                ++patrolIndex;
            else
                patrolIndex = 0;
        }
    }
    // 추적 상태
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
    // 공격 상태
    private void Attack()
    {
        Debug.Log("Attack!");
    }
    // 가시화
    private void OnDrawGizmosSelected()
    {
        // 눈과 플레이어 높이 보정
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(transform.position + Vector3.up * eyeHeight, 0.1f);
        Gizmos.DrawSphere(player.position + Vector3.up * playerHeight, 0.1f);
        Gizmos.DrawLine(transform.position + Vector3.up * eyeHeight, player.position + Vector3.up * playerHeight);

        // 시야 각도 표시
        Gizmos.color = Color.red;
        Vector3 left = Quaternion.Euler(0f, -fovAngle * 0.5f, 0f) * transform.forward;
        Vector3 right = Quaternion.Euler(0f, fovAngle * 0.5f, 0f) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + left * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + right * viewDistance);

        // 가청 거리 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);

        // 순찰 경로 표시
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
