using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Sub_EnemyAI : MonoBehaviour
{
    private enum State { Idle, Chase, Attack, Die }
    [SerializeField] private State currentState;

    [SerializeField] private Transform target;
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int health = 100;

    [SerializeField] private Material material;

    private NavMeshAgent agent;
    private Animator animator;
    private AnimatorStateInfo stateInfo;
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
        if (Keyboard.current.zKey.isPressed)
        {
            material.color = Color.red;
            Invoke("BackColor", 0.5f);
        }
        if(stoped == false)
            agent.enabled =true;
        else
            agent.enabled =false;

        stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0: Base Layer
        if (target != null && currentState != State.Die)
        {
            if (agent.enabled == false)
                agent.enabled = true;

            float distance = Vector3.Distance(transform.position, target.position);

            switch (currentState)
            {
                case State.Idle:
                    if (stateInfo.IsName("Idle"))
                    {
                        if (distance <= attackRange)
                            ChangeState(State.Attack);
                        else if (distance < chaseRange)
                            ChangeState(State.Chase);
                    }
                    break;
                case State.Chase:
                    animator.SetBool("walk", true);

                    if (stateInfo.IsName("Walk"))
                        agent.SetDestination(target.position);
                    
                    if (distance > chaseRange || distance <= attackRange)
                        ChangeState(State.Idle);
                    break;
                case State.Attack:
                    Attack();
                    break;
                case State.Die:
                    animator.SetBool("die", true);

                    agent.isStopped = true;
                    agent.SetDestination(transform.position);
                    // ÇÊ¿ä½Ã: Destroy(gameObject, 3f);
                    break;
            }
        }
    }

    private void Attack()
    {
        ChangeState(State.Idle);
        
        agent.SetDestination(transform.position); // ¸ØÃã

        animator.SetTrigger("attack");
    }

    private void BackColor()
    {
        material.color = Color.white;
    }
    public void TakeDamage(int damage)
    {
        if (currentState == State.Die) return;

        health -= damage;

        if (health <= 0)
            ChangeState(State.Die);

        Debug.Log("³²Àº °ñ·½ÀÇ HP: " + health);
    }

    private void ChangeState(State newState)
    {
        currentState = newState;

        animator.SetBool("walk", false);
    }
    private void OnTriggerEnter(Collider other)
    {
        /*if (other.name.Contains("FireSkill1"))
            TakeDamage(20);
        if (other.name.Contains("FireSkill2") && other.GetComponent<ParticleSystem>()?.isPlaying == true)
            stoped = true;*/
    }
    private void OnTriggerStay(Collider other)
    {
        /*if (other.name.Contains("skill1_area"))
            TakeDamage(1);
        else if (other.name.Contains("FireSkill3"))
            TakeDamage(10);

        if (other.name.Contains("FireSkill2") && other.GetComponent<ParticleSystem>()?.isPlaying == true)
            stoped = true;
        else
            stoped = false;*/
    }
}