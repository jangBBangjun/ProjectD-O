using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sub_TitleSceneManager : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    bool next = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame && next == false)
        {
            if (!next)
            {
                next = true;
                Sub_LoadingManager.LoadScene(nextSceneName);
            }
        }
    }
}
