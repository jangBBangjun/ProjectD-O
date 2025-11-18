using UnityEngine;

[ExecuteAlways]
public class Sub_FollowUI : MonoBehaviour
{
    [SerializeField] private Transform target;           // 타겟 캐릭터
    [SerializeField] private Vector3 offset = new Vector3(0, 2.0f, 0); // 머리 위 위치
    [SerializeField] private float sizeMultiplier = 0.01f; // 크기 보정 계수 (튜닝 필요)
    [SerializeField] private Transform[] players = new Transform[5];

    private Camera cam;
    private Vector3 initialScale;

    private void OnEnable()
    {
        cam = Camera.main;
        initialScale = transform.localScale;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        if (cam == null)
            cam = Camera.main;

        // 1. 머리 위 위치 고정
        transform.position = target.position + offset;

        // 2. 카메라를 바라보게 하고 Z축 회전 제거 (기울어짐 방지)
        transform.LookAt(cam.transform);
        Vector3 euler = transform.rotation.eulerAngles;
        euler.z = 0f;
        transform.rotation = Quaternion.Euler(euler);

        // 3. 거리 기반으로 스케일 보정 → 화면상 크기 유지
        float distance = Vector3.Distance(transform.position, cam.transform.position);
        transform.localScale = initialScale * distance * sizeMultiplier;
    }
    public void SetTarget(int index)
    {
        target = players[index];
    }
}