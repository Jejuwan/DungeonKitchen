using UnityEngine;

public class PotInteractable : MonoBehaviour, IInteractable
{
    public Transform placePoint;

    public Transform Root => transform;
    public bool CanPickUp => false;
    public bool CanPlace => true;

    private IInteractable currentItem;

    public void PickUp(Transform holdPoint)
    {
        // 냄비는 집을 수 없음
    }

    public void Drop(Vector3 dropPosition)
    {
        // 냄비는 내려놓을 수 없음
    }

    public bool Place(IInteractable item)
    {
        if (item == null || currentItem != null || placePoint == null)
        {
            return false;
        }

        item.PickUp(placePoint);
        currentItem = item;
        return true;
    }
}




