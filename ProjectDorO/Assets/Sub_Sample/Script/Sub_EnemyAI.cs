using UnityEngine;
using UnityEngine.AI;

public class Sub_EnemyAI : MonoBehaviour
{
    public enum State { Idle, Chase, Attack, Die }
    private State currentState;

    public Transform target;
    public float chaseRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int health = 100;

    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime;
    private bool stoped;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        currentState = State.Idle;
    }

    void Update()
    {
        if(stoped == false)
            agent.enabled =true;
        else
            agent.enabled =false;

        if (target != null && currentState != State.Die)
        {
            if (agent.enabled == false)
                agent.enabled = true;

            float distance = Vector3.Distance(transform.position, target.position);

            switch (currentState)
            {
                case State.Idle:
                    animator.SetBool("isWalking", false);
                    if (distance < chaseRange)
                        ChangeState(State.Chase);
                    break;

                case State.Chase:
                    agent.SetDestination(target.position);
                    animator.SetBool("isWalking", true);

                    if (distance <= attackRange)
                        ChangeState(State.Attack);
                    else if (distance > chaseRange)
                        ChangeState(State.Idle);
                    break;

                case State.Attack:
                    agent.SetDestination(transform.position); // 멈춤
                    animator.SetBool("isWalking", false);

                    if (Time.time - lastAttackTime > attackCooldown)
                    {
                        lastAttackTime = Time.time;
                        animator.SetTrigger("attack");
                        // 여기에 공격 로직 추가 (예: 타겟에게 데미지 주기)
                    }

                    if (distance > attackRange)
                        ChangeState(State.Chase);
                    break;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentState == State.Die) return;

        health -= damage;

        if (health <= 0)
            ChangeState(State.Die);

        Debug.Log("남은 골렘의 HP: " + health);
    }

    private void ChangeState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        if (newState == State.Die)
        {
            agent.isStopped = true;
            animator.SetBool("die",true);   
            transform.GetComponent<NavMeshAgent>().enabled = false;
            // 필요시: Destroy(gameObject, 3f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("FireSkill1"))
            TakeDamage(20);
        if (other.name.Contains("FireSkill2") && other.GetComponent<ParticleSystem>()?.isPlaying == true)
            stoped = true;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.name.Contains("skill1_area"))
            TakeDamage(1);
        else if (other.name.Contains("FireSkill3"))
            TakeDamage(10);

        if (other.name.Contains("FireSkill2") && other.GetComponent<ParticleSystem>()?.isPlaying == true)
            stoped = true;
        else
            stoped = false;
    }
}