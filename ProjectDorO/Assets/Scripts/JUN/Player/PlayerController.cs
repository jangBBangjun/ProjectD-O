using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerInputReader input; // 인풋을 직접 읽지 않고 Reader에게 받는다
    public CharacterController controller;
    public Transform cameraTransform;
    public ThirdPersonCamera cameraController;

    [Header("Movement")]
    public float speed = 5f;
    public float rotationSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 5f;
    public float gravity = -20f;

    private float verticalVelocity;
    private bool isGrounded;

    [Header("Jump Assist")]
    [SerializeField] float jumpBufferTime = 0.15f;
    [SerializeField] float coyoteTime = 0.1f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    float jumpBufferCounter;
    float coyoteCounter;

    private ICharacterCombat combat;
    private Vector3 velocity;

    private void Awake()
    {
        combat = GetComponent<ICharacterCombat>();
    }
    private void Update()
    {
        if (!enabled) return;

        HandleRotation();
        HandleMovementAndJump();
        HandleActions();

    }
    private void HandleMovementAndJump()
    {
        // Ground 상태 갱신
        if (controller.isGrounded)
        {
            coyoteCounter = coyoteTime;

            if (verticalVelocity < 0)
                verticalVelocity = -2f;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        // 점프 입력 버퍼
        if (input.JumpTriggered)
        {
            jumpBufferCounter = jumpBufferTime;
            input.ConsumeJump();
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // 점프 판정
        bool jumpedThisFrame = false;

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
            jumpedThisFrame = true;
        }

        if (jumpedThisFrame)
        {
            animator.SetTrigger("jump");
        }

        // 중력
        verticalVelocity += gravity * Time.deltaTime;

        // 이동
        Vector2 moveInput = input.Move;
        Quaternion yawRotation = Quaternion.Euler(0, cameraController.CurrentYaw, 0);
        Vector3 moveDir = yawRotation * new Vector3(moveInput.x, 0, moveInput.y);

        Vector3 finalMove =
            moveDir * speed +
            Vector3.up * verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);

        UpdateMoveAnimation(moveInput);
    }
    private void UpdateMoveAnimation(Vector2 moveInput)
    {
        int moveX = 0;
        int moveZ = 0;

        if (moveInput.x > 0.1f) moveX = 1;
        else if (moveInput.x < -0.1f) moveX = -1;

        if (moveInput.y > 0.1f) moveZ = 1;
        else if (moveInput.y < -0.1f) moveZ = -1;

        animator.SetInteger("moveX", moveX);
        animator.SetInteger("moveZ", moveZ);
    }
    private void HandleRotation()
    {
        float yaw = cameraController.CurrentYaw;

        Quaternion targetRotation = Quaternion.Euler(0, yaw, 0);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }


    private void HandleActions()
    {
        if (input.AttackTriggered)
            combat.BasicAttack();

        else if (input.Skill1Triggered)
            combat.Skill1();

        else if (input.Skill2Triggered)
            combat.Skill2();

        else if (input.Skill3Triggered)
            combat.Ultimate();
    }

}
