using UnityEngine;
using UnityEngine.AI;

public class Sub_Enemy : MonoBehaviour
{
    public enum State { Idle, Patrol, Chase, Attack, Skill, Listen, Die }

    [Header("AI 설정")]
    public State currentState = State.Idle;
    public Transform[] patrolPoints;
    public float idleDuration = 2f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    [Header("후딜 설정")]
    public float attackAfterDelay = 1f;
    public float skillAfterDelay = 2f;

    [Header("스킬 설정")]
    public float skillCooldown = 10f;
    public float skillRange = 8f;

    [Header("추적 설정")]
    public float chaseStartRange = 6f;
    public float chaseStopRange = 15f;

    [Header("태그 설정")]
    public string ratTag = "Rat";
    public string playerTag = "Player";

    private int patrolIndex = 0;
    private float stateTimer = 0f;
    private float lastAttackTime = -999f;
    private float lastSkillTime = -999f;
    private bool isDead = false;
    private bool isListening = false;
    private Vector3 targetListenPosition;

    private NavMeshAgent agent;
    private Animator animator;
    private GameObject closestPlayer;
    private State previousStateBeforeSkill = State.Idle;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        TransitionToState(State.Idle);
    }

    void Update()
    {
        if (isDead) return;

        closestPlayer = FindClosestPlayer();
        if (closestPlayer == null) return;

        float distToPlayer = Vector3.Distance(transform.position, closestPlayer.transform.position);

        // ✅ 스킬 조건: 공격 범위 밖 & 추적 범위 안
        if (distToPlayer > attackRange &&
            distToPlayer < chaseStartRange &&
            Time.time - lastSkillTime >= skillCooldown &&
            currentState != State.Skill &&
            currentState != State.Die &&
            currentState != State.Attack)
        {
            previousStateBeforeSkill = currentState; // 상태 저장
            TransitionToState(State.Skill);
            return;
        }

        // ✅ 추적 조건
        if (distToPlayer < chaseStartRange &&
            currentState != State.Chase &&
            currentState != State.Attack &&
            currentState != State.Die)
        {
            TransitionToState(State.Chase);
            return;
        }

        // ✅ FSM 상태 실행
        switch (currentState)
        {
            case State.Idle: HandleIdle(); break;
            case State.Patrol: HandlePatrol(); break;
            case State.Chase: HandleChase(); break;
            case State.Attack: HandleAttack(); break;
            case State.Skill: HandleSkill(); break;
            case State.Listen: HandleListen(); break;
            case State.Die: HandleDie(); break;
        }
    }

    GameObject FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = p;
            }
        }

        return nearest;
    }

    void TransitionToState(State newState)
    {
        currentState = newState;
        stateTimer = 0f;

        switch (newState)
        {
            case State.Idle:
                agent.isStopped = true;
                break;

            case State.Patrol:
                agent.isStopped = false;
                if (patrolPoints.Length > 0)
                    agent.SetDestination(patrolPoints[patrolIndex].position);
                break;

            case State.Chase:
                agent.isStopped = false;
                break;

            case State.Attack:
                agent.isStopped = true;
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
                break;

            case State.Skill:
                agent.isStopped = true;
                agent.ResetPath();
                agent.SetDestination(transform.position); // ✅ 현재 위치로 고정하여 완전 정지
                animator.SetTrigger("Howling");
                UseSkill1();
                break;

            case State.Listen:
                isListening = true;
                agent.isStopped = false;
                agent.SetDestination(targetListenPosition);
                animator.SetTrigger("Listen");
                break;

            case State.Die:
                agent.isStopped = true;
                animator.SetTrigger("Die");
                break;
        }
    }

    void HandleIdle()
    {
        stateTimer += Time.deltaTime;
        if (stateTimer >= idleDuration)
            TransitionToState(State.Patrol);
    }

    void HandlePatrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    void HandleChase()
    {
        if (closestPlayer == null) return;

        float dist = Vector3.Distance(transform.position, closestPlayer.transform.position);

        if (dist <= attackRange)
        {
            TransitionToState(State.Attack);
            return;
        }

        if (dist > chaseStopRange)
        {
            TransitionToState(State.Patrol);
            return;
        }

        agent.SetDestination(closestPlayer.transform.position);
    }

    void HandleAttack()
    {
        if (closestPlayer == null) return;

        float dist = Vector3.Distance(transform.position, closestPlayer.transform.position);

        if (Time.time - lastAttackTime < attackAfterDelay)
            return;

        if (dist > attackRange + 0.5f)
        {
            TransitionToState(State.Chase);
        }
        else
        {
            TransitionToState(State.Attack); // 재공격
        }
    }

    void HandleSkill()
    {
        // ✅ 스킬 중 이동 완전 차단
        if (!agent.isStopped)
            agent.isStopped = true;

        agent.SetDestination(transform.position); // ✅ 매 프레임 현재 위치 고정

        stateTimer += Time.deltaTime;

        if (stateTimer >= skillAfterDelay)
        {
            if (previousStateBeforeSkill == State.Chase)
                TransitionToState(State.Chase);
            else
                TransitionToState(State.Idle);
        }
    }

    void HandleListen()
    {
        if (!isListening) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            isListening = false;
            TransitionToState(State.Idle);
        }
    }

    void HandleDie()
    {
        // 아무 것도 하지 않음
    }

    void UseSkill1()
    {
        Debug.Log("스킬1 발동 - 주변 쥐를 호출합니다.");
        Collider[] rats = Physics.OverlapSphere(transform.position, skillRange);

        foreach (var col in rats)
        {
            if (col.CompareTag(ratTag) && col.gameObject != this.gameObject)
            {
                Sub_Enemy other = col.GetComponent<Sub_Enemy>();
                if (other != null)
                {
                    Vector3 offset = Random.insideUnitSphere * 1.5f;
                    offset.y = 0;
                    Vector3 target = transform.position + offset;
                    other.ReceiveListenCommand(target);
                }
            }
        }

        lastSkillTime = Time.time;
    }

    public void ReceiveListenCommand(Vector3 targetPos)
    {
        if (isDead) return;

        targetListenPosition = targetPos;
        TransitionToState(State.Listen);
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        TransitionToState(State.Die);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || agent == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, skillRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseStartRange);

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, chaseStopRange);

        if (agent.hasPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(agent.destination, 0.2f);
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }
}