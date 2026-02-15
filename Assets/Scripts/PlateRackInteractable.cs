using UnityEngine;

public class PlateRackInteractable : MonoBehaviour, IInteractable
{
    public GameObject platePrefab;

    public Transform Root => transform;
    public bool CanPickUp => true;
    public bool CanPlace => false;

    public void PickUp(Transform holdPoint)
    {
        SpawnPlate(holdPoint);
    }

    public void Drop(Vector3 dropPosition)
    {
        // 거치대는 내려놓을 수 없음
    }

    public bool Place(IInteractable item)
    {
        return false;
    }

    public IInteractable SpawnPlate(Transform holdPoint)
    {
        if (holdPoint == null || platePrefab == null)
        {
            return null;
        }

        GameObject newPlate = Instantiate(platePrefab);
        IInteractable interactable = newPlate.GetComponent<IInteractable>();
        if (interactable == null)
        {
            interactable = newPlate.GetComponentInChildren<IInteractable>();
        }

        if (interactable != null)
        {
            interactable.PickUp(holdPoint);
            return interactable;
        }

        // IInteractable이 없으면 기본적으로 손 위치에 붙임
        newPlate.transform.SetParent(holdPoint);
        newPlate.transform.localPosition = Vector3.zero;
        newPlate.transform.localRotation = Quaternion.identity;
        return null;
    }
}
