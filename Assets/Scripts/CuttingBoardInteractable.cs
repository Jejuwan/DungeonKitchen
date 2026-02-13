using UnityEngine;
using UnityEngine.UI;

public class CuttingBoardInteractable : MonoBehaviour, IInteractable
{
    public Transform placePoint;
    public float processDuration = 2f;
    public Image progressBar;

    public Transform Root => transform;
    public bool CanPickUp => GetCurrentItem() != null && !isProcessing;
    public bool CanPlace => true;
    public bool IsProcessing => isProcessing;

    private IInteractable currentItem;
    private bool isProcessing;
    private float processTimer;
    private bool isProcessedReady;

    private void Update()
    {
        if (!isProcessing)
        {
            return;
        }

        if (currentItem == null)
        {
            StopProcess();
            ResetProgress();
            SetProgressVisible(false);
            return;
        }

        processTimer += Time.deltaTime;
        UpdateProgressBar(processTimer / Mathf.Max(processDuration, 0.01f));

        if (processTimer >= processDuration)
        {
            CompleteProcessing();
        }
    }

    public bool StartProcess()
    {
        if (GetCurrentItem() == null || isProcessing || isProcessedReady)
        {
            return false;
        }

        isProcessing = true;
        return true;
    }

    public void StopProcess()
    {
        isProcessing = false;
    }

    public void PickUp(Transform holdPoint)
    {
        TakeItem(holdPoint);
    }

    public void Drop(Vector3 dropPosition)
    {
        // 도마는 내려놓을 수 없음
    }

    public bool Place(IInteractable item)
    {
        if (item == null || item == this || currentItem != null || placePoint == null)
        {
            return false;
        }

        IngredientInteractable ingredient = item as IngredientInteractable;
        if (ingredient != null && ingredient.isProcessed)
        {
            // 이미 손질된 재료는 도마에 올릴 수 없음
            return false;
        }

        item.PickUp(placePoint);
        currentItem = item;
        ResetProgress();
        isProcessedReady = false;
        SetProgressVisible(true);
        return true;
    }

    public IInteractable TakeItem(Transform holdPoint)
    {
        if (GetCurrentItem() == null || holdPoint == null || isProcessing)
        {
            return null;
        }

        IInteractable item = currentItem;
        item.PickUp(holdPoint);
        currentItem = null;
        ResetProgress();
        isProcessedReady = false;
        SetProgressVisible(false);
        return item;
    }

    private void CompleteProcessing()
    {
        isProcessing = false;
        processTimer = 0f;

        if (placePoint == null || GetCurrentItem() == null)
        {
            UpdateProgressBar(0f);
            SetProgressVisible(false);
            return;
        }

        IProcessable processable = currentItem as IProcessable;
        if (processable == null || processable.ProcessedPrefab == null)
        {
            UpdateProgressBar(0f);
            SetProgressVisible(false);
            return;
        }

        int sourceLayer = currentItem.Root.gameObject.layer;
        Destroy(currentItem.Root.gameObject);
        currentItem = null;

        GameObject newItem = Instantiate(processable.ProcessedPrefab, placePoint, false);
        newItem.transform.localPosition = Vector3.zero;
        newItem.transform.localRotation = Quaternion.identity;
        SetLayerRecursively(newItem, sourceLayer);
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

        IngredientInteractable newIngredient = newItem.GetComponent<IngredientInteractable>();
        if (newIngredient != null)
        {
            newIngredient.isProcessed = true;
        }

        isProcessedReady = true;
        UpdateProgressBar(1f);
        SetProgressVisible(true);
    }

    private void ResetProgress()
    {
        processTimer = 0f;
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

