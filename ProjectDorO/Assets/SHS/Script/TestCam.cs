using UnityEngine;
using UnityEngine.InputSystem; // ✅ Input System 네임스페이스 추가

public class TestCam : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;

    [Header("카메라 타겟")]
    [Tooltip("카메라가 따라다닐 대상 (플레이어)")]
    public Transform target;

    [Header("카메라 설정")]
    [Tooltip("타겟과의 기본 거리")]
    public float distance = 5.0f;
    [Tooltip("카메라의 Y축 높이")]
    public float height = 2.0f;
    [Tooltip("카메라 회전의 부드러움 정도")]
    public float rotationDamping = 10.0f;
    [Tooltip("마우스 X축 감도")]
    public float mouseXSensitivity = 100.0f;
    [Tooltip("마우스 Y축 감도")]
    public float mouseYSensitivity = 80.0f;

    [Tooltip("카메라의 최소/최대 Y각도")]
    public float minYAngle = -20.0f;
    public float maxYAngle = 80.0f;

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // ✅ 마우스 입력을 Input System으로 받기
        Vector2 mouseDelta = Vector2.zero;

        if (Mouse.current != null)
            mouseDelta = Mouse.current.delta.ReadValue(); // 현재 프레임의 마우스 이동값

        // ✅ 마우스 입력 반영
        currentX += mouseDelta.x * mouseXSensitivity * Time.deltaTime;
        currentY -= mouseDelta.y * mouseYSensitivity * Time.deltaTime;

        // Y 각도(상하 회전) 제한
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);

        // 목표 회전 계산
        Quaternion targetRotation = Quaternion.Euler(currentY, currentX, 0);

        // 카메라 위치 계산
        Vector3 targetPosition = target.position - (targetRotation * Vector3.forward * distance) + new Vector3(0, height, 0);

        // 부드럽게 회전/이동
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationDamping * Time.deltaTime);
        transform.position = targetPosition;

        // 타겟 바라보기
        transform.LookAt(target.position + Vector3.up * height * 0.5f);
    }

    public void SetTarget(Transform _target)
    {
        if (target != null)
        {
            var prevPlayer = target.GetComponent<TestPlayer>();
            if (prevPlayer != null)
                prevPlayer.UseAI();
        }

        target = _target;

        var newPlayer = target.GetComponent<TestPlayer>();
        if (newPlayer != null)
            newPlayer.StopAI();
    }
}