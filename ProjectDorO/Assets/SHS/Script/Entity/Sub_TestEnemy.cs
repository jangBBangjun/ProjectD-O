using UnityEngine;
using UnityEngine.InputSystem;

public class Sub_TestEnemy : MonoBehaviour
{
    [SerializeField] private Sub_MiniMapManager miniMapManager;

    [SerializeField] private bool useEnemy = false;
    private enum State { Idle, Die }
    [SerializeField] private State currentState;

    [SerializeField] private int health = 100;

    [SerializeField] private GameObject[] objectList;

    private void Start()
    {
        currentState = State.Idle;
    }

    void Update()
    {
        if (useEnemy)
        {
            switch (currentState)
            {
                case State.Idle:
                    if (Keyboard.current.zKey.isPressed)
                    {
                        ChangeObject(1);
                        Invoke("BackColor", 0.5f);
                    }
                    if (Keyboard.current.cKey.isPressed)
                    {
                        TakeDamage(health);
                    }
                    break;
                case State.Die:
                    ChangeObject(2);
                    break;
            }
        }
    }

    private void ChangeObject(int index)
    {
        for (int i = 0; i < objectList.Length; i++)
        {
            if (i == index)
                objectList[i].SetActive(true);
            else
                objectList[i].SetActive(false);
        }
    }
    private void BackColor()
    {
        ChangeObject(0);
    }
    public void TakeDamage(int damage)
    {
        if (currentState == State.Die) return;

        health -= damage;

        if (health <= 0)
        {
            miniMapManager.RemoveTarget(transform.GetComponent<Rigidbody>());
            ChangeState(State.Die);
        }

        Debug.Log("³²Àº HP: " + health);
    }

    private void ChangeState(State newState)
    {
        currentState = newState;

        Debug.Log("Enemy State Changed to: " + currentState);
    }
}