using UnityEngine;

public class CharacterControlRoot : MonoBehaviour
{
    public PlayerController playerController;
    public BaseAttackController attackController;

    public void EnableControl(PlayerInputReader input)
    {
        playerController.enabled = true;
        playerController.input = input;

        if (attackController != null)
            attackController.enabled = true;
    }

    public void DisableControl()
    {
        playerController.enabled = false;

        if (attackController != null)
            attackController.enabled = false;
    }
}
