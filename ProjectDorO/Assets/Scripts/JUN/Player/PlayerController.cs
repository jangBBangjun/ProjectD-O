using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerInputReader input; // 인풋을 직접 읽지 않고 Reader에게 받는다
    public CharacterController controller;
    public Transform cameraTransform;
    public BaseAttackController attackController;
    public BaseAttackController skill1Controller;
    public BaseAttackController skill2Controller;
    public BaseAttackController skill3Controller;

    [Header("Movement")]
    public float speed = 5f;
    public float rotationSpeed = 10f;

    private Vector3 velocity;

    private void Update()
    {
        HandleMovement();
        HandleLook();
        HandleActions();
    }

    private void HandleMovement()
    {
        Vector2 move = input.Move;
        Vector3 direction = new Vector3(move.x, 0, move.y);

        if (direction.sqrMagnitude > 0.01f)
        {
            // 카메라 기준으로 방향 변환
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = cameraTransform.right;
            camRight.y = 0;
            camRight.Normalize();

            Vector3 moveDir = camForward * direction.z + camRight * direction.x;

            // 이동
            controller.Move(moveDir * speed * Time.deltaTime);

            // 캐릭터 방향 회전
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void HandleLook()
    {
        // 카메라 컨트롤러에서 처리할 수도 있음
        // 지금은 Character 회전과 분리해둠
    }

    private void HandleActions()
    {
        if (input.AttackTriggered)
            attackController?.Attack();

        if (input.Skill1Triggered)
            skill1Controller?.Attack();

        if (input.Skill2Triggered)
            skill2Controller?.Attack();

        if (input.Skill3Triggered)
            skill3Controller?.Attack();
    }
}
