using UnityEngine;
using UnityEngine.InputSystem; // Input System 직접 사용

[RequireComponent(typeof(CharacterController))]
public class TestPlayer : MonoBehaviour
{
    [Header("플레이어 설정")]
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 700.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("카메라")]
    public Transform mainCameraTransform;

    [Header("미니맵")]
    public MiniMapManager miniMap;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;

    private Animator animator;
    private readonly int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

    private AiMove ai;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        ai = GetComponent<AiMove>();
    }

    private void Update()
    {
        if (controller.enabled)
            HandleMovement();
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        // ✅ 키보드 입력 → 방향 계산
        Vector2 moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1;

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        // ✅ 애니메이션
        if (animator != null)
        {
            animator.SetFloat(hashMoveSpeed, moveDirection.magnitude);
        }

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + mainCameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Vector3 moveVector = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveVector.normalized * moveSpeed * Time.deltaTime);
        }

        // ✅ 점프 (Space 키)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }

        // ✅ 중력 처리
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void UseAI()
    {
        ai.Move(ai.transform.position);
        controller.enabled = false;
        ai.enabled = true;
    }

    public void StopAI()
    {
        controller.enabled = true;
        ai.enabled = false;
    }
}
