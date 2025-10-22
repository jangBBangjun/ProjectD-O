﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum IconType { Player, Unit, Enemy }

public class MiniMapManager : MonoBehaviour
{
    private CharacterManager characterManager;

    [Header("UI 참조")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform AllRect;
    [SerializeField] private RectTransform moveRect;

    [Header("아이콘")]
    [SerializeField] private Transform iconParent;
    [SerializeField] private Sprite iconSprite;
    [SerializeField] private float iconSize = 20f;

    [Header("월드 좌표")]
    [SerializeField] private Vector2 worldCenter = Vector2.zero;
    [SerializeField] private Vector2 worldSize = new Vector2(500, 500);
    [SerializeField] private Image selectionBoxImage;

    [Header("추적")]
    [SerializeField] private Transform target;
    [SerializeField] private Camera minimapCamera;

    [Header("시작 배치 가능 영역")]
    [SerializeField] private Transform batchPoint;

    [Header("유닛 배치 기능")]
    [SerializeField] private Image dropImage;
    [SerializeField] private Sprite[] playerSprites;

    [Header("시야 확장 기능")]
    [SerializeField] private RawImage fogRawImage;
    [SerializeField] private int fogResolution = 256;
    [SerializeField] private float revealRadius = 8f;
    private Texture2D fogTexture;
    private Color32[] fogPixels;

    [Header("줌 기능")]
    [SerializeField] private float zoomStep = 50f;
    [SerializeField] private float minWorldSize = 100f;
    [SerializeField] private float maxWorldSize = 1000f;

    [Header("유닛 추적 기능")]
    [SerializeField] private List<Target> targets = new List<Target>();
    [SerializeField] private List<Target> selectedTargets = new List<Target>();

    private bool isMiniMapMoving = false;
    private bool altPressed;
    private float scrollDelta;
    private Vector2 originMinimapSize;
    private Vector2 moveStartMouseScreenPos;
    private Vector2 moveStartMapPos;
    private Vector2 currentMousePos;

    private void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
    }

    private void Start()
    {
        InitFog();
        RenderFog(WorldToFogCoord(batchPoint.position));
        FogTextureUpdate();
        originMinimapSize = moveRect.sizeDelta;

        CloseMap();
    }

    private void Update()
    {
        MoveIcon();
        UpdateFog();
    }

    private IEnumerator HandleMiniMap()
    {
        while (true)
        {
            HandleZoom();
            HandleMiniMapDrag();

            yield return null;
        }
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
            if (RectTransformUtility.RectangleContainsScreenPoint(moveRect, currentMousePos))
            {
                isMiniMapMoving = true;
                moveStartMouseScreenPos = currentMousePos;
                moveStartMapPos = moveRect.anchoredPosition;
            }
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(moveRect, currentMousePos, null, out Vector2 dragStart);
            StartCoroutine(HandleSelection(dragStart, currentMousePos));
        }
    }

    public void OnRightClick()
    {
        PlayerMoveAI();
    }

    // -------------------- 플레이어와 상호작용 --------------------

    private void PlayerMoveAI()
    {
        if (selectedTargets.Count == 0) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            moveRect,
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

        TargetCenter();
        StartCoroutine(nameof(HandleMiniMap));
        StopCoroutine(nameof(UpdateTargetCenter));
    }

    public void CloseMap()
    {
        AllRect.anchorMin = new Vector2(0, 1);
        AllRect.anchorMax = new Vector2(0, 1);
        AllRect.anchoredPosition = new Vector2(150, -150);
        AllRect.sizeDelta = new Vector2(300, 300);
        moveRect.sizeDelta = originMinimapSize;

        TargetCenter();
        StopCoroutine(nameof(HandleMiniMap));
        StartCoroutine(nameof(UpdateTargetCenter));
    }

    private void HandleZoom()
    {
        if (scrollDelta == 0) return;
        if (!RectTransformUtility.RectangleContainsScreenPoint(moveRect, currentMousePos)) return;

        float newSizeX = Mathf.Clamp(moveRect.sizeDelta.x + scrollDelta * zoomStep, minWorldSize, maxWorldSize);
        float newSizeY = Mathf.Clamp(moveRect.sizeDelta.y + scrollDelta * zoomStep, minWorldSize, maxWorldSize);
        moveRect.sizeDelta = new Vector2(newSizeX, newSizeY);
        scrollDelta = 0;
    }

    private void HandleMiniMapDrag()
    {
        if (!isMiniMapMoving) return;

        Vector2 delta = currentMousePos - moveStartMouseScreenPos;
        moveRect.anchoredPosition = moveStartMapPos + delta;

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
            RectTransformUtility.ScreenPointToLocalPointInRectangle(moveRect, currentMousePos, null, out dragEnd);
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
        switch (target.type)
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

            float xNorm = Mathf.InverseLerp(worldCenter.x - worldSize.x / 2, worldCenter.x + worldSize.x / 2, pos.x);
            float yNorm = Mathf.InverseLerp(worldCenter.y - worldSize.y / 2, worldCenter.y + worldSize.y / 2, pos.z);

            Vector2 minimapSize = moveRect.rect.size;
            float xPos = (xNorm - 0.5f) * minimapSize.x;
            float yPos = (yNorm - 0.5f) * minimapSize.y;

            target.rectTransform.anchoredPosition = new Vector2(xPos, yPos);
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
        float x = Mathf.InverseLerp(
            worldCenter.x - worldSize.x / 2, worldCenter.x + worldSize.x / 2, worldPos.x);
        float y = Mathf.InverseLerp(
            worldCenter.y - worldSize.y / 2, worldCenter.y + worldSize.y / 2, worldPos.z);
        return new Vector2(x, y) * fogResolution;
    }

    private Vector3 FogCoordToWorld(Vector2 minimapLocalPoint)
    {
        Vector2 minimapSize = moveRect.rect.size;
        float normX = (minimapLocalPoint.x / minimapSize.x) + 0.5f;
        float normY = (minimapLocalPoint.y / minimapSize.y) + 0.5f;

        return new Vector3(
            worldCenter.x - worldSize.x / 2 + normX * worldSize.x,
            0,
            worldCenter.y - worldSize.y / 2 + normY * worldSize.y
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
        Vector2 canvasLocalPoint = Vector2.zero;
        Vector2 minimapLocalPoint = Vector2.zero;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        while (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPoint = Mouse.current.position.ReadValue();

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPoint,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out canvasLocalPoint))
            {
                dropImage.rectTransform.localPosition = canvasLocalPoint;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                moveRect,
                screenPoint,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out minimapLocalPoint
            );

            yield return null;
        }

        Vector2 finalScreenPoint = Mouse.current.position.ReadValue();
        bool isInsideMinimap = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            moveRect,
            finalScreenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out minimapLocalPoint
        );

        if (!isInsideMinimap)
        {
            Debug.LogWarning("미니맵 외곽입니다. 유닛 소환 취소.");
            dropImage.gameObject.SetActive(false);
            yield break;
        }

        Vector3 worldPos = FogCoordToWorld(minimapLocalPoint);
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

    // -------------------- 선택된 플레이어 추적 --------------------

    private IEnumerator UpdateTargetCenter()
    {
        while (true)
        {
            TargetCenter();

            yield return null;
        }
    }
    private void TargetCenter()
    {
        if (target == null)
        {
            moveRect.anchoredPosition = Vector2.zero;
            return;
        }

        // 월드 좌표 → 정규화 (0~1)
        float xNorm = Mathf.InverseLerp(
            worldCenter.x - worldSize.x / 2, worldCenter.x + worldSize.x / 2,
            target.position.x
        );

        float yNorm = Mathf.InverseLerp(
            worldCenter.y - worldSize.y / 2, worldCenter.y + worldSize.y / 2,
            target.position.z
        );

        // 정규화 → 미니맵 로컬 좌표로 변환 (중심 기준)
        Vector2 minimapSize = moveRect.rect.size;
        float xOffset = (xNorm - 0.5f) * minimapSize.x;
        float yOffset = (yNorm - 0.5f) * minimapSize.y;

        // 미니맵 전체를 반대로 이동시켜 target이 중앙으로 오게 함
        moveRect.anchoredPosition = -new Vector2(xOffset, yOffset);
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
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

    // -------------------- 기즈모 --------------------

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(new Vector3(worldCenter.x, 0, worldCenter.y), new Vector3(worldSize.x, 0, worldSize.y));
    }
}