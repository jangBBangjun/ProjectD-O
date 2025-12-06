using UnityEngine;
using UnityEngine.InputSystem;

public class MiniMapInputReceiver : MonoBehaviour
{
    [SerializeField] private MiniMapManager miniMapManager;
    public bool miniMapLock = false;

    public void OnToggleMap(InputAction.CallbackContext context)
    {
        if(miniMapLock)
            return;

        if (context.performed)
            miniMapManager.ToggleMap();
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
            miniMapManager.OnLeftClick();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
            miniMapManager.OnRightClick();
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        miniMapManager.OnScroll(context.ReadValue<float>());
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        miniMapManager.OnMousePosition(context.ReadValue<Vector2>());
    }

    public void OnAltKey(InputAction.CallbackContext context)
    {
        bool isPressed = context.ReadValue<float>() > 0.5f;
        miniMapManager.OnAltKey(isPressed);
    }
}
