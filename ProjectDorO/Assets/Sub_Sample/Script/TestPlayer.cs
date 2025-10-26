using UnityEngine;
using UnityEngine.InputSystem;

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
    private Sub_EffectPlayer playerEffect;

    private AiMove ai;

    [SerializeField] private float[] effectDelays = new float[4];

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        ai = GetComponent<AiMove>();
        playerEffect = GetComponent<Sub_EffectPlayer>();
    }

    private void Update()
    {
        if (controller.enabled)
        {
            HandleMovement();
            HandleAttack();
        }
        else
        {
            if (ai.GetIsMoving() == true)
                animator.SetInteger("moveZ", 1);
            else
                animator.SetInteger("moveZ", 0);
        }
    }

    void HandleAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Attack") || stateInfo.IsName("Skill_1") || stateInfo.IsName("Skill_2") || stateInfo.IsName("Skill_3"))
            return;

        if (Mouse.current.leftButton.isPressed)
        {
            playerEffect.Play(0, effectDelays[0]);
            animator.Play("Attack");
        }
        else if (Keyboard.current.qKey.isPressed)
        {
            playerEffect.Play(1, effectDelays[1]);
            animator.Play("Skill_1");
        }
        else if (Keyboard.current.eKey.isPressed)
        {
            playerEffect.Play(2, effectDelays[2]);
            animator.Play("Skill_2");
        }
        else if (Keyboard.current.xKey.isPressed)
        {
            playerEffect.Play(3, effectDelays[3]);
            animator.Play("Skill_3");
        }
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        // ✅ 입력 받기
        Vector2Int moveInput = Vector2Int.zero;
        if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1;

        // ✅ 카메라 기준 방향으로 이동 방향 계산
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        // ✅ 애니메이션 (방향 무시, 걷기/정지만)
        if (animator != null)
        {
            animator.SetInteger("moveZ", moveInput.y);
            animator.SetInteger("moveX", moveInput.x);
        }

        moveDirection = mainCameraTransform.TransformDirection(moveDirection);
        moveDirection.y = 0f;
        moveDirection.Normalize();

        // ✅ 캐릭터 방향 = 카메라 정면으로 고정
        Vector3 camForward = mainCameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        if (camForward.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // ✅ 이동 처리
        if (moveDirection.magnitude > 0.1f)
        {
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        // ✅ 점프
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded && (stateInfo.IsName("Standing Idle") || stateInfo.IsName("Standing Walk Forward") || stateInfo.IsName("Standing Walk Back") || stateInfo.IsName("Standing Walk Right") || stateInfo.IsName("Standing Walk Left")))
        {
            animator.SetTrigger("jump");
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }

        // ✅ 중력 적용
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
