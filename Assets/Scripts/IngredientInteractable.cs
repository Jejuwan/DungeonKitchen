using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class IngredientInteractable : MonoBehaviour, IInteractable, IProcessable
{
    public IngredientType ingredientType;
    public GameObject processedPrefab;
    public GameObject cookedPrefab;
    public GameObject burntPrefab;
    public bool isProcessed = false;
    public bool isCooked = false;
    public bool isBurnt = false;

    public Transform Root => transform;
    public bool CanPickUp => true;
    public bool CanPlace => false;
    public GameObject ProcessedPrefab => processedPrefab;

    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPoint)
    {
        if (holdPoint == null)
        {
            return;
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        col.enabled = false;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop(Vector3 dropPosition)
    {
        transform.SetParent(null);
        if (col != null)
        {
            col.enabled = true;
        }

        float bottomOffset = 0f;
        if (col != null)
        {
            // 현재 콜라이더 기준으로 바닥까지의 오프셋 계산
            bottomOffset = transform.position.y - col.bounds.min.y;
        }

        transform.position = GetGroundedPosition(dropPosition, bottomOffset, col);

        rb.isKinematic = false;
    }

    public bool Place(IInteractable item)
    {
        return false;
    }

    private Vector3 GetGroundedPosition(Vector3 desiredPosition, float bottomOffset, Collider self)
    {
        Vector3 origin = desiredPosition + Vector3.up * 1.0f;
        RaycastHit[] hits = Physics.RaycastAll(origin, Vector3.down, 5f, ~0, QueryTriggerInteraction.Ignore);
        float nearest = float.MaxValue;
        RaycastHit bestHit = default;
        bool found = false;

        for (int i = 0; i < hits.Length; i++)
        {
            if (self != null && hits[i].collider == self)
            {
                // 자기 자신 콜라이더는 바닥 판정에서 제외
                continue;
            }

            if (hits[i].distance < nearest)
            {
                nearest = hits[i].distance;
                bestHit = hits[i];
                found = true;
            }
        }

        if (found)
        {
            return bestHit.point + Vector3.up * Mathf.Max(0f, bottomOffset);
        }

        return desiredPosition;
    }
}

