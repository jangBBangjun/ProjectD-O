using UnityEngine;
using UnityEngine.AI;

public class AiMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        targetPosition = transform.position;
    }
    /// <summary>
    /// ������ �������� �̵���ŵ�ϴ�.
    /// </summary>
    public void Move(Vector3 destination)
    {
        targetPosition = destination;
        agent.SetDestination(targetPosition);
        isMoving = true;
    }
    private void Update()
    {
        if(isMoving == true && gameObject.activeSelf == true && agent.destination != targetPosition)
        {
            agent.SetDestination(targetPosition);
        }
    }
}
