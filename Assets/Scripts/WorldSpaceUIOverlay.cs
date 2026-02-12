using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class WorldSpaceUIOverlay : MonoBehaviour
{
    public Camera targetCamera;
    public Transform target;
    public Vector3 worldOffset = new Vector3(0f, 1.2f, 0f);
    public bool clampToScreen = true;
    public Canvas canvas;
    public RectTransform referenceRect;
    public bool useCanvasAsReference = true;
    public bool forceCenterAnchor = true;

    private RectTransform rect;
    private RectTransform canvasRect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        if (referenceRect == null)
        {
            referenceRect = rect.parent as RectTransform;
        }

        if (forceCenterAnchor && rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    private void LateUpdate()
    {
        if (target == null || rect == null)
        {
            return;
        }

        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
        {
            return;
        }

        Vector3 worldPos = target.position + worldOffset;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        if (screenPos.z < 0f)
        {
            // 카메라 뒤에 있을 때는 숨김
            rect.gameObject.SetActive(false);
            return;
        }

        rect.gameObject.SetActive(true);

        if (clampToScreen)
        {
            screenPos.x = Mathf.Clamp(screenPos.x, 0f, Screen.width);
            screenPos.y = Mathf.Clamp(screenPos.y, 0f, Screen.height);
        }

        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // 오버레이는 스크린 좌표를 그대로 사용
            rect.position = new Vector3(screenPos.x, screenPos.y, 0f);
            rect.localRotation = Quaternion.identity;
        }
        else
        {
            RectTransform targetRect = useCanvasAsReference ? canvasRect : referenceRect;
            if (targetRect == null)
            {
                targetRect = referenceRect != null ? referenceRect : canvasRect;
            }
            if (targetRect != null)
            {
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    targetRect,
                    screenPos,
                    canvas != null ? canvas.worldCamera : cam,
                    out localPoint
                );
                rect.anchoredPosition = localPoint;
            }
            else
            {
                rect.position = screenPos;
            }
        }
    }
}

