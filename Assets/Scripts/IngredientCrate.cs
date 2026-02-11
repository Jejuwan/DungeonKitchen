using UnityEngine;

public enum IngredientType
{
    Meat,
    Fish,
    Potato,
    Onion,
    Tomato,
    Herb
}

public class IngredientCrate : MonoBehaviour, IInteractable
{
    public IngredientType crateType;
    public GameObject ingredientPrefab;

    public Transform Root => transform;
    public bool CanPickUp => true;
    public bool CanPlace => false;

    public void PickUp(Transform holdPoint)
    {
        SpawnItem(holdPoint);
    }

    public IInteractable SpawnItem(Transform holdPoint)
    {
        if (holdPoint == null || ingredientPrefab == null)
        {
            return null;
        }

        GameObject newItem = Instantiate(ingredientPrefab);
        IInteractable item = newItem.GetComponent<IInteractable>();
        if (item == null)
        {
            item = newItem.GetComponentInChildren<IInteractable>();
        }

        if (item != null)
        {
            item.PickUp(holdPoint);
            return item;
        }

        // IInteractable이 없으면 기본적으로 손 위치에 붙임
        newItem.transform.SetParent(holdPoint);
        newItem.transform.localPosition = Vector3.zero;
        newItem.transform.localRotation = Quaternion.identity;
        return null;
    }

    public void Drop(Vector3 dropPosition)
    {
        // 보관함은 내려놓을 수 없음
    }

    public bool Place(IInteractable item)
    {
        return false;
    }
}

