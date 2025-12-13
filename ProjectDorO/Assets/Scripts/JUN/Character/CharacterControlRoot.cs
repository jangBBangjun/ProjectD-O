using UnityEngine;

public class CharacterControlRoot : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private void Awake()
    {
        // 기본 상태는 조작 불가
        playerController.enabled = false;
    }

    public void EnableControl(PlayerInputReader input)
    {
        playerController.enabled = true;
        playerController.input = input;
    }

    public void DisableControl()
    {
        playerController.enabled = false;
    }
}
