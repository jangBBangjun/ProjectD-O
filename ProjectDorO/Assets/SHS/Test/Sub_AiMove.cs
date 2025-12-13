using UnityEngine;
using UnityEngine.AI;

public class Sub_AiMove : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;
    private Vector3 targetPosition;
    private bool targeting = false;
    [SerializeField] private bool isMoving = false;
    private float stopDistance = 0;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        stopDistance = agent.stoppingDistance;
        targetPosition = transform.position;
    }
    /// <summary>
    /// 유닛을 목적지로 이동시킵니다.
    /// </summary>
    public void Move(Vector3 destination)
    {
        targetPosition = destination;
        agent.SetDestination(targetPosition);
        targeting = true;
    }
    public bool GetIsMoving()
    {
        return isMoving;
    }
    private void Update()
    {
        if (targeting && targetPosition != transform.position && 0.5f < Vector3.Distance(transform.position, targetPosition))
        {
            isMoving = true;
            agent.SetDestination(targetPosition);
            agent.stoppingDistance = 0;
        }
        else if (targeting == false && target != null && target.GetComponent<NavMeshAgent>().enabled && agent.stoppingDistance < Vector3.Distance(transform.position, target.transform.position))
        {
            isMoving = true;
            agent.SetDestination(target.position);
            targeting = false;
            agent.stoppingDistance = stopDistance;
        }
        else
        {
            targetPosition = transform.position;
            isMoving = false;
            targeting = false;
            agent.stoppingDistance = stopDistance;
        }
    }
}
