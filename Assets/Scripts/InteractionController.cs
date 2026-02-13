using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public Transform holdPoint;
    public float detectRadius = 0.8f;
    public float detectDistance = 1.0f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.Space;

    private IInteractable heldItem;
    private PlayerController playerController;
    private CuttingBoardInteractable activeBoardProcessing;
    private bool isPlayerLockedByBoard;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        IInteractable nearest = FindNearestInteractable();
        UpdateBoardProcessingLock();

        if (Input.GetKeyDown(interactKey))
        {
            if (heldItem == null)
            {
                if (TryStartBoardAutoProcess(nearest))
                {
                    return;
                }
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

    private bool TryStartBoardAutoProcess(IInteractable nearest)
    {
        if (heldItem != null)
        {
            return false;
        }

        CuttingBoardInteractable board = ResolveBoard(nearest);
        return StartBoardAutoProcess(board);
    }

    private bool StartBoardAutoProcess(CuttingBoardInteractable board)
    {

        if (board == null)
        {
            return false;
        }

        if (!board.StartProcess())
        {
            return false;
        }

        activeBoardProcessing = board;
        SetPlayerMovementLocked(true);
        isPlayerLockedByBoard = true;
        return true;
    }

    private CuttingBoardInteractable ResolveBoard(IInteractable nearest)
    {
        CuttingBoardInteractable board = nearest as CuttingBoardInteractable;
        if (board == null && nearest != null)
        {
            board = nearest.Root.GetComponentInParent<CuttingBoardInteractable>();
        }

        return board;
    }

    private CookingPot ResolveCookingPot(IInteractable target)
    {
        CookingPot pot = target as CookingPot;
        if (pot == null && target != null)
        {
            pot = target.Root.GetComponentInParent<CookingPot>();
        }

        return pot;
    }

    private void UpdateBoardProcessingLock()
    {
        if (activeBoardProcessing == null)
        {
            if (isPlayerLockedByBoard)
            {
                SetPlayerMovementLocked(false);
                isPlayerLockedByBoard = false;
            }
            return;
        }

        if (activeBoardProcessing.IsProcessing)
        {
            return;
        }

        activeBoardProcessing = null;
        SetPlayerMovementLocked(false);
        isPlayerLockedByBoard = false;
    }

    private void SetPlayerMovementLocked(bool locked)
    {
        if (playerController == null)
        {
            return;
        }

        playerController.SetMovementLocked(locked);
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

        CuttingBoardInteractable board = ResolveBoard(target);
        if (board != null)
        {
            IInteractable picked = board.TakeItem(holdPoint);
            if (picked != null)
            {
                heldItem = picked;
            }
            return;
        }

        CookingPot pot = ResolveCookingPot(target);
        if (pot != null)
        {
            IInteractable picked = pot.TakeItem(holdPoint);
            if (picked != null)
            {
                heldItem = picked;
            }
            return;
        }

        IngredientCrate crate = target as IngredientCrate;
        if (crate != null)
        {
            IInteractable spawned = crate.SpawnItem(holdPoint);
            if (spawned != null)
            {
                heldItem = spawned;
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

            CuttingBoardInteractable board = ResolveBoard(target);
            StartBoardAutoProcess(board);
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

