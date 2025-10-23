using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -4);

    [Header("Camera Settings")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float minYAngle = -20f;
    [SerializeField] private float maxYAngle = 60f;
    [SerializeField] private float smoothSpeed = 10f;

    private InputSystem_Actions inputActions;
    private Vector2 lookInput;
    private float yaw;
    private float pitch;

    public float CurrentYaw => yaw;   // 다른 스크립트에서 접근할 수 있도록 (읽기 전용)
    public float CurrentPitch => pitch;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        inputActions.Player.Look.Enable();
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;
    }

    private void OnDisable()
    {
        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Look.canceled -= OnLook;
        inputActions.Player.Look.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        // 회전 계산
        yaw += lookInput.x * sensitivity * Time.deltaTime;
        pitch -= lookInput.y * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

        // 카메라 위치 및 회전
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}