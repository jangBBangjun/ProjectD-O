using UnityEngine;
using UnityEngine.InputSystem;

public class TestManager : MonoBehaviour
{
    [SerializeField] TestCam testCam;
    [SerializeField] CharacterManager characterManager;
    [SerializeField] MiniMapManager miniMapManager;

    [SerializeField] int usePlayerNum = 0;
    private void Update()
    {
        if(Keyboard.current.digit1Key.isPressed)
        {
            PlayerSeclect(0);
        }
        else if(Keyboard.current.digit2Key.isPressed)
        {
            PlayerSeclect(1);
        }
        else if(Keyboard.current.digit3Key.isPressed)
        {
            PlayerSeclect(2);
        }
        else if(Keyboard.current.digit4Key.isPressed)
        {
            PlayerSeclect(3);
        }
        else if(Keyboard.current.digit5Key.isPressed)
        {
            PlayerSeclect(4);
        }
    }
    private void PlayerSeclect(int changeNum)
    {
        usePlayerNum = changeNum;

        Transform player = null;
        player = characterManager.GetPlayer(usePlayerNum);

        if (player != null)
        {
            miniMapManager.SetTarget(player);
            testCam.SetTarget(player);
        }
    }
}
