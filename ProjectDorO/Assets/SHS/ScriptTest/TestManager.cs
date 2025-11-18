using UnityEngine;
using UnityEngine.InputSystem;

public class TestManager : MonoBehaviour
{
    [SerializeField] Sub_EntityManager entityManager;

    private void Update()
    {
        if (Keyboard.current.digit1Key.isPressed)
        {
            entityManager.PlayerSeclect(0);
        }
        else if (Keyboard.current.digit2Key.isPressed)
        {
            entityManager.PlayerSeclect(0);
        }
        else if (Keyboard.current.digit3Key.isPressed)
        {
            entityManager.PlayerSeclect(2);
        }
        else if (Keyboard.current.digit4Key.isPressed)
        {
            entityManager.PlayerSeclect(3);
        }
        else if (Keyboard.current.digit5Key.isPressed)
        {
            entityManager.PlayerSeclect(4);
        }
    }
}