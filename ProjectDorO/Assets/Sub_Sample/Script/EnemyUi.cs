using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [Header("카메라 참조")]
    [Tooltip("기준이 될 3D 카메라. 비워두면 MainCamera 자동 참조.")]
    [SerializeField] private Camera targetCamera;

    [Header("스케일 옵션")]
    [Tooltip("스케일 변화 반응 속도 (값이 클수록 빠르게 반응)")]
    [SerializeField] private float smoothSpeed = 8f;
    [Tooltip("너무 작은 스케일 변화는 무시 (값이 작을수록 민감해짐)")]
    [SerializeField] private float epsilon = 0.01f;

    [Header("UI 참조")]
    [Tooltip("적 UI의 채워지는 이미지")]
    [SerializeField] private Image seeFill;

    // 내부 상태값
    private Vector3 initialScale = Vector3.one;
    private float initialDistance = 1f;
    private float currentScaleMultiplier = 1f;

    private void Start()
    {
        if (!targetCamera)
            targetCamera = Camera.main;

        if (targetCamera)
        {
            initialScale = transform.localScale;
            initialDistance = Vector3.Distance(transform.position, targetCamera.transform.position);
            currentScaleMultiplier = 1f;
        }
    }

    private void LateUpdate()
    {
        if (!targetCamera) return;

        LookAtProjectedLine();
        ScaleRender();
    }

    /// <summary>
    /// 카메라의 X축(오른쪽 방향)을 기준으로 한 직선을 향해 오브젝트를 회전시킨다.
    /// </summary>
    private void LookAtProjectedLine()
    {
        Vector3 cameraRight = targetCamera.transform.right.normalized;
        Vector3 toObject = transform.position - targetCamera.transform.position;

        Vector3 projected = Vector3.Project(toObject, cameraRight);
        Vector3 targetPoint = targetCamera.transform.position + projected;

        Vector3 lookDir = transform.position - targetPoint;

        if (lookDir.sqrMagnitude < 0.0001f)
            return;

        transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
    }

    /// <summary>
    /// 카메라와의 거리 비율에 따라 크기를 유지
    /// </summary>
    private void ScaleRender()
    {
        float currentDistance = Vector3.Distance(transform.position, targetCamera.transform.position);
        if (currentDistance <= 0f) return;

        float targetMultiplier = currentDistance / initialDistance;

        if (Mathf.Abs(currentScaleMultiplier - targetMultiplier) > epsilon)
        {
            currentScaleMultiplier = Mathf.Lerp(currentScaleMultiplier, targetMultiplier, Time.deltaTime * smoothSpeed);
            transform.localScale = initialScale * currentScaleMultiplier;
        }
    }
    /// <summary>
    /// 발견 게이지 설정
    /// </summary>
    public void SetSeeFillAmount(float value, float max)
    {
        seeFill.fillAmount = value / max;
    }
}
