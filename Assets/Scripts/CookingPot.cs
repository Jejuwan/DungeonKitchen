using UnityEngine;
using UnityEngine.UI;

public class CookingPot : MonoBehaviour, IInteractable
{
    public Transform placePoint;
    public float cookDuration = 5f;
    public float burnDelay = 6f;
    public Image progressBar;

    public Transform Root => transform;
    public bool CanPickUp => GetCurrentItem() != null && !isCooking;
    public bool CanPlace => true;

    private IInteractable currentItem;
    private bool isCooking;
    private bool isCookedReady;
    private float cookTimer;
    private float burnTimer;

    private void Update()
    {
        if (currentItem == null)
        {
            StopAll();
            SetProgressVisible(false);
            return;
        }

        if (isCooking)
        {
            cookTimer += Time.deltaTime;
            UpdateProgressBar(cookTimer / Mathf.Max(cookDuration, 0.01f));

            if (cookTimer >= cookDuration)
            {
                CompleteCooking();
            }
        }
        else if (isCookedReady)
        {
            burnTimer += Time.deltaTime;
            if (burnTimer >= burnDelay)
            {
                BurnFood();
            }
        }
    }

    public void PickUp(Transform holdPoint)
    {
        TakeItem(holdPoint);
    }

    public void Drop(Vector3 dropPosition)
    {
        // 냄비는 내려놓을 수 없음
    }

    public bool Place(IInteractable item)
    {
        if (item == null || item == this || currentItem != null || placePoint == null)
        {
            return false;
        }

        IngredientInteractable ingredient = item as IngredientInteractable;
        if (ingredient == null || !ingredient.isProcessed || ingredient.isCooked || ingredient.isBurnt)
        {
            // 손질된 재료만 조리 가능, 이미 조리/탄 재료는 거부
            return false;
        }

        item.PickUp(placePoint);
        currentItem = item;
        StartCooking();
        SetProgressVisible(true);
        return true;
    }

    public IInteractable TakeItem(Transform holdPoint)
    {
        if (GetCurrentItem() == null || holdPoint == null || isCooking)
        {
            return null;
        }

        IInteractable item = currentItem;
        item.PickUp(holdPoint);
        currentItem = null;
        StopAll();
        SetProgressVisible(false);
        return item;
    }

    private void StartCooking()
    {
        isCooking = true;
        isCookedReady = false;
        cookTimer = 0f;
        burnTimer = 0f;
        UpdateProgressBar(0f);
    }

    private void CompleteCooking()
    {
        isCooking = false;
        isCookedReady = true;
        cookTimer = 0f;
        burnTimer = 0f;

        IngredientInteractable ingredient = currentItem as IngredientInteractable;
        if (ingredient == null || ingredient.cookedPrefab == null || placePoint == null)
        {
            StopAll();
            SetProgressVisible(false);
            return;
        }

        int sourceLayer = ingredient.Root.gameObject.layer;
        Destroy(ingredient.Root.gameObject);
        currentItem = null;

        GameObject newItem = Instantiate(ingredient.cookedPrefab, placePoint, false);
        newItem.transform.localPosition = Vector3.zero;
        newItem.transform.localRotation = Quaternion.identity;
        SetLayerRecursively(newItem, sourceLayer);

        IngredientInteractable newIngredient = newItem.GetComponent<IngredientInteractable>();
        if (newIngredient != null)
        {
            newIngredient.isProcessed = true;
            newIngredient.isCooked = true;
            newIngredient.isBurnt = false;
        }

        IInteractable interactable = newItem.GetComponent<IInteractable>();
        if (interactable == null)
        {
            interactable = newItem.GetComponentInChildren<IInteractable>();
        }
        if (interactable != null)
        {
            interactable.PickUp(placePoint);
            currentItem = interactable;
        }

        UpdateProgressBar(1f);
        SetProgressVisible(true);
    }

    private void BurnFood()
    {
        isCookedReady = false;
        burnTimer = 0f;

        IngredientInteractable ingredient = currentItem as IngredientInteractable;
        if (ingredient == null || ingredient.burntPrefab == null || placePoint == null)
        {
            StopAll();
            SetProgressVisible(false);
            return;
        }

        int sourceLayer = ingredient.Root.gameObject.layer;
        Destroy(ingredient.Root.gameObject);
        currentItem = null;

        GameObject newItem = Instantiate(ingredient.burntPrefab, placePoint, false);
        newItem.transform.localPosition = Vector3.zero;
        newItem.transform.localRotation = Quaternion.identity;
        SetLayerRecursively(newItem, sourceLayer);

        IngredientInteractable newIngredient = newItem.GetComponent<IngredientInteractable>();
        if (newIngredient != null)
        {
            newIngredient.isProcessed = true;
            newIngredient.isCooked = true;
            newIngredient.isBurnt = true;
        }

        IInteractable interactable = newItem.GetComponent<IInteractable>();
        if (interactable == null)
        {
            interactable = newItem.GetComponentInChildren<IInteractable>();
        }
        if (interactable != null)
        {
            interactable.PickUp(placePoint);
            currentItem = interactable;
        }

        UpdateProgressBar(1f);
        SetProgressVisible(true);
    }

    private void StopAll()
    {
        isCooking = false;
        isCookedReady = false;
        cookTimer = 0f;
        burnTimer = 0f;
        UpdateProgressBar(0f);
    }

    private void SetProgressVisible(bool visible)
    {
        if (progressBar == null)
        {
            return;
        }

        progressBar.gameObject.SetActive(visible);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null)
        {
            return;
        }

        obj.layer = layer;
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            SetLayerRecursively(obj.transform.GetChild(i).gameObject, layer);
        }
    }

    private IInteractable GetCurrentItem()
    {
        if (currentItem == null && placePoint != null)
        {
            currentItem = placePoint.GetComponentInChildren<IInteractable>();
        }

        return currentItem;
    }

    private void UpdateProgressBar(float normalized)
    {
        if (progressBar == null)
        {
            return;
        }

        progressBar.fillAmount = Mathf.Clamp01(normalized);
    }
}


