using UnityEngine;

public interface IInteractable
{
    Transform Root { get; }
    bool CanPickUp { get; }
    bool CanPlace { get; }

    void PickUp(Transform holdPoint);
    void Drop(Vector3 dropPosition);
    bool Place(IInteractable item);
}

public interface IProcessable
{
    GameObject ProcessedPrefab { get; }
}

