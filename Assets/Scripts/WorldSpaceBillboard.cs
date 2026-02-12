using UnityEngine;

public class WorldSpaceBillboard : MonoBehaviour
{
    public Camera targetCamera;
    public bool lockPitch = false;
    public bool lockRoll = true;
    public bool rotateOnly = true;
    public bool lockScreenSize = true;
    public float referenceDistance = 5f;
    public Vector3 referenceScale = Vector3.one;

    private Vector3 cachedScale;

    private void Awake()
    {
        cachedScale = transform.localScale;
    }

    private void LateUpdate()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
        {
            return;
        }

        Vector3 direction = transform.position - cam.transform.position;
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        // 항상 화면을 바라보도록 카메라 회전을 그대로 사용
        Quaternion lookRotation = cam.transform.rotation;
        Vector3 euler = lookRotation.eulerAngles;

        if (lockPitch)
        {
            euler.x = 0f;
        }

        if (lockRoll)
        {
            euler.z = 0f;
        }

        transform.rotation = Quaternion.Euler(euler);

        if (rotateOnly)
        {
            // 회전만 적용하고 크기는 고정
            transform.localScale = cachedScale;
            return;
        }

        if (lockScreenSize)
        {
            float distance = Vector3.Distance(cam.transform.position, transform.position);
            float scaleFactor = referenceDistance > 0f ? distance / referenceDistance : 1f;
            // 화면에서 일정한 크기로 보이도록 거리 비례 스케일 적용
            Vector3 parentScale = GetParentScale();
            Vector3 invParentScale = new Vector3(
                parentScale.x != 0f ? 1f / parentScale.x : 1f,
                parentScale.y != 0f ? 1f / parentScale.y : 1f,
                parentScale.z != 0f ? 1f / parentScale.z : 1f
            );
            transform.localScale = Vector3.Scale(referenceScale * scaleFactor, invParentScale);
        }
    }

    private void Reset()
    {
        referenceScale = transform.localScale;
        cachedScale = transform.localScale;
    }

    private Vector3 GetParentScale()
    {
        if (transform.parent == null)
        {
            return Vector3.one;
        }

        return transform.parent.lossyScale;
    }
}

