using UnityEngine;
using UnityEngine.InputSystem;

public class wow : MonoBehaviour
{
    public Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.wKey.isPressed)
        {
            anim.SetTrigger("wow");
            Debug.Log("wow");
        }
    }
}
