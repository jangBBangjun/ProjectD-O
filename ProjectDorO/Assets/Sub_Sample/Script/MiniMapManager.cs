using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum IconType { Player, Unit, Enemy }

public class MiniMapManager : MonoBehaviour
{
    [Header("필수 참조")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform AllRect;
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private Transform iconParent;
    [SerializeField] private Sprite iconSprite;
    [SerializeField] private float iconSize = 20f;
    [SerializeField] private Vector2 worldSize = new Vector2(500, 500);
    [SerializeField] private Image selectionBoxImage;

    [Header("Fog of War")]
    [SerializeField] private RawImage fogRawImage;
    [SerializeField] private int fogResolution = 256;
    [SerializeField] private float revealRadius = 8f;
    private Texture2D fogTexture;
    private Color32[] fogPixels;

    [Header("배치")]
    [SerializeField] private Transform batchPoint;

    [Header("줌 설정")]
    [SerializeField] private float zoomStep = 50f;
    [SerializeField] private float minWorldSize = 100f;
    [SerializeField] private float maxWorldSize = 1000f;
    private Vector2 originMinimapSize;
    private bool isMiniMapMoving = false;
    private Vector2 moveStartMouseScreenPos;
    private Vector2 moveStartMapPos;

    [Header("유닛 추적")]
    [SerializeField] private List<Target> targets = new List<Target>();
    [SerializeField] private List<Target> selectedTargets = new List<Target>();

    [Header("외부 참조")]
    [SerializeField] private CharacterManager characterManager;

    private Vector2 currentMousePos;
    private float scrollDelta;
    private bool altPressed;

    [Header("유닛 소환")]
    [SerializeField] private Image dropImage;
    [SerializeField] private Sprite[] playerSprites;

    void Start()
    {
        InitFog();
        RenderFog(WorldToFogCoord(batchPoint.position));
        FogTextureUpdate();
        originMinimapSize = minimapRect.sizeDelta;
    }

    void Update()
    {
        MoveIcon();
        UpdateFog();
        HandleZoom();
        HandleMiniMapDrag();
    }

    // -------------------- InputSystem으로부터 연결되는 메서드 --------------------

    public void OnMousePosition(Vector2 pos) => currentMousePos = pos;
    public void OnScroll(float delta) => scrollDelta = delta;
    public void OnAltKey(bool isDown) => altPressed = isDown;

    public void ToggleMap()
    {
        if (!Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OpenMap();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            CloseMap();
        }
    }

    public void OnLeftClick()
    {
        if (altPressed)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(minimapRect, currentMousePos))
            {
                isMiniMapMoving = true;
                moveStartMouseScreenPos = currentMousePos;
                moveStartMapPos = minimapRect.anchoredPosition;
            }
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, currentMousePos, null, out Vector2 dragStart);
            StartCoroutine(HandleSelection(dragStart, currentMousePos));
        }
    }

    public void OnRightClick()
    {
        if (selectedTargets.Count == 0) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minimapRect,
            currentMousePos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint
        );

        foreach (var target in selectedTargets)
        {
            if (target.type != IconType.Enemy)
                target.transform.GetComponent<AiMove>()?.Move(FogCoordToWorld(localPoint));
        }
    }

    // -------------------- 미니맵 UI 동작 --------------------

    public void OpenMap()
    {
        AllRect.anchorMin = new Vector2(0.5f, 0.5f);
        AllRect.anchorMax = new Vector2(0.5f, 0.5f);
        AllRect.anchoredPosition = Vector2.zero;
        AllRect.sizeDelta = new Vector2(1300, 1300);
    }

    public void CloseMap()
    {
        AllRect.anchorMin = new Vector2(0, 1);
        AllRect.anchorMax = new Vector2(0, 1);
        AllRect.anchoredPosition = new Vector2(150, -150);
        AllRect.sizeDelta = new Vector2(300, 300);
        minimapRect.anchoredPosition = Vector2.zero;
        minimapRect.sizeDelta = originMinimapSize;
    }

    private void HandleZoom()
    {
        if (scrollDelta == 0) return;
        if (!RectTransformUtility.RectangleContainsScreenPoint(minimapRect, currentMousePos)) return;

        float newSizeX = Mathf.Clamp(minimapRect.sizeDelta.x + scrollDelta * zoomStep, minWorldSize, maxWorldSize);
        float newSizeY = Mathf.Clamp(minimapRect.sizeDelta.y + scrollDelta * zoomStep, minWorldSize, maxWorldSize);
        minimapRect.sizeDelta = new Vector2(newSizeX, newSizeY);
        scrollDelta = 0;
    }

    private void HandleMiniMapDrag()
    {
        if (!isMiniMapMoving) return;

        Vector2 delta = currentMousePos - moveStartMouseScreenPos;
        minimapRect.anchoredPosition = moveStartMapPos + delta;

        if (!Mouse.current.leftButton.isPressed)
            isMiniMapMoving = false;
    }

    // -------------------- 선택 처리 --------------------

    private IEnumerator HandleSelection(Vector2 dragStart, Vector2 screenClickPos)
    {
        Vector2 dragEnd = dragStart;
        selectionBoxImage.gameObject.SetActive(true);

        while (Mouse.current.leftButton.isPressed)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, currentMousePos, null, out dragEnd);
            Vector2 center = (dragStart + dragEnd) / 2f;
            Vector2 size = new Vector2(Mathf.Abs(dragStart.x - dragEnd.x), Mathf.Abs(dragStart.y - dragEnd.y));
            selectionBoxImage.rectTransform.anchoredPosition = center;
            selectionBoxImage.rectTransform.sizeDelta = size;
            yield return null;
        }

        selectionBoxImage.gameObject.SetActive(false);

        foreach (var target in selectedTargets)
            ResetIconColor(target);

        selectedTargets.Clear();

        float dragDistance = Vector2.Distance(dragStart, dragEnd);

        if (dragDistance < 10f)
        {
            foreach (var target in targets)
            {
                if (target.type == IconType.Enemy) continue;

                if (RectTransformUtility.RectangleContainsScreenPoint(target.rectTransform, screenClickPos))
                {
                    selectedTargets.Add(target);
                    target.image.color = Color.yellow;
                    break;
                }
            }
        }
        else
        {
            Rect selectionRect = new Rect(Vector2.Min(dragStart, dragEnd), Vector2.Max(dragStart, dragEnd) - Vector2.Min(dragStart, dragEnd));

            foreach (var target in targets)
            {
                if (target.type == IconType.Enemy) continue;

                if (selectionRect.Contains(target.rectTransform.anchoredPosition))
                    selectedTargets.Add(target);
            }

            foreach (var t in selectedTargets)
                t.image.color = Color.yellow;
        }
    }



    private void ResetIconColor(Target target)
    {
        switch(target.type)
        {
            case IconType.Player:
                target.image.color = Color.green;
                break;
            case IconType.Unit:
                target.image.color = Color.blue;
                break;
            case IconType.Enemy:
                target.image.color = Color.red;
                break;
            default:
                target.image.color = Color.white;
                break;
        }
    }

    // -------------------- 아이콘 처리 --------------------

    private void MoveIcon()
    {
        foreach (var target in targets)
        {
            if (target.transform == null) continue;

            // 아이콘이 없으면 생성
            if (target.rectTransform == null)
            {
                RectTransform rect = new GameObject("Icon").AddComponent<RectTransform>();
                rect.SetParent(iconParent, false);
                Image img = rect.gameObject.AddComponent<Image>();
                img.sprite = iconSprite;
                rect.sizeDelta = new Vector2(iconSize, iconSize);
                target.rectTransform = rect;
                target.image = img;

                ResetIconColor(target);
            }
            // 아이콘이 있으면 재활용
            else if (!target.image.gameObject.activeSelf)
            {
                target.image.gameObject.SetActive(true);

                ResetIconColor(target);
            }

            // 적 아이콘은 플레이어나 유닛이 근처에 있을 때만 보이도록
            if (target.type == IconType.Enemy)
            {
                bool isVisible = false;

                // 플레이어나 유닛이 적 근처에 있으면 true
                foreach (var t in targets)
                {
                    if (t.type == IconType.Enemy || t.transform == null) continue;

                    float distSqr = (t.transform.position - target.transform.position).sqrMagnitude;
                    if (distSqr <= revealRadius * revealRadius)
                    {
                        isVisible = true;
                        break;
                    }
                }

                target.image.gameObject.SetActive(isVisible);
            }

            // 아이콘 위치 갱신
            Vector3 pos = target.transform.position;
            float xRatio = Mathf.InverseLerp(-worldSize.x / 2, worldSize.x / 2, pos.x) - 0.5f;
            float yRatio = Mathf.InverseLerp(-worldSize.y / 2, worldSize.y / 2, pos.z) - 0.5f;
            target.rectTransform.anchoredPosition = new Vector2(xRatio * minimapRect.rect.size.x, yRatio * minimapRect.rect.size.y);
        }
    }

    // -------------------- Fog 처리 --------------------

    private void InitFog()
    {
        fogTexture = new Texture2D(fogResolution, fogResolution, TextureFormat.RGBA32, false);
        fogPixels = new Color32[fogResolution * fogResolution];
        for (int i = 0; i < fogPixels.Length; i++) fogPixels[i] = new Color32(0, 0, 0, 255);
        fogTexture.SetPixels32(fogPixels);
        fogTexture.Apply();
        fogRawImage.texture = fogTexture;
    }

    private void UpdateFog()
    {
        foreach (var target in targets)
        {
            if (target.type != IconType.Enemy)
                RenderFog(WorldToFogCoord(target.transform.position));
        }

        FogTextureUpdate();
    }

    private void RenderFog(Vector2 center)
    {
        int cx = Mathf.RoundToInt(center.x);
        int cy = Mathf.RoundToInt(center.y);
        int radius = Mathf.RoundToInt(revealRadius * fogResolution / worldSize.x);

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                int px = cx + x;
                int py = cy + y;
                if (px >= 0 && px < fogResolution && py >= 0 && py < fogResolution && x * x + y * y <= radius * radius)
                {
                    int idx = py * fogResolution + px;
                    fogPixels[idx].a = 0;
                }
            }
        }
    }

    private void FogTextureUpdate()
    {
        fogTexture.SetPixels32(fogPixels);
        fogTexture.Apply();
    }

    private Vector2 WorldToFogCoord(Vector3 worldPos)
    {
        float x = Mathf.InverseLerp(-worldSize.x / 2, worldSize.x / 2, worldPos.x);
        float y = Mathf.InverseLerp(-worldSize.y / 2, worldSize.y / 2, worldPos.z);
        return new Vector2(x, y) * fogResolution;
    }

    private Vector3 FogCoordToWorld(Vector2 texPos)
    {
        return new Vector3(
            (texPos.x / minimapRect.rect.size.x) * worldSize.x,
            0,
            (texPos.y / minimapRect.rect.size.y) * worldSize.y
        );
    }

    // -------------------- 유닛 배치 --------------------
    public void PlayerBatchButton(int num)
    {
        dropImage.gameObject.SetActive(true);
        dropImage.sprite = playerSprites[num];
        StartCoroutine(DropImageRender(num));
    }
    private IEnumerator DropImageRender(int num)
    {
        Vector2 localPoint = Vector2.zero;

        while (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPoint = Mouse.current.position.ReadValue();

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(), screenPoint, null, out localPoint))
            {
                dropImage.rectTransform.localPosition = localPoint;
            }

            yield return null;
        }

        Vector3 worldPos = FogCoordToWorld(localPoint - minimapRect.anchoredPosition);
        Vector2 fogPos = WorldToFogCoord(worldPos);
        int x = Mathf.RoundToInt(fogPos.x);
        int y = Mathf.RoundToInt(fogPos.y);

        if (x >= 0 && x < fogResolution && y >= 0 && y < fogResolution)
        {
            int index = y * fogResolution + x;
            if (fogPixels[index].a <= 0)
            {
                characterManager.PlayerBatch(num, worldPos);
            }
            else
            {
                Debug.LogWarning("해당 지역은 시야에 밝혀지지 않았습니다. 유닛 소환 취소.");
            }
        }
        else
        {
            Debug.LogWarning("미니맵 외곽입니다. 유닛 소환 취소.");
        }

        dropImage.gameObject.SetActive(false);
    }

    // -------------------- 내부 클래스 --------------------

    [Serializable]
    private class Target
    {
        public IconType type;
        public Transform transform;
        public Image image;
        public RectTransform rectTransform;

        public Target(IconType type, Transform transform)
        {
            this.type = type;
            this.transform = transform;
        }
    }

    public void AddTarget(IconType type, Transform targetTransform)
    {
        if (targets.Exists(t => t.transform == targetTransform)) return;
        targets.Add(new Target(type, targetTransform));
    }
}
