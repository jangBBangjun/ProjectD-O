using System;
using UnityEngine;

public class Sub_WorldHp : MonoBehaviour
{
    [Serializable]
    private class PlayerInfo
    {
        public Rigidbody rigid;
        public Vector3 offset = new Vector3(0, 2.0f, 0);
    }

    [Header("기본 설정")]
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Canvas canvas;

    [Header("선택된 플레이어")]
    [SerializeField] private PlayerInfo targetPlayer;
    [SerializeField] private RectTransform mainBarPanel;
    [SerializeField] private PlayerInfo[] players = new PlayerInfo[5];

    [Header("UI 이동 설정")]
    [SerializeField] private float lerpSpeed = 10f;

    private Vector2 currentUIPosition;

    private void Start()
    {
        if (mainBarPanel != null)
            currentUIPosition = mainBarPanel.anchoredPosition;
    }

    private void Update()
    {
        if (targetPlayer.rigid == null)
            return;

        UpdateUIPosition(targetPlayer);
    }

    public void SetTarget(int index)
    {
        if (index < 0 || index >= players.Length || players[index] == null)
            return;

        targetPlayer = players[index];
        mainBarPanel.gameObject.SetActive(true);
    }

    private void UpdateUIPosition(PlayerInfo target)
    {
        if (worldCamera == null || mainBarPanel == null)
            return;

        Vector3 worldPos = target.rigid.position + target.offset;
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
