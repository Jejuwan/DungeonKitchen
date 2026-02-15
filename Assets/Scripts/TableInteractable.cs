using UnityEngine;

public class TableInteractable : MonoBehaviour, IInteractable
{
    public Transform placePoint;

    public Transform Root => transform;
    public bool CanPickUp => GetCurrentItem() != null;
    public bool CanPlace => true;

    private IInteractable currentItem;

    public void PickUp(Transform holdPoint)
    {
        TakeItem(holdPoint);
    }

    public void Drop(Vector3 dropPosition)
    {
        // 테이블은 내려놓을 수 없음
    }

    public bool Place(IInteractable item)
    {
        if (item == null || item == this || placePoint == null)
        {
            return false;
        }

        PlateInteractable currentPlate = GetCurrentItem() as PlateInteractable;
        if (currentPlate != null)
        {
            // 테이블 위 접시에만 재료를 추가할 수 있음
            return currentPlate.TryAddIngredient(item);
        }

        if (item is IngredientInteractable)
        {
            // 재료는 접시 없는 테이블에 직접 올릴 수 없음
            return false;
        }

        if (currentItem != null)
        {
            return false;
        }

        item.PickUp(placePoint);
        currentItem = item;
        return true;
    }

    public IInteractable TakeItem(Transform holdPoint)
    {
        if (GetCurrentItem() == null || holdPoint == null)
        {
            return null;
        }

        IInteractable item = currentItem;
        item.PickUp(holdPoint);
        currentItem = null;
        return item;
    }

    private IInteractable GetCurrentItem()
    {
        if (currentItem == null && placePoint != null && placePoint.childCount > 0)
        {
            // 테이블의 직접 자식만 현재 아이템으로 인정
            Transform directChild = placePoint.GetChild(0);
            currentItem = directChild.GetComponent<IInteractable>();
            if (currentItem == null)
            {
                currentItem = directChild.GetComponentInChildren<IInteractable>();
            }
        }

        return currentItem;
    }
}
