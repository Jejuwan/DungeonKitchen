using UnityEngine;
using UnityEngine.UI;

public class CuttingBoardProgressBarVisibility : MonoBehaviour
{
    [SerializeField] private Transform placePoint;
    [SerializeField] private Image progressBar;

    private bool lastVisible;

    private void Awake()
    {
        UpdateVisibility(true);
    }

    private void Update()
    {
        UpdateVisibility(false);
    }

    private void UpdateVisibility(bool force)
    {
        if (placePoint == null || progressBar == null)
        {
            return;
        }

        bool visible = placePoint.childCount > 0;
        if (force || visible != lastVisible)
        {
            progressBar.gameObject.SetActive(visible);
            if (!visible)
            {
                progressBar.fillAmount = 0f;
            }
            lastVisible = visible;
        }
    }
}





