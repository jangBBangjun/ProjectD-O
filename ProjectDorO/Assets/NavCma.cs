using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NavCma : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform target;
    public Transform lookPos;
    bool wow = false;
    bool lookT;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        agent.SetDestination(transform.position);
    }

    private void Update()
    {
        if(Keyboard.current.wKey.isPressed)
            wow = true;
        if(Keyboard.current.vKey.isPressed)
            lookT = true;
        if (target == null || !wow)
        {
            return;
        }
        if(lookT == false)
            transform.LookAt(lookPos);
        else
            transform.LookAt(target);
        agent.SetDestination(target.position);
    }
}
