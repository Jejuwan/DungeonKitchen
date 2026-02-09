using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public Transform holdPoint;
    public float detectRadius = 0.8f;
    public float detectDistance = 1.0f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.Space;

    private IInteractable heldItem;

    private void Update()
    {
        IInteractable nearest = FindNearestInteractable();

        HandleHoldInteraction(nearest);

        if (Input.GetKeyDown(interactKey))
        {
            if (heldItem == null)
            {
                TryPickUp(nearest);
            }
            else
            {
                if (!TryPlace(nearest))
                {
                    DropHeldItem();
                }
            }
        }
    }

    private void HandleHoldInteraction(IInteractable nearest)
    {
        if (heldItem != null)
        {
            return;
        }

        CuttingBoardInteractable board = nearest as CuttingBoardInteractable;
        if (board == null && nearest != null)
        {
            board = nearest.Root.GetComponentInParent<CuttingBoardInteractable>();
        }

        if (board == null)
        {
            return;
        }

        if (Input.GetKey(interactKey))
        {
            board.StartProcess();
        }

        if (Input.GetKeyUp(interactKey))
        {
            board.StopProcess();
        }
    }

    private IInteractable FindNearestInteractable()
    {
        Vector3 origin = transform.position + transform.forward * detectDistance;
        Collider[] hits = Physics.OverlapSphere(origin, detectRadius, interactableLayer);

        IInteractable nearest = null;
        float nearestDist = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            IInteractable interactable = hits[i].GetComponentInParent<IInteractable>();
            if (interactable == null)
            {
                continue;
            }
            if (heldItem != null && interactable.Root == heldItem.Root)
            {
                // 들고 있는 아이템은 상호작용 대상에서 제외
                continue;
            }

            float dist = Vector3.SqrMagnitude(interactable.Root.position - transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = interactable;
            }
        }

        return nearest;
    }

    private void TryPickUp(IInteractable target)
    {
        if (target == null || !target.CanPickUp || holdPoint == null)
        {
            return;
        }

        CuttingBoardInteractable board = target as CuttingBoardInteractable;
        if (board != null)
        {
            IInteractable picked = board.TakeItem(holdPoint);
            if (picked != null)
            {
                heldItem = picked;
            }
            return;
        }

        CookingPot pot = target as CookingPot;
        if (pot != null)
        {
            IInteractable picked = pot.TakeItem(holdPoint);
            if (picked != null)
            {
                heldItem = picked;
            }
            return;
        }

        target.PickUp(holdPoint);
        heldItem = target;
    }

    private bool TryPlace(IInteractable target)
    {
        if (target == null || !target.CanPlace || heldItem == null)
        {
            return false;
        }

        bool placed = target.Place(heldItem);
        if (placed)
        {
            heldItem = null;
        }

        return placed;
    }

    private void DropHeldItem()
    {
        if (heldItem == null)
        {
            return;
        }

        Vector3 dropPos = transform.position + transform.forward * 1.0f;
        heldItem.Drop(dropPos);
        heldItem = null;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + transform.forward * detectDistance;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin, detectRadius);
    }
}


