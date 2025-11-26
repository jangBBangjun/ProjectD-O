using UnityEngine;

public class Sub_WorldHpPool : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Canvas canvas;

    [Header("선택된 플레이어")]
    [SerializeField] private Rigidbody targetPlayer;
    [SerializeField] private RectTransform mainBarPanel;
    [SerializeField] private Vector3 mainOffset = new Vector3(0, 2.0f, 0);
    [SerializeField] private Rigidbody[] players = new Rigidbody[5];

    [Header("UI 이동 설정")]
    [SerializeField] private float lerpSpeed = 10f; // 🔧 부드럽게 따라오는 속도 (사용자 조정 가능)

    private Vector2 currentUIPosition;

    private void Start()
    {
        if (mainBarPanel != null)
            currentUIPosition = mainBarPanel.anchoredPosition;
    }

    private void Update()
    {
        UpdateUIPosition(targetPlayer, mainOffset);
    }

    public void SetTarget(int index)
    {
        if (index < 0 || index >= players.Length || players[index] == null)
            return;

        targetPlayer = players[index];
        mainBarPanel.gameObject.SetActive(true);
    }

    private void UpdateUIPosition(Rigidbody target, Vector3 offset)
    {
        if (target == null || worldCamera == null || mainBarPanel == null)
            return;

        Vector3 worldPos = target.position + offset;
        Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);

        if (screenPos.z < 0.01f)
        {
            mainBarPanel.gameObject.SetActive(false);
            return;
        }

        if (!mainBarPanel.gameObject.activeSelf)
            mainBarPanel.gameObject.SetActive(true);

        // 스크린 좌표 → UI 로컬 좌표 (Canvas가 Overlay라면 camera == null)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            null,
            out Vector2 targetUIPos);

        // UI 위치를 부드럽게 보간       
        currentUIPosition = Vector2.Lerp(currentUIPosition, targetUIPos, Time.deltaTime * lerpSpeed);
        mainBarPanel.anchoredPosition = currentUIPosition;
    }
}
